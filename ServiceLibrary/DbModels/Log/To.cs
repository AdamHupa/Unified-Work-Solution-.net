using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLibrary.DbModels.Log
{
    public static class To
    {
        public static EventLog EventLog(DateTime timeStamp, NLogLevel level, string logger, /*string sequenceId,*/
                                        string address, string machineName, string windowsId,
                                        string callerSide, int lineNumber,
                                        int? threadId, string stackTrace,
                                        string message, string exception, string jsonObject)
        {
            return new EventLog()
            {
                TimeStamp = timeStamp,
                Level = level,
                Logger = logger,
                //SequenceId = sequenceId,
                Address = address,
                MachineName = machineName,
                WindowsId = windowsId,
                CallerSide = callerSide,
                LineNumber = lineNumber,
                ThreadId = threadId,
                StackTrace = stackTrace,
                Message = message,
                Exception = exception,
                Json = jsonObject
            };
        }

        public static EventLog EventLog(string timeStamp, string level, string logger, /*string sequenceId,*/
                                        string address, string machineName, string windowsId,
                                        string callerSide, string lineNumber,
                                        string threadId, string stackTrace,
                                        string message, string exception, string json)
        {
            DateTime deserializedTimeStamp;
            NLogLevel deserializedLevel;
            int deserializedLineNumber;
            int deserializedThreadId;

            if (!DateTime.TryParse(timeStamp, out deserializedTimeStamp) ||
                !Enum.TryParse<NLogLevel>(level, out deserializedLevel) ||
                !Int32.TryParse(lineNumber, out deserializedLineNumber) ||
                !Int32.TryParse(threadId, out deserializedThreadId))
                return null;

            return new EventLog()
            {
                TimeStamp = deserializedTimeStamp,
                Level = deserializedLevel,
                Logger = logger,
                //SequenceId = sequenceId,
                Address = address,
                MachineName = machineName,
                WindowsId = windowsId,
                CallerSide = callerSide,
                LineNumber = deserializedLineNumber,
                ThreadId = deserializedThreadId,
                StackTrace = stackTrace,
                Message = message,
                Exception = exception,
                Json = json
            };
        }
    }
}
