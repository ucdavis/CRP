using System.Collections.Generic;
using System.Linq;
using CRP.Controllers;
using CRP.Controllers.Filter;
using CRP.Controllers.ViewModels;
using CRP.Core.Domain;
using CRP.Tests.Core.Extensions;
using CRP.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.Attributes;
using MvcContrib.TestHelper;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;
using UCDArch.Web.Attributes;

namespace CRP.Tests.Controllers
{
    [TestClass]
    public class ApplicationManagementControllerTests :ControllerTestBase<ApplicationManagementController>
    {
        protected IRepository<ItemType> ItemTypeRepository { get; set; }
        protected List<ItemType> ItemTypes { get; set; }
        protected List<QuestionType> QuestionTypes { get; set; }
        protected IRepository<QuestionType> QuestionTypeRepository { get; set; }

        #region Init

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationManagementControllerTests"/> class.
        /// </summary>
        public ApplicationManagementControllerTests()
        {
            ItemTypes = new List<ItemType>();
            ItemTypeRepository = FakeRepository<ItemType>();
            Controller.Repository.Expect(a => a.OfType<ItemType>()).Return(ItemTypeRepository).Repeat.Any();

            QuestionTypes = new List<QuestionType>();            
            QuestionTypeRepository = FakeRepository<QuestionType>();
            Controller.Repository.Expect(a => a.OfType<QuestionType>()).Return(QuestionTypeRepository).Repeat.Any();

        }


        /// <summary>
        /// Registers the routes.
        /// </summary>
        protected override void RegisterRoutes()
        {
            new RouteConfigurator().RegisterRoutes();
        }

        /// <summary>
        /// Setups the controller.
        /// </summary>
        protected override void SetupController()
        {
            Controller = new TestControllerBuilder().CreateController<ApplicationManagementController>();
        }

        #endregion Init

        #region Route Tests

        /// <summary>
        /// Tests the index mapping.
        /// </summary>
        [TestMethod]
        public void TestIndexMapping()
        {
            "~/ApplicationManagement/Index".ShouldMapTo<ApplicationManagementController>(a => a.Index());
        }

        /// <summary>
        /// Tests the list item types mapping.
        /// </summary>
        [TestMethod]
        public void TestListItemTypesMapping()
        {
            "~/ApplicationManagement/ListItemTypes".ShouldMapTo<ApplicationManagementController>(a => a.ListItemTypes());
        }

        /// <summary>
        /// Tests the create item type mapping.
        /// </summary>
        [TestMethod]
        public void TestCreateItemTypeMapping()
        {
            "~/ApplicationManagement/CreateItemType".ShouldMapTo<ApplicationManagementController>(a => a.CreateItemType());
        }

        /// <summary>
        /// Tests the create item type with parameters mapping.
        /// </summary>
        [TestMethod]
        public void TestCreateItemTypeWithParametersMapping()
        {
            "~/ApplicationManagement/CreateItemType/1".ShouldMapTo<ApplicationManagementController>
                (a => a.CreateItemType(new ItemType(),new ExtendedProperty[1]),true);
        }

        /// <summary>
        /// Tests the edit item type mapping.
        /// </summary>
        [TestMethod]
        public void TestEditItemTypeMapping()
        {
            "~/ApplicationManagement/EditItemType/5".ShouldMapTo<ApplicationManagementController>
                (a => a.EditItemType(5));
        }
        /// <summary>
        /// Tests the edit item type with parameters mapping.
        /// </summary>
        [TestMethod]
        public void TestEditItemTypeWithParametersMapping()
        {
            "~/ApplicationManagement/EditItemType/".ShouldMapTo<ApplicationManagementController>
                (a => a.EditItemType(5, new ItemType()), true);
        }

        /// <summary>
        /// Tests the toggle active mapping.
        /// </summary>
        [TestMethod]
        public void TestToggleActiveMapping()
        {
            "~/ApplicationManagement/ToggleActive/5".ShouldMapTo<ApplicationManagementController>(a => a.ToggleActive(5));
        }
        #endregion Route Tests

