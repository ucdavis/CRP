using System.Collections.Generic;
using System.Linq;
using CRP.Core.Abstractions;
using CRP.Core.Domain;
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

        /// <summary>
        /// Creates the specified View Model.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="openIdRepository">The open id repository.</param>
        /// <param name="item">The item.</param>
        /// <param name="openIdUser">The open id user.</param>
        /// <returns></returns>
        public static ItemDetailViewModel Create(IRepository repository, IRepositoryWithTypedId<OpenIdUser, string> openIdRepository, Item item, string openIdUser)
        {
            Check.Require(repository != null, "Repository is required.");
            Check.Require(openIdRepository != null, "Repository is required.");

            var viewModel = new ItemDetailViewModel() {Item = item};

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
                viewModel.OpenIdUser = openIdRepository.GetNullableByID(openIdUser);
            }

            return viewModel;
        }
    }

    public class UserItemDetailViewModel
    {
        public Item Item { get; set; }
        public IEnumerable<ItemReport> Reports { get; set; }

        public static UserItemDetailViewModel Create(IRepository repository, Item item)
        {
            Check.Require(repository != null, "Repository is required.");

            var viewModel = new UserItemDetailViewModel()
                                {
                                    Item = item//,
                                    //SystemReports = repository.OfType<ItemReport>().Queryable.Where(a => a.SystemReusable).Union(repository.OfType<ItemReport>().Queryable.Where(b => !b.SystemReusable && b.Item == item).ToList()).ToList()
                                };

            var systemReports = repository.OfType<ItemReport>().Queryable.Where(a => a.SystemReusable).ToList();
            var userReports = repository.OfType<ItemReport>().Queryable.Where(b => !b.SystemReusable && b.Item == item).ToList();

            viewModel.Reports = systemReports.Union(userReports);

            return viewModel;
        }
    }

    public class ItemTransactionViewModel
    {
        public ItemTransactionViewModel(Item item, OpenIdUser openIDUser, int quantity, IEnumerable<ItemTransactionAnswer> answers)
        {
            Item  = item;
            OpenIDUser = openIDUser;
            Quantity = quantity;
            Answers = answers;
        }

        public Item Item{ get; set; }
        public OpenIdUser OpenIDUser { get; set; }
        public int Quantity { get; set; }
        public IEnumerable<ItemTransactionAnswer> Answers { get; set; }
    }

    public class ItemQuestionViewModel
    {
        public ItemQuestionViewModel(Question question, OpenIdUser openIDUser, string answer)
        {
            Question = question;
            OpenIDUser = openIDUser;
            Answer = answer;
        }

        public Question Question { get; set; }
        public OpenIdUser OpenIDUser { get; set; }
        public string Answer { get; set; }
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
