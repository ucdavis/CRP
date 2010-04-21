using System.Collections.Generic;
using CRP.Core.Domain;
using UCDArch.Core.PersistanceSupport;
using Check=UCDArch.Core.Utils.Check;

namespace CRP.Controllers.ViewModels
{
    public class ItemTypeViewModel
    {
        public IEnumerable<QuestionType> QuestionTypes { get; set; }
        public ItemType ItemType { get; set; }

        public static ItemTypeViewModel Create(IRepository respository)
        {
            Check.Require(respository != null, "Respository required.");

            var viewModel = new ItemTypeViewModel {QuestionTypes = respository.OfType<QuestionType>().GetAll()};

            return viewModel;
        }
    }
}