        #region Misc Tests

        /// <summary>
        /// Tests the index returns view.
        /// </summary>
        [TestMethod]
        public void TestIndexReturnsView()
        {
            Controller.Index()
                .AssertViewRendered();
        }

        /// <summary>
        /// Tests the list item types returns view with data.
        /// </summary>
        [TestMethod]
        public void TestListItemTypesReturnsViewWithData()
        {
            FakeItemTypes(3);
            ItemTypeRepository.Expect(a => a.Queryable).Return(ItemTypes.AsQueryable()).Repeat.Any();

            var result = Controller.ListItemTypes()
                .AssertViewRendered()
                .WithViewData<IEnumerable<ItemType>>();
            Assert.AreEqual(3, result.Count());
        }
       
        #endregion Misc Tests

        #region CreateItemType Tests (Task 596)

        /// <summary>
        /// Tests the create item type return item type view model.
        /// </summary>
        [TestMethod]
        public void TestCreateItemTypeReturnItemTypeViewModel()
        {
            FakeQuestionTypes(3);
            QuestionTypes[0].ExtendedProperty = true;
            QuestionTypes[1].ExtendedProperty = false;
            QuestionTypes[2].ExtendedProperty = true;
            QuestionTypeRepository.Expect(a => a.Queryable).Return(QuestionTypes.AsQueryable()).Repeat.Any();

            var result = Controller.CreateItemType()
                .AssertViewRendered()
                .WithViewData<ItemTypeViewModel>();
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.QuestionTypes.Count());
        }

        /// <summary>
        /// Tests the create item type with valid data saves.
        /// </summary>
        [TestMethod]
        public void TestCreateItemTypeWithValidDataSaves()
        {
            FakeItemTypes(3);
            ItemTypeRepository.Expect(a => a.Queryable).Return(ItemTypes.AsQueryable()).Repeat.Any();
            var itemTypeToAdd = CreateValidEntities.ItemType(10);
            var extendedProperties = new ExtendedProperty[2];
            extendedProperties[0] = CreateValidEntities.ExtendedProperty(1);
            extendedProperties[1] = CreateValidEntities.ExtendedProperty(2);

            Assert.AreEqual(0, itemTypeToAdd.ExtendedProperties.ToList().Count);

            Controller.CreateItemType(itemTypeToAdd, extendedProperties)
                .AssertActionRedirect()
                .ToAction<ApplicationManagementController>(a => a.ListItemTypes());

            ItemTypeRepository.AssertWasCalled(a => a.EnsurePersistent(itemTypeToAdd));
            Assert.AreEqual("Item Type has been created successfully.", Controller.Message);
            Assert.IsTrue(Controller.ModelState.IsValid);

            Assert.AreEqual(2, itemTypeToAdd.ExtendedProperties.ToList().Count);
            Assert.AreSame(extendedProperties[0], itemTypeToAdd.ExtendedProperties.ToList()[0]);
            Assert.AreSame(extendedProperties[1], itemTypeToAdd.ExtendedProperties.ToList()[1]);            
        }

        /// <summary>
        /// Tests the create item type with name already existing does not save.
        /// </summary>
        [TestMethod]
        public void TestCreateItemTypeWithNameAlreadyExistingDoesNotSave()
        {
            FakeQuestionTypes(3);
            QuestionTypes[0].ExtendedProperty = true;
            QuestionTypes[1].ExtendedProperty = false;
            QuestionTypes[2].ExtendedProperty = true;
            QuestionTypeRepository.Expect(a => a.Queryable).Return(QuestionTypes.AsQueryable()).Repeat.Any();


            FakeItemTypes(3);
            ItemTypeRepository.Expect(a => a.Queryable).Return(ItemTypes.AsQueryable()).Repeat.Any();
            var itemTypeToAdd = CreateValidEntities.ItemType(10);
            itemTypeToAdd.Name = ItemTypes[1].Name;
            var extendedProperties = new ExtendedProperty[2];
            extendedProperties[0] = CreateValidEntities.ExtendedProperty(1);
            extendedProperties[1] = CreateValidEntities.ExtendedProperty(2);
            var result = Controller.CreateItemType(itemTypeToAdd, extendedProperties)
                .AssertViewRendered()
                .WithViewData<ItemTypeViewModel>();

            ItemTypeRepository.AssertWasNotCalled(a => a.EnsurePersistent(itemTypeToAdd));
            Assert.AreNotEqual("Item Type has been created successfully.", Controller.Message);
            Assert.IsFalse(Controller.ModelState.IsValid);
            Controller.ModelState.AssertErrorsAre("A item type of the same name already exists.");
            Assert.AreSame(itemTypeToAdd, result.ItemType);
            Assert.AreEqual(2, result.QuestionTypes.Count());
        }

