using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

// link: https://blogs.msdn.microsoft.com/pedram/2007/10/07/a-performance-comparison-of-readerwriterlockslim-with-readerwriterlock/

/*
    This class is an example of Rube Goldberg machine, an potentially over-complicated solution to a simple problem.
    
    Use with care!
 */

namespace ServiceLibrary.Tools
{
    /// <summary>
    /// Memory cache registry with entry expiration and re-creation live cycle.
    /// </summary>
    /// <remarks>
    /// It is intended for one-time initiation and for long-term operation.
    /// The entry live cycle handling logic is implemented using
    /// the <typeparamref name="Dictionary"/> and <typeparamref name="ReaderWriterLockSlim"/> lock,
    /// instead of <typeparamref name="ConcurrentDictionary"/> which internally uses simple locks.
    /// </remarks>
    public sealed class CyclicCacheRegistry : IEnumerable<KeyValuePair<string, object>>, IDisposable
    {
        public const string DefaultCacheRegistryName = "CyclicCacheRegistry_MemoryCache_Default";

        public static readonly CyclicCacheRegistry Default = new CyclicCacheRegistry();


        private ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
        private Dictionary<string, CacheRegistryLogic> _logicCache = new Dictionary<string, CacheRegistryLogic>();
        private string _name;
        private System.Runtime.Caching.MemoryCache _objectCache = null;


        /// <exception cref="ArgumentNullException">thrown, if <paramref name="memoryCacheName"/> is null.</exception>
        /// <exception cref="ArgumentException">
        /// thrown, if <paramref name="memoryCacheName"/> is an empty string or its value is equal to
        /// either the value of the public fieled DefaultCacheRegistryName
        /// or "default" (case insensitive) - a reserved name for the default instance of MemoryCache class.
        /// </exception>
        public CyclicCacheRegistry(string memoryCacheName)
        {
            if (String.Compare(DefaultCacheRegistryName, memoryCacheName) == 0)
                throw new ArgumentException("memoryCacheName");

            _objectCache = new System.Runtime.Caching.MemoryCache(memoryCacheName); // throws
            _name = memoryCacheName;
        }


        private CyclicCacheRegistry()
        {
            _objectCache = new System.Runtime.Caching.MemoryCache(DefaultCacheRegistryName);
            _name = DefaultCacheRegistryName;
        }


        public int Count
        {
            get
            {
                _lock.EnterReadLock();

                int count = _logicCache.Count;

                _lock.ExitReadLock();
                return count;
            }
        }


        /// <exception cref="ArgumentNullException">thrown, if <paramref name="key"/> is null.</exception>
        public object this[string key]
        {
            get { return this.GetObject(key); }
        }


        /// <summary>
        /// Add entry to cache registry.
        /// </summary>
        /// <param name="key">Unique identificator.</param>
        /// <param name="expiration">Expiration period for each instance of an entry.</param>
        /// <param name="objectRenewalLogic">
        /// The delegate used to create the object after each expiration.
        /// If delegate execution results with null, a <typeparamref name="ArgumentException"/> will be thrown.
        /// </param>
        /// <param name="createOnAdd">
        /// Flag determining if the instance should be created upon registration. Default value: true
        /// </param>
        /// <returns>true, if the registration and optional instance creation succeeded.</returns>
        /// <exception cref="ArgumentNullException">
        /// thrown, if <paramref name="objectRenewalLogic"/> or <paramref name="key"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// thrown, if <paramref name="expiration"/> period is negative or <paramref name="objectRenewalLogic"/> delegate execution results with null.
        /// </exception>
        public bool Add(string key, TimeSpan expiration, Func<object> objectRenewalLogic, bool createOnAdd = true)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            CacheRegistryLogic logic = CacheRegistryLogic.Create(expiration, objectRenewalLogic); // throws

            bool shouldThrow = false;
            bool result = false;


            _lock.EnterReadLock();
            try
            {
                result = _logicCache.ContainsKey(key);
            }
            catch { }
            finally
            {
                _lock.ExitReadLock();
            }

            if (result == true)
                return false;


