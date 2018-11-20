namespace ServiceLibrary.DbModels.Log.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<ServiceLibrary.DbModels.Log.LogDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            MigrationsDirectory = @"DbModels\Log\Migrations";


            SetSqlGenerator("System.Data.SqlClient", new DbModels.Log.Helpers.CustomSqlServerMigrationSqlGenerator());
        }

        protected override void Seed(ServiceLibrary.DbModels.Log.LogDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.
        }
    }
}
