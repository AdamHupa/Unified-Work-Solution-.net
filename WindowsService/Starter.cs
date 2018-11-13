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


            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
            { 
                new ServiceHosts()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
