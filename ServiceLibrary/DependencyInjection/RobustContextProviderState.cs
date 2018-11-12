using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.Entity.Infrastructure;

namespace ServiceLibrary.DependencyInjection
{
    public abstract class RobustContextProviderState<Context> : IContextProviderState<RobustContextProvider<Context>, Context>
        where Context : System.Data.Entity.DbContext, new()
    {
        protected Func<IDbConnectionFactory> _connectionFactory = null;
        protected Func<string> _databaseNameOrConnectionString = null;
        protected bool _isConnectionFactorySet = false;
        protected bool _isStringFactorySet = false;


        public Func<IDbConnectionFactory> ConnectionFactory
        {
            get { return _connectionFactory; }
            set
            {
                _isConnectionFactorySet = true;
                _connectionFactory = value;
            }
        }

        public Func<string> DatabaseNameOrConnectionString
        {
            get { return _databaseNameOrConnectionString; }
            set
            {
                _isStringFactorySet = true;
                _databaseNameOrConnectionString = value;
            }
        }


        public void Handle(RobustContextProvider<Context> context)
        {
            if (this._isConnectionFactorySet)
                context.ConnectionFactory = this._connectionFactory;

            if (this._isStringFactorySet)
                context.NameOrConnectionString = this._databaseNameOrConnectionString;
        }
    }
}
