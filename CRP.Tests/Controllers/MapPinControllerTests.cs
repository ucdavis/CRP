﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using CRP.Controllers;
using CRP.Controllers.Filter;
using CRP.Controllers.Services;
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
    public class MapPinControllerTests : ControllerTestBase<MapPinController>
    {
        private readonly Type _controllerClass = typeof(MapPinController);
        IRepository<MapPin> MapPinRepository { get; set; }
        private IRepository<Item> ItemRepository { get; set; }
        public IAccessControllService AccessControllService;

        #region Init

        public MapPinControllerTests()
        {
            MapPinRepository = FakeRepository<MapPin>();                       
            Controller.Repository.Expect(a => a.OfType<MapPin>()).Return(MapPinRepository).Repeat.Any();
            
            ItemRepository = FakeRepository<Item>();
            Controller.Repository.Expect(a => a.OfType<Item>()).Return(ItemRepository).Repeat.Any();
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
            AccessControllService = MockRepository.GenerateStub<IAccessControllService>();
            Controller = new TestControllerBuilder().CreateController<MapPinController>(AccessControllService);
        }


        #endregion Init

        #region Mapping Tests
        [TestMethod]
        public void TestCreateGetMapping()
        {
            "~/MapPin/Create/5".ShouldMapTo<MapPinController>(a => a.Create(5), true);
        }

        [TestMethod]
        public void TestCreatePostMapping()
        {
            "~/MapPin/Create/5".ShouldMapTo<MapPinController>(a => a.Create(5, new MapPin()), true);
        }

        [TestMethod]
        public void TestEditGetMapping()
        {
            "~/MapPin/Edit/?itemId=5:mapPinId=2".ShouldMapTo<MapPinController>(a => a.Edit(5,2), true);
        }
        #endregion Mapping Tests

        #region Create Tests

        #region Create Get

        [TestMethod]
        public void TestCreateGetRedirectsToListIfItemIsNull()
        {
            #region Arrange
            ControllerRecordFakes.FakeItems(3, ItemRepository);
            AccessControllService
                .Expect(a => a.HasItemAccess(Arg<IPrincipal>.Is.Anything, Arg<Item>.Is.Anything))
                .Return(true).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Create(4)
                .AssertActionRedirect()
                .ToAction<ItemManagementController>(a => a.List(null));
            #endregion Act

            #region Assert
            ItemRepository.AssertWasCalled(a => a.GetNullableByID(4));
            Assert.AreEqual("You do not have editor rights to that item.", Controller.Message);
            AccessControllService.AssertWasNotCalled(a => a.HasItemAccess(Arg<IPrincipal>.Is.Anything, Arg<Item>.Is.Anything));
            #endregion Assert		
        }

        [TestMethod]
        public void TestCreateGetRedirectsToListIfNoItemAccess()
        {
            #region Arrange
            ControllerRecordFakes.FakeItems(3, ItemRepository);
            AccessControllService
                .Expect(a => a.HasItemAccess(Arg<IPrincipal>.Is.Anything, Arg<Item>.Is.Anything))
                .Return(false).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Create(3)
                .AssertActionRedirect()
                .ToAction<ItemManagementController>(a => a.List(null));
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have editor rights to that item.", Controller.Message);
            AccessControllService.AssertWasCalled(a => a.HasItemAccess(Arg<IPrincipal>.Is.Anything, Arg<Item>.Is.Anything));

            var args = (Item)AccessControllService.GetArgumentsForCallsMadeOn(a => a.HasItemAccess(Arg<IPrincipal>.Is.Anything, Arg<Item>.Is.Anything))[0][1];
            Assert.IsNotNull(args);
            Assert.AreEqual(args.Name , "Name3");
            #endregion Assert 

        }


        [TestMethod]
        public void TestCreateGetWithValidAccessReturnsExpectedView()
        {
            #region Arrange
            ControllerRecordFakes.FakeItems(3, ItemRepository);
            AccessControllService
                .Expect(a => a.HasItemAccess(Arg<IPrincipal>.Is.Anything, Arg<Item>.Is.Anything))
                .Return(true).Repeat.Any();
            #endregion Arrange

            #region Act
            var result = Controller.Create(3)
                .AssertViewRendered()
                .WithViewData<MapPinViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Name3", result.Item.Name);
            Assert.AreEqual(0, result.MapPin.Id);
            #endregion Assert		
        }
        #endregion Create Get

        #region Create Post Tests
        [TestMethod]
        public void TestCreatePostRedirectsToListIfItemIsNull()
        {
            #region Arrange
            ControllerRecordFakes.FakeItems(3, ItemRepository);
            AccessControllService
                .Expect(a => a.HasItemAccess(Arg<IPrincipal>.Is.Anything, Arg<Item>.Is.Anything))
                .Return(true).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Create(4, CreateValidEntities.MapPin(3))
                .AssertActionRedirect()
                .ToAction<ItemManagementController>(a => a.List(null));
            #endregion Act

            #region Assert
            ItemRepository.AssertWasCalled(a => a.GetNullableByID(4));
            Assert.AreEqual("You do not have editor rights to that item.", Controller.Message);
            AccessControllService.AssertWasNotCalled(a => a.HasItemAccess(Arg<IPrincipal>.Is.Anything, Arg<Item>.Is.Anything));
            #endregion Assert
        }

        [TestMethod]
        public void TestCreatePostRedirectsToListIfNoItemAccess()
        {
            #region Arrange
            ControllerRecordFakes.FakeItems(3, ItemRepository);
            AccessControllService
                .Expect(a => a.HasItemAccess(Arg<IPrincipal>.Is.Anything, Arg<Item>.Is.Anything))
                .Return(false).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Create(3, CreateValidEntities.MapPin(3))
                .AssertActionRedirect()
                .ToAction<ItemManagementController>(a => a.List(null));
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have editor rights to that item.", Controller.Message);
            AccessControllService.AssertWasCalled(a => a.HasItemAccess(Arg<IPrincipal>.Is.Anything, Arg<Item>.Is.Anything));

            var args = (Item)AccessControllService.GetArgumentsForCallsMadeOn(a => a.HasItemAccess(Arg<IPrincipal>.Is.Anything, Arg<Item>.Is.Anything))[0][1];
            Assert.IsNotNull(args);
            Assert.AreEqual(args.Name, "Name3");
            #endregion Assert

        }


        [TestMethod]
        public void TestCreatePostWithValidDataRedirectsToItemControllerMap1()
        {
            #region Arrange
            ControllerRecordFakes.FakeItems(3, ItemRepository);
            AccessControllService
                .Expect(a => a.HasItemAccess(Arg<IPrincipal>.Is.Anything, Arg<Item>.Is.Anything))
                .Return(true).Repeat.Any();
            var mapPins = new List<MapPin>();
            MapPinRepository.Expect(a => a.Queryable).Return(mapPins.AsQueryable()).Repeat.Any();
            #endregion Arrange

            #region Act
            var result = Controller.Create(3, CreateValidEntities.MapPin(3))
                .AssertActionRedirect()
                .ToAction<ItemManagementController>(a => a.Map(3));
            #endregion Act

            #region Assert
            Assert.AreEqual("Map Pin has been created successfully.", Controller.Message);
            MapPinRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<MapPin>.Is.Anything));
            var args = (MapPin)MapPinRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<MapPin>.Is.Anything))[0][0];
            Assert.IsTrue(args.IsPrimary);
            Assert.AreEqual("Description3", args.Description);
            Assert.AreEqual("Name3", args.Item.Name);
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.RouteValues["id"]);
            #endregion Assert		
        }

        [TestMethod]
        public void TestCreatePostWithValidDataRedirectsToItemControllerMap2()
        {
            #region Arrange
            var items = new List<Item>();
            for (int i = 0; i < 3; i++)
            {
                items.Add(CreateValidEntities.Item(i+1));
            }
            ControllerRecordFakes.FakeItems(0,ItemRepository, items);
            AccessControllService
                .Expect(a => a.HasItemAccess(Arg<IPrincipal>.Is.Anything, Arg<Item>.Is.Anything))
                .Return(true).Repeat.Any();
            var mapPins = new List<MapPin>();
            for (int i = 0; i < 3; i++)
            {
                mapPins.Add(CreateValidEntities.MapPin(i+1));
                mapPins[i].IsPrimary = false;
                items[2].AddMapPin(mapPins[i]);
            }
            MapPinRepository.Expect(a => a.Queryable).Return(mapPins.AsQueryable()).Repeat.Any();
            #endregion Arrange

            #region Act
            var result = Controller.Create(3, CreateValidEntities.MapPin(3))
                .AssertActionRedirect()
                .ToAction<ItemManagementController>(a => a.Map(3));
            #endregion Act

            #region Assert
            Assert.AreEqual("Map Pin has been created successfully.", Controller.Message);
            MapPinRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<MapPin>.Is.Anything));
            var args = (MapPin)MapPinRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<MapPin>.Is.Anything))[0][0];
            Assert.IsTrue(args.IsPrimary);
            Assert.AreEqual("Description3", args.Description);
            Assert.AreEqual("Name3", args.Item.Name);
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.RouteValues["id"]);
            #endregion Assert
        }

        [TestMethod]
        public void TestCreatePostWithValidDataRedirectsToItemControllerMap3()
        {
            #region Arrange
            var items = new List<Item>();
            for (int i = 0; i < 3; i++)
            {
                items.Add(CreateValidEntities.Item(i + 1));
            }
            ControllerRecordFakes.FakeItems(0, ItemRepository, items);
            AccessControllService
                .Expect(a => a.HasItemAccess(Arg<IPrincipal>.Is.Anything, Arg<Item>.Is.Anything))
                .Return(true).Repeat.Any();
            var mapPins = new List<MapPin>();
            for (int i = 0; i < 3; i++)
            {
                mapPins.Add(CreateValidEntities.MapPin(i + 1));
                mapPins[i].IsPrimary = true;
                items[1].AddMapPin(mapPins[i]);
            }
            MapPinRepository.Expect(a => a.Queryable).Return(mapPins.AsQueryable()).Repeat.Any();
            #endregion Arrange

            #region Act
            var result = Controller.Create(3, CreateValidEntities.MapPin(3))
                .AssertActionRedirect()
                .ToAction<ItemManagementController>(a => a.Map(3));
            #endregion Act

            #region Assert
            Assert.AreEqual("Map Pin has been created successfully.", Controller.Message);
            MapPinRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<MapPin>.Is.Anything));
            var args = (MapPin)MapPinRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<MapPin>.Is.Anything))[0][0];
            Assert.IsTrue(args.IsPrimary);
            Assert.AreEqual("Description3", args.Description);
            Assert.AreEqual("Name3", args.Item.Name);
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.RouteValues["id"]);
            #endregion Assert
        }

        [TestMethod]
        public void TestCreatePostWithValidDataRedirectsToItemControllerMap4()
        {
            #region Arrange
            var items = new List<Item>();
            for (int i = 0; i < 3; i++)
            {
                items.Add(CreateValidEntities.Item(i + 1));
            }
            ControllerRecordFakes.FakeItems(0, ItemRepository, items);
            AccessControllService
                .Expect(a => a.HasItemAccess(Arg<IPrincipal>.Is.Anything, Arg<Item>.Is.Anything))
                .Return(true).Repeat.Any();
            var mapPins = new List<MapPin>();
            for (int i = 0; i < 3; i++)
            {
                mapPins.Add(CreateValidEntities.MapPin(i + 1));
                mapPins[i].IsPrimary = true;
                items[2].AddMapPin(mapPins[i]);
            }
            MapPinRepository.Expect(a => a.Queryable).Return(mapPins.AsQueryable()).Repeat.Any();
            #endregion Arrange

            #region Act
            var result = Controller.Create(3, CreateValidEntities.MapPin(3))
                .AssertActionRedirect()
                .ToAction<ItemManagementController>(a => a.Map(3));
            #endregion Act

            #region Assert
            Assert.AreEqual("Map Pin has been created successfully.", Controller.Message);
            MapPinRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<MapPin>.Is.Anything));
            var args = (MapPin)MapPinRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<MapPin>.Is.Anything))[0][0];
            Assert.IsFalse(args.IsPrimary);
            Assert.AreEqual("Description3", args.Description);
            Assert.AreEqual("Name3", args.Item.Name);
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.RouteValues["id"]);
            #endregion Assert
        }

        [TestMethod]
        public void TestCreatePostWithInValidDataReturnsView()
        {
            #region Arrange
            ControllerRecordFakes.FakeItems(3, ItemRepository);
            AccessControllService
                .Expect(a => a.HasItemAccess(Arg<IPrincipal>.Is.Anything, Arg<Item>.Is.Anything))
                .Return(true).Repeat.Any();
            var mapPins = new List<MapPin>();
            MapPinRepository.Expect(a => a.Queryable).Return(mapPins.AsQueryable()).Repeat.Any();
            var mapPin = CreateValidEntities.MapPin(3);
            mapPin.Title = null;
            #endregion Arrange

            #region Act
            var result = Controller.Create(3, mapPin)
                .AssertViewRendered()
                .WithViewData<MapPinViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("Title: may not be null or empty");
            MapPinRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<MapPin>.Is.Anything));
            Assert.IsNotNull(result);
            Assert.AreEqual("Name3", result.Item.Name);
            Assert.IsTrue(result.MapPin.IsPrimary);
            Assert.AreEqual("Description3", result.MapPin.Description);
            #endregion Assert
        }
        
        #endregion Create Post Tests
        #endregion Create Tests

        #region Edit Tests
        #region Edit Get Tests

        [TestMethod]
        public void TestEditGetRedirectsToItemManagementListifItemNotFound()        
        {
            #region Arrange
            ControllerRecordFakes.FakeItems(3, ItemRepository);
            #endregion Arrange

            #region Act
            Controller.Edit(4, 99)
                .AssertActionRedirect()
                .ToAction<ItemManagementController>(a => a.List(null));
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have editor rights to that item.", Controller.Message);
            MapPinRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<MapPin>.Is.Anything));
            AccessControllService.AssertWasNotCalled(a => a.HasItemAccess(Arg<IPrincipal>.Is.Anything, Arg<Item>.Is.Anything));
            ItemRepository.AssertWasCalled(a => a.GetNullableByID(4));
            #endregion Assert
        }

        [TestMethod]
        public void TestEditGetRedirectsToItemManagementListifNoAccess()
        {
            #region Arrange
            ControllerRecordFakes.FakeItems(3, ItemRepository);
            AccessControllService
                .Expect(a => a.HasItemAccess(Arg<IPrincipal>.Is.Anything, Arg<Item>.Is.Anything))
                .Return(false).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Edit(3, 99)
                .AssertActionRedirect()
                .ToAction<ItemManagementController>(a => a.List(null));
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have editor rights to that item.", Controller.Message);
            MapPinRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<MapPin>.Is.Anything));
            AccessControllService.AssertWasCalled(a => a.HasItemAccess(Arg<IPrincipal>.Is.Anything, Arg<Item>.Is.Anything));
            ItemRepository.AssertWasCalled(a => a.GetNullableByID(3));
            var args = (Item)AccessControllService.GetArgumentsForCallsMadeOn(a => a.HasItemAccess(Arg<IPrincipal>.Is.Anything, Arg<Item>.Is.Anything))[0][1];
            Assert.IsNotNull(args);
            Assert.AreEqual(args.Name, "Name3");
            #endregion Assert
        }

        [TestMethod]
        public void TestEditGetRedirectsToItemManagementMapIfMapIdNotInItem()
        {
            #region Arrange
            var items = new List<Item>();
            for (int i = 0; i < 3; i++)
            {
                items.Add(CreateValidEntities.Item(i+1));
            }
            ControllerRecordFakes.FakeItems(0, ItemRepository, items);
            AccessControllService
                .Expect(a => a.HasItemAccess(Arg<IPrincipal>.Is.Anything, Arg<Item>.Is.Anything))
                .Return(true).Repeat.Any();
            var mapPin = CreateValidEntities.MapPin(1);
            mapPin.SetIdTo(1);
            items[0].AddMapPin(mapPin);
            MapPinRepository.Expect(a => a.GetNullableByID(1)).Return(mapPin).Repeat.Any();
            #endregion Arrange

            #region Act
            var result = Controller.Edit(3, 1)
                .AssertActionRedirect()
                .ToAction<ItemManagementController>(a => a.Map(3));
            #endregion Act

            #region Assert
            Assert.AreEqual("MapPin not found.", Controller.Message);
            AccessControllService.AssertWasCalled(a => a.HasItemAccess(Arg<IPrincipal>.Is.Anything, Arg<Item>.Is.Anything));
            ItemRepository.AssertWasCalled(a => a.GetNullableByID(3));
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.RouteValues["id"]);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditGetReturnsViewWhenValid()
        {
            #region Arrange
            var items = new List<Item>();
            for (int i = 0; i < 3; i++)
            {
                items.Add(CreateValidEntities.Item(i + 1));
            }
            ControllerRecordFakes.FakeItems(0, ItemRepository, items);
            AccessControllService
                .Expect(a => a.HasItemAccess(Arg<IPrincipal>.Is.Anything, Arg<Item>.Is.Anything))
                .Return(true).Repeat.Any();
            var mapPin = CreateValidEntities.MapPin(1);
            mapPin.SetIdTo(1);
            items[2].AddMapPin(mapPin);
            MapPinRepository.Expect(a => a.GetNullableByID(1)).Return(mapPin).Repeat.Any();
            #endregion Arrange

            #region Act
            var result = Controller.Edit(3, 1)
                .AssertViewRendered()
                .WithViewData<MapPinViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNull(Controller.Message);

            AccessControllService.AssertWasCalled(a => a.HasItemAccess(Arg<IPrincipal>.Is.Anything, Arg<Item>.Is.Anything));
            ItemRepository.AssertWasCalled(a => a.GetNullableByID(3));
            Assert.IsNotNull(result);
            Assert.AreEqual("Name3", result.Item.Name);
            Assert.AreEqual("Description1", result.MapPin.Description);
            #endregion Assert
        }
        #endregion Edit Get Tests

        #region Edit Post Tests

        [TestMethod]
        public void TestEditPostRedirectsToItemManagementListifItemNotFound()
        {
            #region Arrange
            ControllerRecordFakes.FakeItems(3, ItemRepository);
            var mapPinToUpdate = CreateValidEntities.MapPin(4);
            #endregion Arrange

            #region Act
            Controller.Edit(4, 99, mapPinToUpdate)
                .AssertActionRedirect()
                .ToAction<ItemManagementController>(a => a.List(null));
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have editor rights to that item.", Controller.Message);
            MapPinRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<MapPin>.Is.Anything));
            AccessControllService.AssertWasNotCalled(a => a.HasItemAccess(Arg<IPrincipal>.Is.Anything, Arg<Item>.Is.Anything));
            ItemRepository.AssertWasCalled(a => a.GetNullableByID(4));
            #endregion Assert
        }

        [TestMethod]
        public void TestEditPostRedirectsToItemManagementListifNoAccess()
        {
            #region Arrange
            ControllerRecordFakes.FakeItems(3, ItemRepository);
            AccessControllService
                .Expect(a => a.HasItemAccess(Arg<IPrincipal>.Is.Anything, Arg<Item>.Is.Anything))
                .Return(false).Repeat.Any();
            var mapPinToUpdate = CreateValidEntities.MapPin(4);
            #endregion Arrange

            #region Act
            Controller.Edit(3, 99, mapPinToUpdate)
                .AssertActionRedirect()
                .ToAction<ItemManagementController>(a => a.List(null));
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have editor rights to that item.", Controller.Message);
            MapPinRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<MapPin>.Is.Anything));
            AccessControllService.AssertWasCalled(a => a.HasItemAccess(Arg<IPrincipal>.Is.Anything, Arg<Item>.Is.Anything));
            ItemRepository.AssertWasCalled(a => a.GetNullableByID(3));
            var args = (Item)AccessControllService.GetArgumentsForCallsMadeOn(a => a.HasItemAccess(Arg<IPrincipal>.Is.Anything, Arg<Item>.Is.Anything))[0][1];
            Assert.IsNotNull(args);
            Assert.AreEqual(args.Name, "Name3");
            #endregion Assert
        }

        [TestMethod]
        public void TestEditPostRedirectsToItemManagementMapIfMapIdNotInItem()
        {
            #region Arrange
            var items = new List<Item>();
            for (int i = 0; i < 3; i++)
            {
                items.Add(CreateValidEntities.Item(i + 1));
            }
            ControllerRecordFakes.FakeItems(0, ItemRepository, items);
            AccessControllService
                .Expect(a => a.HasItemAccess(Arg<IPrincipal>.Is.Anything, Arg<Item>.Is.Anything))
                .Return(true).Repeat.Any();
            var mapPin = CreateValidEntities.MapPin(1);
            mapPin.SetIdTo(1);
            items[0].AddMapPin(mapPin);
            MapPinRepository.Expect(a => a.GetNullableByID(1)).Return(mapPin).Repeat.Any();
            var mapPinToUpdate = CreateValidEntities.MapPin(4);
            #endregion Arrange

            #region Act
            var result = Controller.Edit(3, 1, mapPinToUpdate)
                .AssertActionRedirect()
                .ToAction<ItemManagementController>(a => a.Map(3));
            #endregion Act

            #region Assert
            Assert.AreEqual("MapPin not found.", Controller.Message);
            AccessControllService.AssertWasCalled(a => a.HasItemAccess(Arg<IPrincipal>.Is.Anything, Arg<Item>.Is.Anything));
            ItemRepository.AssertWasCalled(a => a.GetNullableByID(3));
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.RouteValues["id"]);
            #endregion Assert
        }
        #endregion Edit Post Tests
        #endregion Edit Tests

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
            Assert.IsNotNull(controllerClass.BaseType);
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

        [TestMethod]
        public void TestControllerHasAnyoneWithRoleAttribute()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            #endregion Arrange

            #region Act
            var result = controllerClass.GetCustomAttributes(true).OfType<AnyoneWithRoleAttribute>();
            #endregion Act

            #region Assert
            Assert.IsTrue(result.Count() > 0, "AnyoneWithRoleAttribute not found.");
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
            //Assert.AreEqual(3, result.Count(), "It looks like a method was added or removed from the controller.");
            Assert.AreEqual(3, result.Count(), "Need to write tests once we agree on methods needed.");
            #endregion Assert
        }

        /// <summary>
        /// Tests the controller method create get contains expected attributes.
        /// #1
        /// </summary>
        [TestMethod]
        public void TestControllerMethodCreateGetContainsExpectedAttributes()
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
        /// Tests the controller method create post contains expected attributes.
        /// #2
        /// </summary>
        [TestMethod]
        public void TestControllerMethodCreatePostContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "Create");
            #endregion Arrange

            #region Act
            var expectedAttribute = controllerMethod.ElementAt(1).GetCustomAttributes(true).OfType<AcceptPostAttribute>();
            var allAttributes = controllerMethod.ElementAt(1).GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "AcceptPostAttribute not found");
            Assert.AreEqual(1, allAttributes.Count(), "More than expected custom attributes found.");
            #endregion Assert
        }

        /// <summary>
        /// Tests the controller method edit get contains expected attributes.
        /// #3
        /// </summary>
        [TestMethod]
        public void TestControllerMethodEditGetContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "Edit");
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



        ///// <summary>
        ///// Tests the controller method details contains expected attributes.
        ///// 2
        ///// </summary>
        //[TestMethod]
        //public void TestControllerMethodDetailsContainsExpectedAttributes()
        //{
        //    #region Arrange
        //    var controllerClass = _controllerClass;
        //    var controllerMethod = controllerClass.GetMethod("Details");
        //    #endregion Arrange

        //    #region Act
        //    var result = controllerMethod.GetCustomAttributes(true);
        //    #endregion Act

        //    #region Assert
        //    Assert.AreEqual(0, result.Count());
        //    #endregion Assert
        //}




        ///// <summary>
        ///// Tests the controller method create contains expected attributes.
        ///// 4
        ///// </summary>
        //[TestMethod]
        //public void TestControllerMethodCreatePostContainsExpectedAttributes()
        //{
        //    #region Arrange
        //    var controllerClass = _controllerClass;
        //    var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "Create");
        //    #endregion Arrange

        //    #region Act
        //    var expectedAttribute = controllerMethod.ElementAt(1).GetCustomAttributes(true).OfType<AcceptPostAttribute>();
        //    var allAttributes = controllerMethod.ElementAt(1).GetCustomAttributes(true);
        //    #endregion Act

        //    #region Assert
        //    Assert.AreEqual(1, expectedAttribute.Count(), "AcceptPostAttribute not found");
        //    Assert.AreEqual(1, allAttributes.Count(), "More than expected custom attributes found.");
        //    #endregion Assert
        //}

        ///// <summary>
        ///// Tests the controller method edit get contains expected attributes.
        ///// 5
        ///// </summary>
        //[TestMethod]
        //public void TestControllerMethodEditGetContainsExpectedAttributes()
        //{
        //    #region Arrange
        //    var controllerClass = _controllerClass;
        //    var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "Edit");
        //    #endregion Arrange

        //    #region Act
        //    //var expectedAttribute = controllerMethod.ElementAt(0).GetCustomAttributes(true).OfType<AdminOnlyAttribute>();
        //    var allAttributes = controllerMethod.ElementAt(0).GetCustomAttributes(true);
        //    #endregion Act

        //    #region Assert
        //    //Assert.AreEqual(1, expectedAttribute.Count(), "AdminOnlyAttribute not found");
        //    Assert.AreEqual(0, allAttributes.Count(), "More than expected custom attributes found.");
        //    #endregion Assert
        //}

        ///// <summary>
        ///// Tests the controller method Edit contains expected attributes.
        ///// 6
        ///// </summary>
        //[TestMethod]
        //public void TestControllerMethodEditPostContainsExpectedAttributes1()
        //{
        //    #region Arrange
        //    var controllerClass = _controllerClass;
        //    var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "Edit");
        //    #endregion Arrange

        //    #region Act
        //    var expectedAttribute = controllerMethod.ElementAt(1).GetCustomAttributes(true).OfType<AcceptPostAttribute>();
        //    var allAttributes = controllerMethod.ElementAt(1).GetCustomAttributes(true);
        //    #endregion Act

        //    #region Assert
        //    Assert.AreEqual(1, expectedAttribute.Count(), "AcceptPostAttribute not found");
        //    Assert.AreEqual(1, allAttributes.Count(), "More than expected custom attributes found.");
        //    #endregion Assert
        //}

        #endregion Controller Method Tests

        #endregion Reflection Tests
    }
}
