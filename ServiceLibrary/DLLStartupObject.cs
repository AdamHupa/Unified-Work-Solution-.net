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



        /// <summary>
        /// DLL initialization and setting initial default values of application domain properties.
        /// </summary>
        public static void Initialize()
        {

        }
    }
}
