﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ServiceLibrary.DbModels.Log.Sql {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class DropResources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal DropResources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("ServiceLibrary.DbModels.Log.Sql.DropResources", typeof(DropResources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to DROP FUNCTION Log.fn_select_eventlog_context;.
        /// </summary>
        internal static string fn_select_eventlog_context_sql {
            get {
                return ResourceManager.GetString("fn_select_eventlog_context_sql", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to DROP FUNCTION Log.fn_select_eventlog;.
        /// </summary>
        internal static string fn_select_eventlog_sql {
            get {
                return ResourceManager.GetString("fn_select_eventlog_sql", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to DROP PROCEDURE Log.usp_insert_eventlog_context;.
        /// </summary>
        internal static string usp_insert_eventlog_context_sql {
            get {
                return ResourceManager.GetString("usp_insert_eventlog_context_sql", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to DROP PROCEDURE Log.usp_insert_eventlog;.
        /// </summary>
        internal static string usp_insert_eventlog_sql {
            get {
                return ResourceManager.GetString("usp_insert_eventlog_sql", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to DROP VIEW Log.vw_sourceaddresses;.
        /// </summary>
        internal static string vw_sourceaddresses_sql {
            get {
                return ResourceManager.GetString("vw_sourceaddresses_sql", resourceCulture);
            }
        }
    }
}
