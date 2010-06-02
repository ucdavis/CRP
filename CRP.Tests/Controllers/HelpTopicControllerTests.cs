using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using CRP.Controllers;
using CRP.Controllers.Filter;
using CRP.Controllers.Helpers;
using CRP.Controllers.ViewModels;
using CRP.Core.Domain;
using CRP.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.Attributes;
using MvcContrib.TestHelper;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;
using UCDArch.Web.Attributes;
using CRP.Tests.Core.Extensions;

namespace CRP.Tests.Controllers
{
    [TestClass]
    public class HelpTopicControllerTests : ControllerTestBase<HelpController>
    {
        private readonly Type _controllerClass = typeof(HelpController);
        protected readonly IPrincipal Principal = new MockPrincipal(new[] { RoleNames.Admin });
        IRepository<HelpTopic> HelpTopicRepository { get; set; }
        List<HelpTopic> HelpTopics { get; set; }

        #region Init

        public HelpTopicControllerTests()
        {
            HelpTopics = new List<HelpTopic>();
            HelpTopicRepository = FakeRepository<HelpTopic>();
            Controller.Repository.Expect(a => a.OfType<HelpTopic>()).Return(HelpTopicRepository).Repeat.Any();
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
            Controller = new TestControllerBuilder().CreateController<HelpController>();
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
            "~/Help/Index".ShouldMapTo<HelpController>(a => a.Index());
        }

        /// <summary>
        /// Tests the details mapping.
        /// 2
        /// </summary>
        [TestMethod]
        public void TestDetailsMapping()
        {
            "~/Help/Details/5".ShouldMapTo<HelpController>(a => a.Details(5));
        }

        /// <summary>
        /// Tests the create get mapping.
        /// 3
        /// </summary>
        [TestMethod]
        public void TestCreateGetMapping()
        {
            "~/Help/Create".ShouldMapTo<HelpController>(a => a.Create());
        }

        /// <summary>
        /// Tests the create post mapping.
        /// 4
        /// </summary>
        [TestMethod]
        public void TestCreatePostMapping()
        {
            "~/Help/Create".ShouldMapTo<HelpController>(a => a.Create(new HelpTopic()),true);
        }

        /// <summary>
        /// Tests the edit get mapping.
        /// 5
        /// </summary>
        [TestMethod]
        public void TestEditGetMapping()
        {
            "~/Help/Edit/5".ShouldMapTo<HelpController>(a => a.Edit(5));
        }

        /// <summary>
        /// Tests the edit post mapping.
        /// 6
        /// </summary>
        [TestMethod]
        public void TestEditPostMapping()
        {
            "~/Help/Edit/".ShouldMapTo<HelpController>(a => a.Edit(5, new HelpTopic()), true);
        }

        #endregion Mapping Tests

        #region Index Tests

