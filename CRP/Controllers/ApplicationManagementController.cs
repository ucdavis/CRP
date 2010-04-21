using System.Web.Mvc;
using CRP.Core.Domain;
using UCDArch.Web.Controller;

namespace CRP.Controllers
{
    public class ApplicationManagementController : SuperController
    {
        //
        // GET: /ApplicationManagement/

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// GET: /ApplicationManagement/ListItemTypes
        /// </summary>
        /// <returns></returns>
        public ActionResult ListItemTypes()
        {
            return View(Repository.OfType<ItemType>().GetAll());
        }
    }
}