        /// <summary>
        /// Tests the create item type with invalid extended property does not save.
        /// </summary>
        [TestMethod]
        public void TestCreateItemTypeWithInvalidExtendedPropertyDoesNotSave()
        {
            FakeQuestionTypes(3);
            QuestionTypes[0].ExtendedProperty = true;
            QuestionTypes[1].ExtendedProperty = false;
            QuestionTypes[2].ExtendedProperty = true;
            QuestionTypeRepository.Expect(a => a.Queryable).Return(QuestionTypes.AsQueryable()).Repeat.Any();

            FakeItemTypes(3);
            ItemTypeRepository.Expect(a => a.Queryable).Return(ItemTypes.AsQueryable()).Repeat.Any();
            var itemTypeToAdd = CreateValidEntities.ItemType(10);
            var extendedProperties = new ExtendedProperty[3];
            extendedProperties[0] = CreateValidEntities.ExtendedProperty(1);
            extendedProperties[1] = CreateValidEntities.ExtendedProperty(2);
            extendedProperties[1].Name = " "; //invalid
            extendedProperties[2] = CreateValidEntities.ExtendedProperty(3);
            var result = Controller.CreateItemType(itemTypeToAdd, extendedProperties)
                .AssertViewRendered()
                .WithViewData<ItemTypeViewModel>();

            ItemTypeRepository.AssertWasNotCalled(a => a.EnsurePersistent(itemTypeToAdd));
            Assert.AreNotEqual("Item Type has been created successfully.", Controller.Message);
            Assert.IsFalse(Controller.ModelState.IsValid);
            Controller.ModelState.AssertErrorsAre("At least one extended property is not valid.");
            Assert.AreEqual(2, itemTypeToAdd.ExtendedProperties.ToList().Count);
            Assert.AreSame(itemTypeToAdd, result.ItemType);
        }

        /// <summary>
        /// Tests the create item type with invalid item type name does not save.
        /// </summary>
        [TestMethod]
        public void TestCreateItemTypeWithInvalidItemTypeNameDoesNotSave()
        {
            FakeQuestionTypes(3);
            QuestionTypes[0].ExtendedProperty = true;
            QuestionTypes[1].ExtendedProperty = false;
            QuestionTypes[2].ExtendedProperty = true;
            QuestionTypeRepository.Expect(a => a.Queryable).Return(QuestionTypes.AsQueryable()).Repeat.Any();

            FakeItemTypes(3);
            ItemTypeRepository.Expect(a => a.Queryable).Return(ItemTypes.AsQueryable()).Repeat.Any();
            var itemTypeToAdd = CreateValidEntities.ItemType(10);
            itemTypeToAdd.Name = " "; //Invalid

            var extendedProperties = new ExtendedProperty[3];
            extendedProperties[0] = CreateValidEntities.ExtendedProperty(1);
            extendedProperties[1] = CreateValidEntities.ExtendedProperty(2);
            extendedProperties[2] = CreateValidEntities.ExtendedProperty(3);
            var result = Controller.CreateItemType(itemTypeToAdd, extendedProperties)
                .AssertViewRendered()
                .WithViewData<ItemTypeViewModel>();

            ItemTypeRepository.AssertWasNotCalled(a => a.EnsurePersistent(itemTypeToAdd));
            Assert.AreNotEqual("Item Type has been created successfully.", Controller.Message);
            Assert.IsFalse(Controller.ModelState.IsValid);
            Controller.ModelState.AssertErrorsAre("Name: may not be null or empty");
            Assert.AreEqual(3, itemTypeToAdd.ExtendedProperties.ToList().Count);
            Assert.AreSame(itemTypeToAdd, result.ItemType);
        }

