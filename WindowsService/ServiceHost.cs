using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ServiceModel;

namespace WindowsService
{
    internal class ServiceHost<TService> where TService : class//, new()
    {
        static ServiceHost serviceHost = null;


        public ServiceHost() { }


        internal static void Start()
        {
            if (serviceHost != null)
            {
                serviceHost.Close();
            }

            serviceHost = new ServiceHost(typeof(TService));
            //serviceHost = new ServiceLibrary.DependencyInjection.ServiceHost(typeof(TService));
            serviceHost.Open();
        }

        internal static void Stop()
        {
            if (serviceHost != null)
            {
                serviceHost.Close();
                serviceHost = null;
            }
        }
    }
}
