using System.Data.Common;
using System.Data.Entity.Infrastructure;

namespace ServiceLibrary.UnitTests.Tools
{
    public class EffortProviderFactory : IDbConnectionFactory
    {
        private static readonly object _lock = new object();
        private static DbConnection _connection;


        public static void ResetDb()
        {
            lock (_lock)
            {
                _connection = null;
            }
        }

        #region IDbConnectionFactory Members

        public DbConnection CreateConnection(string nameOrConnectionString)
        {
            lock (_lock)
            {
                if (_connection == null)
                {
                    _connection = Effort.DbConnectionFactory.CreateTransient();
                }

                return _connection;
            }
        }

        #endregion // IDbConnectionFactory Members
    }
}