        /// <summary>
        /// Tests the index returns view of all help topics when admin.
        /// </summary>
        [TestMethod]
        public void TestIndexReturnsViewOfAllHelpTopicsWhenAdmin()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] {RoleNames.Admin });

            SetupDataForTests();            

            #endregion Arrange

            #region Act
            var result = Controller.Index()
                .AssertViewRendered()
                .WithViewData<HelpTopicViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(6, result.HelpTopics.Count());
            Assert.IsTrue(result.IsUserAdmin);
            Assert.IsTrue(result.IsUserAuthorized);
            #endregion Assert		
        }

        /// <summary>
        /// Tests the index returns view of all active help topics when user.
        /// </summary>
        [TestMethod]
        public void TestIndexReturnsViewOfAllActiveHelpTopicsWhenUser()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.User });

            SetupDataForTests();

            #endregion Arrange

            #region Act
            var result = Controller.Index()
                .AssertViewRendered()
                .WithViewData<HelpTopicViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(4, result.HelpTopics.Count());
            Assert.IsFalse(result.IsUserAdmin);
            Assert.IsTrue(result.IsUserAuthorized);
            #endregion Assert
        }

        /// <summary>
        /// Tests the index returns view of only active and public help topics when joe public.
        /// </summary>
        [TestMethod]
        public void TestIndexReturnsViewOfOnlyActiveAndPublicHelpTopicsWhenJoePublic()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { "" });

            SetupDataForTests();

            #endregion Arrange

            #region Act
            var result = Controller.Index()
                .AssertViewRendered()
                .WithViewData<HelpTopicViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.HelpTopics.Count());
            Assert.IsFalse(result.IsUserAdmin);
            Assert.IsFalse(result.IsUserAuthorized);
            #endregion Assert
        }

        /// <summary>
        /// Tests the index returns view help topics ordered by number of reads when joe public.
        /// </summary>
        [TestMethod]
        public void TestIndexReturnsViewHelpTopicsOrderedByNumberOfReadsWhenJoePublic()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { "" });

            SetupDataForTests();     

            #endregion Arrange

            #region Act
            var result = Controller.Index()
                .AssertViewRendered()
                .WithViewData<HelpTopicViewModel>();
            #endregion Act

            #region Assert
            Assert.AreEqual(5, result.HelpTopics.ElementAt(0).Id);
            Assert.AreEqual(4, result.HelpTopics.ElementAt(1).Id);
            Assert.AreEqual(6, result.HelpTopics.ElementAt(2).Id);
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
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Admin });
            SetupDataForTests();
            #endregion Arrange

            #region Act/Assert
            Controller.Details(7)
                .AssertActionRedirect()
                .ToAction<HelpController>(a => a.Index());
            #endregion Act/Assert
        }


        /// <summary>
        /// Tests the details returns view when id found.
        /// </summary>
        [TestMethod]
        public void TestDetailsReturnsViewWhenIdFound()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Admin });
            SetupDataForTests();
            #endregion Arrange

            #region Act
            var result = Controller.Details(6)
                .AssertViewRendered()
                .WithViewData<HelpTopic>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Question6", result.Question);
            #endregion Assert		
        }

        /// <summary>
        /// Tests the details increments count when id found.
        /// </summary>
        [TestMethod]
        public void TestDetailsIncrementsCountWhenIdFound()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Admin });
            SetupDataForTests();
            Assert.AreEqual(1, HelpTopics[5].NumberOfReads);
            #endregion Arrange

            #region Act
            var result = Controller.Details(6)
                .AssertViewRendered()
                .WithViewData<HelpTopic>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Question6", result.Question);
            Assert.AreEqual(2, HelpTopics[5].NumberOfReads);
            Assert.AreEqual(2, result.NumberOfReads);
            HelpTopicRepository.AssertWasCalled(a => a.EnsurePersistent(HelpTopics[5]));
            #endregion Assert
        }
        #endregion Details Tests

        #region Create Tests

        /// <summary>
        /// Tests the create get returns view with new help topic.
        /// </summary>
        [TestMethod]
        public void TestCreateGetReturnsViewWithNewHelpTopic()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Admin });            
            #endregion Arrange

            #region Act
            var result = Controller.Create()
                .AssertViewRendered()
                .WithViewData<HelpTopic>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Id);
            #endregion Assert		
        }


        /// <summary>
        /// Tests the create post with valid data saves.
        /// </summary>
        [TestMethod]
        public void TestCreatePostWithValidDataSaves()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Admin });
            var helpTopic = CreateValidEntities.HelpTopic(99);
            #endregion Arrange

            #region Act
            Controller.Create(helpTopic)
                .AssertActionRedirect()
                .ToAction<HelpController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.IsTrue(Controller.ModelState.IsValid);
            Assert.AreEqual("Help Topic has been created successfully.", Controller.Message);
            HelpTopicRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<HelpTopic>.Is.Anything));
            #endregion Assert		
        }

        /// <summary>
        /// Tests the create post with invalid data does not save.
        /// </summary>
        [TestMethod]
        public void TestCreatePostWithInvalidDataDoesNotSave()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Admin });
            var helpTopic = CreateValidEntities.HelpTopic(99);
            helpTopic.Question = null;
            #endregion Arrange

            #region Act
            Controller.Create(helpTopic)
                .AssertViewRendered()
                .WithViewData<HelpTopic>();
            #endregion Act

            #region Assert
            Assert.IsFalse(Controller.ModelState.IsValid);
            Assert.IsNull(Controller.Message);
            HelpTopicRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<HelpTopic>.Is.Anything));
            Controller.ModelState.AssertErrorsAre("Question: may not be null or empty");
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
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Admin });
            SetupDataForTests();
            #endregion Arrange

            #region Act
            Controller.Edit(HelpTopics.Count + 1)
                .AssertActionRedirect()
                .ToAction<HelpController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Help Topic not found.", Controller.Message);
            #endregion Assert		
        }


        /// <summary>
        /// Tests the edit get returns view when found.
        /// </summary>
        [TestMethod]
        public void TestEditGetReturnsViewWhenFound()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Admin });
            SetupDataForTests();
            #endregion Arrange

            #region Act
            var result = Controller.Edit(3)
                .AssertViewRendered()
                .WithViewData<HelpTopic>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Question3", result.Question);
            #endregion Assert		
        }

        /// <summary>
        /// Tests the edit post updates values and saves with valid data.
        /// </summary>
        [TestMethod]
        public void TestEditPostUpdatesValuesAndSavesWithValidData()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Admin });
            SetupDataForTests();
            var helpTopic = CreateValidEntities.HelpTopic(null);
            helpTopic.IsActive = false;
            helpTopic.AvailableToPublic = false;
            helpTopic.NumberOfReads = 99;
            helpTopic.Question = "UpdateQ";
            helpTopic.Answer = "UpdateA";
            Assert.AreNotEqual(helpTopic.IsActive, HelpTopics[5].IsActive);
            Assert.AreNotEqual(helpTopic.AvailableToPublic, HelpTopics[5].AvailableToPublic);
            #endregion Arrange

            #region Act
            Controller.Edit(6, helpTopic)
                .AssertActionRedirect()
                .ToAction<HelpController>(a => a.Index());
            #endregion Act

            #region Assert
            HelpTopicRepository.AssertWasCalled(a => a.EnsurePersistent(HelpTopics[5]));
            var args = (HelpTopic)HelpTopicRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<HelpTopic>.Is.Anything))[0][0];
            Assert.IsNotNull(args);
            Assert.AreEqual(6, args.Id);
            Assert.IsFalse(args.IsActive);
            Assert.IsFalse(args.AvailableToPublic);
            Assert.AreEqual(99, args.NumberOfReads);
            Assert.AreEqual("UpdateQ", args.Question);
            Assert.AreEqual("UpdateA", args.Answer);
            Assert.AreEqual("Help Topic has been saved successfully.", Controller.Message);
            #endregion Assert
        }


        /// <summary>
        /// Tests the edit post with invalid data does not save.
        /// </summary>
        [TestMethod]
        public void TestEditPostWithInvalidDataDoesNotSave()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Admin });
            SetupDataForTests();
            var helpTopic = CreateValidEntities.HelpTopic(null);
            helpTopic.Question = null; 
            #endregion Arrange

            #region Act
            Controller.Edit(6, helpTopic)
                .AssertViewRendered()
                .WithViewData<HelpTopic>();
            #endregion Act

            #region Assert
            HelpTopicRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<HelpTopic>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("Question: may not be null or empty");
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
            var result = controllerClass.BaseType.Name;
            #endregion Act

            #region Assert
            Assert.AreEqual("SuperController", result);
            #endregion Assert
        }

        /// <summary>
        /// Tests the controller has only two attributes.
        /// </summary>
        [TestMethod]
        public void TestControllerHasOnlyTwoAttributes()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            #endregion Arrange

            #region Act
            var result = controllerClass.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(2, result.Count());
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
            var expectedAttribute = controllerMethod.ElementAt(0).GetCustomAttributes(true).OfType<AdminOnlyAttribute>();
            var allAttributes = controllerMethod.ElementAt(0).GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "AdminOnlyAttribute not found");
            Assert.AreEqual(1, allAttributes.Count(), "More than expected custom attributes found.");
            #endregion Assert
        }

        /// <summary>
        /// Tests the controller method create contains expected attributes.
        /// 4
        /// </summary>
        [TestMethod]
        public void TestControllerMethodCreatePostContainsExpectedAttributes1()
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
            Assert.AreEqual(3, allAttributes.Count(), "More than expected custom attributes found.");
            #endregion Assert
        }

        /// <summary>
        /// Tests the controller method create contains expected attributes.
        /// 4
        /// </summary>
        [TestMethod]
        public void TestControllerMethodCreatePostContainsExpectedAttributes2()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "Create");
            #endregion Arrange

            #region Act
            var expectedAttribute = controllerMethod.ElementAt(1).GetCustomAttributes(true).OfType<AdminOnlyAttribute>();
            var allAttributes = controllerMethod.ElementAt(1).GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "AdminOnlyAttribute not found");
            Assert.AreEqual(3, allAttributes.Count(), "More than expected custom attributes found.");
            #endregion Assert
        }

        /// <summary>
        /// Tests the controller method create contains expected attributes.
        /// 4
        /// </summary>
        [TestMethod]
        public void TestControllerMethodCreatePostContainsExpectedAttributes3()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "Create");
            #endregion Arrange

            #region Act
            var expectedAttribute = controllerMethod.ElementAt(1).GetCustomAttributes(true).OfType<ValidateInputAttribute>();
            var allAttributes = controllerMethod.ElementAt(1).GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "ValidateInputAttribute not found");
            Assert.IsFalse(expectedAttribute.ElementAt(0).EnableValidation);
            Assert.AreEqual(3, allAttributes.Count(), "More than expected custom attributes found.");
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
            var expectedAttribute = controllerMethod.ElementAt(0).GetCustomAttributes(true).OfType<AdminOnlyAttribute>();
            var allAttributes = controllerMethod.ElementAt(0).GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "AdminOnlyAttribute not found");
            Assert.AreEqual(1, allAttributes.Count(), "More than expected custom attributes found.");
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
            var expectedAttribute = controllerMethod.ElementAt(1).GetCustomAttributes(true).OfType<AcceptPostAttribute>();
            var allAttributes = controllerMethod.ElementAt(1).GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "AcceptPostAttribute not found");
            Assert.AreEqual(3, allAttributes.Count(), "More than expected custom attributes found.");
            #endregion Assert
        }

        /// <summary>
        /// Tests the controller method Edit contains expected attributes.
        /// 6
        /// </summary>
        [TestMethod]
        public void TestControllerMethodEditPostContainsExpectedAttributes2()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "Edit");
            #endregion Arrange

            #region Act
            var expectedAttribute = controllerMethod.ElementAt(1).GetCustomAttributes(true).OfType<AdminOnlyAttribute>();
            var allAttributes = controllerMethod.ElementAt(1).GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "AdminOnlyAttribute not found");
            Assert.AreEqual(3, allAttributes.Count(), "More than expected custom attributes found.");
            #endregion Assert
        }

        /// <summary>
        /// Tests the controller method Edit contains expected attributes.
        /// 6
        /// </summary>
        [TestMethod]
        public void TestControllerMethodEditPostContainsExpectedAttributes3()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "Edit");
            #endregion Arrange

            #region Act
            var expectedAttribute = controllerMethod.ElementAt(1).GetCustomAttributes(true).OfType<ValidateInputAttribute>();
            var allAttributes = controllerMethod.ElementAt(1).GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "ValidateInputAttribute not found");
            Assert.IsFalse(expectedAttribute.ElementAt(0).EnableValidation);
            Assert.AreEqual(3, allAttributes.Count(), "More than expected custom attributes found.");
            #endregion Assert
        }

        #endregion Controller Method Tests

        #endregion Reflection Tests

        #region Helper Methods

        /// <summary>
        /// Setups the data for tests.
        /// </summary>
        private void SetupDataForTests()
        {
            for (int i = 0; i < 6; i++)
            {
                HelpTopics.Add(CreateValidEntities.HelpTopic(i + 1));
                HelpTopics[i].NumberOfReads = i;
            }
            HelpTopics[0].IsActive = false;
            HelpTopics[0].AvailableToPublic = false;
            HelpTopics[1].IsActive = false;
            HelpTopics[1].AvailableToPublic = true;
            HelpTopics[2].IsActive = true;
            HelpTopics[2].AvailableToPublic = false;
            HelpTopics[3].IsActive = true;
            HelpTopics[3].AvailableToPublic = true;
            HelpTopics[4].IsActive = true;
            HelpTopics[4].AvailableToPublic = true;
            HelpTopics[5].IsActive = true;
            HelpTopics[5].AvailableToPublic = true;
            HelpTopics[5].NumberOfReads = 1;


            ControllerRecordFakes.FakeHelpTopic(0, HelpTopicRepository, HelpTopics);
        }

        #endregion Helper Methods
        #region mocks
        /// <summary>
        /// Mock the Identity. Used for getting the current user name
        /// </summary>
        public class MockIdentity : IIdentity
        {
            public string AuthenticationType
            {
                get
                {
                    return "MockAuthentication";
                }
            }

            public bool IsAuthenticated
            {
                get
                {
                    return true;
                }
            }

            public string Name
            {
                get
                {
                    return "UserName";
                }
            }
        }


        /// <summary>
        /// Mock the Principal. Used for getting the current user name
        /// </summary>
        public class MockPrincipal : IPrincipal
        {
            IIdentity _identity;
            public bool RoleReturnValue { get; set; }
            public string[] UserRoles { get; set; }

            public MockPrincipal(string[] userRoles)
            {
                UserRoles = userRoles;
            }

            public IIdentity Identity
            {
                get { return _identity ?? (_identity = new MockIdentity()); }
            }

            public bool IsInRole(string role)
            {
                if (UserRoles.Contains(role))
                {
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Mock the HTTPContext. Used for getting the current user name
        /// </summary>
        public class MockHttpContext : HttpContextBase
        {
            private IPrincipal _user;
            private readonly int _count;
            public string[] UserRoles { get; set; }
            public MockHttpContext(int count, string[] userRoles)
            {
                _count = count;
                UserRoles = userRoles;
            }

            public override IPrincipal User
            {
                get { return _user ?? (_user = new MockPrincipal(UserRoles)); }
                set
                {
                    _user = value;
                }
            }

            public override HttpRequestBase Request
            {
                get
                {
                    return new MockHttpRequest(_count);
                }
            }
        }

        public class MockHttpRequest : HttpRequestBase
        {
            MockHttpFileCollectionBase Mocked { get; set; }

            public MockHttpRequest(int count)
            {
                Mocked = new MockHttpFileCollectionBase(count);
            }
            public override HttpFileCollectionBase Files
            {
                get
                {
                    return Mocked;
                }
            }
        }

        public class MockHttpFileCollectionBase : HttpFileCollectionBase
        {
            public int Counter { get; set; }

            public MockHttpFileCollectionBase(int count)
            {
                Counter = count;
                for (int i = 0; i < count; i++)
                {
                    BaseAdd("Test" + (i + 1), new byte[] { 4, 5, 6, 7, 8 });
                }

            }

            public override int Count
            {
                get
                {
                    return Counter;
                }
            }
            public override HttpPostedFileBase Get(string name)
            {
                return new MockHttpPostedFileBase();
            }
            public override HttpPostedFileBase this[string name]
            {
                get
                {
                    return new MockHttpPostedFileBase();
                }
            }
            public override HttpPostedFileBase this[int index]
            {
                get
                {
                    return new MockHttpPostedFileBase();
                }
            }
        }

        public class MockHttpPostedFileBase : HttpPostedFileBase
        {
            public override int ContentLength
            {
                get
                {
                    return 5;
                }
            }
            public override string FileName
            {
                get
                {
                    return "Mocked File Name";
                }
            }
            public override Stream InputStream
            {
                get
                {
                    var memStream = new MemoryStream(new byte[] { 4, 5, 6, 7, 8 });
                    return memStream;
                }
            }
        }

        #endregion
    }
}
