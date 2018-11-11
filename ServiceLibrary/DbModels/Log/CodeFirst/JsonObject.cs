using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServiceLibrary.DbModels.Log.CodeFirst
{
    /// <summary>
    /// Serialized object entity.
    /// </summary>
    public class JsonObject
    {
        public JsonObject()
        {
            this.ExceptionMatches = new HashSet<LogRecord>();
            this.JsonObjectMatches = new HashSet<LogRecord>();
        }


        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int JsonId { get; set; }


        [Column(TypeName = "smallint")]
        public Int16? LanguageCode { get; set; }

        [Required]
        public string Json { get; set; }


        [InverseProperty("Exception")]
        public virtual ICollection<LogRecord> ExceptionMatches { get; set; }

        [InverseProperty("JsonObject")]
        public virtual ICollection<LogRecord> JsonObjectMatches { get; set; }
    }
}
