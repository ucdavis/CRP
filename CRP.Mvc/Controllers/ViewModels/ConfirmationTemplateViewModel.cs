using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CRP.Core.Domain;
using CRP.Core.Resources;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;

namespace CRP.Controllers.ViewModels
{
    public class ConfirmationTemplateViewModel
    {
        public string PaidText { get; set; }
        public string UnpaidText { get; set; }
        public Template Template { get; set; }

        public static ConfirmationTemplateViewModel Create(Template template)
        {            
            Check.Require(template != null, "Template is required.");

            var viewModel = new ConfirmationTemplateViewModel{Template = template};
            if (template.Text != null && template.Text.Contains(StaticValues.ConfirmationTemplateDelimiter))
            {
                //var index = template.Text.IndexOf("<<PaidTextAbove>>");
                var delimiter = new string[] { StaticValues.ConfirmationTemplateDelimiter };
                var parse = template.Text.Split(delimiter, StringSplitOptions.None);
                viewModel.PaidText = parse[0];
                viewModel.UnpaidText = parse[1];
            }
            else
            {
                viewModel.PaidText = template.Text ?? string.Empty;
                viewModel.UnpaidText = string.Empty;
            }
            return viewModel;
        }

    }
}
