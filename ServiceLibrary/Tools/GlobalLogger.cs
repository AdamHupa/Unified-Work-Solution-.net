using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ServiceLibrary.DbModels.Log;
using ServiceLibrary.DbModels.Log.CodeFirst;
using System.Data.Entity;

namespace ServiceLibrary.Tools
{
    public static class GlobalLogger
    {
        public static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private static readonly DependencyInjection.IInstanceProvider<DbModels.Log.LogDbContext> _contextProvider;


        static GlobalLogger()
        {
            _contextProvider = (DependencyInjection.IInstanceProvider<DbModels.Log.LogDbContext>)
                DependencyInjection.ObjectFactory.GetInstance(typeof(DependencyInjection.IInstanceProvider<DbModels.Log.LogDbContext>));
        }


        #region NLog Referenced Members

        public static async void ProcessLogMessage(string timeStamp, string level, string logger, /*string SequenceId,*/
                                                   string address, string machineName, string windowsId,
                                                   string callSide, string lineNumber,
                                                   string threadId, string stackTrace,
                                                   string message, string exception, string json)
        {
            
            // required assertion to prevent NLog from adding empty logs to the log database
            if (String.IsNullOrWhiteSpace(logger))
                return;

            DateTime deserializedTimeStamp;
            NLogLevel deserializedLevel;
            int deserializedLineNumber;
            int deserializedThreadId;

            if (!DateTime.TryParse(timeStamp, out deserializedTimeStamp) ||
                !Enum.TryParse<NLogLevel>(level, out deserializedLevel) ||
                !Int32.TryParse(lineNumber, out deserializedLineNumber) ||
                !Int32.TryParse(threadId, out deserializedThreadId))
                return;

            try
            {
                using (var logDbContext = _contextProvider.GetInstance())
                {
                    Source sourceEntity =
                        await logDbContext.Sources.Where(s => s.Address == address)
                                                  .Where(s => s.MachineName == machineName && s.WindowsId == windowsId)
                                                  .FirstOrDefaultAsync();
                    if (sourceEntity == null)
                    {
                        sourceEntity = new Source() { Address = address, MachineName = machineName, WindowsId = windowsId };
                        logDbContext.Sources.Add(sourceEntity);
                        await logDbContext.SaveChangesAsync();
                    }


                    CallSide callSideEntity = await logDbContext.CallSides.Where(cs => cs.CallerSide == callSide)
                                                                          .Where(cs => cs.LineNumber == deserializedLineNumber)
                                                                          .FirstOrDefaultAsync();
                    if (callSideEntity == null)
                    {
                        try
                        {
                            callSideEntity = new CallSide() { CallerSide = callSide, LineNumber = deserializedLineNumber };
                            logDbContext.CallSides.Add(callSideEntity);
                            await logDbContext.SaveChangesAsync();
                        }
                        catch (System.Data.Entity.Infrastructure.DbUpdateException)
                        {
                        }
                        catch (Exception)
                        {
                            throw;
                        }

                        callSideEntity = await logDbContext.CallSides.Where(cs => cs.CallerSide == callSide)
                                                                     .Where(cs => cs.LineNumber == deserializedLineNumber)
                                                                     .FirstOrDefaultAsync();
                        if (callSideEntity == null)
                            return;
                    }

                    /* even if the following will failed the previous information
                     * will be reused or, as not connected, will be useful in the statistics. */

                    Message messageEntity = String.IsNullOrWhiteSpace(message) ? null : new Message() { Text = message };

                    JsonObject exceptionEntity = String.IsNullOrWhiteSpace(exception) ? null : new JsonObject() { Json = exception };

                    JsonObject jsonObjectEntity = String.IsNullOrWhiteSpace(json) ? null : new JsonObject() { Json = json };


                    LogRecord logRecordEntity = new LogRecord()
                    {
                        TimeStamp = deserializedTimeStamp,
                        Level = deserializedLevel,
                        Logger = logger,
                        Source = sourceEntity,
                        CallSide = callSideEntity,
                        Message = messageEntity,
                        Exception = exceptionEntity,
                        JsonObject = jsonObjectEntity
                    };
                    logDbContext.LogRecords.Add(logRecordEntity);
                    await logDbContext.SaveChangesAsync();

                    /* low priority information, may be lost */

                    Context contextEntity = new Context() { ThreadId = deserializedThreadId, StackTrace = stackTrace, LogRecord = logRecordEntity };

                    logDbContext.Contexts.Add(contextEntity);
                    await logDbContext.SaveChangesAsync();
                }
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException)
            {
                // client failed
            }
            catch (Exception)
            {
                // TODO: logging failed.. drop connection?
            }
        }

        #endregion // NLog Referenced Members

        /// <summary>
        /// Logging directly to the database, for special cases omitting the use of a dedicated logger.
        /// </summary>
        /// <remarks>Utilizes a blocking connection to the database.</remarks>
        internal static void LogInternalMessage(string logger, DbModels.Log.NLogLevel level, string message)
        {
            using (var context = _contextProvider.GetInstance())
            {
                context.LogRecords.Add(new DbModels.Log.CodeFirst.LogRecord()
                {
                    Level = level,
                    Logger = logger,
                    Message = new DbModels.Log.CodeFirst.Message() { Text = message },
                    TimeStamp = DateTime.UtcNow,
                });

                context.SaveChanges();
            }
        }
    }
}
