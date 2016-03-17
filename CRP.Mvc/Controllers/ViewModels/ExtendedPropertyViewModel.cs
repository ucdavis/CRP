using System.Collections.Generic;
using System.Linq;
using CRP.Core.Domain;
using UCDArch.Core.PersistanceSupport;
using Check=UCDArch.Core.Utils.Check;

namespace CRP.Controllers.ViewModels
{
    public class ExtendedPropertyViewModel
    {
        public IEnumerable<QuestionType> QuestionTypes { get; set; }
        public ItemType ItemType { get; set; }
        
        public static ExtendedPropertyViewModel Create(IRepository repository, ItemType itemType)
        {
            Check.Require(repository != null, "Repository is required.");

            var viewModel = new ExtendedPropertyViewModel()
                                {
                                    QuestionTypes = repository.OfType<QuestionType>().Queryable.Where(a => a.ExtendedProperty).ToList(),
                                    ItemType = itemType
                                };

            return viewModel;
        }
    }
}
