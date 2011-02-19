using System;
using System.Linq;
using System.Web.Mvc;
using CRP.Controllers;
using CRP.Controllers.Filter;
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
    public class FIDControllerTests : ControllerTestBase<FIDController>
    {
        private readonly Type _controllerClass = typeof(FIDController);
        IRepository<TouchnetFID> TouchnetFIDRepository { get; set; }
        
        #region Init

        public FIDControllerTests()
        {        
            TouchnetFIDRepository = FakeRepository<TouchnetFID>();
            ControllerRecordFakes.FakeTouchnetFID(3, TouchnetFIDRepository);

            Controller.Repository.Expect(a => a.OfType<TouchnetFID>()).Return(TouchnetFIDRepository).Repeat.Any();
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
            Controller = new TestControllerBuilder().CreateController<FIDController>();
        }


        #endregion Init

        #region Mapping Tests
        /// <summary>
        /// Tests the index mapping.
        /// 1
        /// </summary>
        [TestMethod]
        public void TestIndexMapping()
        {
            "~/FID/Index".ShouldMapTo<FIDController>(a => a.Index());
        }

        /// <summary>
        /// Tests the details mapping.
        /// 2
        /// </summary>
        [TestMethod]
        public void TestDetailsMapping()
        {
            "~/FID/Details/5".ShouldMapTo<FIDController>(a => a.Details(5));
        }

        /// <summary>
        /// Tests the create get mapping.
        /// 3
        /// </summary>
        [TestMethod]
        public void TestCreateGetMapping()
        {
            "~/FID/Create".ShouldMapTo<FIDController>(a => a.Create());
        }

        /// <summary>
        /// Tests the create post mapping.
        /// 4
        /// </summary>
        [TestMethod]
        public void TestCreatePostMapping()
        {
            "~/FID/Create".ShouldMapTo<FIDController>(a => a.Create(new TouchnetFID()), true);
        }

        /// <summary>
        /// Tests the edit get mapping.
        /// 5
        /// </summary>
        [TestMethod]
        public void TestEditGetMapping()
        {
            "~/FID/Edit/5".ShouldMapTo<FIDController>(a => a.Edit(5));
        }
        #endregion Mapping Tests


        #region Index Tests

        /// <summary>
        /// Tests the index returns view.
        /// </summary>
        [TestMethod]
        public void TestIndexReturnsView()
        {
            #region Arrange                                   
            //Setup in the test's constructor
            #endregion Arrange

            #region Act
            var result = Controller.Index()
                .AssertViewRendered()
                .WithViewData<IQueryable<TouchnetFID>>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Count());
            Assert.AreEqual("002", result.ElementAtOrDefault(1).FID);
            #endregion Assert		
        }

        #endregion Index Tests

        #region Details Tests
        /// <summary>
        /// Tests the details redirects to index when id not found.
        /// </summary>
        [TestMethod]
        public void TestDetailsRedirectsToIndexWhenIdNotFound()
        {
            #region Arrange
            //Setup in the test's constructor
            #endregion Arrange

            #region Act
            Controller.Details(4)
                .AssertActionRedirect()
                .ToAction<FIDController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Touchnet FID not found.", Controller.Message);
            #endregion Assert
        }

        /// <summary>
        /// Tests the details returns view when id found.
        /// </summary>
        [TestMethod]
        public void TestDetailsReturnsViewWhenIdFound()
        {
            #region Arrange
            //Setup in the test's constructor
            #endregion Arrange

            #region Act
            var result = Controller.Details(2)
                .AssertViewRendered()
                .WithViewData<TouchnetFID>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("002", result.FID);
            Assert.IsNull(Controller.Message);
            #endregion Assert
        }

        #endregion Details Tests

        #region Create Tests
        /// <summary>
        /// Tests the create get returns view with new touchnet FID.
        /// </summary>
        [TestMethod]
        public void TestCreateGetReturnsViewWithNewTouchnetFID()
        {
            #region Arrange
            //Setup in the test's constructor
            #endregion Arrange

            #region Act
            var result = Controller.Create()
                .AssertViewRendered()
                .WithViewData<TouchnetFID>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Id);
            Assert.IsNull(result.FID);
            Assert.IsNull(result.Description);
            Assert.IsNull(Controller.Message);
            #endregion Assert
        }

        /// <summary>
        /// Tests the create post redirects to index when valid.
        /// </summary>
        [TestMethod]
        public void TestCreatePostRedirectsToIndexWhenValid()
        {
            #region Arrange
            //Setup in the test's constructor
            var fid = CreateValidEntities.TouchnetFID(4);
            #endregion Arrange

            #region Act
            Controller.Create(fid)
                .AssertActionRedirect()
                .ToAction<FIDController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Touchnet FID has been created successfully.", Controller.Message);
            Assert.IsTrue(Controller.ModelState.IsValid);
            TouchnetFIDRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<TouchnetFID>.Is.Anything));
            var args = (TouchnetFID)TouchnetFIDRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<TouchnetFID>.Is.Anything))[0][0];
            Assert.IsNotNull(args);
            Assert.AreEqual("004", args.FID);
            Assert.AreEqual("Description4", args.Description);
            #endregion Assert
        }

        /// <summary>
        /// Tests the create post trims values when valid.
        /// </summary>
        [TestMethod]
        public void TestCreatePostTrimsValuesWhenValid()
        {
            #region Arrange
            //Setup in the test's constructor
            var fid = CreateValidEntities.TouchnetFID(4);
            fid.FID = "  004  ";
            fid.Description = "  Some Description  ";
            #endregion Arrange

            #region Act
            Controller.Create(fid)
                .AssertActionRedirect()
                .ToAction<FIDController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Touchnet FID has been created successfully.", Controller.Message);
            Assert.IsTrue(Controller.ModelState.IsValid);
            TouchnetFIDRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<TouchnetFID>.Is.Anything));
            var args = (TouchnetFID)TouchnetFIDRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<TouchnetFID>.Is.Anything))[0][0];
            Assert.IsNotNull(args);
            Assert.AreEqual("004", args.FID);
            Assert.AreEqual("Some Description", args.Description);
            #endregion Assert
        }

        /// <summary>
        /// Tests the create post returns view when invalid.
        /// </summary>
        [TestMethod]
        public void TestCreatePostReturnsViewWhenInvalid1()
        {
            #region Arrange
            //Setup in the test's constructor
            var fid = CreateValidEntities.TouchnetFID(4);
            fid.FID = "  003  ";
            fid.Description = "  Some Description  ";
            #endregion Arrange

            #region Act
            var result = Controller.Create(fid)
                .AssertViewRendered()
                .WithViewData<TouchnetFID>();
            #endregion Act

            #region Assert
            Assert.IsNull(Controller.Message);
            Assert.IsFalse(Controller.ModelState.IsValid);
            TouchnetFIDRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<TouchnetFID>.Is.Anything));            
            Assert.IsNotNull(result);
            Assert.AreEqual("003", result.FID);
            Assert.AreEqual("Some Description", result.Description);
            Controller.ModelState.AssertErrorsAre("FID value already used");
            #endregion Assert
        }

        /// <summary>
        /// Tests the create post returns view when invalid.
        /// </summary>
        [TestMethod]
        public void TestCreatePostReturnsViewWhenInvalid2()
        {
            #region Arrange
            //Setup in the test's constructor
            var fid = CreateValidEntities.TouchnetFID(4);
            fid.FID = "  004  ";
            fid.Description = "  Description2  ";
            #endregion Arrange

            #region Act
            var result = Controller.Create(fid)
                .AssertViewRendered()
                .WithViewData<TouchnetFID>();
            #endregion Act

            #region Assert
            Assert.IsNull(Controller.Message);
            Assert.IsFalse(Controller.ModelState.IsValid);
            TouchnetFIDRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<TouchnetFID>.Is.Anything));
            Assert.IsNotNull(result);
            Assert.AreEqual("004", result.FID);
            Assert.AreEqual("Description2", result.Description);
            Controller.ModelState.AssertErrorsAre("Description value already used");
            #endregion Assert
        }

        /// <summary>
        /// Tests the create post returns view when invalid.
        /// </summary>
        [TestMethod]
        public void TestCreatePostReturnsViewWhenInvalid3()
        {
            #region Arrange
            //Setup in the test's constructor
            var fid = CreateValidEntities.TouchnetFID(4);
            fid.FID = "  04  ";
            fid.Description = "  Description4  ";
            #endregion Arrange

            #region Act
            var result = Controller.Create(fid)
                .AssertViewRendered()
                .WithViewData<TouchnetFID>();
            #endregion Act

            #region Assert
            Assert.IsNull(Controller.Message);
            Assert.IsFalse(Controller.ModelState.IsValid);
            TouchnetFIDRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<TouchnetFID>.Is.Anything));
            Assert.IsNotNull(result);
            Assert.AreEqual("04", result.FID);
            Assert.AreEqual("Description4", result.Description);
            Controller.ModelState.AssertErrorsAre("FID: length must be between 3 and 3");
            #endregion Assert
        }

        #endregion Create Tests

        #region Edit Tests

        /// <summary>
        /// Tests the edit get redirects to index when id not found.
        /// </summary>
        [TestMethod]
        public void TestEditGetRedirectsToIndexWhenIdNotFound()
        {
            #region Arrange                                   
            //Setup in the test's constructor
            #endregion Arrange

            #region Act
            Controller.Edit(4)
                .AssertActionRedirect()
                .ToAction<FIDController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Touchnet FID not found.", Controller.Message);
            #endregion Assert		
        }

        /// <summary>
        /// Tests the edit get returns view when id found.
        /// </summary>
        [TestMethod]
        public void TestEditGetReturnsViewWhenIdFound()
        {
            #region Arrange
            //Setup in the test's constructor
            #endregion Arrange

            #region Act
            var result = Controller.Edit(2)
                .AssertViewRendered()
                .WithViewData<TouchnetFID>();
            #endregion Act

            #region Assert
            Assert.IsNull(Controller.Message);
            Assert.IsNotNull(result);
            Assert.AreEqual("002", result.FID);
            #endregion Assert
        }

        /// <summary>
        /// Tests the edit post redirects to index when id not found.
        /// </summary>
        [TestMethod]
        public void TestEditPostRedirectsToIndexWhenIdNotFound()
        {
            #region Arrange
            //Setup in the test's constructor
            var fid = CreateValidEntities.TouchnetFID(4);
            #endregion Arrange

            #region Act
            Controller.Edit(4, fid)
                .AssertActionRedirect()
                .ToAction<FIDController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Touchnet FID not found.", Controller.Message);            
            TouchnetFIDRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<TouchnetFID>.Is.Anything));
            #endregion Assert
        }

        /// <summary>
        /// Tests the edit post trims values when valid.
        /// </summary>
        [TestMethod]
        public void TestEditPostTrimsValuesWhenValid()
        {
            #region Arrange
            //Setup in the test's constructor
            var fid = CreateValidEntities.TouchnetFID(4);
            fid.FID = "  004  ";
            fid.Description = "  Some Description  ";
            #endregion Arrange

            #region Act
            Controller.Edit(2, fid)
                .AssertActionRedirect()
                .ToAction<FIDController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Touchnet FID has been saved successfully.", Controller.Message);
            Assert.IsTrue(Controller.ModelState.IsValid);
            TouchnetFIDRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<TouchnetFID>.Is.Anything));
            var args = (TouchnetFID)TouchnetFIDRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<TouchnetFID>.Is.Anything))[0][0];
            Assert.IsNotNull(args);
            Assert.AreEqual("004", args.FID);
            Assert.AreEqual("Some Description", args.Description);
            Assert.AreEqual(2, args.Id);
            #endregion Assert
        }

        /// <summary>
        /// Tests the edit post returns view when invalid1.
        /// </summary>
        [TestMethod]
        public void TestEditPostReturnsViewWhenInvalid1()
        {
            #region Arrange
            //Setup in the test's constructor
            var fid = CreateValidEntities.TouchnetFID(4);
            fid.FID = "  003  ";
            fid.Description = "  Some Description  ";
            #endregion Arrange

            #region Act
            var result = Controller.Edit(2, fid)
                .AssertViewRendered()
                .WithViewData<TouchnetFID>();
            #endregion Act

            #region Assert
            Assert.IsNull(Controller.Message);
            Assert.IsFalse(Controller.ModelState.IsValid);
            TouchnetFIDRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<TouchnetFID>.Is.Anything));
            Assert.IsNotNull(result);
            Assert.AreEqual("003", result.FID);
            Assert.AreEqual("Some Description", result.Description);
            Controller.ModelState.AssertErrorsAre("FID value already used");
            Assert.AreEqual(2, result.Id);
            #endregion Assert
        }

        /// <summary>
        /// Tests the edit post returns view when invalid2.
        /// </summary>
        [TestMethod]
        public void TestEditPostReturnsViewWhenInvalid2()
        {
            #region Arrange
            //Setup in the test's constructor
            var fid = CreateValidEntities.TouchnetFID(4);
            fid.FID = "  004  ";
            fid.Description = "  Description3  ";
            #endregion Arrange

            #region Act
            var result = Controller.Edit(2, fid)
                .AssertViewRendered()
                .WithViewData<TouchnetFID>();
            #endregion Act

            #region Assert
            Assert.IsNull(Controller.Message);
            Assert.IsFalse(Controller.ModelState.IsValid);
            TouchnetFIDRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<TouchnetFID>.Is.Anything));
            Assert.IsNotNull(result);
            Assert.AreEqual("004", result.FID);
            Assert.AreEqual("Description3", result.Description);
            Controller.ModelState.AssertErrorsAre("Description value already used");
            #endregion Assert
        }

        /// <summary>
        /// Tests the edit post returns view when invalid3.
        /// </summary>
        [TestMethod]
        public void TestEditPostReturnsViewWhenInvalid3()
        {
            #region Arrange
            //Setup in the test's constructor
            var fid = CreateValidEntities.TouchnetFID(4);
            fid.FID = "  04  ";
            fid.Description = "  Description4  ";
            #endregion Arrange

            #region Act
            var result = Controller.Edit(2, fid)
                .AssertViewRendered()
                .WithViewData<TouchnetFID>();
            #endregion Act

            #region Assert
            Assert.IsNull(Controller.Message);
            Assert.IsFalse(Controller.ModelState.IsValid);
            TouchnetFIDRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<TouchnetFID>.Is.Anything));
            Assert.IsNotNull(result);
            Assert.AreEqual("04", result.FID);
            Assert.AreEqual("Description4", result.Description);
            Controller.ModelState.AssertErrorsAre("FID: length must be between 3 and 3");
            #endregion Assert
        }
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
            Assert.AreEqual(6, result.Count(), "It looks like a method was added or removed from the controller.");
            #endregion Assert
        }


        /// <summary>
        /// Tests the controller method index contains expected attributes.
        /// 1
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
        /// Tests the controller method details contains expected attributes.
        /// 2
        /// </summary>
        [TestMethod]
        public void TestControllerMethodDetailsContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethod("Details");
            #endregion Arrange

            #region Act
            var result = controllerMethod.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, result.Count());
            #endregion Assert
        }


        /// <summary>
        /// Tests the controller method create get contains expected attributes.
        /// 3
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
        /// Tests the controller method create contains expected attributes.
        /// 4
        /// </summary>
        [TestMethod]
        public void TestControllerMethodCreatePostContainsExpectedAttributes()
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
        /// Tests the controller method edit get contains expected attributes.
        /// 5
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

        /// <summary>
        /// Tests the controller method Edit contains expected attributes.
        /// 6
        /// </summary>
        [TestMethod]
        public void TestControllerMethodEditPostContainsExpectedAttributes1()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "Edit");
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

        #endregion Controller Method Tests

        #endregion Reflection Tests
    }
}
