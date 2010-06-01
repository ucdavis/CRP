using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using CRP.Controllers.Helpers;
using CRP.Core.Domain;
using UCDArch.Core.PersistanceSupport;
using Check = UCDArch.Core.Utils.Check;

namespace CRP.Controllers.ViewModels
{
    public class HelpTopicViewModel
    {
        public bool IsUserAuthorized { get; set; }
        public bool IsUserAdmin { get; set; }
        public IEnumerable<HelpTopic> HelpTopics { get; set; }

        public static HelpTopicViewModel Create(IRepository repository, IPrincipal currentUser)
        {
            Check.Require(repository != null, "Repository is required.");

            var viewModel = new HelpTopicViewModel();
            viewModel.IsUserAuthorized = currentUser.IsInRole(RoleNames.Admin) || currentUser.IsInRole(RoleNames.User);
            viewModel.IsUserAdmin = currentUser.IsInRole(RoleNames.Admin);

            if(viewModel.IsUserAdmin)
            {
                viewModel.HelpTopics = repository.OfType<HelpTopic>().Queryable;
            }
            else if(viewModel.IsUserAuthorized)
            {
                viewModel.HelpTopics = repository.OfType<HelpTopic>().Queryable.Where(a => a.IsActive);
            }
            else
            {
                viewModel.HelpTopics = repository.OfType<HelpTopic>().Queryable.Where(a => a.AvailableToPublic && a.IsActive).OrderByDescending(a => a.NumberOfReads);
            }

            return viewModel;
        }
    }
}
