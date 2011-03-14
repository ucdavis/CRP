using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;
using CRP.Controllers.Services;
using CRP.Core.Resources;

namespace CRP.Services.Wcf
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class ItemService : IItemService
    {
        private readonly ICouponService _couponService;

        public ItemService()
        {
            _couponService = new CouponService(RepositoryFactory.ItemRepository, RepositoryFactory.CouponRepository);
        }

        public string CreateCoupon(int itemId, string email, bool unlimited, DateTime? expiration, decimal discountAmount, int maxUsage, int? maxQuantity, CouponTypes couponTypes)
        {
            var item = RepositoryFactory.ItemRepository.GetNullableById(itemId);
            if (item == null) throw new ArgumentException("Item", string.Format("Unable to load item with item id ({0})", itemId));

            string couponType = string.Empty;
            switch(couponTypes)
            {
                case CouponTypes.Unlimited:
                    couponType = "Unlimited";
                    break;
                case CouponTypes.LimitedUsage:
                    couponType = "LimitedUsage";
                    break;
                case CouponTypes.SingleUsage:
                    couponType = "SingleUsage";
                    break;
            }

            return _couponService.Create(item, email, unlimited, expiration, discountAmount, "web service", maxUsage, maxQuantity, couponType);
        }

        public bool CancelCoupon(string couponCode)
        {
            var coupon = RepositoryFactory.CouponRepository.Queryable.Where(a => a.Code == couponCode).FirstOrDefault();
            if (coupon == null) throw new ArgumentException("Coupon", string.Format("Unable to load item with coupon code ({0})", couponCode));

            return _couponService.Deactivate(coupon);
        }

        public ServiceTransaction GetRegistrationByReference(string registrationId)
        {
            // find the transaction
            var transaction = RepositoryFactory.TransactionAnswerRepository.Queryable
                                    .Where(a => a.Question.Name == "Registration Id" && a.Answer.Trim() == registrationId.Trim())
                                    .Select(a => a.Transaction).FirstOrDefault();

            if (transaction == null) throw new ArgumentException("Transaction", string.Format("Unable to load transaction with registration id ({0})", registrationId));

            return GetRegistrationById(transaction.Id);
        }

        public ServiceTransaction GetRegistrationById(int transactionId)
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
                serviceTransactions.Add(GetRegistrationById(a.Id));
            }

            return serviceTransactions.ToArray();
        }
    }
}