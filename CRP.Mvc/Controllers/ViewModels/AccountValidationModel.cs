using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CRP.Core.Domain;

namespace CRP.Mvc.Controllers.ViewModels
{
    public class AccountValidationModel
    {
        public bool IsValid { get; set; } = false;
        public bool IsWarning { get; set; } = false; //So admin page can create these where if fails CRP rules, but is otherwise a valid COA 
        public string Field { get; set; }
        public string Message
        {
            get
            {
                if (Messages.Count <= 0)
                {
                    return string.Empty;
                }

                return string.Join(" ", Messages);
            }
        }
        public List<string> Messages { get; set; } = new List<string>();

        //Use this for User Added Accounts
        public FinancialAccount FinancialAccount { get; set; }
    }
}