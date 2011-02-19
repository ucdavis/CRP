using System.ComponentModel;
using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Collections;
using System.Web.Mvc;
using Iesi.Collections.Generic;
using Microsoft.Practices.ServiceLocation;
using UCDArch.Core.CommonValidator;
using UCDArch.Core.DomainModel;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using UCDArch.Web.Validator;

namespace CRP.ModelBinder
{
    /*
    public class CustomModelBinder : DefaultModelBinder
    {
        private readonly bool _autoValidate;

        public CustomModelBinder() : this(false) { }

        /// <summary>
        /// Constructs a new ucd Arch model binder for entity-level support with model binding
        /// </summary>
        /// <param name="autoValidate">true if you want to auto-validate properties during binding (Errors goes into ModelState)</param>
        public CustomModelBinder(bool autoValidate)
        {
            _autoValidate = autoValidate;
        }

        /// <summary>
        /// After the model is updated, there may be a number of ModelState errors added by ASP.NET MVC for 
        /// and data casting problems that it runs into while binding the object.  This gets rid of those
        /// casting errors and uses the registered IValidator to populate the ModelState with any validation
        /// errors.
        /// </summary>
        protected override void OnModelUpdated(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            if (_autoValidate) //Only transfer errors to model state if we are using auto validation
            {
                foreach (string key in bindingContext.ModelState.Keys)
                {
                    for (int i = 0; i < bindingContext.ModelState[key].Errors.Count; i++)
                    {
                        ModelError modelError = bindingContext.ModelState[key].Errors[i];

                        // Get rid of all the MVC errors except those associated with parsing info; e.g., parsing DateTime fields
                        if (IsModelErrorAddedByMvc(modelError) && !IsMvcModelBinderFormatException(modelError))
                        {
                            bindingContext.ModelState[key].Errors.RemoveAt(i);
                            // Decrement the counter since we've shortened the list
                            i--;
                        }
                    }
                }

                // Transfer any errors exposed by IValidator to the ModelState
                if (bindingContext.Model is IValidatable)
                {
                    MvcValidationAdapter.TransferValidationMessagesTo(
                        bindingContext.ModelName, bindingContext.ModelState,
                        ((IValidatable)bindingContext.Model).ValidationResults());
                }
            }
        }

        protected override void BindProperty(ControllerContext controllerContext,
                                             ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor)
        {

            Type propertyType = propertyDescriptor.PropertyType;

            if (IsEntityType(propertyType))
            {
                ReplaceValueProviderWithCustomValueProvider(bindingContext, propertyDescriptor,
                                                            propertyType, typeof(EntityValueProviderResult));
            }
            else if (IsSimpleGenericBindableEntityCollection(propertyType))
            {
                ReplaceValueProviderWithCustomValueProvider(bindingContext, propertyDescriptor,
                                                            propertyType, typeof(EntityCollectionValueProviderResult));
            }

            base.BindProperty(controllerContext, bindingContext, propertyDescriptor);
        }

        /// <summary>
        /// The base implementation of this uses IDataErrorInfo to check for validation errors and 
        /// adds them to the ModelState. This override prevents that from occurring by doing nothing at all.
        /// </summary>
        protected override void OnPropertyValidated(ControllerContext controllerContext,
                                                    ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor, object value)
        {
        }

        /// <summary>
        /// Uses the default implementation to get the model properties to be updated and adds in the "Id" property if available
        /// </summary>
        protected override PropertyDescriptorCollection GetModelProperties(
            ControllerContext controllerContext, ModelBindingContext bindingContext)
        {

            PropertyDescriptorCollection modelProperties = base.GetModelProperties(controllerContext, bindingContext);

            AddIdPropertyIfAvailableTo(modelProperties, bindingContext);

            return modelProperties;
        }

        protected override void SetProperty(ControllerContext controllerContext,
                                            ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor, object value)
        {

            SetIdProperty(bindingContext, propertyDescriptor, value);
            SetEntityCollectionProperty(bindingContext, propertyDescriptor, value);

            base.SetProperty(controllerContext, bindingContext, propertyDescriptor, value);
        }

        /// <summary>
        /// The base implementatoin of this looks to see if a property value provided via a form is 
        /// bindable to the property and adds an error to the ModelState if it's not.  For example, if 
        /// a text box is left blank and the binding property is of type int, then the base implementation
        /// will add an error with the message "A value is required." to the ModelState.  We don't want 
        /// this to occur as we want these type of validation problems to be verified by our business rules.
        /// </summary>
        protected override bool OnPropertyValidating(ControllerContext controllerContext,
                                                     ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor, object value)
        {

            return true;
        }

        private bool IsModelErrorAddedByMvc(ModelError modelError)
        {
            return modelError.Exception != null &&
                   modelError.Exception.GetType().Equals(typeof(InvalidOperationException));
        }

        private bool IsMvcModelBinderFormatException(ModelError modelError)
        {
            return modelError.Exception != null &&
                   modelError.Exception.InnerException != null &&
                   modelError.Exception.InnerException.GetType().Equals(typeof(FormatException));
        }

        private bool IsEntityType(Type propertyType)
        {
            bool isEntityType = propertyType.GetInterfaces()
                .Any(type => type.IsGenericType &&
                             type.GetGenericTypeDefinition() == typeof(IDomainObjectWithTypedId<>));

            return isEntityType;
        }

        private bool IsSimpleGenericBindableEntityCollection(Type propertyType)
        {
            bool isSimpleGenericBindableCollection =
                propertyType.IsGenericType &&
                (propertyType.GetGenericTypeDefinition() == typeof(IList<>) ||
                 propertyType.GetGenericTypeDefinition() == typeof(ICollection<>) ||
                 propertyType.GetGenericTypeDefinition() == typeof(System.Collections.Generic.ISet<>) ||
                 propertyType.GetGenericTypeDefinition() == typeof(Iesi.Collections.Generic.ISet<>));

            bool isSimpleGenericBindableEntityCollection =
                isSimpleGenericBindableCollection && IsEntityType(propertyType.GetGenericArguments().First());

            return isSimpleGenericBindableEntityCollection;
        }

        private void ReplaceValueProviderWithCustomValueProvider(ModelBindingContext bindingContext,
                                                                 PropertyDescriptor propertyDescriptor, Type propertyType, Type typeOfReplacementValueProvider)
        {

            string valueProviderKey =
                CreateSubPropertyName(bindingContext.ModelName, propertyDescriptor.Name);

            ValueProviderResult defaultResult;

            bool couldGetDefaultResult =
                bindingContext.ValueProvider.TryGetValue(valueProviderKey, out defaultResult);

            if (couldGetDefaultResult)
            {
                ValueProviderResult replacementValueProvider =
                    CreateReplacementValueProviderOf(typeOfReplacementValueProvider, propertyType, defaultResult);

                bindingContext.ValueProvider.Remove(valueProviderKey);
                bindingContext.ValueProvider.Add(valueProviderKey, replacementValueProvider);
            }
        }

        private ValueProviderResult CreateReplacementValueProviderOf(Type typeOfReplacementValueProvider,
                                                                     Type propertyType, ValueProviderResult defaultResult)
        {

            ValueProviderResult replacementValueProvider = null;

            if (typeOfReplacementValueProvider == typeof(EntityValueProviderResult))
            {
                replacementValueProvider = new EntityValueProviderResult(defaultResult, propertyType);
            }
            else if (typeOfReplacementValueProvider == typeof(EntityCollectionValueProviderResult))
            {
                replacementValueProvider = new EntityCollectionValueProviderResult(defaultResult, propertyType);
            }

            Check.Ensure(replacementValueProvider != null, "The desired value provider, " +
                                                           typeOfReplacementValueProvider.ToString() + ", does not match any custom value provider.");

            return replacementValueProvider;
        }

        private void AddIdPropertyIfAvailableTo(PropertyDescriptorCollection modelProperties, ModelBindingContext bindingContext)
        {
            PropertyDescriptor idProperty =
                (from PropertyDescriptor property in TypeDescriptor.GetProperties(bindingContext.ModelType)
                 where property.Name == ID_PROPERTY_NAME
                 select property).SingleOrDefault();

            if (idProperty != null && !modelProperties.Contains(idProperty))
            {
                modelProperties.Add(idProperty);
            }
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
                //                                      BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod, null, entityCollection,
                //                                      new object[] { entity });
                //}
            }
        }

        #region Overridable (but not yet overridden) Methods

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            return base.BindModel(controllerContext, bindingContext);
        }

        protected override object CreateModel(ControllerContext controllerContext,
                                              ModelBindingContext bindingContext, Type modelType)
        {

            return base.CreateModel(controllerContext, bindingContext, modelType);
        }

        protected override bool OnModelUpdating(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            return base.OnModelUpdating(controllerContext, bindingContext);
        }

        #endregion

        private const string ID_PROPERTY_NAME = "Id";
    }

    internal class EntityCollectionValueProviderResult : ValueProviderResult
    {
        public EntityCollectionValueProviderResult(ValueProviderResult result, Type collectionType)
            : this(result.RawValue, result.AttemptedValue, result.Culture, collectionType)
        {
        }

        /// <param name="collectionType">Is assumed to be a simple, generic colleciton of entity objects</param>
        public EntityCollectionValueProviderResult(object rawValue, string attemptedValue, CultureInfo culture, Type collectionType)
            : base(rawValue, attemptedValue, culture)
        {
            Check.Require(collectionType != null, "propertyType may not be null");

            this.collectionType = collectionType;
            this.collectionEntityType = collectionType.GetGenericArguments().First();
        }

        public override object ConvertTo(Type type, CultureInfo culture)
        {
            Check.Require(type != null, "type may not be null");
            Check.Require(RawValue as string[] != null,
                          "The EntityCollectionValueProviderResult can only work with a RawValue of type string[]; the RawValue was " +
                          (RawValue != null ? RawValue.ToString() : "null"));

            int countOfEntityIds = (RawValue as string[]).Length;
            Array entities = Array.CreateInstance(collectionEntityType, countOfEntityIds);

            Type entityInterfaceType = collectionEntityType.GetInterfaces()
                .First(interfaceType => interfaceType.IsGenericType
                                        && interfaceType.GetGenericTypeDefinition() == typeof(IDomainObjectWithTypedId<>));

            Type idType = entityInterfaceType.GetGenericArguments().First();

            for (int i = 0; i < countOfEntityIds; i++)
            {
                string rawId = (RawValue as string[])[i];

                if (string.IsNullOrEmpty(rawId))
                    return null;

                object typedId = Convert.ChangeType(rawId, idType);

                object entity = GetEntityFor(typedId, idType);
                entities.SetValue(entity, i);
            }

            return entities;
        }

        private object GetEntityFor(object typedId, Type idType)
        {
            object entityRepository = GenericRepositoryFactory.CreateEntityRepositoryFor(collectionEntityType, idType);

            return entityRepository.GetType()
                .InvokeMember("GetByID", BindingFlags.InvokeMethod, null, entityRepository, new[] { typedId });
        }

        private readonly Type collectionType;
        private readonly Type collectionEntityType;
    }

    internal class EntityValueProviderResult : ValueProviderResult
    {
        public EntityValueProviderResult(ValueProviderResult result, Type propertyType)
            : this(result.RawValue, result.AttemptedValue, result.Culture, propertyType)
        {
        }

        public EntityValueProviderResult(object rawValue, string attemptedValue, CultureInfo culture, Type propertyType)
            : base(rawValue, attemptedValue, culture)
        {
            Check.Require(propertyType != null, "propertyType may not be null");

            this.propertyType = propertyType;
        }

        public override object ConvertTo(Type type, CultureInfo culture)
        {
            Check.Require(type != null, "type may not be null");
            Check.Require(RawValue as string[] != null && (RawValue as string[]).Length == 1,
                          "The EntityValueProviderResult can only work with a RawValue of type string[1]; the RawValue was " +
                          (RawValue != null ? RawValue.ToString() : "null"));

            Type entityInterfaceType = propertyType.GetInterfaces()
                .First(interfaceType => interfaceType.IsGenericType
                                        && interfaceType.GetGenericTypeDefinition() == typeof(IDomainObjectWithTypedId<>));

            Type idType = entityInterfaceType.GetGenericArguments().First();
            string rawId = (RawValue as string[]).First();

            if (string.IsNullOrEmpty(rawId))
                return null;


            try
            {
                object typedId =
                    (idType == typeof(Guid))
                        ? new Guid(rawId)
                        : Convert.ChangeType(rawId, idType);

                return GetEntityFor(typedId, idType);
            }
                // If the Id conversion failed for any reason, just return null
            catch (Exception)
            {
                return null;
            }
        }

        private object GetEntityFor(object typedId, Type idType)
        {
            object entityRepository = GenericRepositoryFactory.CreateEntityRepositoryFor(propertyType, idType);

            return entityRepository.GetType()
                .InvokeMember("GetByID", BindingFlags.InvokeMethod, null, entityRepository, new[] { typedId });
        }

        private Type propertyType;
    }

    internal class GenericRepositoryFactory
    {
        public static object CreateEntityRepositoryFor(Type entityType, Type idType)
        {
            Type genericRepositoryType = typeof(IRepositoryWithTypedId<,>);
            Type concreteRepositoryType =
                genericRepositoryType.MakeGenericType(new[] { entityType, idType });

            object repository;

            try
            {
                repository = ServiceLocator.Current.GetService(concreteRepositoryType);
            }
            catch (NullReferenceException)
            {
                throw new NullReferenceException("ServiceLocator has not been initialized; " +
                                                 "I was trying to retrieve " + concreteRepositoryType);
            }
            catch (ActivationException)
            {
                throw new ActivationException("The needed dependency of type " + concreteRepositoryType.Name +
                                              " could not be located with the ServiceLocator. You'll need to register it with " +
                                              "the Common Service Locator (CSL) via your IoC's CSL adapter.");
            }

            return repository;
        }
    }
     */
}