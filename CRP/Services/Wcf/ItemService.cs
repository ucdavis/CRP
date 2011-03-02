using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CRP.Controllers.Services;
using CRP.Core.Domain;
using CRP.Core.Resources;
using Microsoft.Practices.ServiceLocation;
using UCDArch.Core.PersistanceSupport;

namespace CRP.Services.Wcf
{
    public class ItemService : IItemService
    {
        private readonly ICouponService _couponService;

        public ItemService()
        {
            _couponService = new CouponService(RepositoryFactory.ItemRepository, RepositoryFactory.CouponRepository);
        }

        public string CreateCoupon(int itemId, string email, bool unlimited, DateTime? expiration, decimal discountAmount, int? maxUsage, int? maxQuantity)
        {
            var item = RepositoryFactory.ItemRepository.GetNullableById(itemId);
            if (item == null) throw new ArgumentException("Item", string.Format("Unable to load item with item id ({0})", itemId));

            return _couponService.Create(item, email, unlimited, expiration, discountAmount, "web service", maxUsage, maxQuantity);
        }

        public bool CancelCoupon(string couponCode)
        {
            var coupon = RepositoryFactory.CouponRepository.Queryable.Where(a => a.Code == couponCode).FirstOrDefault();
            if (coupon == null) throw new ArgumentException("Coupon", string.Format("Unable to load item with coupon code ({0})", couponCode));

            return _couponService.Deactivate(coupon);
        }

        public ServiceTransaction GetRegistration(string registrationId)
        {
            // find the transaction
            var transaction = RepositoryFactory.TransactionAnswerRepository.Queryable
                                    .Where(a => a.Question.Name == "Registration Id" && a.Answer.Trim() == registrationId.Trim())
                                    .Select(a => a.Transaction).FirstOrDefault();

            if (transaction == null) throw new ArgumentException("Transaction", string.Format("Unable to load transaction with registration id ({0})", registrationId));

            return GetRegistration(transaction.Id);
        }

        public ServiceTransaction GetRegistration(int transactionId)
        {
            var transaction = RepositoryFactory.TransactionRepository.GetNullableById(transactionId);

            // check transaction is valid)
            if (transaction == null) throw new ArgumentException("Transaction", string.Format("Unable to load transaction with transaction id ({0})", transactionId));

            // find the answers for firstname and last name
            var lastNameQ = transaction.TransactionAnswers.Where(a => a.Question.Name == StaticValues.Question_LastName).FirstOrDefault();
            var firstNameQ = transaction.TransactionAnswers.Where(a => a.Question.Name == StaticValues.Question_FirstName).FirstOrDefault();

            // populate the service objects
            var serviceTransaction = new ServiceTransaction()
            {
                DateRegistered = transaction.TransactionDate,
                FirstName = firstNameQ != null ? firstNameQ.Answer : string.Empty,
                LastName = lastNameQ != null ? lastNameQ.Answer : string.Empty,
                Paid = transaction.Paid
            };

            // load all the questions
            serviceTransaction.ServiceQuestions = transaction.TransactionAnswers.Select(a => new ServiceQuestion(a.Question.Name, a.Answer));
            serviceTransaction.ServiceQuestions = transaction.QuantityAnswers.Select(a => new ServiceQuestion(a.Question.Name, a.Answer, a.QuantityId));

            return serviceTransaction;
        }

        public ServiceTransaction[] GetRegistrations(int itemId)
        {
            var item = RepositoryFactory.ItemRepository.GetNullableById(itemId);
            if (item == null) throw new ArgumentException("Item", string.Format("Unable to load item with item id ({0})", itemId));

            var serviceTransactions = new List<ServiceTransaction>();

            foreach (var a in item.Transactions)
            {
                serviceTransactions.Add(GetRegistration(a.Id));
            }

            return serviceTransactions.ToArray();
        }
    }

    public class RepositoryFactory
    {
        // Private constructor prevents instantiation from other classes
        private RepositoryFactory() { }

        private static class CouponSingletonHolder
        {
            public static readonly IRepository<Coupon> Instance = ServiceLocator.Current.GetInstance<IRepository<Coupon>>();
        }

        private static class ItemSingletonHolder
        {
            public static readonly IRepository<Item> Instance = ServiceLocator.Current.GetInstance<IRepository<Item>>();
        }

        private static class TransactionAnswerSingletonHolder
        {
            public static readonly IRepository<TransactionAnswer> Instance = ServiceLocator.Current.GetInstance<IRepository<TransactionAnswer>>();
        }

        private static class TransactionSingletonHolder
        {
            public static readonly IRepository<Transaction> Instance = ServiceLocator.Current.GetInstance<IRepository<Transaction>>();
        }

        public static IRepository<Coupon> CouponRepository
        {
            get { return CouponSingletonHolder.Instance; }
        }

        public static IRepository<Item> ItemRepository
        {
            get { return ItemSingletonHolder.Instance; }
        }

        public static IRepository<TransactionAnswer> TransactionAnswerRepository
        {
            get { return TransactionAnswerSingletonHolder.Instance; }
        }

        public static IRepository<Transaction> TransactionRepository
        {
            get { return TransactionSingletonHolder.Instance; }
        }
    }
}