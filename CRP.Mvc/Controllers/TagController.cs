using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CRP.Controllers.ViewModels;
using CRP.Core.Domain;
using UCDArch.Web.Controller;

namespace CRP.Controllers
{
    public class TagController : ApplicationController
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
                var items = new List<Item>();
                //viewModel.Items = viewModel.Items.Where(a => a.Tags.Contains(existingTag));
                foreach (var item in viewModel.Items)
                {
                    foreach (var itemtag in item.Tags)
                    {
                        if(itemtag.Name.Trim().ToLower() == tag.Trim().ToLower())
                        {
                            items.Add(item);
                            break;                            
                        }
                    }
                }
                viewModel.Items = items;//.AsQueryable();
            }

            return View(viewModel);
        }
    }
}
