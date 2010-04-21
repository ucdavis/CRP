using System.Collections.Generic;
using System.Linq;
using CRP.Core.Domain;
using UCDArch.Core.PersistanceSupport;
using Check=UCDArch.Core.Utils.Check;

namespace CRP.Controllers.ViewModels
{
    public class ItemTypeViewModel
    {
        public IEnumerable<QuestionType> QuestionTypes { get; set; }
        public ItemType ItemType { get; set; }

        public static ItemTypeViewModel Create(IRepository repository)
        {
            Check.Require(repository != null, "Repository required.");

            var viewModel = new ItemTypeViewModel
                                {
                                    QuestionTypes =
                                        repository.OfType<QuestionType>().Queryable.Where(a => a.ExtendedProperty).
                                        ToList()
                                };

            return viewModel;
        }
    }
}
