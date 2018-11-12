using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLibrary.DependencyInjection
{
    public interface IContextProviderState<ContextProviderType, ContextType>
        where ContextProviderType : class, IInstanceProvider<ContextType>
        where ContextType : System.Data.Entity.DbContext, new()
    {
        void Handle(ContextProviderType context);
    }
}