            _lock.EnterUpgradeableReadLock();
            try
            {
                if (!_logicCache.ContainsKey(key))
                {
                    _lock.EnterWriteLock(); // registry, write lock
                    try
                    {
                        if (createOnAdd)
                        {
                            object item = logic.ObjectRenewalLogic();

                            if (item == null)
                            {
                                shouldThrow = true;
                            }
                            else
                            {
                                System.Runtime.Caching.CacheItemPolicy cacheItemPolicy = new System.Runtime.Caching.CacheItemPolicy()
                                {
                                    AbsoluteExpiration = DateTimeOffset.UtcNow.Add(logic.Expiration),
                                };

                                _objectCache.Set(key, item, cacheItemPolicy);
                                _logicCache.Add(key, logic);
                            }
                        }
                        else
                        {
                            _logicCache.Add(key, logic);
                        }

                        result = true;
                    }
                    catch { }
                    finally
                    {
                        _lock.ExitWriteLock(); // registry, write unlock
                    }
                }
            }
            catch { }
            finally
            {
                _lock.ExitUpgradeableReadLock();
            }

            if (shouldThrow)
                throw new ArgumentException("objectRenewalLogic");

            return result;
        }

        public void Clear()
        {
            _lock.EnterWriteLock(); // registry, write lock
            try
            {
                foreach (var logic in _logicCache.Values)
                {
                    if (logic != null)
                        logic.Dispose();
                }
                _logicCache.Clear();

                foreach (string key in _objectCache.Select(kvp => kvp.Key).ToList())
                {
                    _objectCache.Remove(key);
                }
            }
            catch { }
            finally
            {
                _lock.ExitWriteLock(); // registry, write lock
            }
        }

        /// <exception cref="ArgumentNullException">thrown, if <paramref name="key"/> is null.</exception>
        public bool ContainsKey(string key)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            bool result = false;


            _lock.EnterReadLock();
            try
            {
                result = _logicCache.ContainsKey(key);
            }
            catch { }
            finally
            {
                _lock.ExitReadLock();
            }

