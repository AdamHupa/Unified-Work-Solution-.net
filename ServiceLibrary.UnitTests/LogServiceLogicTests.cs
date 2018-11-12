using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using ServiceLibrary.DbModels.Log;
using ServiceLibrary.DbModels.Log.CodeFirst;
using ServiceLibrary.DependencyInjection;
using ServiceLibrary.ServiceLogics;
using System.Collections.Generic;
using System.Linq;

namespace ServiceLibrary.UnitTests
{
    [TestClass]
    public class LogServiceLogic_1ValidationTests : Tools.BaseTest<LogServiceLogic>
    {
        [TestMethod]
        public void Validate_EventLog()
        {
            List<EventLog> eventLogs_succeess = new List<EventLog>()
            {
                To.EventLog(DateTime.UtcNow,   NLogLevel.Trace, "WOW", "a", "mn", "w", "cs", 13, 17, "st", null, "ex_2", "j_1"),
                To.EventLog(DateTime.MaxValue, NLogLevel.Trace, "WOW", "a", "mn", "w", "cs", 13, null, null, null, null, null),
                To.EventLog(DateTime.Today,    NLogLevel.Trace, "WOW", "a", "mn", "w", "cs", 13, 17, "st", "m", "ex_2", "j_1"),
                To.EventLog(DateTime.MinValue, NLogLevel.Trace, "WOW", "a", "mn", "w", "cs", 13, null, "st", "m", "ex_2", "j_1"),
            };

            foreach (EventLog eventLog in eventLogs_succeess)
            {
                Assert.IsNotNull(eventLog);
                Assert.IsTrue(eventLog.IsValid());
            }


            List<EventLog> eventLogs_fail = new List<EventLog>()
            {
                To.EventLog(DateTime.Now, NLogLevel.Trace, null, "a", "mn", "w", "cs", 13, 17, "st", "m", "ex_2", "j_1"),
                To.EventLog(DateTime.Now, NLogLevel.Trace, "WOW", null, "mn", "w", "cs", 13, 17, "st", "m", "ex_2", "j_1"),
                To.EventLog(DateTime.Now, NLogLevel.Trace, "WOW", "a", null, "w", "cs", 13, 17, "st", "m", "ex_2", "j_1"),
                To.EventLog(DateTime.Now, NLogLevel.Trace, "WOW", "a", "mn", null, "cs", 13, 17, "st", "m", "ex_2", "j_1"),
                To.EventLog(DateTime.Now, NLogLevel.Trace, "WOW", "a", "mn", "w", null, 13, 17, "st", "m", "ex_2", "j_1"),

                To.EventLog(DateTime.Now, NLogLevel.Trace, "WOW", "a", "mn", "w", "cs", -3, 17, "st", "m", "ex_2", "j_1"),
                To.EventLog(DateTime.Now, NLogLevel.Trace, "WOW", "a", "mn", "w", "cs", 13, -7, "st", "m", "ex_2", "j_1"),
                
                To.EventLog(DateTime.Now, NLogLevel.Trace, new string('X', 65), "a", "mn", "w", "cs", 13, 17, "st", "m", "ex_2", "j_1"),
                To.EventLog(DateTime.Now, NLogLevel.Trace, "WOW", new string('X', 81), "mn", "w", "cs", 13, 17, "st", "m", "ex_2", "j_1"),
                To.EventLog(DateTime.Now, NLogLevel.Trace, "WOW", "a", new string('X', 51), "w", "cs", 13, 17, "st", "m", "ex_2", "j_1"),
                To.EventLog(DateTime.Now, NLogLevel.Trace, "WOW", "a", "mn", new string('X', 51), "cs", 13, 17, "st", "m", "ex_2", "j_1"),
                To.EventLog(DateTime.Now, NLogLevel.Trace, "WOW", "a", "mn", "w", new string('X', 129), 13, 17, "st", "m", "ex_2", "j_1"),
                To.EventLog(DateTime.Now, NLogLevel.Trace, "WOW", "a", "mn", "w", "cs", 13, 17, new string('X', 201), "m", "ex_2", "j_1"),
                To.EventLog(DateTime.Now, NLogLevel.Trace, "WOW", "a", "mn", "w", "cs", 13, 17, "st", new string('X', 257), "ex_2", "j_1"),
                
                To.EventLog(DateTime.Now, NLogLevel.Trace, "WOW", null, "mn", null, "cs", -3, 17, "st", "m", "ex_2", "j_1"),
            };

            foreach (EventLog eventLog in eventLogs_fail)
            {
                Assert.IsNotNull(eventLog);
                Assert.IsFalse(eventLog.IsValid());
            }
        }
    }

