using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.Entity.Migrations.Model;
using System.Data.Entity.SqlServer;

namespace ServiceLibrary.DbModels.Helpers
{
    internal abstract class BasicSqlServerMigrationSqlGenerator : System.Data.Entity.SqlServer.SqlServerMigrationSqlGenerator
    {
        public abstract IDictionary<string, string> AffectedTables { get; }


        protected override void Generate(AddColumnOperation addColumnOperation)
        {
            if (AffectedTables.ContainsKey(addColumnOperation.Table) &&
                AffectedTables[addColumnOperation.Table] == addColumnOperation.Column.Name)
            {
                addColumnOperation.Column.DefaultValueSql = "GETUTCDATE()";
            }

            base.Generate(addColumnOperation);
        }

        protected override void Generate(CreateTableOperation createTableOperation)
        {
            if (AffectedTables.ContainsKey(createTableOperation.Name))
            {
                string columnName = AffectedTables[createTableOperation.Name];

                foreach (ColumnModel columnModel in createTableOperation.Columns)
                {
                    if (columnModel.Name == columnName)
                        columnModel.DefaultValueSql = "GETUTCDATE()";
                }
            }

            base.Generate(createTableOperation);
        }
    }
}