        #endregion CreateItemType Tests (Task 596)

        #region EditItemType Tests

        /// <summary>
        /// Tests the edit item type returns view when id found.
        /// </summary>
        [TestMethod]
        public void TestEditItemTypeReturnsViewWhenIdFound()
        {
            FakeItemTypes(1);
            ItemTypeRepository.Expect(a => a.GetNullableByID(1)).Return(ItemTypes[0]).Repeat.Once();
            var result = Controller.EditItemType(1)
                .AssertViewRendered()
                .WithViewData<ItemType>();
            Assert.IsNotNull(result);
            Assert.AreSame(ItemTypes[0], result);
        }

        /// <summary>
        /// Tests the edit item type redirects to list when id not found.
        /// </summary>
        [TestMethod]
        public void TestEditItemTypeRedirectsToListWhenIdNotFound()
        {
            ItemTypeRepository.Expect(a => a.GetNullableByID(1)).Return(null).Repeat.Once();
            Controller.EditItemType(1)
                .AssertActionRedirect()
                .ToAction<ApplicationManagementController>(a => a.ListItemTypes());
        }

        /// <summary>
        /// Tests the edit item type with valid data saves.
        /// </summary>
        [TestMethod]
        public void TestEditItemTypeWithValidDataSaves()
        {
            FakeItemTypes(3);
            
            ItemTypeRepository.Expect(a => a.Queryable).Return(ItemTypes.AsQueryable()).Repeat.Any();
            ItemTypeRepository.Expect(a => a.GetNullableByID(2)).Return(ItemTypes[1]).Repeat.Any();

            var itemTypeToUpdate = ItemTypes[1];
            itemTypeToUpdate.Name = "SomeNewName";
            var result = Controller.EditItemType(2, itemTypeToUpdate)
                .AssertViewRendered()
                .WithViewData<ItemType>();
            Assert.AreSame(itemTypeToUpdate, result);
            ItemTypeRepository.AssertWasCalled(a => a.EnsurePersistent(itemTypeToUpdate));
            Assert.AreEqual("Item Type has been saved successfully.", Controller.Message);
            Assert.IsTrue(Controller.ModelState.IsValid);
        }

        /// <summary>
        /// Tests the edit item type when change name to existing name does not save.
        /// </summary>
        [TestMethod]
        public void TestEditItemTypeWhenChangeNameToExistingNameDoesNotSave()
        {
            //In addition to fixing the controller so this tests passes, review if the id should be passed and only the two fields be allowed to be changed.
            FakeItemTypes(3);

            ItemTypeRepository.Expect(a => a.Queryable).Return(ItemTypes.AsQueryable()).Repeat.Any();
            ItemTypeRepository.Expect(a => a.GetNullableByID(2)).Return(ItemTypes[1]).Repeat.Any();

            var itemTypeToUpdate = ItemTypes[1];
            itemTypeToUpdate.Name = ItemTypes[0].Name;
            var result = Controller.EditItemType(2, itemTypeToUpdate)
                .AssertViewRendered()
                .WithViewData<ItemType>();
            Assert.AreSame(itemTypeToUpdate, result);
            ItemTypeRepository.AssertWasNotCalled(a => a.EnsurePersistent(itemTypeToUpdate));
            Assert.AreNotEqual("Item Type has been saved successfully.", Controller.Message);
            Assert.IsFalse(Controller.ModelState.IsValid);
        }

