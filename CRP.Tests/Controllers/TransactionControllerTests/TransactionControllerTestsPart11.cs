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
        public void TestPaymentResultRedirectsToViewIfTransactionGuidNotFoundAndCallsNotificationProvider()
        {
            #region Arrange
            var parameters = new PaymentResultParameters();
            parameters.EXT_TRANS_ID = Guid.Empty.ToString();
            TransactionRepository.Expect(a => a.Queryable).Return(Transactions.AsQueryable()).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.PaymentResult(parameters)
                .AssertViewRendered();
            #endregion Act

            #region Assert
            NotificationProvider.AssertWasCalled(a => a.SendPaymentResultErrors(Arg<String>.Is.Anything, Arg<PaymentResultParameters>.Is.Anything, Arg<System.Collections.Specialized.NameValueCollection>.Is.Anything, Arg<string>.Is.Anything, Arg<PaymentResultType>.Is.Anything));
            var args = NotificationProvider.GetArgumentsForCallsMadeOn(a => a.SendPaymentResultErrors(Arg<String>.Is.Anything, Arg<PaymentResultParameters>.Is.Anything, Arg<System.Collections.Specialized.NameValueCollection>.Is.Anything, Arg<string>.Is.Anything, Arg<PaymentResultType>.Is.Anything))[0];
            Assert.AreEqual("TransactionNotFound", args[4].ToString());
            #endregion Assert		
        }


        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestPaymentResultThrowsExceptionIfDuplicateGuidRequested()
        {          
            try
            {
                #region Arrange
                ControllerRecordFakes.FakeTransactions(Transactions, 3);
                Transactions[2].TransactionGuid = Transactions[0].TransactionGuid;
                var parameters = new PaymentResultParameters();
                parameters.EXT_TRANS_ID = Transactions[0].TransactionGuid.ToString();
                TransactionRepository.Expect(a => a.Queryable).Return(Transactions.AsQueryable()).Repeat.Any();
                #endregion Arrange

                #region Act
                Controller.PaymentResult(parameters)
                    .AssertViewRendered();
                #endregion Act
            }
            catch (Exception ex)
            {
                #region Assert
                Assert.IsNotNull(ex);
                Assert.AreEqual("Sequence contains more than one element", ex.Message);
                throw;
                #endregion Assert
            }		
        }


        [TestMethod]
        public void TestPaymentResultWithValidPaymentCreatesPaymentLog()
        {
            #region Arrange
            ControllerRecordFakes.FakeTransactions(Transactions, 3);
            
            AssignContactEmail(Transactions[1]);

            Transactions[1].Amount = 200.00m;
            Transactions[1].IsActive = false; // Will be changed
            var parameters = DefaultParameters();
            
            parameters.EXT_TRANS_ID = Transactions[1].TransactionGuid.ToString();
            parameters.PMT_AMT = Transactions[1].Total;
            TransactionRepository.Expect(a => a.Queryable).Return(Transactions.AsQueryable()).Repeat.Any();            
            #endregion Arrange

            #region Act

            Controller.PaymentResult(parameters)
                .AssertViewRendered();
            #endregion Act

            #region Assert
            NotificationProvider.AssertWasNotCalled(a => a.SendPaymentResultErrors(Arg<String>.Is.Anything, Arg<PaymentResultParameters>.Is.Anything, Arg<System.Collections.Specialized.NameValueCollection>.Is.Anything, Arg<string>.Is.Anything, Arg<PaymentResultType>.Is.Anything));
            TransactionRepository.AssertWasCalled(a => a.EnsurePersistent(Transactions[1]));
            NotificationProvider.AssertWasCalled(a => a.SendConfirmation(Controller.Repository, Transactions[1], TransactionAnswers[0].Answer));

            var args = (Transaction)TransactionRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Transactions[1]))[0][0];
            Assert.IsTrue(args.IsActive);
            Assert.AreEqual(1, args.PaymentLogs.Count);
            var arg_PaymentLog = args.PaymentLogs.ElementAt(0);
            Assert.IsTrue(arg_PaymentLog.Credit);
            Assert.IsFalse(arg_PaymentLog.Check);
            Assert.IsTrue(arg_PaymentLog.Accepted);
            Assert.AreEqual(parameters.NAME_ON_ACCT, arg_PaymentLog.Name);
            Assert.AreEqual(parameters.PMT_AMT.Value, arg_PaymentLog.Amount);
            Assert.AreEqual(parameters.TPG_TRANS_ID, arg_PaymentLog.GatewayTransactionId);
            Assert.AreEqual(parameters.CARD_TYPE, arg_PaymentLog.CardType);
            Assert.AreEqual(parameters.acct_addr, arg_PaymentLog.TnBillingAddress1);
            Assert.AreEqual(parameters.acct_addr2, arg_PaymentLog.TnBillingAddress2);
            Assert.AreEqual(parameters.acct_city, arg_PaymentLog.TnBillingCity);
            Assert.AreEqual(parameters.acct_state, arg_PaymentLog.TnBillingState);
            Assert.AreEqual(parameters.acct_zip, arg_PaymentLog.TnBillingZip);
            Assert.AreEqual(parameters.CANCEL_LINK, arg_PaymentLog.TnCancelLink);
            Assert.AreEqual(parameters.ERROR_LINK, arg_PaymentLog.TnErrorLink);
            Assert.AreEqual(parameters.pmt_date, arg_PaymentLog.TnPaymentDate);
            Assert.AreEqual(parameters.Submit, arg_PaymentLog.TnSubmit);
            Assert.AreEqual(parameters.SUCCESS_LINK, arg_PaymentLog.TnSuccessLink);
            Assert.AreEqual(parameters.sys_tracking_id, arg_PaymentLog.TnSysTrackingId);
            Assert.AreEqual(parameters.UPAY_SITE_ID, arg_PaymentLog.TnUpaySiteId);
            Assert.AreEqual("S", arg_PaymentLog.TnStatus);            
            #endregion Assert		
        }

        [TestMethod]
        public void TestPaymentResultWithValidPaymentButCanceledCreatesPaymentLog()
        {
            #region Arrange
            ControllerRecordFakes.FakeTransactions(Transactions, 3);

            AssignContactEmail(Transactions[1]);

            Transactions[1].Amount = 200.00m;
            Transactions[1].IsActive = false; // Will *NOT* be changed because payment was canceled
            var parameters = DefaultParameters();

            parameters.EXT_TRANS_ID = Transactions[1].TransactionGuid.ToString();
            parameters.PMT_AMT = Transactions[1].Total;
            parameters.PMT_STATUS = "Canceled";
            TransactionRepository.Expect(a => a.Queryable).Return(Transactions.AsQueryable()).Repeat.Any();
            #endregion Arrange

            #region Act

            Controller.PaymentResult(parameters)
                .AssertViewRendered();
            #endregion Act

            #region Assert
            NotificationProvider.AssertWasNotCalled(a => a.SendPaymentResultErrors(Arg<String>.Is.Anything, Arg<PaymentResultParameters>.Is.Anything, Arg<System.Collections.Specialized.NameValueCollection>.Is.Anything, Arg<string>.Is.Anything, Arg<PaymentResultType>.Is.Anything));
            TransactionRepository.AssertWasCalled(a => a.EnsurePersistent(Transactions[1]));
            NotificationProvider.AssertWasNotCalled(a => a.SendConfirmation(Controller.Repository, Transactions[1], TransactionAnswers[0].Answer));

            var args = (Transaction)TransactionRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Transactions[1]))[0][0];
            Assert.IsFalse(args.IsActive);
            Assert.AreEqual(1, args.PaymentLogs.Count);
            var arg_PaymentLog = args.PaymentLogs.ElementAt(0);
            Assert.IsTrue(arg_PaymentLog.Credit);
            Assert.IsFalse(arg_PaymentLog.Check);
            Assert.IsFalse(arg_PaymentLog.Accepted);
            Assert.IsNull(arg_PaymentLog.Name);
            Assert.AreEqual(parameters.PMT_AMT.Value, arg_PaymentLog.Amount);
            Assert.IsNull(arg_PaymentLog.GatewayTransactionId);
            Assert.IsNull(arg_PaymentLog.CardType);
            Assert.AreEqual(parameters.acct_addr, arg_PaymentLog.TnBillingAddress1);
            Assert.AreEqual(parameters.acct_addr2, arg_PaymentLog.TnBillingAddress2);
            Assert.AreEqual(parameters.acct_city, arg_PaymentLog.TnBillingCity);
            Assert.AreEqual(parameters.acct_state, arg_PaymentLog.TnBillingState);
            Assert.AreEqual(parameters.acct_zip, arg_PaymentLog.TnBillingZip);
            Assert.AreEqual(parameters.CANCEL_LINK, arg_PaymentLog.TnCancelLink);
            Assert.AreEqual(parameters.ERROR_LINK, arg_PaymentLog.TnErrorLink);
            Assert.AreEqual(parameters.pmt_date, arg_PaymentLog.TnPaymentDate);
            Assert.AreEqual(parameters.Submit, arg_PaymentLog.TnSubmit);
            Assert.AreEqual(parameters.SUCCESS_LINK, arg_PaymentLog.TnSuccessLink);
            Assert.AreEqual(parameters.sys_tracking_id, arg_PaymentLog.TnSysTrackingId);
            Assert.AreEqual(parameters.UPAY_SITE_ID, arg_PaymentLog.TnUpaySiteId);
            Assert.AreEqual("C", arg_PaymentLog.TnStatus);
            #endregion Assert
        }

        [TestMethod]
        public void TestPaymentResultWithValidPaymentButErrorCreatesPaymentLog()
        {
            #region Arrange
            ControllerRecordFakes.FakeTransactions(Transactions, 3);

            AssignContactEmail(Transactions[1]);

            Transactions[1].Amount = 200.00m;
            Transactions[1].IsActive = false; // Will *NOT* be changed because payment was canceled
            var parameters = DefaultParameters();

            parameters.EXT_TRANS_ID = Transactions[1].TransactionGuid.ToString();
            parameters.PMT_AMT = Transactions[1].Total;
            parameters.PMT_STATUS = "Succe";
            TransactionRepository.Expect(a => a.Queryable).Return(Transactions.AsQueryable()).Repeat.Any();
            #endregion Arrange

            #region Act

            Controller.PaymentResult(parameters)
                .AssertViewRendered();
            #endregion Act

            #region Assert
            NotificationProvider.AssertWasNotCalled(a => a.SendPaymentResultErrors(Arg<String>.Is.Anything, Arg<PaymentResultParameters>.Is.Anything, Arg<System.Collections.Specialized.NameValueCollection>.Is.Anything, Arg<string>.Is.Anything, Arg<PaymentResultType>.Is.Anything));
            TransactionRepository.AssertWasCalled(a => a.EnsurePersistent(Transactions[1]));
            NotificationProvider.AssertWasNotCalled(a => a.SendConfirmation(Controller.Repository, Transactions[1], TransactionAnswers[0].Answer));

            var args = (Transaction)TransactionRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Transactions[1]))[0][0];
            Assert.IsFalse(args.IsActive);
            Assert.AreEqual(1, args.PaymentLogs.Count);
            var arg_PaymentLog = args.PaymentLogs.ElementAt(0);
            Assert.IsTrue(arg_PaymentLog.Credit);
            Assert.IsFalse(arg_PaymentLog.Check);
            Assert.IsFalse(arg_PaymentLog.Accepted);
            Assert.IsNull(arg_PaymentLog.Name);
            Assert.AreEqual(parameters.PMT_AMT.Value, arg_PaymentLog.Amount);
            Assert.IsNull(arg_PaymentLog.GatewayTransactionId);
            Assert.IsNull(arg_PaymentLog.CardType);
            Assert.AreEqual(parameters.acct_addr, arg_PaymentLog.TnBillingAddress1);
            Assert.AreEqual(parameters.acct_addr2, arg_PaymentLog.TnBillingAddress2);
            Assert.AreEqual(parameters.acct_city, arg_PaymentLog.TnBillingCity);
            Assert.AreEqual(parameters.acct_state, arg_PaymentLog.TnBillingState);
            Assert.AreEqual(parameters.acct_zip, arg_PaymentLog.TnBillingZip);
            Assert.AreEqual(parameters.CANCEL_LINK, arg_PaymentLog.TnCancelLink);
            Assert.AreEqual(parameters.ERROR_LINK, arg_PaymentLog.TnErrorLink);
            Assert.AreEqual(parameters.pmt_date, arg_PaymentLog.TnPaymentDate);
            Assert.AreEqual(parameters.Submit, arg_PaymentLog.TnSubmit);
            Assert.AreEqual(parameters.SUCCESS_LINK, arg_PaymentLog.TnSuccessLink);
            Assert.AreEqual(parameters.sys_tracking_id, arg_PaymentLog.TnSysTrackingId);
            Assert.AreEqual(parameters.UPAY_SITE_ID, arg_PaymentLog.TnUpaySiteId);
            Assert.AreEqual("E", arg_PaymentLog.TnStatus);
            #endregion Assert
        }

        [TestMethod, Ignore]
        public void TestPaymentResultWithWrongPostingKeyDoesNotSave()
        {
            #region Arrange
            ControllerRecordFakes.FakeTransactions(Transactions, 3);

            AssignContactEmail(Transactions[1]);

            Transactions[1].Amount = 200.00m;
            var parameters = DefaultParameters();

            parameters.EXT_TRANS_ID = Transactions[1].TransactionGuid.ToString();
            parameters.PMT_AMT = Transactions[1].Total;
            parameters.posting_key = "Wrong";
            TransactionRepository.Expect(a => a.Queryable).Return(Transactions.AsQueryable()).Repeat.Any();
            #endregion Arrange

            #region Act

            Controller.PaymentResult(parameters)
                .AssertViewRendered();
            #endregion Act

            #region Assert
            NotificationProvider.AssertWasNotCalled(a => a.SendPaymentResultErrors(Arg<String>.Is.Anything, Arg<PaymentResultParameters>.Is.Anything, Arg<System.Collections.Specialized.NameValueCollection>.Is.Anything, Arg<string>.Is.Anything, Arg<PaymentResultType>.Is.Anything));
            TransactionRepository.AssertWasCalled(a => a.EnsurePersistent(Transactions[1]));
            NotificationProvider.AssertWasCalled(a => a.SendConfirmation(Controller.Repository, Transactions[1], TransactionAnswers[0].Answer));

            var args = (Transaction)TransactionRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Transactions[1]))[0][0];
            Assert.IsTrue(args.IsActive);
            Assert.AreEqual(1, args.PaymentLogs.Count);
            var arg_PaymentLog = args.PaymentLogs.ElementAt(0);
            Assert.IsTrue(arg_PaymentLog.Credit);
            Assert.IsFalse(arg_PaymentLog.Check);
            Assert.IsTrue(arg_PaymentLog.Accepted);
            Assert.AreEqual(parameters.NAME_ON_ACCT, arg_PaymentLog.Name);
            Assert.AreEqual(parameters.PMT_AMT.Value, arg_PaymentLog.Amount);
            Assert.AreEqual(parameters.TPG_TRANS_ID, arg_PaymentLog.GatewayTransactionId);
            Assert.AreEqual(parameters.CARD_TYPE, arg_PaymentLog.CardType);
            Assert.AreEqual(parameters.acct_addr, arg_PaymentLog.TnBillingAddress1);
            Assert.AreEqual(parameters.acct_addr2, arg_PaymentLog.TnBillingAddress2);
            Assert.AreEqual(parameters.acct_city, arg_PaymentLog.TnBillingCity);
            Assert.AreEqual(parameters.acct_state, arg_PaymentLog.TnBillingState);
            Assert.AreEqual(parameters.acct_zip, arg_PaymentLog.TnBillingZip);
            Assert.AreEqual(parameters.CANCEL_LINK, arg_PaymentLog.TnCancelLink);
            Assert.AreEqual(parameters.ERROR_LINK, arg_PaymentLog.TnErrorLink);
            Assert.AreEqual(parameters.pmt_date, arg_PaymentLog.TnPaymentDate);
            Assert.AreEqual(parameters.Submit, arg_PaymentLog.TnSubmit);
            Assert.AreEqual(parameters.SUCCESS_LINK, arg_PaymentLog.TnSuccessLink);
            Assert.AreEqual(parameters.sys_tracking_id, arg_PaymentLog.TnSysTrackingId);
            Assert.AreEqual(parameters.UPAY_SITE_ID, arg_PaymentLog.TnUpaySiteId);
            Assert.AreEqual("S", arg_PaymentLog.TnStatus);
            #endregion Assert
        }

        #endregion PaymentResult Tests

        #region Helper Methods

        private PaymentResultParameters DefaultParameters()
        {
            var parameters = new PaymentResultParameters();

            parameters.acct_addr = "SomeStreet";
            parameters.acct_addr2 = "No This Street";
            parameters.acct_city = "Davis";
            parameters.acct_state = "CA";
            parameters.acct_zip = "95616";
            parameters.CANCEL_LINK = "CancelLink";
            parameters.CARD_TYPE = "MC";
            parameters.ERROR_LINK = "ErrorLink";
            parameters.EXT_TRANS_ID = Guid.Empty.ToString();
            parameters.NAME_ON_ACCT = "Zoid";
            parameters.PMT_AMT = 10.12m;
            parameters.pmt_date = DateTime.Now.ToString();
            parameters.PMT_STATUS = "success";
            parameters.posting_key = "TOUCHNETKEY";
            parameters.SUCCESS_LINK = "SuccessLink";
            parameters.sys_tracking_id = "SystemTrackingId";
            parameters.TPG_TRANS_ID = "GateWayId";
            parameters.UPAY_SITE_ID = "17";
            return parameters;
        }

        private void AssignContactEmail(Transaction transaction)
        {
            if (QuestionSets.Count == 0)
            {
                ControllerRecordFakes.FakeQuestionSets(QuestionSets, 1);   
            }
            if (Questions.Count == 0)
            {
                ControllerRecordFakes.FakeQuestions(Questions, 1);
            }
            if (TransactionAnswers.Count == 0)
            {
                ControllerRecordFakes.FakeTransactionAnswers(TransactionAnswers, 1);
            }
            
            QuestionSets[0].Name = StaticValues.QuestionSet_ContactInformation;
            Questions[0].Name = StaticValues.Question_Email;
            TransactionAnswers[0].Answer = "jasoncsylvestre@gmail.com";
            TransactionAnswers[0].Question = Questions[0];
            TransactionAnswers[0].QuestionSet = QuestionSets[0];
            transaction.AddTransactionAnswer(TransactionAnswers[0]);
        }

        #endregion Helper Methods
    }
}
