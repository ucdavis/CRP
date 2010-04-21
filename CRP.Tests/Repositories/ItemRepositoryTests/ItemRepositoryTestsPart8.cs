using System;
using System.Collections.Generic;
using System.Linq;
using CRP.Core.Abstractions;
using CRP.Core.Domain;
using CRP.Tests.Core;
using CRP.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Data.NHibernate;
using UCDArch.Testing.Extensions;

namespace CRP.Tests.Repositories.ItemRepositoryTests
{
    public partial class ItemRepositoryTests
    {
        #region Transaction Tests
        /// <summary>
        /// Tests the transaction for mapping problem.
        /// </summary>
        [TestMethod]
        public void TestTransactionForMappingProblem()
        {
            #region Arrange

            LoadItemTypes(1);
            LoadUnits(1);
            LoadItems(1);
            LoadTransactions(1);

            var transaction = Repository.OfType<Transaction>().GetNullableByID(1);
            var item = GetValid(null);
            item.Transactions = new List<Transaction>();
            item.Transactions.Add(transaction);
            transaction.Item = item;
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
        

        #endregion Transaction Tests

        #region Report Tests
        /// <summary>
        /// Tests the transaction for mapping problem.
        /// </summary>
        [TestMethod]
        public void TestReportsForMappingProblem()
        {
            #region Arrange

            LoadUsers(1);
            LoadItemTypes(1);
            LoadUnits(1);
            LoadItems(1);
            LoadItemReport(1);

            var itemReport = Repository.OfType<ItemReport>().GetNullableByID(1);
            var item = GetValid(null);
            item.Reports = new List<ItemReport>();
            item.Reports.Add(itemReport);
            itemReport.Item = item;
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


        #endregion Report Tests

        #region Templates Collection Tests

        #region Invalid Tests
        /// <summary>
        /// Tests the Templates with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestTemplatesWithNullValueDoesNotSave()
        {
            Item item = null;
            try
            {
                #region Arrange
                item = GetValid(9);
                item.Templates = null;
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
                results.AssertErrorsAre("Templates: may not be empty");
                Assert.IsTrue(item.IsTransient());
                Assert.IsFalse(item.IsValid());
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestTemplatesWithMoreThanOneValueDoesNotSave()
        {
            Item item = null;
            try
            {
                #region Arrange
                var templates = new List<Template>();
                for (int i = 0; i < 5; i++)
                {
                    templates.Add(CreateValidEntities.Template(i + 1));
                    templates[i].Item = null;
                }
                item = GetValid(9);
                item.Templates = new List<Template>();
                item.Templates.Add(templates[1]);
                item.Templates.Add(templates[2]);
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
                results.AssertErrorsAre("Templates: size must be between 0 and 1");
                Assert.IsTrue(item.IsTransient());
                Assert.IsFalse(item.IsValid());
                throw;
            }
        }

        #endregion Invalid Tests

        #region Valid Tests
        /// <summary>
        /// Tests the Templates with empty list saves.
        /// </summary>
        [TestMethod]
        public void TestsTemplatesWithEmptyListSaves()
        {
            #region Arrange
            var item = GetValid(9);
            item.Templates = new List<Template>();
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
        /// Tests the Templates with value saves. (Done through Template, so we don't really need this)
        /// </summary>
        [TestMethod]
        public void TestTemplatesWithValueSaves()
        {
            #region Arrange
            var template = CreateValidEntities.Template(1);
            var item = GetValid(9);
            item.Templates = new List<Template>();
            item.Templates.Add(template);
            template.Item = item;
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

        #region CRUD Tests

        /// <summary>
        /// Tests the templates cascades with remove.
        /// </summary>
        [TestMethod]
        public void TestTemplatesCascadesWithRemove()
        {
            #region Arrange
            var templates = new List<Template>();
            for (int i = 0; i < 5; i++)
            {
                templates.Add(CreateValidEntities.Template(i + 1));
                templates[i].Item = null;
            }

            Assert.AreEqual(0, Repository.OfType<Template>().GetAll().Count);

            var item = GetValid(9);
            item.Templates = new List<Template>();
            item.Templates.Add(templates[1]);
            templates[1].Item = item;

            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            Assert.AreEqual(1, Repository.OfType<Template>().GetAll().Count);
            #endregion Arrange

            #region Act
            item.Templates.Remove(templates[1]);
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            Assert.AreEqual(0, Repository.OfType<Template>().GetAll().Count);

            #endregion Assert
        }

        /// <summary>
        /// Tests the Templates cascades with remove item.
        /// </summary>
        [TestMethod]
        public void TestTemplatesCascadesWithRemoveItem()
        {
            #region Arrange
            var templates = new List<Template>();
            for (int i = 0; i < 5; i++)
            {
                templates.Add(CreateValidEntities.Template(i + 1));
                templates[i].Item = null;
            }

            Assert.AreEqual(0, Repository.OfType<Template>().GetAll().Count);

            var item = GetValid(9);
            item.Templates = new List<Template>();
            item.Templates.Add(templates[1]);
            //item.Templates.Add(templates[2]);
            //item.Templates.Add(templates[4]);

            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.EnsurePersistent(item);
            ItemRepository.DbContext.CommitTransaction();

            #endregion Arrange

            #region Act
            ItemRepository.DbContext.BeginTransaction();
            ItemRepository.Remove(item);
            ItemRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(item.IsTransient());
            Assert.IsTrue(item.IsValid());
            Assert.AreEqual(0, Repository.OfType<Template>().GetAll().Count);
            #endregion Assert
        }
        #endregion CRUD Tests

        #endregion Templates Collection Tests

        #region Template Tests

        /// <summary>
        /// Tests the set template when templates is null adds template.
        /// </summary>
        [TestMethod]
        public void TestSetTemplateWhenTemplatesIsNullAddsTemplate()
        {
            #region Arrange
            var item = CreateValidEntities.Item(1);
            item.Templates = null;
            var template = CreateValidEntities.Template(1);
            #endregion Arrange

            #region Act
            item.Template = template;
            #endregion Act

            #region Assert
            Assert.IsNotNull(item.Template);
            Assert.IsNotNull(item.Templates);
            Assert.AreEqual(1, item.Templates.Count);
            Assert.AreSame(template, item.Templates.First());
            #endregion Assert		
        }

        /// <summary>
        /// Tests the set template when templates is empty list adds template.
        /// </summary>
        [TestMethod]
        public void TestSetTemplateWhenTemplatesIsEmptyListAddsTemplate()
        {
            #region Arrange
            var item = CreateValidEntities.Item(1);
            item.Templates = new List<Template>();
            var template = CreateValidEntities.Template(1);
            Assert.IsNotNull(item.Templates);
            Assert.AreEqual(0, item.Templates.Count);
            #endregion Arrange

            #region Act
            item.Template = template;
            #endregion Act

            #region Assert
            Assert.IsNotNull(item.Template);
            Assert.IsNotNull(item.Templates);
            Assert.AreEqual(1, item.Templates.Count);
            Assert.AreSame(template, item.Templates.First());
            #endregion Assert
        }


        /// <summary>
        /// Tests the set template replaces exiting template.
        /// </summary>
        [TestMethod]
        public void TestSetTemplateReplacesExitingTemplate()
        {
            #region Arrange
            var templates = new List<Template>();
            for (int i = 0; i < 6; i++)
            {
                templates.Add(CreateValidEntities.Template(i+1));
            }
            var item = CreateValidEntities.Item(1);
            #endregion Arrange

            #region Act

            for (int i = 0; i < 5; i++)
            {
                item.Template = templates[i];   
            }
            #endregion Act

            #region Assert
            Assert.IsNotNull(item.Template);
            Assert.IsNotNull(item.Templates);
            Assert.AreEqual(1, item.Templates.Count);
            Assert.AreEqual(templates[4].Text, item.Templates.First().Text);
            Assert.AreNotSame(templates[4], item.Templates.First());
            #endregion Assert		
        }

        #endregion Template Tests

        #region Sold Tests 
        //TODO: Update test to reflect Transaction.IsActive change

        /// <summary>
        /// Tests that sold returns expected results.
        /// </summary>
        [TestMethod]
        public void TestSoldReturnsExpectedResults()
        {
            #region Arrange

            var transactions = new List<Transaction>();
            for (int i = 0; i < 5; i++)
            {
                transactions.Add(CreateValidEntities.Transaction(i+1));
            }
            transactions[0].Quantity = 1;
            transactions[1].Quantity = 10;
            transactions[2].Quantity = 100;
            transactions[3].Quantity = 1000;
            transactions[4].Quantity = 10000;

            var item = CreateValidEntities.Item(1);
            for (int i = 0; i < 4; i++)
            {
                item.Transactions.Add(transactions[i]);
            }
            item.Quantity = 100000;

            #endregion Arrange

            #region Act
            var result = item.Sold;
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1111, result);
            #endregion Assert		
        }
        

        #endregion Sold Tests

        #region ItemTags Tests

        /// <summary>
        /// Tests the item tags prevents tag with invalid data from saving.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestItemTagsPreventsTagWithInvalidDataFromSaving()
        {
            Item item = null;
            try
            {
                #region Arrange
                var tag1 = CreateValidEntities.Tag(1);
                var tag2 = CreateValidEntities.Tag(2);
                var tag3 = CreateValidEntities.Tag(3);
                tag2.Name = " "; //Invalid

                item = GetValid(null);
                item.Tags = new List<Tag>();
                item.AddTag(tag1);
                item.AddTag(tag2);
                item.AddTag(tag3);
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
                results.AssertErrorsAre("ItemTags: One or more tags is not valid");
                Assert.IsTrue(item.IsTransient());
                Assert.IsFalse(item.IsValid());
                throw;
            }	
        }


        #endregion ItemTags Tests

        #region IsAvailableForReg Tests
        [TestMethod]
        public void TestIsAvailableForRegWhenSoldGreaterThanQuantityReturnsFalse()
        {
            #region Arrange
            var transactions = new List<Transaction>();
            for (int i = 0; i < 5; i++)
            {
                transactions.Add(CreateValidEntities.Transaction(i + 1));
            }
            transactions[0].Quantity = 1;
            transactions[1].Quantity = 10;

            var item = CreateValidEntities.Item(1);
            for (int i = 0; i < 2; i++)
            {
                item.Transactions.Add(transactions[i]);
            }
            item.Quantity = 10;
            var fakeDate = new DateTime(2010, 02, 01);
            SystemTime.Now = () => fakeDate;
            item.Expiration = null;
            item.Available = true;

            #endregion Arrange

            #region Act
            var result = item.IsAvailableForReg;
            #endregion Act

            #region Assert
            Assert.IsTrue(item.Sold > item.Quantity);
            Assert.IsFalse(result);
            #endregion Assert
        }

        [TestMethod]
        public void TestIsAvailableForRegWhenNotAvailableReturnsFalse()
        {
            #region Arrange
            var transactions = new List<Transaction>();
            for (int i = 0; i < 5; i++)
            {
                transactions.Add(CreateValidEntities.Transaction(i + 1));
            }
            transactions[0].Quantity = 1;
            transactions[1].Quantity = 10;

            var item = CreateValidEntities.Item(1);
            for (int i = 0; i < 2; i++)
            {
                item.Transactions.Add(transactions[i]);
            }
            item.Quantity = 20;
            var fakeDate = new DateTime(2010, 02, 01);
            SystemTime.Now = () => fakeDate;
            item.Expiration = new DateTime(2010,02,02);
            item.Available = false;

            #endregion Arrange

            #region Act
            var result = item.IsAvailableForReg;
            #endregion Act

            #region Assert
            Assert.IsTrue(item.Sold < item.Quantity);
            Assert.IsFalse(result);
            #endregion Assert
        }

        [TestMethod]
        public void TestIsAvailableForRegWhenExpiredReturnsFalse()
        {
            #region Arrange
            var transactions = new List<Transaction>();
            for (int i = 0; i < 5; i++)
            {
                transactions.Add(CreateValidEntities.Transaction(i + 1));
            }
            transactions[0].Quantity = 1;
            transactions[1].Quantity = 10;

            var item = CreateValidEntities.Item(1);
            for (int i = 0; i < 2; i++)
            {
                item.Transactions.Add(transactions[i]);
            }
            item.Quantity = 20;
            var fakeDate = new DateTime(2010, 02, 01);
            SystemTime.Now = () => fakeDate;
            item.Expiration = new DateTime(2010, 01, 30);
            item.Available = true;

            #endregion Arrange

            #region Act
            var result = item.IsAvailableForReg;
            #endregion Act

            #region Assert
            Assert.IsTrue(item.Sold < item.Quantity);
            Assert.IsFalse(result);
            #endregion Assert
        }
        [TestMethod]
        public void TestIsAvailableForRegWhenAllValuesInvalidReturnsFalse()
        {
            #region Arrange
            var transactions = new List<Transaction>();
            for (int i = 0; i < 5; i++)
            {
                transactions.Add(CreateValidEntities.Transaction(i + 1));
            }
            transactions[0].Quantity = 1;
            transactions[1].Quantity = 10;

            var item = CreateValidEntities.Item(1);
            for (int i = 0; i < 2; i++)
            {
                item.Transactions.Add(transactions[i]);
            }
            item.Quantity = 11;
            var fakeDate = new DateTime(2010, 02, 01);
            SystemTime.Now = () => fakeDate;
            item.Expiration = new DateTime(2010, 01, 30);
            item.Available = false;

            #endregion Arrange

            #region Act
            var result = item.IsAvailableForReg;
            #endregion Act

            #region Assert
            Assert.IsTrue(item.Sold == item.Quantity);
            Assert.IsFalse(result);
            #endregion Assert
        }

        [TestMethod]
        public void TestIsAvailableForRegWhenSoldEqualsQuantityReturnsFalse()
        {
            #region Arrange
            var transactions = new List<Transaction>();
            for (int i = 0; i < 5; i++)
            {
                transactions.Add(CreateValidEntities.Transaction(i + 1));
            }
            transactions[0].Quantity = 1;
            transactions[1].Quantity = 10;

            var item = CreateValidEntities.Item(1);
            for (int i = 0; i < 2; i++)
            {
                item.Transactions.Add(transactions[i]);
            }
            item.Quantity = 11;
            var fakeDate = new DateTime(2010, 02, 01);
            SystemTime.Now = () => fakeDate;
            item.Expiration = null;
            item.Available = true;

            #endregion Arrange

            #region Act
            var result = item.IsAvailableForReg;
            #endregion Act

            #region Assert
            Assert.AreEqual(item.Sold, item.Quantity);
            Assert.IsFalse(result);
            #endregion Assert
        }


        [TestMethod]
        public void TestIsAvailableForRegWhenSoldLessThanQuantityReturnsTrue1()
        {
            #region Arrange
            var transactions = new List<Transaction>();
            for (int i = 0; i < 5; i++)
            {
                transactions.Add(CreateValidEntities.Transaction(i + 1));
            }
            transactions[0].Quantity = 1;
            transactions[1].Quantity = 10;

            var item = CreateValidEntities.Item(1);
            for (int i = 0; i < 2; i++)
            {
                item.Transactions.Add(transactions[i]);
            }
            item.Quantity = 12;
            var fakeDate = new DateTime(2010, 02, 01);
            SystemTime.Now = () => fakeDate;
            item.Expiration = null;
            item.Available = true;

            #endregion Arrange

            #region Act
            var result = item.IsAvailableForReg;
            #endregion Act

            #region Assert
            Assert.IsTrue(item.Sold < item.Quantity);
            Assert.IsTrue(result);
            #endregion Assert
        }

        [TestMethod]
        public void TestIsAvailableForRegWhenSoldLessThanQuantityReturnsTrue2()
        {
            #region Arrange
            var transactions = new List<Transaction>();
            for (int i = 0; i < 5; i++)
            {
                transactions.Add(CreateValidEntities.Transaction(i + 1));
            }
            transactions[0].Quantity = 1;
            transactions[1].Quantity = 10;

            var item = CreateValidEntities.Item(1);
            for (int i = 0; i < 2; i++)
            {
                item.Transactions.Add(transactions[i]);
            }
            item.Quantity = 12;
            var fakeDate = new DateTime(2010, 02, 01);
            SystemTime.Now = () => fakeDate;
            item.Expiration = fakeDate.AddDays(1);
            item.Available = true;

            #endregion Arrange

            #region Act
            var result = item.IsAvailableForReg;
            #endregion Act

            #region Assert
            Assert.IsTrue(item.Sold < item.Quantity);
            Assert.IsTrue(result);
            #endregion Assert
        }


        #endregion IsAvailableForReg Tests

        #region ItemCoupons Tests

        /// <summary>
        /// Tests the item coupons prevents coupon with discount amount greater than cost per item from saving.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestItemCouponsPreventsCouponWithDiscountAmountGreaterThanCostPerItemFromSaving()
        {
            Item item = null;
            try
            {
                #region Arrange
                item = GetValid(null);
                item.CostPerItem = 9.99m;
                var coupon = CreateValidEntities.Coupon(3);
                coupon.DiscountAmount = 10.00m;
                item.AddCoupon(CreateValidEntities.Coupon(1));
                item.AddCoupon(coupon);
                item.AddCoupon(CreateValidEntities.Coupon(2));
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
                results.AssertErrorsAre("ItemCoupons: One or more active coupons has a discount amount greater than the cost per item");
                Assert.IsTrue(item.IsTransient());
                Assert.IsFalse(item.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the item coupons with valid active coupons saves.
        /// </summary>
        [TestMethod]
        public void TestItemCouponsWithValidActiveCouponsSaves()
        {
            #region Arrange
            var item = GetValid(null);
            item.CostPerItem = 9.99m;
            var coupon = CreateValidEntities.Coupon(3);
            coupon.DiscountAmount = 10.00m;
            coupon.IsActive = false;
            item.AddCoupon(CreateValidEntities.Coupon(1));
            item.AddCoupon(coupon);
            item.AddCoupon(CreateValidEntities.Coupon(2));
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
        /// Tests the item coupons with valid data saves.
        /// </summary>
        [TestMethod]
        public void TestItemCouponsWithValidDataSaves1()
        {
            #region Arrange
            var item = GetValid(null);
            item.CostPerItem = 9.99m;
            var coupon = CreateValidEntities.Coupon(3);
            coupon.DiscountAmount = 1m;
            item.AddCoupon(CreateValidEntities.Coupon(1));
            item.AddCoupon(coupon);
            item.AddCoupon(CreateValidEntities.Coupon(2));
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
        /// Tests the item coupons with valid data saves.
        /// </summary>
        [TestMethod]
        public void TestItemCouponsWithValidDataSaves2()
        {
            #region Arrange
            var item = GetValid(null);
            item.CostPerItem = 9.99m;
            var coupon = CreateValidEntities.Coupon(3);
            coupon.DiscountAmount = 1m;
            item.AddCoupon(CreateValidEntities.Coupon(1));
            item.AddCoupon(coupon);
            item.AddCoupon(CreateValidEntities.Coupon(2));
            item.Coupons.ElementAt(0).IsActive = false;
            item.Coupons.ElementAt(1).IsActive = false;
            item.Coupons.ElementAt(2).IsActive = false;
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

        #endregion ItemCoupons Tests
    }
}
