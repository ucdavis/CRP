using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CRP.Core.Domain;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;

namespace CRP.Controllers.ViewModels
{
    public class ConfirmationTemplateViewModel
    {
        public string PaidText { get; set; }
        public string UnpaidText { get; set; }
        public Template Template { get; set; }

        public static ConfirmationTemplateViewModel Create(IRepository repository, Template template)
        {
            Check.Require(repository != null, "Repository is required.");
            Check.Require(template != null, "Template is required.");

            var viewModel = new ConfirmationTemplateViewModel{Template = template};
            if(template.Text.Contains("{PaidTextAbove}"))
            {
                //var index = template.Text.IndexOf("<<PaidTextAbove>>");
                var delimiter = new string[]{"{PaidTextAbove}"};
                var parse = template.Text.Split(delimiter, StringSplitOptions.None);
                viewModel.PaidText = parse[0];
                viewModel.UnpaidText = parse[1];
            }
            else
            {
                viewModel.PaidText = template.Text;
                viewModel.UnpaidText = string.Empty;
            }
            return viewModel;
        }

    }
}
