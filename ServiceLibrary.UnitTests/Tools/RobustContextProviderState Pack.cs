using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ServiceLibrary.DbModels.Log;
using ServiceLibrary.DependencyInjection;
using System.Data.Entity.Infrastructure;

namespace ServiceLibrary.UnitTests.Tools
{
    public class LogContextProviderState : RobustContextProviderState<LogDbContext>
    {

    }

    //public class UnitTestContextProviderState : RobustContextProviderState<UnitTestDbContext>
    //{
    //
    //}
}
