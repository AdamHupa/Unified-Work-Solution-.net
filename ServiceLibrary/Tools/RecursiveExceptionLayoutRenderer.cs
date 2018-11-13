using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Custom NLog Layout Renderer must be registered before using it, by either method:
// 
// 1) Separate DLL and <extensions> tag
//    add the following attribute to the class
//    [NLog.LayoutRenderers.LayoutRenderer("json-recursive-exception")]
// 2) Manual registration
//    add the following line in the applications start-up method
//    NLog.LayoutRenderers.LayoutRenderer.Register("json-recursive-exception", typeof(RecursiveExceptionLayoutRenderer));

namespace ServiceLibrary.Tools
{
    [NLog.LayoutRenderers.LayoutRenderer(RecursiveExceptionLayoutRenderer.DefaultName)]
    public sealed class RecursiveExceptionLayoutRenderer : NLog.LayoutRenderers.LayoutRenderer
    {
        private static readonly List<string> _defaultPropertyNames = new List<string>()
        {
            "InnerException", "Message", "Source", "StackTrace", "TargetSite"
        };

        public const string DefaultName = "json-recursive-exception";

        private string _className = "ClassName";
        private bool _includeClassName = true;
        private WorkMode _mode = WorkMode.ISerializable;
        private IList<string> _propertyNames = null;


        //public RecursiveExceptionLayoutRenderer() { }


        public enum WorkMode : byte
        {
            Full = 0,     // Full serialization without serializable interface.
            Partial,      // Selective serialization without null or default values.
            Slim,         // Selective serialization, no indentation or null values, advised for remote logging.
            ISerializable // Full Json.net serialization including ISerializable, advised for local logging.
        }


        /// <summary>
        /// Get or set json token's name representing type of exception being serialized.
        /// Needed when ISerializable work mode is NOT set. Default value: "ClassName".
        /// </summary>
        /// <exception cref="NLog.NLogConfigurationException">throws, if string is empty.</exception>
        public string ClassName
        {
            get { return _className; }
            set
            {
                if (String.IsNullOrWhiteSpace(value))
                    throw new NLog.NLogConfigurationException("This property (ClassName) can not be an empty string.");
                _className = value;
            }
        }

        /// <summary>
        /// Get or set if an additional json token representing type of exception should be added.
        /// Needed when ISerializable work mode is NOT set. Default value: true.
        /// </summary>
        public bool IncludeClassName
        {
            get { return _includeClassName; }
            set { _includeClassName = value; }
        }

        /// <summary>
        /// Available values Full, Partial, Slim, and the default ISerializable.
        /// </summary>
        [NLog.Config.DefaultParameter] //, NLog.Config.RequiredParameter]
        public WorkMode Mode
        {
            get { return _mode; }
            set { _mode = value; }
        }

        /// <summary>
        /// List of allowed properties to be serialized, if empty default list will be serialized.
        /// Should be initialize with priory sorted list of exception property names.
        /// Default properties: "InnerException", "Message", "Source", "StackTrace", "TargetSite"
        /// </summary>
        /// <exception cref="NLog.NLogConfigurationException">throws, if "InnerException" value is not included.</exception>
        public IList<string> Properties
        {
            get { return _propertyNames; }
            set
            {
                if (value == null || value.Count() == 0 || !value.Contains("InnerException"))
                    throw new NLog.NLogConfigurationException("List of allowed properties by design has to include \"InnerException\".");
                _propertyNames = value;
            }
        }


        protected override void Append(StringBuilder builder, NLog.LogEventInfo logEvent)
        {
            System.Exception exception = logEvent.Exception;
            if (exception == null)
                return;

            if (_mode == WorkMode.ISerializable)
                StandardExceptionSerialization(builder, exception);
            else
                CustomizedExceptionSerialization(builder, exception);
        }

        private void CustomizedExceptionSerialization(StringBuilder builder, System.Exception exception)
        {
            try
            {
                Newtonsoft.Json.JsonSerializer jsonSerializer = new Newtonsoft.Json.JsonSerializer()
                {
                    // disable serializable interface to allow e.g.: null value handling
                    // this is needed to work properly with types implementing ISerializable interface
                    ContractResolver =
                          (_mode == WorkMode.Full)
                        ? (new Newtonsoft.Json.Serialization.DefaultContractResolver() { IgnoreSerializableInterface = true })
                        : (new PartialSerializationContractResolver((_propertyNames != null) ? _propertyNames : _defaultPropertyNames)),

                    Formatting =
                        _mode == WorkMode.Slim ? Newtonsoft.Json.Formatting.None : Newtonsoft.Json.Formatting.Indented,

                    NullValueHandling =
                        _mode == WorkMode.Full ? Newtonsoft.Json.NullValueHandling.Include : Newtonsoft.Json.NullValueHandling.Ignore,

                    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore,
                };


                // partial serialization to JObject
                var jsonObject = Newtonsoft.Json.Linq.JObject.FromObject(exception, jsonSerializer);

                // appending Exception type property to serialized
                if (_includeClassName)
                {
                    if (exception.InnerException == null)
                    {
                        // optimization for simple exception
                        jsonObject.Add(_className, exception.GetType().ToString());
                    }
                    else
                    {
                        System.Collections.Generic.Stack<Tuple<string, Newtonsoft.Json.Linq.JObject>> stack =
                            new Stack<Tuple<string, Newtonsoft.Json.Linq.JObject>>();

                        Newtonsoft.Json.Linq.JObject j = jsonObject;


                        for (System.Exception e = exception; ; j = (Newtonsoft.Json.Linq.JObject)j["InnerException"])
                        {
                            stack.Push(new Tuple<string, Newtonsoft.Json.Linq.JObject>(e.GetType().ToString(), j));

                            e = e.InnerException;

                            if (e == null)
                                break;
                        }

                        foreach (var pair in stack)
                        {
                            pair.Item2.Add(_className, pair.Item1);
                        }
                    }
                }

                builder.Append(jsonObject.ToString(jsonSerializer.Formatting));
            }
            catch (Exception ex)
            {
                NLog.Common.InternalLogger.Error(ex, "Exception serialization failed. Newtonsoft.Json error.");
            }
        }

        private void StandardExceptionSerialization(StringBuilder builder, System.Exception exception)
        {
            StringBuilder stringBuilder = new StringBuilder();

            try
            {
                Newtonsoft.Json.JsonSerializerSettings settings = new Newtonsoft.Json.JsonSerializerSettings
                {
                    Formatting = Newtonsoft.Json.Formatting.Indented,
                    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                };

                /* builder.Append(Newtonsoft.Json.JsonConvert.SerializeObject(exception, settings)); */

                var jsonSerializer = Newtonsoft.Json.JsonSerializer.CreateDefault(settings);
                var stringWriter = new System.IO.StringWriter(stringBuilder, System.Globalization.CultureInfo.InvariantCulture);
                using (var jsonTextWriter = new Newtonsoft.Json.JsonTextWriter(stringWriter))
                {
                    jsonTextWriter.Formatting = jsonSerializer.Formatting;

                    jsonSerializer.Serialize(jsonTextWriter, exception, null);
                }
            }
            catch (Exception ex)
            {
                NLog.Common.InternalLogger.Error(ex, "Exception serialization failed. Newtonsoft.Json error.");
            }

            builder.Append(stringBuilder);
        }
    }
}
