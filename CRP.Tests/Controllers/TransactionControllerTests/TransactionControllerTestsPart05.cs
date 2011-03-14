using System;
using CRP.Controllers;
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
        #region Checkout Post Tests (Part 2)
        #region Coupon Tests
        /// <summary>
        /// Tests the checkout with valid data and coupon saves.
        /// </summary>
        [TestMethod]
        public void TestCheckoutWithValidDataAndCouponSaves()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            Items[1].CostPerItem = 20m;
            #endregion Arrange

            #region Act
            Controller.Checkout(2, 3, null, 49.98m, StaticValues.CreditCard, string.Empty, "COUPON", TransactionAnswerParameters, null, true)
                .AssertActionRedirect()
                .ToAction<TransactionController>(a => a.Confirmation(1));
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            var args = (Transaction)TransactionRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything))[0][0];
            Assert.IsNotNull(args);
            Assert.AreEqual(49.98m, args.Amount);
            Assert.AreEqual("COUPON", args.Coupon.Code);
            #endregion Assert
        }


        /// <summary>
        /// Tests the coupon is not active so not used.
        /// </summary>
        [TestMethod]
        public void TestCouponIsNotActiveSoNotUsed()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            Items[1].CostPerItem = 20m;
            Coupons[1].IsActive = false;
            #endregion Arrange

            #region Act
            Controller.Checkout(2, 3, null, (Items[1].CostPerItem * 3), StaticValues.CreditCard, string.Empty, "COUPON", TransactionAnswerParameters, null, true)
                .AssertActionRedirect()
                .ToAction<TransactionController>(a => a.Confirmation(1));
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            var args = (Transaction)TransactionRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything))[0][0];
            Assert.IsNotNull(args);
            Assert.AreEqual(60.00m, args.Amount);
            #endregion Assert
        }

        /// <summary>
        /// Tests the coupon is not found so not used.
        /// </summary>
        [TestMethod]
        public void TestCouponIsNotFoundSoNotUsed()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            Items[1].CostPerItem = 20m;
            Coupons[1].Code = "COUPONNot";
            #endregion Arrange

            #region Act
            Controller.Checkout(2, 3, null, (Items[1].CostPerItem * 3), StaticValues.CreditCard, string.Empty, "COUPON", TransactionAnswerParameters, null, true)
                .AssertActionRedirect()
                .ToAction<TransactionController>(a => a.Confirmation(1));
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            var args = (Transaction)TransactionRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything))[0][0];
            Assert.IsNotNull(args);
            Assert.AreEqual(60.00m, args.Amount);
            #endregion Assert
        }

        /// <summary>
        /// Tests the coupon is for A different item so not used.
        /// </summary>
        [TestMethod]
        public void TestCouponIsForADifferentItemSoNotUsed()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            Items[1].CostPerItem = 20m;
            Coupons[1].Item = Items[2];
            #endregion Arrange

            #region Act
            Controller.Checkout(2, 3, null, (Items[1].CostPerItem * 3), StaticValues.CreditCard, string.Empty, "COUPON", TransactionAnswerParameters, null, true)
                .AssertActionRedirect()
                .ToAction<TransactionController>(a => a.Confirmation(1));
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            var args = (Transaction)TransactionRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything))[0][0];
            Assert.IsNotNull(args);
            Assert.AreEqual(60.00m, args.Amount);
            #endregion Assert
        }

        /// <summary>
        /// Tests the checkout with valid data and coupon with email saves.
        /// </summary>
        [TestMethod]
        public void TestCheckoutWithValidDataAndCouponWithEmailSaves()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            Items[1].CostPerItem = 20m;
            Coupons[1].Email = "bob@TEST.com";
            #endregion Arrange

            #region Act
            Controller.Checkout(2, 3, null, 49.98m, StaticValues.CreditCard, string.Empty, "COUPON", TransactionAnswerParameters, null, true)
                .AssertActionRedirect()
                .ToAction<TransactionController>(a => a.Confirmation(1));
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            var args = (Transaction)TransactionRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything))[0][0];
            Assert.IsNotNull(args);
            Assert.AreEqual(49.98m, args.Amount);
            #endregion Assert
        }

        /// <summary>
        /// Tests the checkout with valid data and coupon with different email does not save.
        /// </summary>
        [TestMethod]
        public void TestCheckoutWithValidDataAndCouponWithDifferentEmailDoesNotSave()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            Items[1].CostPerItem = 20m;
            Coupons[1].Email = "NotFound@TEST.com";
            #endregion Arrange

            #region Act
            Controller.Checkout(2, 3, null, (Items[1].CostPerItem * 3), StaticValues.CreditCard, string.Empty, "COUPON", TransactionAnswerParameters, null, true)
                .AssertViewRendered()
                .WithViewData<ItemDetailViewModel>();
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("Coupon could not be used.");
            #endregion Assert
        }

        /// <summary>
        /// Tests the checkout used coupon does not save.
        /// </summary>
        [TestMethod]
        public void TestCheckoutUsedCouponDoesNotSave()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            Items[1].CostPerItem = 20m;
            //Coupons[1].Unlimited = false; //Not unlimited
            //Coupons[1].Used = true; //And used
            #endregion Arrange

            #region Act
            Controller.Checkout(2, 3, null, (Items[1].CostPerItem * 3), StaticValues.CreditCard, string.Empty, "COUPON", TransactionAnswerParameters, null, true)
                .AssertViewRendered()
                .WithViewData<ItemDetailViewModel>();
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("Coupon could not be used.");
            #endregion Assert
        }

        /// <summary>
        /// Tests the checkout expired coupon does not save.
        /// </summary>
        [TestMethod]
        public void TestCheckoutExpiredCouponDoesNotSave()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            Items[1].CostPerItem = 20m;
            Coupons[1].Expiration = DateTime.Now;
            #endregion Arrange

            #region Act
            Controller.Checkout(2, 3, null, (Items[1].CostPerItem * 3), StaticValues.CreditCard, string.Empty, "COUPON", TransactionAnswerParameters, null, true)
                .AssertViewRendered()
                .WithViewData<ItemDetailViewModel>();
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("Coupon could not be used.");
            #endregion Assert
        }

        /// <summary>
        /// Tests the checkout not expired coupon saves.
        /// </summary>
        [TestMethod]
        public void TestCheckoutNotExpiredCouponSaves()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            Items[1].CostPerItem = 20m;
            Coupons[1].Expiration = DateTime.Now.AddDays(1);
            #endregion Arrange

            #region Act
            Controller.Checkout(2, 3, null, 49.98m, StaticValues.CreditCard, string.Empty, "COUPON", TransactionAnswerParameters, null, true)
                .AssertActionRedirect()
                .ToAction<TransactionController>(a => a.Confirmation(1));
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            var args = (Transaction)TransactionRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything))[0][0];
            Assert.IsNotNull(args);
            Assert.AreEqual(49.98m, args.Amount);
            #endregion Assert
        }


        /// <summary>
        /// Tests the checkout with no email coupon does not save.
        /// </summary>
        [TestMethod]
        public void TestCheckoutWithNoEmailCouponDoesNotSave()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            Items[1].CostPerItem = 20m;
            Coupons[1].Email = "bob@test.com";
            TransactionAnswerParameters[0].QuestionId = 99;
            #endregion Arrange

            #region Act
            Controller.Checkout(2, 3, null, (Items[1].CostPerItem * 3), StaticValues.CreditCard, string.Empty, "COUPON", TransactionAnswerParameters, null, true)
                .AssertViewRendered()
                .WithViewData<ItemDetailViewModel>();
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("Coupon could not be used.");
            #endregion Assert
        }

        /// <summary>
        /// Tests the checkout with no email saves if coupon has not email.
        /// </summary>
        [TestMethod]
        public void TestCheckoutWithNoEmailSavesIfCouponHasNotEmail()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            Items[1].CostPerItem = 20m;
            Coupons[1].Email = string.Empty;
            TransactionAnswerParameters[0].QuestionId = 99;
            #endregion Arrange

            #region Act
            Controller.Checkout(2, 3, null, 49.98m, StaticValues.CreditCard, string.Empty, "COUPON", TransactionAnswerParameters, null, true)
                .AssertActionRedirect()
                .ToAction<TransactionController>(a => a.Confirmation(1));
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            var args = (Transaction)TransactionRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything))[0][0];
            Assert.IsNotNull(args);
            Assert.AreEqual(49.98m, args.Amount);
            #endregion Assert
        }

        /// <summary>
        /// Tests the checkout coupon used flag saves.
        /// </summary>
        [TestMethod]
        public void TestCheckoutCouponUsedFlagSaves()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            Items[1].CostPerItem = 20m;
            Coupons[1].Email = "bob@TEST.com";
            //Coupons[1].Unlimited = false;
            //Coupons[1].Used = false;
            #endregion Arrange

            #region Act
            Controller.Checkout(2, 3, null, 49.98m, StaticValues.CreditCard, string.Empty, "COUPON", TransactionAnswerParameters, null, true)
                .AssertActionRedirect()
                .ToAction<TransactionController>(a => a.Confirmation(1));
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            var args = (Transaction)TransactionRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything))[0][0];
            Assert.IsNotNull(args);
            Assert.AreEqual(49.98m, args.Amount);
            //Assert.IsTrue(Coupons[1].Used);
            #endregion Assert
        }
        #endregion Coupon Tests
        #endregion Checkout Post Tests (Part 2)


    }
}
