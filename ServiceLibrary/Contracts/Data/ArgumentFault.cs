using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.Serialization;

namespace ServiceLibrary.Contracts.Data
{
    [DataContract]
    public class ArgumentFault
    {
        [DataMember]
        public string Message { get; set; }

        //[System.ComponentModel.DataAnnotations.Required]
        [DataMember]
        public string ParameterName { get; set; }
    }
}
