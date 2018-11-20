namespace ServiceLibrary.DbModels.Log.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Start : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Log.CallSides",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CallerSide = c.String(nullable: false, maxLength: 128),
                        LineNumber = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id, clustered: false) // changed
                .Index(t => new { t.CallerSide, t.LineNumber }, unique: true, clustered: true, name: "IX_CallSides_UniqueClusteredPair_MethodAndLine");
            
            CreateTable(
                "Log.LogRecords",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TimeStamp = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        Level = c.Byte(nullable: false),
                        Logger = c.String(nullable: false, maxLength: 64),
                        SourceId = c.Int(),
                        CallSideId = c.Int(),
                        MessageId = c.Int(),
                        ExceptionId = c.Int(),
                        JsonId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("Log.CallSides", t => t.CallSideId)
                .ForeignKey("Log.JsonObjects", t => t.ExceptionId)
                .ForeignKey("Log.JsonObjects", t => t.JsonId)
                .ForeignKey("Log.Messages", t => t.MessageId)
                .ForeignKey("Log.Sources", t => t.SourceId)
                .Index(t => t.SourceId)
                .Index(t => t.CallSideId)
                .Index(t => t.MessageId)
                .Index(t => t.ExceptionId)
                .Index(t => t.JsonId);
            
            CreateTable(
                "Log.Contexts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LogId = c.Int(),
                        ThreadId = c.Int(nullable: false),
                        StackTrace = c.String(maxLength: 200),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("Log.LogRecords", t => t.LogId, cascadeDelete: true) // changed
                .Index(t => t.LogId);
            
            CreateTable(
                "Log.JsonObjects",
                c => new
                    {
                        JsonId = c.Int(nullable: false, identity: true),
                        LanguageCode = c.Short(),
                        Json = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.JsonId);
            
            CreateTable(
                "Log.Messages",
                c => new
                    {
                        MessageId = c.Int(nullable: false, identity: true),
                        LanguageCode = c.Short(),
                        Text = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.MessageId);
            
            CreateTable(
                "Log.Sources",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Creation = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        Address = c.String(maxLength: 80),
                        MachineName = c.String(maxLength: 50),
                        WindowsId = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Address, name: "IX_Sources_Address");
            
        }
        
        public override void Down()
        {
            DropForeignKey("Log.LogRecords", "SourceId", "Log.Sources");
            DropForeignKey("Log.LogRecords", "MessageId", "Log.Messages");
            DropForeignKey("Log.LogRecords", "JsonId", "Log.JsonObjects");
            DropForeignKey("Log.LogRecords", "ExceptionId", "Log.JsonObjects");
            DropForeignKey("Log.Contexts", "LogId", "Log.LogRecords");
            DropForeignKey("Log.LogRecords", "CallSideId", "Log.CallSides");
            DropIndex("Log.Sources", "IX_Sources_Address");
            DropIndex("Log.Contexts", new[] { "LogId" });
            DropIndex("Log.LogRecords", new[] { "JsonId" });
            DropIndex("Log.LogRecords", new[] { "ExceptionId" });
            DropIndex("Log.LogRecords", new[] { "MessageId" });
            DropIndex("Log.LogRecords", new[] { "CallSideId" });
            DropIndex("Log.LogRecords", new[] { "SourceId" });
            DropIndex("Log.CallSides", "IX_CallSides_UniqueClusteredPair_MethodAndLine");
            DropTable("Log.Sources");
            DropTable("Log.Messages");
            DropTable("Log.JsonObjects");
            DropTable("Log.Contexts");
            DropTable("Log.LogRecords");
            DropTable("Log.CallSides");
        }
    }
}
