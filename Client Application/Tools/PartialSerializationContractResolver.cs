using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json; // for: MemberSerialization
using Newtonsoft.Json.Serialization; // for: DefaultContractResolver, JsonProperty, 
using System.Reflection; // for: MemberInfo

// some Json.net functionalities or its customizations may need to disable serializable interface
// to properly work on types implementing ISerializable interface
// IgnoreSerializableInterface = true

namespace Client_Application.Tools
{
    public class PartialSerializationContractResolver : Newtonsoft.Json.Serialization.DefaultContractResolver
    {
        public static readonly PartialSerializationContractResolver Default = new PartialSerializationContractResolver();

        private IList<string> _propertyNames = new List<string>();


        public PartialSerializationContractResolver() { base.IgnoreSerializableInterface = true; }

        /// <exception cref="ArgumentException">throws, if (propertyNames) is null or empty.</exception>
        public PartialSerializationContractResolver(IList<string> propertyNames)
        {
            if (propertyNames == null || propertyNames.Count == 0)
                throw new ArgumentException("propertyNames"); // nameof()

            this._propertyNames = propertyNames;
            base.IgnoreSerializableInterface = true;
        }


        /// <summary>
        /// List of allowed properties to be serialized. The default list is empty.
        /// Should be initialize with priory sorted list of object property names.
        /// </summary>
        /// <exception cref="ArgumentException">throws, if setting null or empty.</exception>
        public IList<string> PropertyNames
        {
            get { return _propertyNames; }
            set
            {
                if (value == null || value.Count == 0)
                    throw new ArgumentException("propertyNames"); // nameof()
                _propertyNames = value;
            }
        }


        /// <remarks>
        /// 1. stack trace: DefaultContractResolver -> CreateContract -> CreateDynamicContract/CreateObjectContract -> CreateProperties -> CreateProperty
        /// 2. NLog always enters this method: public override JsonContract ResolveContract(Type type)
        /// </remarks>
        protected override JsonProperty CreateProperty(MemberInfo member, Newtonsoft.Json.MemberSerialization memberSerialization)
        {
            JsonProperty jsonProperty = base.CreateProperty(member, memberSerialization);

            if (_propertyNames.Contains(jsonProperty.PropertyName))
                jsonProperty.ShouldSerialize = instance => true;
            else
                jsonProperty.ShouldSerialize = instance => false;

            return jsonProperty;
        }
    }
}
