using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web.Mvc;
using CRP.Core.Domain;
using CRP.Core.Resources;
using UCDArch.Core.PersistanceSupport;
using Check=UCDArch.Core.Utils.Check;

namespace CRP.Controllers.ViewModels
{
    public class QuestionSetViewModel
    {
        public static QuestionSetViewModel Create(IRepository repository, IPrincipal principal, IRepositoryWithTypedId<School, string> schoolRepository)
        {
            Check.Require(repository != null, "Repository is required.");
            Check.Require(schoolRepository != null, "School repository is required.");

            var user = repository.OfType<User>().Queryable.Where(a => a.LoginID == principal.Identity.Name).FirstOrDefault();

            var viewModel = new QuestionSetViewModel() {
                QuestionTypes = repository.OfType<QuestionType>().GetAll()
                //Schools = schoolRepository.GetAll()
            };

            if (user != null)
            {
                viewModel.Schools = user.Units.Select(a => a.School).Distinct();
            }

            return viewModel;
        }

        public IEnumerable<QuestionType> QuestionTypes { get; set; }
        public IEnumerable<School> Schools { get; set; }
        public QuestionSet QuestionSet { get; set; }
        public Item Item { get; set; }
        public ItemType ItemType { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsSchoolAdmin { get; set; }
        public bool IsUser { get; set; }
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
                        where (x.SystemReusable
                            || colleges.Contains(x.School)
                            || x.UserReusable && x.User.LoginID == loginId)
                            && x.Name != StaticValues.QuestionSet_ContactInformation
                        select x;

            var viewModel = new QuestionSetLinkViewModel() { QuestionSets = query };

            return viewModel;
        }

        public IQueryable<QuestionSet> QuestionSets { get; set;}
        public int ItemTypeId { get; set; }
        public int ItemId { get; set; }
        public bool Transaction { get; set; }
        public bool Quantity { get; set; }
    }
}