    [TestClass]
    public class LogServiceLogic_2LoggingTests : Tools.BaseTest<LogServiceLogic>
    {
        private IInstanceProvider<LogDbContext> _logDbContextProvider = null;
        private LogServiceLogic _logic = null;

        private List<EventLog> _eventLogs = new List<EventLog>()
        {
            null,
            To.EventLog(DateTime.Now, NLogLevel.Trace, null, "a", "mn", "w", "cs", 13, 17, "st", "m", "ex_2", "j_1"),
            To.EventLog(DateTime.Now, NLogLevel.Trace, "Invalid.1", null, "mn", "w", "cs", 13, 17, "st", "m", "ex_2", "j_1"),
            To.EventLog(DateTime.Now, NLogLevel.Trace, "Invalid.2", new string('X', 81), "mn", "w", "cs", 13, 17, "st", "m", "ex_2", "j_1"),
            To.EventLog(DateTime.Now, NLogLevel.Trace, "Invalid.3", "a", "mn", "w", "cs", -3, 17, "st", "m", "ex_2", "j_1"),
            To.EventLog(DateTime.Now, NLogLevel.Trace, "Invalid.4", "a", "mn", "w", "cs", 13, -7, "st", "m", "ex_2", "j_1"),
            To.EventLog(DateTime.Now, NLogLevel.Trace, "Invalid.5", null, "mn", null, "cs", -3, 17, "st", "m", "ex_2", "j_1"),
        
            To.EventLog(DateTime.UtcNow,   NLogLevel.Trace, "Valid.1", "a", "mn", "w", "cs", 13, 17, "st", null, "ex_2", "j_1"),
            To.EventLog(DateTime.MaxValue, NLogLevel.Trace, "Valid.2", "a", "mn", "w", "cs", 13, null, null, null, null, null),
            To.EventLog(DateTime.Today,    NLogLevel.Trace, "Valid.3", "a", "mn", "w", "cs", 13, 17, "st", "m", "ex_2", "j_1"),
            To.EventLog(DateTime.MinValue, NLogLevel.Trace, "Valid.4", "a", "mn", "w", "cs", 13, null, "st", "m", "ex_2", "j_1"),
        };


        [TestInitialize]
        public override void InitializeMethod()
        {
            base.InitializeMethod();

            //Tools.EffortProviderFactory.ResetDb();


            ContextProviderManufacture manufacture = new ContextProviderManufacture() { ContextProviderPrototypes = ContextProvider.Prototypes };

            //Tools.LogContextProviderState state = new Tools.LogContextProviderState()
            //{
            //    ConnectionFactory = () => new ServiceLibrary.UnitTests.Tools.EffortProviderFactory()
            //};

            _logDbContextProvider = manufacture.Manufacture("LogDbContext") as IInstanceProvider<LogDbContext>;//, state as Tools.LogContextProviderState);

            _logic = new LogServiceLogic(_logDbContextProvider);
        }

        [TestCleanup]
        public override void CleanUpMethod()
        {
            base.CleanUpMethod();

            _logDbContextProvider = null;
            _logic = null;
        }

        [TestMethod]
        public void RunOperation_ReceiveLogA()
        {
            IList<LogRecord> logRecords = null;

            try
            {
                using (var context = _logDbContextProvider.GetInstance())
                {
                    context.LogRecords.RemoveRange(context.LogRecords);
                    context.SaveChanges();

                    foreach (EventLog eventLog in _eventLogs)
                    {
                        if (eventLog != null)
                        {
                            _logic.ReceiveLogA(eventLog).GetAwaiter().GetResult();

                            logRecords = context.LogRecords.Where(lr => lr.Logger == eventLog.Logger).ToList();

                            if (eventLog.IsValid())
                                Assert.AreEqual(logRecords.Count, 1);
                            else
                                Assert.AreEqual(logRecords.Count, 0);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public void RunOperation_ReceiveLogS()
        {
            IList<LogRecord> logRecords = null;

            try
            {
                using (var context = _logDbContextProvider.GetInstance())
                {
                    context.LogRecords.RemoveRange(context.LogRecords);
                    context.SaveChanges();

                    foreach (EventLog eventLog in _eventLogs)
                    {
                        if (eventLog != null)
                        {
                            _logic.ReceiveLogS(eventLog);

                            logRecords = context.LogRecords.Where(lr => lr.Logger == eventLog.Logger).ToList();

                            if (eventLog.IsValid())
                                Assert.AreEqual(logRecords.Count, 1);
                            else
                                Assert.AreEqual(logRecords.Count, 0);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }
    }
}
