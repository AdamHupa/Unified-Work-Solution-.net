using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.SqlClient;
using ServiceLibrary.DbModels.Log.CodeFirst;

// Enable-Migrations -ContextTypeName ServiceLibrary.DbModels.Log.LogDbContext -MigrationsDirectory DbModels\Log\Migrations
// Add-Migration -Configuration ServiceLibrary.DbModels.Log.Migrations.Configuration Start
// Add-Migration -Configuration ServiceLibrary.DbModels.Log.Migrations.Configuration -IgnoreChanges ManualInitialization
// Update-Database -Configuration ServiceLibrary.DbModels.Log.Migrations.Configuration –TargetMigration 201806171434136_ManualInitialization

namespace ServiceLibrary.DbModels.Log
{
    public class LogDbContext : System.Data.Entity.DbContext
    {
        public const string DefaultSchema = "Log";


        static LogDbContext()
        {
            Database.SetInitializer<LogDbContext>(new CreateDatabaseIfNotExists<LogDbContext>());
        }

        public LogDbContext()
            : base("")
        {
            throw new NotImplementedException();
        }
        //: base(ServiceLibrary.Properties.Settings.Default.RelativeDefaultConnectionString) { }

        public LogDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString) { }

        /// <remarks>Intended for Unit Testing.</remarks>
        public LogDbContext(System.Data.Common.DbConnection connection)
            : base(connection, true) // false for in transaction context
        {
            //this.Configuration.LazyLoadingEnabled = false;
            //this.Configuration.ProxyCreationEnabled = false;
        }


        public DbSet<CallSide> CallSides { get; set; }
        public DbSet<Context> Contexts { get; set; }
        public DbSet<JsonObject> JsonObjects { get; set; }
        public DbSet<LogRecord> LogRecords { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Source> Sources { get; set; }
        //public DbSet<> s { get; set; }


        /// <remarks>Ensure that Source table's row would not be modified.</remarks>
        public override int SaveChanges()
        {
            foreach (DbEntityEntry entity in this.ChangeTracker.Entries())
            {
                if (entity.State == EntityState.Modified && entity.Entity.GetType() == typeof(Source))
                    entity.State = EntityState.Unchanged;
            }

            return base.SaveChanges();
        }


        /// <remarks>Ensure that this method will be only run once. Read remarks from base method.</remarks>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasDefaultSchema(DefaultSchema);

            //modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            //modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
            //modelBuilder.Conventions.Add<DbModels.Helpers.CascadeDeleteAttributeConvention>();

            //modelBuilder.Conventions.Add(new AttributeToColumnAnnotationConvention<DbModels.Helpers.DefaultValueAttribute, string>(
            //    "SqlDefaultValue", (p, attributes) => attributes.Single().SqlDefaultValue));
        }
    }
}
