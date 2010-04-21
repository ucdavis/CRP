using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using Iesi.Collections.Generic;
using UCDArch.Core.DomainModel;
using System.Collections.Generic;

namespace CRP.ModelBinder
{
    public class CustomModelBinder : UCDArch.Web.ModelBinder.UCDArchModelBinder
    {
        private const string ID_PROPERTY_NAME = "Id";

        protected override void SetProperty(System.Web.Mvc.ControllerContext controllerContext, System.Web.Mvc.ModelBindingContext bindingContext, System.ComponentModel.PropertyDescriptor propertyDescriptor, object value)
        {
            SetIdProperty(bindingContext, propertyDescriptor, value);
            SetEntityCollectionProperty(bindingContext, propertyDescriptor, value);
        }

        /// <summary>
        /// If the property being bound is an Id property, then use reflection to get past the 
        /// protected visibility of the Id property, accordingly.
        /// </summary>
        private void SetIdProperty(ModelBindingContext bindingContext,
            PropertyDescriptor propertyDescriptor, object value)
        {

            if (propertyDescriptor.Name == ID_PROPERTY_NAME && value != null)
            {
                Type idType = propertyDescriptor.PropertyType;
                object typedId = Convert.ChangeType(value, idType);

                // First, look to see if there's an Id property declared on the entity itself; 
                // e.g., using the new keyword
                PropertyInfo idProperty = bindingContext.ModelType
                    .GetProperty(propertyDescriptor.Name,
                        BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

                // If an Id property wasn't found on the entity, then grab the Id property from
                // the entity base class
                if (idProperty == null)
                {
                    idProperty = bindingContext.ModelType
                        .GetProperty(propertyDescriptor.Name,
                            BindingFlags.Public | BindingFlags.Instance);
                }

                // Set the value of the protected Id property
                idProperty.SetValue(bindingContext.Model, typedId, null);
            }
        }

        /// <summary>
        /// If the property being bound is a simple, generic collection of entiy objects, then use 
        /// reflection to get past the protected visibility of the collection property, if necessary.
        /// </summary>
        private void SetEntityCollectionProperty(ModelBindingContext bindingContext,
            PropertyDescriptor propertyDescriptor, object value)
        {

            if (value as IEnumerable != null &&
                IsSimpleGenericBindableEntityCollection(propertyDescriptor.PropertyType))
            {

                object entityCollection = propertyDescriptor.GetValue(bindingContext.Model);
                Type entityCollectionType = entityCollection.GetType();

                //foreach (object entity in (value as IEnumerable))
                //{
                //    entityCollectionType.InvokeMember("Add",
                //        BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod, null, entityCollection,
                //        new object[] { entity });
                //}
            }
        }

        private bool IsSimpleGenericBindableEntityCollection(Type propertyType)
        {
            bool isSimpleGenericBindableCollection =
                propertyType.IsGenericType &&
                (propertyType.GetGenericTypeDefinition() == typeof(IList<>) ||
                 propertyType.GetGenericTypeDefinition() == typeof(ICollection<>) ||
                 propertyType.GetGenericTypeDefinition() == typeof(ISet<>));

            bool isSimpleGenericBindableEntityCollection =
                isSimpleGenericBindableCollection && IsEntityType(propertyType.GetGenericArguments().First());

            return isSimpleGenericBindableEntityCollection;
        }

        private bool IsEntityType(Type propertyType)
        {
            bool isEntityType = propertyType.GetInterfaces()
                .Any(type => type.IsGenericType &&
                    type.GetGenericTypeDefinition() == typeof(IDomainObjectWithTypedId<>));

            return isEntityType;
        }

    }
}