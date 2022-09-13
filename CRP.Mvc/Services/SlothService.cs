using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using CRP.Core.Domain;
using CRP.Core.Helpers;
using CRP.Mvc.Helpers;
using CRP.Mvc.Models.Configuration;
using CRP.Mvc.Models.Sloth;
using Microsoft.Azure;
using Serilog;

namespace CRP.Mvc.Services
{
    public interface ISlothService
    {

        Task<CreateSlothTransactionResponse> CreateTransaction(CreateTransaction transaction);

    }

    public class SlothService : ISlothService
    {
        private readonly SlothSettings _settings;

        public SlothService()
        {
            _settings = new SlothSettings();
            _settings.BaseUrl = CloudConfigurationManager.GetSetting("Sloth.BaseUrl");
            _settings.ApiKey = CloudConfigurationManager.GetSetting("Sloth.ApiKey");

            _settings.RequireKfs = CloudConfigurationManager.GetSetting("RequireKfs").SafeToUpper() == "TRUE";
            _settings.BaseUrlV2 = CloudConfigurationManager.GetSetting("Sloth.BaseUrlV2");
        }


        public async Task<CreateSlothTransactionResponse> CreateTransaction(CreateTransaction transaction)
        {
            using (var client = GetHttpClient())
            {
                var url = "transactions";

                var response = await client.PostAsJsonAsync(url, transaction);
                try
                {
                    return await response.GetContentOrNullAsync<CreateSlothTransactionResponse>();
                }
                catch (Exception e)
                {
                    Log.Error(e, "Exception trying to create transaction in sloth");
                    Log.Information($"DepositNotify - Error info: {await response.GetContentOrEmptyAsync()}");
                    throw;
                }
                
                
            }
        }


        private HttpClient GetHttpClient()
        {
            var client = new HttpClient()
            {
                BaseAddress = new Uri(_settings.BaseUrl),
            };
            if (!_settings.RequireKfs)
            {
                client.BaseAddress = new Uri(_settings.BaseUrlV2);
            }
                
                
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            client.DefaultRequestHeaders.Add("X-Auth-Token", _settings.ApiKey);

            return client;
        }
    }

    public class CreateSlothTransactionResponse
    {
        public string Id { get; set; }
    }
}