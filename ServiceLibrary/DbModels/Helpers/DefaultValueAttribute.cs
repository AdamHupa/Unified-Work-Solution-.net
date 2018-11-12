using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLibrary.DbModels.Helpers
{
    /// <summary>
    /// Attribute denoting default value generated on the database side.
    /// This value will override any value when creating a new record.
    /// </summary>
    /// <example>
    /// [DefaultValue("'default_name'")]
    /// [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    /// public string Name { get; set; }
    /// </example>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DefaultValueAttribute : Attribute
    {
        public string SqlDefaultValue { get; private set; }


        public DefaultValueAttribute(string sqlDefaultValue)
            : base()
        {
            SqlDefaultValue = sqlDefaultValue;
        }
    }
}
