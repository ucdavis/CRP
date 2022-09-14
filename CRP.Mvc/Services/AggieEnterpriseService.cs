﻿using AggieEnterpriseApi.Types;
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
                    if (fund != "13U20")
                    {
                        rtValue.IsWarning = true;
                        rtValue.Message = $"Fund portion of the Financial Segment String must be 13U20 not {fund}";
                    }
 
                    //Check if Financial Dept roles up to Level C value AAES00C (College of Agricultural and Environmental Sciences)
                    var rollupDepts = await _aggieClient.DeptParents.ExecuteAsync(data.GlValidateChartstring.Segments.Department);
                    var dataRollupDeps = rollupDepts.ReadData();
                    if (!DoesDeptRollUp.Dept(dataRollupDeps.ErpFinancialDepartment, "AAES00C")) //TODO: Use app setting?
                    {
                        var saveWarning = rtValue.Message;
                        rtValue.IsWarning = true;
                        rtValue.Message = $"Department portion of the Financial Segment String must roll up to CAES. Dept does not: {data.GlValidateChartstring.Segments.Department} {saveWarning}";
                    }

                }

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
                        rtValue.Message = $"Department portion of the Financial Segment String must roll up to CAES. Dept(Organization) does not: {ppmSegments.Organization}";
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