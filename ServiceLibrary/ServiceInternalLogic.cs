using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLibrary
{
    public static class ServiceInternalLogic
    {
        private static readonly Lazy<ServiceLogics.ServiceLogic> _internalLogic = new Lazy<ServiceLogics.ServiceLogic>(() =>
        {
            throw new NotImplementedException();
        });


        public static bool Initialization()
        {
            // quick test
            NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Error(new Exception(DateTime.Now.ToString()), "ServiceInternalLogic_Exception");

            return true;
        }
    }
}
