using CRP.Controllers;
using CRP.Core.Domain;
using CRP.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Rhino.Mocks;

namespace CRP.Tests.Controllers.QuestionSetControllerTests
{
    public partial class QuestionSetControllerTests
    {
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
    }
}
