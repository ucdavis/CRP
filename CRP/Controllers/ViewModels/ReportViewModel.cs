using System;
using System.Collections.Generic;
using System.Linq;
using CRP.Core.Domain;
using CRP.Core.Resources;
using UCDArch.Core.PersistanceSupport;
using Check = UCDArch.Core.Utils.Check;

namespace CRP.Controllers.ViewModels
{
    public class ReportViewModel
    {
        public ReportViewModel()
        {
            ColumnNames = new List<string>();
            RowValues = new List<string[]>();
        }

        public ICollection<string> ColumnNames { get; set; }
        public ICollection<string[]> RowValues { get; set; }
        //public ICollection<CRP.Core.Domain.Check> Checks { get; set; }
        public int ItemId { get; set; }
        public string ReportName { get; set; }
        public int ItemReportId { get; set; }

        public static ReportViewModel Create(IRepository repository, ItemReport itemReport, Item item, bool fromExcel = true)
        {
            Check.Require(repository != null, "Repository is required.");

            var viewModel = new ReportViewModel
            {
                ItemId = item.Id,
                ReportName = itemReport.Name,
                ItemReportId = itemReport.Id
            };

            if (itemReport.Name == "Checks" && itemReport.SystemReusable)
            {
                return GenerateChecks(viewModel, itemReport, item);
            }

            return GenerateGeneric(viewModel, itemReport, item, fromExcel);
        }

        private static ReportViewModel GenerateGeneric (ReportViewModel viewModel, ItemReport itemReport, Item item, bool fromExcel)
        {
            //deal with the column names
            foreach (var ir in itemReport.Columns)
            {
                viewModel.ColumnNames.Add(!string.IsNullOrWhiteSpace(ir.Format) && !fromExcel
                                              ? string.Format("{0} ({1})", ir.Name, ir.Format)
                                              : ir.Name);
            }

            // deal with the row values, if there are any quantity properties, we need to go through the quantity values
            if (itemReport.Columns.Any(a => a.Quantity))
            {
                // go through all the transactions
                foreach (var x in item.Transactions.Where(a => a.ParentTransaction == null))
                {
                    // go through all the unqiue quantity ids
                    foreach (var y in x.QuantityAnswers.Select(a => a.QuantityId).Distinct())
                    {
                        // this represents one row worth of data
                        var row = new List<string>();

                        // go through all the requested columns, x=transaction, y=quantityId
                        foreach (var z in itemReport.Columns)
                        {
                            row.Add(ExtractValue(z, x, y));
                        }

                        viewModel.RowValues.Add(row.ToArray());
                    }
                }
            }
            // otherwise it's a transaction level report
            else
            {
                // go through all the transactions
                foreach (var x in item.Transactions.Where(a => a.ParentTransaction == null))
                {
                    var row = new List<string>();

                    foreach (var z in itemReport.Columns)
                    {
                        row.Add(ExtractValue(z, x, null));
                    }

                    viewModel.RowValues.Add(row.ToArray());
                }
            }

            return viewModel;
        }

        private static ReportViewModel GenerateChecks (ReportViewModel viewModel, ItemReport itemReport, Item item)
        {
            //deal with the column names
            foreach (var ir in itemReport.Columns)
            {
                viewModel.ColumnNames.Add(ir.Name);
            }

            // go through all the transactions
            foreach (var x in item.Transactions.Where(a => a.ParentTransaction == null))
            {
                // go through all the unqiue quantity ids
                foreach (var y in x.PaymentLogs.Where(a => a.Check && a.Accepted))
                {
                    // this represents one row worth of data
                    var row = new List<string>();

                    // go through all the requested columns, x=transaction, y=quantityId
                    foreach (var z in itemReport.Columns)
                    {
                        row.Add(ExtractCheckValue(y, z.Name));
                    }

                    viewModel.RowValues.Add(row.ToArray());
                }
            }

            return viewModel;
        }

