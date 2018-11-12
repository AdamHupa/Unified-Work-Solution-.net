using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLibrary.DependencyInjection
{
    public class CompactContextProvider<DbContext> : IInstanceProvider<DbContext>
        where DbContext : System.Data.Entity.DbContext, new()
    {
        private Func<string, DbContext> _factory = null;
        private string _nameOrConnectionString = null;


        /// <exception cref="ArgumentNullException">thrown, if factory is null.</exception>
        public CompactContextProvider(Func<string, DbContext> factory)
        {
            if (factory == null)
                throw new ArgumentNullException("factory"); // nameof()

            _factory = factory;
        }

        /// <exception cref="ArgumentNullException">thrown, if factory is null.</exception>
        /// <exception cref="ArgumentException">thrown, if nameOrConnectionString is null or white space.</exception>
        public CompactContextProvider(Func<string, DbContext> factory, string nameOrConnectionString)
        {
            if (factory == null)
                throw new ArgumentNullException("factory"); // nameof()

            if (String.IsNullOrWhiteSpace(nameOrConnectionString))
                throw new ArgumentException("nameOrConnectionString"); // nameof()

            _factory = factory;
            _nameOrConnectionString = nameOrConnectionString;
        }


        internal string NameOrConnectionString
        {
            get { return _nameOrConnectionString; }
            set { _nameOrConnectionString = value; }
        }


        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public DbContext GetInstance()
        {
            return _factory(_nameOrConnectionString);
        }
    }
}
