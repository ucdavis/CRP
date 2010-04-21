using System.Linq;
using CRP.Controllers;
using CRP.Controllers.Helpers;
using CRP.Core.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;

namespace CRP.Tests.Controllers.QuestionSetControllerTests
{
    public partial class QuestionSetControllerTests
    {
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
    }
}
