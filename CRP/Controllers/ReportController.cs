using System;
using System.Linq;
using System.Web.Mvc;
//using CRP.App_GlobalResources;
using CRP.Controllers.Filter;
using CRP.Controllers.Helpers;
using CRP.Controllers.ViewModels;
using CRP.Core.Domain;
using MvcContrib.Attributes;
using CRP.Core.Resources;
using UCDArch.Web.Controller;
using MvcContrib;
using UCDArch.Web.Validator;

namespace CRP.Controllers
{
    public class ReportController : SuperController
    {
        /// <summary>
        /// GET: /Report/ViewReport/
        /// </summary>
        /// <param name="id"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        [RolesFilter.UserOnlyAttribute]
        public ActionResult ViewReport(int id, int itemId)
        {
            var itemReport = Repository.OfType<ItemReport>().GetNullableByID(id);
            var item = Repository.OfType<Item>().GetNullableByID(itemId);
            var viewModel = ReportViewModel.Create(Repository, itemReport, item);

            return View(viewModel);
        }

        /// <summary>
        /// GET: /Report/Create/{itemId}
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        [RolesFilter.UserOnlyAttribute]
        public ActionResult Create(int itemId)
        {
            var item = Repository.OfType<Item>().GetNullableByID(itemId);

            if (item == null)
            {
                return this.RedirectToAction<ItemManagementController>(a => a.List());
            }

            var viewModel = CreateReportViewModel.Create(Repository, item);

            return View(viewModel);
        }

        /// <summary>
        /// POST: /Report/Create/
        /// </summary>
        /// <remarks>
        /// Description:
        ///     Create a report that is used specifically with one report.
        /// PreCondition:
        ///     Item is created, valid and has one or more question sets available
        ///     createReportParameters parameter has at least one item
        ///     name is not null or empty
        /// PostCondition:
        ///     Item Report is created
        ///     Each createReportParameters is created into an ItemReportColumn object
        ///     Item Report is added to an Item
        /// </remarks>
        /// <param name="itemId"></param>
        /// <param name="name"></param>
        /// <param name="createReportParameters"></param>
        /// <returns></returns>
        [AcceptPost]
        [RolesFilter.UserOnlyAttribute]
        public ActionResult Create(int itemId, string name, CreateReportParameter[] createReportParameters)
        {
            var item = Repository.OfType<Item>().GetNullableByID(itemId);

            if (item == null)
            {
                return this.RedirectToAction<ItemManagementController>(a => a.List());
            }

            var report = new ItemReport(name, item,
                                        Repository.OfType<User>().Queryable.Where(
                                            a => a.LoginID == CurrentUser.Identity.Name).FirstOrDefault());

            foreach(var crp in createReportParameters)
            {
                var questionSet = Repository.OfType<QuestionSet>().GetNullableByID(crp.QuestionSetId);
                var question = Repository.OfType<Question>().GetNullableByID(crp.QuestionId);

                ItemReportColumn itemReportColumn = crp.Property ? new ItemReportColumn(crp.PropertyName, report) 
                    : new ItemReportColumn(question.Name, report);

                itemReportColumn.Transaction = crp.Transaction;
                itemReportColumn.Quantity = crp.Quantity;
                itemReportColumn.Property = crp.Property;
                itemReportColumn.QuestionSet = questionSet;

                report.AddReportColumn(itemReportColumn);
            }

            MvcValidationAdapter.TransferValidationMessagesTo(ModelState, report.ValidationResults());

            if (ModelState.IsValid)
            {
                Repository.OfType<ItemReport>().EnsurePersistent(report);
                Message = NotificationMessages.STR_ObjectCreated.Replace(NotificationMessages.ObjectType, "Report");
                //return Redirect(ReturnUrlGenerator.DetailItemUrl(item.Id, StaticValues.Tab_Reports));

                return Redirect(Url.DetailItemUrl(item.Id, StaticValues.Tab_Reports));
            }
            
            var viewModel = CreateReportViewModel.Create(Repository, item);
            viewModel.ItemReport = report;
            return View(viewModel);
        }
    }

    public class CreateReportParameter
    {
        public CreateReportParameter()
        {
            Transaction = false;
            Quantity = false;
            Property = false;
        }

        public bool Transaction { get; set; }
        public bool Quantity { get; set; }
        public bool Property { get; set; }
        public int QuestionId { get; set; }
        public string PropertyName { get; set; }
        public int QuestionSetId { get; set; }
    }
}
