using System.Web.Mvc;
using CRP.Controllers.Helpers;
using CRP.Controllers.ViewModels;
using CRP.Core.Abstractions;
using CRP.Core.Domain;
using CRP.Core.Resources;
using UCDArch.Core.PersistanceSupport;
using MvcContrib;

namespace CRP.Controllers
{
    public class ItemController : ApplicationController
    {
        private readonly IRepositoryWithTypedId<OpenIdUser, string> _openIdUserRepository;
        private readonly ISearchTermProvider _searchTermProvider;

        public ItemController(IRepositoryWithTypedId<OpenIdUser, string> openIdUserRepository, ISearchTermProvider searchTermProvider)
        {
            _openIdUserRepository = openIdUserRepository;
            _searchTermProvider = searchTermProvider;
        }

        /// <summary>
        /// Tested 20200512
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Details(int id)
        {
            var item = Repository.OfType<Item>().GetNullableById(id);

            if (item == null)
            {
                Message = NotificationMessages.STR_ObjectNotFound.Replace(NotificationMessages.ObjectType, "Item");
                return this.RedirectToAction<HomeController>(a => a.Index());
            }

            if (!item.Available)
            {
                if (!Access.HasItemAccess(CurrentUser, item)) //Allow editors to override and register for things (also allows preview)
                {
                    Message = NotificationMessages.STR_ObjectNotFound.Replace(NotificationMessages.ObjectType, "Item");
                    return this.RedirectToAction<HomeController>(a => a.Index());
                }
                else
                {
                    Message = "Event is not available to public";
                }
            }

            var viewModel = ItemDetailViewModel.Create(Repository, _openIdUserRepository, item, CurrentUser.Identity.Name, null, null, null);

            if (!item.IsAvailableForReg)
            {
                if (!Access.HasItemAccess(CurrentUser, item)) //Allow editors to override and register for things (also allows preview)
                {
                    Message = "Online registration for this event has passed. Or it has sold out.";
                }
            }


            return View(viewModel);
        }

        /// <summary>
        /// GET: /Item/GetImage/{id}
        /// Tested 20200512
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


        /// <summary>
        /// Tested 20200512
        /// </summary>
        /// <param name="id"></param>
        /// <param name="usePins"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Tested 20200512
        /// </summary>
        /// <param name="id"></param>
        /// <param name="usePins"></param>
        /// <returns></returns>
        public ActionResult MapDirections(int id, bool usePins)
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
