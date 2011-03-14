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

namespace CRP.Tests.Repositories
{
    [TestClass]
    public class CouponRepositoryTests :AbstractRepositoryTests<Coupon, int>
    {
        protected IRepository<Coupon> CouponRepository { get; set; }

        #region Init and Overrides

        /// <summary>
        /// Initializes a new instance of the <see cref="CouponRepositoryTests"/> class.
        /// </summary>
        public CouponRepositoryTests()
        {
            CouponRepository = new Repository<Coupon>();
        }

        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override Coupon GetValid(int? counter)
        {
            var rtValue = CreateValidEntities.Coupon(counter);
            rtValue.Item = Repository.OfType<Item>().GetById(1);
            rtValue.Item.CostPerItem = 10000.00m;
            return rtValue;
        }

        /// <summary>
        /// A Query which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<Coupon> GetQuery(int numberAtEnd)
        {
            return Repository.OfType<Coupon>().Queryable.Where(a => a.Email.EndsWith(numberAtEnd.ToString()));
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(Coupon entity, int counter)
        {
            Assert.AreEqual("email@test.edu" + counter, entity.Email);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(Coupon entity, ARTAction action)
        {
            const string updateValue = "updated@test.edu";
            switch (action)
            {
                case ARTAction.Compare:
                    Assert.AreEqual(updateValue, entity.Email);
                    break;
                case ARTAction.Restore:
                    entity.Email = RestoreValue;
                    break;
                case ARTAction.Update:
                    RestoreValue = entity.Email;
                    entity.Email = updateValue;
                    break;
            }
        }

        /// <summary>
        /// Loads the data.
        /// Coupon Requires Item.
        /// Item Requires Unit.
        /// Item requires ItemType
        /// </summary>
        protected override void LoadData()
        {
            Repository.OfType<Coupon>().DbContext.BeginTransaction();
            LoadUnits(1);
            LoadItemTypes(1);
            LoadItems(1);
            LoadRecords(5);
            Repository.OfType<Coupon>().DbContext.CommitTransaction();
        }

        

        #endregion Init and Overrides

        #region Code Tests

        #region Invalid Tests

        /// <summary>
        /// Tests the code with null does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestCodeWithNullDoesNotSave()
        {
            Coupon coupon = null;
            try
            {
                #region Arrange
                coupon = GetValid(9);
                coupon.Code = null;
                #endregion Arrange

                #region Act
                CouponRepository.DbContext.BeginTransaction();
                CouponRepository.EnsurePersistent(coupon);
                CouponRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                #region Assert
                Assert.IsNotNull(coupon);
                var results = coupon.ValidationResults().AsMessageList();
                results.AssertErrorsAre(
                    "Code: may not be null or empty");
                Assert.IsTrue(coupon.IsTransient());
                Assert.IsFalse(coupon.IsValid());
                #endregion Assert

                throw;
            }
        }

        /// <summary>
        /// Tests the code with empty string does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestCodeWithEmptyStringDoesNotSave()
        {
            Coupon coupon = null;
            try
            {
                #region Arrange
                coupon = GetValid(9);
                coupon.Code = string.Empty;
                #endregion Arrange

                #region Act
                CouponRepository.DbContext.BeginTransaction();
                CouponRepository.EnsurePersistent(coupon);
                CouponRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                #region Assert
                Assert.IsNotNull(coupon);
                var results = coupon.ValidationResults().AsMessageList();
                results.AssertErrorsAre(
                    "Code: may not be null or empty",
                    "Code: length must be between 10 and 10");
                Assert.IsTrue(coupon.IsTransient());
                Assert.IsFalse(coupon.IsValid());
                #endregion Assert

                throw;
            }
        }

        /// <summary>
        /// Tests the code with spaces only does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestCodeWithSpacesOnlyDoesNotSave()
        {
            Coupon coupon = null;
            try
            {
                #region Arrange
                coupon = GetValid(9);
                coupon.Code = " ";
                #endregion Arrange

                #region Act
                CouponRepository.DbContext.BeginTransaction();
                CouponRepository.EnsurePersistent(coupon);
                CouponRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                #region Assert
                Assert.IsNotNull(coupon);
                var results = coupon.ValidationResults().AsMessageList();
                results.AssertErrorsAre(
                    "Code: may not be null or empty",
                    "Code: length must be between 10 and 10");
                Assert.IsTrue(coupon.IsTransient());
                Assert.IsFalse(coupon.IsValid());
                #endregion Assert

                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestCodeWithTestSpacesOnlyDoesNotSave()
        {
            Coupon coupon = null;
            try
            {
                #region Arrange
                coupon = GetValid(9);
                coupon.Code = "          ";
                #endregion Arrange

                #region Act
                CouponRepository.DbContext.BeginTransaction();
                CouponRepository.EnsurePersistent(coupon);
                CouponRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                #region Assert
                Assert.IsNotNull(coupon);
                var results = coupon.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Code: may not be null or empty");
                Assert.IsTrue(coupon.IsTransient());
                Assert.IsFalse(coupon.IsValid());
                #endregion Assert

                throw;
            }
        }

        /// <summary>
        /// Tests the code too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestCodeTooLongValueDoesNotSave()
        {
            Coupon coupon = null;
            try
            {
                #region Arrange
                coupon = GetValid(9);
                coupon.Code = "12345678901";
                #endregion Arrange

                #region Act
                CouponRepository.DbContext.BeginTransaction();
                CouponRepository.EnsurePersistent(coupon);
                CouponRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                #region Assert
                Assert.IsNotNull(coupon);
                var results = coupon.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Code: length must be between 10 and 10");
                Assert.IsTrue(coupon.IsTransient());
                Assert.IsFalse(coupon.IsValid());
                #endregion Assert

                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestCodeTooShortValueDoesNotSave()
        {
            Coupon coupon = null;
            try
            {
                #region Arrange
                coupon = GetValid(9);
                coupon.Code = "123456789";
                #endregion Arrange

                #region Act
                CouponRepository.DbContext.BeginTransaction();
                CouponRepository.EnsurePersistent(coupon);
                CouponRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                #region Assert
                Assert.IsNotNull(coupon);
                var results = coupon.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Code: length must be between 10 and 10");
                Assert.IsTrue(coupon.IsTransient());
                Assert.IsFalse(coupon.IsValid());
                #endregion Assert

                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the code with ten characters saves.
        /// </summary>
        [TestMethod]
        public void TestCodeWithTenCharactersSaves()
        {
            #region Arrange
            var coupon = GetValid(9);
            coupon.Code = "1234567890";
            #endregion Arrange

            #region Act
            CouponRepository.DbContext.BeginTransaction();
            CouponRepository.EnsurePersistent(coupon);
            CouponRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(10, coupon.Code.Length);
            Assert.IsFalse(coupon.IsTransient());
            Assert.IsTrue(coupon.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests


        #endregion Code Tests

        #region Item Tests

        #region Invalid Tests

        /// <summary>
        /// Tests the item when null does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestItemWhenNullDoesNotSave()
        {
            Coupon coupon = null;
            try
            {
                #region Arrange
                coupon = GetValid(9);
                coupon.Item = null;
                #endregion Arrange

                #region Act
                CouponRepository.DbContext.BeginTransaction();
                CouponRepository.EnsurePersistent(coupon);
                CouponRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                #region Assert
                Assert.IsNotNull(coupon);
                var results = coupon.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Item: may not be null");
                Assert.IsTrue(coupon.IsTransient());
                Assert.IsFalse(coupon.IsValid());
                #endregion Assert

                throw;
            }		
        }

        /// <summary>
        /// Tests the item when new item does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(NHibernate.PropertyValueException))]
        public void TestItemWhenNewItemDoesNotSave()
        {
            Coupon coupon = null;
            try
            {
                #region Arrange
                coupon = GetValid(9);
                coupon.Item = new Item();
                coupon.Item.CostPerItem = 10000.00m;
                #endregion Arrange

                #region Act
                CouponRepository.DbContext.BeginTransaction();
                CouponRepository.EnsurePersistent(coupon);
                CouponRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                #region Assert
                Assert.IsNotNull(coupon);
                Assert.IsNotNull(ex);
                Assert.AreEqual("not-null property references a null or transient valueCRP.Core.Domain.Coupon.Item", ex.Message);
                #endregion Assert

                throw;
            }
        }
        
        #endregion Invalid Tests

        #region Valid Test

        /// <summary>
        /// Tests the different item saves.
        /// </summary>
        [TestMethod]
        public void TestDifferentItemSaves()
        {
            #region Arrange
            LoadItems(3);
            var coupon = GetValid(9);
            coupon.Item = Repository.OfType<Item>().GetNullableById(3);
            coupon.Item.CostPerItem = coupon.DiscountAmount + 1;
            #endregion Arrange

            #region Act
            CouponRepository.DbContext.BeginTransaction();
            CouponRepository.EnsurePersistent(coupon);
            CouponRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(coupon.IsTransient());
            Assert.IsTrue(coupon.IsValid());
            #endregion Assert	
        }
        
        #endregion Valid Test
        
        #endregion Item Tests

        #region Unlimited Tests

        /// <summary>
        /// Tests the unlimited is false saves.
        /// </summary>
        [TestMethod]
        public void TestUnlimitedIsFalseSaves()
        {
            #region Arrange
            var coupon = GetValid(9);
            //coupon.Unlimited = false;
            #endregion Arrange

            #region Act
            CouponRepository.DbContext.BeginTransaction();
            CouponRepository.EnsurePersistent(coupon);
            CouponRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(coupon.IsTransient());
            Assert.IsTrue(coupon.IsValid());
            #endregion Assert		
        }

        /// <summary>
        /// Tests the unlimited is true saves.
        /// </summary>
        [TestMethod]
        public void TestUnlimitedIsTrueSaves()
        {
            #region Arrange
            var coupon = GetValid(9);
            //coupon.Unlimited = true;
            #endregion Arrange

            #region Act
            CouponRepository.DbContext.BeginTransaction();
            CouponRepository.EnsurePersistent(coupon);
            CouponRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(coupon.IsTransient());
            Assert.IsTrue(coupon.IsValid());
            #endregion Assert
        }
        
        #endregion Unlimited Tests

        #region Expiration Tests

        /// <summary>
        /// Tests the expiration with null value saves.
        /// </summary>
        [TestMethod]
        public void TestExpirationWithNullValueSaves()
        {
            #region Arrange
            var fakeDate = new DateTime(2010, 02, 14);
            SystemTime.Now = () => fakeDate;
            var coupon = GetValid(9);
            coupon.Expiration = null;
            #endregion Arrange

            #region Act
            CouponRepository.DbContext.BeginTransaction();
            CouponRepository.EnsurePersistent(coupon);
            CouponRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNull(coupon.Expiration);
            Assert.IsFalse(coupon.IsTransient());
            Assert.IsTrue(coupon.IsValid());
            #endregion Assert		
        }

        /// <summary>
        /// Tests the expiration with past value saves.
        /// </summary>
        [TestMethod]
        public void TestExpirationWithPastValueSaves()
        {
            #region Arrange
            var fakeDate = new DateTime(2010, 02, 14);
            SystemTime.Now = () => fakeDate;
            var coupon = GetValid(9);
            coupon.Expiration = new DateTime(2009,01,01);
            #endregion Arrange

            #region Act
            CouponRepository.DbContext.BeginTransaction();
            CouponRepository.EnsurePersistent(coupon);
            CouponRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(new DateTime(2009,01,01), coupon.Expiration);
            Assert.IsFalse(coupon.IsTransient());
            Assert.IsTrue(coupon.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the expiration with future value saves.
        /// </summary>
        [TestMethod]
        public void TestExpirationWithFutureValueSaves()
        {
            #region Arrange
            var fakeDate = new DateTime(2010, 02, 14);
            SystemTime.Now = () => fakeDate;
            var coupon = GetValid(9);
            coupon.Expiration = DateTime.Now.AddYears(1);
            #endregion Arrange

            #region Act
            CouponRepository.DbContext.BeginTransaction();
            CouponRepository.EnsurePersistent(coupon);
            CouponRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(coupon.IsTransient());
            Assert.IsTrue(coupon.IsValid());
            #endregion Assert
        }
        
        #endregion Expiration Tests

        #region Email Tests

        #region Invalid Tests

        /// <summary>
        /// Tests the email with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestEmailWithTooLongValueDoesNotSave()
        {
            Coupon coupon = null;
            try
            {
                #region Arrange
                coupon = GetValid(9);
                coupon.Email = "x".RepeatTimes(101);
                #endregion Arrange

                #region Act
                CouponRepository.DbContext.BeginTransaction();
                CouponRepository.EnsurePersistent(coupon);
                CouponRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                #region Assert                
                Assert.IsNotNull(coupon);
                Assert.AreEqual(101, coupon.Email.Length);
                var results = coupon.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Email: length must be between 0 and 100");
                Assert.IsTrue(coupon.IsTransient());
                Assert.IsFalse(coupon.IsValid());
                #endregion Assert

                throw;
            }		
        }
      
        #endregion Invalid Tests

        #region Email Valid Tests

        /// <summary>
        /// Tests the email with null value saves.
        /// </summary>
        [TestMethod]
        public void TestEmailWithNullValueSaves()
        {
            #region Arrange
            var coupon = GetValid(9);
            //coupon.Unlimited = true;
            coupon.Email = null;
            #endregion Arrange

            #region Act
            CouponRepository.DbContext.BeginTransaction();
            CouponRepository.EnsurePersistent(coupon);
            CouponRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(coupon.IsTransient());
            Assert.IsTrue(coupon.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the email with empty string saves.
        /// </summary>
        [TestMethod]
        public void TestEmailWithEmptyStringSaves()
        {
            #region Arrange
            var coupon = GetValid(9);
            //coupon.Unlimited = true;
            coupon.Email = string.Empty;
            #endregion Arrange

            #region Act
            CouponRepository.DbContext.BeginTransaction();
            CouponRepository.EnsurePersistent(coupon);
            CouponRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(coupon.IsTransient());
            Assert.IsTrue(coupon.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the email with spaces only saves.
        /// </summary>
        [TestMethod]
        public void TestEmailWithSpacesOnlySaves()
        {
            #region Arrange
            var coupon = GetValid(9);
            //coupon.Unlimited = true;
            coupon.Email = " ";
            #endregion Arrange

            #region Act
            CouponRepository.DbContext.BeginTransaction();
            CouponRepository.EnsurePersistent(coupon);
            CouponRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(coupon.IsTransient());
            Assert.IsTrue(coupon.IsValid());
            #endregion Assert
        }
        
        #endregion Email Valid Tests
        
        #endregion Email Tests

        #region Used Tests

        /// <summary>
        /// Tests the used is false saves.
        /// </summary>
        [TestMethod]
        public void TestUsedIsFalseSaves()
        {
            #region Arrange
            var coupon = GetValid(9);
            //coupon.Used = false;
            #endregion Arrange

            #region Act
            CouponRepository.DbContext.BeginTransaction();
            CouponRepository.EnsurePersistent(coupon);
            CouponRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(coupon.IsTransient());
            Assert.IsTrue(coupon.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the used is true saves.
        /// </summary>
        [TestMethod]
        public void TestUsedIsTrueSaves()
        {
            #region Arrange
            var coupon = GetValid(9);
            //coupon.Used = true;
            #endregion Arrange

            #region Act
            CouponRepository.DbContext.BeginTransaction();
            CouponRepository.EnsurePersistent(coupon);
            CouponRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(coupon.IsTransient());
            Assert.IsTrue(coupon.IsValid());
            #endregion Assert
        }
        
        #endregion Used Tests

        #region Discount Amount

        #region Invalid Tests

        /// <summary>
        /// Tests the discount amount less than A penny does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestDiscountAmountLessThanAPennyDoesNotSave1()
        {
            Coupon coupon = null;
            try
            {
                #region Arrange
                coupon = GetValid(9);
                coupon.DiscountAmount = 0.001m;
                #endregion Arrange

                #region Act
                CouponRepository.DbContext.BeginTransaction();
                CouponRepository.EnsurePersistent(coupon);
                CouponRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                #region Assert
                Assert.IsNotNull(coupon);
                var results = coupon.ValidationResults().AsMessageList();
                results.AssertErrorsAre("DiscountAmount: must be more than $0.00");
                Assert.IsTrue(coupon.IsTransient());
                Assert.IsFalse(coupon.IsValid());
                #endregion Assert

                throw;
            }	
        }

        /// <summary>
        /// Tests the discount amount less than A penny does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestDiscountAmountLessThanAPennyDoesNotSave2()
        {
            Coupon coupon = null;
            try
            {
                #region Arrange
                coupon = GetValid(9);
                coupon.DiscountAmount = 0;
                #endregion Arrange

                #region Act
                CouponRepository.DbContext.BeginTransaction();
                CouponRepository.EnsurePersistent(coupon);
                CouponRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                #region Assert
                Assert.IsNotNull(coupon);
                var results = coupon.ValidationResults().AsMessageList();
                results.AssertErrorsAre("DiscountAmount: must be more than $0.00");
                Assert.IsTrue(coupon.IsTransient());
                Assert.IsFalse(coupon.IsValid());
                #endregion Assert

                throw;
            }
        }

        /// <summary>
        /// Tests the discount amount less than A penny does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestDiscountAmountLessThanAPennyDoesNotSave3()
        {
            Coupon coupon = null;
            try
            {
                #region Arrange
                coupon = GetValid(9);
                coupon.DiscountAmount = -1;
                #endregion Arrange

                #region Act
                CouponRepository.DbContext.BeginTransaction();
                CouponRepository.EnsurePersistent(coupon);
                CouponRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                #region Assert
                Assert.IsNotNull(coupon);
                var results = coupon.ValidationResults().AsMessageList();
                results.AssertErrorsAre("DiscountAmount: must be more than $0.00");
                Assert.IsTrue(coupon.IsTransient());
                Assert.IsFalse(coupon.IsValid());
                #endregion Assert

                throw;
            }
        }

        /// <summary>
        /// Tests the discount amount really big not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestDiscountAmountReallyBigNotSave()
        {
            Coupon coupon = null;
            try
            {
                #region Arrange
                coupon = GetValid(9);
                coupon.DiscountAmount = decimal.MaxValue;
                #endregion Arrange

                #region Act
                CouponRepository.DbContext.BeginTransaction();
                CouponRepository.EnsurePersistent(coupon);
                CouponRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                #region Assert
                Assert.IsNotNull(coupon);
                var results = coupon.ValidationResults().AsMessageList();
                results.AssertErrorsAre(
                    "DiscountAmount: must be more than $0.00", 
                    "DiscountAmountCostPerItem: The discount amount must not be greater than the cost per item.");
                Assert.IsTrue(coupon.IsTransient());
                Assert.IsFalse(coupon.IsValid());
                #endregion Assert

                throw;
            }
        }

        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the discount amount of one penny saves.
        /// </summary>
        [TestMethod]
        public void TestDiscountAmountOfOnePennySaves()
        {
            #region Arrange
            var coupon = GetValid(9);
            coupon.DiscountAmount = 0.01m;
            #endregion Arrange

            #region Act
            CouponRepository.DbContext.BeginTransaction();
            CouponRepository.EnsurePersistent(coupon);
            CouponRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(coupon.IsTransient());
            Assert.IsTrue(coupon.IsValid());
            #endregion Assert		
        }

        /// <summary>
        /// Tests the discount amount of one penny saves.
        /// </summary>
        [TestMethod]
        public void TestDiscountAmountWithLargeValueSaves()
        {
            #region Arrange
            var coupon = GetValid(9);
            coupon.DiscountAmount = 999999999.99m;
            coupon.Item.CostPerItem = coupon.DiscountAmount + 0.01m;
            #endregion Arrange

            #region Act
            CouponRepository.DbContext.BeginTransaction();
            CouponRepository.EnsurePersistent(coupon);
            CouponRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(coupon.IsTransient());
            Assert.IsTrue(coupon.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests

        #endregion Discount Amount

        #region UserId Tests

        #region Invalid Tests

        /// <summary>
        /// Tests the user id with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestUserIdWithNullValueDoesNotSave()
        {
            Coupon coupon = null;
            try
            {
                #region Arrange
                coupon = GetValid(9);
                coupon.UserId = null;
                #endregion Arrange

                #region Act
                CouponRepository.DbContext.BeginTransaction();
                CouponRepository.EnsurePersistent(coupon);
                CouponRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                #region Assert
                Assert.IsNotNull(coupon);
                var results = coupon.ValidationResults().AsMessageList();
                results.AssertErrorsAre("UserId: may not be null or empty");
                Assert.IsTrue(coupon.IsTransient());
                Assert.IsFalse(coupon.IsValid());
                #endregion Assert

                throw;
            }		
        }

        /// <summary>
        /// Tests the user id with empty string does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestUserIdWithEmptyStringDoesNotSave()
        {
            Coupon coupon = null;
            try
            {
                #region Arrange
                coupon = GetValid(9);
                coupon.UserId = string.Empty;
                #endregion Arrange

                #region Act
                CouponRepository.DbContext.BeginTransaction();
                CouponRepository.EnsurePersistent(coupon);
                CouponRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                #region Assert
                Assert.IsNotNull(coupon);
                var results = coupon.ValidationResults().AsMessageList();
                results.AssertErrorsAre("UserId: may not be null or empty");
                Assert.IsTrue(coupon.IsTransient());
                Assert.IsFalse(coupon.IsValid());
                #endregion Assert

                throw;
            }
        }

        /// <summary>
        /// Tests the user id with spaces only does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestUserIdWithSpacesOnlyDoesNotSave()
        {
            Coupon coupon = null;
            try
            {
                #region Arrange
                coupon = GetValid(9);
                coupon.UserId = "  ";
                #endregion Arrange

                #region Act
                CouponRepository.DbContext.BeginTransaction();
                CouponRepository.EnsurePersistent(coupon);
                CouponRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                #region Assert
                Assert.IsNotNull(coupon);
                var results = coupon.ValidationResults().AsMessageList();
                results.AssertErrorsAre("UserId: may not be null or empty");
                Assert.IsTrue(coupon.IsTransient());
                Assert.IsFalse(coupon.IsValid());
                #endregion Assert

                throw;
            }
        }

        /// <summary>
        /// Tests the user id with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestUserIdWithTooLongValueDoesNotSave()
        {
            Coupon coupon = null;
            try
            {
                #region Arrange
                coupon = GetValid(9);
                coupon.UserId = "x".RepeatTimes(51);
                #endregion Arrange

                #region Act
                CouponRepository.DbContext.BeginTransaction();
                CouponRepository.EnsurePersistent(coupon);
                CouponRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                #region Assert
                Assert.IsNotNull(coupon);
                Assert.AreEqual(51, coupon.UserId.Length);
                var results = coupon.ValidationResults().AsMessageList();
                results.AssertErrorsAre("UserId: length must be between 0 and 50");
                Assert.IsTrue(coupon.IsTransient());
                Assert.IsFalse(coupon.IsValid());
                #endregion Assert

                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the user id one character long saves.
        /// </summary>
        [TestMethod]
        public void TestUserIdOneCharacterLongSaves()
        {
            #region Arrange
            var coupon = GetValid(9);
            coupon.UserId = "x";
            #endregion Arrange

            #region Act
            CouponRepository.DbContext.BeginTransaction();
            CouponRepository.EnsurePersistent(coupon);
            CouponRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(1, coupon.UserId.Length);
            Assert.IsFalse(coupon.IsTransient());
            Assert.IsTrue(coupon.IsValid());
            #endregion Assert		
        }

        /// <summary>
        /// Tests the user id fifty characters long saves.
        /// </summary>
        [TestMethod]
        public void TestUserIdFiftyCharactersLongSaves()
        {
            #region Arrange
            var coupon = GetValid(9);
            coupon.UserId = "x".RepeatTimes(50);
            #endregion Arrange

            #region Act
            CouponRepository.DbContext.BeginTransaction();
            CouponRepository.EnsurePersistent(coupon);
            CouponRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(50, coupon.UserId.Length);
            Assert.IsFalse(coupon.IsTransient());
            Assert.IsTrue(coupon.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests

        #endregion UserId Tests

        #region IsActive Tests

        /// <summary>
        /// Tests the IsActive is false saves.
        /// </summary>
        [TestMethod]
        public void TestIsActiveIsFalseSaves()
        {
            #region Arrange
            var coupon = GetValid(9);
            coupon.IsActive = false;
            #endregion Arrange

            #region Act
            CouponRepository.DbContext.BeginTransaction();
            CouponRepository.EnsurePersistent(coupon);
            CouponRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(coupon.IsTransient());
            Assert.IsTrue(coupon.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the IsActive is true saves.
        /// </summary>
        [TestMethod]
        public void TestIsActiveIsTrueSaves()
        {
            #region Arrange
            var coupon = GetValid(9);
            coupon.IsActive = true;
            #endregion Arrange

            #region Act
            CouponRepository.DbContext.BeginTransaction();
            CouponRepository.EnsurePersistent(coupon);
            CouponRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(coupon.IsTransient());
            Assert.IsTrue(coupon.IsValid());
            #endregion Assert
        }

        #endregion IsActive Tests

        #region UnlimitedAndEmail Tests

        #region Invalid Tests

        /// <summary>
        /// Tests the unlimited and email are invalid does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestUnlimitedAndEmailAreInvalidDoesNotSave1()
        {
            Coupon coupon = null;
            try
            {
                #region Arrange
                coupon = GetValid(9);
                coupon.Email = null;
                //coupon.Unlimited = false;
                #endregion Arrange

                #region Act
                CouponRepository.DbContext.BeginTransaction();
                CouponRepository.EnsurePersistent(coupon);
                CouponRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                #region Assert
                Assert.IsNotNull(coupon);
                var results = coupon.ValidationResults().AsMessageList();
                results.AssertErrorsAre("UnlimitedAndEmail: When not unlimited a coupon requires an email");
                Assert.IsTrue(coupon.IsTransient());
                Assert.IsFalse(coupon.IsValid());
                #endregion Assert

                throw;
            }
        }

        /// <summary>
        /// Tests the unlimited and email are invalid does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestUnlimitedAndEmailAreInvalidDoesNotSave2()
        {
            Coupon coupon = null;
            try
            {
                #region Arrange
                coupon = GetValid(9);
                coupon.Email = string.Empty;
                //coupon.Unlimited = false;
                #endregion Arrange

                #region Act
                CouponRepository.DbContext.BeginTransaction();
                CouponRepository.EnsurePersistent(coupon);
                CouponRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                #region Assert
                Assert.IsNotNull(coupon);
                var results = coupon.ValidationResults().AsMessageList();
                results.AssertErrorsAre("UnlimitedAndEmail: When not unlimited a coupon requires an email");
                Assert.IsTrue(coupon.IsTransient());
                Assert.IsFalse(coupon.IsValid());
                #endregion Assert

                throw;
            }
        }

        /// <summary>
        /// Tests the unlimited and email are invalid does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestUnlimitedAndEmailAreInvalidDoesNotSave3()
        {
            Coupon coupon = null;
            try
            {
                #region Arrange
                coupon = GetValid(9);
                coupon.Email = " ";
                //coupon.Unlimited = false;
                #endregion Arrange

                #region Act
                CouponRepository.DbContext.BeginTransaction();
                CouponRepository.EnsurePersistent(coupon);
                CouponRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                #region Assert
                Assert.IsNotNull(coupon);
                var results = coupon.ValidationResults().AsMessageList();
                results.AssertErrorsAre("UnlimitedAndEmail: When not unlimited a coupon requires an email");
                Assert.IsTrue(coupon.IsTransient());
                Assert.IsFalse(coupon.IsValid());
                #endregion Assert

                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the unlimited and email with valid data saves.
        /// </summary>
        [TestMethod]
        public void TestUnlimitedAndEmailWithValidDataSaves1()
        {
            #region Arrange
            var coupon = GetValid(9);
            //coupon.Unlimited = true;
            coupon.Email = null;
            #endregion Arrange

            #region Act
            CouponRepository.DbContext.BeginTransaction();
            CouponRepository.EnsurePersistent(coupon);
            CouponRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(coupon.IsTransient());
            Assert.IsTrue(coupon.IsValid());
            #endregion Assert		
        }

        /// <summary>
        /// Tests the unlimited and email with valid data saves.
        /// </summary>
        [TestMethod]
        public void TestUnlimitedAndEmailWithValidDataSaves2()
        {
            #region Arrange
            var coupon = GetValid(9);
            //coupon.Unlimited = true;
            coupon.Email = string.Empty;
            #endregion Arrange

            #region Act
            CouponRepository.DbContext.BeginTransaction();
            CouponRepository.EnsurePersistent(coupon);
            CouponRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(coupon.IsTransient());
            Assert.IsTrue(coupon.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the unlimited and email with valid data saves.
        /// </summary>
        [TestMethod]
        public void TestUnlimitedAndEmailWithValidDataSaves3()
        {
            #region Arrange
            var coupon = GetValid(9);
            //coupon.Unlimited = true;
            coupon.Email = " ";
            #endregion Arrange

            #region Act
            CouponRepository.DbContext.BeginTransaction();
            CouponRepository.EnsurePersistent(coupon);
            CouponRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(coupon.IsTransient());
            Assert.IsTrue(coupon.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the unlimited and email with valid data saves.
        /// </summary>
        [TestMethod]
        public void TestUnlimitedAndEmailWithValidDataSaves4()
        {
            #region Arrange
            var coupon = GetValid(9);
            //coupon.Unlimited = false;
            coupon.Email = "SomeEmail@test.edu";
            #endregion Arrange

            #region Act
            CouponRepository.DbContext.BeginTransaction();
            CouponRepository.EnsurePersistent(coupon);
            CouponRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(coupon.IsTransient());
            Assert.IsTrue(coupon.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests

        #endregion UnlimitedAndEmail Tests

        #region MaxQuantity Tests

        [TestMethod]
        public void TestMaxQuantityWithNullValueSaves()
        {
            #region Arrange
            var coupon = GetValid(9);
            coupon.MaxQuantity = null;
            #endregion Arrange

            #region Act
            CouponRepository.DbContext.BeginTransaction();
            CouponRepository.EnsurePersistent(coupon);
            CouponRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(coupon.IsTransient());
            Assert.IsTrue(coupon.IsValid());
            #endregion Assert	
        }
        [TestMethod]
        public void TestMaxQuantityWithZeroValueSaves()
        {
            #region Arrange
            var coupon = GetValid(9);
            coupon.MaxQuantity = 0;
            #endregion Arrange

            #region Act
            CouponRepository.DbContext.BeginTransaction();
            CouponRepository.EnsurePersistent(coupon);
            CouponRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(coupon.IsTransient());
            Assert.IsTrue(coupon.IsValid());
            #endregion Assert
        }
        [TestMethod]
        public void TestMaxQuantityWithOneValueSaves()
        {
            #region Arrange
            var coupon = GetValid(9);
            coupon.MaxQuantity = 1;
            #endregion Arrange

            #region Act
            CouponRepository.DbContext.BeginTransaction();
            CouponRepository.EnsurePersistent(coupon);
            CouponRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(coupon.IsTransient());
            Assert.IsTrue(coupon.IsValid());
            #endregion Assert
        }
        [TestMethod]
        public void TestMaxQuantityWithLargeValueSaves()
        {
            #region Arrange
            var coupon = GetValid(9);
            coupon.MaxQuantity = 999999999;
            #endregion Arrange

            #region Act
            CouponRepository.DbContext.BeginTransaction();
            CouponRepository.EnsurePersistent(coupon);
            CouponRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(coupon.IsTransient());
            Assert.IsTrue(coupon.IsValid());
            #endregion Assert
        }
        #endregion MaxQuantity Tests

        #region DiscountAmountCostPerItem Tests
        /// <summary>
        /// Tests the discount amount cost per item is invalid does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestDiscountAmountCostPerItemIsInvalidDoesNotSave()
        {
            Coupon coupon = null;
            try
            {
                #region Arrange
                coupon = GetValid(9);
                coupon.DiscountAmount = 10;
                coupon.Item.CostPerItem = 9.99m;
                #endregion Arrange

                #region Act
                CouponRepository.DbContext.BeginTransaction();
                CouponRepository.EnsurePersistent(coupon);
                CouponRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                #region Assert
                Assert.IsNotNull(coupon);
                var results = coupon.ValidationResults().AsMessageList();
                results.AssertErrorsAre("DiscountAmountCostPerItem: The discount amount must not be greater than the cost per item.");
                Assert.IsTrue(coupon.IsTransient());
                Assert.IsFalse(coupon.IsValid());
                #endregion Assert

                throw;
            }
        }


        /// <summary>
        /// Tests the discount amount cost per item when equal saves.
        /// </summary>
        [TestMethod]
        public void TestDiscountAmountCostPerItemWhenEqualSaves()
        {
            #region Arrange
            var coupon = GetValid(9);
            coupon.DiscountAmount = 9.99m;
            coupon.Item.CostPerItem = 9.99m;
            #endregion Arrange

            #region Act
            CouponRepository.DbContext.BeginTransaction();
            CouponRepository.EnsurePersistent(coupon);
            CouponRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(coupon.IsTransient());
            Assert.IsTrue(coupon.IsValid());
            #endregion Assert		
        }

        /// <summary>
        /// Tests the discount amount cost per item when coupon less saves.
        /// </summary>
        [TestMethod]
        public void TestDiscountAmountCostPerItemWhenCouponLessSaves()
        {
            #region Arrange
            var coupon = GetValid(9);
            coupon.DiscountAmount = 9.98m;
            coupon.Item.CostPerItem = 9.99m;
            #endregion Arrange

            #region Act
            CouponRepository.DbContext.BeginTransaction();
            CouponRepository.EnsurePersistent(coupon);
            CouponRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(coupon.IsTransient());
            Assert.IsTrue(coupon.IsValid());
            #endregion Assert
        }
        #endregion DiscountAmountCostPerItem Tests

        #region MaxUsage Tests

        /// <summary>
        /// Tests the MaxUsage with null value saves.
        /// </summary>
        [TestMethod]
        public void TestMaxUsageWithNullValueSaves()
        {
            #region Arrange
            Coupon record = GetValid(9);
            //record.MaxUsage = null;
            #endregion Arrange

            #region Act
            CouponRepository.DbContext.BeginTransaction();
            CouponRepository.EnsurePersistent(record);
            CouponRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNull(record.MaxUsage);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert		
        }

        /// <summary>
        /// Tests the MaxUsage with max int value saves.
        /// </summary>
        [TestMethod]
        public void TestMaxUsageWithMaxIntValueSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.MaxUsage = int.MaxValue;
            //record.Unlimited = false;
            #endregion Arrange

            #region Act
            CouponRepository.DbContext.BeginTransaction();
            CouponRepository.EnsurePersistent(record);
            CouponRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(int.MaxValue, record.MaxUsage);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the MaxUsage with 0 value saves.
        /// </summary>
        [TestMethod]
        public void TestMaxUsageWithZeroValueSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.MaxUsage = 0;
            //record.Unlimited = false;
            #endregion Arrange

            #region Act
            CouponRepository.DbContext.BeginTransaction();
            CouponRepository.EnsurePersistent(record);
            CouponRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(0, record.MaxUsage);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestMaxUsageWithNegativeOneDoesNotSave()
        {
            Coupon coupon = null;
            try
            {
                #region Arrange
                coupon = GetValid(9);
                coupon.MaxUsage = -1;
                //coupon.Unlimited = false;
                #endregion Arrange

                #region Act
                CouponRepository.DbContext.BeginTransaction();
                CouponRepository.EnsurePersistent(coupon);
                CouponRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                #region Assert
                Assert.IsNotNull(coupon);
                var results = coupon.ValidationResults().AsMessageList();
                results.AssertErrorsAre("MaxUsage: must be greater than or equal to 0");
                Assert.IsTrue(coupon.IsTransient());
                Assert.IsFalse(coupon.IsValid());
                #endregion Assert

                throw;
            }
        }

        #endregion MaxUsage Tests

        #region Transactions Tests
        #region Invalid Tests
        /// <summary>
        /// Tests the Transactions with A value of Null does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestTransactionsWithAValueOfNullDoesNotSave()
        {
            Coupon coupon = null;
            try
            {
                #region Arrange
                coupon = GetValid(9);
                coupon.Transactions = null;
                #endregion Arrange

                #region Act
                CouponRepository.DbContext.BeginTransaction();
                CouponRepository.EnsurePersistent(coupon);
                CouponRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(coupon);
                Assert.AreEqual(coupon.Transactions, null);
                var results = coupon.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Transactions: may not be null");
                Assert.IsTrue(coupon.IsTransient());
                Assert.IsFalse(coupon.IsValid());
                throw;
            }
        }

        #endregion Invalid Tests
        #region Valid Tests

        [TestMethod]
        public void TestTransactionsWithEmptyListSaves()
        {
            #region Arrange
            Coupon record = GetValid(9);
            record.Transactions = new List<Transaction>();
            #endregion Arrange

            #region Act
            CouponRepository.DbContext.BeginTransaction();
            CouponRepository.EnsurePersistent(record);
            CouponRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(0, record.Transactions.Count());
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestTransactionsWithPopulatedListSaves()
        {
            #region Arrange
            Coupon record = GetValid(9);
            record.Transactions = new List<Transaction>();
            record.Transactions.Add(CreateValidEntities.Transaction(90));
            record.Transactions.Add(CreateValidEntities.Transaction(91));
            record.Transactions.Add(CreateValidEntities.Transaction(92));
            #endregion Arrange

            #region Act
            CouponRepository.DbContext.BeginTransaction();
            CouponRepository.EnsurePersistent(record);
            CouponRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(3, record.Transactions.Count());
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests
        #region Cascade Tests

        [TestMethod]
        public void TestTransactionsWithNewValuesDoesNotCascadeInsertTransactions()
        {
            #region Arrange
            Repository.OfType<Transaction>().DbContext.BeginTransaction();
            LoadUnits(3);
            LoadItemTypes(3);
            LoadItems(3);
            LoadTransactions(2);
            Repository.OfType<Transaction>().DbContext.CommitTransaction();

            var transaction = Repository.OfType<Transaction>().Queryable.First();
            var transactionCount = Repository.OfType<Transaction>().Queryable.Count();
            Assert.AreEqual(2, transactionCount);

            Coupon record = GetValid(9);
            record.Transactions = new List<Transaction>();
            record.Transactions.Add(CreateValidEntities.Transaction(90));
            record.Transactions.Add(transaction);
            record.Transactions.Add(CreateValidEntities.Transaction(92));
            #endregion Arrange

            #region Act
            CouponRepository.DbContext.BeginTransaction();
            CouponRepository.EnsurePersistent(record);
            CouponRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(3, record.Transactions.Count());
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            var saveCouponId = record.Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            NHibernateSessionManager.Instance.GetSession().Evict(transaction);

            #endregion Assert

            #region Cascade Checks
            Assert.AreEqual(transactionCount, Repository.OfType<Transaction>().Queryable.Count());
            record = Repository.OfType<Coupon>().GetNullableById(saveCouponId);
            Assert.IsNotNull(record);
            Assert.AreEqual(0, record.Transactions.Count);            

            #endregion Cascade Checks

        }


        [TestMethod]
        public void TestTransactionsWithExistingValuesAreRead()
        {
            #region Arrange
            Repository.OfType<Transaction>().DbContext.BeginTransaction();
            LoadUnits(3);
            LoadItemTypes(3);
            LoadItems(3);
            LoadTransactions(4);
            Repository.OfType<Transaction>().DbContext.CommitTransaction();
            var coupon = GetValid(9);
            CouponRepository.DbContext.BeginTransaction();
            CouponRepository.EnsurePersistent(coupon);
            CouponRepository.DbContext.CommitTransaction();

            var transaction1 = Repository.OfType<Transaction>().Queryable.Where(a => a.Id == 1).Single();
            var transaction2 = Repository.OfType<Transaction>().Queryable.Where(a => a.Id == 3).Single();

            transaction1.Coupon = coupon;
            transaction2.Coupon = coupon;

            Repository.OfType<Transaction>().DbContext.BeginTransaction();
            Repository.OfType<Transaction>().EnsurePersistent(transaction1);
            Repository.OfType<Transaction>().EnsurePersistent(transaction2);
            Repository.OfType<Transaction>().DbContext.CommitTransaction();

            var saveCouponId = coupon.Id;
            NHibernateSessionManager.Instance.GetSession().Evict(transaction1);
            NHibernateSessionManager.Instance.GetSession().Evict(transaction2);
            NHibernateSessionManager.Instance.GetSession().Evict(coupon);

            #endregion Arrange

            #region Act
            coupon = CouponRepository.GetNullableById(saveCouponId);
            #endregion Act

            #region Assert
            Assert.IsNotNull(coupon);
            Assert.AreEqual(2, coupon.Transactions.Count);
            #endregion Assert		
        }

        #endregion Cascade Tests
        #endregion Transactions Tests

        #region CalculateUsage Tests


        [TestMethod]
        public void TestCalculateUsageReturnsExpectedValues1()
        {
            #region Arrange
            var transaction1 = CreateValidEntities.Transaction(1);
            var transaction2 = CreateValidEntities.Transaction(1);
            var transaction3 = CreateValidEntities.Transaction(1);

            transaction1.Quantity = 1;
            transaction2.Quantity = 3;
            transaction3.Quantity = 5;

            var coupon = CreateValidEntities.Coupon(1);
            coupon.Transactions.Add(transaction1);
            coupon.Transactions.Add(transaction2);
            coupon.Transactions.Add(transaction3);

            coupon.MaxQuantity = null;

            #endregion Arrange

            #region Act
            //var max = coupon.CalculateUsage();
            #endregion Act

            #region Assert
            //Assert.AreEqual(9, max);
            #endregion Assert		
        }

        [TestMethod]
        public void TestCalculateUsageReturnsExpectedValues2()
        {
            #region Arrange
            var transaction1 = CreateValidEntities.Transaction(1);
            var transaction2 = CreateValidEntities.Transaction(1);
            var transaction3 = CreateValidEntities.Transaction(1);

            transaction1.Quantity = 1;
            transaction2.Quantity = 3;
            transaction3.Quantity = 5;

            var coupon = CreateValidEntities.Coupon(1);
            coupon.Transactions.Add(transaction1);
            coupon.Transactions.Add(transaction2);
            coupon.Transactions.Add(transaction3);

            coupon.MaxQuantity = 5;

            #endregion Arrange

            #region Act
            //var max = coupon.CalculateUsage();
            #endregion Act

            #region Assert
            //Assert.AreEqual(9, max);
            #endregion Assert
        }

        [TestMethod]
        public void TestCalculateUsageReturnsExpectedValues3()
        {
            #region Arrange
            var transaction1 = CreateValidEntities.Transaction(1);
            var transaction2 = CreateValidEntities.Transaction(1);
            var transaction3 = CreateValidEntities.Transaction(1);

            transaction1.Quantity = 1;
            transaction2.Quantity = 3;
            transaction3.Quantity = 5;

            var coupon = CreateValidEntities.Coupon(1);
            coupon.Transactions.Add(transaction1);
            coupon.Transactions.Add(transaction2);
            coupon.Transactions.Add(transaction3);

            coupon.MaxQuantity = 4;

            #endregion Arrange

            #region Act
            //var max = coupon.CalculateUsage();
            #endregion Act

            #region Assert
            //Assert.AreEqual(8, max);
            #endregion Assert
        }

        [TestMethod]
        public void TestCalculateUsageReturnsExpectedValues4()
        {
            #region Arrange
            var transaction1 = CreateValidEntities.Transaction(1);
            var transaction2 = CreateValidEntities.Transaction(1);
            var transaction3 = CreateValidEntities.Transaction(1);

            transaction1.Quantity = 1;
            transaction2.Quantity = 3;
            transaction3.Quantity = 5;

            var coupon = CreateValidEntities.Coupon(1);
            coupon.Transactions.Add(transaction1);
            coupon.Transactions.Add(transaction2);
            coupon.Transactions.Add(transaction3);

            coupon.MaxQuantity = 3;

            #endregion Arrange

            #region Act
            //var max = coupon.CalculateUsage();
            #endregion Act

            #region Assert
            //Assert.AreEqual(7, max);
            #endregion Assert
        }

        [TestMethod]
        public void TestCalculateUsageReturnsExpectedValues5()
        {
            #region Arrange
            var transaction1 = CreateValidEntities.Transaction(1);
            var transaction2 = CreateValidEntities.Transaction(1);
            var transaction3 = CreateValidEntities.Transaction(1);

            transaction1.Quantity = 1;
            transaction2.Quantity = 3;
            transaction3.Quantity = 5;

            var coupon = CreateValidEntities.Coupon(1);
            coupon.Transactions.Add(transaction1);
            coupon.Transactions.Add(transaction2);
            coupon.Transactions.Add(transaction3);

            coupon.MaxQuantity = 2;

            #endregion Arrange

            #region Act
            //var max = coupon.CalculateUsage();
            #endregion Act

            #region Assert
            //Assert.AreEqual(5, max);
            #endregion Assert
        }

        [TestMethod]
        public void TestCalculateUsageReturnsExpectedValues6()
        {
            #region Arrange
            var transaction1 = CreateValidEntities.Transaction(1);
            var transaction2 = CreateValidEntities.Transaction(1);
            var transaction3 = CreateValidEntities.Transaction(1);

            transaction1.Quantity = 1;
            transaction2.Quantity = 3;
            transaction3.Quantity = 5;

            var coupon = CreateValidEntities.Coupon(1);
            coupon.Transactions.Add(transaction1);
            coupon.Transactions.Add(transaction2);
            coupon.Transactions.Add(transaction3);

            coupon.MaxQuantity = 1;

            #endregion Arrange

            #region Act
            //var max = coupon.CalculateUsage();
            #endregion Act

            #region Assert
            //Assert.AreEqual(3, max);
            #endregion Assert
        }

        #endregion CalculateUsage Tests

        #region UnlimitedAndMaxUage Tests
        /// <summary>
        /// Tests the MaxUsage with A value of 5 does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestMaxUsageWithAValueOf5ValueDoesNotSave()
        {
            Coupon coupon = null;
            try
            {
                #region Arrange
                coupon = GetValid(9);
                coupon.MaxUsage = 5;
                //coupon.Unlimited = true;
                #endregion Arrange

                #region Act
                CouponRepository.DbContext.BeginTransaction();
                CouponRepository.EnsurePersistent(coupon);
                CouponRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(coupon);
                Assert.AreEqual(coupon.MaxUsage, 5);
                var results = coupon.ValidationResults().AsMessageList();
                results.AssertErrorsAre("UnlimitedAndMaxUsage: Cannot have unlimited and a max usage defined, one or the other.");
                Assert.IsTrue(coupon.IsTransient());
                Assert.IsFalse(coupon.IsValid());
                throw;
            }	
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestMaxUsageWithAValueOf0ValueDoesNotSave()
        {
            Coupon coupon = null;
            try
            {
                #region Arrange
                coupon = GetValid(9);
                coupon.MaxUsage = 0;
                //coupon.Unlimited = true;
                #endregion Arrange

                #region Act
                CouponRepository.DbContext.BeginTransaction();
                CouponRepository.EnsurePersistent(coupon);
                CouponRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(coupon);
                Assert.AreEqual(coupon.MaxUsage, 0);
                var results = coupon.ValidationResults().AsMessageList();
                results.AssertErrorsAre("UnlimitedAndMaxUsage: Cannot have unlimited and a max usage defined, one or the other.");
                Assert.IsTrue(coupon.IsTransient());
                Assert.IsFalse(coupon.IsValid());
                throw;
            }
        }

        #endregion UnlimitedAndMaxUage Tests

        #region Reflection of Database.

        /// <summary>
        /// Tests all fields in the database have been tested.
        /// If this fails and no other tests, it means that a field has been added which has not been tested above.
        /// </summary>
        [TestMethod]
        public void TestAllFieldsInTheDatabaseHaveBeenTested()
        {
            #region Arrange
            List<AttributeList> attributeList;
            var expectedFields = new List<NameAndType>();
            attributeList = new List<AttributeList>();
            attributeList.Add(new AttributeList("[NHibernate.Validator.Constraints.LengthAttribute(", new List<string>
            {
                "Min = 10",
                "Max = 10"                                                          
            }));
            attributeList.Add(new AttributeList("[UCDArch.Core.NHibernateValidator.Extensions.RequiredAttribute(", new List<string>()));
            expectedFields.Add(new NameAndType("Code", "System.String", new List<AttributeList>(attributeList)));
            //expectedFields.Add(new NameAndType("Code", "System.String", new List<string>
            //{
            //    "[NHibernate.Validator.Constraints.LengthAttribute(Min = 10, Max = 10)]",
            //    "[UCDArch.Core.NHibernateValidator.Extensions.RequiredAttribute()]"
            //}));
            attributeList = new List<AttributeList>();
            attributeList.Add(new AttributeList("[UCDArch.Core.NHibernateValidator.Extensions.RangeDoubleAttribute(", new List<string>
            {
                "Min = 0.01",
                "Message = \"must be more than $0.00\"",
                "Max = 922337203685477"                                                           
            }));
            expectedFields.Add(new NameAndType("DiscountAmount", "System.Decimal", new List<AttributeList>(attributeList)));
            //expectedFields.Add(new NameAndType("DiscountAmount", "System.Decimal", new List<string>
            //{
            //    "[UCDArch.Core.NHibernateValidator.Extensions.RangeDoubleAttribute(Min = 0.01, Max = 922337203685477, Message = \"must be more than $0.00\")]"
            //}));
            expectedFields.Add(new NameAndType("DiscountAmountCostPerItem", "System.Boolean", new List<string>
            {
                "[NHibernate.Validator.Constraints.AssertTrueAttribute(Message = \"The discount amount must not be greater than the cost per item.\")]"
            }));
            expectedFields.Add(new NameAndType("Email", "System.String", new List<string>
            {
                "[NHibernate.Validator.Constraints.LengthAttribute((Int32)100)]"
            }));
            expectedFields.Add(new NameAndType("Expiration", "System.Nullable`1[System.DateTime]", new List<string>()));
            expectedFields.Add(new NameAndType("Id", "System.Int32", new List<string>
            {
                 "[Newtonsoft.Json.JsonPropertyAttribute()]", 
                 "[System.Xml.Serialization.XmlIgnoreAttribute()]"
            }));
            expectedFields.Add(new NameAndType("IsActive", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("Item", "CRP.Core.Domain.Item", new List<string>
            {
                "[NHibernate.Validator.Constraints.NotNullAttribute()]"
            }));
            expectedFields.Add(new NameAndType("MaxQuantity", "System.Nullable`1[System.Int32]", new List<string>()));
            expectedFields.Add(new NameAndType("MaxUsage", "System.Nullable`1[System.Int32]", new List<string>
            {
                "[NHibernate.Validator.Constraints.MinAttribute((Int64)0)]"
            }));
            expectedFields.Add(new NameAndType("Transactions", "System.Collections.Generic.IList`1[CRP.Core.Domain.Transaction]", new List<string>
            {
                "[NHibernate.Validator.Constraints.NotNullAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Unlimited", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("UnlimitedAndEmail", "System.Boolean", new List<string>
            {
                "[NHibernate.Validator.Constraints.AssertTrueAttribute(Message = \"When not unlimited a coupon requires an email\")]"
            }));
            expectedFields.Add(new NameAndType("UnlimitedAndMaxUsage", "System.Boolean", new List<string>
            {
                "[NHibernate.Validator.Constraints.AssertTrueAttribute(Message = \"Cannot have unlimited and a max usage defined, one or the other.\")]"
            }));
            expectedFields.Add(new NameAndType("Used", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("UserId", "System.String", new List<string>
            {
                "[NHibernate.Validator.Constraints.LengthAttribute((Int32)50)]",
                "[UCDArch.Core.NHibernateValidator.Extensions.RequiredAttribute()]"
            }));
            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(Coupon));

        }



        #endregion Reflection of Database.
    }
}
