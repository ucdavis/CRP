using System.Linq;
using CRP.Controllers;
using CRP.Controllers.Helpers;
using CRP.Controllers.ViewModels;
using CRP.Core.Domain;
using CRP.Tests.Core.Extensions;
using CRP.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Rhino.Mocks;

namespace CRP.Tests.Controllers.QuestionSetControllerTests
{
    public partial class QuestionSetControllerTests
    {
        #region LinkToItemType Get Tests

        /// <summary>
        /// Tests the link to item type redirects to list item types if item type id not found.
        /// </summary>
        [TestMethod]
        public void TestLinkToItemTypeRedirectsToListItemTypesIfItemTypeIdNotFound()
        {
            #region Arrange
            SetupDataForLinkToTests();
            ItemTypeRepository.Expect(a => a.GetNullableByID(1)).Return(null).Repeat.Any();
            #endregion Arrange

            #region Act/Assert
            Controller.LinkToItemType(1, true, false)
                .AssertActionRedirect()
                .ToAction<ApplicationManagementController>(a => a.ListItemTypes());
            #endregion Act/Assert
        }

        /// <summary>
        /// Tests the link to item type redirects to list item types if item type id found but transaction and quantity are both true.
        /// </summary>
        [TestMethod]
        public void TestLinkToItemTypeRedirectsToListItemTypesIfItemTypeIdFoundButTransactionAndQuantityAreBothTrue()
        {
            #region Arrange
            SetupDataForLinkToTests();
            #endregion Arrange

            #region Act/Assert
            Controller.LinkToItemType(2, true, true)
                .AssertActionRedirect()
                .ToAction<ApplicationManagementController>(a => a.ListItemTypes());
            #endregion Act/Assert
        }

        /// <summary>
        /// Tests the link to item type redirects to list item types if item type id found but transaction and quantity are both false.
        /// </summary>
        [TestMethod]
        public void TestLinkToItemTypeRedirectsToListItemTypesIfItemTypeIdFoundButTransactionAndQuantityAreBothFalse()
        {
            #region Arrange
            SetupDataForLinkToTests();
            #endregion Arrange

            #region Act/Assert
            Controller.LinkToItemType(2, false, false)
                .AssertActionRedirect()
                .ToAction<ApplicationManagementController>(a => a.ListItemTypes());
            #endregion Act/Assert
        }

        /// <summary>
        /// Tests the link to item type returns view when item type found and transaction true and quantity false.
        /// </summary>
        [TestMethod]
        public void TestLinkToItemTypeReturnsViewWhenItemTypeFoundAndTransactionTrueAndQuantityFalse()
        {
            #region Arrange
            SetupDataForLinkToTests();           
            #endregion Arrange

            #region Act
            var result = Controller.LinkToItemType(2, true, false)
                .AssertViewRendered()
                .WithViewData<QuestionSetLinkViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Transaction);
            Assert.IsFalse(result.Quantity);
            Assert.AreEqual(5, result.QuestionSets.Count());
            Assert.IsTrue(result.QuestionSets.Contains(QuestionSets[0]));
            Assert.IsTrue(result.QuestionSets.Contains(QuestionSets[2]));
            Assert.IsTrue(result.QuestionSets.Contains(QuestionSets[3]));
            Assert.IsTrue(result.QuestionSets.Contains(QuestionSets[5]));
            Assert.IsTrue(result.QuestionSets.Contains(QuestionSets[6]));
            #endregion Assert		
        }

        /// <summary>
        /// Tests the link to item type returns view when item type found and transaction false and quantity true.
        /// </summary>
        [TestMethod]
        public void TestLinkToItemTypeReturnsViewWhenItemTypeFoundAndTransactionFalseAndQuantityTrue()
        {
            #region Arrange
            SetupDataForLinkToTests();
            #endregion Arrange

            #region Act
            var result = Controller.LinkToItemType(2, false, true)
                .AssertViewRendered()
                .WithViewData<QuestionSetLinkViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Transaction);
            Assert.IsTrue(result.Quantity);
            Assert.AreEqual(5, result.QuestionSets.Count());
            Assert.IsTrue(result.QuestionSets.Contains(QuestionSets[0]));
            Assert.IsTrue(result.QuestionSets.Contains(QuestionSets[2]));
            Assert.IsTrue(result.QuestionSets.Contains(QuestionSets[3]));
            Assert.IsTrue(result.QuestionSets.Contains(QuestionSets[5]));
            Assert.IsTrue(result.QuestionSets.Contains(QuestionSets[6]));
            #endregion Assert
        }
        #endregion LinkToItemType Get Tests
    }
}
