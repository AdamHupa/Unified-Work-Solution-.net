using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
    ASP.net DLL initialization, order in respect to other DLL is not guaranteed
    1. create a class with a public static void method without any argument
    2. in Properties\AssemblyInfo.cs file reference the created method using PreApplicationStartMethod attribute
    
    [assembly: PreApplicationStartMethod(typeof(ServiceLibrary.DLLStartupObject), "Initialize")] 
    link: https://msdn.microsoft.com/en-us/library/system.web.preapplicationstartmethodattribute.aspx
 */

namespace ServiceLibrary
{
    public static class DLLStartupObject
    {
        private static readonly Dictionary<string, object> _registeredDomainProperties = new Dictionary<string, object>();
        private static readonly object _lock = new object();


        //static DLLStartupObject() {}


        public static IDictionary<string, object> RegisteredDomainProperties
        {
            get { return _registeredDomainProperties; }
        }


        /// <summary>
        /// DLL initialization and setting initial default values of application domain properties.
        /// </summary>
        public static void Initialize()
        {
            lock (_lock)
            {
                // for RelativeDefaultConnectionString
                InitializeDomainProperty("DataDirectory", AppDomain.CurrentDomain.BaseDirectory); // System.IO.Path.Combine(, "..\..\
                InitializeDomainProperty("ActiveConnectionStringName", ""); //"RelativeDefaultConnectionString");

                // CHANGE HERE
            }
        }

        public static bool RegisterDomainProperty(string name, object data, System.Security.IPermission permission = null)
        {
            bool result = false;

            lock (_lock)
            {
                try
                {
                    if (_registeredDomainProperties.ContainsKey(name) == false)
                    {
                        if (permission == null)
                            AppDomain.CurrentDomain.SetData(name, data);
                        else
                            AppDomain.CurrentDomain.SetData(name, data, permission);

                        _registeredDomainProperties.Add(name, data);

                        result = true;
                    }
                }
                catch { }
            }

            return result;
        }

        /// <summary>
        /// Creation, configuration and, initial connection to the database.
        /// Should be called after the Initialize method otherwise it will rely on default settings.
        /// </summary>
        /// <returns>true, if database exists and connection was achieved.</returns>
        public static bool ValidateDatabaseConnection()
        {
            try
            {
                /* service method -> database context -> if needed create database -> add message to database */

                Tools.GlobalLogger.LogInternalMessage(
                    typeof(DLLStartupObject).Name, DbModels.Log.NLogLevel.Custom, "Attempting to connect to the database."); // expecting exception
                Tools.GlobalLogger.LogInternalMessage(
                    typeof(DLLStartupObject).Name, DbModels.Log.NLogLevel.Custom, "The connection to the database has been validated.");
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }


        private static void InitializeDomainProperty(string name, object data)
        {
            try
            {
                if (_registeredDomainProperties.ContainsKey(name) == false)
                {
                    AppDomain.CurrentDomain.SetData(name, data);
                    _registeredDomainProperties.Add(name, data);
                }

            }
            catch { }
        }
    }
}
