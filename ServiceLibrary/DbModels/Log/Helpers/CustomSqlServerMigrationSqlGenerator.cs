using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
    add the following line to the constructor of Configuration class, created during enabling migrations:
    SetSqlGenerator("System.Data.SqlClient", new DbModels.Log.Helpers.CustomSqlServerMigrationSqlGenerator());
 */

namespace ServiceLibrary.DbModels.Log.Helpers
{
    internal class CustomSqlServerMigrationSqlGenerator : DbModels.Helpers.BasicSqlServerMigrationSqlGenerator
    {
        private static readonly IDictionary<string, string> LocalAffectedTables = new Dictionary<string, string>()
        {
            { LogDbContext.DefaultSchema + ".Sources", "Creation" },
        };


        public override IDictionary<string, string> AffectedTables
        {
            get { return LocalAffectedTables; }
        }
    }
}
