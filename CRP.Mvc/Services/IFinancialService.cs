using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CRP.Core.Domain;
using CRP.Core.Helpers;
using CRP.Mvc.Controllers.ViewModels;
using CRP.Mvc.Models.FinancialModels;
using Microsoft.Azure;
using Newtonsoft.Json;
using NHibernate.Cfg;
using Serilog;

namespace CRP.Mvc.Services
{
    public interface IFinancialService
    {
        Task<string> GetAccountName(string chart, string account, string subAccount);
        Task<KfsAccount> GetAccount(string chart, string account);
        Task<bool> IsAccountValid(string chart, string account, string subAccount);
        Task<bool> IsObjectValid(string chart, string objectCode);
        Task<bool> IsSubObjectValid(string chart, string account, string objectCode, string subObject);
        Task<bool> IsProjectValid(string project);
        Task<string> GetProjectName(string project);
        Task<string> GetSubAccountName(string chart, string account, string subAccount);
        Task<string> GetObjectName(string chart, string objectCode);
        Task<string> GetSubObjectName(string chart, string account, string objectCode, string subObject);

        Task<bool> IsOrgChildOfOrg(string childChart, string childOrg, string parentChart, string parentOrg);

        Task<AccountValidationModel> IsAccountValidForRegistration(FinancialAccount account);
        Task<AccountValidationModel> IsAccountValidForRegistration(string account);

    }

    public class FinancialService : IFinancialService
    {
        private static readonly string FinancialLookupUrl = CloudConfigurationManager.GetSetting("FinancialLookupUrl");
        private static readonly string FinancialOrgLookupUrl = CloudConfigurationManager.GetSetting("FinancialOrgLookupUrl");
        private IAggieEnterpriseService _aggieEnterpriseService;

        private readonly bool RequireKfs;

        public FinancialService(IAggieEnterpriseService aggieEnterpriseService)
        {
            _aggieEnterpriseService = aggieEnterpriseService;
            RequireKfs = CloudConfigurationManager.GetSetting("RequireKfs").SafeToUpper() == "TRUE";
        }
        

        public async Task<string> GetAccountName(string chart, string account, string subAccount)
        {
            //https://kfs.ucdavis.edu/kfs-prd/api-docs/ //Documentation - Maybe under a new URL after Nov 26, 2020
            string url;
            string validationUrl;
            if (!String.IsNullOrWhiteSpace(subAccount))
            {
                validationUrl =
                    $"{FinancialLookupUrl}/subaccount/{chart}/{account}/{subAccount}/isvalid";
                url =
                    $"{FinancialLookupUrl}/subaccount/{chart}/{account}/{subAccount}/name";
            }
            else
            {
                validationUrl = $"{FinancialLookupUrl}/account/{chart}/{account}/isvalid";
                url = $"{FinancialLookupUrl}/account/{chart}/{account}/name";
            }

            using (var client = new HttpClient())
            {
                var validationResponse = await client.GetAsync(validationUrl);
                validationResponse.EnsureSuccessStatusCode();

                var validationContents = await validationResponse.Content.ReadAsStringAsync();
                if (!JsonConvert.DeserializeObject<bool>(validationContents))
                {
                    Log.Information($"Account not valid {account}");
                    throw new Exception("Invalid Account");
                }


                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();


                var contents = await response.Content.ReadAsStringAsync();
                return contents;
            }

        }

        public async Task<bool> IsAccountValid(string chart, string account, string subAccount)
        {
            string validationUrl;
            if (!String.IsNullOrWhiteSpace(subAccount))
            {
                validationUrl =
                    $"{FinancialLookupUrl}/subaccount/{chart}/{account}/{subAccount}/isvalid";
            }
            else
            {
                validationUrl = $"{FinancialLookupUrl}/account/{chart}/{account}/isvalid";
            }

            using (var client = new HttpClient())
            {
                var validationResponse = await client.GetAsync(validationUrl);
                validationResponse.EnsureSuccessStatusCode();

                var validationContents = await validationResponse.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<bool>(validationContents); //TEST THIS!!!
            }
        }

        /// <summary>
        /// Current fiscal year
        /// </summary>
        /// <param name="chart"></param>
        /// <param name="account"></param>
        /// <param name="subAccount"></param>
        /// <returns></returns>
        public async Task<bool> IsObjectValid(string chart, string objectCode)
        {
            string validationUrl = $"{FinancialLookupUrl}/object/{chart}/{objectCode}/isvalid";


            using (var client = new HttpClient())
            {
                var validationResponse = await client.GetAsync(validationUrl);
                validationResponse.EnsureSuccessStatusCode();

                var validationContents = await validationResponse.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<bool>(validationContents); //TEST THIS!!!
            }
        }

        /// <summary>
        /// GET /fau/subobject/{chart}/{account}/{object}/{subobject}/name
        /// </summary>
        /// <param name="chart"></param>
        /// <param name="account"></param>
        /// <param name="objectCode"></param>
        /// <param name="subObject"></param>
        /// <returns></returns>
        public async Task<bool> IsSubObjectValid(string chart, string account, string objectCode, string subObject)
        {
            string validationUrl = $"{FinancialLookupUrl}/subobject/{chart}/{account}/{objectCode}/{subObject}/isvalid";


            using (var client = new HttpClient())
            {
                var validationResponse = await client.GetAsync(validationUrl);
                validationResponse.EnsureSuccessStatusCode();

                var validationContents = await validationResponse.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<bool>(validationContents); //TEST THIS!!!
            }
        }

