using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ServiceModel;

namespace ServiceLibrary.Contracts
{
    [ServiceContract(Namespace = Namespaces.Logger)] // Name = "ILogService",
    public interface ILogServiceAsync
    {
        [OperationContract]
        //[FaultContract(typeof(Contracts.Data.ServiceFault))]
        Task ReceiveLogA();
    }
}
