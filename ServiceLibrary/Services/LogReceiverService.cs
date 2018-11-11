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
    
    public class LogReceiverService : NLog.LogReceiverService.ILogReceiverServer, ILogService, ILogServiceAsync
    {
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();


        public Task ReceiveLogA()
        {
            throw new NotImplementedException();
        }

        public void ReceiveLogS()
        {
            throw new NotImplementedException();
        }


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
