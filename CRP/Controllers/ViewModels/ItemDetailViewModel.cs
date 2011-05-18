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

        /// <summary>
        /// Creates the specified View Model.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="openIdRepository">The open id repository.</param>
        /// <param name="item">The item.</param>
        /// <param name="openIdUser">The open id user.</param>
        /// <returns></returns>
        public static ItemDetailViewModel Create(IRepository repository, IRepositoryWithTypedId<OpenIdUser, string> openIdRepository, Item item, string openIdUser, string referenceId)
        {
            Check.Require(repository != null, "Repository is required.");
            Check.Require(openIdRepository != null, "Repository is required.");

            var viewModel = new ItemDetailViewModel() {Item = item, ReferenceId = referenceId};

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
        public string Fid { get; set; }
        public IList<CheckName> CheckName { get; set; }

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
            //viewModel.Fid = string.Format(" FID={0}", ConfigurationManager.AppSettings["TouchNetFid"]);
            viewModel.Fid = string.Format(" FID={0}", string.IsNullOrEmpty(item.TouchnetFID) ? string.Empty : item.TouchnetFID);

            foreach (var transaction in viewModel.Item.Transactions.Where(a => a.Check && a.ParentTransaction == null && a.IsActive))
            {
                var checkName = new CheckName();
                checkName.TransactionNumber = transaction.TransactionNumber;
                checkName.LastName = transaction.TransactionAnswers.Where(a =>
                        a.QuestionSet.Name == StaticValues.QuestionSet_ContactInformation &&
                        a.Question.Name == StaticValues.Question_LastName).FirstOrDefault().Answer;
                checkName.FirstName = transaction.TransactionAnswers.Where(a =>
                        a.QuestionSet.Name == StaticValues.QuestionSet_ContactInformation &&
                        a.Question.Name == StaticValues.Question_FirstName).FirstOrDefault().Answer;

                viewModel.CheckName.Add(checkName);
            }

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
