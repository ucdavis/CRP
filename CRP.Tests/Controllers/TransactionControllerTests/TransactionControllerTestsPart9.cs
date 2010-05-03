using System.Linq;
using CRP.Controllers;
using CRP.Controllers.ViewModels;
using CRP.Core.Domain;
using CRP.Core.Resources;
using CRP.Tests.Core.Extensions;
using CRP.Tests.Core.Helpers;
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
        #region Checkout Post Tests (Part 6)


        #region Checkout Quantity Answers Tests

        /// <summary>
        /// Tests the checkout quantity answers.
        /// </summary>
        [TestMethod]
        public void TestCheckoutQuantityAnswers()
        {
            #region Arrange
            SetupDataForCheckoutTests();
            //SetupDataForAllContactInformationTransactionAnswerParameters();
            SetupDataForQuantityQuestionsAnswerParameters();
            #endregion Arrange

            #region Act
            Controller.Checkout(2, 3, null, (Items[1].CostPerItem * 3), StaticValues.CreditCard, string.Empty, null, TransactionAnswerParameters, QuantityAnswerParameters, true)
                .AssertActionRedirect()
                .ToAction<TransactionController>(a => a.Confirmation(1));
            #endregion Act

            #region Assert
            TransactionRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            var args = (Transaction)TransactionRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything))[0][0];
            Assert.IsNotNull(args);
            //One because we have a TransactionAnswer here
            Assert.AreEqual(1, args.TransactionAnswers.Count);
            Assert.AreEqual(9, args.QuantityAnswers.Count);
            #endregion Assert
        }

        #endregion Checkout Quantity Answers Tests

        //test quantity answers
        //test donation creates another transaction
        //test restricted key
        //test inventory exists

        #endregion Checkout Post Tests (Part 6)
    }
}
