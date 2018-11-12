using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Reflection;

// link: https://gist.github.com/tystol/20b07bd4e0043d43faff

namespace ServiceLibrary.DbModels.Helpers
{
    public class CascadeDeleteAttributeConvention : IConceptualModelConvention<AssociationType> //, IConvention
    {
        private static readonly Func<AssociationType, bool> _isSelfReferencing;
        private static readonly Func<AssociationType, bool> _isRequiredToMany;
        private static readonly Func<AssociationType, bool> _isManyToRequired;
        private static readonly Func<AssociationType, object> _getConfiguration;
        private static readonly Func<object, OperationAction?> _navigationPropertyConfigurationGetDeleteAction;


        static CascadeDeleteAttributeConvention()
        {
            Type associationTypeExtensionsType = typeof(AssociationType).Assembly.GetType(
                "System.Data.Entity.ModelConfiguration.Edm.AssociationTypeExtensions"); // internal class

            MethodInfo isSelfRefencingMethod =
                associationTypeExtensionsType.GetMethod("IsSelfReferencing", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
            MethodInfo isRequiredToManyMethod =
                associationTypeExtensionsType.GetMethod("IsRequiredToMany", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
            MethodInfo isManyToRequiredMethod =
                associationTypeExtensionsType.GetMethod("IsManyToRequired", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
            MethodInfo getConfigurationMethod =
                associationTypeExtensionsType.GetMethod("GetConfiguration", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);

            _isSelfReferencing = associationType => (bool)isSelfRefencingMethod.Invoke(null, new object[] { associationType });
            _isRequiredToMany = associationType => (bool)isRequiredToManyMethod.Invoke(null, new object[] { associationType });
            _isManyToRequired = associationType => (bool)isManyToRequiredMethod.Invoke(null, new object[] { associationType });
            _getConfiguration = associationType => getConfigurationMethod.Invoke(null, new object[] { associationType });


            Type navigationPropertyConfigurationType = typeof(AssociationType).Assembly.GetType(
                "System.Data.Entity.ModelConfiguration.Configuration.Properties.Navigation.NavigationPropertyConfiguration"); // internal class

            PropertyInfo deleteActionProperty =
                navigationPropertyConfigurationType.GetProperty("DeleteAction", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

            _navigationPropertyConfigurationGetDeleteAction = navigationProperty => (OperationAction?)deleteActionProperty.GetValue(navigationProperty);
        }


        #region IConceptualModelConvention

        public virtual void Apply(AssociationType item, System.Data.Entity.Infrastructure.DbModel model)
        {
            if (_isSelfReferencing(item))
                return;

            var propertyConfiguration = _getConfiguration(item);
            if (propertyConfiguration != null && _navigationPropertyConfigurationGetDeleteAction(propertyConfiguration).HasValue)
                return;

            AssociationEndMember collectionEndMember = null;
            AssociationEndMember singleNavigationEndMember = null;

            if (_isRequiredToMany(item))
            {
                collectionEndMember = GetSourceEnd(item);
                singleNavigationEndMember = GetTargetEnd(item);
            }
            else if (_isManyToRequired(item))
            {
                collectionEndMember = GetTargetEnd(item);
                singleNavigationEndMember = GetSourceEnd(item);
            }

            if (collectionEndMember == null || singleNavigationEndMember == null)
                return;

            var collectionCascadeDeleteAttribute = GetCascadeDeleteAttribute(collectionEndMember);
            var singleCascadeDeleteAttribute = GetCascadeDeleteAttribute(singleNavigationEndMember);

            // TODO

            //if (collectionCascadeDeleteAttribute != null || singleCascadeDeleteAttribute != null)
            //    collectionEndMember.DeleteBehavior = OperationAction.Cascade;

            if (collectionCascadeDeleteAttribute != null)
            {
                collectionEndMember.DeleteBehavior = collectionCascadeDeleteAttribute.CascadeDelete ? OperationAction.Cascade : OperationAction.None;
                //singleNavigationEndMember.DeleteBehavior = collectionCascadeDeleteAttribute.CascadeDelete ? OperationAction.Cascade : OperationAction.None;
            }
            else if (singleCascadeDeleteAttribute != null)
            {
                //collectionEndMember.DeleteBehavior = singleCascadeDeleteAttribute.CascadeDelete ? OperationAction.Cascade : OperationAction.None;
                singleNavigationEndMember.DeleteBehavior = singleCascadeDeleteAttribute.CascadeDelete ? OperationAction.Cascade : OperationAction.None;
            }
        }

        #endregion

        private static CascadeDeleteAttribute GetCascadeDeleteAttribute(EdmMember edmMember)
        {
            MetadataProperty clrProperties = edmMember.MetadataProperties.FirstOrDefault(m => m.Name == "ClrPropertyInfo");
            if (clrProperties == null)
                return null;

            PropertyInfo property = clrProperties.Value as PropertyInfo;
            if (property == null)
                return null;

            return property.GetCustomAttribute<CascadeDeleteAttribute>();
        }

        private static AssociationEndMember GetSourceEnd(AssociationType item)
        {
            return item.KeyMembers.FirstOrDefault() as AssociationEndMember;
        }

        private static AssociationEndMember GetTargetEnd(AssociationType item)
        {
            return item.KeyMembers.ElementAtOrDefault(1) as AssociationEndMember;
        }
    }
}
