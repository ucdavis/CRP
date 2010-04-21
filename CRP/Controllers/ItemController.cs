using System.Web.Mvc;
using CRP.Controllers.ViewModels;
using CRP.Core.Abstractions;
using CRP.Core.Domain;
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
        /// <remarks>
        /// if the item is not available to the public do not display
        /// </remarks>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Details(int id)
        {
            var item = Repository.OfType<Item>().GetNullableByID(id);

            if (item == null || !item.Available)
            {
                return this.RedirectToAction(a => a.List());
            }

            var viewModel = ItemDetailViewModel.Create(Repository, _openIdUserRepository, item, CurrentUser.Identity.Name);
            
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
            var item = Repository.OfType<Item>().GetNullableByID(id);

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
        /// GET: /Item/Search/{SearchTerm}
        /// </summary>
        /// <param name="searchTerm"></param>
        /// <returns></returns>
        public ActionResult Search(string searchTerm)
        {
            return View(_searchTermProvider.GetByTerm(searchTerm));
        }
    }
}
