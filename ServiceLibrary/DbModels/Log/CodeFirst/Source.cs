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
    /// Record of network addresses, devices, and particular users.
    /// </summary>
    /// <remarks>
    /// This design omites using an unique index of values (Address, MachineName, WindowsId)
    /// which would prevent duplicating entries but would unnecessary increase the cost of logging.
    /// </remarks>
    public class Source
    {
        public Source()
        {
            this.LogEntries = new HashSet<LogRecord>();
        }


        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }


        ///// <summary>
        ///// Entity column used to reduce database load by placing some of it to the client/service side.
        ///// </summary>
        ///// <remarks>Hash = Hasher<MD5>(Address, MachineName, WindowsId)</remarks>
        //[Required]
        //[StringLength(maximumLength: 32)]
        //[Index("IX_Sources_Unique_Hash", IsClustered = false, IsUnique = true)]
        //public string Hash { get; set; }

        [Column(TypeName = "datetime2")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime Creation { get; set; } // UTC


        [StringLength(maximumLength: 80)]
        [Index("IX_Sources_Address", IsClustered = false, IsUnique = false)]
        public string Address { get; set; }

        [StringLength(maximumLength: 50)]
        public string MachineName { get; set; }

        [StringLength(maximumLength: 50)]
        public string WindowsId { get; set; }


        public virtual ICollection<LogRecord> LogEntries { get; set; }
    }
}
