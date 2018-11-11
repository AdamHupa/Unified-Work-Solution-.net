using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ServiceModel;

namespace ServiceLibrary.Contracts
{
    [ServiceContract(Namespace = Namespaces.Logger)]
    public interface ILogService
    {
        [OperationContract]
        //[FaultContract(typeof(Contracts.Data.ServiceFault))]
        void ReceiveLogS();
    }
}
