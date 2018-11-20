namespace ServiceLibrary.DbModels.Log.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class ManualInitialization : DbMigration
    {
        public override void Up()
        {
            Sql(DbModels.Log.Sql.CreateResources.fn_select_eventlog);
            Sql(DbModels.Log.Sql.CreateResources.fn_select_eventlog_context);
            Sql(DbModels.Log.Sql.CreateResources.usp_insert_eventlog);
            Sql(DbModels.Log.Sql.CreateResources.usp_insert_eventlog_context);
            Sql(DbModels.Log.Sql.CreateResources.vw_sourceaddresses);
        }

        public override void Down()
        {
            Sql(DbModels.Log.Sql.DropResources.fn_select_eventlog);
            Sql(DbModels.Log.Sql.DropResources.fn_select_eventlog_context);
            Sql(DbModels.Log.Sql.DropResources.usp_insert_eventlog);
            Sql(DbModels.Log.Sql.DropResources.usp_insert_eventlog_context);
            Sql(DbModels.Log.Sql.DropResources.vw_sourceaddresses);
        }
    }
}
