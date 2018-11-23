using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ServiceLibrary.DbModels.Log;
using ServiceLibrary.Services;

namespace ServiceLibrary.DependencyInjection
{
    public static class ObjectFactory
    {
        private static readonly ContextProviderManufacture _contextProviderManufacture = new ContextProviderManufacture()
        {
            ContextProviderPrototypes = new ContextProviderCache<ICloneable>()
            {
                {
                    typeof(DbModels.Log.LogDbContext).Name,
                    ContextProviderManufacture.Create(cs => new DbModels.Log.LogDbContext(cs),
                                                      Properties.Settings.Default.RelativeDefaultConnectionString) // complete string
                },
            }
        };

        private static string _activeConnectionStringName = null;


        static ObjectFactory()
        {
            DLLStartupObject.Initialize();

            string name = AppDomain.CurrentDomain.GetData("ActiveConnectionStringName") as string;
            if (String.IsNullOrWhiteSpace(name) == false)
            {
                System.Configuration.ConnectionStringSettings connectionStringSettings =
                    System.Configuration.ConfigurationManager.ConnectionStrings[name];

                if (connectionStringSettings != null && String.IsNullOrWhiteSpace(connectionStringSettings.ConnectionString) == false)
                    _activeConnectionStringName = name;
            }
        }


        public static object GetInstance(Type type)
        {
            object instance = null;

            if (type == typeof(LogReceiverService))
            {
                CompactContextProvider<LogDbContext> provider;

                if (_activeConnectionStringName == null)
                {
                    provider = _contextProviderManufacture.Manufacture(typeof(LogDbContext).Name) as CompactContextProvider<LogDbContext>;
                }
                else
                {
                    provider = _contextProviderManufacture.Manufacture<CompactContextProvider<LogDbContext>, LogDbContext>(
                        typeof(LogDbContext).Name, context => context.NameOrConnectionString = _activeConnectionStringName);
                }

                instance = (provider == null) ? null : new LogReceiverService(new ServiceLogics.LogServiceLogic(provider));
            }
            else if (type == typeof(DependencyInjection.IInstanceProvider<DbModels.Log.LogDbContext>)) // for global logger
            {
                CompactContextProvider<LogDbContext> provider;

                if (_activeConnectionStringName == null)
                {
                    provider = _contextProviderManufacture.Manufacture(typeof(LogDbContext).Name) as CompactContextProvider<LogDbContext>;
                }
                else
                {
                    provider = _contextProviderManufacture.Manufacture<CompactContextProvider<LogDbContext>, LogDbContext>(
                        typeof(LogDbContext).Name, context => context.NameOrConnectionString = _activeConnectionStringName);
                }

                instance = provider;
            }

            return instance;
        }
    }
}
