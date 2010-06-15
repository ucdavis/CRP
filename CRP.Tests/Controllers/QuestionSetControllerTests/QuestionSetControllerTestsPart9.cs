using System.Linq;
using System.Web.Mvc;
using CRP.Controllers;
using CRP.Controllers.Helpers;
using CRP.Controllers.ViewModels;
using CRP.Core.Domain;
using CRP.Core.Resources;
using CRP.Tests.Core.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Rhino.Mocks;
using UCDArch.Testing;

namespace CRP.Tests.Controllers.QuestionSetControllerTests
{
    public partial class QuestionSetControllerTests
    {
        #region LinkToItem Get Tests

        /// <summary>
        /// Tests the link to item get redirects to list when item not found.
        /// </summary>
        [TestMethod]
        public void TestLinkToItemGetRedirectsToListWhenItemNotFound()
        {
            #region Arrange
            SetupDataForLinkToTests();
            ItemRepository.Expect(a => a.GetNullableByID(1)).Return(null).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.LinkToItem(1, true, false)
                .AssertActionRedirect()
                .ToAction<ItemManagementController>(a => a.List(null));
            #endregion Act

            #region Assert
            Assert.AreEqual("Item not found.", Controller.Message);
            #endregion Assert		
        }

        /// <summary>
        /// Tests the link to item get redirects to list when transaction and quantity are both true.
        /// </summary>
        [TestMethod]
        public void TestLinkToItemGetRedirectsToListWhenTransactionAndQuantityAreBothTrue()
        {
            #region Arrange
            SetupDataForLinkToTests();
            ItemRepository.Expect(a => a.GetNullableByID(1)).Return(null).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.LinkToItem(2, true, true)
                .AssertActionRedirect()
                .ToAction<ItemManagementController>(a => a.List(null));
            #endregion Act

            #region Assert
            Assert.AreEqual("Unable to determine if this is to be linked to a Transaction or a Quantity QuestionSet.", Controller.Message);
            #endregion Assert
        }

        /// <summary>
        /// Tests the link to item get redirects to list when transaction and quantity are both false.
        /// </summary>
        [TestMethod]
        public void TestLinkToItemGetRedirectsToListWhenTransactionAndQuantityAreBothFalse()
        {
            #region Arrange
            SetupDataForLinkToTests();
            ItemRepository.Expect(a => a.GetNullableByID(1)).Return(null).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.LinkToItem(2, false, false)
                .AssertActionRedirect()
                .ToAction<ItemManagementController>(a => a.List(null));
            #endregion Act

            #region Assert
            Assert.AreEqual("Unable to determine if this is to be linked to a Transaction or a Quantity QuestionSet.", Controller.Message);
            #endregion Assert
        }
        /// <summary>
        /// Tests the link to item get returns view when valid data.
        /// </summary>
        [TestMethod]
        public void TestLinkToItemGetReturnsViewWhenValidData1()
        {
            #region Arrange
            SetupDataForLinkToTests();
            #endregion Arrange

            #region Act
            var result = Controller.LinkToItem(2, true, false)
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
        /// Tests the link to item get returns view when valid data.
        /// </summary>
        [TestMethod]
        public void TestLinkToItemGetReturnsViewWhenValidData2()
        {
            #region Arrange
            SetupDataForLinkToTests();
            #endregion Arrange

            #region Act
            var result = Controller.LinkToItem(2, false, true)
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
        #endregion LinkToItem Get Tests
        
        #region LinkToItem Post Tests

        /// <summary>
        /// Tests the link to item post redirects to list if question set not found.
        /// </summary>
        [TestMethod]
        public void TestLinkToItemPostRedirectsToListIfQuestionSetNotFound()
        {
            #region Arrange
            SetupDataForLinkToTests();
            QuestionSetRepository.Expect(a => a.GetNullableByID(1)).Return(null).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.LinkToItem(1, 2, true, false)
                .AssertActionRedirect()
                .ToAction<ItemManagementController>(a => a.List(null));
            #endregion Act

            #region Assert
            Assert.AreEqual("QuestionSet not found.", Controller.Message);
            ItemRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Item>.Is.Anything));
            #endregion Assert		
        }

