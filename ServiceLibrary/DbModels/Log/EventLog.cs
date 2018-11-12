using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

// Note: avoid if possible direct mixing Entity Framework with Windows Communication Foundation

namespace ServiceLibrary.DbModels.Log
{
    [DataContract]
    public class EventLog
    {
        [Required]
        [DataMember]
        [Column("time_stamp", TypeName = "datetime2")]
        public DateTime TimeStamp { get; set; }

        [Required]
        [DataMember]
        [Column("level", TypeName = "tinyint")]
        public NLogLevel Level { get; set; }

        [Required]
        [DataMember]
        [Column("logger", TypeName = "nvarchar(64)"), StringLength(64)]
        public string Logger { get; set; }

        //[Required]
        //[DataMember]
        //[Column("sequence_id", TypeName = "int")]
        //public int SequenceId { get; set; }

        // Source

        [Required]
        [DataMember]
        [Column("adress", TypeName = "nvarchar(80)"), StringLength(maximumLength: 80)]
        public string Address { get; set; }

        [Required]
        [DataMember]
        [Column("machine_name", TypeName = "nvarchar(50)"), StringLength(maximumLength: 50)]
        public string MachineName { get; set; }

        [Required]
        [DataMember]
        [Column("windows_id", TypeName = "nvarchar(50)"), StringLength(maximumLength: 50)]
        public string WindowsId { get; set; }

        // CallSide

        [Required]
        [DataMember]
        [Column("call_side", TypeName = "nvarchar(128)"), StringLength(maximumLength: 128)]
        public string CallerSide { get; set; }

        [Required]
        [DataMember]
        [Column("line_number", TypeName = "int"), Range(0, int.MaxValue)]
        public int LineNumber { get; set; } // Nullable<int> ?

        // Context

        [DataMember]
        [Column("thread_id", TypeName = "int"), Range(0, int.MaxValue)]
        public Nullable<int> ThreadId { get; set; }

        [DataMember]
        [Column("stack_trace", TypeName = "nvarchar(200)"), StringLength(maximumLength: 200)]
        public string StackTrace { get; set; }

        // Message

        [DataMember]
        [Column("message", TypeName = "nvarchar(256)"), StringLength(maximumLength: 256)]
        public string Message { get; set; }

        // JsonObject

        [DataMember]
        [Column("exception", TypeName = "nvarchar(max)")]
        public string Exception { get; set; }

        [DataMember]
        [Column("json_object", TypeName = "nvarchar(max)")]
        public string Json { get; set; }


        public bool IsValid()
        {
            ValidationContext validationContext = new ValidationContext(this, null, null); //??

            /* validationResults argument is set to null to break at the first validation failure */
            return Validator.TryValidateObject(this, validationContext, null, true);
        }
    }
}
