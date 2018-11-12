using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLibrary.DependencyInjection
{
    public class ServiceHost : System.ServiceModel.ServiceHost
    {
        public ServiceHost()
        { }

        public ServiceHost(object singletonInstance, params Uri[] baseAddresses)
            : base(singletonInstance, baseAddresses)
        { }

        public ServiceHost(Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        { }


        protected override void OnOpening()
        {
            Description.Behaviors.Add(new ServiceLibrary.DependencyInjection.ServiceBehavior());

            base.OnOpening();
        }
    }
}
