using System.Collections.Generic;
using CRP.Core.Domain;
using UCDArch.Core.PersistanceSupport;
using Check=UCDArch.Core.Utils.Check;

namespace CRP.Controllers.ViewModels
{
    public class QuestionSetViewModel
    {
        public static QuestionSetViewModel Create(IRepository repository)
        {
            Check.Require(repository != null, "Repository is required.");

            var viewModel = new QuestionSetViewModel() {QuestionTypes = repository.OfType<QuestionType>().GetAll()};

            return viewModel;
        }

        public IEnumerable<QuestionType> QuestionTypes { get; set; }
        public QuestionSet QuestionSet { get; set; }
    }
}
