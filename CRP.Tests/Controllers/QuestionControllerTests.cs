using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using CRP.Controllers;
using CRP.Controllers.Helpers;
using CRP.Controllers.ViewModels;
using CRP.Core.Domain;
using CRP.Core.Resources;
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
        protected List<Question> Questions { get; set; }
        protected IRepository<Question> QuestionRepository { get; set; }
        protected IRepository<TransactionAnswer> TransactionAnswerRepository { get; set; }
        protected IRepository<QuantityAnswer> QuantityAnswerRepository { get; set; }

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

            TransactionAnswerRepository = FakeRepository<TransactionAnswer>();
            Controller.Repository.Expect(a => a.OfType<TransactionAnswer>()).Return(TransactionAnswerRepository).Repeat.Any();

            QuantityAnswerRepository = FakeRepository<QuantityAnswer>();
            Controller.Repository.Expect(a => a.OfType<QuantityAnswer>()).Return(QuantityAnswerRepository).Repeat.Any();
            
            Editors = new List<Editor>();
            ItemQuestionSets = new List<ItemQuestionSet>();
            Items = new List<Item>();
            Units = new List<Unit>();
            Schools = new List<School>();

            Questions = new List<Question>();
            QuestionRepository = FakeRepository<Question>();
            Controller.Repository.Expect(a => a.OfType<Question>()).Return(QuestionRepository).Repeat.Any();

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
            QuestionSetRepository.Expect(a => a.GetNullableById(1)).Return(null).Repeat.Any();
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
            QuestionSetRepository.Expect(a => a.GetNullableById(1)).Return(QuestionSets[0]).Repeat.Any();
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
            QuestionSetRepository.Expect(a => a.GetNullableById(1)).Return(QuestionSets[0]).Repeat.Any();
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
            QuestionSetRepository.Expect(a => a.GetNullableById(1)).Return(QuestionSets[0]).Repeat.Any();
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
            QuestionSetRepository.Expect(a => a.GetNullableById(1)).Return(QuestionSets[0]).Repeat.Any();
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
            QuestionSetRepository.Expect(a => a.GetNullableById(1)).Return(QuestionSets[0]).Repeat.Any();
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
            QuestionSetRepository.Expect(a => a.GetNullableById(1)).Return(QuestionSets[0]).Repeat.Any();
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
            QuestionSetRepository.Expect(a => a.GetNullableById(1)).Return(QuestionSets[0]).Repeat.Any();
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
            QuestionSetRepository.Expect(a => a.GetNullableById(1)).Return(QuestionSets[0]).Repeat.Any();
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
            QuestionSetRepository.Expect(a => a.GetNullableById(1)).Return(QuestionSets[0]).Repeat.Any();
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
            QuestionSetRepository.Expect(a => a.GetNullableById(1)).Return(QuestionSets[0]).Repeat.Any();
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
            QuestionSetRepository.Expect(a => a.GetNullableById(1)).Return(QuestionSets[0]).Repeat.Any();
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
            QuestionSetRepository.Expect(a => a.GetNullableById(1)).Return(QuestionSets[0]).Repeat.Any();
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
            QuestionSetRepository.Expect(a => a.GetNullableById(1)).Return(QuestionSets[0]).Repeat.Any();
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
            QuestionSetRepository.Expect(a => a.GetNullableById(1)).Return(QuestionSets[0]).Repeat.Any();
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
            QuestionSetRepository.Expect(a => a.GetNullableById(1)).Return(null).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Create(1, CreateValidEntities.Question(null), new[] {"Option1, option2"})
                .AssertActionRedirect()
                .ToAction<QuestionSetController>(a => a.List());
            #endregion Act

            #region Assert
            QuestionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
            #endregion Assert
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
            QuestionSetRepository.Expect(a => a.GetNullableById(1)).Return(QuestionSets[0]).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Create(1, CreateValidEntities.Question(null), new[]{"option"})
                .AssertActionRedirect()
                .ToAction<QuestionSetController>(a => a.List());
            #endregion Act

            #region Assert
            Assert.IsNull(Controller.Message);
            QuestionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
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
            QuestionSetRepository.Expect(a => a.GetNullableById(1)).Return(QuestionSets[0]).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Create(1, CreateValidEntities.Question(null), new[] { "option" })
                .AssertActionRedirect()
                .ToAction<QuestionSetController>(a => a.List());
            #endregion Act

            #region Assert
            Assert.IsNull(Controller.Message);
            QuestionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
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
            QuestionSetRepository.Expect(a => a.GetNullableById(1)).Return(QuestionSets[0]).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Create(1, CreateValidEntities.Question(null), new[] { "option" })
                .AssertActionRedirect()
                .ToAction<QuestionSetController>(a => a.List());
            #endregion Act

            #region Assert
            Assert.IsNull(Controller.Message);
            QuestionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
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
            QuestionSetRepository.Expect(a => a.GetNullableById(1)).Return(QuestionSets[0]).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Create(1, CreateValidEntities.Question(null), new[] { "option" })
                .AssertActionRedirect()
                .ToAction<QuestionSetController>(a => a.List());
            #endregion Act

            #region Assert
            Assert.IsNull(Controller.Message);
            QuestionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
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
            QuestionSetRepository.Expect(a => a.GetNullableById(1)).Return(QuestionSets[0]).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Create(1, CreateValidEntities.Question(null), new[] { "option" })
                .AssertActionRedirect()
                .ToAction<QuestionSetController>(a => a.List());
            #endregion Act

            #region Assert
            Assert.IsNull(Controller.Message);
            QuestionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
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
            QuestionSetRepository.Expect(a => a.GetNullableById(1)).Return(QuestionSets[0]).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Create(1, CreateValidEntities.Question(null), new[] { "option" })
                .AssertActionRedirect()
                .ToAction<QuestionSetController>(a => a.List());
            #endregion Act

            #region Assert
            Assert.IsNull(Controller.Message);
            QuestionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
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
            QuestionSetRepository.Expect(a => a.GetNullableById(1)).Return(QuestionSets[0]).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Create(1, CreateValidEntities.Question(null), new[] { "option" })
                .AssertActionRedirect()
                .ToAction<QuestionSetController>(a => a.List());
            #endregion Act

            #region Assert
            Assert.IsNull(Controller.Message);
            QuestionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
            #endregion Assert
        }
        #endregion Create Post Redirects To List Because Of HasQuestionSetAccess

        /// <summary>
        /// If a questionSet is reusable, and has been used, we should not be able to add a question
        /// </summary>
        [TestMethod]
        public void TestCreatePostRedirectsToEditWhenQuestionSetIsUsedAndReusable1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.User, RoleNames.Admin });
            SetUpDataForCreateGetTests();
            QuestionSets[0].SystemReusable = false;
            QuestionSets[0].CollegeReusable = false;
            QuestionSets[0].UserReusable = true;
            QuestionSets[0].Items.Add(ItemQuestionSets[0]);

            QuestionSetRepository.Expect(a => a.GetNullableById(1)).Return(QuestionSets[0]).Repeat.Any();
            #endregion Arrange

            #region Act

            Controller.Create(1, CreateValidEntities.Question(null), new[] {"Option1, option2"})
                .AssertActionRedirect()
                .ToAction<QuestionSetController>(a => a.Edit(1));
            #endregion Act

            #region Assert
            Assert.AreEqual("Question cannot be added to the question set because it is already being used by an item.", Controller.Message);
            QuestionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
            #endregion Assert		
        }

        /// <summary>
        /// If a questionSet is reusable, and has been used, we should not be able to add a question
        /// </summary>
        [TestMethod]
        public void TestCreatePostRedirectsToEditWhenQuestionSetIsUsedAndReusable2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.User, RoleNames.Admin });
            SetUpDataForCreateGetTests();
            QuestionSets[0].SystemReusable = false;
            QuestionSets[0].CollegeReusable = true;
            QuestionSets[0].UserReusable = false;
            QuestionSets[0].Items.Add(ItemQuestionSets[0]);

            QuestionSetRepository.Expect(a => a.GetNullableById(1)).Return(QuestionSets[0]).Repeat.Any();
            #endregion Arrange

            #region Act

            Controller.Create(1, CreateValidEntities.Question(null), new[] { "Option1, option2" })
                .AssertActionRedirect()
                .ToAction<QuestionSetController>(a => a.Edit(1));
            #endregion Act

            #region Assert
            Assert.AreEqual("Question cannot be added to the question set because it is already being used by an item.", Controller.Message);
            QuestionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
            #endregion Assert
        }
        /// <summary>
        /// If a questionSet is reusable, and has been used, we should not be able to add a question
        /// </summary>
        [TestMethod]
        public void TestCreatePostRedirectsToEditWhenQuestionSetIsUsedAndReusable3()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.User, RoleNames.Admin });
            SetUpDataForCreateGetTests();
            QuestionSets[0].SystemReusable = true;
            QuestionSets[0].CollegeReusable = false;
            QuestionSets[0].UserReusable = false;
            QuestionSets[0].Items.Add(ItemQuestionSets[0]);

            QuestionSetRepository.Expect(a => a.GetNullableById(1)).Return(QuestionSets[0]).Repeat.Any();
            #endregion Arrange

            #region Act

            Controller.Create(1, CreateValidEntities.Question(null), new[] { "Option1, option2" })
                .AssertActionRedirect()
                .ToAction<QuestionSetController>(a => a.Edit(1));
            #endregion Act

            #region Assert
            Assert.AreEqual("Question cannot be added to the question set because it is already being used by an item.", Controller.Message);
            QuestionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
            #endregion Assert
        }

        #region Create Post Invalid Tests

        /// <summary>
        /// Tests the create post when system contact does not save.
        /// </summary>
        [TestMethod]
        public void TestCreatePostWhenSystemContactDoesNotSave()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.User, RoleNames.Admin });
            SetUpDataForCreateGetTests();
            QuestionSets[0].SystemReusable = true;
            QuestionSets[0].CollegeReusable = false;
            QuestionSets[0].UserReusable = false;

            QuestionSets[0].Items = new List<ItemQuestionSet>();
            QuestionSets[0].Name = StaticValues.QuestionSet_ContactInformation;
            var questionToAdd = CreateValidEntities.Question(null);
            questionToAdd.QuestionType = CreateValidEntities.QuestionType(null);
            questionToAdd.QuestionType.HasOptions = false; //Not testing this part here.

            QuestionSetRepository.Expect(a => a.GetNullableById(1)).Return(QuestionSets[0]).Repeat.Any();            
            #endregion Arrange

            #region Act
            var result = Controller.Create(1, questionToAdd, null)
                .AssertViewRendered()
                .WithViewData<QuestionViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Controller.ModelState.AssertErrorsAre("This is a system default question set and cannot be modified.");
            Assert.AreSame(questionToAdd, result.Question);
            Assert.AreSame(QuestionSets[0], result.QuestionSet);
            QuestionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
            #endregion Assert		
        }

        /// <summary>
        /// Tests the create post when question name is empty does not save.
        /// </summary>
        [TestMethod]
        public void TestCreatePostWhenQuestionNameIsEmptyDoesNotSave()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.User, RoleNames.Admin });
            SetUpDataForCreateGetTests();
            QuestionSets[0].SystemReusable = false;
            QuestionSets[0].CollegeReusable = false;
            QuestionSets[0].UserReusable = false;
          
            var questionToAdd = CreateValidEntities.Question(null);
            questionToAdd.QuestionType = CreateValidEntities.QuestionType(null);
            questionToAdd.QuestionType.HasOptions = false; //Not testing this part here.

            questionToAdd.Name = string.Empty;

            QuestionSetRepository.Expect(a => a.GetNullableById(1)).Return(QuestionSets[0]).Repeat.Any();
            #endregion Arrange

            #region Act
            var result = Controller.Create(1, questionToAdd, null)
                .AssertViewRendered()
                .WithViewData<QuestionViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Controller.ModelState.AssertErrorsAre("Name: may not be null or empty");
            Assert.AreSame(questionToAdd, result.Question);
            Assert.AreSame(QuestionSets[0], result.QuestionSet);
            QuestionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
            #endregion Assert
        }
        #endregion Create Post Invalid Tests

        #region Create Post Valid Tests

        /// <summary>
        /// Tests the create post with valid question and no options saves.
        /// </summary>
        [TestMethod]
        public void TestCreatePostWithValidQuestionAndNoOptionsSaves()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.User, RoleNames.Admin });
            SetUpDataForCreateGetTests();
            QuestionSets[0].SystemReusable = false;
            QuestionSets[0].CollegeReusable = false;
            QuestionSets[0].UserReusable = false;

            var questionToAdd = CreateValidEntities.Question(null);
            questionToAdd.QuestionType = CreateValidEntities.QuestionType(null);
            questionToAdd.QuestionType.HasOptions = false; //Not testing this part here.

            QuestionSetRepository.Expect(a => a.GetNullableById(1)).Return(QuestionSets[0]).Repeat.Any();
            #endregion Arrange

            #region Act
            var result = Controller.Create(1, questionToAdd, null)
                .AssertActionRedirect()
                .ToAction<QuestionSetController>(a => a.Edit(1));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            QuestionRepository.AssertWasCalled(a => a.EnsurePersistent(questionToAdd));
            #endregion Assert
        }

        /// <summary>
        /// Tests the create post with valid question and options saves.
        /// </summary>
        [TestMethod]
        public void TestCreatePostWithValidQuestionAndOptionsSaves()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.User, RoleNames.Admin });
            SetUpDataForCreateGetTests();
            QuestionSets[0].SystemReusable = false;
            QuestionSets[0].CollegeReusable = false;
            QuestionSets[0].UserReusable = false;

            var questionToAdd = CreateValidEntities.Question(null);
            questionToAdd.QuestionType = CreateValidEntities.QuestionType(null);
            questionToAdd.QuestionType.HasOptions = true; 

            QuestionSetRepository.Expect(a => a.GetNullableById(1)).Return(QuestionSets[0]).Repeat.Any();
            #endregion Arrange

            #region Act
            var result = Controller.Create(1, questionToAdd, new[]{"Option1", "Option3"})
                .AssertActionRedirect()
                .ToAction<QuestionSetController>(a => a.Edit(1));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, questionToAdd.Options.Count);
            Assert.AreEqual("Option1", questionToAdd.Options.ElementAt(0).Name);
            Assert.AreEqual("Option3", questionToAdd.Options.ElementAt(1).Name);
            QuestionRepository.AssertWasCalled(a => a.EnsurePersistent(questionToAdd));
            #endregion Assert
        }

        /// <summary>
        /// Tests the create post with valid question and options saves and ignores empty options.
        /// </summary>
        [TestMethod]
        public void TestCreatePostWithValidQuestionAndOptionsSavesAndIgnoresEmptyOptions()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.User, RoleNames.Admin });
            SetUpDataForCreateGetTests();
            QuestionSets[0].SystemReusable = false;
            QuestionSets[0].CollegeReusable = false;
            QuestionSets[0].UserReusable = false;

            var questionToAdd = CreateValidEntities.Question(null);
            questionToAdd.QuestionType = CreateValidEntities.QuestionType(null);
            questionToAdd.QuestionType.HasOptions = true;

            QuestionSetRepository.Expect(a => a.GetNullableById(1)).Return(QuestionSets[0]).Repeat.Any();
            #endregion Arrange

            #region Act
            var result = Controller.Create(1, questionToAdd, new[] { "Option1","", "Option3" })
                .AssertActionRedirect()
                .ToAction<QuestionSetController>(a => a.Edit(1));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, questionToAdd.Options.Count);
            Assert.AreEqual("Option1", questionToAdd.Options.ElementAt(0).Name);
            Assert.AreEqual("Option3", questionToAdd.Options.ElementAt(1).Name);
            QuestionRepository.AssertWasCalled(a => a.EnsurePersistent(questionToAdd));
            #endregion Assert
        }

        /// <summary>
        /// Tests the create post with valid question set model state message.
        /// </summary>
        [TestMethod]
        public void TestCreatePostWithValidQuestionSetModelStateMessage()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.User, RoleNames.Admin });
            SetUpDataForCreateGetTests();
            QuestionSets[0].SystemReusable = false;
            QuestionSets[0].CollegeReusable = false;
            QuestionSets[0].UserReusable = false;

            var questionToAdd = CreateValidEntities.Question(null);
            questionToAdd.QuestionType = CreateValidEntities.QuestionType(null);
            questionToAdd.QuestionType.HasOptions = true;

            QuestionSetRepository.Expect(a => a.GetNullableById(1)).Return(QuestionSets[0]).Repeat.Any();
            #endregion Arrange

            #region Act
            var result = Controller.Create(1, questionToAdd, new[] {"Option3" })
                .AssertActionRedirect()
                .ToAction<QuestionSetController>(a => a.Edit(1));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            QuestionRepository.AssertWasCalled(a => a.EnsurePersistent(questionToAdd));
            Assert.AreEqual("Question has been created successfully.", Controller.Message);
            #endregion Assert
        }
        #endregion Create Post Valid Tests

        #region Create Post Validator Tests

        #region Text Box Tests
        #region Valid Validators
        [TestMethod]
        public void TestTextBoxWithNoValidatorsSaves()
        {
            #region Arrange
            SetUpDataForValidatorTests();

            var questionToAdd = CreateValidEntities.Question(null);
            questionToAdd.QuestionType = QuestionTypes.Where(a => a.Name == "Text Box").Single();
            questionToAdd.QuestionSet = QuestionSets[0];
            #endregion Arrange

            #region Act
            var result = Controller.Create(1, questionToAdd, new string[0])
                .AssertActionRedirect()
                .ToAction<QuestionSetController>(a => a.Edit(1));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            QuestionRepository.AssertWasCalled(a => a.EnsurePersistent(questionToAdd));
            Assert.AreEqual("Question has been created successfully.", Controller.Message);
            #endregion Assert		
        }

        [TestMethod]
        public void TestTextBoxWithRequiredValidatorOnlySaves()
        {
            #region Arrange
            SetUpDataForValidatorTests();

            var questionToAdd = CreateValidEntities.Question(null);
            questionToAdd.QuestionType = QuestionTypes.Where(a => a.Name == "Text Box").Single();
            questionToAdd.QuestionSet = QuestionSets[0];
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Required").Single());
            #endregion Arrange

            #region Act
            var result = Controller.Create(1, questionToAdd, new string[0])
                .AssertActionRedirect()
                .ToAction<QuestionSetController>(a => a.Edit(1));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            QuestionRepository.AssertWasCalled(a => a.EnsurePersistent(questionToAdd));
            Assert.AreEqual("Question has been created successfully.", Controller.Message);
            #endregion Assert
        }

        [TestMethod]
        public void TestTextBoxWithRequiredValidatorAndOneMoreSaves1()
        {
            #region Arrange
            SetUpDataForValidatorTests();

            var questionToAdd = CreateValidEntities.Question(null);
            questionToAdd.QuestionType = QuestionTypes.Where(a => a.Name == "Text Box").Single();
            questionToAdd.QuestionSet = QuestionSets[0];
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Required").Single());
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Email").Single());
            #endregion Arrange

            #region Act
            var result = Controller.Create(1, questionToAdd, new string[0])
                .AssertActionRedirect()
                .ToAction<QuestionSetController>(a => a.Edit(1));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            QuestionRepository.AssertWasCalled(a => a.EnsurePersistent(questionToAdd));
            Assert.AreEqual("Question has been created successfully.", Controller.Message);
            #endregion Assert
        }

        [TestMethod]
        public void TestTextBoxWithRequiredValidatorAndOneMoreSaves2()
        {
            #region Arrange
            SetUpDataForValidatorTests();

            var questionToAdd = CreateValidEntities.Question(null);
            questionToAdd.QuestionType = QuestionTypes.Where(a => a.Name == "Text Box").Single();
            questionToAdd.QuestionSet = QuestionSets[0];
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Required").Single());
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Url").Single());
            #endregion Arrange

            #region Act
            var result = Controller.Create(1, questionToAdd, new string[0])
                .AssertActionRedirect()
                .ToAction<QuestionSetController>(a => a.Edit(1));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            QuestionRepository.AssertWasCalled(a => a.EnsurePersistent(questionToAdd));
            Assert.AreEqual("Question has been created successfully.", Controller.Message);
            #endregion Assert
        }

        [TestMethod]
        public void TestTextBoxWithRequiredValidatorAndOneMoreSaves3()
        {
            #region Arrange
            SetUpDataForValidatorTests();

            var questionToAdd = CreateValidEntities.Question(null);
            questionToAdd.QuestionType = QuestionTypes.Where(a => a.Name == "Text Box").Single();
            questionToAdd.QuestionSet = QuestionSets[0];
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Required").Single());
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Date").Single());
            #endregion Arrange

            #region Act
            var result = Controller.Create(1, questionToAdd, new string[0])
                .AssertActionRedirect()
                .ToAction<QuestionSetController>(a => a.Edit(1));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            QuestionRepository.AssertWasCalled(a => a.EnsurePersistent(questionToAdd));
            Assert.AreEqual("Question has been created successfully.", Controller.Message);
            #endregion Assert
        }

        [TestMethod]
        public void TestTextBoxWithRequiredValidatorAndOneMoreSaves4()
        {
            #region Arrange
            SetUpDataForValidatorTests();

            var questionToAdd = CreateValidEntities.Question(null);
            questionToAdd.QuestionType = QuestionTypes.Where(a => a.Name == "Text Box").Single();
            questionToAdd.QuestionSet = QuestionSets[0];
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Required").Single());
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Phone Number").Single());
            #endregion Arrange

            #region Act
            var result = Controller.Create(1, questionToAdd, new string[0])
                .AssertActionRedirect()
                .ToAction<QuestionSetController>(a => a.Edit(1));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            QuestionRepository.AssertWasCalled(a => a.EnsurePersistent(questionToAdd));
            Assert.AreEqual("Question has been created successfully.", Controller.Message);
            #endregion Assert
        }

        [TestMethod]
        public void TestTextBoxWithEmailOnlyValidatorSaves()
        {
            #region Arrange
            SetUpDataForValidatorTests();

            var questionToAdd = CreateValidEntities.Question(null);
            questionToAdd.QuestionType = QuestionTypes.Where(a => a.Name == "Text Box").Single();
            questionToAdd.QuestionSet = QuestionSets[0];
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Required").Single());
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Email").Single());
            #endregion Arrange

            #region Act
            var result = Controller.Create(1, questionToAdd, new string[0])
                .AssertActionRedirect()
                .ToAction<QuestionSetController>(a => a.Edit(1));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            QuestionRepository.AssertWasCalled(a => a.EnsurePersistent(questionToAdd));
            Assert.AreEqual("Question has been created successfully.", Controller.Message);
            #endregion Assert
        }

        [TestMethod]
        public void TestTextBoxWithUrlOnlyValidatorSaves()
        {
            #region Arrange
            SetUpDataForValidatorTests();

            var questionToAdd = CreateValidEntities.Question(null);
            questionToAdd.QuestionType = QuestionTypes.Where(a => a.Name == "Text Box").Single();
            questionToAdd.QuestionSet = QuestionSets[0];
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Required").Single());
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Url").Single());
            #endregion Arrange

            #region Act
            var result = Controller.Create(1, questionToAdd, new string[0])
                .AssertActionRedirect()
                .ToAction<QuestionSetController>(a => a.Edit(1));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            QuestionRepository.AssertWasCalled(a => a.EnsurePersistent(questionToAdd));
            Assert.AreEqual("Question has been created successfully.", Controller.Message);
            #endregion Assert
        }

        [TestMethod]
        public void TestTextBoxWithDateOnlyValidatorSaves()
        {
            #region Arrange
            SetUpDataForValidatorTests();

            var questionToAdd = CreateValidEntities.Question(null);
            questionToAdd.QuestionType = QuestionTypes.Where(a => a.Name == "Text Box").Single();
            questionToAdd.QuestionSet = QuestionSets[0];
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Required").Single());
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Date").Single());
            #endregion Arrange

            #region Act
            var result = Controller.Create(1, questionToAdd, new string[0])
                .AssertActionRedirect()
                .ToAction<QuestionSetController>(a => a.Edit(1));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            QuestionRepository.AssertWasCalled(a => a.EnsurePersistent(questionToAdd));
            Assert.AreEqual("Question has been created successfully.", Controller.Message);
            #endregion Assert
        }

        [TestMethod]
        public void TestTextBoxWithPhoneNumberOnlyValidatorSaves()
        {
            #region Arrange
            SetUpDataForValidatorTests();

            var questionToAdd = CreateValidEntities.Question(null);
            questionToAdd.QuestionType = QuestionTypes.Where(a => a.Name == "Text Box").Single();
            questionToAdd.QuestionSet = QuestionSets[0];
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Required").Single());
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Phone Number").Single());
            #endregion Arrange

            #region Act
            var result = Controller.Create(1, questionToAdd, new string[0])
                .AssertActionRedirect()
                .ToAction<QuestionSetController>(a => a.Edit(1));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            QuestionRepository.AssertWasCalled(a => a.EnsurePersistent(questionToAdd));
            Assert.AreEqual("Question has been created successfully.", Controller.Message);
            #endregion Assert
        }
        #endregion Valid Validators

        #region Invalid Validators

        [TestMethod]
        public void TestTextBoxWithTwoValidatorsButNotRequiredDoesNotSave1()
        {
            #region Arrange
            SetUpDataForValidatorTests();

            var questionToAdd = CreateValidEntities.Question(null);
            questionToAdd.QuestionType = QuestionTypes.Where(a => a.Name == "Text Box").Single();
            questionToAdd.QuestionSet = QuestionSets[0];
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Required").Single());
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Email").Single());
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Url").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Date").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Phone Number").Single());
            #endregion Arrange

            #region Act
            var result = Controller.Create(1, questionToAdd, new string[0])
                .AssertViewRendered()
                .WithViewData<QuestionViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            QuestionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("Cannot have Email, Url, Date, or Phone Number validators selected together.");
            #endregion Assert
        }

        [TestMethod]
        public void TestTextBoxWithTwoValidatorsButNotRequiredDoesNotSave2()
        {
            #region Arrange
            SetUpDataForValidatorTests();

            var questionToAdd = CreateValidEntities.Question(null);
            questionToAdd.QuestionType = QuestionTypes.Where(a => a.Name == "Text Box").Single();
            questionToAdd.QuestionSet = QuestionSets[0];
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Required").Single());
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Email").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Url").Single());
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Date").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Phone Number").Single());
            #endregion Arrange

            #region Act
            var result = Controller.Create(1, questionToAdd, new string[0])
                .AssertViewRendered()
                .WithViewData<QuestionViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            QuestionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("Cannot have Email, Url, Date, or Phone Number validators selected together.");
            #endregion Assert
        }

        [TestMethod]
        public void TestTextBoxWithTwoValidatorsButNotRequiredDoesNotSave3()
        {
            #region Arrange
            SetUpDataForValidatorTests();

            var questionToAdd = CreateValidEntities.Question(null);
            questionToAdd.QuestionType = QuestionTypes.Where(a => a.Name == "Text Box").Single();
            questionToAdd.QuestionSet = QuestionSets[0];
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Required").Single());
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Email").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Url").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Date").Single());
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Phone Number").Single());
            #endregion Arrange

            #region Act
            var result = Controller.Create(1, questionToAdd, new string[0])
                .AssertViewRendered()
                .WithViewData<QuestionViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            QuestionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("Cannot have Email, Url, Date, or Phone Number validators selected together.");
            #endregion Assert
        }

        [TestMethod]
        public void TestTextBoxWithTwoValidatorsButNotRequiredDoesNotSave4()
        {
            #region Arrange
            SetUpDataForValidatorTests();

            var questionToAdd = CreateValidEntities.Question(null);
            questionToAdd.QuestionType = QuestionTypes.Where(a => a.Name == "Text Box").Single();
            questionToAdd.QuestionSet = QuestionSets[0];
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Required").Single());
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Email").Single());
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Url").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Date").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Phone Number").Single());
            #endregion Arrange

            #region Act
            var result = Controller.Create(1, questionToAdd, new string[0])
                .AssertViewRendered()
                .WithViewData<QuestionViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            QuestionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("Cannot have Email, Url, Date, or Phone Number validators selected together.");
            #endregion Assert
        }

        [TestMethod]
        public void TestTextBoxWithTwoValidatorsButNotRequiredDoesNotSave5()
        {
            #region Arrange
            SetUpDataForValidatorTests();

            var questionToAdd = CreateValidEntities.Question(null);
            questionToAdd.QuestionType = QuestionTypes.Where(a => a.Name == "Text Box").Single();
            questionToAdd.QuestionSet = QuestionSets[0];
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Required").Single());
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Email").Single());
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Url").Single());
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Date").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Phone Number").Single());
            #endregion Arrange

            #region Act
            var result = Controller.Create(1, questionToAdd, new string[0])
                .AssertViewRendered()
                .WithViewData<QuestionViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            QuestionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("Cannot have Email, Url, Date, or Phone Number validators selected together.");
            #endregion Assert
        }

        [TestMethod]
        public void TestTextBoxWithTwoValidatorsButNotRequiredDoesNotSave6()
        {
            #region Arrange
            SetUpDataForValidatorTests();

            var questionToAdd = CreateValidEntities.Question(null);
            questionToAdd.QuestionType = QuestionTypes.Where(a => a.Name == "Text Box").Single();
            questionToAdd.QuestionSet = QuestionSets[0];
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Required").Single());
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Email").Single());
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Url").Single());
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Date").Single());
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Phone Number").Single());
            #endregion Arrange

            #region Act
            var result = Controller.Create(1, questionToAdd, new string[0])
                .AssertViewRendered()
                .WithViewData<QuestionViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            QuestionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("Cannot have Email, Url, Date, or Phone Number validators selected together.");
            #endregion Assert
        }

        [TestMethod]
        public void TestTextBoxWithTwoValidatorsButNotRequiredDoesNotSave7()
        {
            #region Arrange
            SetUpDataForValidatorTests();

            var questionToAdd = CreateValidEntities.Question(null);
            questionToAdd.QuestionType = QuestionTypes.Where(a => a.Name == "Text Box").Single();
            questionToAdd.QuestionSet = QuestionSets[0];
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Required").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Email").Single());
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Url").Single());
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Date").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Phone Number").Single());
            #endregion Arrange

            #region Act
            var result = Controller.Create(1, questionToAdd, new string[0])
                .AssertViewRendered()
                .WithViewData<QuestionViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            QuestionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("Cannot have Email, Url, Date, or Phone Number validators selected together.");
            #endregion Assert
        }

        [TestMethod]
        public void TestTextBoxWithTwoValidatorsButNotRequiredDoesNotSave8()
        {
            #region Arrange
            SetUpDataForValidatorTests();

            var questionToAdd = CreateValidEntities.Question(null);
            questionToAdd.QuestionType = QuestionTypes.Where(a => a.Name == "Text Box").Single();
            questionToAdd.QuestionSet = QuestionSets[0];
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Required").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Email").Single());
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Url").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Date").Single());
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Phone Number").Single());
            #endregion Arrange

            #region Act
            var result = Controller.Create(1, questionToAdd, new string[0])
                .AssertViewRendered()
                .WithViewData<QuestionViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            QuestionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("Cannot have Email, Url, Date, or Phone Number validators selected together.");
            #endregion Assert
        }

        [TestMethod]
        public void TestTextBoxWithTwoValidatorsButNotRequiredDoesNotSave9()
        {
            #region Arrange
            SetUpDataForValidatorTests();

            var questionToAdd = CreateValidEntities.Question(null);
            questionToAdd.QuestionType = QuestionTypes.Where(a => a.Name == "Text Box").Single();
            questionToAdd.QuestionSet = QuestionSets[0];
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Required").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Email").Single());
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Url").Single());
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Date").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Phone Number").Single());
            #endregion Arrange

            #region Act
            var result = Controller.Create(1, questionToAdd, new string[0])
                .AssertViewRendered()
                .WithViewData<QuestionViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            QuestionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("Cannot have Email, Url, Date, or Phone Number validators selected together.");
            #endregion Assert
        }

        [TestMethod]
        public void TestTextBoxWithTwoValidatorsButNotRequiredDoesNotSave10()
        {
            #region Arrange
            SetUpDataForValidatorTests();

            var questionToAdd = CreateValidEntities.Question(null);
            questionToAdd.QuestionType = QuestionTypes.Where(a => a.Name == "Text Box").Single();
            questionToAdd.QuestionSet = QuestionSets[0];
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Required").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Email").Single());
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Url").Single());
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Date").Single());
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Phone Number").Single());
            #endregion Arrange

            #region Act
            var result = Controller.Create(1, questionToAdd, new string[0])
                .AssertViewRendered()
                .WithViewData<QuestionViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            QuestionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("Cannot have Email, Url, Date, or Phone Number validators selected together.");
            #endregion Assert
        }

        [TestMethod]
        public void TestTextBoxWithTwoValidatorsButNotRequiredDoesNotSave11()
        {
            #region Arrange
            SetUpDataForValidatorTests();

            var questionToAdd = CreateValidEntities.Question(null);
            questionToAdd.QuestionType = QuestionTypes.Where(a => a.Name == "Text Box").Single();
            questionToAdd.QuestionSet = QuestionSets[0];
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Required").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Email").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Url").Single());
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Date").Single());
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Phone Number").Single());
            #endregion Arrange

            #region Act
            var result = Controller.Create(1, questionToAdd, new string[0])
                .AssertViewRendered()
                .WithViewData<QuestionViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            QuestionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("Cannot have Email, Url, Date, or Phone Number validators selected together.");
            #endregion Assert
        }

        [TestMethod]
        public void TestTextBoxWithTwoValidatorsButNotRequiredDoesNotSave12()
        {
            #region Arrange
            SetUpDataForValidatorTests();

            var questionToAdd = CreateValidEntities.Question(null);
            questionToAdd.QuestionType = QuestionTypes.Where(a => a.Name == "Text Box").Single();
            questionToAdd.QuestionSet = QuestionSets[0];
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Required").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Email").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Url").Single());
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Date").Single());
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Phone Number").Single());
            #endregion Arrange

            #region Act
            var result = Controller.Create(1, questionToAdd, new string[0])
                .AssertViewRendered()
                .WithViewData<QuestionViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            QuestionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("Cannot have Email, Url, Date, or Phone Number validators selected together.");
            #endregion Assert
        }

        #endregion Invalid Validators
        #endregion Text Box Tests

        #region Boolean Tests

        #region Valid Tests
        [TestMethod]
        public void TestBooleanWithNoValidatorsSaves()
        {
            #region Arrange
            SetUpDataForValidatorTests();

            var questionToAdd = CreateValidEntities.Question(null);
            questionToAdd.QuestionType = QuestionTypes.Where(a => a.Name == "Boolean").Single();
            questionToAdd.QuestionSet = QuestionSets[0];
            #endregion Arrange

            #region Act
            var result = Controller.Create(1, questionToAdd, new string[0])
                .AssertActionRedirect()
                .ToAction<QuestionSetController>(a => a.Edit(1));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            QuestionRepository.AssertWasCalled(a => a.EnsurePersistent(questionToAdd));
            Assert.AreEqual("Question has been created successfully.", Controller.Message);
            #endregion Assert
        }
        #endregion Valid Tests

        #region Invalid Tests
        [TestMethod]
        public void TestBooleanWithAnyValidatorDoesNotSave1()
        {
            #region Arrange
            SetUpDataForValidatorTests();

            var questionToAdd = CreateValidEntities.Question(null);
            questionToAdd.QuestionType = QuestionTypes.Where(a => a.Name == "Boolean").Single();
            questionToAdd.QuestionSet = QuestionSets[0];
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Required").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Email").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Url").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Date").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Phone Number").Single());
            #endregion Arrange

            #region Act
            var result = Controller.Create(1, questionToAdd, new string[0])
                .AssertViewRendered()
                .WithViewData<QuestionViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            QuestionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("Boolean Question Type should not have validators.");
            #endregion Assert
        }
        [TestMethod]
        public void TestBooleanWithAnyValidatorDoesNotSave2()
        {
            #region Arrange
            SetUpDataForValidatorTests();

            var questionToAdd = CreateValidEntities.Question(null);
            questionToAdd.QuestionType = QuestionTypes.Where(a => a.Name == "Boolean").Single();
            questionToAdd.QuestionSet = QuestionSets[0];
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Required").Single());
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Email").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Url").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Date").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Phone Number").Single());
            #endregion Arrange

            #region Act
            var result = Controller.Create(1, questionToAdd, new string[0])
                .AssertViewRendered()
                .WithViewData<QuestionViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            QuestionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("Boolean Question Type should not have validators.");
            #endregion Assert
        }
        [TestMethod]
        public void TestBooleanWithAnyValidatorDoesNotSave3()
        {
            #region Arrange
            SetUpDataForValidatorTests();

            var questionToAdd = CreateValidEntities.Question(null);
            questionToAdd.QuestionType = QuestionTypes.Where(a => a.Name == "Boolean").Single();
            questionToAdd.QuestionSet = QuestionSets[0];
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Required").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Email").Single());
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Url").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Date").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Phone Number").Single());
            #endregion Arrange

            #region Act
            var result = Controller.Create(1, questionToAdd, new string[0])
                .AssertViewRendered()
                .WithViewData<QuestionViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            QuestionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("Boolean Question Type should not have validators.");
            #endregion Assert
        }
        [TestMethod]
        public void TestBooleanWithAnyValidatorDoesNotSave4()
        {
            #region Arrange
            SetUpDataForValidatorTests();

            var questionToAdd = CreateValidEntities.Question(null);
            questionToAdd.QuestionType = QuestionTypes.Where(a => a.Name == "Boolean").Single();
            questionToAdd.QuestionSet = QuestionSets[0];
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Required").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Email").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Url").Single());
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Date").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Phone Number").Single());
            #endregion Arrange

            #region Act
            var result = Controller.Create(1, questionToAdd, new string[0])
                .AssertViewRendered()
                .WithViewData<QuestionViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            QuestionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("Boolean Question Type should not have validators.");
            #endregion Assert
        }
        [TestMethod]
        public void TestBooleanWithAnyValidatorDoesNotSave5()
        {
            #region Arrange
            SetUpDataForValidatorTests();

            var questionToAdd = CreateValidEntities.Question(null);
            questionToAdd.QuestionType = QuestionTypes.Where(a => a.Name == "Boolean").Single();
            questionToAdd.QuestionSet = QuestionSets[0];
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Required").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Email").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Url").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Date").Single());
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Phone Number").Single());
            #endregion Arrange

            #region Act
            var result = Controller.Create(1, questionToAdd, new string[0])
                .AssertViewRendered()
                .WithViewData<QuestionViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            QuestionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("Boolean Question Type should not have validators.");
            #endregion Assert
        }
        [TestMethod]
        public void TestBooleanWithAnyValidatorDoesNotSave6()
        {
            #region Arrange
            SetUpDataForValidatorTests();

            var questionToAdd = CreateValidEntities.Question(null);
            questionToAdd.QuestionType = QuestionTypes.Where(a => a.Name == "Boolean").Single();
            questionToAdd.QuestionSet = QuestionSets[0];
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Required").Single());
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Email").Single());
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Url").Single());
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Date").Single());
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Phone Number").Single());
            #endregion Arrange

            #region Act
            var result = Controller.Create(1, questionToAdd, new string[0])
                .AssertViewRendered()
                .WithViewData<QuestionViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            QuestionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("Boolean Question Type should not have validators.");
            #endregion Assert
        }
        #endregion Invalid Tests

        #endregion Boolean Tests

        #region Text Area Tests

        #region Valid Tests
        [TestMethod]
        public void TestTextAreaWithNoValidatorsSaves()
        {
            #region Arrange
            SetUpDataForValidatorTests();

            var questionToAdd = CreateValidEntities.Question(null);
            questionToAdd.QuestionType = QuestionTypes.Where(a => a.Name == "Text Area").Single();
            questionToAdd.QuestionSet = QuestionSets[0];
            #endregion Arrange

            #region Act
            var result = Controller.Create(1, questionToAdd, new string[0])
                .AssertActionRedirect()
                .ToAction<QuestionSetController>(a => a.Edit(1));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            QuestionRepository.AssertWasCalled(a => a.EnsurePersistent(questionToAdd));
            Assert.AreEqual("Question has been created successfully.", Controller.Message);
            #endregion Assert
        }

        [TestMethod]
        public void TestTextAreaWithRequiredValidatorOnlySaves()
        {
            #region Arrange
            SetUpDataForValidatorTests();

            var questionToAdd = CreateValidEntities.Question(null);
            questionToAdd.QuestionType = QuestionTypes.Where(a => a.Name == "Text Area").Single();
            questionToAdd.QuestionSet = QuestionSets[0];
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Required").Single());
            #endregion Arrange

            #region Act
            var result = Controller.Create(1, questionToAdd, new string[0])
                .AssertActionRedirect()
                .ToAction<QuestionSetController>(a => a.Edit(1));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            QuestionRepository.AssertWasCalled(a => a.EnsurePersistent(questionToAdd));
            Assert.AreEqual("Question has been created successfully.", Controller.Message);
            #endregion Assert
        }
        #endregion Valid Tests

        #region Invalid Tests
        [TestMethod]
        public void TestTextAreaWithAnyValidatorsButNotRequiredDoesNotSave1()
        {
            #region Arrange
            SetUpDataForValidatorTests();

            var questionToAdd = CreateValidEntities.Question(null);
            questionToAdd.QuestionType = QuestionTypes.Where(a => a.Name == "Text Area").Single();
            questionToAdd.QuestionSet = QuestionSets[0];
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Required").Single());
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Email").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Url").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Date").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Phone Number").Single());
            #endregion Arrange

            #region Act
            var result = Controller.Create(1, questionToAdd, new string[0])
                .AssertViewRendered()
                .WithViewData<QuestionViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            QuestionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("The only validator allowed for a Question Type of Text Area is Required.");
            #endregion Assert
        }
        [TestMethod]
        public void TestTextAreaWithAnyValidatorsButNotRequiredDoesNotSave2()
        {
            #region Arrange
            SetUpDataForValidatorTests();

            var questionToAdd = CreateValidEntities.Question(null);
            questionToAdd.QuestionType = QuestionTypes.Where(a => a.Name == "Text Area").Single();
            questionToAdd.QuestionSet = QuestionSets[0];
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Required").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Email").Single());
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Url").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Date").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Phone Number").Single());
            #endregion Arrange

            #region Act
            var result = Controller.Create(1, questionToAdd, new string[0])
                .AssertViewRendered()
                .WithViewData<QuestionViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            QuestionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("The only validator allowed for a Question Type of Text Area is Required.");
            #endregion Assert
        }
        [TestMethod]
        public void TestTextAreaWithAnyValidatorsButNotRequiredDoesNotSave3()
        {
            #region Arrange
            SetUpDataForValidatorTests();

            var questionToAdd = CreateValidEntities.Question(null);
            questionToAdd.QuestionType = QuestionTypes.Where(a => a.Name == "Text Area").Single();
            questionToAdd.QuestionSet = QuestionSets[0];
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Required").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Email").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Url").Single());
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Date").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Phone Number").Single());
            #endregion Arrange

            #region Act
            var result = Controller.Create(1, questionToAdd, new string[0])
                .AssertViewRendered()
                .WithViewData<QuestionViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            QuestionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("The only validator allowed for a Question Type of Text Area is Required.");
            #endregion Assert
        }
        [TestMethod]
        public void TestTextAreaWithAnyValidatorsButNotRequiredDoesNotSave4()
        {
            #region Arrange
            SetUpDataForValidatorTests();

            var questionToAdd = CreateValidEntities.Question(null);
            questionToAdd.QuestionType = QuestionTypes.Where(a => a.Name == "Text Area").Single();
            questionToAdd.QuestionSet = QuestionSets[0];
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Required").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Email").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Url").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Date").Single());
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Phone Number").Single());
            #endregion Arrange

            #region Act
            var result = Controller.Create(1, questionToAdd, new string[0])
                .AssertViewRendered()
                .WithViewData<QuestionViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            QuestionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("The only validator allowed for a Question Type of Text Area is Required.");
            #endregion Assert
        }
        [TestMethod]
        public void TestTextAreaWithAnyValidatorsButNotRequiredDoesNotSave5()
        {
            #region Arrange
            SetUpDataForValidatorTests();

            var questionToAdd = CreateValidEntities.Question(null);
            questionToAdd.QuestionType = QuestionTypes.Where(a => a.Name == "Text Area").Single();
            questionToAdd.QuestionSet = QuestionSets[0];
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Required").Single());
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Email").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Url").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Date").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Phone Number").Single());
            #endregion Arrange

            #region Act
            var result = Controller.Create(1, questionToAdd, new string[0])
                .AssertViewRendered()
                .WithViewData<QuestionViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            QuestionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("The only validator allowed for a Question Type of Text Area is Required.");
            #endregion Assert
        }
        [TestMethod]
        public void TestTextAreaWithAnyValidatorsButNotRequiredDoesNotSave6()
        {
            #region Arrange
            SetUpDataForValidatorTests();

            var questionToAdd = CreateValidEntities.Question(null);
            questionToAdd.QuestionType = QuestionTypes.Where(a => a.Name == "Text Area").Single();
            questionToAdd.QuestionSet = QuestionSets[0];
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Required").Single());
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Email").Single());
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Url").Single());
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Date").Single());
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Phone Number").Single());
            #endregion Arrange

            #region Act
            var result = Controller.Create(1, questionToAdd, new string[0])
                .AssertViewRendered()
                .WithViewData<QuestionViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            QuestionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("The only validator allowed for a Question Type of Text Area is Required.");
            #endregion Assert
        }
        #endregion Invalid Tests

        #endregion Text Area Tests

        #region RadioButton Test

        #region Valid Tests
        [TestMethod]
        public void TestRadioButtonsWithNoValidatorsSaves()
        {
            #region Arrange
            SetUpDataForValidatorTests();

            var questionToAdd = CreateValidEntities.Question(null);
            questionToAdd.QuestionType = QuestionTypes.Where(a => a.Name == "Radio Buttons").Single();
            questionToAdd.QuestionSet = QuestionSets[0];
            #endregion Arrange

            #region Act
            var result = Controller.Create(1, questionToAdd, new []{"Option1", "Option2"})
                .AssertActionRedirect()
                .ToAction<QuestionSetController>(a => a.Edit(1));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            QuestionRepository.AssertWasCalled(a => a.EnsurePersistent(questionToAdd));
            Assert.AreEqual("Question has been created successfully.", Controller.Message);
            #endregion Assert
        }

        [TestMethod]
        public void TestRadioButtonsWithRequiredValidatorOnlySaves()
        {
            #region Arrange
            SetUpDataForValidatorTests();

            var questionToAdd = CreateValidEntities.Question(null);
            questionToAdd.QuestionType = QuestionTypes.Where(a => a.Name == "Radio Buttons").Single();
            questionToAdd.QuestionSet = QuestionSets[0];
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Required").Single());
            #endregion Arrange

            #region Act
            var result = Controller.Create(1, questionToAdd, new [] { "Option1", "Option2" })
                .AssertActionRedirect()
                .ToAction<QuestionSetController>(a => a.Edit(1));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            QuestionRepository.AssertWasCalled(a => a.EnsurePersistent(questionToAdd));
            Assert.AreEqual("Question has been created successfully.", Controller.Message);
            #endregion Assert
        }
        #endregion Valid Tests
        #region Invalid Tests
        [TestMethod]
        public void TestRadioButtonsWithAnyValidatorsButNotRequiredDoesNotSave1()
        {
            #region Arrange
            SetUpDataForValidatorTests();

            var questionToAdd = CreateValidEntities.Question(null);
            questionToAdd.QuestionType = QuestionTypes.Where(a => a.Name == "Radio Buttons").Single();
            questionToAdd.QuestionSet = QuestionSets[0];
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Required").Single());
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Email").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Url").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Date").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Phone Number").Single());
            #endregion Arrange

            #region Act
            var result = Controller.Create(1, questionToAdd, new[] { "Option1", "Option2" })
                .AssertViewRendered()
                .WithViewData<QuestionViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            QuestionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("The only validator allowed for a Question Type of Radio Buttons is Required.");
            #endregion Assert
        }
        [TestMethod]
        public void TestRadioButtonsWithAnyValidatorsButNotRequiredDoesNotSave2()
        {
            #region Arrange
            SetUpDataForValidatorTests();

            var questionToAdd = CreateValidEntities.Question(null);
            questionToAdd.QuestionType = QuestionTypes.Where(a => a.Name == "Radio Buttons").Single();
            questionToAdd.QuestionSet = QuestionSets[0];
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Required").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Email").Single());
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Url").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Date").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Phone Number").Single());
            #endregion Arrange

            #region Act
            var result = Controller.Create(1, questionToAdd, new[] { "Option1", "Option2" })
                .AssertViewRendered()
                .WithViewData<QuestionViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            QuestionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("The only validator allowed for a Question Type of Radio Buttons is Required.");
            #endregion Assert
        }
        [TestMethod]
        public void TestRadioButtonsWithAnyValidatorsButNotRequiredDoesNotSave3()
        {
            #region Arrange
            SetUpDataForValidatorTests();

            var questionToAdd = CreateValidEntities.Question(null);
            questionToAdd.QuestionType = QuestionTypes.Where(a => a.Name == "Radio Buttons").Single();
            questionToAdd.QuestionSet = QuestionSets[0];
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Required").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Email").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Url").Single());
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Date").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Phone Number").Single());
            #endregion Arrange

            #region Act
            var result = Controller.Create(1, questionToAdd, new[] { "Option1", "Option2" })
                .AssertViewRendered()
                .WithViewData<QuestionViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            QuestionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("The only validator allowed for a Question Type of Radio Buttons is Required.");
            #endregion Assert
        }
        [TestMethod]
        public void TestRadioButtonsWithAnyValidatorsButNotRequiredDoesNotSave4()
        {
            #region Arrange
            SetUpDataForValidatorTests();

            var questionToAdd = CreateValidEntities.Question(null);
            questionToAdd.QuestionType = QuestionTypes.Where(a => a.Name == "Radio Buttons").Single();
            questionToAdd.QuestionSet = QuestionSets[0];
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Required").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Email").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Url").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Date").Single());
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Phone Number").Single());
            #endregion Arrange

            #region Act
            var result = Controller.Create(1, questionToAdd, new[] { "Option1", "Option2" })
                .AssertViewRendered()
                .WithViewData<QuestionViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            QuestionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("The only validator allowed for a Question Type of Radio Buttons is Required.");
            #endregion Assert
        }
        [TestMethod]
        public void TestRadioButtonsWithAnyValidatorsButNotRequiredDoesNotSave5()
        {
            #region Arrange
            SetUpDataForValidatorTests();

            var questionToAdd = CreateValidEntities.Question(null);
            questionToAdd.QuestionType = QuestionTypes.Where(a => a.Name == "Radio Buttons").Single();
            questionToAdd.QuestionSet = QuestionSets[0];
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Required").Single());
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Email").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Url").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Date").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Phone Number").Single());
            #endregion Arrange

            #region Act
            var result = Controller.Create(1, questionToAdd, new[] { "Option1", "Option2" })
                .AssertViewRendered()
                .WithViewData<QuestionViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            QuestionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("The only validator allowed for a Question Type of Radio Buttons is Required.");
            #endregion Assert
        }
        [TestMethod]
        public void TestRadioButtonsWithAnyValidatorsButNotRequiredDoesNotSave6()
        {
            #region Arrange
            SetUpDataForValidatorTests();

            var questionToAdd = CreateValidEntities.Question(null);
            questionToAdd.QuestionType = QuestionTypes.Where(a => a.Name == "Radio Buttons").Single();
            questionToAdd.QuestionSet = QuestionSets[0];
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Required").Single());
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Email").Single());
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Url").Single());
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Date").Single());
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Phone Number").Single());
            #endregion Arrange

            #region Act
            var result = Controller.Create(1, questionToAdd, new[] { "Option1", "Option2" })
                .AssertViewRendered()
                .WithViewData<QuestionViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            QuestionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("The only validator allowed for a Question Type of Radio Buttons is Required.");
            #endregion Assert
        }
        #endregion Invalid Tests
        #endregion RadioButton Test

        #region Checkbox List Test

        #region Valid Tests
        [TestMethod]
        public void TestCheckboxListWithNoValidatorsSaves()
        {
            #region Arrange
            SetUpDataForValidatorTests();

            var questionToAdd = CreateValidEntities.Question(null);
            questionToAdd.QuestionType = QuestionTypes.Where(a => a.Name == "Checkbox List").Single();
            questionToAdd.QuestionSet = QuestionSets[0];
            #endregion Arrange

            #region Act
            var result = Controller.Create(1, questionToAdd, new[] { "Option1", "Option2" })
                .AssertActionRedirect()
                .ToAction<QuestionSetController>(a => a.Edit(1));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            QuestionRepository.AssertWasCalled(a => a.EnsurePersistent(questionToAdd));
            Assert.AreEqual("Question has been created successfully.", Controller.Message);
            #endregion Assert
        }

        [TestMethod]
        public void TestCheckboxListWithRequiredValidatorOnlySaves()
        {
            #region Arrange
            SetUpDataForValidatorTests();

            var questionToAdd = CreateValidEntities.Question(null);
            questionToAdd.QuestionType = QuestionTypes.Where(a => a.Name == "Checkbox List").Single();
            questionToAdd.QuestionSet = QuestionSets[0];
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Required").Single());
            #endregion Arrange

            #region Act
            var result = Controller.Create(1, questionToAdd, new[] { "Option1", "Option2" })
                .AssertActionRedirect()
                .ToAction<QuestionSetController>(a => a.Edit(1));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            QuestionRepository.AssertWasCalled(a => a.EnsurePersistent(questionToAdd));
            Assert.AreEqual("Question has been created successfully.", Controller.Message);
            #endregion Assert
        }
        #endregion Valid Tests
        #region Invalid Tests
        [TestMethod]
        public void TestCheckboxListWithAnyValidatorsButNotRequiredDoesNotSave1()
        {
            #region Arrange
            SetUpDataForValidatorTests();

            var questionToAdd = CreateValidEntities.Question(null);
            questionToAdd.QuestionType = QuestionTypes.Where(a => a.Name == "Checkbox List").Single();
            questionToAdd.QuestionSet = QuestionSets[0];
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Required").Single());
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Email").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Url").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Date").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Phone Number").Single());
            #endregion Arrange

            #region Act
            var result = Controller.Create(1, questionToAdd, new[] { "Option1", "Option2" })
                .AssertViewRendered()
                .WithViewData<QuestionViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            QuestionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("The only validator allowed for a Question Type of Checkbox List is Required.");
            #endregion Assert
        }
        [TestMethod]
        public void TestCheckboxListWithAnyValidatorsButNotRequiredDoesNotSave2()
        {
            #region Arrange
            SetUpDataForValidatorTests();

            var questionToAdd = CreateValidEntities.Question(null);
            questionToAdd.QuestionType = QuestionTypes.Where(a => a.Name == "Checkbox List").Single();
            questionToAdd.QuestionSet = QuestionSets[0];
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Required").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Email").Single());
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Url").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Date").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Phone Number").Single());
            #endregion Arrange

            #region Act
            var result = Controller.Create(1, questionToAdd, new[] { "Option1", "Option2" })
                .AssertViewRendered()
                .WithViewData<QuestionViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            QuestionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("The only validator allowed for a Question Type of Checkbox List is Required.");
            #endregion Assert
        }
        [TestMethod]
        public void TestCheckboxListWithAnyValidatorsButNotRequiredDoesNotSave3()
        {
            #region Arrange
            SetUpDataForValidatorTests();

            var questionToAdd = CreateValidEntities.Question(null);
            questionToAdd.QuestionType = QuestionTypes.Where(a => a.Name == "Checkbox List").Single();
            questionToAdd.QuestionSet = QuestionSets[0];
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Required").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Email").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Url").Single());
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Date").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Phone Number").Single());
            #endregion Arrange

            #region Act
            var result = Controller.Create(1, questionToAdd, new[] { "Option1", "Option2" })
                .AssertViewRendered()
                .WithViewData<QuestionViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            QuestionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("The only validator allowed for a Question Type of Checkbox List is Required.");
            #endregion Assert
        }
        [TestMethod]
        public void TestCheckboxListWithAnyValidatorsButNotRequiredDoesNotSave4()
        {
            #region Arrange
            SetUpDataForValidatorTests();

            var questionToAdd = CreateValidEntities.Question(null);
            questionToAdd.QuestionType = QuestionTypes.Where(a => a.Name == "Checkbox List").Single();
            questionToAdd.QuestionSet = QuestionSets[0];
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Required").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Email").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Url").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Date").Single());
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Phone Number").Single());
            #endregion Arrange

            #region Act
            var result = Controller.Create(1, questionToAdd, new[] { "Option1", "Option2" })
                .AssertViewRendered()
                .WithViewData<QuestionViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            QuestionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("The only validator allowed for a Question Type of Checkbox List is Required.");
            #endregion Assert
        }
        [TestMethod]
        public void TestCheckboxListWithAnyValidatorsButNotRequiredDoesNotSave5()
        {
            #region Arrange
            SetUpDataForValidatorTests();

            var questionToAdd = CreateValidEntities.Question(null);
            questionToAdd.QuestionType = QuestionTypes.Where(a => a.Name == "Checkbox List").Single();
            questionToAdd.QuestionSet = QuestionSets[0];
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Required").Single());
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Email").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Url").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Date").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Phone Number").Single());
            #endregion Arrange

            #region Act
            var result = Controller.Create(1, questionToAdd, new[] { "Option1", "Option2" })
                .AssertViewRendered()
                .WithViewData<QuestionViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            QuestionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("The only validator allowed for a Question Type of Checkbox List is Required.");
            #endregion Assert
        }
        [TestMethod]
        public void TestCheckboxListWithAnyValidatorsButNotRequiredDoesNotSave6()
        {
            #region Arrange
            SetUpDataForValidatorTests();

            var questionToAdd = CreateValidEntities.Question(null);
            questionToAdd.QuestionType = QuestionTypes.Where(a => a.Name == "Checkbox List").Single();
            questionToAdd.QuestionSet = QuestionSets[0];
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Required").Single());
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Email").Single());
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Url").Single());
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Date").Single());
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Phone Number").Single());
            #endregion Arrange

            #region Act
            var result = Controller.Create(1, questionToAdd, new[] { "Option1", "Option2" })
                .AssertViewRendered()
                .WithViewData<QuestionViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            QuestionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("The only validator allowed for a Question Type of Checkbox List is Required.");
            #endregion Assert
        }
        #endregion Invalid Tests
        #endregion Checkbox List Test

        #region RadioButton Test

        #region Valid Tests
        [TestMethod]
        public void TestDropDownWithNoValidatorsSaves()
        {
            #region Arrange
            SetUpDataForValidatorTests();

            var questionToAdd = CreateValidEntities.Question(null);
            questionToAdd.QuestionType = QuestionTypes.Where(a => a.Name == "Drop Down").Single();
            questionToAdd.QuestionSet = QuestionSets[0];
            #endregion Arrange

            #region Act
            var result = Controller.Create(1, questionToAdd, new[] { "Option1", "Option2" })
                .AssertActionRedirect()
                .ToAction<QuestionSetController>(a => a.Edit(1));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            QuestionRepository.AssertWasCalled(a => a.EnsurePersistent(questionToAdd));
            Assert.AreEqual("Question has been created successfully.", Controller.Message);
            #endregion Assert
        }

        [TestMethod]
        public void TestDropDownWithRequiredValidatorOnlySaves()
        {
            #region Arrange
            SetUpDataForValidatorTests();

            var questionToAdd = CreateValidEntities.Question(null);
            questionToAdd.QuestionType = QuestionTypes.Where(a => a.Name == "Drop Down").Single();
            questionToAdd.QuestionSet = QuestionSets[0];
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Required").Single());
            #endregion Arrange

            #region Act
            var result = Controller.Create(1, questionToAdd, new[] { "Option1", "Option2" })
                .AssertActionRedirect()
                .ToAction<QuestionSetController>(a => a.Edit(1));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            QuestionRepository.AssertWasCalled(a => a.EnsurePersistent(questionToAdd));
            Assert.AreEqual("Question has been created successfully.", Controller.Message);
            #endregion Assert
        }
        #endregion Valid Tests
        #region Invalid Tests
        [TestMethod]
        public void TestDropDownWithAnyValidatorsButNotRequiredDoesNotSave1()
        {
            #region Arrange
            SetUpDataForValidatorTests();

            var questionToAdd = CreateValidEntities.Question(null);
            questionToAdd.QuestionType = QuestionTypes.Where(a => a.Name == "Drop Down").Single();
            questionToAdd.QuestionSet = QuestionSets[0];
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Required").Single());
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Email").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Url").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Date").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Phone Number").Single());
            #endregion Arrange

            #region Act
            var result = Controller.Create(1, questionToAdd, new[] { "Option1", "Option2" })
                .AssertViewRendered()
                .WithViewData<QuestionViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            QuestionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("The only validator allowed for a Question Type of Drop Down is Required.");
            #endregion Assert
        }
        [TestMethod]
        public void TestDropDownWithAnyValidatorsButNotRequiredDoesNotSave2()
        {
            #region Arrange
            SetUpDataForValidatorTests();

            var questionToAdd = CreateValidEntities.Question(null);
            questionToAdd.QuestionType = QuestionTypes.Where(a => a.Name == "Drop Down").Single();
            questionToAdd.QuestionSet = QuestionSets[0];
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Required").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Email").Single());
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Url").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Date").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Phone Number").Single());
            #endregion Arrange

            #region Act
            var result = Controller.Create(1, questionToAdd, new[] { "Option1", "Option2" })
                .AssertViewRendered()
                .WithViewData<QuestionViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            QuestionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("The only validator allowed for a Question Type of Drop Down is Required.");
            #endregion Assert
        }
        [TestMethod]
        public void TestDropDownWithAnyValidatorsButNotRequiredDoesNotSave3()
        {
            #region Arrange
            SetUpDataForValidatorTests();

            var questionToAdd = CreateValidEntities.Question(null);
            questionToAdd.QuestionType = QuestionTypes.Where(a => a.Name == "Drop Down").Single();
            questionToAdd.QuestionSet = QuestionSets[0];
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Required").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Email").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Url").Single());
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Date").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Phone Number").Single());
            #endregion Arrange

            #region Act
            var result = Controller.Create(1, questionToAdd, new[] { "Option1", "Option2" })
                .AssertViewRendered()
                .WithViewData<QuestionViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            QuestionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("The only validator allowed for a Question Type of Drop Down is Required.");
            #endregion Assert
        }
        [TestMethod]
        public void TestDropDownWithAnyValidatorsButNotRequiredDoesNotSave4()
        {
            #region Arrange
            SetUpDataForValidatorTests();

            var questionToAdd = CreateValidEntities.Question(null);
            questionToAdd.QuestionType = QuestionTypes.Where(a => a.Name == "Drop Down").Single();
            questionToAdd.QuestionSet = QuestionSets[0];
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Required").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Email").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Url").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Date").Single());
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Phone Number").Single());
            #endregion Arrange

            #region Act
            var result = Controller.Create(1, questionToAdd, new[] { "Option1", "Option2" })
                .AssertViewRendered()
                .WithViewData<QuestionViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            QuestionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("The only validator allowed for a Question Type of Drop Down is Required.");
            #endregion Assert
        }
        [TestMethod]
        public void TestDropDownWithAnyValidatorsButNotRequiredDoesNotSave5()
        {
            #region Arrange
            SetUpDataForValidatorTests();

            var questionToAdd = CreateValidEntities.Question(null);
            questionToAdd.QuestionType = QuestionTypes.Where(a => a.Name == "Drop Down").Single();
            questionToAdd.QuestionSet = QuestionSets[0];
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Required").Single());
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Email").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Url").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Date").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Phone Number").Single());
            #endregion Arrange

            #region Act
            var result = Controller.Create(1, questionToAdd, new[] { "Option1", "Option2" })
                .AssertViewRendered()
                .WithViewData<QuestionViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            QuestionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("The only validator allowed for a Question Type of Drop Down is Required.");
            #endregion Assert
        }
        [TestMethod]
        public void TestDropDownWithAnyValidatorsButNotRequiredDoesNotSave6()
        {
            #region Arrange
            SetUpDataForValidatorTests();

            var questionToAdd = CreateValidEntities.Question(null);
            questionToAdd.QuestionType = QuestionTypes.Where(a => a.Name == "Drop Down").Single();
            questionToAdd.QuestionSet = QuestionSets[0];
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Required").Single());
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Email").Single());
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Url").Single());
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Date").Single());
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Phone Number").Single());
            #endregion Arrange

            #region Act
            var result = Controller.Create(1, questionToAdd, new[] { "Option1", "Option2" })
                .AssertViewRendered()
                .WithViewData<QuestionViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            QuestionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("The only validator allowed for a Question Type of Drop Down is Required.");
            #endregion Assert
        }
        #endregion Invalid Tests
        #endregion Drop Down Test

        #region Date Tests
        #region Valid Tests
        [TestMethod]
        public void TestDateWithNoValidatorsSaves()
        {
            #region Arrange
            SetUpDataForValidatorTests();

            var questionToAdd = CreateValidEntities.Question(null);
            questionToAdd.QuestionType = QuestionTypes.Where(a => a.Name == "Date").Single();
            questionToAdd.QuestionSet = QuestionSets[0];
            #endregion Arrange

            #region Act
            var result = Controller.Create(1, questionToAdd, new string[0])
                .AssertActionRedirect()
                .ToAction<QuestionSetController>(a => a.Edit(1));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            QuestionRepository.AssertWasCalled(a => a.EnsurePersistent(questionToAdd));
            Assert.AreEqual("Question has been created successfully.", Controller.Message);
            #endregion Assert
        }

        [TestMethod]
        public void TestDateWithRequiredValidatorOnlySaves()
        {
            #region Arrange
            SetUpDataForValidatorTests();

            var questionToAdd = CreateValidEntities.Question(null);
            questionToAdd.QuestionType = QuestionTypes.Where(a => a.Name == "Date").Single();
            questionToAdd.QuestionSet = QuestionSets[0];
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Required").Single());
            #endregion Arrange

            #region Act
            var result = Controller.Create(1, questionToAdd, new string[0])
                .AssertActionRedirect()
                .ToAction<QuestionSetController>(a => a.Edit(1));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            QuestionRepository.AssertWasCalled(a => a.EnsurePersistent(questionToAdd));
            Assert.AreEqual("Question has been created successfully.", Controller.Message);
            #endregion Assert
        }

        [TestMethod]
        public void TestDateWithDateValidatorOnlySaves()
        {
            #region Arrange
            SetUpDataForValidatorTests();

            var questionToAdd = CreateValidEntities.Question(null);
            questionToAdd.QuestionType = QuestionTypes.Where(a => a.Name == "Date").Single();
            questionToAdd.QuestionSet = QuestionSets[0];
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Date").Single());
            #endregion Arrange

            #region Act
            var result = Controller.Create(1, questionToAdd, new string[0])
                .AssertActionRedirect()
                .ToAction<QuestionSetController>(a => a.Edit(1));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            QuestionRepository.AssertWasCalled(a => a.EnsurePersistent(questionToAdd));
            Assert.AreEqual("Question has been created successfully.", Controller.Message);
            #endregion Assert
        }

        [TestMethod]
        public void TestDateWithDateAndRequiredValidatorsOnlySaves()
        {
            #region Arrange
            SetUpDataForValidatorTests();

            var questionToAdd = CreateValidEntities.Question(null);
            questionToAdd.QuestionType = QuestionTypes.Where(a => a.Name == "Date").Single();
            questionToAdd.QuestionSet = QuestionSets[0];
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Date").Single());
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Required").Single());
            #endregion Arrange

            #region Act
            var result = Controller.Create(1, questionToAdd, new string[0])
                .AssertActionRedirect()
                .ToAction<QuestionSetController>(a => a.Edit(1));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            QuestionRepository.AssertWasCalled(a => a.EnsurePersistent(questionToAdd));
            Assert.AreEqual("Question has been created successfully.", Controller.Message);
            #endregion Assert
        }
        #endregion Valid Tests
        #region Invalid Tests
        [TestMethod]
        public void TestDateWithAnyValidatorsButNotRequiredAndOrDateDoesNotSave1()
        {
            #region Arrange
            SetUpDataForValidatorTests();

            var questionToAdd = CreateValidEntities.Question(null);
            questionToAdd.QuestionType = QuestionTypes.Where(a => a.Name == "Date").Single();
            questionToAdd.QuestionSet = QuestionSets[0];
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Required").Single());
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Email").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Url").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Date").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Phone Number").Single());
            #endregion Arrange

            #region Act
            var result = Controller.Create(1, questionToAdd, new string[0])
                .AssertViewRendered()
                .WithViewData<QuestionViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            QuestionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("Email is not a valid validator for a Question Type of Date");
            #endregion Assert
        }
        [TestMethod]
        public void TestDateWithAnyValidatorsButNotRequiredAndOrDateDoesNotSave2()
        {
            #region Arrange
            SetUpDataForValidatorTests();

            var questionToAdd = CreateValidEntities.Question(null);
            questionToAdd.QuestionType = QuestionTypes.Where(a => a.Name == "Date").Single();
            questionToAdd.QuestionSet = QuestionSets[0];
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Required").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Email").Single());
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Url").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Date").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Phone Number").Single());
            #endregion Arrange

            #region Act
            var result = Controller.Create(1, questionToAdd, new string[0])
                .AssertViewRendered()
                .WithViewData<QuestionViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            QuestionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("Url is not a valid validator for a Question Type of Date");
            #endregion Assert
        }
        [TestMethod]
        public void TestDateWithAnyValidatorsButNotRequiredAndOrDateDoesNotSave3()
        {
            #region Arrange
            SetUpDataForValidatorTests();

            var questionToAdd = CreateValidEntities.Question(null);
            questionToAdd.QuestionType = QuestionTypes.Where(a => a.Name == "Date").Single();
            questionToAdd.QuestionSet = QuestionSets[0];
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Required").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Email").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Url").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Date").Single());
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Phone Number").Single());
            #endregion Arrange

            #region Act
            var result = Controller.Create(1, questionToAdd, new string[0])
                .AssertViewRendered()
                .WithViewData<QuestionViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            QuestionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("Phone Number is not a valid validator for a Question Type of Date");
            #endregion Assert
        }
        [TestMethod]
        public void TestDateWithAnyValidatorsButNotRequiredAndOrDateDoesNotSave4()
        {
            #region Arrange
            SetUpDataForValidatorTests();

            var questionToAdd = CreateValidEntities.Question(null);
            questionToAdd.QuestionType = QuestionTypes.Where(a => a.Name == "Date").Single();
            questionToAdd.QuestionSet = QuestionSets[0];
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Required").Single());
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Email").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Url").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Date").Single());
            //questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Phone Number").Single());
            #endregion Arrange

            #region Act
            var result = Controller.Create(1, questionToAdd, new string[0])
                .AssertViewRendered()
                .WithViewData<QuestionViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            QuestionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("Email is not a valid validator for a Question Type of Date");
            #endregion Assert
        }
        [TestMethod]
        public void TestDateWithAnyValidatorsButNotRequiredAndOrDateDoesNotSave5()
        {
            #region Arrange
            SetUpDataForValidatorTests();

            var questionToAdd = CreateValidEntities.Question(null);
            questionToAdd.QuestionType = QuestionTypes.Where(a => a.Name == "Date").Single();
            questionToAdd.QuestionSet = QuestionSets[0];
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Required").Single());
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Email").Single());
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Url").Single());
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Date").Single());
            questionToAdd.Validators.Add(Validators.Where(a => a.Name == "Phone Number").Single());
            #endregion Arrange

            #region Act
            var result = Controller.Create(1, questionToAdd, new string[0])
                .AssertViewRendered()
                .WithViewData<QuestionViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            QuestionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre(
                "Email is not a valid validator for a Question Type of Date",
                "Url is not a valid validator for a Question Type of Date",
                "Phone Number is not a valid validator for a Question Type of Date");
            #endregion Assert
        }
        #endregion Invalid Tests
        #endregion Date Tests

        #endregion Create Post Validator Tests

        #endregion Create Post Tests

        #region Delete Tests
        /// <summary>
        /// Tests the delete redirects to list if the question is not found.
        /// </summary>
        [TestMethod]
        public void TestDeleteRedirectsToListIfTheQuestionIsNotFound()
        {
            #region Arrange
            QuestionRepository.Expect(a => a.GetNullableById(1)).Return(null).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Delete(1)
                .AssertActionRedirect()
                .ToAction<QuestionSetController>(a => a.List());
            #endregion Act

            #region Assert
            QuestionRepository.AssertWasNotCalled(a => a.Remove(Arg<Question>.Is.Anything));
            #endregion Assert
        }

        #region Delete Redirects To List Because Of HasQuestionSetAccess
        [TestMethod]
        public void TestDeleteWhereQuestionIdIsFoundButNoAccessRedirectsToList1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.User });
            SetupDataForDeleteQuestionTests();
            QuestionSets[0].SystemReusable = true;
            QuestionSets[0].CollegeReusable = false;
            QuestionSets[0].UserReusable = false;
            #endregion Arrange

            #region Act
            Controller.Delete(1)
                .AssertActionRedirect()
                .ToAction<QuestionSetController>(a => a.List());
            #endregion Act

            #region Assert
            Assert.IsNull(Controller.Message);
            QuestionRepository.AssertWasNotCalled(a => a.Remove(Arg<Question>.Is.Anything));
            #endregion Assert
        }

        /// <summary>
        /// College reusable
        /// </summary>
        [TestMethod]
        public void TestDeleteWhereQuestionSetIdIFoundButNoAccessRedirectsToList2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.User });
            SetupDataForDeleteQuestionTests();
            QuestionSets[0].SystemReusable = false;
            QuestionSets[0].CollegeReusable = true;
            QuestionSets[0].UserReusable = false;
            QuestionSetRepository.Expect(a => a.GetNullableById(1)).Return(QuestionSets[0]).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Delete(1)
                .AssertActionRedirect()
                .ToAction<QuestionSetController>(a => a.List());
            #endregion Act

            #region Assert
            Assert.IsNull(Controller.Message);
            QuestionRepository.AssertWasNotCalled(a => a.Remove(Arg<Question>.Is.Anything));
            #endregion Assert
        }

        /// <summary>
        /// College reusable
        /// </summary>
        [TestMethod]
        public void TestDeleteWhereQuestionSetIdIFoundButNoAccessRedirectsToList3()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Admin });
            SetupDataForDeleteQuestionTests();
            QuestionSets[0].SystemReusable = false;
            QuestionSets[0].CollegeReusable = true;
            QuestionSets[0].UserReusable = false;
            QuestionSets[0].School = Schools[2];
            QuestionSetRepository.Expect(a => a.GetNullableById(1)).Return(QuestionSets[0]).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Delete(1)
                .AssertActionRedirect()
                .ToAction<QuestionSetController>(a => a.List());
            #endregion Act

            #region Assert
            Assert.IsNull(Controller.Message);
            QuestionRepository.AssertWasNotCalled(a => a.Remove(Arg<Question>.Is.Anything));
            #endregion Assert
        }

        /// <summary>
        /// College reusable
        /// </summary>
        [TestMethod]
        public void TestDeleteWhereQuestionSetIdIFoundButNoAccessRedirectsToList4()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.SchoolAdmin });
            SetupDataForDeleteQuestionTests();
            QuestionSets[0].SystemReusable = false;
            QuestionSets[0].CollegeReusable = true;
            QuestionSets[0].UserReusable = false;
            QuestionSets[0].School = Schools[2];
            QuestionSetRepository.Expect(a => a.GetNullableById(1)).Return(QuestionSets[0]).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Delete(1)
                .AssertActionRedirect()
                .ToAction<QuestionSetController>(a => a.List());
            #endregion Act

            #region Assert
            Assert.IsNull(Controller.Message);
            QuestionRepository.AssertWasNotCalled(a => a.Remove(Arg<Question>.Is.Anything));
            #endregion Assert
        }

        /// <summary>
        /// User reusable
        /// </summary>
        [TestMethod]
        public void TestDeleteWhereQuestionSetIdIFoundButNoAccessRedirectsToList5()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.User });
            SetupDataForDeleteQuestionTests();
            QuestionSets[0].SystemReusable = false;
            QuestionSets[0].CollegeReusable = false;
            QuestionSets[0].UserReusable = true;
            QuestionSets[0].User = Users[0]; //Not the owner
            QuestionSetRepository.Expect(a => a.GetNullableById(1)).Return(QuestionSets[0]).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Delete(1)
                .AssertActionRedirect()
                .ToAction<QuestionSetController>(a => a.List());
            #endregion Act

            #region Assert
            Assert.IsNull(Controller.Message);
            QuestionRepository.AssertWasNotCalled(a => a.Remove(Arg<Question>.Is.Anything));
            #endregion Assert
        }

        /// <summary>
        /// no reusable, not admin, and has an item type
        /// </summary>
        [TestMethod]
        public void TestDeleteWhereQuestionSetIdIFoundButNoAccessRedirectsToList6()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.User });
            SetupDataForDeleteQuestionTests();
            QuestionSets[0].SystemReusable = false;
            QuestionSets[0].CollegeReusable = false;
            QuestionSets[0].UserReusable = false;
            QuestionSets[0].User = Users[0]; //Not the owner
            QuestionSets[0].ItemTypes.Add(CreateValidEntities.ItemTypeQuestionSet(null));
            QuestionSetRepository.Expect(a => a.GetNullableById(1)).Return(QuestionSets[0]).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Delete(1)
                .AssertActionRedirect()
                .ToAction<QuestionSetController>(a => a.List());
            #endregion Act

            #region Assert
            Assert.IsNull(Controller.Message);
            QuestionRepository.AssertWasNotCalled(a => a.Remove(Arg<Question>.Is.Anything));
            #endregion Assert
        }

        [TestMethod]
        public void TestDeleteWhereQuestionSetIdIFoundButNoAccessRedirectsToList7()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.User });
            SetupDataForDeleteQuestionTests();
            QuestionSets[0].SystemReusable = false;
            QuestionSets[0].CollegeReusable = false;
            QuestionSets[0].UserReusable = false;
            QuestionSets[0].User = Users[0]; //Not the owner
            //QuestionSets[0].ItemTypes.Add(CreateValidEntities.ItemTypeQuestionSet(null));
            QuestionSetRepository.Expect(a => a.GetNullableById(1)).Return(QuestionSets[0]).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Delete(1)
                .AssertActionRedirect()
                .ToAction<QuestionSetController>(a => a.List());
            #endregion Act

            #region Assert
            Assert.IsNull(Controller.Message);
            QuestionRepository.AssertWasNotCalled(a => a.Remove(Arg<Question>.Is.Anything));
            #endregion Assert
        }
        #endregion Delete Redirects To List Because Of HasQuestionSetAccess


        /// <summary>
        /// Tests the delete when question set is used by items does not save.
        /// </summary>
        [TestMethod]
        public void TestDeleteWhenQuestionSetIsUsedByItemsDoesNotSave1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Admin });
            var transactionAnswers = new List<TransactionAnswer>();
            var quantityAnswers = new List<QuantityAnswer>();
            TransactionAnswerRepository.Expect(a => a.Queryable).Return(transactionAnswers.AsQueryable()).Repeat.Any();
            QuantityAnswerRepository.Expect(a => a.Queryable).Return(quantityAnswers.AsQueryable()).Repeat.Any();
            SetupDataForDeleteQuestionTests();
            QuestionSets[0].SystemReusable = false;
            QuestionSets[0].CollegeReusable = false;
            QuestionSets[0].UserReusable = true;
            #endregion Arrange

            #region Act
            Controller.Delete(1)
                .AssertActionRedirect()
                .ToAction<QuestionSetController>(a => a.Edit(1));
            #endregion Act

            #region Assert
            Assert.AreEqual("Question cannot be deleted from the question set because it is already being used by an item.", Controller.Message);
            QuestionRepository.AssertWasNotCalled(a => a.Remove(Arg<Question>.Is.Anything));
            #endregion Assert		
        }

        /// <summary>
        /// Tests the delete when question set is used by items does not save.
        /// </summary>
        [TestMethod]
        public void TestDeleteWhenQuestionSetIsUsedByItemsDoesNotSave2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Admin });
            var transactionAnswers = new List<TransactionAnswer>();
            var quantityAnswers = new List<QuantityAnswer>();
            TransactionAnswerRepository.Expect(a => a.Queryable).Return(transactionAnswers.AsQueryable()).Repeat.Any();
            QuantityAnswerRepository.Expect(a => a.Queryable).Return(quantityAnswers.AsQueryable()).Repeat.Any();
            SetupDataForDeleteQuestionTests();
            QuestionSets[0].SystemReusable = false;
            QuestionSets[0].CollegeReusable = true;
            QuestionSets[0].UserReusable = false;
            #endregion Arrange

            #region Act
            Controller.Delete(1)
                .AssertActionRedirect()
                .ToAction<QuestionSetController>(a => a.Edit(1));
            #endregion Act

            #region Assert
            Assert.AreEqual("Question cannot be deleted from the question set because it is already being used by an item.", Controller.Message);
            QuestionRepository.AssertWasNotCalled(a => a.Remove(Arg<Question>.Is.Anything));
            #endregion Assert
        }

        /// <summary>
        /// Tests the delete when question set is used by items does not save.
        /// </summary>
        [TestMethod]
        public void TestDeleteWhenQuestionSetIsUsedByItemsDoesNotSave3()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Admin });
            var transactionAnswers = new List<TransactionAnswer>();
            var quantityAnswers = new List<QuantityAnswer>();
            TransactionAnswerRepository.Expect(a => a.Queryable).Return(transactionAnswers.AsQueryable()).Repeat.Any();
            QuantityAnswerRepository.Expect(a => a.Queryable).Return(quantityAnswers.AsQueryable()).Repeat.Any();
            SetupDataForDeleteQuestionTests();
            QuestionSets[0].SystemReusable = true;
            QuestionSets[0].CollegeReusable = false;
            QuestionSets[0].UserReusable = false;
            #endregion Arrange

            #region Act
            Controller.Delete(1)
                .AssertActionRedirect()
                .ToAction<QuestionSetController>(a => a.Edit(1));
            #endregion Act

            #region Assert
            Assert.AreEqual("Question cannot be deleted from the question set because it is already being used by an item.", Controller.Message);
            QuestionRepository.AssertWasNotCalled(a => a.Remove(Arg<Question>.Is.Anything));
            #endregion Assert
        }

        /// <summary>
        /// Tests the delete when question set is contact information does not save.
        /// </summary>
        [TestMethod]
        public void TestDeleteWhenQuestionSetIsContactInformationDoesNotSave()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Admin });
            SetupDataForDeleteQuestionTests();
            QuestionSets[0].SystemReusable = true;
            QuestionSets[0].CollegeReusable = false;
            QuestionSets[0].UserReusable = false;
            QuestionSets[0].Name = StaticValues.QuestionSet_ContactInformation;
            QuestionSets[0].Items = new List<ItemQuestionSet>();
            #endregion Arrange

            #region Act
            Controller.Delete(1)
                .AssertActionRedirect()
                .ToAction<QuestionSetController>(a => a.Edit(1));
            #endregion Act

            #region Assert
            Assert.AreEqual("Question cannot be deleted from the question set because it is a system default.", Controller.Message);
            QuestionRepository.AssertWasNotCalled(a => a.Remove(Arg<Question>.Is.Anything));
            #endregion Assert
        }


        /// <summary>
        /// Tests the delete does not save if answer associated with question.
        /// </summary>
        [TestMethod]
        public void TestDeleteDoesNotSaveIfAnswerAssociatedWithQuestion1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Admin });
            SetupDataForDeleteQuestionTests();
            QuestionSets[0].SystemReusable = false;
            QuestionSets[0].CollegeReusable = false;
            QuestionSets[0].UserReusable = true;
            QuestionSets[0].Items = new List<ItemQuestionSet>();

            var transactionAnswers = new List<TransactionAnswer>();
            var quantityAnswers = new List<QuantityAnswer>();
            transactionAnswers.Add(CreateValidEntities.TransactionAnswer(1));
            transactionAnswers[0].Answer = "Answered";
            transactionAnswers[0].Question = Questions[0];
            TransactionAnswerRepository.Expect(a => a.Queryable).Return(transactionAnswers.AsQueryable()).Repeat.Any();
            QuantityAnswerRepository.Expect(a => a.Queryable).Return(quantityAnswers.AsQueryable()).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Delete(1)
                .AssertActionRedirect()
                .ToAction<QuestionSetController>(a => a.Edit(1));
            #endregion Act

            #region Assert
            Assert.AreEqual("Question cannot be deleted from the question set because it has an answer associated with it.", Controller.Message);
            QuestionRepository.AssertWasNotCalled(a => a.Remove(Arg<Question>.Is.Anything));
            #endregion Assert			
        }

        /// <summary>
        /// Tests the delete does not save if answer associated with question.
        /// </summary>
        [TestMethod]
        public void TestDeleteDoesNotSaveIfAnswerAssociatedWithQuestion2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Admin });
            SetupDataForDeleteQuestionTests();
            QuestionSets[0].SystemReusable = false;
            QuestionSets[0].CollegeReusable = false;
            QuestionSets[0].UserReusable = true;
            QuestionSets[0].Items = new List<ItemQuestionSet>();

            var transactionAnswers = new List<TransactionAnswer>();
            var quantityAnswers = new List<QuantityAnswer>();
            quantityAnswers.Add(CreateValidEntities.QuantityAnswer(1));
            quantityAnswers[0].Answer = "Answered";
            quantityAnswers[0].Question = Questions[0];
            TransactionAnswerRepository.Expect(a => a.Queryable).Return(transactionAnswers.AsQueryable()).Repeat.Any();
            QuantityAnswerRepository.Expect(a => a.Queryable).Return(quantityAnswers.AsQueryable()).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Delete(1)
                .AssertActionRedirect()
                .ToAction<QuestionSetController>(a => a.Edit(1));
            #endregion Act

            #region Assert
            Assert.AreEqual("Question cannot be deleted from the question set because it has an answer associated with it.", Controller.Message);
            QuestionRepository.AssertWasNotCalled(a => a.Remove(Arg<Question>.Is.Anything));
            #endregion Assert
        }


        /// <summary>
        /// Tests the delete when only used by item does save.
        /// </summary>
        [TestMethod]
        public void TestDeleteWhenOnlyUsedByItemDoesSave()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Admin });
            var transactionAnswers = new List<TransactionAnswer>();
            var quantityAnswers = new List<QuantityAnswer>();
            TransactionAnswerRepository.Expect(a => a.Queryable).Return(transactionAnswers.AsQueryable()).Repeat.Any();
            QuantityAnswerRepository.Expect(a => a.Queryable).Return(quantityAnswers.AsQueryable()).Repeat.Any();
            SetupDataForDeleteQuestionTests();
            QuestionSets[0].SystemReusable = false;
            QuestionSets[0].CollegeReusable = false;
            QuestionSets[0].UserReusable = false;           
            QuestionSets[0].Items = new List<ItemQuestionSet>();
            QuestionSets[0].Items.Add(CreateValidEntities.ItemQuestionSet(1));
            #endregion Arrange

            #region Act
            Controller.Delete(1)
                .AssertActionRedirect()
                .ToAction<QuestionSetController>(a => a.Edit(1));
            #endregion Act

            #region Assert
            Assert.AreEqual("Question has been removed successfully.", Controller.Message);
            QuestionRepository.AssertWasCalled(a => a.Remove(Questions[0]));
            #endregion Assert		
        }


        /// <summary>
        /// Tests the delete when valid saves.
        /// </summary>
        [TestMethod]
        public void TestDeleteWhenValidSaves()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Admin });
            var transactionAnswers = new List<TransactionAnswer>();
            var quantityAnswers = new List<QuantityAnswer>();
            TransactionAnswerRepository.Expect(a => a.Queryable).Return(transactionAnswers.AsQueryable()).Repeat.Any();
            QuantityAnswerRepository.Expect(a => a.Queryable).Return(quantityAnswers.AsQueryable()).Repeat.Any();
            SetupDataForDeleteQuestionTests();
            QuestionSets[0].SystemReusable = false;
            QuestionSets[0].CollegeReusable = false;
            QuestionSets[0].UserReusable = true;
            QuestionSets[0].Items = new List<ItemQuestionSet>();
            #endregion Arrange

            #region Act
            Controller.Delete(1)
                .AssertActionRedirect()
                .ToAction<QuestionSetController>(a => a.Edit(1));
            #endregion Act

            #region Assert
            Assert.AreEqual("Question has been removed successfully.", Controller.Message);
            QuestionRepository.AssertWasCalled(a => a.Remove(Questions[0]));
            #endregion Assert		
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
            Assert.AreEqual("ApplicationController", result);
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
            var expectedAttribute = controllerMethod.ElementAt(1).GetCustomAttributes(true).OfType<HttpPostAttribute>();
            var allAttributes = controllerMethod.ElementAt(1).GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "HttpPostAttribute not found");
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

        private void SetUpDataForValidatorTests()
        {
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.User, RoleNames.Admin });
            ControllerRecordFakes.FakeUsers(Users, 3);
            ControllerRecordFakes.FakeValidators(Validators);
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes);
            ControllerRecordFakes.FakeQuestionSets(QuestionSets, 1);

            Users[1].LoginID = "UserName";

            QuestionSets[0].SystemReusable = true;
            QuestionSets[0].CollegeReusable = false;
            QuestionSets[0].UserReusable = false;
            
            QuestionSetRepository.Expect(a => a.GetNullableById(1)).Return(QuestionSets[0]).Repeat.Any();
            UserRepository.Expect(a => a.Queryable).Return(Users.AsQueryable()).Repeat.Any();
        }

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

        private void SetupDataForDeleteQuestionTests()
        {
            ControllerRecordFakes.FakeItemQuestionSets(ItemQuestionSets, 1);
            ControllerRecordFakes.FakeUnits(Units, 2);
            ControllerRecordFakes.FakeSchools(Schools, 3);
            ControllerRecordFakes.FakeEditors(Editors, 2);
            ControllerRecordFakes.FakeItems(Items, 1);
            ControllerRecordFakes.FakeUsers(Users, 3);
            ControllerRecordFakes.FakeQuestionSets(QuestionSets, 1);
            ControllerRecordFakes.FakeQuestions(Questions, 3);
            foreach (var question in Questions)
            {
                QuestionSets[0].AddQuestion(question);
            }
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

            QuestionRepository.Expect(a => a.GetNullableById(1)).Return(Questions[0]).Repeat.Any();
            QuestionRepository.Expect(a => a.GetNullableById(2)).Return(Questions[1]).Repeat.Any();
            QuestionRepository.Expect(a => a.GetNullableById(3)).Return(Questions[2]).Repeat.Any();
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
                get { return _identity ?? (_identity = new MockIdentity()); }
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

        #endregion Helper Methods
    }
}
