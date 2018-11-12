using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLibrary.DependencyInjection
{
    public interface IInstanceProvider<InstanceType> : System.ICloneable where InstanceType : class, new()
    {
        InstanceType GetInstance();
    }
}