        /// <summary>
        /// Tests the link to item post redirects to list if item not found.
        /// </summary>
        [TestMethod]
        public void TestLinkToItemPostRedirectsToListIfItemNotFound()
        {
            #region Arrange
            SetupDataForLinkToTests();
            ItemRepository.Expect(a => a.GetNullableByID(1)).Return(null).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.LinkToItem(2, 1, true, false)
                .AssertActionRedirect()
                .ToAction<ItemManagementController>(a => a.List(null));
            #endregion Act

            #region Assert
            Assert.AreEqual("Item not found.", Controller.Message);
            ItemRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Item>.Is.Anything));
            #endregion Assert
        }

        /// <summary>
        /// Tests the link to item post redirects to list if both transaction and quantity are true.
        /// </summary>
        [TestMethod]
        public void TestLinkToItemPostRedirectsToListIfBothTransactionAndQuantityAreTrue()
        {
            #region Arrange
            SetupDataForLinkToTests();
            ItemRepository.Expect(a => a.GetNullableByID(1)).Return(null).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.LinkToItem(2, 2, true, true)
                .AssertActionRedirect()
                .ToAction<ItemManagementController>(a => a.List(null));
            #endregion Act

            #region Assert
            Assert.AreEqual("Unable to determine if this is to be linked to a Transaction or a Quantity QuestionSet.", Controller.Message);
            ItemRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Item>.Is.Anything));
            #endregion Assert
        }

        /// <summary>
        /// Tests the link to item post redirects to list if both transaction and quantity are false.
        /// </summary>
        [TestMethod]
        public void TestLinkToItemPostRedirectsToListIfBothTransactionAndQuantityAreFalse()
        {
            #region Arrange
            SetupDataForLinkToTests();
            ItemRepository.Expect(a => a.GetNullableByID(1)).Return(null).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.LinkToItem(2, 2, false, false)
                .AssertActionRedirect()
                .ToAction<ItemManagementController>(a => a.List(null));
            #endregion Act

            #region Assert
            Assert.AreEqual("Unable to determine if this is to be linked to a Transaction or a Quantity QuestionSet.", Controller.Message);
            ItemRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Item>.Is.Anything));
            #endregion Assert
        }


        /// <summary>
        /// Tests the link to item post returns view if same question set is added to A transaction level.
        /// </summary>
        [TestMethod]
        public void TestLinkToItemPostReturnsViewIfSameQuestionSetIsAddedToATransactionLevel()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Response
                .Expect(a => a.ApplyAppPathModifier(null)).IgnoreArguments()
                .Return("http://sample.com/ItemManagement/Edit/2").Repeat.Any();
            Controller.Url = MockRepository.GenerateStub<UrlHelper>(Controller.ControllerContext.RequestContext);
            SetupDataForLinkToTests();
            Controller.LinkToItem(2, 2, true, false)
                .AssertHttpRedirect();
            #endregion Arrange

            #region Act
            Controller.LinkToItem(2, 2, true, false)
                .AssertViewRendered()
                .WithViewData<QuestionSetLinkViewModel>();
            #endregion Act

            #region Assert
            Assert.AreEqual("That question set is already linked to the transaction question sets", Controller.Message);
            #endregion Assert		
        }

        /// <summary>
        /// Tests the link to item post returns view if same question set is added to A quantity level.
        /// </summary>
        [TestMethod]
        public void TestLinkToItemPostReturnsViewIfSameQuestionSetIsAddedToAQuantityLevel()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Response
                .Expect(a => a.ApplyAppPathModifier(null)).IgnoreArguments()
                .Return("http://sample.com/ItemManagement/Edit/2").Repeat.Any();
            Controller.Url = MockRepository.GenerateStub<UrlHelper>(Controller.ControllerContext.RequestContext);
            SetupDataForLinkToTests();
            Controller.LinkToItem(2, 2, false, true)
                .AssertHttpRedirect();
            #endregion Arrange

            #region Act
            Controller.LinkToItem(2, 2, false, true)
                .AssertViewRendered()
                .WithViewData<QuestionSetLinkViewModel>();
            #endregion Act

            #region Assert
            Assert.AreEqual("That question set is already linked to the quantity question sets", Controller.Message);
            #endregion Assert
        }

        /// <summary>
        /// Tests the link to item post saves if same question set is added to A quantity level that is in A transaction level.
        /// </summary>
        [TestMethod]
        public void TestLinkToItemPostSavesIfSameQuestionSetIsAddedToAQuantityLevelThatIsInATransactionLevel()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Response
                .Expect(a => a.ApplyAppPathModifier(null)).IgnoreArguments()
                .Return("http://sample.com/ItemManagement/Edit/2").Repeat.Any();
            Controller.Url = MockRepository.GenerateStub<UrlHelper>(Controller.ControllerContext.RequestContext);
            SetupDataForLinkToTests();
            Controller.LinkToItem(2, 2, true, false)
                .AssertHttpRedirect();
            #endregion Arrange

            #region Act
            Controller.LinkToItem(2, 2, false, true)
                .AssertHttpRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual(NotificationMessages.STR_ObjectCreated.Replace(NotificationMessages.ObjectType, "Question Set"), Controller.Message);
            ItemRepository.AssertWasCalled(a => a.EnsurePersistent(Items[1]));
            #endregion Assert
        }

        /// <summary>
        /// Tests the link to item post returns view if item has an error.
        /// </summary>
        [TestMethod]
        public void TestLinkToItemPostReturnsViewIfItemHasAnError()
        {
            #region Arrange
            SetupDataForLinkToTests();
            Items[1].Name = null;
            #endregion Arrange

            #region Act
            Controller.LinkToItem(2, 2, true, false)
                .AssertViewRendered()
                .WithViewData<QuestionSetLinkViewModel>();
            #endregion Act

            #region Assert
            Assert.AreEqual("An error with the item prevents this from saving.", Controller.Message);
            ItemRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Item>.Is.Anything));
            #endregion Assert
        }


        [TestMethod]
        public void TestLinkToItemSavesWithValidData()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Response
                .Expect(a => a.ApplyAppPathModifier(null)).IgnoreArguments()
                .Return("http://sample.com/ItemManagement/Edit/2").Repeat.Any();
            Controller.Url = MockRepository.GenerateStub<UrlHelper>(Controller.ControllerContext.RequestContext);
            SetupDataForLinkToTests();
            #endregion Arrange

            #region Act
            Controller.LinkToItem(2, 2, true, false)
                .AssertHttpRedirect();
            #endregion Act

            #region Assert
            ItemRepository.AssertWasCalled(a => a.EnsurePersistent(Items[1]));
            Assert.AreEqual("Question Set has been created successfully.", Controller.Message);
            #endregion Assert		
        }
        [TestMethod]
        public void TestLinkToItemDoesNotSaveIfNotAdminAndNotEditor()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Response
                .Expect(a => a.ApplyAppPathModifier(null)).IgnoreArguments()
                .Return("http://sample.com/ItemManagement/Edit/2").Repeat.Any();
            Controller.Url = MockRepository.GenerateStub<UrlHelper>(Controller.ControllerContext.RequestContext);
            SetupDataForLinkToTests();

            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.User });
            #endregion Arrange

            #region Act
            Controller.LinkToItem(2, 2, true, false)
                .AssertActionRedirect()
                .ToAction<ItemManagementController>(a => a.List(null));
            #endregion Act

            #region Assert
            ItemRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Item>.Is.Anything));
            Assert.AreEqual(NotificationMessages.STR_NoEditorRights, Controller.Message);
            #endregion Assert
        }
        [TestMethod]
        public void TestLinkToItemSavesIfNotAdminButEditor()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Response
                .Expect(a => a.ApplyAppPathModifier(null)).IgnoreArguments()
                .Return("http://sample.com/ItemManagement/Edit/2").Repeat.Any();
            Controller.Url = MockRepository.GenerateStub<UrlHelper>(Controller.ControllerContext.RequestContext);
            SetupDataForLinkToTests();
            Items[1].AddEditor(Editors[1]);

            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.User });
            #endregion Arrange

            #region Act
            Controller.LinkToItem(2, 2, true, false)
                .AssertHttpRedirect();
            #endregion Act

            #region Assert
            ItemRepository.AssertWasCalled(a => a.EnsurePersistent(Items[1]));
            Assert.AreEqual("Question Set has been created successfully.", Controller.Message);
            #endregion Assert	
        }
        #endregion LinkToItem Post Tests

        #region UnlinkFromItem Post Tests

        /// <summary>
        /// Tests the unlink from item redirects to list if item question set not found.
        /// </summary>
        [TestMethod]
        public void TestUnlinkFromItemRedirectsToListIfItemQuestionSetNotFound()
        {
            #region Arrange
            SetupDataForUnlinkFromTests();
            ItemQuestionSetRepository.Expect(a => a.GetNullableByID(1)).Return(null).Repeat.Any();

            #endregion Arrange

            #region Act
            Controller.UnlinkFromItem(1)
                .AssertActionRedirect()
                .ToAction<ItemManagementController>(a => a.List(null));
            #endregion Act

            #region Assert
            Assert.AreEqual("ItemQuestionSet not found.", Controller.Message);
            ItemQuestionSetRepository.AssertWasNotCalled(a => a.Remove(Arg<ItemQuestionSet>.Is.Anything));
            #endregion Assert		
        }


        /// <summary>
        /// Tests the unlink from item does not save if question set is contact information.
        /// </summary>
        [TestMethod]
        public void TestUnlinkFromItemDoesNotSaveIfQuestionSetIsContactInformation()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Response
               .Expect(a => a.ApplyAppPathModifier(null)).IgnoreArguments()
               .Return("http://sample.com/ItemManagement/Edit/2").Repeat.Any();
            Controller.Url = MockRepository.GenerateStub<UrlHelper>(Controller.ControllerContext.RequestContext);            
            SetupDataForUnlinkFromTests();
            Items[1].QuestionSets.ElementAt(0).QuestionSet.Name = StaticValues.QuestionSet_ContactInformation;
            Items[1].QuestionSets.ElementAt(0).QuestionSet.SetIdTo(1);
            Items[1].QuestionSets.ElementAt(0).QuestionSet.SystemReusable = true;
            ItemQuestionSetRepository.Expect(a => a.GetNullableByID(2)).Return(Items[1].QuestionSets.ElementAt(0)).Repeat.Any();
            QuestionSetRepository.Expect(a => a.GetNullableByID(1))
                .Return(Items[1].QuestionSets.ElementAt(0).QuestionSet).Repeat.Any();
            
            #endregion Arrange

            #region Act
            Controller.UnlinkFromItem(2)
                .AssertHttpRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual("Unable to remove question set.", Controller.Message);
            ItemQuestionSetRepository.AssertWasNotCalled(a => a.Remove(Arg<ItemQuestionSet>.Is.Anything));
            Controller.ModelState.AssertErrorsAre("This is a system default question set and cannot be modified.");
            #endregion Assert		
        }

        /// <summary>
        /// Tests the unlink from item does not save if there is an answer associated with the question.
        /// </summary>
        [TestMethod]
        public void TestUnlinkFromItemDoesNotSaveIfThereIsAnAnswerAssociatedWithTheQuestion()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Response
                          .Expect(a => a.ApplyAppPathModifier(null)).IgnoreArguments()
                          .Return("http://sample.com/ItemManagement/Edit/2").Repeat.Any();
            Controller.Url = MockRepository.GenerateStub<UrlHelper>(Controller.ControllerContext.RequestContext);
            SetupDataForUnlinkFromTests();
            Items[1].QuestionSets.ElementAt(0).QuestionSet.SetIdTo(1);
            TransactionAnswers[1].QuestionSet = QuestionSets[0];
            ItemQuestionSetRepository.Expect(a => a.GetNullableByID(2)).Return(Items[1].QuestionSets.ElementAt(0)).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.UnlinkFromItem(2)
                .AssertHttpRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual("Unable to remove question set.", Controller.Message);
            ItemQuestionSetRepository.AssertWasNotCalled(a => a.Remove(Arg<ItemQuestionSet>.Is.Anything));
            Controller.ModelState.AssertErrorsAre("Someone has already entered a response to this question set and it cannot be deleted.");            
            #endregion Assert		
        }


        [TestMethod]
        public void TestDescriptionUnlinkFromItemDoesNotSaveIfNotAdminOrEditor()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Response
                          .Expect(a => a.ApplyAppPathModifier(null)).IgnoreArguments()
                          .Return("http://sample.com/ItemManagement/Edit/2").Repeat.Any();
            Controller.Url = MockRepository.GenerateStub<UrlHelper>(Controller.ControllerContext.RequestContext);
            SetupDataForUnlinkFromTests();

            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.User });

            Items[1].QuestionSets.ElementAt(0).QuestionSet.SetIdTo(1);

            ItemQuestionSetRepository.Expect(a => a.GetNullableByID(2)).Return(Items[1].QuestionSets.ElementAt(0)).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.UnlinkFromItem(2)
                .AssertActionRedirect()
                .ToAction<ItemManagementController>(a => a.List(null));
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have editor rights to that item.", Controller.Message);
            ItemQuestionSetRepository.AssertWasNotCalled(a => a.Remove(Arg<ItemQuestionSet>.Is.Anything));
            #endregion Assert		
        }

        [TestMethod]
        public void TestDescriptionUnlinkFromItemSavesIfNotAdminButEditor()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Response
                          .Expect(a => a.ApplyAppPathModifier(null)).IgnoreArguments()
                          .Return("http://sample.com/ItemManagement/Edit/2").Repeat.Any();
            Controller.Url = MockRepository.GenerateStub<UrlHelper>(Controller.ControllerContext.RequestContext);
            SetupDataForUnlinkFromTests();

            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.User });

            Items[1].QuestionSets.ElementAt(0).QuestionSet.SetIdTo(1);
            Items[1].AddEditor(Editors[1]);
            var copyOfItemQuestionSet = Items[1].QuestionSets.ElementAt((0));

            ItemQuestionSetRepository.Expect(a => a.GetNullableByID(2)).Return(Items[1].QuestionSets.ElementAt(0)).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.UnlinkFromItem(2)
                .AssertHttpRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual("Question Set has been removed successfully.", Controller.Message);
            ItemQuestionSetRepository.AssertWasCalled(a => a.Remove(copyOfItemQuestionSet));
            #endregion Assert
        }

        [TestMethod]
        public void TestUnlinkFromItemRemovesItemIfNotYetAnswered()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext.Response
                          .Expect(a => a.ApplyAppPathModifier(null)).IgnoreArguments()
                          .Return("http://sample.com/ItemManagement/Edit/2").Repeat.Any();
            Controller.Url = MockRepository.GenerateStub<UrlHelper>(Controller.ControllerContext.RequestContext);
            SetupDataForUnlinkFromTests();
            Items[1].QuestionSets.ElementAt(0).QuestionSet.SetIdTo(1);
            //TransactionAnswers[1].QuestionSet = QuestionSets[0]; //Commented out so it doesn't have an answer
            ItemQuestionSetRepository.Expect(a => a.GetNullableByID(2)).Return(Items[1].QuestionSets.ElementAt(0)).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.UnlinkFromItem(2)
                .AssertHttpRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual("Question Set has been removed successfully.", Controller.Message);
            ItemQuestionSetRepository.AssertWasCalled(a => a.Remove(Arg<ItemQuestionSet>.Is.Anything));
            #endregion Assert		
        }

        #endregion UnlinkFromItem Post Tests
    }
}
