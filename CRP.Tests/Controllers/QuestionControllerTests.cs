using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using CRP.Controllers;
using CRP.Controllers.Helpers;
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
    /// <summary>
    /// Question Controller Tests
    /// </summary>
    [TestClass]
    public class QuestionControllerTests : ControllerTestBase<QuestionController>
    {
        protected readonly IPrincipal Principal = new MockPrincipal(new []{RoleNames.User});
        private readonly Type _controllerClass = typeof(QuestionController);
        protected List<QuestionSet> QuestionSets { get; set; }
        protected IRepository<QuestionSet> QuestionSetRepository { get; set; }
        protected List<User> Users { get; set; }
        protected IRepository<User> UserRepository { get; set; }
        protected List<QuestionType> QuestionTypes { get; set; }
        protected IRepository<QuestionType> QuestionTypeRepository { get; set; }
        protected List<Validator> Validators { get; set; }
        protected IRepository<Validator> ValidatorRepository { get; set; }
        protected List<Unit> Units { get; set; }
        protected List<School> Schools { get; set; }
        protected List<Item> Items { get; set; }
        protected List<Editor> Editors { get; set; }
        protected List<ItemQuestionSet> ItemQuestionSets { get; set; }

        #region Init
        public QuestionControllerTests()
        {
            QuestionSets = new List<QuestionSet>();
            QuestionSetRepository = FakeRepository<QuestionSet>();
            Controller.Repository.Expect(a => a.OfType<QuestionSet>()).Return(QuestionSetRepository).Repeat.Any();

            Users = new List<User>();
            UserRepository = FakeRepository<User>();
            Controller.Repository.Expect(a => a.OfType<User>()).Return(UserRepository).Repeat.Any();

            QuestionTypes = new List<QuestionType>();
            QuestionTypeRepository = FakeRepository<QuestionType>();
            Controller.Repository.Expect(a => a.OfType<QuestionType>()).Return(QuestionTypeRepository).Repeat.Any();

            Validators = new List<Validator>();
            ValidatorRepository = FakeRepository<Validator>();
            Controller.Repository.Expect(a => a.OfType<Validator>()).Return(ValidatorRepository).Repeat.Any();
            
            Editors = new List<Editor>();
            ItemQuestionSets = new List<ItemQuestionSet>();
            Items = new List<Item>();
            Units = new List<Unit>();
            Schools = new List<School>();

            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new [] {RoleNames.Admin});
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
            Controller = new TestControllerBuilder().CreateController<QuestionController>();
        }

        #endregion Init

        #region Route Tests

        /// <summary>
        /// Tests the create mapping ignore parameters.
        /// </summary>
        [TestMethod]
        public void TestCreateMappingIgnoreParameters()
        {
            "~/Question/Create/5".ShouldMapTo<QuestionController>(a => a.Create(5), true);
        }

        /// <summary>
        /// Tests the create mapping ignore parameters.
        /// </summary>
        [TestMethod]
        public void TestCreateMappingIgnoreParameters2()
        {
            "~/Question/Create/5".ShouldMapTo<QuestionController>(a => a.Create(5, new Question(), new string[1]), true);
        }

        /// <summary>
        /// Tests the delete mapping.
        /// </summary>
        [TestMethod]
        public void TestDeleteMapping()
        {
            "~/Question/Delete/5".ShouldMapTo<QuestionController>(a => a.Delete(5));
        }
        #endregion Route Tests

        #region Create Get Tests

        /// <summary>
        /// Tests the create get where question set id is not found redirects to list.
        /// </summary>
        [TestMethod]
        public void TestCreateGetWhereQuestionSetIdIsNotFoundRedirectsToList()
        {
            #region Arrange
            SetUpDataForCreateGetTests();
            QuestionSetRepository.Expect(a => a.GetNullableByID(1)).Return(null).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Create(1)
                .AssertActionRedirect()
                .ToAction<QuestionSetController>(a => a.List());
            #endregion Act

            #region Assert
            Assert.IsNull(Controller.Message);
            #endregion Assert		
        }

        /// <summary>
        /// Tests the create get where question set id I found but no access redirects to list
        /// Because is not admin and the question set is system reusable
        /// </summary>
        [TestMethod]
        public void TestCreateGetWhereQuestionSetIdIFoundButNoAccessRedirectsToList1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.User });
            SetUpDataForCreateGetTests();
            QuestionSets[0].SystemReusable = true;
            QuestionSets[0].CollegeReusable = false;
            QuestionSets[0].UserReusable = false;
            QuestionSetRepository.Expect(a => a.GetNullableByID(1)).Return(QuestionSets[0]).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Create(1)
                .AssertActionRedirect()
                .ToAction<QuestionSetController>(a => a.List());
            #endregion Act

            #region Assert
            Assert.IsNull(Controller.Message);
            #endregion Assert
        }
        /// <summary>
        /// This is the same as above except admin is true
        /// </summary>
        [TestMethod]
        public void TestCreateGetWhereQuestionSetIdIFoundButAndAccessReturnsView1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Admin });
            SetUpDataForCreateGetTests();
            QuestionSets[0].SystemReusable = true;
            QuestionSets[0].CollegeReusable = false;
            QuestionSets[0].UserReusable = false;
            QuestionSetRepository.Expect(a => a.GetNullableByID(1)).Return(QuestionSets[0]).Repeat.Any();
            #endregion Arrange

            #region Act
            var result = Controller.Create(1)
                .AssertViewRendered()
                .WithViewData<QuestionViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.QuestionTypes.Count());
            Assert.AreEqual(3, result.Validators.Count());
            #endregion Assert
        }
        /// <summary>
        /// College reusable
        /// </summary>
        [TestMethod]
        public void TestCreateGetWhereQuestionSetIdIFoundButNoAccessRedirectsToList2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.User });
            SetUpDataForCreateGetTests();
            QuestionSets[0].SystemReusable = false;
            QuestionSets[0].CollegeReusable = true;
            QuestionSets[0].UserReusable = false;
            QuestionSetRepository.Expect(a => a.GetNullableByID(1)).Return(QuestionSets[0]).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Create(1)
                .AssertActionRedirect()
                .ToAction<QuestionSetController>(a => a.List());
            #endregion Act

            #region Assert
            Assert.IsNull(Controller.Message);
            #endregion Assert
        }
        /// <summary>
        /// This is the same as above except admin is true
        /// </summary>
        [TestMethod]
        public void TestCreateGetWhereQuestionSetIdIFoundButAndAccessReturnsView2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Admin });
            SetUpDataForCreateGetTests();
            QuestionSets[0].SystemReusable = false;
            QuestionSets[0].CollegeReusable = true;
            QuestionSets[0].UserReusable = false;
            QuestionSetRepository.Expect(a => a.GetNullableByID(1)).Return(QuestionSets[0]).Repeat.Any();
            #endregion Arrange

            #region Act
            var result = Controller.Create(1)
                .AssertViewRendered()
                .WithViewData<QuestionViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.QuestionTypes.Count());
            Assert.AreEqual(3, result.Validators.Count());
            #endregion Assert
        }

        /// <summary>
        /// This is the same as above except school admin is true
        /// </summary>
        [TestMethod]
        public void TestCreateGetWhereQuestionSetIdIFoundButAndAccessReturnsView3()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.SchoolAdmin });
            SetUpDataForCreateGetTests();
            QuestionSets[0].SystemReusable = false;
            QuestionSets[0].CollegeReusable = true;
            QuestionSets[0].UserReusable = false;
            QuestionSetRepository.Expect(a => a.GetNullableByID(1)).Return(QuestionSets[0]).Repeat.Any();
            #endregion Arrange

            #region Act
            var result = Controller.Create(1)
                .AssertViewRendered()
                .WithViewData<QuestionViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.QuestionTypes.Count());
            Assert.AreEqual(3, result.Validators.Count());
            #endregion Assert
        }
        /// <summary>
        /// College reusable
        /// </summary>
        [TestMethod]
        public void TestCreateGetWhereQuestionSetIdIFoundButNoAccessRedirectsToList3()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Admin });
            SetUpDataForCreateGetTests();
            QuestionSets[0].SystemReusable = false;
            QuestionSets[0].CollegeReusable = true;
            QuestionSets[0].UserReusable = false;
            QuestionSets[0].School = Schools[2];
            QuestionSetRepository.Expect(a => a.GetNullableByID(1)).Return(QuestionSets[0]).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Create(1)
                .AssertActionRedirect()
                .ToAction<QuestionSetController>(a => a.List());
            #endregion Act

            #region Assert
            Assert.IsNull(Controller.Message);
            #endregion Assert
        }
        /// <summary>
        /// College reusable
        /// </summary>
        [TestMethod]
        public void TestCreateGetWhereQuestionSetIdIFoundButNoAccessRedirectsToList4()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.SchoolAdmin });
            SetUpDataForCreateGetTests();
            QuestionSets[0].SystemReusable = false;
            QuestionSets[0].CollegeReusable = true;
            QuestionSets[0].UserReusable = false;
            QuestionSets[0].School = Schools[2];
            QuestionSetRepository.Expect(a => a.GetNullableByID(1)).Return(QuestionSets[0]).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Create(1)
                .AssertActionRedirect()
                .ToAction<QuestionSetController>(a => a.List());
            #endregion Act

            #region Assert
            Assert.IsNull(Controller.Message);
            #endregion Assert
        }

        /// <summary>
        /// User reusable
        /// </summary>
        [TestMethod]
        public void TestCreateGetWhereQuestionSetIdIFoundButNoAccessRedirectsToList5()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.User });
            SetUpDataForCreateGetTests();
            QuestionSets[0].SystemReusable = false;
            QuestionSets[0].CollegeReusable = false;
            QuestionSets[0].UserReusable = true;
            QuestionSets[0].User = Users[0]; //Not the owner
            QuestionSetRepository.Expect(a => a.GetNullableByID(1)).Return(QuestionSets[0]).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Create(1)
                .AssertActionRedirect()
                .ToAction<QuestionSetController>(a => a.List());
            #endregion Act

            #region Assert
            Assert.IsNull(Controller.Message);
            #endregion Assert
        }
        /// <summary>
        /// This is the same as above except User
        /// </summary>
        [TestMethod]
        public void TestCreateGetWhereQuestionSetIdIFoundButAndAccessReturnsView4()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.User });
            SetUpDataForCreateGetTests();
            QuestionSets[0].SystemReusable = false;
            QuestionSets[0].CollegeReusable = false;
            QuestionSets[0].UserReusable = true;
            //QuestionSets[0].User = Users[0]; //Not the owner
            QuestionSetRepository.Expect(a => a.GetNullableByID(1)).Return(QuestionSets[0]).Repeat.Any();
            #endregion Arrange

            #region Act
            var result = Controller.Create(1)
                .AssertViewRendered()
                .WithViewData<QuestionViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.QuestionTypes.Count());
            Assert.AreEqual(3, result.Validators.Count());
            #endregion Assert
        }

        /// <summary>
        /// no reusable, not admin, and has an item type
        /// </summary>
        [TestMethod]
        public void TestCreateGetWhereQuestionSetIdIFoundButNoAccessRedirectsToList6()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.User });
            SetUpDataForCreateGetTests();
            QuestionSets[0].SystemReusable = false;
            QuestionSets[0].CollegeReusable = false;
            QuestionSets[0].UserReusable = false;
            QuestionSets[0].User = Users[0]; //Not the owner
            QuestionSets[0].ItemTypes.Add(CreateValidEntities.ItemTypeQuestionSet(null));
            QuestionSetRepository.Expect(a => a.GetNullableByID(1)).Return(QuestionSets[0]).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Create(1)
                .AssertActionRedirect()
                .ToAction<QuestionSetController>(a => a.List());
            #endregion Act

            #region Assert
            Assert.IsNull(Controller.Message);
            #endregion Assert
        }

        /// <summary>
        /// Same as above except admin
        /// </summary>
        [TestMethod]
        public void TestCreateGetWhereQuestionSetIdIFoundButAndAccessReturnsView5()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Admin });
            SetUpDataForCreateGetTests();
            QuestionSets[0].SystemReusable = false;
            QuestionSets[0].CollegeReusable = false;
            QuestionSets[0].UserReusable = false;
            QuestionSets[0].User = Users[0]; //Not the owner
            QuestionSets[0].ItemTypes.Add(CreateValidEntities.ItemTypeQuestionSet(null));
            QuestionSetRepository.Expect(a => a.GetNullableByID(1)).Return(QuestionSets[0]).Repeat.Any();
            #endregion Arrange

            #region Act
            var result = Controller.Create(1)
                .AssertViewRendered()
                .WithViewData<QuestionViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.QuestionTypes.Count());
            Assert.AreEqual(3, result.Validators.Count());
            #endregion Assert
        }


        /// <summary>
        /// Has item access test
        /// </summary>
        [TestMethod]
        public void TestCreateGetWhereQuestionSetIdIFoundButNoAccessRedirectsToList7()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.User });
            SetUpDataForCreateGetTests();
            QuestionSets[0].SystemReusable = false;
            QuestionSets[0].CollegeReusable = false;
            QuestionSets[0].UserReusable = false;
            QuestionSets[0].User = Users[0]; //Not the owner
            //QuestionSets[0].ItemTypes.Add(CreateValidEntities.ItemTypeQuestionSet(null));
            QuestionSetRepository.Expect(a => a.GetNullableByID(1)).Return(QuestionSets[0]).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Create(1)
                .AssertActionRedirect()
                .ToAction<QuestionSetController>(a => a.List());
            #endregion Act

            #region Assert
            Assert.IsNull(Controller.Message);
            #endregion Assert
        }

        /// <summary>
        /// Same as above except Current use is an editor
        /// </summary>
        [TestMethod]
        public void TestCreateGetWhereQuestionSetIdIFoundButAndAccessReturnsView6()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.User });
            SetUpDataForCreateGetTests();
            QuestionSets[0].SystemReusable = false;
            QuestionSets[0].CollegeReusable = false;
            QuestionSets[0].UserReusable = false;
            QuestionSets[0].User = Users[0]; //Not the owner
            //QuestionSets[0].ItemTypes.Add(CreateValidEntities.ItemTypeQuestionSet(null));
            QuestionSetRepository.Expect(a => a.GetNullableByID(1)).Return(QuestionSets[0]).Repeat.Any();
            Items[0].AddEditor(Editors[1]);
            #endregion Arrange

            #region Act
            var result = Controller.Create(1)
                .AssertViewRendered()
                .WithViewData<QuestionViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.QuestionTypes.Count());
            Assert.AreEqual(3, result.Validators.Count());
            #endregion Assert
        }

        /// <summary>
        /// Same as above except Current use is an admin
        /// </summary>
        [TestMethod]
        public void TestCreateGetWhereQuestionSetIdIFoundButAndAccessReturnsView7()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.User, RoleNames.Admin });
            SetUpDataForCreateGetTests();
            QuestionSets[0].SystemReusable = false;
            QuestionSets[0].CollegeReusable = false;
            QuestionSets[0].UserReusable = false;
            QuestionSets[0].User = Users[0]; //Not the owner
            //QuestionSets[0].ItemTypes.Add(CreateValidEntities.ItemTypeQuestionSet(null));
            QuestionSetRepository.Expect(a => a.GetNullableByID(1)).Return(QuestionSets[0]).Repeat.Any();
            //Items[0].AddEditor(Editors[1]); //Not an editor
            #endregion Arrange

            #region Act
            var result = Controller.Create(1)
                .AssertViewRendered()
                .WithViewData<QuestionViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.QuestionTypes.Count());
            Assert.AreEqual(3, result.Validators.Count());
            #endregion Assert
        }

        #endregion Create Get Tests 

        #region Create Post Tests

        [TestMethod]
        public void TestCreatePostRedirectsToListIfTheQuestionsSetIsNotFound()
        {
            #region Arrange
            QuestionSetRepository.Expect(a => a.GetNullableByID(1)).Return(null).Repeat.Any();
            #endregion Arrange

            #region Act/Assert
            Controller.Create(1, CreateValidEntities.Question(null), new[] {"Option1, option2"})
                .AssertActionRedirect()
                .ToAction<QuestionSetController>(a => a.List());
            #endregion Act/Assert
        }

        #region Create Post Redirects To List Because Of HasQuestionSetAccess
        [TestMethod]
        public void TestCreatePostWhereQuestionSetIdIFoundButNoAccessRedirectsToList1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.User });
            SetUpDataForCreateGetTests();
            QuestionSets[0].SystemReusable = true;
            QuestionSets[0].CollegeReusable = false;
            QuestionSets[0].UserReusable = false;
            QuestionSetRepository.Expect(a => a.GetNullableByID(1)).Return(QuestionSets[0]).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Create(1, CreateValidEntities.Question(null), new[]{"option"})
                .AssertActionRedirect()
                .ToAction<QuestionSetController>(a => a.List());
            #endregion Act

            #region Assert
            Assert.IsNull(Controller.Message);
            #endregion Assert
        }
       
        /// <summary>
        /// College reusable
        /// </summary>
        [TestMethod]
        public void TestCreatePostWhereQuestionSetIdIFoundButNoAccessRedirectsToList2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.User });
            SetUpDataForCreateGetTests();
            QuestionSets[0].SystemReusable = false;
            QuestionSets[0].CollegeReusable = true;
            QuestionSets[0].UserReusable = false;
            QuestionSetRepository.Expect(a => a.GetNullableByID(1)).Return(QuestionSets[0]).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Create(1, CreateValidEntities.Question(null), new[] { "option" })
                .AssertActionRedirect()
                .ToAction<QuestionSetController>(a => a.List());
            #endregion Act

            #region Assert
            Assert.IsNull(Controller.Message);
            #endregion Assert
        }

        /// <summary>
        /// College reusable
        /// </summary>
        [TestMethod]
        public void TestCreatePostWhereQuestionSetIdIFoundButNoAccessRedirectsToList3()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Admin });
            SetUpDataForCreateGetTests();
            QuestionSets[0].SystemReusable = false;
            QuestionSets[0].CollegeReusable = true;
            QuestionSets[0].UserReusable = false;
            QuestionSets[0].School = Schools[2];
            QuestionSetRepository.Expect(a => a.GetNullableByID(1)).Return(QuestionSets[0]).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Create(1, CreateValidEntities.Question(null), new[] { "option" })
                .AssertActionRedirect()
                .ToAction<QuestionSetController>(a => a.List());
            #endregion Act

            #region Assert
            Assert.IsNull(Controller.Message);
            #endregion Assert
        }

        /// <summary>
        /// College reusable
        /// </summary>
        [TestMethod]
        public void TestCreatePostWhereQuestionSetIdIFoundButNoAccessRedirectsToList4()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.SchoolAdmin });
            SetUpDataForCreateGetTests();
            QuestionSets[0].SystemReusable = false;
            QuestionSets[0].CollegeReusable = true;
            QuestionSets[0].UserReusable = false;
            QuestionSets[0].School = Schools[2];
            QuestionSetRepository.Expect(a => a.GetNullableByID(1)).Return(QuestionSets[0]).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Create(1, CreateValidEntities.Question(null), new[] { "option" })
                .AssertActionRedirect()
                .ToAction<QuestionSetController>(a => a.List());
            #endregion Act

            #region Assert
            Assert.IsNull(Controller.Message);
            #endregion Assert
        }

        /// <summary>
        /// User reusable
        /// </summary>
        [TestMethod]
        public void TestCreatePostWhereQuestionSetIdIFoundButNoAccessRedirectsToList5()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.User });
            SetUpDataForCreateGetTests();
            QuestionSets[0].SystemReusable = false;
            QuestionSets[0].CollegeReusable = false;
            QuestionSets[0].UserReusable = true;
            QuestionSets[0].User = Users[0]; //Not the owner
            QuestionSetRepository.Expect(a => a.GetNullableByID(1)).Return(QuestionSets[0]).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Create(1, CreateValidEntities.Question(null), new[] { "option" })
                .AssertActionRedirect()
                .ToAction<QuestionSetController>(a => a.List());
            #endregion Act

            #region Assert
            Assert.IsNull(Controller.Message);
            #endregion Assert
        }
       
        /// <summary>
        /// no reusable, not admin, and has an item type
        /// </summary>
        [TestMethod]
        public void TestCreatePostWhereQuestionSetIdIFoundButNoAccessRedirectsToList6()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.User });
            SetUpDataForCreateGetTests();
            QuestionSets[0].SystemReusable = false;
            QuestionSets[0].CollegeReusable = false;
            QuestionSets[0].UserReusable = false;
            QuestionSets[0].User = Users[0]; //Not the owner
            QuestionSets[0].ItemTypes.Add(CreateValidEntities.ItemTypeQuestionSet(null));
            QuestionSetRepository.Expect(a => a.GetNullableByID(1)).Return(QuestionSets[0]).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Create(1, CreateValidEntities.Question(null), new[] { "option" })
                .AssertActionRedirect()
                .ToAction<QuestionSetController>(a => a.List());
            #endregion Act

            #region Assert
            Assert.IsNull(Controller.Message);
            #endregion Assert
        }

        [TestMethod]
        public void TestCreatePostWhereQuestionSetIdIFoundButNoAccessRedirectsToList7()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.User });
            SetUpDataForCreateGetTests();
            QuestionSets[0].SystemReusable = false;
            QuestionSets[0].CollegeReusable = false;
            QuestionSets[0].UserReusable = false;
            QuestionSets[0].User = Users[0]; //Not the owner
            //QuestionSets[0].ItemTypes.Add(CreateValidEntities.ItemTypeQuestionSet(null));
            QuestionSetRepository.Expect(a => a.GetNullableByID(1)).Return(QuestionSets[0]).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Create(1, CreateValidEntities.Question(null), new[] { "option" })
                .AssertActionRedirect()
                .ToAction<QuestionSetController>(a => a.List());
            #endregion Act

            #region Assert
            Assert.IsNull(Controller.Message);
            #endregion Assert
        }
        #endregion Create Post Redirects To List Because Of HasQuestionSetAccess

        /// <summary>
        /// If a questionSet is reusable, and has been used, we should not be able to add a question
        /// </summary>
        [TestMethod]
        public void TestCreatePostRedirectsToEditWhenQuestionSetIsUsedAndReusable()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.User, RoleNames.Admin });
            SetUpDataForCreateGetTests();
            QuestionSets[0].SystemReusable = false;
            QuestionSets[0].CollegeReusable = false;
            QuestionSets[0].UserReusable = true;
            QuestionSets[0].Items.Add(ItemQuestionSets[0]);

            QuestionSetRepository.Expect(a => a.GetNullableByID(1)).Return(QuestionSets[0]).Repeat.Any();
            #endregion Arrange

            #region Act

            Controller.Create(1, CreateValidEntities.Question(null), new[] {"Option1, option2"})
                .AssertActionRedirect()
                .ToAction<QuestionSetController>(a => a.Edit(1));
            #endregion Act

            #region Assert
            Assert.AreEqual("Question cannot be added to the question set because it is already being used by an item.", Controller.Message);
            #endregion Assert		
        }
        #endregion Create Post Tests

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
        /// Tests the controller has handle transactions manually attribute.
        /// </summary>
        [TestMethod]
        public void TestControllerHasAuthorizeAttribute()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            #endregion Arrange

            #region Act
            var result = controllerClass.GetCustomAttributes(true).OfType<AuthorizeAttribute>();
            #endregion Act

            #region Assert
            Assert.IsTrue(result.Count() > 0, "AuthorizeAttribute not found.");
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
            Assert.AreEqual(3, result.Count(), "It looks like a method was added or removed from the controller.");
            #endregion Assert
        }


        /// <summary>
        /// Tests the controller method create  contains expected attributes.
        /// </summary>
        [TestMethod]
        public void TestControllerMethodCreateContainsExpectedAttributes1()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "Create");
            #endregion Arrange

            #region Act
            var allAttributes = controllerMethod.ElementAt(0).GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, allAttributes.Count());
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
            var expectedAttribute = controllerMethod.ElementAt(1).GetCustomAttributes(true).OfType<AcceptPostAttribute>();
            var allAttributes = controllerMethod.ElementAt(1).GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "AcceptPostAttribute not found");
            Assert.AreEqual(1, allAttributes.Count(), "More than expected custom attributes found.");
            #endregion Assert
        }


        /// <summary>
        /// Tests the controller method get extended properties contains expected attributes.
        /// </summary>
        [TestMethod]
        public void TestControllerMethodDeleteContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethod("Delete");
            #endregion Arrange

            #region Act
            var expectedAttribute = controllerMethod.GetCustomAttributes(true).OfType<AcceptPostAttribute>();
            var allAttributes = controllerMethod.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "AcceptPostAttribute not found");
            Assert.AreEqual(1, allAttributes.Count(), "More than expected custom attributes found.");
            #endregion Assert
        }

      
        #endregion Controller Method Tests

        #endregion Reflection Tests

        #region Helper Methods

        private void SetUpDataForCreateGetTests()
        {
            ControllerRecordFakes.FakeQuestionSets(QuestionSets, 1);
            ControllerRecordFakes.FakeUsers(Users,3);
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes, 3);
            ControllerRecordFakes.FakeValidators(Validators, 3);
            ControllerRecordFakes.FakeUnits(Units, 2);
            ControllerRecordFakes.FakeSchools(Schools, 3);
            ControllerRecordFakes.FakeItems(Items, 1);
            ControllerRecordFakes.FakeEditors(Editors,2);
            ControllerRecordFakes.FakeItemQuestionSets(ItemQuestionSets, 1);
            Editors[0].User = Users[0];
            Editors[1].User = Users[1]; //Current user, but don't add to the item here.
            Items[0].AddEditor(Editors[0]);
            Units[0].School = Schools[0];
            Units[1].School = Schools[1];
            Users[1].LoginID = "UserName";
            Users[1].Units.Add(Units[0]);
            Users[1].Units.Add(Units[1]);
            QuestionSets[0].School = Schools[1]; //Exists
            QuestionSets[0].User = Users[1];
            ItemQuestionSets[0].Item = Items[0];
            QuestionSets[0].Items.Add(ItemQuestionSets[0]);
            UserRepository.Expect(a => a.Queryable).Return(Users.AsQueryable()).Repeat.Any();
            QuestionTypeRepository.Expect(a => a.GetAll()).Return(QuestionTypes).Repeat.Any();
            ValidatorRepository.Expect(a => a.GetAll()).Return(Validators).Repeat.Any();
        }


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
                get
                {
                    if (_identity == null)
                    {
                        _identity = new MockIdentity();
                    }
                    return _identity;
                }
            }

            public bool IsInRole(string role)
            {
                if(UserRoles.Contains(role))
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
            private int _count;
            public string[] UserRoles { get; set; }
            public MockHttpContext(int count, string[] userRoles)
            {
                _count = count;
                UserRoles = userRoles;
            }

            public override IPrincipal User
            {
                get
                {
                    if (_user == null)
                    {
                        _user = new MockPrincipal(UserRoles);
                    }
                    return _user;
                }
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

        #endregion Helper Methods
    }
}