            return result;
        }

        /// <exception cref="ArgumentNullException">thrown, if <paramref name="key"/> is null.</exception>
        public bool ContainsObject(string key)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            CacheRegistryLogic logic;
            bool result = false;


            _lock.EnterReadLock();
            try
            {
                if (_logicCache.TryGetValue(key, out logic))
                {
                    logic.Lock.EnterReadLock(); // entry, read lock
                    try
                    {
                        result = _objectCache.Contains(key);
                    }
                    catch { }
                    finally
                    {
                        logic.Lock.ExitReadLock(); // entry, read unlock
                    }
                }
            }
            catch { }
            finally
            {
                _lock.ExitReadLock();
            }

            return result;
        }

        /// <summary>
        /// Forcefully re-creates the object under the given unique key value using object renewal logic.
        /// The method is not optimised for cases when executing the re-creation logic will result with null.
        /// </summary>
        /// <param name="key">Unique identificator.</param>
        /// <returns>Returns the instance, if failed null.</returns>
        /// <exception cref="ArgumentNullException">thrown, if <paramref name="key"/> is null.</exception>
        public object ForceRenewal(string key)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            CacheRegistryLogic logic;
            object item = null;

            _lock.EnterUpgradeableReadLock();
            try
            {
                if (_logicCache.TryGetValue(key, out logic))
                {
                    logic.Lock.EnterUpgradeableReadLock(); // entry, update lock
                    try
                    {
                        item = _objectCache.Get(key);

                        if (item == null)
                        {
                            item = logic.ObjectRenewalLogic();

                            if (item != null)
                            {
                                logic.Lock.EnterWriteLock(); // entry, write lock
                                try
                                {
                                    System.Runtime.Caching.CacheItemPolicy cacheItemPolicy = new System.Runtime.Caching.CacheItemPolicy()
                                    {
                                        AbsoluteExpiration = DateTimeOffset.UtcNow.Add(logic.Expiration),
                                    };

                                    _objectCache.Set(key, item, cacheItemPolicy);
                                }
                                catch { }
                                finally
                                {
                                    logic.Lock.ExitWriteLock(); // entry, write unlock
                                }
                            }
                        }
                    }
                    catch { }
                    finally
                    {
                        logic.Lock.ExitUpgradeableReadLock(); // entry, update unlock
                    }
                }
            }
            catch { }
            finally
            {
                _lock.ExitUpgradeableReadLock();
            }

            return item;
        }

        /// <summary>
        /// Retrieve the object under the given unique key value.
        /// If the entry instance has already expired, it will re-create it form the object renewal logic.
        /// The method is not optimised for cases when executing the re-creation logic will result with null.
        /// </summary>
        /// <param name="key">Unique identificator.</param>
        /// <returns>Returns the instance, if failed null.</returns>
        /// <exception cref="ArgumentNullException">thrown, if <paramref name="key"/> is null.</exception>
        public object GetObject(string key)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            CacheRegistryLogic logic = null;
            object item = null;


            _lock.EnterReadLock();
            try
            {
                if (_logicCache.TryGetValue(key, out logic))
                {
                    logic.Lock.EnterReadLock(); // entry, read lock
                    try
                    {
                        item = _objectCache.Get(key);
                    }
                    catch { }
                    finally
                    {
                        logic.Lock.ExitReadLock(); // entry, read unlock
                    }
                }
            }
            catch { }
            finally
            {
                _lock.ExitReadLock();
            }

            if (logic == null)
                return null;

            return (item != null) ? item : ForceRenewal(key);
        }

        /// <summary>
        /// Retrieve the delegate implementing object renewal logic for a given unique key.
        /// </summary>
        /// <param name="key">Unique identificator.</param>
        /// <returns>Returns the instance, if failed null.</returns>
        /// <exception cref="ArgumentNullException">thrown, if <paramref name="key"/> is null.</exception>
        public Func<object> GetRenewalLogic(string key)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            CacheRegistryLogic logic = null;


            _lock.EnterReadLock();
            try
            {
                if (!_logicCache.TryGetValue(key, out logic))
                    logic = null;
            }
            catch { }
            finally
            {
                _lock.ExitReadLock();
            }

            return (logic == null) ? null : logic.ObjectRenewalLogic;
        }

        /// <exception cref="ArgumentNullException">thrown, if <paramref name="key"/> is null.</exception>
        public bool Remove(string key)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            bool result = false;
            IDisposable item = null;


            _lock.EnterUpgradeableReadLock();
            try
            {
                if (_logicCache.ContainsKey(key))
                {
                    _lock.EnterWriteLock(); // registry, write lock
                    try
                    {
                        if (_objectCache.Contains(key))
                        {
                            item = _objectCache.Get(key) as IDisposable;
                            if (item != null)
                                item.Dispose();

                            _objectCache.Remove(key);
                        }

                        _logicCache.Remove(key);

                        result = true;
                    }
                    finally
                    {
                        _lock.ExitWriteLock(); // registry, write unlock
                    }
                }
            }
            finally
            {
                _lock.ExitUpgradeableReadLock();
            }

            return result;
        }


        #region IDisposable Members

        void IDisposable.Dispose()
        {
            if (_lock != null)
            {
                _lock.EnterWriteLock(); // registry, write lock

                if (_logicCache != null)
                {
                    foreach (var logic in _logicCache.Values)
                    {
                        if (logic != null)
                            logic.Dispose();
                    }
                    _logicCache.Clear();
                    _logicCache = null;
                }

                if (_objectCache != null)
                {
                    _objectCache.Dispose();
                    _objectCache = null;
                }

                _lock.ExitWriteLock(); // registry, write unlock

                _lock.Dispose();
                _lock = null;
            }
        }

        #endregion // IDisposable Members

        #region IEnumerable<> Members

        IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator()
        {
            _lock.EnterReadLock();

            IEnumerator<KeyValuePair<string, object>> result = ((IEnumerable<KeyValuePair<string, object>>)_objectCache).GetEnumerator();

            _lock.ExitReadLock();
            return result;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            _lock.EnterReadLock();

            System.Collections.IEnumerator result = ((System.Collections.IEnumerable)_objectCache).GetEnumerator();

            _lock.ExitReadLock();
            return result;
        }

        #endregion // IEnumerable<> Members


        private class CacheRegistryLogic : IDisposable
        {
            public TimeSpan Expiration { get; private set; }
            public ReaderWriterLockSlim Lock { get; private set; }
            public Func<object> ObjectRenewalLogic { get; private set; }


            private CacheRegistryLogic()
            { }


            /// <exception cref="ArgumentNullException">thrown, if <paramref name="objectRenewalLogic"/> is null.</exception>
            /// <exception cref="ArgumentException">thrown, if <paramref name="expiration"/> period is negative.</exception>
            public static CacheRegistryLogic Create(TimeSpan expiration, Func<object> objectRenewalLogic)
            {
                if (objectRenewalLogic == null)
                    throw new ArgumentNullException("objectRenewalLogic");

                if (expiration < TimeSpan.Zero)
                    throw new ArgumentException("expiration");

                return new CacheRegistryLogic()
                {
                    Expiration = expiration,
                    Lock = new ReaderWriterLockSlim(),
                    ObjectRenewalLogic = objectRenewalLogic,
                };
            }

            public void Dispose()
            {
                Lock.Dispose();
                Lock = null;
                ObjectRenewalLogic = null;
            }
        }
    }
}