        /// <summary>
        /// Tests the edit item type only allows name and is active to be changed.
        /// </summary>
        [TestMethod]
        public void TestEditItemTypeOnlyAllowsNameAndIsActiveToBeChanged()
        {
            FakeItemTypes(3);
            ItemTypes[1].AddExtendedProperty(CreateValidEntities.ExtendedProperty(1));
            ItemTypes[1].AddQuantityQuestionSet(CreateValidEntities.QuestionSet(1));

            ItemTypeRepository.Expect(a => a.Queryable).Return(ItemTypes.AsQueryable()).Repeat.Any();
            ItemTypeRepository.Expect(a => a.GetNullableByID(2)).Return(ItemTypes[1]).Repeat.Any();

            var itemTypeToUpdateWithSameId = CreateValidEntities.ItemType(2);
            itemTypeToUpdateWithSameId.SetIdTo(ItemTypes[1].Id);
            itemTypeToUpdateWithSameId.Name = "Updated";

            Assert.AreEqual(1, ItemTypes[1].ExtendedProperties.Count);
            Assert.AreEqual(1, ItemTypes[1].QuestionSets.Count);

            Assert.AreEqual(0, itemTypeToUpdateWithSameId.ExtendedProperties.Count);
            Assert.AreEqual(0, itemTypeToUpdateWithSameId.QuestionSets.Count);

            var result = Controller.EditItemType(2, itemTypeToUpdateWithSameId)
                .AssertViewRendered()
                .WithViewData<ItemType>();

            ItemTypeRepository.AssertWasCalled(a => a.EnsurePersistent(ItemTypes[1]));
            Assert.AreEqual("Item Type has been saved successfully.", Controller.Message);


            Assert.AreEqual(1, result.ExtendedProperties.Count);
            Assert.AreEqual(1, result.QuestionSets.Count);
            Assert.AreEqual(1, ItemTypes[1].QuestionSets.Count);
            Assert.AreEqual(1, ItemTypes[1].QuestionSets.Count);
        }

