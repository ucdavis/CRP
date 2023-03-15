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
using MvcContrib.FluentHtml.Elements;

namespace CRP.Mvc.Services
{
    public interface IAggieEnterpriseService
    {
        Task<AccountValidationModel> ValidateAccount(string financialSegmentString, bool validateCVRs = true);
    }
    public class AggieEnterpriseService : IAggieEnterpriseService
    {


        public async Task<AccountValidationModel> ValidateAccount(string financialSegmentString, bool validateCVRs = true)
        {

            //var _aggieClient = GraphQlClient.Get(CloudConfigurationManager.GetSetting("AggieEnterprise:GraphQlUrl"), CloudConfigurationManager.GetSetting("AggieEnterprise:GraphToken"));
            var _aggieClient = GraphQlClient.Get(
                CloudConfigurationManager.GetSetting("AggieEnterprise:GraphQlUrl"), 
                CloudConfigurationManager.GetSetting("AggieEnterprise:TokenEndpoint"), 
                CloudConfigurationManager.GetSetting("AggieEnterprise:ConsumerKey"),
                CloudConfigurationManager.GetSetting("AggieEnterprise:ConsumerSecret"), 
                $"{CloudConfigurationManager.GetSetting("AggieEnterprise:ScopeApp")}-{CloudConfigurationManager.GetSetting("AggieEnterprise:ScopeEnv")}");


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
                        rtValue.Messages.Add(err);
                    }
                }

                if (rtValue.IsValid)
                {
                    var fund = data.GlValidateChartstring.Segments.Fund;
                    if (fund != "13U20")
                    {
                        rtValue.IsWarning = true;
                        rtValue.Messages.Add($"Fund portion of the Financial Segment String must be 13U20 not {fund}");
                    }
 
                    //Check if Financial Dept roles up to Level C value AAES00C (College of Agricultural and Environmental Sciences)
                    var rollupDepts = await _aggieClient.DeptParents.ExecuteAsync(data.GlValidateChartstring.Segments.Department);
                    var dataRollupDeps = rollupDepts.ReadData();
                    if (!DoesDeptRollUp.Dept(dataRollupDeps.ErpFinancialDepartment, "AAES00C")) //TODO: Use app setting?
                    {
                        rtValue.IsWarning = true;
                        rtValue.Messages.Add($"Department portion of the Financial Segment String must roll up to CAES. Dept does not: {data.GlValidateChartstring.Segments.Department}");
                    }
                    
                    if (data.GlValidateChartstring.Segments.Account != "410004")
                    {
                        rtValue.IsWarning = true;
                        rtValue.Messages.Add($"Natural Account portion of the Financial Segment String must be 410004 not {data.GlValidateChartstring.Segments.Account}");
                    }
                }


                return rtValue;
            }

            if (segmentStringType == FinancialChartStringType.Ppm)
            {
                //TODO: Have a config setting to see if we allow PPM? (May not be working for go-live date)

                
                var result = await _aggieClient.PpmStringSegmentsValidate.ExecuteAsync(financialSegmentString);

                var data = result.ReadData();

                rtValue.IsValid = data.PpmStringSegmentsValidate.ValidationResponse.Valid;
                if (!rtValue.IsValid)
                {
                    foreach (var err in data.PpmStringSegmentsValidate.ValidationResponse.ErrorMessages)
                    {
                        rtValue.Messages.Add(err);
                    }
                }
                else
                {
                    // Validate the org rolls up to caes? If so, need to parse it out of the string, or grab the first word of the returned organization 
                    //I think this is correct, but we will need to test...
                    var ppmSegments = FinancialChartValidation.GetPpmSegments(financialSegmentString);
                    var rollupDepts = await _aggieClient.DeptParents.ExecuteAsync(ppmSegments.Organization);
                    var dataRollupDeps = rollupDepts.ReadData();
                    if (!DoesDeptRollUp.Dept(dataRollupDeps.ErpFinancialDepartment, "AAES00C")) //TODO: Use app setting?
                    {
                        rtValue.IsWarning = true;
                        rtValue.Messages.Add($"Department portion of the Financial Segment String must roll up to CAES. Dept(Organization) does not: {ppmSegments.Organization}");
                    }

                    // Task will need "glPostingFundCode": 13U20, to be valid from non admin side.
                    var checkFundCode = await _aggieClient.PpmTaskByProjectNumberAndTaskNumber.ExecuteAsync(ppmSegments.Project, ppmSegments.Task);
                    var checkFundCodeData = checkFundCode.ReadData();
                    if(checkFundCodeData == null)
                    {
                        rtValue.IsValid = false;
                        rtValue.Messages.Add("Unable to check Task's GL funding code.");
                    }
                    else
                    {
                        if(checkFundCodeData.PpmTaskByProjectNumberAndTaskNumber.GlPostingFundCode == null)
                        {
                            rtValue.IsValid = false;
                            rtValue.Messages.Add("GlPostingFundCode is null for this Task.");
                        }
                        else if (checkFundCodeData.PpmTaskByProjectNumberAndTaskNumber.GlPostingFundCode != "13U20")
                        {
                            rtValue.IsWarning = false;
                            rtValue.Messages.Add($"Task's GL funding code must be 13U20 not {checkFundCodeData.PpmTaskByProjectNumberAndTaskNumber.GlPostingFundCode}");
                        }
                    }

                    if(ppmSegments.ExpenditureType != "410004")
                    {
                        rtValue.IsValid = false;
                        rtValue.Messages.Add($"Expenditure Type (Natural Account) must be 410004 not {ppmSegments.ExpenditureType}");
                    }
                }





                return rtValue;
            }

            if (segmentStringType == FinancialChartStringType.Invalid)
            {
                rtValue.IsValid = false;
                rtValue.Messages.Add("Invalid Financial Segment String format");
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