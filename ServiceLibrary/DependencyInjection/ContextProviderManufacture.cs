using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Configuration;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq.Expressions;

namespace ServiceLibrary.DependencyInjection
{
    public enum ConnectionFactory
    {
        Default = 0,  // System.Data.Entity.Database.DefaultConnectionFactory
        EffortDb,     // Effort.DbConnectionFactory
        EffortEntity, // Effort.EntityConnectionFactory
        LocalDb,      // System.Data.Entity.Infrastructure.LocalDbConnectionFactory
        Sql,          // System.Data.Entity.Infrastructure.SqlConnectionFactory
        SqlCe         // System.Data.Entity.Infrastructure.SqlCeConnectionFactory
    }


    /// <remarks>Manufacture Pattern*</remarks>
    public class ContextProviderManufacture
    {
        private ContextProviderCache<ICloneable> _contextProviderPrototypes = new ContextProviderCache<ICloneable>();

        protected static Lazy<string> _localDbVersion = new Lazy<string>(() => RetrieveLocalDbVersion());


        public ContextProviderCache<ICloneable> ContextProviderPrototypes
        {
            get { return _contextProviderPrototypes; }
            set
            {
                if (value != null)
                    _contextProviderPrototypes = value;
                else
                    _contextProviderPrototypes = new ContextProviderCache<ICloneable>();
            }
        }

        public static IDbConnectionFactory DefaultConnectionFactory
        {
#pragma warning disable 0618
            get
            {
                return System.Data.Entity.Database.DefaultConnectionFactory;
            }
            protected set
            {
                System.Data.Entity.Database.DefaultConnectionFactory = value;
            }
#pragma warning restore 0618
        }

        public static string LocalDbVersion
        {
            get { return _localDbVersion.Value; }
        }


        #region Create Methods

        /// <remarks>
        /// Designed to create simple DbContext instance providers with a fixed default connection string.
        /// e.g.: Create(_ => new DbModels.Log.LogDbContext("RelativeDefaultConnectionString"))
        /// </remarks>
        public static IInstanceProvider<Context> Create<Context>(Func<string, Context> contextFactory)
            where Context : System.Data.Entity.DbContext, new()
        {
            return new CompactContextProvider<Context>(contextFactory);
        }

        public static IInstanceProvider<Context> Create<Context>(Func<string, Context> contextFactory, string nameOrConnectionString)
            where Context : System.Data.Entity.DbContext, new()
        {
            return new CompactContextProvider<Context>(contextFactory, nameOrConnectionString);
        }

        /// <summary>
        /// Creates an instance of context provider that implements the IInstanceProvider interface.
        /// </summary>
        /// <typeparam name="Context">A class that extends the System.Data.Entity.DbContext class.</typeparam>
        /// <param name="contextFactory">The method creates the database Context.</param>
        /// <param name="connectionFactory">The method returns the applicable Connection Factory.</param>
        /// <param name="databaseNameOrConnectionString">Method returns a connection string or database file name.</param>
        /// <returns>Returns an instance that implements the IInstanceProvider interface.</returns>
        public static IInstanceProvider<Context> Create<Context>(
            Func<IDbConnectionFactory, string, Context> contextFactory, Func<IDbConnectionFactory> connectionFactory, Func<string> databaseNameOrConnectionString)
            where Context : System.Data.Entity.DbContext, new()
        {
            return new RobustContextProvider<Context>(contextFactory, connectionFactory, databaseNameOrConnectionString);
        }

        #endregion // Create Methods

        /// <summary>
        /// Manufacture an instance of ContextProvider from a clone of a prototype object.
        /// </summary>
        /// <param name="prototypeIdentifier">Prototype identifier in the registry.</param>
        /// <returns>Returns the unchanged prototype clone, otherwise null.</returns>
        public object Manufacture(string prototypeIdentifier)
        {
            if (!_contextProviderPrototypes.ContainsKey(prototypeIdentifier))
                return null;

            return _contextProviderPrototypes[prototypeIdentifier].Clone();
        }

        /// <summary>
        /// Manufacture a modified instance of ContextProvider from a clone of a prototype object.
        /// </summary>
        /// <typeparam name="ContextProviderType">Class implementing DependencyInjection.IInstanceProvider interface.</typeparam>
        /// <typeparam name="ContextType">A class that extends the System.Data.Entity.DbContext class.</typeparam>
        /// <param name="prototypeIdentifier">Prototype identifier in the registry.</param>
        /// <param name="state">ContextProvider state modification object.</param>
        /// <returns>Returns the modified prototype clone, otherwise null.</returns>
        public ContextProviderType Manufacture<ContextProviderType, ContextType>(
            string prototypeIdentifier, IContextProviderState<ContextProviderType, ContextType> state)
            where ContextProviderType : class, IInstanceProvider<ContextType>
            where ContextType : System.Data.Entity.DbContext, new()
        {
            if (!_contextProviderPrototypes.ContainsKey(prototypeIdentifier))
                return null;

            var contextProvider = _contextProviderPrototypes[prototypeIdentifier].Clone() as ContextProviderType;
            if (state != null)
                state.Handle(contextProvider);

            return contextProvider;
        }

        /// <summary>
        /// Manufacture a modified instance of ContextProvider from a clone of a prototype object.
        /// </summary>
        /// <typeparam name="ContextProviderType">Class implementing DependencyInjection.IInstanceProvider interface.</typeparam>
        /// <typeparam name="ContextType">A class that extends the System.Data.Entity.DbContext class.</typeparam>
        /// <param name="prototypeIdentifier">Prototype identifier in the registry.</param>
        /// <param name="action">ContextProvider state modification delegate.</param>
        /// <returns>Returns the modified prototype clone, otherwise null.</returns>
        public ContextProviderType Manufacture<ContextProviderType, ContextType>(
            string prototypeIdentifier, Action<ContextProviderType> action)
            where ContextProviderType : class, IInstanceProvider<ContextType>
            where ContextType : System.Data.Entity.DbContext, new()
        {
            if (!_contextProviderPrototypes.ContainsKey(prototypeIdentifier))
                return null;

            var contextProvider = _contextProviderPrototypes[prototypeIdentifier].Clone() as ContextProviderType;
            if (action != null)
                action(contextProvider);

            return contextProvider;
        }

        /// <summary>
        /// Retrieves Sql LocalDb version by using the SqlLocalDB Utility.
        /// </summary>
        /// <returns>Returns Sql LocalDb version or null.</returns>
        /// <remarks>
        /// The implementation assumes that under the default identifications (mssqllocaldb, v11.0 ect.) there are no custom LocalDb.
        /// </remarks>
        public static string RetrieveLocalDbVersion()
        {
            List<string> identificatorList = ShellCommand.GetSqlLocalDbInstances();

            if (identificatorList.Contains("mssqllocaldb"))
                return "mssqllocaldb";

            var regex = new System.Text.RegularExpressions.Regex(@"\A[vV]1?\d.\d\z");

            identificatorList = identificatorList.FindAll(s => regex.IsMatch(s));
            if (identificatorList.Count > 1)
                identificatorList.Sort();

            return identificatorList.FirstOrDefault();
        }

        /// <returns>The default implementation return an instance of System.Data.Entity.Infrastructure.LocalDbConnectionFactory.</returns>
        protected virtual IDbConnectionFactory CreateDefaultConnectionFactory(string localDbVersion)
        {
            return new System.Data.Entity.Infrastructure.LocalDbConnectionFactory(localDbVersion);
        }
    }
}
