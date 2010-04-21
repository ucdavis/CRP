using System.Web.Mvc;
using CRP.Core.Domain;
using UCDArch.Web.Controller;

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

        public ActionResult List()
        {
            return View();
        }

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
