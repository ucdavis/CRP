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
    /// <c>QuestionSet</c> Controller Tests
    /// </summary>
    [TestClass]
    public class QuestionSetControllerTests : ControllerTestBase<QuestionSetController>
    {
        private readonly Type _controllerClass = typeof(QuestionSetController);
        protected readonly IPrincipal Principal = new MockPrincipal(new[] { RoleNames.User });
        protected IRepositoryWithTypedId<School, string> SchoolRepository { get; set; }        
        protected List<School> Schools { get; set; }

        protected List<User> Users { get; set; }
        protected IRepository<User> UserRepository { get; set; }
        protected List<Unit> Units { get; set; }
        protected IRepository<Unit> UnitRepository { get; set; }
        protected List<QuestionSet> QuestionSets { get; set; }
        protected IRepository<QuestionSet> QuestionSetRepository { get; set; }
        protected List<QuestionType> QuestionTypes { get; set; }
        protected IRepository<QuestionType> QuestionTypeRepository { get; set; }

        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="QuestionSetControllerTests"/> class.
        /// </summary>
        public QuestionSetControllerTests()
        {
            QuestionTypes = new List<QuestionType>();
            QuestionTypeRepository = FakeRepository<QuestionType>();
            Controller.Repository.Expect(a => a.OfType<QuestionType>()).Return(QuestionTypeRepository).Repeat.Any();
        
            QuestionSets = new List<QuestionSet>();
            QuestionSetRepository = FakeRepository<QuestionSet>();
            Controller.Repository.Expect(a => a.OfType<QuestionSet>()).Return(QuestionSetRepository).Repeat.Any();
      
            Units = new List<Unit>();
            UnitRepository = FakeRepository<Unit>();
            Controller.Repository.Expect(a => a.OfType<Unit>()).Return(UnitRepository).Repeat.Any();

            Users = new List<User>();
            UserRepository = FakeRepository<User>();
            Controller.Repository.Expect(a => a.OfType<User>()).Return(UserRepository).Repeat.Any();

            Schools = new List<School>();
        }
        //Controller.ControllerContext.HttpContext = new MockHttpContext(1, new [] {RoleNames.Admin});
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
            SchoolRepository = MockRepository.GenerateStub<IRepositoryWithTypedId<School, string>>();
            Controller = new TestControllerBuilder().CreateController<QuestionSetController>(SchoolRepository);
        }

        #endregion Init

        #region Route Tests

        /// <summary>
        /// Tests the index mapping.
        /// </summary>
        [TestMethod]
        public void TestIndexMapping()
        {
            "~/QuestionSet/Index".ShouldMapTo<QuestionSetController>(a => a.Index());
        }

        /// <summary>
        /// Tests the list mapping.
        /// </summary>
        [TestMethod]
        public void TestListMapping()
        {
            "~/QuestionSet/List".ShouldMapTo<QuestionSetController>(a => a.List());
        }

        /// <summary>
        /// Tests the details mapping.
        /// </summary>
        [TestMethod]
        public void TestDetailsMapping()
        {
            "~/QuestionSet/Details/5".ShouldMapTo<QuestionSetController>(a => a.Details(5));
        }

        /// <summary>
        /// Tests the edit mapping.
        /// </summary>
        [TestMethod]
        public void TestEditMapping()
        {
            "~/QuestionSet/Edit/5".ShouldMapTo<QuestionSetController>(a => a.Edit(5));
        }

        /// <summary>
        /// Tests the edit with parameters mapping.
        /// </summary>
        [TestMethod]
        public void TestEditWithParametersMapping()
        {
            "~/QuestionSet/Edit/5".ShouldMapTo<QuestionSetController>(a => a.Edit(5,new QuestionSet()), true);
        }

        /// <summary>
        /// Tests the create with parameters mapping 1.
        /// </summary>
        [TestMethod]
        public void TestCreateWithParametersMapping1()
        {
            "~/QuestionSet/Create".ShouldMapTo<QuestionSetController>(a => a.Create(null, null, null, null), true);
        }

        /// <summary>
        /// Tests the create with parameters mapping 2.
        /// </summary>
        [TestMethod]
        public void TestCreateWithParametersMapping2()
        {
            "~/QuestionSet/Create".ShouldMapTo<QuestionSetController>(a => a.Create(null, null,new QuestionSet(), string.Empty, true,false ), true);
        }

        /// <summary>
        /// Tests the link to item type with parameters mapping 1.
        /// </summary>
        [TestMethod]
        public void TestLinkToItemTypeWithParametersMapping1()
        {
            "~/QuestionSet/LinkToItemType/5".ShouldMapTo<QuestionSetController>(a => a.LinkToItemType(5, true, false), true);
        }

        /// <summary>
        /// Tests the link to item type with parameters mapping 2.
        /// </summary>
        [TestMethod]
        public void TestLinkToItemTypeWithParametersMapping2()
        {
            "~/QuestionSet/LinkToItemType/5".ShouldMapTo<QuestionSetController>(a => a.LinkToItemType(5, 1, true, false), true);
        }

        /// <summary>
        /// Tests the link to item with parameters mapping 1.
        /// </summary>
        [TestMethod]
        public void TestLinkToItemWithParametersMapping1()
        {
            "~/QuestionSet/LinkToItem/5".ShouldMapTo<QuestionSetController>(a => a.LinkToItem(5, true, false), true);
        }

        /// <summary>
        /// Tests the link to item with parameters mapping 2.
        /// </summary>
        [TestMethod]
        public void TestLinkToItemWithParametersMapping2()
        {
            "~/QuestionSet/LinkToItem/5".ShouldMapTo<QuestionSetController>(a => a.LinkToItem(5, 1, true, false), true);
        }

        /// <summary>
        /// Tests the unlink from item mapping.
        /// </summary>
        [TestMethod]
        public void TestUnlinkFromItemMapping()
        {
            "~/QuestionSet/UnlinkFromItem/5".ShouldMapTo<QuestionSetController>(a => a.UnlinkFromItem(5));
        }
        #endregion Route Tests

        #region Misc Methods

        /// <summary>
        /// Tests the index redirects to list.
        /// </summary>
        [TestMethod]
        public void TestIndexRedirectsToList()
        {
            Controller.Index()
                .AssertActionRedirect()
                .ToAction<QuestionSetController>(a => a.List());
        }


        /// <summary>
        /// Tests the list when user is only user only shows question sets linked to that user.
        /// </summary>
        [TestMethod]
        public void TestListWhenUserIsOnlyUserOnlyShowsQuestionSetsLinkedToThatUser()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.User });
            SetupDataForTests();
            #endregion Arrange

            #region Act
            var result = Controller.List()
                .AssertViewRendered()
                .WithViewData<IQueryable<QuestionSet>>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
            Assert.IsTrue(result.Contains(QuestionSets[0]));
            Assert.IsTrue(result.Contains(QuestionSets[2]));
            #endregion Assert		
        }

        /// <summary>
        /// Tests the list when user is school admin shows question sets linked to that user and school.
        /// </summary>
        [TestMethod]
        public void TestListWhenUserIsSchoolAdminShowsQuestionSetsLinkedToThatUserAndSchool()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.SchoolAdmin });
            SetupDataForTests();
            #endregion Arrange

            #region Act
            var result = Controller.List()
                .AssertViewRendered()
                .WithViewData<IQueryable<QuestionSet>>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(4, result.Count());
            Assert.IsTrue(result.Contains(QuestionSets[0]));
            Assert.IsTrue(result.Contains(QuestionSets[2]));
            Assert.IsTrue(result.Contains(QuestionSets[3]));
            Assert.IsTrue(result.Contains(QuestionSets[5]));
            #endregion Assert
        }

        /// <summary>
        /// Tests the list when user is admin shows question sets linked to that user and school and system reuasable.
        /// </summary>
        [TestMethod]
        public void TestListWhenUserIsAdminShowsQuestionSetsLinkedToThatUserAndSchoolAndSystemReuasable()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Admin });
            SetupDataForTests();
            #endregion Arrange

            #region Act
            var result = Controller.List()
                .AssertViewRendered()
                .WithViewData<IQueryable<QuestionSet>>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(5, result.Count());
            Assert.IsTrue(result.Contains(QuestionSets[0]));
            Assert.IsTrue(result.Contains(QuestionSets[2]));
            Assert.IsTrue(result.Contains(QuestionSets[3]));
            Assert.IsTrue(result.Contains(QuestionSets[5]));
            Assert.IsTrue(result.Contains(QuestionSets[6]));
            #endregion Assert
        }
        #endregion Misc Methods

        #region Details Tests

        /// <summary>
        /// Tests the details redirects to list when id not found.
        /// </summary>
        [TestMethod]
        public void TestDetailsRedirectsToListWhenIdNotFound()
        {
            #region Arrange
            QuestionSetRepository.Expect(a => a.GetNullableByID(1)).Return(null).Repeat.Any();
            #endregion Arrange

            #region Act/Assert
            Controller.Details(1)
                .AssertActionRedirect()
                .ToAction<QuestionSetController>(a => a.List());
            #endregion Act/Assert
        }               
               
        /// <summary>
        /// Tests the details returns view with question set when id is found.
        /// </summary>
        [TestMethod]
        public void TestDetailsReturnsViewWithQuestionSetWhenIdIsFound()
        {
            #region Arrange
            ControllerRecordFakes.FakeQuestionSets(QuestionSets, 3);
            QuestionSetRepository.Expect(a => a.GetNullableByID(2)).Return(QuestionSets[1]).Repeat.Any();
            #endregion Arrange

            #region Act
            var result = Controller.Details(2)
                .AssertViewRendered()
                .WithViewData<QuestionSet>();
            #region Assert

            #endregion Act
            Assert.IsNotNull(result);
            Assert.AreSame(QuestionSets[1], result);
            #endregion Assert		
        }
        #endregion Details Tests

        #region Edit Get Tests

        /// <summary>
        /// Tests the edit get when question setid not found redirects to list.
        /// </summary>
        [TestMethod]
        public void TestEditGetWhenQuestionSetidNotFoundRedirectsToList()
        {
            #region Arrange
            ControllerRecordFakes.FakeQuestionSets(QuestionSets, 3);
            QuestionSetRepository.Expect(a => a.GetNullableByID(2)).Return(null).Repeat.Any();            
            #endregion Arrange

            #region Act
            Controller.Edit(2)
                .AssertActionRedirect()
                .ToAction<QuestionSetController>(a => a.List());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to the requested Question Set.", Controller.Message);
            #endregion Assert		
        }

        #region Edit Get Redirects To List Because Of HasQuestionSetAccess

        /// <summary>
        /// Tests the edit get where question set id Is found but no access redirects to list.
        /// </summary>
        [TestMethod]
        public void TestEditGetWhereQuestionSetIdIsFoundButNoAccessRedirectsToList1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.User });
            SetupDataForTests();
            QuestionSetRepository.Expect(a => a.GetNullableByID(7)).Return(QuestionSets[6]).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Edit(7)
                .AssertActionRedirect()
                .ToAction<QuestionSetController>(a => a.List());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to the requested Question Set.", Controller.Message);           
            #endregion Assert
        }

        /// <summary>
        /// College reusable
        /// </summary>
        [TestMethod]
        public void TestEditGetWhereQuestionSetIdIsFoundButNoAccessRedirectsToList2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.User });
            SetupDataForTests();
            QuestionSetRepository.Expect(a => a.GetNullableByID(4)).Return(QuestionSets[3]).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Edit(4)
                .AssertActionRedirect()
                .ToAction<QuestionSetController>(a => a.List());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to the requested Question Set.", Controller.Message); 
            #endregion Assert
        }

        /// <summary>
        /// College reusable
        /// QuestionSet 4 is a school, but not one the user has access to
        /// </summary>
        [TestMethod]
        public void TestEditGetWhereQuestionSetIdIsFoundButNoAccessRedirectsToList3()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Admin });
            SetupDataForTests();

            QuestionSetRepository.Expect(a => a.GetNullableByID(5)).Return(QuestionSets[4]).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Edit(5)
                .AssertActionRedirect()
                .ToAction<QuestionSetController>(a => a.List());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to the requested Question Set.", Controller.Message);
            #endregion Assert
        }

        /// <summary>
        /// College reusable
        /// QuestionSet 4 is a school, but not one the user has access to
        /// </summary>
        [TestMethod]
        public void TestEditGetWhereQuestionSetIdIsFoundButNoAccessRedirectsToList4()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.SchoolAdmin });
            SetupDataForTests();
            QuestionSetRepository.Expect(a => a.GetNullableByID(5)).Return(QuestionSets[4]).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Edit(5)
                .AssertActionRedirect()
                .ToAction<QuestionSetController>(a => a.List());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to the requested Question Set.", Controller.Message);
            #endregion Assert
        }

        /// <summary>
        /// User reusable, but QuestionSet 1 is attached to a different user
        /// </summary>
        [TestMethod]
        public void TestEditGetWhereQuestionSetIdIsFoundButNoAccessRedirectsToList5()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.User });
            SetupDataForTests();
            QuestionSetRepository.Expect(a => a.GetNullableByID(2)).Return(QuestionSets[1]).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Edit(2)
                .AssertActionRedirect()
                .ToAction<QuestionSetController>(a => a.List());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to the requested Question Set.", Controller.Message);
            #endregion Assert
        }

        /// <summary>
        /// no reusable, not admin, and has an item type
        /// </summary>
        [TestMethod]
        public void TestEditGetWhereQuestionSetIdIsFoundButNoAccessRedirectsToList6()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.User });
            SetupDataForTests();
            QuestionSetRepository.Expect(a => a.GetNullableByID(8)).Return(QuestionSets[7]).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Edit(8)
                .AssertActionRedirect()
                .ToAction<QuestionSetController>(a => a.List());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to the requested Question Set.", Controller.Message);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditGetWhereQuestionSetIdIsFoundButNoAccessRedirectsToList7()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.User });
            SetupDataForTests();

            QuestionSetRepository.Expect(a => a.GetNullableByID(9)).Return(QuestionSets[8]).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Edit(9)
                .AssertActionRedirect()
                .ToAction<QuestionSetController>(a => a.List());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to the requested Question Set.", Controller.Message);
            #endregion Assert
        }
        #endregion Edit Get Redirects To List Because Of HasQuestionSetAccess

        /// <summary>
        /// Tests the edit get where question set id is found and has item access because user is an editor returns view.
        /// </summary>
        [TestMethod]
        public void TestEditGetWhereQuestionSetIdIsFoundAndHasItemAccessBecauseUserIsAnEditorReturnsView()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.User });
            SetupDataForTests();

            QuestionSetRepository.Expect(a => a.GetNullableByID(10)).Return(QuestionSets[9]).Repeat.Any();
            #endregion Arrange

            #region Act
            var result = Controller.Edit(10)
                .AssertViewRendered()
                .WithViewData<QuestionSetViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNull(Controller.Message);
            Assert.IsNotNull(result);
            Assert.AreSame(QuestionSets[9], result.QuestionSet);
            #endregion Assert
        }

        #endregion Edit Get Tests

        #region Edit Post Tests

        /// <summary>
        /// Tests the edit post when question setid not found redirects to list.
        /// </summary>
        [TestMethod]
        public void TestEditPostWhenQuestionSetidNotFoundRedirectsToList()
        {
            #region Arrange
            ControllerRecordFakes.FakeQuestionSets(QuestionSets, 3);
            var questionSetToUpdate = CreateValidEntities.QuestionSet(null);
            QuestionSetRepository.Expect(a => a.GetNullableByID(2)).Return(null).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Edit(2, questionSetToUpdate)
                .AssertActionRedirect()
                .ToAction<QuestionSetController>(a => a.List());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to the requested Question Set.", Controller.Message);
            #endregion Assert
        }

        #region Edit Post Redirects To List Because Of HasQuestionSetAccess

        /// <summary>
        /// Tests the edit post where question set id Is found but no access redirects to list.
        /// </summary>
        [TestMethod]
        public void TestEditPostWhereQuestionSetIdIsFoundButNoAccessRedirectsToList1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.User });
            SetupDataForTests();
            var questionSetToUpdate = CreateValidEntities.QuestionSet(null);
            QuestionSetRepository.Expect(a => a.GetNullableByID(7)).Return(QuestionSets[6]).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Edit(7, questionSetToUpdate)
                .AssertActionRedirect()
                .ToAction<QuestionSetController>(a => a.List());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to the requested Question Set.", Controller.Message);
            #endregion Assert
        }

        /// <summary>
        /// College reusable
        /// </summary>
        [TestMethod]
        public void TestEditPostWhereQuestionSetIdIsFoundButNoAccessRedirectsToList2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.User });
            SetupDataForTests();
            var questionSetToUpdate = CreateValidEntities.QuestionSet(null);
            QuestionSetRepository.Expect(a => a.GetNullableByID(4)).Return(QuestionSets[3]).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Edit(4, questionSetToUpdate)
                .AssertActionRedirect()
                .ToAction<QuestionSetController>(a => a.List());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to the requested Question Set.", Controller.Message);
            #endregion Assert
        }

        /// <summary>
        /// College reusable
        /// QuestionSet 4 is a school, but not one the user has access to
        /// </summary>
        [TestMethod]
        public void TestEditPostWhereQuestionSetIdIsFoundButNoAccessRedirectsToList3()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Admin });
            SetupDataForTests();
            var questionSetToUpdate = CreateValidEntities.QuestionSet(null);
            QuestionSetRepository.Expect(a => a.GetNullableByID(5)).Return(QuestionSets[4]).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Edit(5, questionSetToUpdate)
                .AssertActionRedirect()
                .ToAction<QuestionSetController>(a => a.List());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to the requested Question Set.", Controller.Message);
            #endregion Assert
        }

        /// <summary>
        /// College reusable
        /// QuestionSet 4 is a school, but not one the user has access to
        /// </summary>
        [TestMethod]
        public void TestEditPostWhereQuestionSetIdIsFoundButNoAccessRedirectsToList4()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.SchoolAdmin });
            SetupDataForTests();
            var questionSetToUpdate = CreateValidEntities.QuestionSet(null);
            QuestionSetRepository.Expect(a => a.GetNullableByID(5)).Return(QuestionSets[4]).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Edit(5, questionSetToUpdate)
                .AssertActionRedirect()
                .ToAction<QuestionSetController>(a => a.List());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to the requested Question Set.", Controller.Message);
            #endregion Assert
        }

        /// <summary>
        /// User reusable, but QuestionSet 1 is attached to a different user
        /// </summary>
        [TestMethod]
        public void TestEditPostWhereQuestionSetIdIsFoundButNoAccessRedirectsToList5()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.User });
            SetupDataForTests();
            var questionSetToUpdate = CreateValidEntities.QuestionSet(null);
            QuestionSetRepository.Expect(a => a.GetNullableByID(2)).Return(QuestionSets[1]).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Edit(2, questionSetToUpdate)
                .AssertActionRedirect()
                .ToAction<QuestionSetController>(a => a.List());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to the requested Question Set.", Controller.Message);
            #endregion Assert
        }

        /// <summary>
        /// no reusable, not admin, and has an item type
        /// </summary>
        [TestMethod]
        public void TestEditPostWhereQuestionSetIdIsFoundButNoAccessRedirectsToList6()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.User });
            SetupDataForTests();
            var questionSetToUpdate = CreateValidEntities.QuestionSet(null);
            QuestionSetRepository.Expect(a => a.GetNullableByID(8)).Return(QuestionSets[7]).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Edit(8, questionSetToUpdate)
                .AssertActionRedirect()
                .ToAction<QuestionSetController>(a => a.List());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to the requested Question Set.", Controller.Message);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditPostWhereQuestionSetIdIsFoundButNoAccessRedirectsToList7()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.User });
            SetupDataForTests();
            var questionSetToUpdate = CreateValidEntities.QuestionSet(null);
            QuestionSetRepository.Expect(a => a.GetNullableByID(9)).Return(QuestionSets[8]).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Edit(9, questionSetToUpdate)
                .AssertActionRedirect()
                .ToAction<QuestionSetController>(a => a.List());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to the requested Question Set.", Controller.Message);
            #endregion Assert
        }
        #endregion Edit Post Redirects To List Because Of HasQuestionSetAccess


        /// <summary>
        /// Tests the edit post only updates name and active values.
        /// </summary>
        [TestMethod]
        public void TestEditPostOnlyUpdatesNameAndActiveValues()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.User });
            SetupDataForTests();
            var questionSetToUpdate = CreateValidEntities.QuestionSet(null);
            questionSetToUpdate.Name = "nameUpdate";
            questionSetToUpdate.IsActive = true;
            questionSetToUpdate.Items = null;
            questionSetToUpdate.ItemTypes = null;
            questionSetToUpdate.Questions = null;
            questionSetToUpdate.School = Schools[0];
            questionSetToUpdate.SystemReusable = true;
            questionSetToUpdate.User = null;
            questionSetToUpdate.UserReusable = true;
            questionSetToUpdate.CollegeReusable = true;
            QuestionSetRepository.Expect(a => a.GetNullableByID(10)).Return(QuestionSets[9]).Repeat.Any();

            //Verify that fields are different
            Assert.AreNotEqual(QuestionSets[9].Name, questionSetToUpdate.Name);
            Assert.AreNotEqual(QuestionSets[9].IsActive, questionSetToUpdate.IsActive);
            Assert.AreNotEqual(QuestionSets[9].Items, questionSetToUpdate.Items);
            Assert.AreNotEqual(QuestionSets[9].ItemTypes, questionSetToUpdate.ItemTypes);
            Assert.AreNotEqual(QuestionSets[9].Questions, questionSetToUpdate.Questions);
            Assert.AreNotEqual(QuestionSets[9].School, questionSetToUpdate.School);
            Assert.AreNotEqual(QuestionSets[9].SystemReusable, questionSetToUpdate.SystemReusable);
            Assert.AreNotEqual(QuestionSets[9].User, questionSetToUpdate.User);
            Assert.AreNotEqual(QuestionSets[9].UserReusable, questionSetToUpdate.UserReusable);
            Assert.AreNotEqual(QuestionSets[9].CollegeReusable, questionSetToUpdate.CollegeReusable);

            #endregion Arrange

            #region Act
            var result = Controller.Edit(10, questionSetToUpdate)
                .AssertViewRendered()
                .WithViewData<QuestionSetViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            QuestionSetRepository.AssertWasCalled(a => a.EnsurePersistent(QuestionSets[9]));
            Assert.AreEqual("Question Set has been saved successfully.", Controller.Message);
            //Check that only expected fields are updated
            Assert.AreEqual(QuestionSets[9].Name, questionSetToUpdate.Name);
            Assert.AreEqual(QuestionSets[9].IsActive, questionSetToUpdate.IsActive);
            Assert.AreNotEqual(QuestionSets[9].Items, questionSetToUpdate.Items);
            Assert.AreNotEqual(QuestionSets[9].ItemTypes, questionSetToUpdate.ItemTypes);
            Assert.AreNotEqual(QuestionSets[9].Questions, questionSetToUpdate.Questions);
            Assert.AreNotEqual(QuestionSets[9].School, questionSetToUpdate.School);
            Assert.AreNotEqual(QuestionSets[9].SystemReusable, questionSetToUpdate.SystemReusable);
            Assert.AreNotEqual(QuestionSets[9].User, questionSetToUpdate.User);
            Assert.AreNotEqual(QuestionSets[9].UserReusable, questionSetToUpdate.UserReusable);
            Assert.AreNotEqual(QuestionSets[9].CollegeReusable, questionSetToUpdate.CollegeReusable);
            Assert.AreSame(result.QuestionSet, QuestionSets[9]);
            #endregion Assert		
        }


        /// <summary>
        /// Tests the empty name of the edit post does not save with an.
        /// </summary>
        [TestMethod]
        public void TestEditPostDoesNotSaveWithAnEmptyName()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.User });
            SetupDataForTests();
            var questionSetToUpdate = CreateValidEntities.QuestionSet(null);
            questionSetToUpdate.Name = string.Empty;
            QuestionSetRepository.Expect(a => a.GetNullableByID(10)).Return(QuestionSets[9]).Repeat.Any();
            #endregion Arrange

            #region Act
            var result = Controller.Edit(10, questionSetToUpdate)
                .AssertViewRendered()
                .WithViewData<QuestionSetViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            QuestionSetRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<QuestionSet>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("Name: may not be null or empty");
            #endregion Assert		
        }

        /// <summary>
        /// Tests the edit post will not modify contact information.
        /// </summary>
        [TestMethod]
        public void TestEditPostWillNotModifyContactInformation()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Admin });
            SetupDataForTests();
            //Make #9 the Contact Info record.
            QuestionSets[9].Name = StaticValues.QuestionSet_ContactInformation;
            QuestionSets[9].IsActive = true;
            QuestionSets[9].SystemReusable = true;
            var questionSetToUpdate = CreateValidEntities.QuestionSet(null);
            questionSetToUpdate.Name = StaticValues.QuestionSet_ContactInformation + "oops";
            QuestionSetRepository.Expect(a => a.GetNullableByID(10)).Return(QuestionSets[9]).Repeat.Any();
            #endregion Arrange

            #region Act
            var result = Controller.Edit(10, questionSetToUpdate)
                .AssertViewRendered()
                .WithViewData<QuestionSetViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            QuestionSetRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<QuestionSet>.Is.Anything));
            Assert.AreEqual("This is a system default question set and cannot be modified", Controller.Message);
            Controller.ModelState.AssertErrorsAre("This is a system default question set and cannot be modified");
            #endregion Assert		
        } 

        #endregion Edit Post Tests

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
            Assert.AreEqual(12, result.Count(), "It looks like a method was added or removed from the controller.");
            #endregion Assert
        }

        /// <summary>
        /// Tests the controller method index contains expected attributes.
        /// </summary>
        [TestMethod]
        public void TestControllerMethodIndexContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethod("Index");
            #endregion Arrange

            #region Act
            //var expectedAttribute = controllerMethod.GetCustomAttributes(true).OfType<AcceptPostAttribute>();
            var allAttributes = controllerMethod.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            //Assert.AreEqual(1, expectedAttribute.Count(), "AcceptPostAttribute not found");
            Assert.AreEqual(0, allAttributes.Count(), "More than expected custom attributes found.");
            #endregion Assert
        }

        /// <summary>
        /// Tests the controller method list contains expected attributes.
        /// </summary>
        [TestMethod]
        public void TestControllerMethodListContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethod("List");
            #endregion Arrange

            #region Act
            //var expectedAttribute = controllerMethod.GetCustomAttributes(true).OfType<AcceptPostAttribute>();
            var allAttributes = controllerMethod.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            //Assert.AreEqual(1, expectedAttribute.Count(), "AcceptPostAttribute not found");
            Assert.AreEqual(0, allAttributes.Count(), "More than expected custom attributes found.");
            #endregion Assert
        }

        [TestMethod]
        public void TestControllerMethodDetailsContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethod("Details");
            #endregion Arrange

            #region Act
            //var expectedAttribute = controllerMethod.GetCustomAttributes(true).OfType<AcceptPostAttribute>();
            var allAttributes = controllerMethod.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            //Assert.AreEqual(1, expectedAttribute.Count(), "AcceptPostAttribute not found");
            Assert.AreEqual(0, allAttributes.Count(), "More than expected custom attributes found.");
            #endregion Assert
        }

        /// <summary>
        /// Tests the controller method edit contains expected attributes.
        /// </summary>
        [TestMethod]
        public void TestControllerMethodEditContainsExpectedAttributes1()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "Edit");
            #endregion Arrange

            #region Act
            var allAttributes = controllerMethod.ElementAt(0).GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, allAttributes.Count());
            Assert.AreEqual(2, controllerMethod.Count(), "Over loads for edit changed");
            #endregion Assert
        }

        /// <summary>
        /// Tests the controller method edit contains expected attributes.
        /// </summary>
        [TestMethod]
        public void TestControllerMethodEditContainsExpectedAttributes2()
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
            Assert.AreEqual(1, allAttributes.Count(), "More than expected custom attributes found.");
            #endregion Assert
        }

        /// <summary>
        /// Tests the controller method create contains expected attributes.
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
            Assert.AreEqual(2, controllerMethod.Count(), "Over loads for create changed");
            #endregion Assert
        }

        /// <summary>
        /// Tests the controller method create contains expected attributes.
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
            Assert.AreEqual(2, allAttributes.Count(), "More than expected custom attributes found.");
            #endregion Assert
        }

        /// <summary>
        /// Tests the controller method create contains expected attributes.
        /// </summary>
        [TestMethod]
        public void TestControllerMethodCreateContainsExpectedAttributes3()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "Create");
            #endregion Arrange

            #region Act
            var expectedAttribute = controllerMethod.ElementAt(1).GetCustomAttributes(true).OfType<HandleTransactionsManuallyAttribute>();
            var allAttributes = controllerMethod.ElementAt(1).GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "HandleTransactionsManuallyAttribute not found");
            Assert.AreEqual(2, allAttributes.Count(), "More than expected custom attributes found.");
            #endregion Assert
        }

        /// <summary>
        /// Tests the controller method link to item type contains expected attributes.
        /// </summary>
        [TestMethod]
        public void TestControllerMethodLinkToItemTypeContainsExpectedAttributes1()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "LinkToItemType");
            #endregion Arrange

            #region Act
            var expectedAttribute = controllerMethod.ElementAt(0).GetCustomAttributes(true).OfType<AuthorizeAttribute>();
            var allAttributes = controllerMethod.ElementAt(0).GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "AuthorizeAttribute not found");
            Assert.AreEqual("Admin", expectedAttribute.ElementAt(0).Roles);
            Assert.AreEqual(1, allAttributes.Count());
            Assert.AreEqual(2, controllerMethod.Count(), "Over loads for create changed");
            #endregion Assert
        }
        /// <summary>
        /// Tests the controller method link to item type contains expected attributes.
        /// </summary>
        [TestMethod]
        public void TestControllerMethodLinkToItemTypeContainsExpectedAttributes2()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "LinkToItemType");
            #endregion Arrange

            #region Act
            var expectedAttribute = controllerMethod.ElementAt(1).GetCustomAttributes(true).OfType<AcceptPostAttribute>();
            var allAttributes = controllerMethod.ElementAt(1).GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "AcceptPostAttribute not found");
            Assert.AreEqual(2, allAttributes.Count());
            #endregion Assert
        }
        /// <summary>
        /// Tests the controller method link to item type contains expected attributes.
        /// </summary>
        [TestMethod]
        public void TestControllerMethodLinkToItemTypeContainsExpectedAttributes3()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "LinkToItemType");
            #endregion Arrange

            #region Act
            var expectedAttribute = controllerMethod.ElementAt(1).GetCustomAttributes(true).OfType<AdminOnlyAttribute>();
            var allAttributes = controllerMethod.ElementAt(1).GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "AdminOnlyAttribute not found");
            Assert.AreEqual(2, allAttributes.Count());
            #endregion Assert
        }

        /// <summary>
        /// Tests the controller method link to item contains expected attributes.
        /// </summary>
        [TestMethod]
        public void TestControllerMethodLinkToItemContainsExpectedAttributes1()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "LinkToItem");
            #endregion Arrange

            #region Act
            var expectedAttribute = controllerMethod.ElementAt(0).GetCustomAttributes(true).OfType<UserOnlyAttribute>();
            var allAttributes = controllerMethod.ElementAt(0).GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "UserOnlyAttribute not found");
            Assert.AreEqual(1, allAttributes.Count());
            Assert.AreEqual(2, controllerMethod.Count(), "Over loads for create changed");
            #endregion Assert
        }
        /// <summary>
        /// Tests the controller method link to item contains expected attributes.
        /// </summary>
        [TestMethod]
        public void TestControllerMethodLinkToItemContainsExpectedAttributes2()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "LinkToItem");
            #endregion Arrange

            #region Act
            var expectedAttribute = controllerMethod.ElementAt(1).GetCustomAttributes(true).OfType<AcceptPostAttribute>();
            var allAttributes = controllerMethod.ElementAt(1).GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "AcceptPostAttribute not found");
            Assert.AreEqual(2, allAttributes.Count());
            #endregion Assert
        }
        /// <summary>
        /// Tests the controller method link to item contains expected attributes.
        /// </summary>
        [TestMethod]
        public void TestControllerMethodLinkToItemContainsExpectedAttributes3()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "LinkToItem");
            #endregion Arrange

            #region Act
            var expectedAttribute = controllerMethod.ElementAt(1).GetCustomAttributes(true).OfType<UserOnlyAttribute>();
            var allAttributes = controllerMethod.ElementAt(1).GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "UserOnlyAttribute not found");
            Assert.AreEqual(2, allAttributes.Count());
            #endregion Assert
        }

        /// <summary>
        /// Tests the controller method unlink from item contains expected attributes.
        /// </summary>
        [TestMethod]
        public void TestControllerMethodUnlinkFromItemContainsExpectedAttributes1()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethod("UnlinkFromItem");
            #endregion Arrange

            #region Act
            var expectedAttribute = controllerMethod.GetCustomAttributes(true).OfType<AcceptPostAttribute>();
            var allAttributes = controllerMethod.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "AcceptPostAttribute not found");
            Assert.AreEqual(2, allAttributes.Count(), "More than expected custom attributes found.");
            #endregion Assert
        }

        /// <summary>
        /// Tests the controller method unlink from item contains expected attributes.
        /// </summary>
        [TestMethod]
        public void TestControllerMethodUnlinkFromItemContainsExpectedAttributes2()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethod("UnlinkFromItem");
            #endregion Arrange

            #region Act
            var expectedAttribute = controllerMethod.GetCustomAttributes(true).OfType<UserOnlyAttribute>();
            var allAttributes = controllerMethod.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "UserOnlyAttribute not found");
            Assert.AreEqual(2, allAttributes.Count(), "More than expected custom attributes found.");
            #endregion Assert
        }
        #endregion Controller Method Tests

        #endregion Reflection Tests

        #region Helper Methods

      

        /// <summary>
        /// Sets up data for edit tests.
        /// </summary>
        private void SetupDataForTests()
        {
            ControllerRecordFakes.FakeQuestionTypes(QuestionTypes, 3);
            ControllerRecordFakes.FakeUsers(Users, 3);
            ControllerRecordFakes.FakeUnits(Units, 5);
            ControllerRecordFakes.FakeSchools(Schools, 5);
            var editor = CreateValidEntities.Editor(1);
            editor.User = Users[0];
            var itemQuestionSet = CreateValidEntities.ItemQuestionSet(1);
            itemQuestionSet.Item = CreateValidEntities.Item(1);
            itemQuestionSet.Item.AddEditor(editor);

            for (int i = 0; i < 5; i++)
            {
                Units[i].School = Schools[i];
            }

            Users[1].Units.Add(Units[1]);
            Users[1].Units.Add(Units[3]);
            Users[1].Units.Add(Units[4]);
            Users[1].LoginID = "UserName";

            var editor2 = CreateValidEntities.Editor(2);
            editor2.User = Users[1];
            var itemQuestionSet2 = CreateValidEntities.ItemQuestionSet(2);
            itemQuestionSet2.Item = CreateValidEntities.Item(2);
            itemQuestionSet2.Item.AddEditor(editor2);

            ControllerRecordFakes.FakeQuestionSets(QuestionSets, 10);
            QuestionSets[0].UserReusable = true;
            QuestionSets[0].User = Users[1];
            QuestionSets[1].UserReusable = true;
            QuestionSets[2].UserReusable = true;
            QuestionSets[2].User = Users[1];
            QuestionSets[3].CollegeReusable = true;
            QuestionSets[3].School = Schools[1]; //Has
            QuestionSets[4].CollegeReusable = true;
            QuestionSets[4].School = Schools[2]; //Doesn't have
            QuestionSets[5].CollegeReusable = true;
            QuestionSets[5].School = Schools[4]; //Has
            QuestionSets[6].SystemReusable = true;
            QuestionSets[7].User = Users[0]; //Not the owner
            QuestionSets[7].ItemTypes.Add(CreateValidEntities.ItemTypeQuestionSet(null));
            QuestionSets[8].User = Users[0]; //Not the owner
            QuestionSets[8].ItemTypes = new List<ItemTypeQuestionSet>();
            QuestionSets[8].Items.Add(itemQuestionSet);
            QuestionSets[9].User = Users[0]; //Not the owner
            QuestionSets[9].ItemTypes = new List<ItemTypeQuestionSet>();
            QuestionSets[9].Items.Add(itemQuestionSet2); //Is an editor

            UserRepository.Expect(a => a.Queryable).Return(Users.AsQueryable()).Repeat.Any();
            QuestionSetRepository.Expect(a => a.Queryable).Return(QuestionSets.AsQueryable()).Repeat.Any();
            QuestionTypeRepository.Expect(a => a.Queryable).Return(QuestionTypes.AsQueryable()).Repeat.Any();
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

        #endregion Helper Methods
    }
}
