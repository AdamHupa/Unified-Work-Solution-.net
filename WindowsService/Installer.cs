using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ServiceProcess;
using System.Configuration.Install;

namespace WindowsService
{
    [System.ComponentModel.RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        private ServiceProcessInstaller _process;
        private ServiceInstaller _service;


        public ProjectInstaller()
        {
            _process = new ServiceProcessInstaller();
            _process.Account = ServiceAccount.LocalSystem;
            //_process.Account = System.ServiceProcess.ServiceAccount.NetworkService;
            //_process.Password = null;
            //_process.Username = null;

            _service = new ServiceInstaller();
            _service.StartType = ServiceStartMode.Automatic;
            _service.ServiceName = "_Unified Work Solution"; // directly the name of this service in: services.msc
            _service.Description = "Unified Work Solution Services";
            //_service.StartType = System.ServiceProcess.ServiceStartMode.Automatic;

            Installers.Add(_process);
            Installers.Add(_service);
        }
    }
}
