using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.Serialization;

namespace ServiceLibrary.Contracts.Data
{
    [DataContract]
    public class TimeoutFault
    {
        [DataMember]
        public string Message { get; set; }
    }
}
