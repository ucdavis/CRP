using System.Linq;
using CRP.Controllers;
using CRP.Controllers.Helpers;
using CRP.Controllers.ViewModels;
using CRP.Core.Domain;
using CRP.Core.Resources;
using CRP.Tests.Core.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Rhino.Mocks;

namespace CRP.Tests.Controllers.TransactionControllerTests
{
    /// <summary>
    /// Transaction Controller Tests 
    /// </summary>
    public partial class TransactionControllerTests
    {
        #region Checkout Post Tests (Part 1)

        /// <summary>
        /// Tests the checkout post redirects to home controller if item not found.
        /// </summary>
        [TestMethod]
        public void TestCheckoutPostRedirectsToHomeControllerIfItemNotFound()
        {
            #region Arrange
            SetupDataForTests();
            SetupDataForPopulateItemTransactionAnswer();
            #endregion Arrange

            #region Act
            Controller.Checkout(1, 1, null, "Check", string.Empty, string.Empty, null, null, true)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            Assert.AreEqual("Item not found.", Controller.Message);
            #endregion Assert		
        }


        /// <summary>
        /// Tests the checkout post redirects to home controller if item is not available1.
        /// </summary>
        [TestMethod]
        public void TestCheckoutPostRedirectsToHomeControllerIfItemIsNotAvailable1()
        {
            #region Arrange            
            SetupDataForCheckoutTests();
            Items[1].Quantity = 10;
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.User });
            #endregion Arrange

