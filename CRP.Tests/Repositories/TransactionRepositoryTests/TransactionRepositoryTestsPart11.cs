using System;
using System.Collections.Generic;
using System.Linq;
using CRP.Core.Abstractions;
using CRP.Core.Domain;
using CRP.Tests.Core;
using CRP.Tests.Core.Extensions;
using CRP.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Data.NHibernate;
using UCDArch.Testing.Extensions;

namespace CRP.Tests.Repositories.TransactionRepositoryTests
{
    public partial class TransactionRepositoryTests
    {
        #region TotalPaidByCheck Tests
        /// <summary>
        /// Tests the total paid by check returns expected result.
        /// </summary>
        [TestMethod]
        public void TestTotalPaidByCheckReturnsExpectedResult()
        {
            #region Arrange
            var transaction = GetValid(9);
            transaction.Amount = 3m;
            var paymentLogs = new List<PaymentLog>();
            for (int i = 0; i < 5; i++)
            {
                paymentLogs.Add(CreateValidEntities.PaymentLog(i + 1));
                paymentLogs[i].Accepted = true;
                paymentLogs[i].Check = false;
                paymentLogs[i].Credit = true;
            }

            paymentLogs[0].Amount = 1;
            paymentLogs[1].Amount = 10;
            paymentLogs[2].Amount = 100;
            paymentLogs[3].Amount = 1000;
            paymentLogs[4].Amount = 10000;

            paymentLogs[2].Check = true;
            paymentLogs[2].Credit = false;
            paymentLogs[3].Check = true;
            paymentLogs[3].Credit = false;
            paymentLogs[3].Accepted = false;
            paymentLogs[4].Check = true;
            paymentLogs[4].Credit = false;

            transaction.AddPaymentLog(paymentLogs[0]);
            transaction.AddPaymentLog(paymentLogs[1]);
            transaction.AddPaymentLog(paymentLogs[2]);
            transaction.AddPaymentLog(paymentLogs[3]);
            transaction.AddPaymentLog(paymentLogs[4]);
            #endregion Arrange

            #region Act
            var result = transaction.TotalPaidByCheck;
            #endregion Act

            #region Assert
            Assert.AreEqual(10100, result);
            #endregion Assert
        }
        #endregion TotalPaidByCheck Tests
        #region TotalPaidByCredit Tests
        /// <summary>
        /// Tests the total paid by check returns expected result.
        /// </summary>
        [TestMethod]
        public void TestTotalPaidByCreditReturnsExpectedResult()
        {
            #region Arrange
            var transaction = GetValid(9);
            transaction.Amount = 3m;
            var paymentLogs = new List<PaymentLog>();
            for (int i = 0; i < 5; i++)
            {
                paymentLogs.Add(CreateValidEntities.PaymentLog(i + 1));
                paymentLogs[i].Accepted = true;
                paymentLogs[i].Credit = false;
                paymentLogs[i].Check = true;
            }

            paymentLogs[0].Amount = 1;
            paymentLogs[1].Amount = 10;
            paymentLogs[2].Amount = 100;
            paymentLogs[3].Amount = 1000;
            paymentLogs[4].Amount = 10000;

            paymentLogs[2].Credit = true;
            paymentLogs[2].Check = false;
            paymentLogs[3].Credit = true;
            paymentLogs[3].Check = false;
            paymentLogs[3].Accepted = false;
            paymentLogs[4].Credit = true;
            paymentLogs[4].Check = false;

            transaction.AddPaymentLog(paymentLogs[0]);
            transaction.AddPaymentLog(paymentLogs[1]);
            transaction.AddPaymentLog(paymentLogs[2]);
            transaction.AddPaymentLog(paymentLogs[3]);
            transaction.AddPaymentLog(paymentLogs[4]);
            #endregion Arrange

            #region Act
            var result = transaction.TotalPaidByCredit;
            #endregion Act

            #region Assert
            Assert.AreEqual(10100, result);
            #endregion Assert
        }
        #endregion TotalPaidByCredit Tests


    }
}
