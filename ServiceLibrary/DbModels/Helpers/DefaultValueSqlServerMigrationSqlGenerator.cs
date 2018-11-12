using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.Migrations.Model;
using System.Data.Entity.SqlServer;

namespace ServiceLibrary.DbModels.Helpers
{
    internal class DefaultValueSqlServerMigrationSqlGenerator : System.Data.Entity.SqlServer.SqlServerMigrationSqlGenerator
    {
        protected override void Generate(AddColumnOperation addColumnOperation)
        {
            SetAnnotatedColumn(addColumnOperation.Column, addColumnOperation.Table);

            base.Generate(addColumnOperation);
        }

        protected override void Generate(AlterColumnOperation alterColumnOperation)
        {
            SetAnnotatedColumn(alterColumnOperation.Column, alterColumnOperation.Table);

            base.Generate(alterColumnOperation);
        }

        protected override void Generate(CreateTableOperation createTableOperation)
        {
            SetAnnotatedColumns(createTableOperation.Columns, createTableOperation.Name);

            base.Generate(createTableOperation);
        }

        protected override void Generate(AlterTableOperation alterTableOperation)
        {
            SetAnnotatedColumns(alterTableOperation.Columns, alterTableOperation.Name);

            base.Generate(alterTableOperation);
        }


        private void SetAnnotatedColumn(ColumnModel column, string tableFullName)
        {
            AnnotationValues values;
            if (column.Annotations.TryGetValue("SqlDefaultValue", out values))
            {
                column.DefaultValueSql = (string)values.NewValue;
            }
        }

        private void SetAnnotatedColumns(IEnumerable<ColumnModel> columns, string tableName)
        {
            foreach (ColumnModel column in columns)
            {
                SetAnnotatedColumn(column, tableName);
            }
        }
    }
}
