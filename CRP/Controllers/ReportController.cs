using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
//using CRP.App_GlobalResources;
using System.Web.UI.DataVisualization.Charting;
using CRP.Controllers.Filter;
using CRP.Controllers.Helpers;
using CRP.Controllers.ViewModels;
using CRP.Core.Abstractions;
using CRP.Core.Domain;
using MvcContrib.Attributes;
using CRP.Core.Resources;
using UCDArch.Web.Controller;
using MvcContrib;
using UCDArch.Web.Validator;

namespace CRP.Controllers
{
    public class ReportController : ApplicationController
    {
        const string ImageType = @"image/png";

        private readonly IChartProvider _chartProvider;

        public ReportController(IChartProvider chartProvider)
        {
            _chartProvider = chartProvider;
        }

        /// <summary>
        /// GET: /Report/ViewReport/
        /// </summary>
        /// <param name="id"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        [UserOnly]
        public ActionResult ViewReport(int id, int itemId)
        {
            var itemReport = Repository.OfType<ItemReport>().GetNullableById(id);
            if(itemReport == null)
            {
                Message = NotificationMessages.STR_ObjectNotFound.Replace(NotificationMessages.ObjectType, "ItemReport");
                return this.RedirectToAction<ItemManagementController>(a => a.List(null));
            }
            var item = Repository.OfType<Item>().GetNullableById(itemId);
            if (item == null)
            {
                Message = NotificationMessages.STR_ObjectNotFound.Replace(NotificationMessages.ObjectType, "Item");
                return this.RedirectToAction<ItemManagementController>(a => a.List(null));
            }

            if(!Access.HasItemAccess(CurrentUser, item))
            {
                Message = NotificationMessages.STR_NoEditorRights;
                return this.RedirectToAction<ItemManagementController>(a => a.List(null));
            }

            var viewModel = ReportViewModel.Create(Repository, itemReport, item);

            return View(viewModel);
        }

        /// <summary>
        /// GET: /Report/Create/{itemId}
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        [UserOnly]
        public ActionResult Create(int itemId)
        {
            var item = Repository.OfType<Item>().GetNullableById(itemId);

            if (item == null)
            {
                Message = NotificationMessages.STR_ObjectNotFound.Replace(NotificationMessages.ObjectType, "Item");
                return this.RedirectToAction<ItemManagementController>(a => a.List(null));
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
        [HttpPost]
        [UserOnly]
        public ActionResult Create(int itemId, string name, CreateReportParameter[] createReportParameters)
        {
            var item = Repository.OfType<Item>().GetNullableById(itemId);

            if (item == null)
            {
                Message = NotificationMessages.STR_ObjectNotFound.Replace(NotificationMessages.ObjectType, "Item");
                return this.RedirectToAction<ItemManagementController>(a => a.List(null));
            }


            var report = new ItemReport(name, item,
                                        Repository.OfType<User>().Queryable.Where(
                                            a => a.LoginID == CurrentUser.Identity.Name).FirstOrDefault());
            if (createReportParameters == null || createReportParameters.Count() < 1)
            {
                Message = "Report Columns not Selected";
                var viewModel2 = CreateReportViewModel.Create(Repository, item);
                viewModel2.ItemReport = report;
                return View(viewModel2);
            }
            foreach(var crp in createReportParameters)
            {
                var questionSet = Repository.OfType<QuestionSet>().GetNullableById(crp.QuestionSetId);
                var question = Repository.OfType<Question>().GetNullableById(crp.QuestionId);

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

            Message = "Errors with report found.";
            var viewModel = CreateReportViewModel.Create(Repository, item);
            viewModel.ItemReport = report;
            return View(viewModel);
        }

        [AdminOnly]
        public ActionResult ViewSystemReport(int? reportId)
        {
            var viewModel = SystemReportViewModel.Create(Repository);
            viewModel.Reports = Enum.GetValues(typeof(SystemReport));
            viewModel.SelectedReport = reportId;

            if (reportId.HasValue) viewModel.SystemReportData = GetData(reportId.Value);

            return View(viewModel);
        }

        private IEnumerable<SystemReportData> GetData(int reportId)
        {
            switch ((SystemReport)reportId)
            {
                // generates a report to show who is using the system the most
                case SystemReport.DepartmentUsage:
                    var data = Repository.OfType<Item>().GetAll();

                    return (from i in data.AsQueryable()
                           group i by i.Unit.FullName into g
                           select new SystemReportData(g.Key, g.Count())).ToList();
                case SystemReport.DepartmentMoneyYtd:
                    var data2 = Repository.OfType<Transaction>().GetAll();

                    return (from t in data2.AsQueryable()
                           group t by t.Item.Unit.FullName into g
                           select new SystemReportData(g.Key, g.Sum(a => a.TotalPaid), "C")).ToList();
            };

            return new List<SystemReportData>();
        }

        [AdminOnly]
        public ActionResult GenerateChart(int reportId)
        {
            var xParameter = new List<string>();
            var yParameter = new List<decimal>();

            var data = GetData(reportId);

            foreach (var s in data)
            {
                xParameter.Add(s.Name);
                yParameter.Add(s.Value);
            }

            switch ((SystemReport)reportId)
            {
                    // generates a report to show who is using the system the most
                case SystemReport.DepartmentUsage:
                    var chart = _chartProvider.CreateChart(xParameter.ToArray(),
                                                           yParameter.ToArray(), "Departmental Usage",
                                                           SeriesChartType.Pie);
                    return File(chart, ImageType);
                case SystemReport.DepartmentMoneyYtd:
                    var chart2 = _chartProvider.CreateChart(xParameter.ToArray(),
                                       yParameter.ToArray(), "Departmental Money Collected YTD",
                                       SeriesChartType.Pie);
                    return File(chart2, ImageType);
            };

            return File(new byte[0], ImageType);
        }

        public enum SystemReport
        {
            DepartmentUsage = 0,
            DepartmentMoneyYtd,
        } ;
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