        public async Task<bool> IsProjectValid(string project)
        {
            string url = $"{FinancialLookupUrl}/project/{project}/isvalid";


            using (var client = new HttpClient())
            {
                var validationResponse = await client.GetAsync(url);
                validationResponse.EnsureSuccessStatusCode();

                var validationContents = await validationResponse.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<bool>(validationContents); //TEST THIS!!!
            }
        }

        public async Task<KfsAccount> GetAccount(string chart, string account)
        {
            string url = $"{FinancialLookupUrl}/account/{chart}/{account}";
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var contents = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<KfsAccount>(contents);
            }
        }

        public async Task<string> GetProjectName(string project)
        {
            string url = $"{FinancialLookupUrl}/project/{project}/name";
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var contents = await response.Content.ReadAsStringAsync();
                return contents.Trim('"');
            }
        }

        public async Task<string> GetSubAccountName(string chart, string account, string subAccount)
        {
            string url = $"{FinancialLookupUrl}/subaccount/{chart}/{account}/{subAccount}/name";
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var contents = await response.Content.ReadAsStringAsync();
                return contents.Trim('"');
            }
        }

        public async Task<string> GetObjectName(string chart, string objectCode)
        {
            string url = $"{FinancialLookupUrl}/object/{chart}/{objectCode}/name";
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var contents = await response.Content.ReadAsStringAsync();
                return contents.Trim('"');
            }
        }
        //GET /fau/subobject/{chart}/{account}/{object}/{subobject}/name
        public async Task<string> GetSubObjectName(string chart, string account, string objectCode, string subObject)
        {
            string url = $"{FinancialLookupUrl}/subobject/{chart}/{account}/{objectCode}/{subObject}/name";
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var contents = await response.Content.ReadAsStringAsync();
                return contents.Trim('"');
            }
        }

        public async Task<bool> IsOrgChildOfOrg(string childChart, string childOrg, string parentChart, string parentOrg)
        {
            //https://financials.api.adminit.ucdavis.edu/org/3/OSBC/ischildof/3/AAES
            string url = $"{FinancialOrgLookupUrl}/{childChart}/{childOrg}/ischildof/{parentChart}/{parentOrg}";


            using (var client = new HttpClient())
            {
                var validationResponse = await client.GetAsync(url);
                validationResponse.EnsureSuccessStatusCode();

                var validationContents = await validationResponse.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<bool>(validationContents); //TEST THIS!!!
            }
        }

        public async Task<AccountValidationModel> IsAccountValidForRegistration(FinancialAccount account)
        {
            var rtValue = new AccountValidationModel();
            
            if(RequireKfs)
            {
                if (!await IsAccountValid(account.Chart, account.Account, account.SubAccount))
                {
                    rtValue.IsValid = false;
                    rtValue.Field = "Account";
                    rtValue.Message = "Valid Account Not Found. (Invalid or Expired).";

                    return rtValue;
                }

                if (!string.IsNullOrWhiteSpace(account.Project) && !await IsProjectValid(account.Project))
                {
                    rtValue.IsValid = false;
                    rtValue.Field = "Project";
                    rtValue.Message = "Project Not Valid.";
                    return rtValue;
                }


                var accountLookup = new KfsAccount();
                accountLookup = await GetAccount(account.Chart, account.Account);
                if (!accountLookup.IsValidIncomeAccount)
                {
                    rtValue.IsValid = false;
                    rtValue.Field = "Account";
                    rtValue.Message = "Not An Income Account/Events Account.";
                    return rtValue;
                }


                //Ok, not check if the org rolls up to our orgs
                if (await IsOrgChildOfOrg(accountLookup.chartOfAccountsCode,
                        accountLookup.organizationCode, "3", "AAES") ||
                    await IsOrgChildOfOrg(accountLookup.chartOfAccountsCode,
                        accountLookup.organizationCode,
                        "L", "AAES"))
                {
                    rtValue.IsValid = true;
                }
                else
                {
                    rtValue.IsValid = false;
                    rtValue.Field = "Account";
                    rtValue.Message = "Account not in CAES org.";
                }
            }
            else
            {
                rtValue = await _aggieEnterpriseService.ValidateAccount(account.FinancialSegmentString);
            }

            return rtValue;
        }

        public async Task<AccountValidationModel> IsAccountValidForRegistration(string account)
        {
            var rtValue = new AccountValidationModel();
            if (RequireKfs)
            {
                try
                {
                    account = account.Trim();
                    var financialAccount = new FinancialAccount();
                    var delimiter = new string[] { "-" };
                    var accountArray = account.Split(delimiter, StringSplitOptions.None);
                    if (accountArray.Length < 2)
                    {
                        rtValue.IsValid = false;
                        rtValue.Message = "Need chart and account";
                        rtValue.Field = "Account";
                        return rtValue;
                    }

                    financialAccount.Chart = accountArray[0].ToUpper();
                    financialAccount.Account = accountArray[1].ToUpper();
                    if (accountArray.Length > 2)
                    {
                        financialAccount.SubAccount = accountArray[2].ToUpper();
                    }

                    rtValue = await IsAccountValidForRegistration(financialAccount);
                    rtValue.FinancialAccount = financialAccount;
                }
                catch
                {
                    rtValue.IsValid = false;
                    rtValue.Message = "Unable to parse account string";
                    rtValue.Field = "Account";
                }
            }
            else
            {
                rtValue = await _aggieEnterpriseService.ValidateAccount(account);
            }
            return rtValue;
        }
    }
}