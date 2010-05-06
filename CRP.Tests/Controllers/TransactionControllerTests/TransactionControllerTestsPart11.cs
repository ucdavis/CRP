using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CRP.Controllers;
using CRP.Controllers.Helpers;
using CRP.Controllers.ViewModels;
using CRP.Core.Abstractions;
using CRP.Core.Domain;
using CRP.Core.Helpers;
using CRP.Core.Resources;
using CRP.Tests.Core.Extensions;
using CRP.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Rhino.Mocks;
using UCDArch.Testing;

namespace CRP.Tests.Controllers.TransactionControllerTests
{
    /// <summary>
    /// Transaction Controller Tests 
    /// </summary>
    public partial class TransactionControllerTests
    {
        #region PaymentResult Tests

        [TestMethod]
        public void TestPaymentResultDoesNothingIfTransactionIdIsEmpty()
        {
            #region Arrange
            var parameters = new PaymentResultParameters();
            parameters.EXT_TRANS_ID = null;            
            #endregion Arrange

            #region Act
            var result = Controller.PaymentResult(parameters)
                .AssertViewRendered();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            #endregion Assert		
        }


        [TestMethod]
        public void TestPaymentResultRedirectsToViewIfTransactionIdNotFoundAndCallsNotificationProvider()
        {
            #region Arrange
            var parameters = new PaymentResultParameters();
            parameters.EXT_TRANS_ID = Guid.Empty.ToString();
            TransactionRepository.Expect(a => a.Queryable).Return(Transactions.AsQueryable()).Repeat.Any();
            #endregion Arrange

            #region Act
            var result = Controller.PaymentResult(parameters)
                .AssertViewRendered();
            #endregion Act

            #region Assert
            NotificationProvider.AssertWasCalled(a => a.SendPaymentResultErrors(Arg<String>.Is.Anything, Arg<PaymentResultParameters>.Is.Anything, Arg<System.Collections.Specialized.NameValueCollection>.Is.Anything, Arg<string>.Is.Anything, Arg<PaymentResultType>.Is.Anything));
            #endregion Assert		
        }

        #endregion PaymentResult Tests
    }
}
