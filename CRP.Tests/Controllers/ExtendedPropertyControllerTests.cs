using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
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
    public class ExtendedPropertyControllerTests : ControllerTestBase<ExtendedPropertyController>
    {
        protected List<ItemType> ItemTypes { get; set; }
        protected IRepository<ItemType> ItemTypeRepository { get; set; }
        protected List<QuestionType> QuestionTypes { get; set; }
        protected IRepository<QuestionType> QuestionTypeRepository { get; set; }
        protected List<ExtendedProperty> ExtendedProperties { get; set; }
        protected IRepository<ExtendedProperty> ExtendedPropertyRepository { get; set; }
        protected List<ExtendedPropertyAnswer> ExtendedPropertyAnswers { get; set; }
        protected IRepository<ExtendedPropertyAnswer> ExtendedPropertyAnswerRepository { get; set; }
        private readonly Type _controllerClass = typeof(ExtendedPropertyController);

        #region Init

        public ExtendedPropertyControllerTests()
        {
            ItemTypes = new List<ItemType>();
            ItemTypeRepository = FakeRepository<ItemType>();
            Controller.Repository.Expect(a => a.OfType<ItemType>()).Return(ItemTypeRepository).Repeat.Any();

            QuestionTypes = new List<QuestionType>();
            QuestionTypeRepository = FakeRepository<QuestionType>();
            Controller.Repository.Expect(a => a.OfType<QuestionType>()).Return(QuestionTypeRepository).Repeat.Any();

            ExtendedProperties = new List<ExtendedProperty>();
            ExtendedPropertyRepository = FakeRepository<ExtendedProperty>();
            Controller.Repository.Expect(a => a.OfType<ExtendedProperty>()).Return(ExtendedPropertyRepository).Repeat.Any();

            ExtendedPropertyAnswers = new List<ExtendedPropertyAnswer>();
            ExtendedPropertyAnswerRepository = FakeRepository<ExtendedPropertyAnswer>();
            Controller.Repository.Expect(a => a.OfType<ExtendedPropertyAnswer>()).Return(ExtendedPropertyAnswerRepository).Repeat.Any();
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
            Controller = new TestControllerBuilder().CreateController<ExtendedPropertyController>();
        }

        #endregion Init

        #region Route Tests

        /// <summary>
        /// Tests the index mapping.
        /// </summary>
        [TestMethod]
        public void TestIndexMapping()
        {
            "~/ExtendedProperty/Index".ShouldMapTo<ExtendedPropertyController>(a => a.Index());    
        }

        /// <summary>
        /// Tests the create mapping.
        /// </summary>
        [TestMethod]
        public void TestCreateMappingIgnoreParameter()
        {
            "~/ExtendedProperty/Create/5".ShouldMapTo<ExtendedPropertyController>(a => a.Create(5), true);
        }

        /// <summary>
        /// Tests the create with parameters mapping.
        /// </summary>
        [TestMethod]
        public void TestCreateWithParametersMapping()
        {
            "~/ExtendedProperty/Create/5".ShouldMapTo<ExtendedPropertyController>(a => a.Create(5, new ExtendedProperty()), true);
        }

        /// <summary>
        /// Tests the delete mapping.
        /// </summary>
        [TestMethod]
        public void TestDeleteMapping()
        {
            "~/ExtendedProperty/Delete/5".ShouldMapTo<ExtendedPropertyController>(a => a.Delete(5));
        }
        #endregion Route Tests

        #region Index Tests

        /// <summary>
        /// Tests the index of the index redirects to home controller.
        /// </summary>
        [TestMethod]
        public void TestIndexRedirectsToHomeControllerIndex()
        {
            Controller.Index()
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
        }

        #endregion Index Tests

        #region Create Tests

        /// <summary>
        /// Tests the create with one parameter with item type id not found redirects to index.
        /// </summary>
        [TestMethod]
        public void TestCreateWithOneParameterWithItemTypeIdNotFoundRedirectsToIndex()
        {
            ItemTypeRepository.Expect(a => a.GetNullableById(1)).Return(null).Repeat.Any();

            Controller.Create(1)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
        }

        /// <summary>
        /// Tests the create with one parameter with found item type id returns view.
        /// </summary>
        [TestMethod]
        public void TestCreateWithOneParameterWithFoundItemTypeIdReturnsView()
        {
            FakeItemTypes(1);
            FakeQuestionTypes(3);
            QuestionTypes[1].ExtendedProperty = true;

            ItemTypeRepository.Expect(a => a.GetNullableById(1)).Return(ItemTypes[0]).Repeat.Any();
            QuestionTypeRepository.Expect(a => a.Queryable).Return(QuestionTypes.AsQueryable()).Repeat.Any();

            var result = Controller.Create(1)
                .AssertViewRendered()
                .WithViewData<ExtendedPropertyViewModel>();

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.QuestionTypes.Count());
            Assert.AreSame(ItemTypes[0], result.ItemType);
        }

        /// <summary>
        /// Tests the create redirects to index when item type id not found.
        /// </summary>
        [TestMethod]
        public void TestCreateRedirectsToIndexWhenItemTypeIdNotFound()
        {
            ItemTypeRepository.Expect(a => a.GetNullableById(1)).Return(null).Repeat.Any();
            Controller.Create(1, new ExtendedProperty())
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
        }

        /// <summary>
        /// Tests the create with valid data saves.
        /// </summary>
        [TestMethod]
        public void TestCreateWithValidDataSaves1()
        {
            FakeItemTypes(3);
            ItemTypeRepository.Expect(a => a.GetNullableById(2)).Return(ItemTypes[1]).Repeat.Any();

            var extendedProperty = CreateValidEntities.ExtendedProperty(1);
            Controller.Create(2, extendedProperty)
                .AssertActionRedirect()
                .ToAction<ApplicationManagementController>(a => a.EditItemType(2));
            ItemTypeRepository.AssertWasCalled(a => a.EnsurePersistent(ItemTypes[1]));
            Assert.AreEqual("Extended property has been created successfully.", Controller.Message);
        }

        /// <summary>
        /// Tests the create with valid data saves.
        /// </summary>
        [TestMethod]
        public void TestCreateWithValidDataSaves2()
        {
            FakeItemTypes(3);
            ItemTypeRepository.Expect(a => a.GetNullableById(2)).Return(ItemTypes[1]).Repeat.Any();
            FakeExtendedProperties(3);
            foreach (var list in ExtendedProperties)
            {
                ItemTypes[1].AddExtendedProperty(list);
            }
            Assert.AreEqual(3, ItemTypes[1].ExtendedProperties.Count);

            var extendedProperty = CreateValidEntities.ExtendedProperty(99);
            Controller.Create(2, extendedProperty)
                .AssertActionRedirect()
                .ToAction<ApplicationManagementController>(a => a.EditItemType(2));
            ItemTypeRepository.AssertWasCalled(a => a.EnsurePersistent(ItemTypes[1]));
            Assert.AreEqual("Extended property has been created successfully.", Controller.Message);
            Assert.AreEqual(4, extendedProperty.Order);
            Assert.AreEqual(4, ItemTypes[1].ExtendedProperties.Count);
        }

        /// <summary>
        /// Tests the create with invalid data does not save.
        /// </summary>
        [TestMethod]
        public void TestCreateWithInvalidDataDoesNotSave1()
        {
            FakeQuestionTypes(1);
            FakeItemTypes(3);
            ItemTypeRepository.Expect(a => a.GetNullableById(2)).Return(ItemTypes[1]).Repeat.Any();
            QuestionTypeRepository.Expect(a => a.Queryable).Return(QuestionTypes.AsQueryable()).Repeat.Any();

            var extendedProperty = CreateValidEntities.ExtendedProperty(1);
            extendedProperty.Name = " "; //Invalid
            Controller.Create(2, extendedProperty)
                .AssertViewRendered()
                .WithViewData<ExtendedPropertyViewModel>();
            ItemTypeRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<ItemType>.Is.Anything));
            Assert.AreNotEqual("Extended property has been created successfully.", Controller.Message);
            Controller.ModelState.AssertErrorsAre("Name: may not be null or empty");
        }

        /// <summary>
        /// Tests the create with invalid data does not save.
        /// Duplicate ExtendedPropertyName in ItemTypes
        /// </summary>
        [TestMethod]
        public void TestCreateWithInvalidDataDoesNotSave2()
        {
            FakeQuestionTypes(1);
            FakeItemTypes(3);
            ItemTypeRepository.Expect(a => a.GetNullableById(2)).Return(ItemTypes[1]).Repeat.Any();
            QuestionTypeRepository.Expect(a => a.Queryable).Return(QuestionTypes.AsQueryable()).Repeat.Any();

            FakeExtendedProperties(3);
            foreach (var list in ExtendedProperties)
            {
                ItemTypes[1].AddExtendedProperty(list);
            }
            Assert.AreEqual(3, ItemTypes[1].ExtendedProperties.Count);

            var extendedProperty = CreateValidEntities.ExtendedProperty(1);

            Controller.Create(2, extendedProperty)
                .AssertViewRendered()
                .WithViewData<ExtendedPropertyViewModel>();
            ItemTypeRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<ItemType>.Is.Anything));
            Assert.AreNotEqual("Extended property has been created successfully.", Controller.Message);
            Controller.ModelState.AssertErrorsAre("Item type already has extended property with the same name.");
        }

        #endregion Create Tests

        #region Delete Tests

        /// <summary>
        /// Tests the delete where id is not found redirects to list item types.
        /// </summary>
        [TestMethod]
        public void TestDeleteWhereIdIsNotFoundRedirectsToListItemTypes()
        {
            ExtendedPropertyRepository.Expect(a => a.GetNullableById(1)).Return(null).Repeat.Any();
            Controller.Delete(1)
                .AssertActionRedirect()
                .ToAction<ApplicationManagementController>(a => a.ListItemTypes());
        }

        /// <summary>
        /// Tests the delete with valid data saves.
        /// </summary>
        [TestMethod]
        public void TestDeleteWithValidDataSaves()
        {
            FakeItemTypes(3);            
            FakeExtendedPropertyAnswers(3);
            ExtendedPropertyAnswerRepository.Expect(a => a.Queryable).Return(ExtendedPropertyAnswers.AsQueryable());
            FakeExtendedProperties(4);
            ExtendedPropertyRepository.Expect(a => a.GetNullableById(2)).Return(ExtendedProperties[1]).Repeat.Any();

            ExtendedPropertyAnswers[0].ExtendedProperty = ExtendedProperties[0];
            ExtendedPropertyAnswers[1].ExtendedProperty = ExtendedProperties[2]; //Skipped 1 so it will delete
            ExtendedPropertyAnswers[2].ExtendedProperty = ExtendedProperties[3]; 

            for (int i = 0; i < 3; i++)
            {
                ItemTypes[1].AddExtendedProperty(ExtendedProperties[i]);
            }

            Controller.Delete(2)
                .AssertActionRedirect()
                .ToAction<ApplicationManagementController>(a => a.EditItemType(ItemTypes[1].Id));
            Assert.AreEqual("Extended property has been removed successfully.", Controller.Message);
            ExtendedPropertyRepository.AssertWasCalled(a => a.Remove(ExtendedProperties[1]));
        }

        /// <summary>
        /// Tests the delete where extended property answer has extended property prevents delete.
        /// </summary>
        [TestMethod]
        public void TestDeleteWhereExtendedPropertyAnswerHasExtendedPropertyPreventsDelete()
        {
            FakeItemTypes(3);
            FakeExtendedPropertyAnswers(3);
            ExtendedPropertyAnswerRepository.Expect(a => a.Queryable).Return(ExtendedPropertyAnswers.AsQueryable());
            FakeExtendedProperties(4);
            ExtendedPropertyRepository.Expect(a => a.GetNullableById(2)).Return(ExtendedProperties[1]).Repeat.Any();

            ExtendedPropertyAnswers[0].ExtendedProperty = ExtendedProperties[0];
            ExtendedPropertyAnswers[1].ExtendedProperty = ExtendedProperties[1]; //Did NOT skipped 1 so it will NOT delete
            ExtendedPropertyAnswers[2].ExtendedProperty = ExtendedProperties[2];

            for (int i = 0; i < 3; i++)
            {
                ItemTypes[1].AddExtendedProperty(ExtendedProperties[i]);
            }

            Controller.Delete(2)
                .AssertActionRedirect()
                .ToAction<ApplicationManagementController>(a => a.EditItemType(ItemTypes[1].Id));
            Assert.AreEqual("Extended property cannot be deleted, because there is already an item associated with the item type.", Controller.Message);
            ExtendedPropertyRepository.AssertWasNotCalled(a => a.Remove(Arg<ExtendedProperty>.Is.Anything));
        }

        #endregion Delete Tests

        #region Reflection Tests

        #region Controller Class Tests
        /// <summary>
        /// Tests the controller inherits from super controller.
        /// </summary>
        [TestMethod]
        public void TestControllerInheritsFromSuperController()
        {
            #region Arrange
            var controllerClass = _controllerClass;
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
            var controllerClass = _controllerClass;
            #endregion Arrange

            #region Act
            var result = controllerClass.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(3, result.Count());
            #endregion Assert
        }

        /// <summary>
        /// Tests the controller has transaction attribute.
        /// </summary>
        [TestMethod]
        public void TestControllerHasTransactionAttribute()
        {
            #region Arrange
            var controllerClass = _controllerClass;
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
            var controllerClass = _controllerClass;
            #endregion Arrange

            #region Act
            var result = controllerClass.GetCustomAttributes(true).OfType<UseAntiForgeryTokenOnPostByDefault>();
            #endregion Act

            #region Assert
            Assert.IsTrue(result.Count() > 0, "UseAntiForgeryTokenOnPostByDefault not found.");
            #endregion Assert
        }

        /// <summary>
        /// Tests the controller has admin only attribute.
        /// </summary>
        [TestMethod]
        public void TestControllerHasAdminOnlyAttribute()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            #endregion Arrange

            #region Act
            var result = controllerClass.GetCustomAttributes(true).OfType<AdminOnlyAttribute>();
            #endregion Act

            #region Assert
            Assert.IsTrue(result.Count() > 0, "AdminOnlyAttribute not found.");
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
            var controllerClass = _controllerClass;
            #endregion Arrange

            #region Act
            var result = controllerClass.GetMethods().Where(a => a.DeclaringType == controllerClass);
            #endregion Act

            #region Assert
            Assert.AreEqual(4, result.Count(), "It looks like a method was added or removed from the controller.");
            #endregion Assert
        }


        /// <summary>
        /// Tests the controller method Index contains expected attributes.
        /// </summary>
        [TestMethod]
        public void TestControllerMethodIndexContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = _controllerClass;
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
        /// Tests the controller method create contains expected attributes.
        /// </summary>
        [TestMethod]
        public void TestControllerMethodCreateContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "Create");
            #endregion Arrange

            #region Act
            //var expectedAttribute = controllerMethod.ElementAt(0).GetCustomAttributes(true).OfType<AdminOnlyAttribute>();
            var allAttributes = controllerMethod.ElementAt(0).GetCustomAttributes(true);
            #endregion Act

            #region Assert
            //Assert.AreEqual(1, expectedAttribute.Count(), "AdminOnlyAttribute not found");
            Assert.AreEqual(0, allAttributes.Count(), "More than expected custom attributes found.");
            #endregion Assert
        }

        /// <summary>
        /// Tests the controller method create contains expected attributes2.
        /// </summary>
        [TestMethod]
        public void TestControllerMethodCreateContainsExpectedAttributes2()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "Create");
            #endregion Arrange

            #region Act
            var expectedAttribute = controllerMethod.ElementAt(1).GetCustomAttributes(true).OfType<HttpPostAttribute>();
            var allAttributes = controllerMethod.ElementAt(1).GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "HttpPostAttribute not found");
            Assert.AreEqual(1, allAttributes.Count(), "More than expected custom attributes found.");
            #endregion Assert
        }

        /// <summary>
        /// Tests the controller method delete contains expected attributes2.
        /// </summary>
        [TestMethod]
        public void TestControllerMethodDeleteContainsExpectedAttributes2()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethod("Delete");
            #endregion Arrange

            #region Act
            var expectedAttribute = controllerMethod.GetCustomAttributes(true).OfType<HttpPostAttribute>();
            var allAttributes = controllerMethod.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "HttpPostAttribute not found");
            Assert.AreEqual(1, allAttributes.Count(), "More than expected custom attributes found.");
            #endregion Assert
        }
 
        #endregion Controller Method Tests

        #endregion Reflection Tests

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
                ItemTypes.Add(CreateValidEntities.ItemType(i + 1 + offSet));
                ItemTypes[i + offSet].SetIdTo(i + 1 + offSet);
            }
        }
        /// <summary>
        /// Fakes the QuestionTypes.
        /// Author: Sylvestre, Jason
        /// Create: 2010/01/28
        /// </summary>
        /// <param name="count">The number of QuestionTypes to add.</param>
        private void FakeQuestionTypes(int count)
        {
            var offSet = QuestionTypes.Count;
            for (int i = 0; i < count; i++)
            {
                QuestionTypes.Add(CreateValidEntities.QuestionType(i + 1 + offSet));
                QuestionTypes[i + offSet].SetIdTo(i + 1 + offSet);
            }
        }

        /// <summary>
        /// Fakes the ExtendedProperties.
        /// Author: Sylvestre, Jason
        /// Create: 2010/01/28
        /// </summary>
        /// <param name="count">The number of ExtendedProperties to add.</param>
        private void FakeExtendedProperties(int count)
        {
            var offSet = ExtendedProperties.Count;
            for (int i = 0; i < count; i++)
            {
                ExtendedProperties.Add(CreateValidEntities.ExtendedProperty(i + 1 + offSet));
                ExtendedProperties[i + offSet].SetIdTo(i + 1 + offSet);
            }
        }
        /// <summary>
        /// Fakes the ExtendedPropertyAnswers.
        /// Author: Sylvestre, Jason
        /// Create: 2010/01/28
        /// </summary>
        /// <param name="count">The number of ExtendedPropertyAnswers to add.</param>
        private void FakeExtendedPropertyAnswers(int count)
        {
            var offSet = ExtendedPropertyAnswers.Count;
            for (int i = 0; i < count; i++)
            {
                ExtendedPropertyAnswers.Add(CreateValidEntities.ExtendedPropertyAnswer(i + 1 + offSet));
                ExtendedPropertyAnswers[i + offSet].SetIdTo(i + 1 + offSet);
            }
        }

        #endregion Helper Methods
    }
}
