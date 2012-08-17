using System;
using System.Linq;
using CRP.Core.Domain;
using CRP.Tests.Core.Extensions;
using CRP.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UCDArch.Testing.Extensions;

namespace CRP.Tests.Repositories.ItemRepositoryTests
{
    public partial class ItemRepositoryTests
    {
        #region TransactionQuestionSet Tests
        #region Invalid Tests        
        /// <summary>
        /// Tests the question sets with duplicate transaction value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestQuestionSetsWithDuplicateTransactionValueDoesNotSave1()
        {
            Item item = null;
            try
            {
                #region Arrange
                var questionSet = CreateValidEntities.QuestionSet(1);
                item = GetValid(9);
                item.AddTransactionQuestionSet(questionSet);
                item.AddTransactionQuestionSet(questionSet);
                #endregion Arrange

                #region Act
                ItemRepository.DbContext.BeginTransaction();
                ItemRepository.EnsurePersistent(item);
                ItemRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(item);
                var results = item.ValidationResults().AsMessageList();
                results.AssertErrorsAre("TransactionQuestionSet: Transaction Question is already added");
                Assert.IsTrue(item.IsTransient());
                Assert.IsFalse(item.IsValid());
                throw;
            }
        }
        /// <summary>
        /// Tests the question sets with duplicate transaction value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestQuestionSetsWithDuplicateTransactionValueDoesNotSave2()
        {
            Item item = null;
            try
            {
                #region Arrange
                var questionSet1 = CreateValidEntities.QuestionSet(1);
                var questionSet2 = CreateValidEntities.QuestionSet(1);
                item = GetValid(9);
                item.AddTransactionQuestionSet(questionSet1);
                item.AddTransactionQuestionSet(questionSet2);
                item.AddTransactionQuestionSet(questionSet1);
                #endregion Arrange

                #region Act
                ItemRepository.DbContext.BeginTransaction();
                ItemRepository.EnsurePersistent(item);
                ItemRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(item);
                var results = item.ValidationResults().AsMessageList();
                results.AssertErrorsAre("TransactionQuestionSet: Transaction Question is already added");
                Assert.IsTrue(item.IsTransient());
                Assert.IsFalse(item.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests
        /// <summary>
        /// Tests the transaction ignores duplicate in quantity and saves.
        /// </summary>
        [TestMethod]
        public void TestTransactionIgnoresDuplicateInQuantityAndSaves()
        {
            #region Arrange
            var questionSet = CreateValidEntities.QuestionSet(1);
            var item = GetValid(9);
            item.AddTransactionQuestionSet(questionSet);
            item.AddQuantityQuestionSet(questionSet);
            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(1, item.QuestionSets.Where(a => a.TransactionLevel).Count());
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            #endregion Assert	
        }

        #endregion Valid Tests
        #endregion TransactionQuestionSet Tests
        #region QuantityQuestionSet Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the question sets with duplicate quantity value does not save1.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestQuestionSetsWithDuplicateQuantityValueDoesNotSave1()
        {
            Item item = null;
            try
            {
                #region Arrange
                var questionSet = CreateValidEntities.QuestionSet(1);
                item = GetValid(9);
                item.AddQuantityQuestionSet(questionSet);
                item.AddQuantityQuestionSet(questionSet);
                #endregion Arrange

                #region Act
                ItemRepository.DbContext.BeginTransaction();
                ItemRepository.EnsurePersistent(item);
                ItemRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(item);
                var results = item.ValidationResults().AsMessageList();
                results.AssertErrorsAre("QuantityQuestionSet: Quantity Question is already added");
                Assert.IsTrue(item.IsTransient());
                Assert.IsFalse(item.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the question sets with duplicate quantity value does not save2.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestQuestionSetsWithDuplicateQuantityValueDoesNotSave2()
        {
            Item item = null;
            try
            {
                #region Arrange
                var questionSet1 = CreateValidEntities.QuestionSet(1);
                var questionSet2 = CreateValidEntities.QuestionSet(1);
                item = GetValid(9);
                item.AddQuantityQuestionSet(questionSet1);
                item.AddQuantityQuestionSet(questionSet2);
                item.AddQuantityQuestionSet(questionSet1);
                #endregion Arrange

                #region Act
                ItemRepository.DbContext.BeginTransaction();
                ItemRepository.EnsurePersistent(item);
                ItemRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(item);
                var results = item.ValidationResults().AsMessageList();
                results.AssertErrorsAre("QuantityQuestionSet: Quantity Question is already added");
                Assert.IsTrue(item.IsTransient());
                Assert.IsFalse(item.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests
        /// <summary>
        /// Tests the transaction ignores duplicate in transaction and saves.
        /// </summary>
        [TestMethod]
        public void TestTransactionIgnoresDuplicateInTransactionAndSaves()
        {
            #region Arrange
            var questionSet = CreateValidEntities.QuestionSet(1);
            var item = GetValid(9);
            item.AddQuantityQuestionSet(questionSet);
            item.AddTransactionQuestionSet(questionSet);            
            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(1, item.QuestionSets.Where(a => a.QuantityLevel).Count());
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion QuantityQuestionSet Tests

        #region AllowCheckPayment Tests

        /// <summary>
        /// Tests the AllowCheckPayment is false saves.
        /// </summary>
        [TestMethod]
        public void TestAllowCheckPaymentIsFalseSaves()
        {
            #region Arrange
            Item item = GetValid(9);
            item.AllowCheckPayment = false;            
            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(item.AllowCheckPayment);
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the AllowCheckPayment is true saves.
        /// </summary>
        [TestMethod]
        public void TestAllowCheckPaymentIsTrueSaves()
        {
            #region Arrange
            var item = GetValid(9);
            item.AllowCheckPayment = true;
            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsTrue(item.AllowCheckPayment);
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            #endregion Assert
        }

        #endregion AllowCheckPayment Tests

        #region AllowCreditPayment Tests

        /// <summary>
        /// Tests the AllowCreditPayment is false saves.
        /// </summary>
        [TestMethod]
        public void TestAllowCreditPaymentIsFalseSaves()
        {
            #region Arrange

            Item item = GetValid(9);
            item.AllowCreditPayment = false;

            #endregion Arrange

            #region Act

            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();

            #endregion Act

            #region Assert

            Assert.IsFalse(item.AllowCreditPayment);
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());

            #endregion Assert
        }

        /// <summary>
        /// Tests the AllowCreditPayment is true saves.
        /// </summary>
        [TestMethod]
        public void TestAllowCreditPaymentIsTrueSaves()
        {
            #region Arrange

            var item = GetValid(9);
            item.AllowCreditPayment = true;

            #endregion Arrange

            #region Act

            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();

            #endregion Act

            #region Assert

            Assert.IsTrue(item.AllowCreditPayment);
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());

            #endregion Assert
        }

        #endregion AllowCreditPayment Tests

        #region AllowedPaymentMethods Tests

        /// <summary>
        /// Tests the allowed payment methods allows save with default values.
        /// </summary>
        [TestMethod]
        public void TestAllowedPaymentMethodsAllowsSaveWithDefaultValues()
        {
            #region Arrange
            var item = GetValid(9);
            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsTrue(item.AllowCreditPayment);
            Assert.IsTrue(item.AllowCheckPayment);
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the allowed payment methods prevents save with both payment method values false.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestAllowedPaymentMethodsPreventsSaveWithBothPaymentMethodValuesFalse()
        {
            Item item = null;
            try
            {
                #region Arrange
                item = GetValid(9);
                item.AllowCreditPayment = false;
                item.AllowCheckPayment = false;
                #endregion Arrange

                #region Act
                ItemRepository.DbContext.BeginTransaction();
                ItemRepository.EnsurePersistent(item);
                ItemRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(item);
                Assert.IsFalse(item.AllowCreditPayment);
                Assert.IsFalse(item.AllowCheckPayment);
                var results = item.ValidationResults().AsMessageList();
                results.AssertErrorsAre("AllowedPaymentMethods: Must check at least one payment method");
                Assert.IsTrue(item.IsTransient());
                Assert.IsFalse(item.IsValid());
                throw;
            }
        }



        #endregion AllowedPaymentMethods Tests

        #region Summary Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the Summary with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestSummaryWithNullValueDoesNotSave()
        {
            Item item = null;
            try
            {
                #region Arrange
                item = GetValid(9);
                item.Summary = null;
                #endregion Arrange

                #region Act
                ItemRepository.DbContext.BeginTransaction();
                ItemRepository.EnsurePersistent(item);
                ItemRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(item);
                var results = item.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Summary: may not be null or empty");
                Assert.IsTrue(item.IsTransient());
                Assert.IsFalse(item.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Summary with empty string does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestSummaryWithEmptyStringDoesNotSave()
        {
            Item item = null;
            try
            {
                #region Arrange
                item = GetValid(9);
                item.Summary = string.Empty;
                #endregion Arrange

                #region Act
                ItemRepository.DbContext.BeginTransaction();
                ItemRepository.EnsurePersistent(item);
                ItemRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(item);
                var results = item.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Summary: may not be null or empty");
                Assert.IsTrue(item.IsTransient());
                Assert.IsFalse(item.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Summary with spaces only does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestSummaryWithSpacesOnlyDoesNotSave()
        {
            Item item = null;
            try
            {
                #region Arrange
                item = GetValid(9);
                item.Summary = " ";
                #endregion Arrange

                #region Act
                ItemRepository.DbContext.BeginTransaction();
                ItemRepository.EnsurePersistent(item);
                ItemRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(item);
                var results = item.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Summary: may not be null or empty");
                Assert.IsTrue(item.IsTransient());
                Assert.IsFalse(item.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Summary with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestSummaryWithTooLongValueDoesNotSave()
        {
            Item item = null;
            try
            {
                #region Arrange
                item = GetValid(9);
                item.Summary = "x".RepeatTimes((750 + 1));
                #endregion Arrange

                #region Act
                ItemRepository.DbContext.BeginTransaction();
                ItemRepository.EnsurePersistent(item);
                ItemRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(item);
                Assert.AreEqual(750 + 1, item.Summary.Length);
                var results = item.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Summary: length must be between 0 and 750");
                Assert.IsTrue(item.IsTransient());
                Assert.IsFalse(item.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the Summary with one character saves.
        /// </summary>
        [TestMethod]
        public void TestSummaryWithOneCharacterSaves()
        {
            #region Arrange
            var item = GetValid(9);
            item.Summary = "x";
            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Summary with long value saves.
        /// </summary>
        [TestMethod]
        public void TestSummaryWithLongValueSaves()
        {
            #region Arrange
            var item = GetValid(9);
            item.Summary = "x".RepeatTimes(750);
            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(750, item.Summary.Length);
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Summary Tests

        #region DonationLinkLegend Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the DonationLinkLegend with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestDonationLinkLegendWithTooLongValueDoesNotSave()
        {
            Item item = null;
            try
            {
                #region Arrange
                item = GetValid(9);
                item.DonationLinkLegend = "x".RepeatTimes((50 + 1));
                #endregion Arrange

                #region Act
                ItemRepository.DbContext.BeginTransaction();
                ItemRepository.EnsurePersistent(item);
                ItemRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(item);
                Assert.AreEqual(50 + 1, item.DonationLinkLegend.Length);
                var results = item.ValidationResults().AsMessageList();
                results.AssertErrorsAre("DonationLinkLegend: length must be between 0 and 50");
                Assert.IsTrue(item.IsTransient());
                Assert.IsFalse(item.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the DonationLinkLegend with null value saves.
        /// </summary>
        [TestMethod]
        public void TestDonationLinkLegendWithNullValueSaves()
        {
            #region Arrange
            var item = GetValid(9);
            item.DonationLinkLegend = null;
            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the DonationLinkLegend with empty string saves.
        /// </summary>
        [TestMethod]
        public void TestDonationLinkLegendWithEmptyStringSaves()
        {
            #region Arrange
            var item = GetValid(9);
            item.DonationLinkLegend = string.Empty;
            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the DonationLinkLegend with one space saves.
        /// </summary>
        [TestMethod]
        public void TestDonationLinkLegendWithOneSpaceSaves()
        {
            #region Arrange
            var item = GetValid(9);
            item.DonationLinkLegend = " ";
            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the DonationLinkLegend with one character saves.
        /// </summary>
        [TestMethod]
        public void TestDonationLinkLegendWithOneCharacterSaves()
        {
            #region Arrange
            var item = GetValid(9);
            item.DonationLinkLegend = "x";
            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the DonationLinkLegend with long value saves.
        /// </summary>
        [TestMethod]
        public void TestDonationLinkLegendWithLongValueSaves()
        {
            #region Arrange
            var item = GetValid(9);
            item.DonationLinkLegend = "x".RepeatTimes(50);
            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(50, item.DonationLinkLegend.Length);
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion DonationLinkLegend Tests

        #region DonationLinkInformation Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the DonationLinkInformation with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestDonationLinkInformationWithTooLongValueDoesNotSave()
        {
            Item item = null;
            try
            {
                #region Arrange
                item = GetValid(9);
                item.DonationLinkInformation = "x".RepeatTimes((500 + 1));
                #endregion Arrange

                #region Act
                ItemRepository.DbContext.BeginTransaction();
                ItemRepository.EnsurePersistent(item);
                ItemRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(item);
                Assert.AreEqual(500 + 1, item.DonationLinkInformation.Length);
                var results = item.ValidationResults().AsMessageList();
                results.AssertErrorsAre("DonationLinkInformation: length must be between 0 and 500");
                Assert.IsTrue(item.IsTransient());
                Assert.IsFalse(item.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the DonationLinkInformation with null value saves.
        /// </summary>
        [TestMethod]
        public void TestDonationLinkInformationWithNullValueSaves()
        {
            #region Arrange
            var item = GetValid(9);
            item.DonationLinkInformation = null;
            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the DonationLinkInformation with empty string saves.
        /// </summary>
        [TestMethod]
        public void TestDonationLinkInformationWithEmptyStringSaves()
        {
            #region Arrange
            var item = GetValid(9);
            item.DonationLinkInformation = string.Empty;
            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the DonationLinkInformation with one space saves.
        /// </summary>
        [TestMethod]
        public void TestDonationLinkInformationWithOneSpaceSaves()
        {
            #region Arrange
            var item = GetValid(9);
            item.DonationLinkInformation = " ";
            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the DonationLinkInformation with one character saves.
        /// </summary>
        [TestMethod]
        public void TestDonationLinkInformationWithOneCharacterSaves()
        {
            #region Arrange
            var item = GetValid(9);
            item.DonationLinkInformation = "x";
            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the DonationLinkInformation with long value saves.
        /// </summary>
        [TestMethod]
        public void TestDonationLinkInformationWithLongValueSaves()
        {
            #region Arrange
            var item = GetValid(9);
            item.DonationLinkInformation = "x".RepeatTimes(500);
            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(500, item.DonationLinkInformation.Length);
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion DonationLinkInformation Tests

        #region DonationLinkText Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the DonationLinkText with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestDonationLinkTextWithTooLongValueDoesNotSave()
        {
            Item item = null;
            try
            {
                #region Arrange
                item = GetValid(9);
                item.DonationLinkText = "x".RepeatTimes((50 + 1));
                #endregion Arrange

                #region Act
                ItemRepository.DbContext.BeginTransaction();
                ItemRepository.EnsurePersistent(item);
                ItemRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(item);
                Assert.AreEqual(50 + 1, item.DonationLinkText.Length);
                var results = item.ValidationResults().AsMessageList();
                results.AssertErrorsAre("DonationLinkText: length must be between 0 and 50");
                Assert.IsTrue(item.IsTransient());
                Assert.IsFalse(item.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the DonationLinkText with null value saves.
        /// </summary>
        [TestMethod]
        public void TestDonationLinkTextWithNullValueSaves()
        {
            #region Arrange
            var item = GetValid(9);
            item.DonationLinkText = null;
            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the DonationLinkText with empty string saves.
        /// </summary>
        [TestMethod]
        public void TestDonationLinkTextWithEmptyStringSaves()
        {
            #region Arrange
            var item = GetValid(9);
            item.DonationLinkText = string.Empty;
            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the DonationLinkText with one space saves.
        /// </summary>
        [TestMethod]
        public void TestDonationLinkTextWithOneSpaceSaves()
        {
            #region Arrange
            var item = GetValid(9);
            item.DonationLinkText = " ";
            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the DonationLinkText with one character saves.
        /// </summary>
        [TestMethod]
        public void TestDonationLinkTextWithOneCharacterSaves()
        {
            #region Arrange
            var item = GetValid(9);
            item.DonationLinkText = "x";
            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the DonationLinkText with long value saves.
        /// </summary>
        [TestMethod]
        public void TestDonationLinkTextWithLongValueSaves()
        {
            #region Arrange
            var item = GetValid(9);
            item.DonationLinkText = "x".RepeatTimes(50);
            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(50, item.DonationLinkText.Length);
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion DonationLinkText Tests

        #region DonationLinkLink Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the DonationLinkLink with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestDonationLinkLinkWithTooLongValueDoesNotSave()
        {
            Item item = null;
            try
            {
                #region Arrange
                item = GetValid(9);
                item.DonationLinkLink = "x".RepeatTimes((200 + 1));
                #endregion Arrange

                #region Act
                ItemRepository.DbContext.BeginTransaction();
                ItemRepository.EnsurePersistent(item);
                ItemRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(item);
                Assert.AreEqual(200 + 1, item.DonationLinkLink.Length);
                var results = item.ValidationResults().AsMessageList();
                results.AssertErrorsAre("DonationLinkLink: length must be between 0 and 200");
                Assert.IsTrue(item.IsTransient());
                Assert.IsFalse(item.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the DonationLinkLink with null value saves.
        /// </summary>
        [TestMethod]
        public void TestDonationLinkLinkWithNullValueSaves()
        {
            #region Arrange
            var item = GetValid(9);
            item.DonationLinkLink = null;
            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the DonationLinkLink with empty string saves.
        /// </summary>
        [TestMethod]
        public void TestDonationLinkLinkWithEmptyStringSaves()
        {
            #region Arrange
            var item = GetValid(9);
            item.DonationLinkLink = string.Empty;
            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the DonationLinkLink with one space saves.
        /// </summary>
        [TestMethod]
        public void TestDonationLinkLinkWithOneSpaceSaves()
        {
            #region Arrange
            var item = GetValid(9);
            item.DonationLinkLink = " ";
            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the DonationLinkLink with one character saves.
        /// </summary>
        [TestMethod]
        public void TestDonationLinkLinkWithOneCharacterSaves()
        {
            #region Arrange
            var item = GetValid(9);
            item.DonationLinkLink = "x";
            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the DonationLinkLink with long value saves.
        /// </summary>
        [TestMethod]
        public void TestDonationLinkLinkWithLongValueSaves()
        {
            #region Arrange
            var item = GetValid(9);
            item.DonationLinkLink = "x".RepeatTimes(200);
            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(200, item.DonationLinkLink.Length);
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion DonationLinkLink Tests
 

    }
}