        /// <summary>
        /// Tests the edit item type where item type does not exist does not save.
        /// </summary>
        [TestMethod]
        public void TestEditItemTypeWhereItemTypeDoesNotExistDoesNotSave()
        {
            FakeItemTypes(3);

            ItemTypeRepository.Expect(a => a.Queryable).Return(ItemTypes.AsQueryable()).Repeat.Any();
            ItemTypeRepository.Expect(a => a.GetNullableByID(2)).Return(null).Repeat.Any(); //So It Is Not Found

            Controller.EditItemType(2, ItemTypes[1])
                .AssertActionRedirect()
                .ToAction<ApplicationManagementController>(a => a.ListItemTypes());

            ItemTypeRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<ItemType>.Is.Anything));

        }

        #endregion EditItemType Tests

        #region ToggleActive Tests

        [TestMethod]
        public void TestToggleActiveRedirectsToListWhenIdNotFound()
        {
            ItemTypeRepository.Expect(a => a.GetNullableByID(2)).Return(null).Repeat.Any(); //So It Is Not Found
            Controller.ToggleActive(2)
                .AssertActionRedirect()
                .ToAction<ApplicationManagementController>(a => a.ListItemTypes());
            ItemTypeRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<ItemType>.Is.Anything));
        }

        [TestMethod]
        public void TestToggleActiveToFalseWithFoundId()
        {
            FakeItemTypes(1);
            ItemTypeRepository.Expect(a => a.GetNullableByID(1)).Return(ItemTypes[0]).Repeat.Any();
            ItemTypes[0].IsActive = true;
            Controller.ToggleActive(1)
                .AssertActionRedirect()
                .ToAction<ApplicationManagementController>(a => a.ListItemTypes());
            Assert.IsFalse(ItemTypes[0].IsActive);
            ItemTypeRepository.AssertWasCalled(a => a.EnsurePersistent(ItemTypes[0]));
            Assert.AreEqual("Item Type has been deactivated.", Controller.Message);
        }

        [TestMethod]
        public void TestToggleActiveToTrueWithFoundId()
        {
            FakeItemTypes(1);
            ItemTypeRepository.Expect(a => a.GetNullableByID(1)).Return(ItemTypes[0]).Repeat.Any();
            ItemTypes[0].IsActive = false;
            Controller.ToggleActive(1)
                .AssertActionRedirect()
                .ToAction<ApplicationManagementController>(a => a.ListItemTypes());
            Assert.IsTrue(ItemTypes[0].IsActive);
            ItemTypeRepository.AssertWasCalled(a => a.EnsurePersistent(ItemTypes[0]));
            Assert.AreEqual("Item Type has been activated.", Controller.Message);
        }
        #endregion ToggleActive Tests

        #region Helper Methods

        /// <summary>
        /// Fakes the item types.
        /// </summary>
        /// <param name="count">The count.</param>
        private void FakeItemTypes(int count)
        {
            var offSet = ItemTypes.Count;
            for (int i = 0; i < count; i++)
            {
                ItemTypes.Add(CreateValidEntities.ItemType(i+1+offSet));
                ItemTypes[i + offSet].SetIdTo(i + 1 + offSet);
            }
        }

        /// <summary>
        /// Fakes the question types.
        /// </summary>
        /// <param name="count">The count.</param>
        private void FakeQuestionTypes(int count)
        {
            var offSet = QuestionTypes.Count;
            for (int i = 0; i < count; i++)
            {
                QuestionTypes.Add(CreateValidEntities.QuestionType(i + 1 + offSet));
                QuestionTypes[i + offSet].SetIdTo(i + 1 + offSet);
            }
        }

        #endregion Helper Methods

        #region Reflection Tests

        #region Controller Class Tests
        /// <summary>
        /// Tests the controller inherits from super controller.
        /// </summary>
        [TestMethod]
        public void TestControllerInheritsFromSuperController()
        {
            #region Arrange
            var controllerClass = typeof(ApplicationManagementController);
            #endregion Arrange

            #region Act
            var result = controllerClass.BaseType.Name;
            #endregion Act

            #region Assert
            Assert.AreEqual("SuperController", result);
            #endregion Assert		
        }

        /// <summary>
        /// Tests the controller has only three attributes.
        /// </summary>
        [TestMethod]
        public void TestControllerHasOnlyThreeAttributes()
        {
            #region Arrange
            var controllerClass = typeof(ApplicationManagementController);
            #endregion Arrange

            #region Act
            var result = controllerClass.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(3, result.Count());
            #endregion Assert		
        }
        /// <summary>
        /// Tests the controller has admin only attribute.
        /// </summary>
        [TestMethod]
        public void TestControllerHasAdminOnlyAttribute()
        {
            #region Arrange
            var controllerClass = typeof(ApplicationManagementController);
            #endregion Arrange

            #region Act
            var result = controllerClass.GetCustomAttributes(true).OfType<AdminOnlyAttribute>();
            #endregion Act

            #region Assert            
            Assert.IsTrue(result.Count() > 0, "AdminOnlyAttribute not found.");
            #endregion Assert		
        }

        /// <summary>
        /// Tests the controller has transaction attribute.
        /// </summary>
        [TestMethod]
        public void TestControllerHasTransactionAttribute()
        {
            #region Arrange
            var controllerClass = typeof(ApplicationManagementController);
            #endregion Arrange

            #region Act
            var result = controllerClass.GetCustomAttributes(true).OfType<UseTransactionsByDefaultAttribute>();
            #endregion Act

            #region Assert
            Assert.IsTrue(result.Count() > 0, "UseTransactionsByDefaultAttribute not found.");
            #endregion Assert
        }

        /// <summary>
        /// Tests the controller has anti forgery token attribute.
        /// </summary>
        [TestMethod]
        public void TestControllerHasAntiForgeryTokenAttribute()
        {
            #region Arrange
            var controllerClass = typeof(ApplicationManagementController);
            #endregion Arrange

            #region Act
            var result = controllerClass.GetCustomAttributes(true).OfType<UseAntiForgeryTokenOnPostByDefault>();
            #endregion Act

            #region Assert
            Assert.IsTrue(result.Count() > 0, "UseAntiForgeryTokenOnPostByDefault not found.");
            #endregion Assert
        }
        #endregion Controller Class Tests

        #region Controller Method Tests

        /// <summary>
        /// Tests the controller contains expected number of public methods.
        /// </summary>
        [TestMethod]
        public void TestControllerContainsExpectedNumberOfPublicMethods()
        {
            #region Arrange
            var controllerClass = typeof(ApplicationManagementController);            
            #endregion Arrange

            #region Act
            var result = controllerClass.GetMethods().Where(a => a.DeclaringType == controllerClass);
            #endregion Act

            #region Assert
            Assert.AreEqual(7, result.Count(), "It looks like a method was added or removed from the controller.");
            #endregion Assert		
        }


        /// <summary>
        /// Tests the controller method list contains expected attributes.
        /// </summary>
        [TestMethod]
        public void TestControllerMethodListContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = typeof(ApplicationManagementController);
            var controllerMethod = controllerClass.GetMethod("Index");
            #endregion Arrange

            #region Act
            var result = controllerMethod.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, result.Count());
            #endregion Assert		
        }

        /// <summary>
        /// Tests the controller method list item types contains expected attributes.
        /// </summary>
        [TestMethod]
        public void TestControllerMethodListItemTypesContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = typeof(ApplicationManagementController);
            var controllerMethod = controllerClass.GetMethod("ListItemTypes");
            #endregion Arrange

            #region Act
            var result = controllerMethod.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, result.Count());
            #endregion Assert
        }
        /// <summary>
        /// Tests the controller method create item type contains expected attributes.
        /// </summary>
        [TestMethod]
        public void TestControllerMethodCreateItemTypeContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = typeof(ApplicationManagementController);
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "CreateItemType");
            #endregion Arrange

            #region Act
            var result = controllerMethod.ElementAt(0).GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, result.Count());
            #endregion Assert
        }
        /// <summary>
        /// Tests the controller method create item type contains expected attributes2.
        /// </summary>
        [TestMethod]
        public void TestControllerMethodCreateItemTypeContainsExpectedAttributes2()
        {
            #region Arrange
            var controllerClass = typeof(ApplicationManagementController);
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "CreateItemType");
            #endregion Arrange

            #region Act
            var result = controllerMethod.ElementAt(1).GetCustomAttributes(true).OfType<AcceptPostAttribute>();

            #endregion Act

            #region Assert
            Assert.AreEqual(1, result.Count());
            #endregion Assert
        }

        /// <summary>
        /// Tests the controller method edit item type contains expected attributes.
        /// </summary>
        [TestMethod]
        public void TestControllerMethodEditItemTypeContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = typeof(ApplicationManagementController);
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "EditItemType");
            #endregion Arrange

            #region Act
            var result = controllerMethod.ElementAt(0).GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, result.Count());
            #endregion Assert
        }

        /// <summary>
        /// Tests the controller method edit item type contains expected attributes2.
        /// </summary>
        [TestMethod]
        public void TestControllerMethodEditItemTypeContainsExpectedAttributes2()
        {
            #region Arrange
            var controllerClass = typeof(ApplicationManagementController);
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "EditItemType");
            #endregion Arrange

            #region Act
            var result = controllerMethod.ElementAt(1).GetCustomAttributes(true).OfType<AcceptPostAttribute>();

            #endregion Act

            #region Assert
            Assert.AreEqual(1, result.Count());
            #endregion Assert
        }

        /// <summary>
        /// Tests the controller method toggle active contains expected attributes.
        /// </summary>
        [TestMethod]
        public void TestControllerMethodToggleActiveContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = typeof(ApplicationManagementController);
            var controllerMethod = controllerClass.GetMethod("ToggleActive");
            #endregion Arrange

            #region Act
            var result = controllerMethod.GetCustomAttributes(true).OfType<AcceptPostAttribute>();

            #endregion Act

            #region Assert
            Assert.AreEqual(1, result.Count());
            #endregion Assert
        }

        #endregion Controller Method Tests
        #endregion Reflection Tests


    }
}
