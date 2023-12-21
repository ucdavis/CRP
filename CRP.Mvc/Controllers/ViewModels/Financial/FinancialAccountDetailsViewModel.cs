using CRP.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRP.Mvc.Controllers.ViewModels.Financial
{
    public class FinancialAccountDetailsViewModel
    {
        public FinancialAccountDetailsViewModel()
        {
            RelatedItems = new List<ItemModel>();
            Duplicates = new List<FinancialAccount>();
        }
        public FinancialAccount FinancialAccount { get; set; }
        public List<ItemModel> RelatedItems { get; set; }
        public List<FinancialAccount> Duplicates { get; set; }
    }

    public class ItemModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Created { get; set; }
    }
}