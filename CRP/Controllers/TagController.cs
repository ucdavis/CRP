using System.Linq;
using System.Web.Mvc;
using CRP.Controllers.ViewModels;
using CRP.Core.Domain;
using UCDArch.Web.Controller;

namespace CRP.Controllers
{
    public class TagController : SuperController
    {
        //
        // GET: /Tag/

        public ActionResult Index(string tag)
        {
            var existingTag = Repository.OfType<Tag>().Queryable.Where(a => a.Name == tag).FirstOrDefault();

            var viewModel = BrowseItemsViewModel.Create(Repository);

            if (existingTag == null)
            {
                Message = "No items match that tag.";
            }
            else
            {
                viewModel.Items = viewModel.Items.Where(a => a.Tags.Contains(existingTag));    
            }

            return View(viewModel);
        }
    }
}
