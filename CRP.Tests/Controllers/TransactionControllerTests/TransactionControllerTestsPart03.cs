using System.Collections.Generic;
using System.Linq;
using CRP.Controllers;
using CRP.Controllers.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;

namespace CRP.Tests.Controllers.TransactionControllerTests
{
    /// <summary>
    /// Transaction Controller Tests 
    /// </summary>
    public partial class TransactionControllerTests
    {
        #region Checkout Get Tests

        /// <summary>
        /// Tests the checkout get redirects to home controller if item is not found.
        /// </summary>
        [TestMethod]
        public void TestCheckoutGetRedirectsToHomeControllerIfItemIsNotFound()
        {
            #region Arrange
            SetupDataForTests();
            #endregion Arrange

            #region Act
            Controller.Checkout(1, null, null, null)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Item not found.", Controller.Message);
            #endregion Assert
        }

        /// <summary>
        /// Tests the checkout get returns view when item is found.
        /// </summary>
        [TestMethod]
        public void TestCheckoutGetReturnsViewWhenItemIsFound()
        {
            #region Arrange
            SetupDataForTests();
            #endregion Arrange

            #region Act
            var result = Controller.Checkout(2, null, null)
                .AssertViewRendered()
                .WithViewData<ItemDetailViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            #endregion Assert
        }


        /// <summary>
        /// Tests the checkout get will populate contact information if open id found.
        /// </summary>
        [TestMethod]
        public void TestCheckoutGetWillPopulateContactInformationIfOpenIdFound()
        {
            #region Arrange
            SetupDataForTests();
            SetupDataForPopulateItemTransactionAnswer();
            #endregion Arrange

            #region Act
            var result = Controller.Checkout(2, null, null)
                .AssertViewRendered()
                .WithViewData<ItemDetailViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(9, result.Answers.Count());
            Assert.AreEqual("FirstName", result.Answers.ElementAt(0).Answer);
            Assert.AreEqual("LastName", result.Answers.ElementAt(1).Answer);
            Assert.AreEqual("Address1", result.Answers.ElementAt(2).Answer);
            Assert.AreEqual("Address2", result.Answers.ElementAt(3).Answer);
            Assert.AreEqual("City", result.Answers.ElementAt(4).Answer);
            Assert.AreEqual("CA", result.Answers.ElementAt(5).Answer);
            Assert.AreEqual("95616", result.Answers.ElementAt(6).Answer);
            Assert.AreEqual("555 555 5555", result.Answers.ElementAt(7).Answer);
            Assert.AreEqual("Email@maily.org", result.Answers.ElementAt(8).Answer);
            #endregion Assert
        }

        /// <summary>
        /// Tests the checkout get will throw exception when populate contact information if A different question name is found.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void TestCheckoutGetWillThrowExceptionWhenPopulateContactInformationIfADifferentQuestionNameIsFound()
        {
            #region Arrange
            SetupDataForTests();
            SetupDataForPopulateItemTransactionAnswer();
            Questions[1].Name = "NotValid"; //This is not a contact Information Question. It should not be in the data. 
            #endregion Arrange

            #region Act
            Controller.Checkout(2, null, null);
            #endregion Act
        }

        /// <summary>
        /// Tests the checkout get will default quantity to one.
        /// </summary>
        [TestMethod]
        public void TestCheckoutGetWillDefaultQuantityToOne()
        {
            #region Arrange
            SetupDataForTests();
            #endregion Arrange

            #region Act
            var result = Controller.Checkout(2, null, null)
                .AssertViewRendered()
                .WithViewData<ItemDetailViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Quantity);
            #endregion Assert
        }


        #endregion Checkout Get Tests
    }
}
