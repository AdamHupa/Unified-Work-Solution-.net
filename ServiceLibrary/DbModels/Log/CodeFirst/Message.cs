using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServiceLibrary.DbModels.Log.CodeFirst
{
    public class Message
    {
        public Message()
        {
            this.LogEntries = new HashSet<LogRecord>();
        }


        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MessageId { get; set; }


        [Column(TypeName = "smallint")]
        public Int16? LanguageCode { get; set; }

        [Required]
        [StringLength(maximumLength: 256)]
        public string Text { get; set; }


        public virtual ICollection<LogRecord> LogEntries { get; set; }
    }
}
