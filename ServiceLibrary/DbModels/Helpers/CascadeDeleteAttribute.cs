using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
    link: https://gist.github.com/tystol/20b07bd4e0043d43faff
    add the following lines to OnModelCreating the overridden method of classes inheriting DbContext class:
    
    modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();  // optional
    modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>(); // optional
    modelBuilder.Conventions.Add<CascadeDeleteAttributeConvention>();
 */

namespace ServiceLibrary.DbModels.Helpers
{
    /// <summary>
    /// Apply on properties representing entities referencing this entity, that should cascade on delete.
    /// </summary>
    /// <example>
    /// [DbModels.Helpers.CascadeDelete]
    /// public virtual ICollection<DependentEntity> DependentEntities { get; set; }
    /// </example>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class CascadeDeleteAttribute : Attribute
    {
        public bool CascadeDelete { get; set; }


        public CascadeDeleteAttribute(bool cascadeDelete = true)
        {
            CascadeDelete = cascadeDelete;
        }
    }
}
