using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.Serialization;

namespace ServiceLibrary.Contracts.Data
{
    [DataContract]
    public class ServiceFault
    {
        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public bool IsCritical { get; set; }
    }
}
