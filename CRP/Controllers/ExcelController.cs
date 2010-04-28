using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using CRP.Controllers.Filter;
using CRP.Controllers.Helpers;
using CRP.Controllers.ViewModels;
using CRP.Core.Domain;
using CRP.Core.Resources;
using MvcContrib;
using NPOI.HSSF.UserModel;
using UCDArch.Web.Controller;

namespace CRP.Controllers
{
    [UserOnly]
    public class ExcelController : SuperController
    {

        public ActionResult CreateExcelReport(int id, int itemId)
        {
            var itemReport = Repository.OfType<ItemReport>().GetNullableByID(id);
            if (itemReport == null)
            {
                Message = NotificationMessages.STR_ObjectNotFound.Replace(NotificationMessages.ObjectType, "ItemReport");
                return this.RedirectToAction<ItemManagementController>(a => a.List());
            }
            var item = Repository.OfType<Item>().GetNullableByID(itemId);
            if (item == null)
            {
                Message = NotificationMessages.STR_ObjectNotFound.Replace(NotificationMessages.ObjectType, "Item");
                return this.RedirectToAction<ItemManagementController>(a => a.List());
            }

            if (!Access.HasItemAccess(CurrentUser, item))
            {
                Message = NotificationMessages.STR_NoEditorRights;
                return this.RedirectToAction<ItemManagementController>(a => a.List());
            }

            var viewModel = ReportViewModel.Create(Repository, itemReport, item);

            var fileName = string.Format("{0}-{1}.xls", itemReport.Name.Replace(" ", string.Empty), DateTime.Now.Date.ToString("MMddyyyy"));

            try
            {
                // Opening the Excel template...
                var fs = new FileStream(Server.MapPath(@"~\Content\NPOITemplate.xls"), FileMode.Open, FileAccess.Read);

                // Getting the complete workbook...
                var templateWorkbook = new HSSFWorkbook(fs, true);

                // Getting the worksheet by its name...
                var sheet = templateWorkbook.GetSheetAt(0);// GetSheet("Sheet1");

                // Getting the row... 0 is the first row.
                var dataRow = sheet.CreateRow(0);
                for (int i = 0; i < viewModel.ColumnNames.Count; i++)
                {
                    dataRow.CreateCell(i).SetCellValue(viewModel.ColumnNames.ElementAt(i));
                }

                var rowCount = 0;
                foreach (var rowValue in viewModel.RowValues)
                {
                    rowCount++;
                    dataRow = sheet.CreateRow(rowCount);
                    for (int i = 0; i < rowValue.Count(); i++)
                    {
                        dataRow.CreateCell(i).SetCellValue(rowValue.ElementAtOrDefault(i));
                    }
                }

                // Forcing formula recalculation...
                sheet.ForceFormulaRecalculation = true;

                var ms = new MemoryStream();

                // Writing the workbook content to the FileStream...
                templateWorkbook.Write(ms);

                Message = "Excel report created successfully!";

                // Sending the server processed data back to the user computer...
                return File(ms.ToArray(), "application/vnd.ms-excel", fileName);
            }
            catch (Exception ex)
            {
                Message = "Error Creating Excel Report " + ex.Message;

                return this.RedirectToAction<HomeController>(a => a.AdminHome());
            }
        }
    }
}
