using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using NPOI.HSSF.UserModel;
using UCDArch.Web.Controller;
using MvcContrib;

namespace CRP.Controllers
{
    public class TestExcellController : SuperController
    {
        //
        // GET: /TestExcell/

        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// Creates a new Excel spreadsheet based on a template using the ExcelPackage library.
        /// A new file is created on the server based on a template.
        /// </summary>
        /// <returns>Excel report</returns>
        //[AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult ExcelPackageCreate()
        //{
        //    try
        //    {
        //        FileInfo template = new FileInfo(Server.MapPath(@"\Content\ExcelPackageTemplate.xlsx"));

        //        FileInfo newFile = new FileInfo(Server.MapPath(@"\Content\ExcelPackageNewFile.xlsx"));

        //        // Using the template to create the newFile...
        //        using (ExcelPackage excelPackage = new ExcelPackage(newFile, template))
        //        {
        //            // Getting the complete workbook...
        //            ExcelWorkbook myWorkbook = excelPackage.Workbook;

        //            // Getting the worksheet by its name...
        //            ExcelWorksheet myWorksheet = myWorkbook.Worksheets["Sheet1"];

        //            // Setting the value 77 at row 5 column 1...
        //            myWorksheet.Cell(5, 1).Value = 77.ToString();

        //            // Saving the change...
        //            excelPackage.Save();
        //        }

        //        TempData["Message"] = "Excel report created successfully!";

        //        return RedirectToAction("ExcelPackage");
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["Message"] = "Oops! Something went wrong.";

        //        return RedirectToAction("ExcelPackage");
        //    }
        //}
        /// <summary>
        /// Creates a new Excel spreadsheet based on a template using the NPOI library.
        /// The template is changed in memory and a copy of it is sent to
        /// the user computer through a file stream.
        /// </summary>
        /// <returns>Excel report</returns>
        public ActionResult NPOICreate()
        {
            try
            {
                // Opening the Excel template...
                FileStream fs =
                    new FileStream(Server.MapPath(@"~\Content\NPOITemplate.xls"), FileMode.Open, FileAccess.Read);

                // Getting the complete workbook...
                HSSFWorkbook templateWorkbook = new HSSFWorkbook(fs, true);

                // Getting the worksheet by its name...
                HSSFSheet sheet = templateWorkbook.GetSheetAt(0);// GetSheet("Sheet1");

                // Getting the row... 0 is the first row.
                HSSFRow dataRow = sheet.CreateRow(0);// .GetRow(4);

                // Setting the value 77 at row 5 column 1
                dataRow.CreateCell(0).SetCellValue(77);

                // Forcing formula recalculation...
                sheet.ForceFormulaRecalculation = true;

                MemoryStream ms = new MemoryStream();

                // Writing the workbook content to the FileStream...
                templateWorkbook.Write(ms);

                Message = "Excel report created successfully!";

                // Sending the server processed data back to the user computer...
                return File(ms.ToArray(), "application/vnd.ms-excel", "NPOINewFile.xls");
            }
            catch (Exception ex)
            {
                Message = ex.Message;

                return this.RedirectToAction(a => a.Index());
            }
        }
    }
}
