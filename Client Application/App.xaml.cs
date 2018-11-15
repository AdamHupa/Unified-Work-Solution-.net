using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Client_Application
{
    using Autofac;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public const string GlobalAutofacRegistry = "Global AutoFac Registry";
        

        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                ContainerBuilder containerBuilder = new ContainerBuilder();

                containerBuilder.RegisterModule<Models.Configuration.ModelAutofacRegistry>();
                containerBuilder.RegisterModule<ViewModels.Configuration.ViewModelAutofacRegistry>();
                //containerBuilder.RegisterAssemblyModules(System.Reflection.Assembly.GetExecutingAssembly()).PublicOnly();

                IContainer autofacRegistry = containerBuilder.Build(Autofac.Builder.ContainerBuildOptions.None);

                if (autofacRegistry != null)
                    App.Current.Resources.Add(GlobalAutofacRegistry, autofacRegistry);
                    //AppDomain.CurrentDomain.SetData(GlobalAutofacRegistry, autofacRegistry);
                    //Application.Current.Resources.Add(GlobalAutofacRegistry, autofacRegistry);
            
                
            }
            catch (Exception ex)
            {
                

                this.Shutdown(13);
            }

            base.OnStartup(e);
        }
    }
}
