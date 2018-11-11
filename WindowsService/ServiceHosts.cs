using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace WindowsService
{
    partial class ServiceHosts : ServiceBase
    {
        public ServiceHosts()
        {
            InitializeComponent();
            ServiceName = "_Unified Work Solution 2"; // should be the same as in the installer class, but this is not required
        }

        protected override void OnStart(string[] args)
        {
            
            ServiceHost<ServiceLibrary.Services.LogReceiverService>.Start();
        }

        protected override void OnStop()
        {
            
            ServiceHost<ServiceLibrary.Services.LogReceiverService>.Stop();
        }
    }
}
