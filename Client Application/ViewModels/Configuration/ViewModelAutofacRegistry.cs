using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_Application.ViewModels.Configuration
{
    using Autofac;
    using ViewModels;


    public class ViewModelAutofacRegistry : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<MainWindowViewModel>()
                   .AsSelf()
                   .InstancePerDependency();


            base.Load(builder);
        }
    }
}
