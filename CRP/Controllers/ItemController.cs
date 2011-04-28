using System.Web.Mvc;
using CRP.Controllers.ViewModels;
using CRP.Core.Abstractions;
using CRP.Core.Domain;
using CRP.Core.Resources;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Web.Controller;
using MvcContrib;

namespace CRP.Controllers
{
    public class ItemController : SuperController
    {
        private readonly IRepositoryWithTypedId<OpenIdUser, string> _openIdUserRepository;
        private readonly ISearchTermProvider _searchTermProvider;

        public ItemController(IRepositoryWithTypedId<OpenIdUser, string> openIdUserRepository, ISearchTermProvider searchTermProvider)
        {
            _openIdUserRepository = openIdUserRepository;
            _searchTermProvider = searchTermProvider;
        }


        /// <summary>
        /// GET /Item/Details/{id}
        /// </summary>
        /// <remarks>
        /// if the item is not available to the public do not display
        /// </remarks>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Details(int id)
        {
            var item = Repository.OfType<Item>().GetNullableById(id);

            if (item == null || !item.Available)
            {
                Message = NotificationMessages.STR_ObjectNotFound.Replace(NotificationMessages.ObjectType, "Item");
                return this.RedirectToAction<HomeController>(a => a.Index());
            }

            var viewModel = ItemDetailViewModel.Create(Repository, _openIdUserRepository, item, CurrentUser.Identity.Name, null);
            
            if (!item.IsAvailableForReg)
            {
                Message = "Online registration for this event has passed.";
            }

            return View(viewModel);
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
            var item = Repository.OfType<Item>().GetNullableById(id);

            //TODO: Review if this is what you want to happpen, or if it is possible.
            if(item == null)
            {
                return File(new byte[0], "image/jpg");
            }

            if (item.Image != null)
            {
                return File(item.Image, "image/jpg");
            }
            else
            {
                return File(new byte[0], "image/jpg");
            }
        }

        public ActionResult Map(int id, bool usePins)
        {
            var item = Repository.OfType<Item>().GetNullableById(id);

            if (item == null || !item.Available)
            {
                Message = NotificationMessages.STR_ObjectNotFound.Replace(NotificationMessages.ObjectType, "Item");
                return this.RedirectToAction<HomeController>(a => a.Index());
            }
            var viewModel = BigMapViewModel.Create(item);
            viewModel.UsePins = usePins;
            return View(viewModel);
        }   
    }
}
