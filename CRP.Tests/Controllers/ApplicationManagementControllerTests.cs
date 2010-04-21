﻿using System.Collections.Generic;
using System.Linq;
using CRP.Controllers;
using CRP.Controllers.ViewModels;
using CRP.Core.Domain;
using CRP.Tests.Core.Extensions;
using CRP.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;

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
                (a => a.EditItemType(new ItemType()), true);
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
    }
}
