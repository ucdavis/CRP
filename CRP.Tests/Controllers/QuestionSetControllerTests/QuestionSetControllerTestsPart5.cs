using CRP.Controllers;
using CRP.Controllers.Helpers;
using CRP.Controllers.ViewModels;
using CRP.Core.Domain;
using CRP.Core.Resources;
using CRP.Tests.Core.Extensions;
using CRP.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Rhino.Mocks;

namespace CRP.Tests.Controllers.QuestionSetControllerTests
{
    public partial class QuestionSetControllerTests
    {
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
        /// Tests the edit post when question set id not found redirects to list.
        /// </summary>
        [TestMethod]
        public void TestEditPostWhenQuestionSetIdNotFoundRedirectsToList()
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


        /// <summary>
        /// Tests the edit post will not save if the contact name is changed to contact information.
        /// </summary>
        [TestMethod]
        public void TestEditPostWillNotSaveIfTheContactNameIsChangedToContactInformation()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.User });
            SetupDataForTests();
            var questionSetToUpdate = CreateValidEntities.QuestionSet(null);
            questionSetToUpdate.Name = StaticValues.QuestionSet_ContactInformation.ToUpper();
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
            Assert.AreEqual(StaticValues.QuestionSet_ContactInformation + " is reserved for internal system use only.", Controller.Message);
            Controller.ModelState.AssertErrorsAre(StaticValues.QuestionSet_ContactInformation + " is reserved for internal system use only.");
            #endregion Assert
        }

        #endregion Edit Post Tests
    }
}
