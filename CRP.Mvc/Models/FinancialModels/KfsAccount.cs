using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRP.Mvc.Models.FinancialModels
{
    public class KfsAccount
    {
        public string chartOfAccountsCode { get; set; }
        public string organizationCode { get; set; }
        public string accountNumber { get; set; }
        public string accountName { get; set; }
        public DateTime? accountExpirationDate { get; set; }
        public bool? closed { get; set; }

        public string subFundGroupTypeCode { get; set; }

        public string subFundGroupName { get; set; }
        public string subFundGroupCode { get; set; }

        public string ProjectName { get; set; } //Different Lookup
        public string SubAccountName { get; set; } //Different Lookup

        public bool IsValidIncomeAccount
        {
            get
            {
                if (closed.HasValue && closed.Value)
                {
                    return false;
                }
                //Look at Payments subFundGroupTypeCode checks to see other income checks, but don't really apply to Registration
                

                if (string.IsNullOrWhiteSpace(subFundGroupCode))
                {
                    return false;
                }

                if (subFundGroupCode.Equals("OTHUNV", StringComparison.OrdinalIgnoreCase)) //OTHER SOURCE-UNIV RELATED EVENTS
                {
                    return true;
                }

                if (subFundGroupCode.Equals("OTHER", StringComparison.OrdinalIgnoreCase)) //OTHER SOURCES - NON RATE BASED
                {
                    return true;
                }


                return false;
            }
        }

        public static implicit operator KfsAccount(string v)
        {
            throw new NotImplementedException();
        }
    }
}