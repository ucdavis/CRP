using System;
using CRP.Core.Domain;
using CRP.Tests.Core.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UCDArch.Testing.Extensions;

namespace CRP.Tests.Repositories.ItemRepositoryTests
{
    public partial class ItemRepositoryTests
    {
        #region CheckPaymentInstructions Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the CheckPaymentInstructions with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestCheckPaymentInstructionsWithNullValueDoesNotSave()
        {
            Item item = null;
            try
            {
                #region Arrange
                item = GetValid(9);
                item.CheckPaymentInstructions = null;
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
                results.AssertErrorsAre("CheckPaymentInstructions: may not be null or empty");
                Assert.IsTrue(item.IsTransient());
                Assert.IsFalse(item.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the CheckPaymentInstructions with empty string does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestCheckPaymentInstructionsWithEmptyStringDoesNotSave()
        {
            Item item = null;
            try
            {
                #region Arrange
                item = GetValid(9);
                item.CheckPaymentInstructions = string.Empty;
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
                results.AssertErrorsAre("CheckPaymentInstructions: may not be null or empty");
                Assert.IsTrue(item.IsTransient());
                Assert.IsFalse(item.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the CheckPaymentInstructions with spaces only does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestCheckPaymentInstructionsWithSpacesOnlyDoesNotSave()
        {
            Item item = null;
            try
            {
                #region Arrange
                item = GetValid(9);
                item.CheckPaymentInstructions = " ";
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
                results.AssertErrorsAre("CheckPaymentInstructions: may not be null or empty");
                Assert.IsTrue(item.IsTransient());
                Assert.IsFalse(item.IsValid());
                throw;
            }
        }
        
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the CheckPaymentInstructions with one character saves.
        /// </summary>
        [TestMethod]
        public void TestCheckPaymentInstructionsWithOneCharacterSaves()
        {
            #region Arrange
            var item = GetValid(9);
            item.CheckPaymentInstructions = "x";
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
        /// Tests the CheckPaymentInstructions with long value saves.
        /// </summary>
        [TestMethod]
        public void TestCheckPaymentInstructionsWithLongValueSaves()
        {
            #region Arrange
            var item = GetValid(9);
            item.CheckPaymentInstructions = "x".RepeatTimes(999);
            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(999, item.CheckPaymentInstructions.Length);
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion CheckPaymentInstructions Tests


        #region TouchnetFID Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the TouchnetFID with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestTouchnetFIDWithNullValueDoesNotSave()
        {
            Item item = null;
            try
            {
                #region Arrange
                item = GetValid(9);
                item.TouchnetFID = null;
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
                results.AssertErrorsAre("FID: Must select an Account Number when available to public is checked and credit payment is allowed");
                Assert.IsTrue(item.IsTransient());
                Assert.IsFalse(item.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the TouchnetFID with empty string does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestTouchnetFIDWithEmptyStringDoesNotSave()
        {
            Item item = null;
            try
            {
                #region Arrange
                item = GetValid(9);
                item.TouchnetFID = string.Empty;
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
                results.AssertErrorsAre("FID: Must select an Account Number when available to public is checked and credit payment is allowed");
                Assert.IsTrue(item.IsTransient());
                Assert.IsFalse(item.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the TouchnetFID with spaces only does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestTouchnetFIDWithSpacesOnlyDoesNotSave()
        {
            Item item = null;
            try
            {
                #region Arrange
                item = GetValid(9);
                item.TouchnetFID = " ";
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
                results.AssertErrorsAre("FID_Length: FID must be 3 characters long when selected");
                Assert.IsTrue(item.IsTransient());
                Assert.IsFalse(item.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the TouchnetFID with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestTouchnetFIDWithTooLongValueDoesNotSave()
        {
            Item item = null;
            try
            {
                #region Arrange
                item = GetValid(9);
                item.TouchnetFID = "x".RepeatTimes((3 + 1));
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
                Assert.AreEqual(3 + 1, item.TouchnetFID.Length);
                var results = item.ValidationResults().AsMessageList();
                results.AssertErrorsAre("FID_Length: FID must be 3 characters long when selected");
                Assert.IsTrue(item.IsTransient());
                Assert.IsFalse(item.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the TouchnetFID with too long short does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestTouchnetFIDWithTooLongShortDoesNotSave()
        {
            Item item = null;
            try
            {
                #region Arrange
                item = GetValid(9);
                item.TouchnetFID = "xx";
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
                Assert.AreEqual(2, item.TouchnetFID.Length);
                var results = item.ValidationResults().AsMessageList();
                results.AssertErrorsAre("FID_Length: FID must be 3 characters long when selected");
                Assert.IsTrue(item.IsTransient());
                Assert.IsFalse(item.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the TouchnetFID with three character saves.
        /// </summary>
        [TestMethod]
        public void TestTouchnetFIDWithOneCharacterSaves()
        {
            #region Arrange
            var item = GetValid(9);
            item.TouchnetFID = "xxx";
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
        /// Tests the TouchnetFID with long value saves.
        /// </summary>
        [TestMethod]
        public void TestTouchnetFIDWithLongValueSaves()
        {
            #region Arrange
            var item = GetValid(9);
            item.TouchnetFID = "x".RepeatTimes(3);
            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(3, item.TouchnetFID.Length);
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion TouchnetFID Tests

        #region FID Tests

        #region Invalid Tests
        /// <summary>
        /// Tests the touchnet FID with empty string and available and allow checks does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestTouchnetFIDWithEmptyStringAndAvailableAndAllowChecksDoesNotSave()
        {
            Item item = null;
            try
            {
                #region Arrange
                item = GetValid(9);
                item.Available = true;
                item.AllowCreditPayment = true;
                item.TouchnetFID = string.Empty;
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
                results.AssertErrorsAre("FID: Must select an Account Number when available to public is checked and credit payment is allowed");
                Assert.IsTrue(item.IsTransient());
                Assert.IsFalse(item.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the touchnet FID with null value and available and allow checks does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestTouchnetFIDWithNullValueAndAvailableAndAllowChecksDoesNotSave()
        {
            Item item = null;
            try
            {
                #region Arrange
                item = GetValid(9);
                item.Available = true;
                item.AllowCreditPayment = true;
                item.TouchnetFID = string.Empty;
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
                results.AssertErrorsAre("FID: Must select an Account Number when available to public is checked and credit payment is allowed");
                Assert.IsTrue(item.IsTransient());
                Assert.IsFalse(item.IsValid());
                throw;
            }
        }
        

        #endregion Invalid Tests

        #region Valid Tests
        /// <summary>
        /// Tests the touchnet FID with empty string and not available saves.
        /// </summary>
        [TestMethod]
        public void TestTouchnetFIDWithEmptyStringAndNotAvailableSaves()
        {
            #region Arrange
            var item = GetValid(9);
            item.Available = false;
            item.AllowCreditPayment = true;
            item.TouchnetFID = string.Empty;
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
        /// Tests the touchnet FID with nullvalue and not available saves.
        /// </summary>
        [TestMethod]
        public void TestTouchnetFIDWithNullvalueAndNotAvailableSaves()
        {
            #region Arrange
            var item = GetValid(9);
            item.Available = false;
            item.AllowCreditPayment = true;
            item.TouchnetFID = null;
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
        /// Tests the touchnet FID with empty string and not allow credit saves.
        /// </summary>
        [TestMethod]
        public void TestTouchnetFIDWithEmptyStringAndNotAllowCreditSaves()
        {
            #region Arrange
            var item = GetValid(9);
            item.Available = true;
            item.AllowCreditPayment = false;
            item.TouchnetFID = string.Empty;
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
        /// Tests the touchnet FID with nullvalue and not allow credit saves.
        /// </summary>
        [TestMethod]
        public void TestTouchnetFIDWithNullvalueAndNotAllowCreditSaves()
        {
            #region Arrange
            var item = GetValid(9);
            item.Available = true;
            item.AllowCreditPayment = false;
            item.TouchnetFID = null;
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
        /// Tests the touchnet FID with empty string and not allow credit and not available saves.
        /// </summary>
        [TestMethod]
        public void TestTouchnetFIDWithEmptyStringAndNotAllowCreditAndNotAvailableSaves()
        {
            #region Arrange
            var item = GetValid(9);
            item.Available = false;
            item.AllowCreditPayment = false;
            item.TouchnetFID = string.Empty;
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
        /// Tests the touchnet FID with nullvalue and not allow credit and not available saves.
        /// </summary>
        [TestMethod]
        public void TestTouchnetFIDWithNullvalueAndNotAllowCreditAndNotAvailableSaves()
        {
            #region Arrange
            var item = GetValid(9);
            item.Available = false;
            item.AllowCreditPayment = false;
            item.TouchnetFID = null;
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
        

        #endregion Valid Tests

        #endregion FID Tests

        #region FID Length Tests

        #region Invalid Tests
        /// <summary>
        /// Tests the touchnet FID with short value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestTouchnetFIDWithShortValueDoesNotSave1()
        {
            Item item = null;
            try
            {
                #region Arrange
                item = GetValid(9);
                item.Available = true;
                item.AllowCreditPayment = true;
                item.TouchnetFID = "x";
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
                results.AssertErrorsAre("FID_Length: FID must be 3 characters long when selected");
                Assert.IsTrue(item.IsTransient());
                Assert.IsFalse(item.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the touchnet FID with short value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestTouchnetFIDWithShortValueDoesNotSave2()
        {
            Item item = null;
            try
            {
                #region Arrange
                item = GetValid(9);
                item.Available = false;
                item.AllowCreditPayment = false;
                item.TouchnetFID = "x";
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
                results.AssertErrorsAre("FID_Length: FID must be 3 characters long when selected");
                Assert.IsTrue(item.IsTransient());
                Assert.IsFalse(item.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the touchnet FID with short value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestTouchnetFIDWithShortValueDoesNotSave3()
        {
            Item item = null;
            try
            {
                #region Arrange
                item = GetValid(9);
                item.Available = false;
                item.AllowCreditPayment = true;
                item.TouchnetFID = "xx";
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
                results.AssertErrorsAre("FID_Length: FID must be 3 characters long when selected");
                Assert.IsTrue(item.IsTransient());
                Assert.IsFalse(item.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the touchnet FID with short value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestTouchnetFIDWithShortValueDoesNotSave4()
        {
            Item item = null;
            try
            {
                #region Arrange
                item = GetValid(9);
                item.Available = true;
                item.AllowCreditPayment = false;
                item.TouchnetFID = "xx";
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
                results.AssertErrorsAre("FID_Length: FID must be 3 characters long when selected");
                Assert.IsTrue(item.IsTransient());
                Assert.IsFalse(item.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the touchnet FID with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestTouchnetFIDWithTooLongValueDoesNotSave1()
        {
            Item item = null;
            try
            {
                #region Arrange
                item = GetValid(9);
                item.Available = true;
                item.AllowCreditPayment = true;
                item.TouchnetFID = "xxxx";
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
                results.AssertErrorsAre("FID_Length: FID must be 3 characters long when selected");
                Assert.IsTrue(item.IsTransient());
                Assert.IsFalse(item.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the touchnet FID with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestTouchnetFIDWithTooLongValueDoesNotSave2()
        {
            Item item = null;
            try
            {
                #region Arrange
                item = GetValid(9);
                item.Available = false;
                item.AllowCreditPayment = false;
                item.TouchnetFID = "xxxx";
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
                results.AssertErrorsAre("FID_Length: FID must be 3 characters long when selected");
                Assert.IsTrue(item.IsTransient());
                Assert.IsFalse(item.IsValid());
                throw;
            }
        }

        #endregion Invalid Tests
        

        #endregion FID Length Tests

    }
}
