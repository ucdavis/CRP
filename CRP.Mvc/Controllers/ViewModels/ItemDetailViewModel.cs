using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using CRP.Core.Abstractions;
using CRP.Core.Domain;
using CRP.Core.Resources;
using UCDArch.Core.PersistanceSupport;
using Check=UCDArch.Core.Utils.Check;

namespace CRP.Controllers.ViewModels
{
    public class ItemDetailViewModel
    {
        public Item Item { get; set; }
        public DisplayProfile DisplayProfile { get; set; }
        public OpenIdUser OpenIdUser { get; set; }
        public Transaction Transaction { get; set; }
        public int Quantity { get; set; }
        public IEnumerable<ItemTransactionAnswer> Answers { get; set; }
        public bool CreditPayment { get; set; }
        public bool CheckPayment { get; set; }
        public decimal TotalAmountToRedisplay { get; set; }
        public decimal CouponAmountToDisplay { get; set; }
        public decimal CouponTotalDiscountToDisplay { get; set; }
        public bool HasMapPins { get; set; }

        public string ReferenceId { get; set; }
        public string Coupon { get; set; }
        public string Password { get; set; }

        /// <summary>
        /// Creates the specified View Model.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="openIdRepository">The open id repository.</param>
        /// <param name="item">The item.</param>
        /// <param name="openIdUser">The open id user.</param>
        /// <param name="referenceId"></param>
        /// <param name="coupon"></param>
        /// <returns></returns>
        public static ItemDetailViewModel Create(IRepository repository, IRepositoryWithTypedId<OpenIdUser, string> openIdRepository, Item item, string openIdUser, string referenceId, string coupon, string password)
        {
            Check.Require(repository != null, "Repository is required.");
            Check.Require(openIdRepository != null, "Repository is required.");

            var viewModel = new ItemDetailViewModel() {Item = item, ReferenceId = referenceId, Coupon = coupon, Password = password};

            // get the proper display profile
            var unit = item.Unit;

            viewModel.DisplayProfile = repository.OfType<DisplayProfile>().Queryable.Where(a => a.Unit == unit).FirstOrDefault();

            if (viewModel.DisplayProfile == null)
            {
                // get the college profile
                viewModel.DisplayProfile = repository.OfType<DisplayProfile>().Queryable.Where(a => a.School == unit.School && a.SchoolMaster).FirstOrDefault();
            }

            if (!string.IsNullOrEmpty(openIdUser))
            {
                viewModel.OpenIdUser = openIdRepository.GetNullableById(openIdUser);
            }
            if (viewModel.Item != null && viewModel.Item.MapPins != null && viewModel.Item.MapPins.Count > 0)
            {
                viewModel.HasMapPins = true;
            }
            else
            {
                viewModel.HasMapPins = false;
            }

            return viewModel;
        }
    }

    public class UserItemDetailViewModel
    {
        public Item Item { get; set; }
        public IEnumerable<ItemReport> Reports { get; set; }
        public FinancialAccount FinancialAccount { get; set; }
        public IList<CheckName> CheckName { get; set; }

        public IList<Transaction> Transactions { get; set; }

