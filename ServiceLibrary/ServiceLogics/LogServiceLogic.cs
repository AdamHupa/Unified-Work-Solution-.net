using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ServiceLibrary.DbModels.Log; // for: LogDbContextExtensions
using ServiceLibrary.DbModels.Log.CodeFirst;
using System.Data.Entity;

namespace ServiceLibrary.ServiceLogics
{
    public class LogServiceLogic : ILogServiceLogic
    {
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private DependencyInjection.IInstanceProvider<DbModels.Log.LogDbContext> _contextProvider;


        public LogServiceLogic(DependencyInjection.IInstanceProvider<DbModels.Log.LogDbContext> contextProvider)
        {
            _contextProvider = contextProvider;
        }


        #region Asynchronous Members

        public async Task ReceiveLogA(DbModels.Log.EventLog eventLog)
        {
            if (eventLog != null && eventLog.IsValid())
            {
                using (var context = _contextProvider.GetInstance())
                {
                    await context.InsertLogEntryAsync(eventLog);
                }
            }
        }

        #endregion // Asynchronous Members

        #region Synchronous Members

        public void ReceiveLogS(DbModels.Log.EventLog eventLog)
        {
            if (eventLog != null && eventLog.IsValid())
            {
                using (var context = _contextProvider.GetInstance())
                {
                    context.InsertLogEntry(eventLog);
                }
            }
        }

        #endregion // Synchronous Members
    }
}
