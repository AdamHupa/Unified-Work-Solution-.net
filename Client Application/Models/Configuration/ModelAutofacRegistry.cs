using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_Application.Models.Configuration
{
    using Autofac;
    using Models;


    public class ModelAutofacRegistry : Autofac.Module
    {
        protected override void Load(Autofac.ContainerBuilder builder)
        {
            


            base.Load(builder);
        }
    }
}