        public static UserItemDetailViewModel Create(IRepository repository, Item item)
        {
            Check.Require(repository != null, "Repository is required.");

            var viewModel = new UserItemDetailViewModel()
                                {
                                    Item = item,
                                    CheckName = new List<CheckName>()//,
                                    //SystemReports = repository.OfType<ItemReport>().Queryable.Where(a => a.SystemReusable).Union(repository.OfType<ItemReport>().Queryable.Where(b => !b.SystemReusable && b.Item == item).ToList()).ToList()
                                };

            var systemReports = repository.OfType<ItemReport>().Queryable.Where(a => a.SystemReusable).ToList();
            var userReports = repository.OfType<ItemReport>().Queryable.Where(b => !b.SystemReusable && b.Item == item).ToList();

            viewModel.Reports = systemReports.Union(userReports);

            viewModel.FinancialAccount = item.FinancialAccount;

            var transactions = repository.OfType<Transaction>().Queryable.Where(a => a.Item.Id == item.Id && a.ParentTransaction == null).ToArray();
            var transIds = transactions.Select(a => a.Id).ToArray();
            var childTransactions = repository.OfType<Transaction>().Queryable
                .Where(a => transIds.Contains(a.ParentTransaction.Id)).ToArray();
            var paymentLogs = repository.OfType<PaymentLog>().Queryable.Where(a => transIds.Contains(a.Transaction.Id))
                .ToArray();

            var allNames = repository.OfType<TransactionAnswer>().Queryable.Where(a =>
                a.Transaction.Item.Id == item.Id && a.QuestionSet.Name == StaticValues.QuestionSet_ContactInformation &&
                (a.Question.Name == StaticValues.Question_LastName ||
                 a.Question.Name == StaticValues.Question_FirstName)).Select(a =>
                new {TransactionId = a.Transaction.Id, QuestionName = a.Question.Name, Answer = a}).ToArray();

            foreach (var transaction in transactions.Where(a => a.IsActive))
            {
                transaction.ChildTransactions =
                    childTransactions.Where(a => a.ParentTransaction.Id == transaction.Id).ToArray();
                transaction.PaymentLogs = paymentLogs.Where(a => a.Transaction.Id == transaction.Id).ToArray();
                if (!transaction.Check)
                {
                    continue;
                }
                var checkName = new CheckName();
                checkName.TransactionNumber = transaction.TransactionNumber;

                var lastName = allNames.Where(a =>
                        a.TransactionId == transaction.Id && a.QuestionName == StaticValues.Question_LastName).Select(a => a.Answer)
                    .FirstOrDefault();

                var firstName = allNames.Where(a =>
                        a.TransactionId == transaction.Id && a.QuestionName == StaticValues.Question_FirstName).Select(a => a.Answer)
                    .FirstOrDefault();

                if (lastName != null)
                {
                    checkName.LastName = lastName.Answer;
                }
                else
                {
                    checkName.LastName = "!ERR!";
                }
                if (firstName != null)
                {
                    checkName.FirstName = firstName.Answer;
                }
                else
                {
                    checkName.FirstName = "!ERR!";
                }

                viewModel.CheckName.Add(checkName);
            }

            viewModel.Transactions = transactions;

            return viewModel;
        }
    }

    public class CheckName
    {
        public string TransactionNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName
        {
            get { return string.Format("{0}, {1}", LastName, FirstName); }
        }
    }

    public class ItemTransactionViewModel
    {
        public ItemTransactionViewModel(Item item, OpenIdUser openIDUser, int quantity, IEnumerable<ItemTransactionAnswer> answers, string referenceId)
        {
            Item  = item;
            OpenIDUser = openIDUser;
            Quantity = quantity;
            Answers = answers;
            ReferenceId = referenceId;
        }

        public Item Item{ get; set; }
        public OpenIdUser OpenIDUser { get; set; }
        public int Quantity { get; set; }
        public IEnumerable<ItemTransactionAnswer> Answers { get; set; }

        public string ReferenceId { get; set; }
    }

    public class ItemQuestionViewModel
    {
        public ItemQuestionViewModel(Question question, OpenIdUser openIDUser, string answer, bool disable)
        {
            Question = question;
            OpenIDUser = openIDUser;
            Answer = answer;
            Disable = disable;
        }

        public Question Question { get; set; }
        public OpenIdUser OpenIDUser { get; set; }
        public string Answer { get; set; }

        public bool Disable { get; set; }
    }

    public class ItemTransactionAnswer
    {
        public int QuestionId { get; set; }
        public int QuestionSetId { get; set; }
        public int QuantityIndex { get; set; }
        public string Answer { get; set; }

        /// <summary>
        /// If true, transaction, else quantity answer
        /// </summary>
        public bool Transaction{ get; set; }
    }
}
