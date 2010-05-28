
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Collections.Generic;
using CRP.Controllers.Helpers;
using CRP.Core.Domain;
using UCDArch.Core.PersistanceSupport;
using Check = UCDArch.Core.Utils.Check;

namespace CRP.Controllers.ViewModels
{
    public class HelpTopicViewModel
    {
        public bool IsUserAuthorized { get; set; }
        public IEnumerable<HelpTopic> HelpTopics { get; set; }

        public static HelpTopicViewModel Create(IRepository repository, IPrincipal currentUser)
        {
            Check.Require(repository != null, "Repository is required.");

            var viewModel = new HelpTopicViewModel();
            viewModel.IsUserAuthorized = currentUser.IsInRole(RoleNames.Admin) || currentUser.IsInRole(RoleNames.User);

            if(viewModel.IsUserAuthorized)
            {
                viewModel.HelpTopics = repository.OfType<HelpTopic>().Queryable;
            }
            else
            {
                viewModel.HelpTopics = repository.OfType<HelpTopic>().Queryable.Where(a => a.AvailableToPublic);
            }

            return viewModel;
        }
    }
}
