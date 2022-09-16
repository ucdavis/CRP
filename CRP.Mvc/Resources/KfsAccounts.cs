using CRP.Core.Helpers;
using Microsoft.Azure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRP.Mvc.Resources
{
    public class KfsAccounts //TODO: Rename
    {
        public const string HoldingChart = "3";

        public const string HoldingAccount = "REGIHLD";

        public const string FeeChart = "3";

        public const string FeeAccount = "REGIFEE";

        //Same as holding
        public string ClearingFinancialSegmentString { get; set; } = CloudConfigurationManager.GetSetting("ClearingFinancialSegmentString").SafeToUpper();
        public string FeeFinancialSegmentString { get; set; } = CloudConfigurationManager.GetSetting("FeeFinancialSegmentString").SafeToUpper();
    }
}