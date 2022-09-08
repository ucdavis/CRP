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
        Task<AccountValidationModel> ValidateAccount(string financialSegmentString, bool validateCVRs = true);
    }
    public class AggieEnterpriseService : IAggieEnterpriseService
    {
        private readonly IAggieEnterpriseClient _aggieClient;

        public AggieEnterpriseService()
        {           
            _aggieClient = GraphQlClient.Get(CloudConfigurationManager.GetSetting("GraphQlUrl"), CloudConfigurationManager.GetSetting("GraphToken"));
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

                if (rtValue.IsValid)
                {
                    var fund = data.GlValidateChartstring.Segments.Fund;
                    if(fund != "13U20")
                    {
                        rtValue.IsWarning = true;
                        rtValue.Message = $"Fund portion of the Financial Segment String must be 13U20 not {fund}";
                    }
                    else
                    {
                        //Check if Financial Dept roles up to Level C value AAES00C (College of Agricultural and Environmental Sciences)
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
                // Task will need "glPostingFundCode": 13U20, to be valid from non admin side.

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