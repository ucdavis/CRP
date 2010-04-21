using System.Collections.Generic;
using System.Linq;
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
        public ItemTransactionViewModel(Item item, OpenIdUser openIDUser)
        {
            Item  = item;
            OpenIDUser = openIDUser;
        }

        public Item Item{ get; set; }
        public OpenIdUser OpenIDUser { get; set; }
    }

    public class ItemQuestionViewModel
    {
        public ItemQuestionViewModel(Question question, OpenIdUser openIDUser)
        {
            Question = question;
            OpenIDUser = openIDUser;
        }

        public Question Question { get; set; }
        public OpenIdUser OpenIDUser { get; set; }
    }
}
