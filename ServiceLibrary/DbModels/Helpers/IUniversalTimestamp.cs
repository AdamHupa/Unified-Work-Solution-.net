using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLibrary.DbModels.Helpers
{
    /// <remarks>UTC values intended to be only initiated or updated by the database.</remarks>
    public interface IUniversalTimestamp
    {
        DateTime Creation { get; set; }
        Nullable<DateTime> EditedOn { get; set; }
    }
}
