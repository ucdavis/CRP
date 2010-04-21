using System.Linq;
using CRP.Controllers;
using CRP.Controllers.ViewModels;
using CRP.Core.Domain;
using CRP.Core.Resources;
using CRP.Tests.Core.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Rhino.Mocks;

namespace CRP.Tests.Controllers.QuestionSetControllerTests
{
    public partial class QuestionSetControllerTests
    {
        #region LinkToItemType Get Tests

        /// <summary>
        /// Tests the link to item type Get redirects to list item types if item type id not found.
        /// </summary>
        [TestMethod]
        public void TestLinkToItemTypeGetRedirectsToListItemTypesIfItemTypeIdNotFound()
        {
            #region Arrange
            SetupDataForLinkToTests();
            ItemTypeRepository.Expect(a => a.GetNullableByID(1)).Return(null).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.LinkToItemType(1, true, false)
                .AssertActionRedirect()
                .ToAction<ApplicationManagementController>(a => a.ListItemTypes());
            #endregion Act

            #region Assert
            Assert.AreEqual("ItemType not found.", Controller.Message);
            #endregion Assert
        }

        /// <summary>
        /// Tests the link to item type Get redirects to list item types if item type id found but transaction and quantity are both true.
        /// </summary>
        [TestMethod]
        public void TestLinkToItemTypeGetRedirectsToListItemTypesIfItemTypeIdFoundButTransactionAndQuantityAreBothTrue()
        {
            #region Arrange
            SetupDataForLinkToTests();
            #endregion Arrange

            #region Act
            Controller.LinkToItemType(2, true, true)
                .AssertActionRedirect()
                .ToAction<ApplicationManagementController>(a => a.ListItemTypes());
            #endregion Act

            #region Assert
            Assert.AreEqual("Unable to determine if this is to be linked to a Transaction or a Quantity QuestionSet.", Controller.Message);
            #endregion Assert
        }

        /// <summary>
        /// Tests the link to item type Get redirects to list item types if item type id found but transaction and quantity are both false.
        /// </summary>
        [TestMethod]
        public void TestLinkToItemTypeGetRedirectsToListItemTypesIfItemTypeIdFoundButTransactionAndQuantityAreBothFalse()
        {
            #region Arrange
            SetupDataForLinkToTests();
            #endregion Arrange

            #region Act
            Controller.LinkToItemType(2, false, false)
                .AssertActionRedirect()
                .ToAction<ApplicationManagementController>(a => a.ListItemTypes());
            #endregion Act

            #region Assert
            Assert.AreEqual("Unable to determine if this is to be linked to a Transaction or a Quantity QuestionSet.", Controller.Message);
            #endregion Assert
        }

        /// <summary>
        /// Tests the link to item type Get returns view when item type found and transaction true and quantity false.
        /// </summary>
        [TestMethod]
        public void TestLinkToItemTypeGetReturnsViewWhenItemTypeFoundAndTransactionTrueAndQuantityFalse()
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
        /// Tests the link to item type Get returns view when item type found and transaction false and quantity true.
        /// </summary>
        [TestMethod]
        public void TestLinkToItemTypeGetReturnsViewWhenItemTypeFoundAndTransactionFalseAndQuantityTrue()
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

        #region LinkToItemTypes Post Tests

