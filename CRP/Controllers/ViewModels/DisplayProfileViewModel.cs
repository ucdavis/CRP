using System.Collections.Generic;
using CRP.Core.Domain;
using UCDArch.Core.PersistanceSupport;
using Check=UCDArch.Core.Utils.Check;

namespace CRP.Controllers.ViewModels
{
    public class DisplayProfileViewModel
    {
        public IEnumerable<Unit> Units { get; set; }
        public IEnumerable<School> Schools { get; set; }
        public DisplayProfile DisplayProfile { get; set; }

        public static DisplayProfileViewModel Create(IRepository repository)
        {
            Check.Require(repository != null, "Repository is required.");

            var viewModel = new DisplayProfileViewModel()
                                {
                                    Units = repository.OfType<Unit>().GetAll(),
                                    Schools = repository.OfType<School>().GetAll()
                                };

            return viewModel;
        }
    }
}
