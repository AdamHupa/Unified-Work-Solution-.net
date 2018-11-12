using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

namespace ServiceLibrary.DependencyInjection
{
    public class ServiceInstanceProvider : System.ServiceModel.Dispatcher.IInstanceProvider
    {
        private readonly Type _serviceType;


        public ServiceInstanceProvider(Type serviceType)
        {
            _serviceType = serviceType;
        }


        public object GetInstance(InstanceContext instanceContext)
        {
            return GetInstance(instanceContext, null);
        }

        public object GetInstance(InstanceContext instanceContext, Message message)
        {
            /* Inversion Of Control */

            // very simple replacement for AutoFac
            return ObjectFactory.GetInstance(_serviceType);
        }

        public void ReleaseInstance(InstanceContext instanceContext, object instance)
        {

        }
    }
}
