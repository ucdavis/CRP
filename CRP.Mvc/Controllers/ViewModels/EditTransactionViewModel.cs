using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using CRP.Core.Domain;
using CRP.Core.Resources;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;

namespace CRP.Controllers.ViewModels
{

    public class EditTransactionViewModel
    {
        public Transaction TransactionValue { get; set; }
        public string ContactName { get; set; }
        public string ContactEmail { get; set; }
        public decimal Amount { get; set; }
        public string CorrectionReason { get; set; }
        public string Sort { get; set; }
        public string Page { get; set; }
        public FinancialAccount FinancialAccount { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreatedBy { get; set; }
        public decimal RefundAmount { get; set; }
        public string Fid { get; set; } //For CreditCard transactions before CyberSource

        public static EditTransactionViewModel Create(IRepository repository, Transaction transaction)
        {
            Check.Require(repository != null, "Repository is required.");

            var viewModel = new EditTransactionViewModel() { };
            viewModel.TransactionValue = transaction;
            viewModel.FinancialAccount = transaction.FinancialAccount;


            var fid = string.Empty;
            if (viewModel.TransactionValue.FidUsed != null)
            {
                fid = viewModel.TransactionValue.FidUsed;
                viewModel.Fid = string.Format(" FID={0}", fid);
            }
            
            return viewModel;
        }

    }
}
