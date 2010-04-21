using System;
using System.Web.Mvc;

namespace CRP.Controllers
{
    public class TransactionController : Controller
    {
        //
        // GET: /Transaction/

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// GET: /Transaction/Register/{id}
        /// </summary>
        /// <param name="id">Item id</param>
        /// <returns></returns>
        public ActionResult Register(int id)
        {
            throw new NotImplementedException();
        }
    }
}
