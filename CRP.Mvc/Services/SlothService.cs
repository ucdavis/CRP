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
        Task<Transaction> GetTransactionsByProcessorId(string id);

        Task<IList<Transaction>> GetTransactionsByKfsKey(string kfskey);

        Task<CreateSlothTransactionResponse> CreateTransaction(CreateTransaction transaction);

        Task<Transaction> Test();
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

        public async Task<Transaction> GetTransactionsByProcessorId(string id)
        {
            //Note: The transaction currently being used is CRP not Sloth so will not work.
            using (var client = GetHttpClient())
            {
                var escapedId = Uri.EscapeUriString(id);
                var url = $"transactions/processor/{escapedId}";

                var response = await client.GetAsync(url);
                var result = await response.GetContentOrNullAsync<Transaction>(); //Think this is the wrong transaction...
                return result;
            }
        }

        public async Task<IList<Transaction>> GetTransactionsByKfsKey(string kfskey)
        {
            //Note: The transaction currently being used is CRP not Sloth so will not work.
            using (var client = GetHttpClient())
            {
                var escapedKey = Uri.EscapeUriString(kfskey);
                var url = $"transactions/kfskey/{escapedKey}";

                var response = await client.GetAsync(url);
                var result = await response.GetContentOrNullAsync<IList<Transaction>>();
                return result;
            }
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

        public async Task<Transaction> Test()
        {
            //Note: The transaction currently being used is CRP not Sloth so will not work.
            using (var client = GetHttpClient())
            {
                var url = $"transactions";

                var response = await client.GetAsync(url);
                var result = await response.GetContentOrNullAsync<Transaction>();
                return result;
            }
        }

        private HttpClient GetHttpClient()
        {
            var client = new HttpClient()
            {
                BaseAddress = new Uri(_settings.BaseUrl),
            };
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