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
    /// Low importance logging context.
    /// </summary>
    public class Context
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }


        /// <remarks>Remember to update migration code so the ForeignKey will allow cascade delete.</remarks>
        public int? LogId { get; set; }


        public int ThreadId { get; set; }

        [StringLength(maximumLength: 200)]
        public string StackTrace { get; set; }


        [ForeignKey("LogId")]
        public virtual LogRecord LogRecord { get; set; }
    }
}
