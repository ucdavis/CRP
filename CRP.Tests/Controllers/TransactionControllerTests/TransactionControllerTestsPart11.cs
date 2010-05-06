using System;
using System.Linq;
using CRP.Core.Abstractions;
using CRP.Core.Domain;
using CRP.Core.Helpers;
using CRP.Core.Resources;
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
            Assert.IsNotNull(args.PaymentLogs.ElementAt(0));
            var argPaymentLog = args.PaymentLogs.ElementAt(0);
            if (argPaymentLog != null)
            {
                Assert.IsTrue(argPaymentLog.Credit);
                Assert.IsFalse(argPaymentLog.Check);
                Assert.IsTrue(argPaymentLog.Accepted);
                Assert.AreEqual(parameters.NAME_ON_ACCT, argPaymentLog.Name);
                Assert.AreEqual(parameters.PMT_AMT.Value, argPaymentLog.Amount);
                Assert.AreEqual(parameters.TPG_TRANS_ID, argPaymentLog.GatewayTransactionId);
                Assert.AreEqual(parameters.CARD_TYPE, argPaymentLog.CardType);
                Assert.AreEqual(parameters.acct_addr, argPaymentLog.TnBillingAddress1);
                Assert.AreEqual(parameters.acct_addr2, argPaymentLog.TnBillingAddress2);
                Assert.AreEqual(parameters.acct_city, argPaymentLog.TnBillingCity);
                Assert.AreEqual(parameters.acct_state, argPaymentLog.TnBillingState);
                Assert.AreEqual(parameters.acct_zip, argPaymentLog.TnBillingZip);
                Assert.AreEqual(parameters.CANCEL_LINK, argPaymentLog.TnCancelLink);
                Assert.AreEqual(parameters.ERROR_LINK, argPaymentLog.TnErrorLink);
                Assert.AreEqual(parameters.pmt_date, argPaymentLog.TnPaymentDate);
                Assert.AreEqual(parameters.Submit, argPaymentLog.TnSubmit);
                Assert.AreEqual(parameters.SUCCESS_LINK, argPaymentLog.TnSuccessLink);
                Assert.AreEqual(parameters.sys_tracking_id, argPaymentLog.TnSysTrackingId);
                Assert.AreEqual(parameters.UPAY_SITE_ID, argPaymentLog.TnUpaySiteId);
                Assert.AreEqual("S", argPaymentLog.TnStatus);
            }

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
            var argPaymentLog = args.PaymentLogs.ElementAt(0);
            Assert.IsTrue(argPaymentLog.Credit);
            Assert.IsFalse(argPaymentLog.Check);
            Assert.IsFalse(argPaymentLog.Accepted);
            Assert.IsNull(argPaymentLog.Name);
            Assert.AreEqual(parameters.PMT_AMT.Value, argPaymentLog.Amount);
            Assert.IsNull(argPaymentLog.GatewayTransactionId);
            Assert.IsNull(argPaymentLog.CardType);
            Assert.AreEqual(parameters.acct_addr, argPaymentLog.TnBillingAddress1);
            Assert.AreEqual(parameters.acct_addr2, argPaymentLog.TnBillingAddress2);
            Assert.AreEqual(parameters.acct_city, argPaymentLog.TnBillingCity);
            Assert.AreEqual(parameters.acct_state, argPaymentLog.TnBillingState);
            Assert.AreEqual(parameters.acct_zip, argPaymentLog.TnBillingZip);
            Assert.AreEqual(parameters.CANCEL_LINK, argPaymentLog.TnCancelLink);
            Assert.AreEqual(parameters.ERROR_LINK, argPaymentLog.TnErrorLink);
            Assert.AreEqual(parameters.pmt_date, argPaymentLog.TnPaymentDate);
            Assert.AreEqual(parameters.Submit, argPaymentLog.TnSubmit);
            Assert.AreEqual(parameters.SUCCESS_LINK, argPaymentLog.TnSuccessLink);
            Assert.AreEqual(parameters.sys_tracking_id, argPaymentLog.TnSysTrackingId);
            Assert.AreEqual(parameters.UPAY_SITE_ID, argPaymentLog.TnUpaySiteId);
            Assert.AreEqual("C", argPaymentLog.TnStatus);
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
            parameters.PMT_STATUS = "Fail";
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
            var argPaymentLog = args.PaymentLogs.ElementAt(0);
            Assert.IsTrue(argPaymentLog.Credit);
            Assert.IsFalse(argPaymentLog.Check);
            Assert.IsFalse(argPaymentLog.Accepted);
            Assert.IsNull(argPaymentLog.Name);
            Assert.AreEqual(parameters.PMT_AMT.Value, argPaymentLog.Amount);
            Assert.IsNull(argPaymentLog.GatewayTransactionId);
            Assert.IsNull(argPaymentLog.CardType);
            Assert.AreEqual(parameters.acct_addr, argPaymentLog.TnBillingAddress1);
            Assert.AreEqual(parameters.acct_addr2, argPaymentLog.TnBillingAddress2);
            Assert.AreEqual(parameters.acct_city, argPaymentLog.TnBillingCity);
            Assert.AreEqual(parameters.acct_state, argPaymentLog.TnBillingState);
            Assert.AreEqual(parameters.acct_zip, argPaymentLog.TnBillingZip);
            Assert.AreEqual(parameters.CANCEL_LINK, argPaymentLog.TnCancelLink);
            Assert.AreEqual(parameters.ERROR_LINK, argPaymentLog.TnErrorLink);
            Assert.AreEqual(parameters.pmt_date, argPaymentLog.TnPaymentDate);
            Assert.AreEqual(parameters.Submit, argPaymentLog.TnSubmit);
            Assert.AreEqual(parameters.SUCCESS_LINK, argPaymentLog.TnSuccessLink);
            Assert.AreEqual(parameters.sys_tracking_id, argPaymentLog.TnSysTrackingId);
            Assert.AreEqual(parameters.UPAY_SITE_ID, argPaymentLog.TnUpaySiteId);
            Assert.AreEqual("E", argPaymentLog.TnStatus);
            #endregion Assert
        }

        [TestMethod]
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
            NotificationProvider.AssertWasCalled(a => a.SendPaymentResultErrors(Arg<String>.Is.Anything, Arg<PaymentResultParameters>.Is.Anything, Arg<System.Collections.Specialized.NameValueCollection>.Is.Anything, Arg<string>.Is.Anything, Arg<PaymentResultType>.Is.Anything));
            TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            NotificationProvider.AssertWasNotCalled(a => a.SendConfirmation(Controller.Repository, Transactions[1], TransactionAnswers[0].Answer));
            var args = NotificationProvider.GetArgumentsForCallsMadeOn(a => a.SendPaymentResultErrors(Arg<String>.Is.Anything, Arg<PaymentResultParameters>.Is.Anything, Arg<System.Collections.Specialized.NameValueCollection>.Is.Anything, Arg<string>.Is.Anything, Arg<PaymentResultType>.Is.Anything))[0];
            Assert.IsNotNull(args);
            Assert.AreEqual("<br/><br/>Payment log values:<br/>Name:Zoid<br/>Amount:200.00<br/>Accepted:False<br/>Gateway transaction id:GateWayId<br/>Card Type: MC<br/>ModelState: False<br/><br/>===== modelstate errors text===<br/>Error:Posting Key Error<br/>", args[3]);
            Assert.AreEqual("InValidPaymentLog", args[4].ToString());
            Assert.AreSame(parameters, args[1]);
            #endregion Assert
        }

        [TestMethod]
        public void TestPaymentResultWithWrongSiteIdDoesNotSave()
        {
            #region Arrange
            ControllerRecordFakes.FakeTransactions(Transactions, 3);

            AssignContactEmail(Transactions[1]);

            Transactions[1].Amount = 200.00m;
            var parameters = DefaultParameters();

            parameters.EXT_TRANS_ID = Transactions[1].TransactionGuid.ToString();
            parameters.PMT_AMT = Transactions[1].Total;
            parameters.UPAY_SITE_ID = "5";
            TransactionRepository.Expect(a => a.Queryable).Return(Transactions.AsQueryable()).Repeat.Any();
            #endregion Arrange

            #region Act

            Controller.PaymentResult(parameters)
                .AssertViewRendered();
            #endregion Act

            #region Assert
            NotificationProvider.AssertWasCalled(a => a.SendPaymentResultErrors(Arg<String>.Is.Anything, Arg<PaymentResultParameters>.Is.Anything, Arg<System.Collections.Specialized.NameValueCollection>.Is.Anything, Arg<string>.Is.Anything, Arg<PaymentResultType>.Is.Anything));
            TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            NotificationProvider.AssertWasNotCalled(a => a.SendConfirmation(Controller.Repository, Transactions[1], TransactionAnswers[0].Answer));
            var args = NotificationProvider.GetArgumentsForCallsMadeOn(a => a.SendPaymentResultErrors(Arg<String>.Is.Anything, Arg<PaymentResultParameters>.Is.Anything, Arg<System.Collections.Specialized.NameValueCollection>.Is.Anything, Arg<string>.Is.Anything, Arg<PaymentResultType>.Is.Anything))[0];
            Assert.IsNotNull(args);
            Assert.AreEqual("<br/><br/>Payment log values:<br/>Name:Zoid<br/>Amount:200.00<br/>Accepted:False<br/>Gateway transaction id:GateWayId<br/>Card Type: MC<br/>ModelState: False<br/><br/>===== modelstate errors text===<br/>Error:TouchNet Site Id Error<br/>", args[3]);
            Assert.AreEqual("InValidPaymentLog", args[4].ToString());
            Assert.AreSame(parameters, args[1]);
            #endregion Assert
        }

        [TestMethod]
        public void TestPaymentResultWithDummyTransIdDoesNotSave()
        {
            #region Arrange
            ControllerRecordFakes.FakeTransactions(Transactions, 3);

            AssignContactEmail(Transactions[1]);

            Transactions[1].Amount = 200.00m;
            var parameters = DefaultParameters();

            parameters.EXT_TRANS_ID = Transactions[1].TransactionGuid.ToString();
            parameters.PMT_AMT = Transactions[1].Total;
            parameters.TPG_TRANS_ID = "DUMMY_TRANS_ID";
            TransactionRepository.Expect(a => a.Queryable).Return(Transactions.AsQueryable()).Repeat.Any();
            #endregion Arrange

            #region Act

            Controller.PaymentResult(parameters)
                .AssertViewRendered();
            #endregion Act

            #region Assert
            NotificationProvider.AssertWasCalled(a => a.SendPaymentResultErrors(Arg<String>.Is.Anything, Arg<PaymentResultParameters>.Is.Anything, Arg<System.Collections.Specialized.NameValueCollection>.Is.Anything, Arg<string>.Is.Anything, Arg<PaymentResultType>.Is.Anything));
            TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            NotificationProvider.AssertWasNotCalled(a => a.SendConfirmation(Controller.Repository, Transactions[1], TransactionAnswers[0].Answer));
            var args = NotificationProvider.GetArgumentsForCallsMadeOn(a => a.SendPaymentResultErrors(Arg<String>.Is.Anything, Arg<PaymentResultParameters>.Is.Anything, Arg<System.Collections.Specialized.NameValueCollection>.Is.Anything, Arg<string>.Is.Anything, Arg<PaymentResultType>.Is.Anything))[0];
            Assert.IsNotNull(args);
            Assert.AreEqual("<br/><br/>Payment log values:<br/>Name:Zoid<br/>Amount:200.00<br/>Accepted:False<br/>Gateway transaction id:DUMMY_TRANS_ID<br/>Card Type: MC<br/>ModelState: False<br/><br/>===== modelstate errors text===<br/>Error:TouchNet TPG_TRANS_ID Error<br/>", args[3]);
            Assert.AreEqual("InValidPaymentLog", args[4].ToString());
            Assert.AreSame(parameters, args[1]);
            #endregion Assert
        }

        [TestMethod]
        public void TestPaymentResultWithDifferentAmountDoesNotSave()
        {
            #region Arrange
            ControllerRecordFakes.FakeTransactions(Transactions, 3);

            AssignContactEmail(Transactions[1]);

            Transactions[1].Amount = 200.00m;
            var parameters = DefaultParameters();

            parameters.EXT_TRANS_ID = Transactions[1].TransactionGuid.ToString();
            parameters.PMT_AMT = 50.00m;
            TransactionRepository.Expect(a => a.Queryable).Return(Transactions.AsQueryable()).Repeat.Any();
            #endregion Arrange

            #region Act

            Controller.PaymentResult(parameters)
                .AssertViewRendered();
            #endregion Act

            #region Assert
            NotificationProvider.AssertWasCalled(a => a.SendPaymentResultErrors(Arg<String>.Is.Anything, Arg<PaymentResultParameters>.Is.Anything, Arg<System.Collections.Specialized.NameValueCollection>.Is.Anything, Arg<string>.Is.Anything, Arg<PaymentResultType>.Is.Anything));
            TransactionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Transaction>.Is.Anything));
            NotificationProvider.AssertWasNotCalled(a => a.SendConfirmation(Controller.Repository, Transactions[1], TransactionAnswers[0].Answer));
            var args = NotificationProvider.GetArgumentsForCallsMadeOn(a => a.SendPaymentResultErrors(Arg<String>.Is.Anything, Arg<PaymentResultParameters>.Is.Anything, Arg<System.Collections.Specialized.NameValueCollection>.Is.Anything, Arg<string>.Is.Anything, Arg<PaymentResultType>.Is.Anything))[0];
            Assert.IsNotNull(args);
            Assert.AreEqual("<br/><br/>Payment log values:<br/>Name:Zoid<br/>Amount:50.00<br/>Accepted:False<br/>Gateway transaction id:GateWayId<br/>Card Type: MC<br/>ModelState: False<br/><br/>===== modelstate errors text===<br/>Error:TouchNet Amount does not match local amount<br/>", args[3]);
            Assert.AreEqual("InValidPaymentLog", args[4].ToString());
            Assert.AreSame(parameters, args[1]);
            #endregion Assert
        }

        [TestMethod]
        public void TestPaymentResultWithValidPaymentButOverPaymentCreatesPaymentLog()
        {
            #region Arrange
            ControllerRecordFakes.FakeTransactions(Transactions, 3);

            AssignContactEmail(Transactions[1]);

            Transactions[1].Amount = 200.00m;
            var payment = CreateValidEntities.PaymentLog(null);
            payment.Amount = 100.00m;
            payment.Accepted = true;
            payment.Credit = true;
            payment.Check = false;
            Transactions[1].AddPaymentLog(payment);

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
            NotificationProvider.AssertWasCalled(a => a.SendPaymentResultErrors(Arg<String>.Is.Anything, Arg<PaymentResultParameters>.Is.Anything, Arg<System.Collections.Specialized.NameValueCollection>.Is.Anything, Arg<string>.Is.Anything, Arg<PaymentResultType>.Is.Anything));
            TransactionRepository.AssertWasCalled(a => a.EnsurePersistent(Transactions[1]));
            NotificationProvider.AssertWasCalled(a => a.SendConfirmation(Controller.Repository, Transactions[1], TransactionAnswers[0].Answer));

            var args = (Transaction)TransactionRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Transactions[1]))[0][0];
            Assert.IsTrue(args.IsActive);
            Assert.AreEqual(2, args.PaymentLogs.Count);
            Assert.IsNotNull(args.PaymentLogs.ElementAt(1));
            var argPaymentLog = args.PaymentLogs.ElementAt(1);
            if (argPaymentLog != null)
            {
                Assert.IsTrue(argPaymentLog.Credit);
                Assert.IsFalse(argPaymentLog.Check);
                Assert.IsTrue(argPaymentLog.Accepted);
                Assert.AreEqual(parameters.NAME_ON_ACCT, argPaymentLog.Name);
                Assert.AreEqual(parameters.PMT_AMT.Value, argPaymentLog.Amount);
                Assert.AreEqual(parameters.TPG_TRANS_ID, argPaymentLog.GatewayTransactionId);
                Assert.AreEqual(parameters.CARD_TYPE, argPaymentLog.CardType);
                Assert.AreEqual(parameters.acct_addr, argPaymentLog.TnBillingAddress1);
                Assert.AreEqual(parameters.acct_addr2, argPaymentLog.TnBillingAddress2);
                Assert.AreEqual(parameters.acct_city, argPaymentLog.TnBillingCity);
                Assert.AreEqual(parameters.acct_state, argPaymentLog.TnBillingState);
                Assert.AreEqual(parameters.acct_zip, argPaymentLog.TnBillingZip);
                Assert.AreEqual(parameters.CANCEL_LINK, argPaymentLog.TnCancelLink);
                Assert.AreEqual(parameters.ERROR_LINK, argPaymentLog.TnErrorLink);
                Assert.AreEqual(parameters.pmt_date, argPaymentLog.TnPaymentDate);
                Assert.AreEqual(parameters.Submit, argPaymentLog.TnSubmit);
                Assert.AreEqual(parameters.SUCCESS_LINK, argPaymentLog.TnSuccessLink);
                Assert.AreEqual(parameters.sys_tracking_id, argPaymentLog.TnSysTrackingId);
                Assert.AreEqual(parameters.UPAY_SITE_ID, argPaymentLog.TnUpaySiteId);
                Assert.AreEqual("S", argPaymentLog.TnStatus);
            }

            var emailArgs = NotificationProvider.GetArgumentsForCallsMadeOn(a => a.SendPaymentResultErrors(Arg<String>.Is.Anything, Arg<PaymentResultParameters>.Is.Anything, Arg<System.Collections.Specialized.NameValueCollection>.Is.Anything, Arg<string>.Is.Anything, Arg<PaymentResultType>.Is.Anything))[0];
            Assert.IsNotNull(emailArgs);
            Assert.IsNull(emailArgs[3]);
            Assert.AreEqual("OverPaid", emailArgs[4].ToString());
            Assert.AreSame(parameters, emailArgs[1]);

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
