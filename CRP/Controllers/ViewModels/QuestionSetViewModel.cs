using System.Collections.Generic;
using System.Linq;
using CRP.Core.Domain;
using UCDArch.Core.PersistanceSupport;
using Check=UCDArch.Core.Utils.Check;

namespace CRP.Controllers.ViewModels
{
    public class QuestionSetViewModel
    {
        public static QuestionSetViewModel Create(IRepository repository)
        {
            Check.Require(repository != null, "Repository is required.");

            var viewModel = new QuestionSetViewModel() {
                QuestionTypes = repository.OfType<QuestionType>().GetAll(),
                Schools = repository.OfType<School>().GetAll()
            };

            return viewModel;
        }

        public IEnumerable<QuestionType> QuestionTypes { get; set; }
        public IEnumerable<School> Schools { get; set; }
        public QuestionSet QuestionSet { get; set; }
        public Item Item { get; set; }
        public ItemType ItemType { get; set; }
    }

    public class QuestionSetLinkViewModel
    {
        public static QuestionSetLinkViewModel Create(IRepository repository, string loginId)
        {
            Check.Require(repository != null, "Repository is required.");

            // get the user's colleges
            var user = repository.OfType<User>().Queryable.Where(x => x.LoginID == loginId).FirstOrDefault();
            var colleges = (from x in user.Units
                            select x.School).ToList();

            var query = from x in repository.OfType<QuestionSet>().Queryable
                        where x.SystemReusable
                            || colleges.Contains(x.School)
                            || x.UserReusable && x.User.LoginID == loginId
                        select x;

            var viewModel = new QuestionSetLinkViewModel() { QuestionSets = query };

            return viewModel;
        }

        public IQueryable<QuestionSet> QuestionSets { get; set;}
        public int ItemTypeId { get; set; }
        public bool Transaction { get; set; }
        public bool Quantity { get; set; }
    }
}
