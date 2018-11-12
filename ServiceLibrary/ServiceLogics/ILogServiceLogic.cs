using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLibrary.ServiceLogics
{
    public interface ILogServiceLogic //: Contracts.ILogService, Contracts.ILogServiceAsync
    {
        Task ReceiveLogA(DbModels.Log.EventLog eventLog);

        void ReceiveLogS(DbModels.Log.EventLog eventLog);
    }
}
