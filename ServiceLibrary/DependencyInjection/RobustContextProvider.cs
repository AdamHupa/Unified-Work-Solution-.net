using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.Entity.Infrastructure;

namespace ServiceLibrary.DependencyInjection
{
    public class RobustContextProvider<Context> : IInstanceProvider<Context>
        where Context : System.Data.Entity.DbContext, new()
    {
        private Func<IDbConnectionFactory> _connectionFactory = null;
        private Func<IDbConnectionFactory, string, Context> _contextFactory = null;
        private Func<string> _nameOrConnectionString = null;


        /// <exception cref="ArgumentNullException">thrown, if contextFactory is null.</exception>
        public RobustContextProvider(Func<IDbConnectionFactory, string, Context> contextFactory)
        {
            if (contextFactory == null)
                throw new ArgumentNullException("contextFactory"); // nameof()

            _contextFactory = contextFactory;
        }

        /// <exception cref="ArgumentNullException">thrown, if contextFactory is null.</exception>
        public RobustContextProvider(Func<IDbConnectionFactory, string, Context> contextFactory,
                                     Func<string> nameOrConnectionString)
        {
            if (contextFactory == null)
                throw new ArgumentNullException("contextFactory"); // nameof()

            _contextFactory = contextFactory;
            _nameOrConnectionString = nameOrConnectionString;
        }

        /// <exception cref="ArgumentNullException">thrown, if contextFactory is null.</exception>
        public RobustContextProvider(Func<IDbConnectionFactory, string, Context> contextFactory,
                                     Func<IDbConnectionFactory> connectionFactory)
        {
            if (contextFactory == null)
                throw new ArgumentNullException("contextFactory"); // nameof()

            _connectionFactory = connectionFactory;
            _contextFactory = contextFactory;
        }

        /// <exception cref="ArgumentNullException">thrown, if contextFactory is null.</exception>
        public RobustContextProvider(Func<IDbConnectionFactory, string, Context> contextFactory,
                                     Func<IDbConnectionFactory> connectionFactory,
                                     Func<string> nameOrConnectionString)
        {
            if (contextFactory == null)
                throw new ArgumentNullException("contextFactory"); // nameof()

            _connectionFactory = connectionFactory;
            _contextFactory = contextFactory;
            _nameOrConnectionString = nameOrConnectionString;
        }


        internal Func<IDbConnectionFactory> ConnectionFactory
        {
            get { return _connectionFactory; }
            set { _connectionFactory = value; }
        }

        internal Func<string> NameOrConnectionString
        {
            get { return _nameOrConnectionString; }
            set { _nameOrConnectionString = value; }
        }


        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public Context GetInstance()
        {
            if (_connectionFactory == null)
                return _contextFactory(null, (_nameOrConnectionString == null) ? null : _nameOrConnectionString());
            else
                return _contextFactory(_connectionFactory(), (_nameOrConnectionString == null) ? null : _nameOrConnectionString());
        }
    }
}