        /// <summary>
        /// Tests the link to item type post redirects to list item types when question set id not found.
        /// </summary>
        [TestMethod]
        public void TestLinkToItemTypePostRedirectsToListItemTypesWhenQuestionSetIdNotFound()
        {
            #region Arrange
            SetupDataForLinkToTests();
            QuestionSetRepository.Expect(a => a.GetNullableByID(1)).Return(null).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.LinkToItemType(1, 2, true, false)
                .AssertActionRedirect()
                .ToAction<ApplicationManagementController>(a => a.ListItemTypes());
            #endregion Act

            #region Assert
            Assert.AreEqual("QuestionSet not found.", Controller.Message);
            ItemTypeRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<ItemType>.Is.Anything));
            #endregion Assert		
        }

        /// <summary>
        /// Tests the link to item type post redirects to list item types when item type id not found.
        /// </summary>
        [TestMethod]
        public void TestLinkToItemTypePostRedirectsToListItemTypesWhenItemTypeIdNotFound()
        {
            #region Arrange
            SetupDataForLinkToTests();
            ItemTypeRepository.Expect(a => a.GetNullableByID(1)).Return(null).Repeat.Any();

            #endregion Arrange

            #region Act
            Controller.LinkToItemType(2, 1, true, false)
                .AssertActionRedirect()
                .ToAction<ApplicationManagementController>(a => a.ListItemTypes());
            #endregion Act

            #region Assert
            Assert.AreEqual("ItemType not found.", Controller.Message);
            ItemTypeRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<ItemType>.Is.Anything));
            #endregion Assert
        }


        /// <summary>
        /// Tests the link to item type post redirects to list if transaction and quantity are both true.
        /// </summary>
        [TestMethod]
        public void TestLinkToItemTypePostRedirectsToListIfTransactionAndQuantityAreBothTrue()
        {
            #region Arrange
            SetupDataForLinkToTests();
            ItemTypeRepository.Expect(a => a.GetNullableByID(1)).Return(null).Repeat.Any();

            #endregion Arrange

            #region Act
            Controller.LinkToItemType(2, 2, true, true)
                .AssertActionRedirect()
                .ToAction<ApplicationManagementController>(a => a.ListItemTypes());
            #endregion Act

            #region Assert
            Assert.AreEqual("Unable to determine if this is to be linked to a Transaction or a Quantity QuestionSet.", Controller.Message);
            ItemTypeRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<ItemType>.Is.Anything));
            #endregion Assert	
        }

        /// <summary>
        /// Tests the link to item type post redirects to list if transaction and quantity are both false.
        /// </summary>
        [TestMethod]
        public void TestLinkToItemTypePostRedirectsToListIfTransactionAndQuantityAreBothFalse()
        {
            #region Arrange
            SetupDataForLinkToTests();
            ItemTypeRepository.Expect(a => a.GetNullableByID(1)).Return(null).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.LinkToItemType(2, 2, false, false)
                .AssertActionRedirect()
                .ToAction<ApplicationManagementController>(a => a.ListItemTypes());
            #endregion Act

            #region Assert
            Assert.AreEqual("Unable to determine if this is to be linked to a Transaction or a Quantity QuestionSet.", Controller.Message);
            ItemTypeRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<ItemType>.Is.Anything));
            #endregion Assert
        }

        /// <summary>
        /// Tests the link to item type does not save if the question set has already been added.
        /// </summary>
        [TestMethod]
        public void TestLinkToItemTypeDoesNotSaveIfTheQuestionSetHasAlreadyBeenAdded()
        {
            #region Arrange
            SetupDataForLinkToTests();
            Controller.LinkToItemType(2, 2, false, true)
                .AssertActionRedirect()
                .ToAction<ApplicationManagementController>(a => a.EditItemType(ItemTypes[1].Id));
            #endregion Arrange

            #region Act
            //Do it again to force failure (But with transaction, not quantity to show that doesn't matter)
            Controller.LinkToItemType(2, 2, true, false)
                .AssertViewRendered()
                .WithViewData<QuestionSetLinkViewModel>();
            #endregion Act

            #region Assert
            Controller.ModelState.AssertErrorsAre("QuestionSet \"Name2\" is already added to the ItemType.");
            Assert.AreEqual("QuestionSet was already added", Controller.Message);
            //ItemTypeRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<ItemType>.Is.Anything));
            #endregion Assert		
        }


        /// <summary>
        /// Tests the link to item type post does not save if the question name is contact information.
        /// In reality, this is a minor test as Contact Information can't be added twice and it is filtered from the list provided
        /// </summary>
        [TestMethod]
        public void TestLinkToItemTypePostDoesNotSaveIfTheQuestionNameIsContactInformation()
        {
            #region Arrange
            SetupDataForLinkToTests();
            QuestionSets[1].Name = StaticValues.QuestionSet_ContactInformation;
            #endregion Arrange

            #region Act
            Controller.LinkToItemType(2, 2, true, false)
                .AssertViewRendered()
                .WithViewData<QuestionSetLinkViewModel>();
            #endregion Act

            #region Assert
            Controller.ModelState.AssertErrorsAre("A duplicate Contact Information question set cannot be added.");
            Assert.IsNull(Controller.Message);
            ItemTypeRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<ItemType>.Is.Anything));
            #endregion Assert		
        }

        /// <summary>
        /// Tests the link to item type post assigns the question set to transaction when transaction is true.
        /// </summary>
        [TestMethod]
        public void TestLinkToItemTypePostAssignsTheQuestionSetToTransactionWhenTransactionIsTrue()
        {
            #region Arrange
            SetupDataForLinkToTests();
            #endregion Arrange

            #region Act
            Controller.LinkToItemType(2, 2, true, false)
                .AssertActionRedirect()
                .ToAction<ApplicationManagementController>(a => a.EditItemType(ItemTypes[1].Id));
            #endregion Act

            #region Assert
            ItemTypeRepository.AssertWasCalled(a => a.EnsurePersistent(ItemTypes[1]));
            Assert.AreEqual("Question Set has been created successfully.", Controller.Message);
            Assert.IsTrue(ItemTypes[1].QuestionSets.ElementAt(0).TransactionLevel);
            Assert.IsFalse(ItemTypes[1].QuestionSets.ElementAt(0).QuantityLevel);
            #endregion Assert		
        }

        /// <summary>
        /// Tests the link to item type post assigns the question set to quantity when quantity is true.
        /// </summary>
        [TestMethod]
        public void TestLinkToItemTypePostAssignsTheQuestionSetToQuantityWhenQuantityIsTrue()
        {
            #region Arrange
            SetupDataForLinkToTests();
            #endregion Arrange

            #region Act
            Controller.LinkToItemType(2, 2, false, true)
                .AssertActionRedirect()
                .ToAction<ApplicationManagementController>(a => a.EditItemType(ItemTypes[1].Id));
            #endregion Act

            #region Assert
            ItemTypeRepository.AssertWasCalled(a => a.EnsurePersistent(ItemTypes[1]));
            Assert.AreEqual("Question Set has been created successfully.", Controller.Message);
            Assert.IsFalse(ItemTypes[1].QuestionSets.ElementAt(0).TransactionLevel);
            Assert.IsTrue(ItemTypes[1].QuestionSets.ElementAt(0).QuantityLevel);
            #endregion Assert
        }
        #endregion LinkToItemTypes Post Tests
    }
}
