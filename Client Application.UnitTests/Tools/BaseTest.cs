using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Client_Application.UnitTests.Tools
{
    [TestClass]
    public abstract class BaseTest<T>
    {
        private static bool failAllTests;
        private static Type failInTestClass = null;


        public TestContext TestContext { get; set; }


        [TestInitialize]
        public virtual void InitializeMethod()
        {
            if (failAllTests && this.GetType() != failInTestClass)
            {
                Assert.Fail("Aborting farther test.");
            }
        }

        [TestCleanup]
        public virtual void CleanUpMethod()
        {
            if (TestContext.CurrentTestOutcome == UnitTestOutcome.Failed)
            {
                failAllTests = true;
                failInTestClass = this.GetType(); // TestContext.FullyQualifiedTestClassName // MethodBase.GetCurrentMethod().DeclaringType;
            }
        }

        public int LineNumber([System.Runtime.CompilerServices.CallerLineNumber] int lineNumber = 0)
        {
            return lineNumber;
        }
    }
}
