using System.Collections.Generic;
using CRP.Core.Domain;
using UCDArch.Core.PersistanceSupport;
using Check = UCDArch.Core.Utils.Check;

namespace CRP.Controllers.ViewModels
{
    public class QuestionViewModel
    {
        public IEnumerable<QuestionType> QuestionTypes { get; set; }
        public Question Question { get; set; }
        public QuestionSet QuestionSet { get; set; }

        public static QuestionViewModel Create(IRepository repository)
        {
            Check.Require(repository != null, "Repository is required.");

            var viewModel = new QuestionViewModel() {QuestionTypes = repository.OfType<QuestionType>().GetAll()};

            return viewModel;
        }
    }
}
