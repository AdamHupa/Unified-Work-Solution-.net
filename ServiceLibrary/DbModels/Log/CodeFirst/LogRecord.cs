using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServiceLibrary.DbModels.Log.CodeFirst
{
    /// <remarks>Entity equivalent for: NLog.LogEventInfo</remarks>
    //[Table("LogRecord", Schema = "Log")]
    public class LogRecord
    {
        public LogRecord()
        {
            this.Contexts = new HashSet<Context>();
        }


        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }


        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime TimeStamp { get; set; } // should be UTC

        public NLogLevel Level { get; set; }

        [Required]
        [StringLength(64)] //??
        public string Logger { get; set; }

        ///// <summary>
        ///// Log event's unique identifier, automatically generated, monotonously increamented.
        ///// </summary>
        ///// <remarks>SequenceId was introduced in NLog v4.5.</remarks>
        //  public int SequenceId { get; set; }


        public int? SourceId { get; set; }

        public int? CallSideId { get; set; }

        public int? MessageId { get; set; }

        public int? ExceptionId { get; set; }

        public int? JsonId { get; set; }


        [ForeignKey("SourceId")]
        public virtual Source Source { get; set; }

        [ForeignKey("CallSideId")]
        public virtual CallSide CallSide { get; set; }

        [ForeignKey("MessageId")]
        public virtual Message Message { get; set; }

        [ForeignKey("ExceptionId")]
        public virtual JsonObject Exception { get; set; }

        [ForeignKey("JsonId")]
        public virtual JsonObject JsonObject { get; set; }


        public virtual ICollection<Context> Contexts { get; set; }
    }
}
