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
    /// Log call side.
    /// Record of a large but finite set of log call sides, intended for permanent revisites.
    /// </summary>
    public class CallSide
    {
        public CallSide()
        {
            this.LogEntries = new HashSet<LogRecord>();
        }


        /// <remarks>Remember to update migration code so the PrimaryKey will be NON-clustered.</remarks>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }


        [Required]
        [StringLength(maximumLength: 128)]
        [Index("IX_CallSides_UniqueClusteredPair_MethodAndLine", 1, IsClustered = true, IsUnique = true)]
        public string CallerSide { get; set; }

        //[Range(0, int.MaxValue)]
        [Index("IX_CallSides_UniqueClusteredPair_MethodAndLine", 2, IsClustered = true, IsUnique = true)]
        public int LineNumber { get; set; }


        public virtual ICollection<LogRecord> LogEntries { get; set; }
    }
}
