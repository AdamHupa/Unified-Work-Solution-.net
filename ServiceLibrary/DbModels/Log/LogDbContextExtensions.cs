using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using ServiceLibrary.DbModels.Log;
using ServiceLibrary.DbModels.Log.CodeFirst;

namespace ServiceLibrary.DbModels.Log
{
    internal static class LogDbContextExtensions // {target_class_name}Extensions
    {
        private const string _uspInsertEventlogQuery =
            "EXEC Log.usp_insert_eventlog_context " +
            "@time_stamp, @level, @logger, @address, @machine_name, @windows_id, @call_side, @line_number, " +
            "@message, @exception, @json_object";

        private const string _uspInsertEventlogContextQuery =
            "EXEC Log.usp_insert_eventlog_context " +
            "@time_stamp, @level, @logger, @address, @machine_name, @windows_id, @call_side, @line_number, " +
            "@thread_id, @stack_trace, @message, @exception, @json_object";


        /// <example>
        /// var result = await context.GetLogEntry(11).FirstOrDefaultAsync();
        /// </example>
        /// <remarks>Returned object due to internal structure is not reusable.</remarks>
        public static DbRawSqlQuery<EventLog> GetLogEntryQuery(this LogDbContext context, Nullable<int> id)
        {
            SqlParameter paramater = new SqlParameter("@id", SqlDbType.Int) { Value = id.Value };

            return context.Database.SqlQuery<EventLog>("SELECT * FROM Log.fn_select_eventlog_context(@id)", paramater);
        }


        public static int InsertLogEntry(this LogDbContext context, EventLog eventLog)
        {
            object[] parameters = InsertEventlogContextParameters(
                eventLog.TimeStamp, eventLog.Level, eventLog.Logger, eventLog.Address, eventLog.MachineName, eventLog.WindowsId,
                eventLog.CallerSide, eventLog.LineNumber, eventLog.ThreadId, eventLog.StackTrace,
                eventLog.Message, eventLog.Exception, eventLog.Json);

            return context.Database.ExecuteSqlCommand(_uspInsertEventlogContextQuery, parameters);
        }

        public static int InsertLogEntry(
            this LogDbContext context, DateTime timeStamp, byte level, string logger,
            string address, string machineName, string windowsId, string callerSide, int lineNumber,
            int? threadId, string stackTrace, string message, string exception, string jsonObject)
        {
            object[] parameters = InsertEventlogContextParameters(
                timeStamp, level, logger, address, machineName, windowsId, callerSide, lineNumber,
                threadId, stackTrace, message, exception, jsonObject);

            return context.Database.ExecuteSqlCommand(_uspInsertEventlogContextQuery, parameters);
        }

        public static async Task<int> InsertLogEntryAsync(this LogDbContext context, EventLog eventLog)
        {
            object[] parameters = InsertEventlogContextParameters(
                eventLog.TimeStamp, eventLog.Level, eventLog.Logger, eventLog.Address, eventLog.MachineName, eventLog.WindowsId,
                eventLog.CallerSide, eventLog.LineNumber, eventLog.ThreadId, eventLog.StackTrace,
                eventLog.Message, eventLog.Exception, eventLog.Json);

            return await context.Database.ExecuteSqlCommandAsync(_uspInsertEventlogContextQuery, parameters);
        }

        public static async Task<int> InsertLogEntryAsync(
            this LogDbContext context, DateTime timeStamp, byte level, string logger,
            string address, string machineName, string windowsId, string callerSide, int lineNumber,
            int? threadId, string stackTrace, string message, string exception, string jsonObject)
        {
            object[] parameters = InsertEventlogContextParameters(
                timeStamp, level, logger, address, machineName, windowsId, callerSide, lineNumber,
                threadId, stackTrace, message, exception, jsonObject);

            return await context.Database.ExecuteSqlCommandAsync(_uspInsertEventlogContextQuery, parameters);
        }


        private static SqlParameter[] InsertEventlogParameters(
            object timeStamp, object level, object logger,
            object address, object machineName, object windowsId, object callerSide, object lineNumber,
            object message, object exception, object jsonObject)
        {
            SqlParameter[] parameter = 
            {
                new SqlParameter("@time_stamp",   SqlDbType.DateTime2) { Value = timeStamp },
                new SqlParameter("@level",        SqlDbType.TinyInt)   { Value = level },
                new SqlParameter("@logger",       SqlDbType.NVarChar)  { Value = logger },
                //new SqlParameter("@sequence_id",  SqlDbType.Int),     { Value = sequenceId },

                new SqlParameter("@address",      SqlDbType.NVarChar) { Value = address },
                new SqlParameter("@machine_name", SqlDbType.NVarChar) { Value = machineName },
                new SqlParameter("@windows_id",   SqlDbType.NVarChar) { Value = windowsId },
                
                new SqlParameter("@call_side",    SqlDbType.NVarChar) { Value = callerSide },
                new SqlParameter("@line_number",  SqlDbType.Int)      { Value = lineNumber },

                new SqlParameter("@message",      SqlDbType.NVarChar) { IsNullable = true, Value = message ?? DBNull.Value },
                new SqlParameter("@exception",    SqlDbType.NVarChar) { IsNullable = true, Value = exception ?? DBNull.Value },
                new SqlParameter("@json_object",  SqlDbType.NVarChar) { IsNullable = true, Value = jsonObject ?? DBNull.Value }
            };

            return parameter;
        }

        private static SqlParameter[] InsertEventlogContextParameters(
            object timeStamp, object level, object logger,
            object address, object machineName, object windowsId, object callerSide, object lineNumber,
            object threadId, object stackTrace, object message, object exception, object jsonObject)
        {
            SqlParameter[] parameter = 
            {
                new SqlParameter("@time_stamp",   SqlDbType.DateTime2) { Value = timeStamp },
                new SqlParameter("@level",        SqlDbType.TinyInt)   { Value = level },
                new SqlParameter("@logger",       SqlDbType.NVarChar)  { Value = logger },
                //new SqlParameter("@sequence_id",  SqlDbType.Int),     { Value = sequenceId },

                new SqlParameter("@address",      SqlDbType.NVarChar) { Value = address },
                new SqlParameter("@machine_name", SqlDbType.NVarChar) { Value = machineName },
                new SqlParameter("@windows_id",   SqlDbType.NVarChar) { Value = windowsId },
                
                new SqlParameter("@call_side",    SqlDbType.NVarChar) { Value = callerSide },
                new SqlParameter("@line_number",  SqlDbType.Int)      { Value = lineNumber },

                new SqlParameter("@thread_id",    SqlDbType.Int)      { IsNullable = true, Value = threadId ?? DBNull.Value },
                new SqlParameter("@stack_trace",  SqlDbType.NVarChar) { IsNullable = true, Value = stackTrace ?? DBNull.Value },

                new SqlParameter("@message",      SqlDbType.NVarChar) { IsNullable = true, Value = message ?? DBNull.Value },
                new SqlParameter("@exception",    SqlDbType.NVarChar) { IsNullable = true, Value = exception ?? DBNull.Value },
                new SqlParameter("@json_object",  SqlDbType.NVarChar) { IsNullable = true, Value = jsonObject ?? DBNull.Value }
            };

            return parameter;
        }
    }
}
