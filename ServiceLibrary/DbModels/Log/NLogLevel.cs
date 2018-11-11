using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.Serialization;

namespace ServiceLibrary.DbModels.Log
{
    [DataContract]
    public enum NLogLevel : byte
    {
        [EnumMember]
        Trace = 0,
        [EnumMember]
        Debug = 1,
        [EnumMember]
        Info = 2,
        [EnumMember]
        Warn = 3,
        [EnumMember]
        Error = 4,
        [EnumMember]
        Fatal = 5,
        [EnumMember]
        Off = 6,
        [EnumMember]
        Custom = 7, // Custom log level, not included in NLog list.
        [EnumMember]
        Apt = 8, // Unrecognised or requiring reevaluation log level, not included in NLog list.
    }
}
