using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ServiceLibrary.Contracts;
using System.ServiceModel;
using System.Xml.Linq;

// link: https://github.com/jkowalski/NLog/blob/master/tests/NLogReceiverService/LogReceiverServer.cs

namespace ServiceLibrary.Services
{
    //[ServiceBehavior(Namespace = Namespaces.Logger)]
    [ServiceLibrary.DependencyInjection.ServiceBehavior]
    public class LogReceiverService : NLog.LogReceiverService.ILogReceiverServer, ILogService, ILogServiceAsync
    {
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private ServiceLogics.ILogServiceLogic _logic = null;


        //public LogReceiverService() { }

        public LogReceiverService(ServiceLogics.ILogServiceLogic serviceLogic)
        {
            if (serviceLogic == null)
                throw new ArgumentNullException("serviceLogic");
            _logic = serviceLogic;
        }


        #region ILogService & ILogServiceAsync Members

        public async Task ReceiveLogA(DbModels.Log.EventLog eventLog)
        {
            if (eventLog.Address == String.Empty)
                eventLog.Address = Tools.ClientAddress.DeducedClientAddress();
            else
                eventLog.Address = String.Format("{0}; {1}", Tools.ClientAddress.DeducedClientAddress(), eventLog.Address);

            await _logic.ReceiveLogA(eventLog);
        }

        public void ReceiveLogS(DbModels.Log.EventLog eventLog)
        {
            if (eventLog.Address == String.Empty)
                eventLog.Address = Tools.ClientAddress.DeducedClientAddress();
            else
                eventLog.Address = String.Format("{0}; {1}", Tools.ClientAddress.DeducedClientAddress(), eventLog.Address);

            _logic.ReceiveLogS(eventLog);
        }

        #endregion // ILogService & ILogServiceAsync Members

        #region ILogReceiverServer Members

        /// <remarks>Designed primarily for receiving critical events sent from crashing clients.</remarks>
        public void ProcessLogMessages(NLog.LogReceiverService.NLogEvents events)
        {
            string address = Tools.ClientAddress.DeducedClientAddress();


            IList<NLog.LogEventInfo> logEventInfos = events.ToEventInfo("Client.");

            foreach (var logEventInfo in logEventInfos)
            {
                logEventInfo.Properties["Address"] = address;
                //logEventInfo.Properties["Logger"] = logEventInfo.LoggerName;

                NLog.Logger logger = NLog.LogManager.GetLogger(logEventInfo.LoggerName);
                logger.Log(logEventInfo);
            }
        }

        #endregion // ILogReceiverServer Members
    }
}
