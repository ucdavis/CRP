using CRP.Controllers;
using CRP.Core.Domain;
using CRP.Core.Helpers;
using CRP.Tests.Core.Extensions;
using CRP.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;

namespace CRP.Tests.Controllers.TransactionControllerTests
{
    /// <summary>
    /// Transaction Controller Tests 
    /// </summary>
    public partial class TransactionControllerTests
    {
        #region Route Tests


        /// <summary>
        /// Tests the checkout get mapping.
        /// </summary>
        [TestMethod]
        public void TestCheckoutGetMapping()
        {
            "~/Transaction/Checkout/5".ShouldMapTo<TransactionController>(a => a.Checkout(5));
        }

        /// <summary>
        /// Tests the checkout post mapping.
        /// </summary>
        [TestMethod]
        public void TestCheckoutPostMapping()
        {
            "~/Transaction/Checkout/5".ShouldMapTo<TransactionController>(a => a.Checkout(5, 1, null, null, "test", string.Empty, string.Empty, new QuestionAnswerParameter[1], new QuestionAnswerParameter[1], true), true);
        }

        /// <summary>
        /// Tests the confirmation get mapping.
        /// </summary>
        [TestMethod]
        public void TestConfirmationGetMapping()
        {
            "~/Transaction/Confirmation/5".ShouldMapTo<TransactionController>(a => a.Confirmation(5));
        }

        /// <summary>
        /// Tests the payment success mapping.
        /// </summary>
        [TestMethod]
        public void TestPaymentSuccessMapping()
        {
            "~/Transaction/PaymentSuccess/5".ShouldMapTo<TransactionController>(a => a.PaymentSuccess("12", "12"), true);
        }

        /// <summary>
        /// Tests the payment error mapping.
        /// </summary>
        [TestMethod]
        public void TestPaymentErrorMapping()
        {
            "~/Transaction/PaymentError/5".ShouldMapTo<TransactionController>(a => a.PaymentError("12", "12"), true);
        }

        /// <summary>
        /// Tests the payment cancel mapping.
        /// </summary>
        [TestMethod]
        public void TestPaymentCancelMapping()
        {
            "~/Transaction/PaymentCancel/5".ShouldMapTo<TransactionController>(a => a.PaymentCancel("12", "12"), true);
        }

        /// <summary>
        /// Tests the edit get mapping.
        /// </summary>
        [TestMethod]
        public void TestEditGetMapping()
        {
            "~/Transaction/Edit/5".ShouldMapTo<TransactionController>(a => a.Edit(5, null, null));
        }

        /// <summary>
        /// Tests the edit post mapping.
        /// </summary>
        [TestMethod]
        public void TestEditPostMapping()
        {
            "~/Transaction/Edit/".ShouldMapTo<TransactionController>(a => a.Edit(new Transaction(), null, null), true);
        }

        /// <summary>
        /// Tests the payment result mapping.
        /// </summary>
        [TestMethod]
        public void TestPaymentResultMapping()
        {
            "~/Transaction/PaymentResult/".ShouldMapTo<TransactionController>(a => a.PaymentResult(new PaymentResultParameters()), true);
        }

        /// <summary>
        /// Tests the lookup get mapping.
        /// </summary>
        [TestMethod]
        public void TestLookupGetMapping()
        {
            "~/Transaction/Lookup".ShouldMapTo<TransactionController>(a => a.Lookup());
        }

        /// <summary>
        /// Tests the lookup post mapping.
        /// </summary>
        [TestMethod]
        public void TestLookupPostMapping()
        {
            "~/Transaction/Lookup/test".ShouldMapTo<TransactionController>(a => a.Lookup("order", "email"), true);
        }
        #endregion Route Tests

        #region Misc Tests


        /// <summary>
        /// Tests the total to string format.
        /// Used for the MD5 hash for touchNet.
        /// </summary>
        [TestMethod]
        public void TestTotalToStringFormat()
        {
            #region Arrange
            ControllerRecordFakes.FakeTransactions(Transactions, 1);
            Transactions[0].Amount = 123456789.12m;
            #endregion Arrange

            #region Act
            var result = Transactions[0].Total.ToString();
            #endregion Act

            #region Assert
            Assert.AreEqual("123456789.12", result);
            #endregion Assert
        }


        [TestMethod]
        public void TestMd5Calculation()
        {
            #region Arrange
            ControllerRecordFakes.FakeTransactions(Transactions, 1);
            Transactions[0].Amount = 12.35m;
            const string postingKey = "FB8E61EF5F63028C";
            const string transactionId = "A234";
            #endregion Arrange

            #region Act
            var result = TransactionController.CalculateValidationString(
                postingKey,
                transactionId,
                Transactions[0].Total.ToString());
            #endregion Act

            #region Assert
            Assert.AreEqual("hAGcy7esDK7joiFIPJQKRA==", result);
            #endregion Assert
        }

        #endregion Misc Tests
    }
}
