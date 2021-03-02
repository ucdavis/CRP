using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRP.Mvc.Controllers.ViewModels
{
    public class AccountValidationModel
    {
        public bool IsValid { get; set; } = false;
        public string Field { get; set; }
        public string Message { get; set; }
    }
}