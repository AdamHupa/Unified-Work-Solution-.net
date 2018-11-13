using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

/*
    installutil WindowsService.exe
    net start "_Unified Work Solution"
    net stop  "_Unified Work Solution"
    installutil /u WindowsService.exe
 */

namespace WindowsService
{
    static class Starter
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            NLog.LayoutRenderers.LayoutRenderer.Register(ServiceLibrary.Tools.RecursiveExceptionLayoutRenderer.DefaultName,
                                                         typeof(ServiceLibrary.Tools.RecursiveExceptionLayoutRenderer));

            // 1. Ensure that the dedicated logger configuration refers to existing database and methods.
            // 2. Initialize global settings and variables use by the Service Library DLL.
            // 3. Validate connection to the database before logging anything with the dedicated logger.

            ServiceLibrary.DLLStartupObject.Initialize();
            if (ServiceLibrary.DLLStartupObject.RegisteredDomainProperties.ContainsKey("ActiveConnectionStringName"))
                AppDomain.CurrentDomain.SetData("ActiveConnectionStringName", "MultipleModelGlobalConnectionString");

            if (ServiceLibrary.DLLStartupObject.ValidateDatabaseConnection() == false)
                throw new TypeInitializationException(typeof(ServiceLibrary.DLLStartupObject).FullName, null); // nameof()




            ServiceLibrary.ServiceInternalLogic.Initialization();

            ///////////////////////////////////////////////////////////////////
            ServiceLibrary.Tools.GlobalLogger.Logger.Info("Running services.");


            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
            { 
                new ServiceHosts()
            };
            ServiceBase.Run(ServicesToRun);


            ServiceLibrary.Tools.GlobalLogger.Logger.Info("Terminating services.");
        }
    }
}
