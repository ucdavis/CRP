using UCDArch.Core.PersistanceSupport;

namespace CRP.Controllers.ViewModels
{
    public class SystemReportViewModel
    {
        public System.Array Reports { get; set; }

        public static SystemReportViewModel Create(IRepository repository)
        {
            var viewModel = new SystemReportViewModel();
            return viewModel;
        }
    }
}