        /// <summary>
        /// Extract the requested value from the transaction answers
        /// </summary>
        /// <param name="itemReportColumn"></param>
        /// <param name="transaction"></param>
        /// <param name="quantityId"></param>
        /// <returns></returns>
        private static string ExtractValue(ItemReportColumn itemReportColumn, Transaction transaction, Guid? quantityId)
        {
            var result = string.Empty;

            if (itemReportColumn.Transaction)
            {
                var transactionAnswer = transaction.TransactionAnswers.Where(a => a.Question.Name == itemReportColumn.Name && a.QuestionSet == itemReportColumn.QuestionSet).FirstOrDefault();

                if (transactionAnswer != null)
                {
                    result = transactionAnswer.Answer;
                }
            }
            else if (itemReportColumn.Quantity)
            {
                var quantityAnswer = transaction.QuantityAnswers.Where(a => a.QuantityId == quantityId.Value && a.Question.Name == itemReportColumn.Name && a.QuestionSet == itemReportColumn.QuestionSet).FirstOrDefault();

                if (quantityAnswer != null)
                {
                    result = quantityAnswer.Answer;
                }
            }
            else if (itemReportColumn.Property)
            {
                if (itemReportColumn.Name == StaticValues.Report_DonationTotal)
                {
                    result = transaction.DonationTotal.ToString("C");
                }
                else if (itemReportColumn.Name == StaticValues.Report_TransactionNumber)
                {
                    result = transaction.TransactionNumber;
                }
                else if (itemReportColumn.Name == StaticValues.Report_TransactionDate)
                {
                    result = transaction.TransactionDate.ToString();
                }
                else if (itemReportColumn.Name == StaticValues.Report_Active)
                {
                    result = transaction.IsActive.ToString();
                }
                else if(itemReportColumn.Name == StaticValues.Report_AmountTotal)
                {
                    result = transaction.AmountTotal.ToString("C");
                }
                else if (itemReportColumn.Name == StaticValues.Report_Total)
                {
                    result = transaction.Total.ToString("C");
                }
                else if (itemReportColumn.Name == StaticValues.Report_PaymentType)
                {
                    result = transaction.Credit ? "Credit Card" : "Check";
                }
                else if (itemReportColumn.Name == StaticValues.Report_Quantity)
                {
                    result = transaction.Quantity.ToString();
                }
                else if (itemReportColumn.Name == StaticValues.Report_Paid)
                {
                    result = transaction.Paid.ToString();
                }
                else if (itemReportColumn.Name == StaticValues.Report_TotalPaid)
                {
                    result = transaction.TotalPaid.ToString("C");
                }
                else if (itemReportColumn.Name == StaticValues.Report_RefundIssued)
                {
                    result = transaction.RefundIssued.ToString();
                }
                else if (itemReportColumn.Name == StaticValues.Report_RefundAmount)
                {
                    result = transaction.RefundAmount.ToString("C");
                }
                else if (itemReportColumn.Name == StaticValues.Report_Notified)
                {
                    result = transaction.Notified.ToString();
                }
                else if (itemReportColumn.Name == StaticValues.Report_NotifiedDate)
                {
                    result = transaction.NotifiedDate == null ? string.Empty : transaction.NotifiedDate.ToString();
                }
                else if (itemReportColumn.Name == StaticValues.Report_TransactionGuid)
                {
                    if (transaction.Credit) result = string.Format("{0} FID={1}", transaction.TransactionGuid, transaction.FidUsed ?? transaction.Item.TouchnetFID);
                    else result = "n/a";
                }
            }


            if (result != null && itemReportColumn.Format == StaticValues.FormatCapitalize)
            {
                result = UCDArch.Core.Utils.Inflector.Capitalize(result);
            }
            return result;
        }
        /// <summary>
        /// Extract check values
        /// </summary>
        /// <param name="check"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        private static string ExtractCheckValue(PaymentLog paymentLog, string field)
        {
            var result = string.Empty;

            if (field == StaticValues.Report_Checks_Payee)
            {
                result = paymentLog.Name;
            }
            else if (field == StaticValues.Report_Checks_CheckNumber)
            {
                result = paymentLog.CheckNumber.ToString();
            }
            else if(field == StaticValues.Report_Checks_Amount)
            {
                result = paymentLog.Amount.ToString("C");
            }
            else if(field == StaticValues.Report_Checks_DateReceived)
            {
                result = paymentLog.DatePayment.ToString("d");
            }
            else if (field == StaticValues.Report_Checks_Notes)
            {
                result = paymentLog.Notes;
            }
            else if (field == StaticValues.Report_Checks_TransactionId)
            {
                result = paymentLog.Transaction.TransactionNumber;
            }

            return result;
        }
    }



    public class CreateReportViewModel
    {
        public Item Item { get; set; }
        public ItemReport ItemReport { get; set; }
        public QuestionType QuestionTypeNoAnswer { get; set; }

        public static CreateReportViewModel Create(IRepository repository, Item item)
        {
            Check.Require(repository != null, "Repository is required.");

            var viewModel = new CreateReportViewModel() {Item = item};
            viewModel.QuestionTypeNoAnswer =
                repository.OfType<QuestionType>().Queryable.Where(a => a.Name == QuestionTypeText.STR_NoAnswer).
                    FirstOrDefault();

            return viewModel;
        }
    }
}
