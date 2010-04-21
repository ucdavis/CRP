using System.Web.Mvc;
using CRP.Controllers.ViewModels;
using CRP.Core.Domain;
using UCDArch.Web.Controller;
using MvcContrib;

namespace CRP.Controllers
{
    public class ItemController : SuperController
    {
        //
        // GET: /Item/

        public ActionResult Index()
        {
            return View();
        }


        /// <summary>
        /// GET: /Item/List/
        /// </summary>
        /// <returns></returns>
        public ActionResult List()
        {
            var viewModel = BrowseItemsViewModel.Create(Repository);
            return View(viewModel);
        }

        /// <summary>
        /// GET /Item/Details/{id}
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Details(int id)
        {
            var item = Repository.OfType<Item>().GetNullableByID(id);

            if (item == null)
            {
                return this.RedirectToAction(a => a.List());
            }

            return View(item);
        }

        /// <summary>
        /// GET: /Item/GetImage/{id}
        /// </summary>
        /// <remarks>
        /// Returns an image for an item, should have unrestricted access
        /// </remarks>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult GetImage(int id)
        {
            var item = Repository.OfType<Item>().GetNullableByID(id);

            if (item.Image != null)
            {
                return File(item.Image, "image/jpg");
            }
            else
            {
                return File(new byte[0], "image/jpg");
            }
        }

    }
}
