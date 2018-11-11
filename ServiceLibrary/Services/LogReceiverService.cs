using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ServiceLibrary.Contracts;
using System.ServiceModel;
using System.Xml.Linq;

namespace ServiceLibrary.Services
{
    public class LogReceiverService : ILogService, ILogServiceAsync
    {
        public Task ReceiveLogA()
        {
            throw new NotImplementedException();
        }

        public void ReceiveLogS()
        {
            throw new NotImplementedException();
        }
    }
}
