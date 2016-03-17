using System.Collections.Generic;
using UCDArch.Core.PersistanceSupport;

namespace CRP.Controllers.ViewModels
{
    public class SystemReportViewModel
    {
        public System.Array Reports { get; set; }
        public int? SelectedReport { get; set; }
        public IEnumerable<SystemReportData> SystemReportData { get; set; }
        

        public static SystemReportViewModel Create(IRepository repository)
        {
            var viewModel = new SystemReportViewModel();
            viewModel.SelectedReport = null;
            return viewModel;
        }
    }

    public class SystemReportData
    {
        public SystemReportData(string name, decimal value)
        {
            Name = name;
            Value = value;
        }
        public SystemReportData(string name, decimal value, string format)
        {
            Name = name;
            Value = value;
            ValueFormat = format;
        }
        public string Name { get; set; }
        public decimal Value { get; set; }
        public string ValueFormat { get; set; }
    }
}
