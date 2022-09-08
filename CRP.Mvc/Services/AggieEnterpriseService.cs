using AggieEnterpriseApi.Types;
using AggieEnterpriseApi.Validation;
using AggieEnterpriseApi;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Azure;
using AggieEnterpriseApi.Extensions;
using CRP.Mvc.Controllers.ViewModels;

namespace CRP.Mvc.Services
{
    public interface IAggieEnterpriseService
    {
        Task<bool> IsAccountValid(string financialSegmentString, bool validateCVRs = true);
        Task<AccountValidationModel> ValidateAccount(string financialSegmentString, bool validateCVRs = true);
    }
    public class AggieEnterpriseService : IAggieEnterpriseService
    {
        private readonly IAggieEnterpriseClient _aggieClient;

        public AggieEnterpriseService()
        {           
            _aggieClient = GraphQlClient.Get(CloudConfigurationManager.GetSetting("GraphQlUrl"), CloudConfigurationManager.GetSetting("GraphToken"));
        }

        public async Task<bool> IsAccountValid(string financialSegmentString, bool validateCVRs = true)
        {
            var segmentStringType = FinancialChartValidation.GetFinancialChartStringType(financialSegmentString);

            if (segmentStringType == FinancialChartStringType.Gl)
            {
                var result = await _aggieClient.GlValidateChartstring.ExecuteAsync(financialSegmentString, validateCVRs);

                var data = result.ReadData();

                var isValid = data.GlValidateChartstring.ValidationResponse.Valid;

                //if (isValid)
                //{
                //    //Is fund valid?
                //    var fund = data.GlValidateChartstring.Segments.Fund;
                //    if ("13U00,13U01,13U02".Contains(fund))//TODO: Make a configurable list of valid funds
                //    {
                //        //These three are excluded
                //        isValid = false;
                //    }
                //    else
                //    {
                //        var funds = await _aggieClient.FundParents.ExecuteAsync(fund);
                //        var dataFunds = funds.ReadData();
                //        if (DoesFundRollUp.Fund(dataFunds.ErpFund, 2, "1200C") || DoesFundRollUp.Fund(dataFunds.ErpFund, 2, "1300C") || DoesFundRollUp.Fund(dataFunds.ErpFund, 2, "5000C"))
                //        {
                //            isValid = true;
                //        }
                //        else
                //        {
                //            isValid = false;
                //        }
                //    }
                //}

                return isValid;
            }

            if (segmentStringType == FinancialChartStringType.Ppm)
            {
                var result = await _aggieClient.PpmStringSegmentsValidate.ExecuteAsync(financialSegmentString);

                var data = result.ReadData();

                var isValid = data.PpmStringSegmentsValidate.ValidationResponse.Valid;

                //TODO: Extra validation for PPM strings?

                return isValid;
            }



            return false;
        }

        public async Task<AccountValidationModel> ValidateAccount(string financialSegmentString, bool validateCVRs = true)
        {
            var rtValue = new AccountValidationModel();
            var segmentStringType = FinancialChartValidation.GetFinancialChartStringType(financialSegmentString);

            if (segmentStringType == FinancialChartStringType.Gl)
            {
                var result = await _aggieClient.GlValidateChartstring.ExecuteAsync(financialSegmentString, validateCVRs);

                var data = result.ReadData();

                rtValue.IsValid = data.GlValidateChartstring.ValidationResponse.Valid;
                if (!rtValue.IsValid)
                {
                    foreach(var err in data.GlValidateChartstring.ValidationResponse.ErrorMessages)
                    {
                        rtValue.Message = $"{rtValue.Message} {err}";
                    }
                    
                }

                //if (isValid)
                //{
                //    //Is fund valid?
                //    var fund = data.GlValidateChartstring.Segments.Fund;
                //    if ("13U00,13U01,13U02".Contains(fund))//TODO: Make a configurable list of valid funds
                //    {
                //        //These three are excluded
                //        isValid = false;
                //    }
                //    else
                //    {
                //        var funds = await _aggieClient.FundParents.ExecuteAsync(fund);
                //        var dataFunds = funds.ReadData();
                //        if (DoesFundRollUp.Fund(dataFunds.ErpFund, 2, "1200C") || DoesFundRollUp.Fund(dataFunds.ErpFund, 2, "1300C") || DoesFundRollUp.Fund(dataFunds.ErpFund, 2, "5000C"))
                //        {
                //            isValid = true;
                //        }
                //        else
                //        {
                //            isValid = false;
                //        }
                //    }
                //}

                return rtValue;
            }

            if (segmentStringType == FinancialChartStringType.Ppm)
            {
                var result = await _aggieClient.PpmStringSegmentsValidate.ExecuteAsync(financialSegmentString);

                var data = result.ReadData();

                rtValue.IsValid = data.PpmStringSegmentsValidate.ValidationResponse.Valid;
                if (!rtValue.IsValid)
                {
                    foreach (var err in data.PpmStringSegmentsValidate.ValidationResponse.ErrorMessages)
                    {
                        rtValue.Message = $"{rtValue.Message} {err}";
                    }

                }


                //TODO: Extra validation for PPM strings?

                return rtValue;
            }



            return rtValue;
        }

        private PpmSegmentInput ConvertToPpmSegmentInput(PpmSegments segments)
        {
            return new PpmSegmentInput
            {
                Award = segments.Award,
                Organization = segments.Organization,
                Project = segments.Project,
                Task = segments.Task,
                ExpenditureType = segments.ExpenditureType,
                FundingSource = segments.FundingSource,
            };
        }
    }
}