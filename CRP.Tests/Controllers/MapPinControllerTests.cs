using System;
using System.Linq;
using CRP.Controllers;
using CRP.Controllers.Filter;
using CRP.Controllers.Services;
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
        public IAccessControllService AccessControllService;

        #region Init

        public MapPinControllerTests()
        {
            MapPinRepository = FakeRepository<MapPin>();
                        
            Controller.Repository.Expect(a => a.OfType<MapPin>()).Return(MapPinRepository).Repeat.Any();
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

        #endregion Mapping Tests



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
            //Assert.AreEqual(0, result.Count(), "It looks like a method was added or removed from the controller.");
            Assert.AreEqual(0, result.Count(), "Need to write tests once we agree on methods needed.");
            #endregion Assert
        }

        //Examples
        ///// <summary>
        ///// Tests the controller method index contains expected attributes.
        ///// 1
        ///// </summary>
        //[TestMethod]
        //public void TestControllerMethodIndexContainsExpectedAttributes()
        //{
        //    #region Arrange
        //    var controllerClass = _controllerClass;
        //    var controllerMethod = controllerClass.GetMethod("Index");
        //    #endregion Arrange

        //    #region Act
        //    var result = controllerMethod.GetCustomAttributes(true);
        //    #endregion Act

        //    #region Assert
        //    Assert.AreEqual(0, result.Count());
        //    #endregion Assert
        //}

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
        ///// Tests the controller method create get contains expected attributes.
        ///// 3
        ///// </summary>
        //[TestMethod]
        //public void TestControllerMethodCreateGetContainsExpectedAttributes()
        //{
        //    #region Arrange
        //    var controllerClass = _controllerClass;
        //    var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "Create");
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
