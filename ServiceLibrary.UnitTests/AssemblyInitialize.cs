using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ServiceLibrary.UnitTests
{
    [TestClass]
    public class AssemblyInitialize
    {
        [AssemblyInitialize]
        public static void Initialize(TestContext context)
        {
            // EffortProviderFactory class -> defaultConnectionFactory (config file, Entity Framework section) -> register/override provider
            Effort.Provider.EffortProviderConfiguration.RegisterProvider();

            // set domain's property used by the RelativeDefaultConnectionString
            ServiceLibrary.DLLStartupObject.Initialize();
        }
    }
}
