using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLibrary.DbModels.Log
{
    public static class LogExtensions // {work_namespace}Extensions
    {
        public static NLog.LogLevel ToLogLevel(this DbModels.Log.NLogLevel value)
        {
            // LogLevel <0, 6>
            return NLog.LogLevel.FromOrdinal((int)value > 6 ? 0 : (int)value); //??
        }

        public static DbModels.Log.NLogLevel ToNLogLevel(this NLog.LogLevel value)
        {
            return (DbModels.Log.NLogLevel)value.Ordinal;
        }
    }
}
