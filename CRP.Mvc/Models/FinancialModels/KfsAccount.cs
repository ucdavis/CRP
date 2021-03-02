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
                //These subFundGroupTypeCode checks were copied from payments, but don't really apply to Registration
                //if (string.IsNullOrWhiteSpace(subFundGroupTypeCode))
                //{
                //    return false;
                //}

                //if (subFundGroupTypeCode == "1") //Agency Accounts
                //{
                //    return true;
                //}
                //if (subFundGroupTypeCode == "4") //Sales and Service of Teaching Hospital
                //{
                //    return true;
                //}
                //if (subFundGroupTypeCode.Equals("M", StringComparison.OrdinalIgnoreCase)) //Self Supporting Activities(Other Sources
                //{
                //    return true;
                //}

                //if (subFundGroupTypeCode.Equals("Y", StringComparison.OrdinalIgnoreCase)) //Sales and Service Educational Activities
                //{
                //    return true;
                //}

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