            #region Act
            Controller.Checkout(2, 1, null, "Check", string.Empty, string.Empty, TransactionAnswerParameters, null, true)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            Assert.AreEqual("Item is not available.", Controller.Message);
            #endregion Assert		
        }

        /// <summary>
        /// Tests the checkout post redirects to home controller if item is not available2.
        /// </summary>
        [TestMethod]
        public void TestCheckoutPostRedirectsToHomeControllerIfItemIsNotAvailable2()
        {
            #region Arrange
            var fakeDate = SetupDataForCheckoutTests();
            Items[1].Expiration = fakeDate.AddDays(-1);
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.User });
            #endregion Arrange

            #region Act
            Controller.Checkout(2, 1, null, "Check", string.Empty, string.Empty, TransactionAnswerParameters, null, true)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            Assert.AreEqual("Item is not available.", Controller.Message);
            #endregion Assert
        }

        /// <summary>
        /// Tests the checkout post redirects to home controller if item is not available3.
        /// </summary>
        [TestMethod]
        public void TestCheckoutPostRedirectsToHomeControllerIfItemIsNotAvailable3()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            Items[1].Available = false;
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.User });
            #endregion Arrange

            #region Act
            Controller.Checkout(2, 1, null, "Check", string.Empty, string.Empty, TransactionAnswerParameters, null, true)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            Assert.AreEqual("Item is not available.", Controller.Message);
            #endregion Assert
        }
        /// <summary>
        /// Tests the checkout post does not redirect to home controller if item is not available but admin.
        /// </summary>
        [TestMethod]
        public void TestCheckoutPostDoesNotRedirectToHomeControllerIfItemIsNotAvailableButAdmin4()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            Items[1].Available = false;
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Admin });
            #endregion Arrange

            #region Act
            Controller.Checkout(2, 1, null, "Check", string.Empty, string.Empty, TransactionAnswerParameters, null, true)
                .AssertActionRedirect()
                .ToAction<TransactionController>(a => a.Confirmation(1));
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            #endregion Assert
        }
        /// <summary>
        /// Tests the checkout post does not redirect to home controller if item is not available but admin.
        /// </summary>
        [TestMethod]
        public void TestCheckoutPostDoesNotRedirectToHomeControllerIfItemIsNotAvailableButAdmin5()
        {
            #region Arrange
            var fakeDate = SetupDataForCheckoutTests();
            Items[1].Expiration = fakeDate.AddDays(-1);
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Admin });
            #endregion Arrange

            #region Act
            Controller.Checkout(2, 1, null, "Check", string.Empty, string.Empty, TransactionAnswerParameters, null, true)
                .AssertActionRedirect()
                .ToAction<TransactionController>(a => a.Confirmation(1));
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            #endregion Assert
        }
        /// <summary>
        /// Tests the checkout post does not redirect to home controller if item is not available but admin.
        /// </summary>
        [TestMethod]
        public void TestCheckoutPostDoesNotRedirectToHomeControllerIfItemIsNotAvailableButAdmin6()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            Items[1].Quantity = 10;
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Admin });
            #endregion Arrange

            #region Act
            Controller.Checkout(2, 1, null, "Check", string.Empty, string.Empty, TransactionAnswerParameters, null, true)
                .AssertActionRedirect()
                .ToAction<TransactionController>(a => a.Confirmation(1));
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            #endregion Assert
        } 

        /// <summary>
        /// Tests the checkout with minimal valid data saves.
        /// </summary>
        [TestMethod]
        public void TestCheckoutWithValidDataSaves()
        {
            #region Arrange
            SetupDataForCheckoutTests();            
            #endregion Arrange

            #region Act
            Controller.Checkout(2, 1, null, "Check", string.Empty, string.Empty, TransactionAnswerParameters, null, true)
                .AssertActionRedirect()
                .ToAction<TransactionController>(a => a.Confirmation(1));
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            #endregion Assert		
        }

        /// <summary>
        /// Tests the checkout with false recaptch does not save.
        /// </summary>
        [TestMethod]
        public void TestCheckoutWithFalseRecaptchDoesNotSave()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            #endregion Arrange

            #region Act
            Controller.Checkout(2, 1, null, "Check", string.Empty, string.Empty, TransactionAnswerParameters, null, false)
                .AssertViewRendered()
                .WithViewData<ItemDetailViewModel>();
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("Captcha values are not valid.");
            #endregion Assert
        }
        /// <summary>
        /// Tests the checkout with donation creates child transaction.
        /// </summary>
        [TestMethod]
        public void TestCheckoutWithDonationCreatesChildTransaction()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            #endregion Arrange

            #region Act
            Controller.Checkout(2, 1, 25.01m, "Check", string.Empty, string.Empty, TransactionAnswerParameters, null, true)
                .AssertActionRedirect()
                .ToAction<TransactionController>(a => a.Confirmation(1));
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            var args = (Transaction)TransactionRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything))[0][0];
            Assert.IsNotNull(args);
            Assert.AreEqual(20.00m, args.Amount);
            Assert.AreEqual(1, args.ChildTransactions.Count);
            Assert.AreEqual(25.01m, args.ChildTransactions.ElementAt(0).Amount);
            Assert.IsTrue(args.ChildTransactions.ElementAt(0).Donation);
            #endregion Assert
        }

        /// <summary>
        /// Tests the checkout with valid data and credit card saves.
        /// </summary>
        [TestMethod]
        public void TestCheckoutWithValidDataAndCreditCardSaves()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            #endregion Arrange

            #region Act
            Controller.Checkout(2, 1, null, StaticValues.CreditCard, string.Empty, string.Empty, TransactionAnswerParameters, null, true)
                .AssertActionRedirect()
                .ToAction<TransactionController>(a => a.Confirmation(1));
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            #endregion Assert
        }

        [TestMethod]
        public void TestCheckoutWithCheckOrCreditNotSpecifiedDoesNotSave()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            #endregion Arrange

            #region Act
            Controller.Checkout(2, 1, null, string.Empty, string.Empty, string.Empty, TransactionAnswerParameters, null, true)
                .AssertViewRendered()
                .WithViewData<ItemDetailViewModel>();
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("PaymentType: Payment type was not selected.");
            #endregion Assert
        }
        #endregion Checkout Post Tests (Part 1)

        
    }
}
