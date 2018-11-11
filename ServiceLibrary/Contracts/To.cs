using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLibrary.Contracts
{
    public static class To
    {
        #region ArgumentFault

        public static Data.ArgumentFault ArgumentFault(string parameterName)
        {
            return new Data.ArgumentFault() { ParameterName = parameterName };
        }

        public static Data.ArgumentFault ArgumentFault(string parameterName, string message)
        {
            return new Data.ArgumentFault() { ParameterName = parameterName, Message = message };
        }

        #endregion // ArgumentFault

        #region FaultException<>

        public static System.ServiceModel.FaultException<SerializableType> FaultException<SerializableType>(SerializableType detail)
        {
            return new System.ServiceModel.FaultException<SerializableType>(detail);
        }

        public static System.ServiceModel.FaultException<SerializableType> FaultException<SerializableType>(SerializableType detail, string reason)
        {
            return new System.ServiceModel.FaultException<SerializableType>(detail, reason);
        }

        #endregion // FaultException<>

        #region ServiceFault

        public static Data.ServiceFault ServiceFault(string message)
        {
            return new Data.ServiceFault() { IsCritical = false, Message = message };
        }

        public static Data.ServiceFault ServiceFault(string message, bool isCritical)
        {
            return new Data.ServiceFault() { IsCritical = isCritical, Message = message };
        }

        public static Data.ServiceFault ServiceFault(System.Exception ex)
        {
            return new Data.ServiceFault() { IsCritical = true, Message = ex.Message };
        }

        #endregion // ServiceFault

        #region TimeoutFault

        public static Data.TimeoutFault TimeoutFault()
        {
            return new Data.TimeoutFault();
        }

        public static Data.TimeoutFault TimeoutFault(string message)
        {
            return new Data.TimeoutFault() { Message = message };
        }

        #endregion // TimeoutFault
    }
}
