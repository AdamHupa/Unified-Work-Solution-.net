using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//using System.Runtime.Caching; // for: MemoryCache

namespace ServiceLibrary.DependencyInjection
{
    /// <summary>
    /// Non expiring object cache.
    /// </summary>
    /// <remarks>
    /// Collection Initializer requires a class implementing System.Collections.IEnumerable and containing the Add method with matching parameters.
    /// </remarks>
    [System.Diagnostics.DebuggerDisplay("Count = {_cache.Count}")]
    public class ContextProviderCache<TValue> : System.Collections.IEnumerable
    {
        //private MemoryCache _cache = new MemoryCache("");
        private System.Collections.Concurrent.ConcurrentDictionary<string, TValue> _cache =
            new System.Collections.Concurrent.ConcurrentDictionary<string, TValue>();


        /// <exception cref="ArgumentNullException">thrown, if key is null.</exception>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">
        /// thrown, if the property is retrieved and key does not exist in the collection.
        /// </exception>
        public TValue this[string key]
        {
            get { return _cache[key]; }
            private set { _cache[key] = value; }
        }


        public bool Add(string key, TValue value)
        {
            if (key == null || _cache.ContainsKey(key))
                return false;

            _cache[key] = value;
            return true;
        }

        public void Clear()
        {
            _cache.Clear();
        }

        /// <exception cref="ArgumentNullException">thrown, if key is null.</exception>
        public bool ContainsKey(string key)
        {
            return _cache.ContainsKey(key);
        }

        /// <exception cref="ArgumentNullException">thrown, if key is null.</exception>
        public TValue Remove(string key)
        {
            TValue value;
            return _cache.TryRemove(key, out value) ? value : default(TValue);
        }

        #region System.Collections.IEnumerator Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion // System.Collections.IEnumerator Members
    }
}
