using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ServiceLibrary.DependencyInjection;

namespace ServiceLibrary.UnitTests
{
    public static class ContextProvider
    {
        private static readonly ContextProviderCache<ICloneable> _cache = new ContextProviderCache<ICloneable>()
        {
            {
                typeof(DbModels.Log.LogDbContext).Name,
                ContextProviderManufacture.Create((cf, cs) => new DbModels.Log.LogDbContext(cf.CreateConnection(cs)),
                                                  () => new System.Data.Entity.Infrastructure.LocalDbConnectionFactory(ContextProviderManufacture.LocalDbVersion),
                                                  () => System.Configuration.ConfigurationManager.ConnectionStrings["RelativeDefaultConnectionString2"].ConnectionString)
            },
            //{
            //    typeof(UnitTests.UnitTestDbContext).Name,
            //    ContextProviderManufacture.Create((cf, cs) => new UnitTests.UnitTestDbContext(cf.CreateConnection(cs)),
            //                                      () => new System.Data.Entity.Infrastructure.LocalDbConnectionFactory(ContextProviderManufacture.LocalDbVersion),
            //                                      () => System.Configuration.ConfigurationManager.ConnectionStrings["RelativeDefaultConnectionString2"].ConnectionString)
            //},
        };


        public static ContextProviderCache<ICloneable> Prototypes
        {
            get { return _cache; }
        }
    }
}
