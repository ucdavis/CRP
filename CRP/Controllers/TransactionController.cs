using System;
using System.Linq;
using System.Web.Mvc;
using CRP.Controllers.ViewModels;
using CRP.Core.Domain;
using MvcContrib.Attributes;
using UCDArch.Web.ActionResults;
using UCDArch.Web.Controller;
using MvcContrib;

namespace CRP.Controllers
{
    public class TransactionController : SuperController
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
            var item = Repository.OfType<Item>().GetNullableByID(id);

            if (item == null)
            {
                return this.RedirectToAction<ItemController>(a => a.List());
            }

            var viewModel = ItemDetailViewModel.Create(Repository, item);
            return View(viewModel);
        }

        [AcceptPost]
        public ActionResult Register(int id, int quantity, QuestionAnswerParameter[] transactionAnswers, QuestionAnswerParameter[] quantityAnswers)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// GET: /Transaction/GetQuantityQuestionSets/{id}
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonNetResult GetQuantityQuestionSets(int id)
        {
            var item = Repository.OfType<Item>().GetNullableByID(id);

            if (item == null)
            {
                return new JsonNetResult(false);
            }

            // go through and figure out how to deal with the questions


            throw new NotImplementedException();
        }
    }

    public class QuestionAnswerParameter
    {
        public int QuestionId { get; set; }
        public int QuestionSetId { get; set; }
        public int QuantityIndex { get; set; }
        public string Answer { get; set; }
    }
}
