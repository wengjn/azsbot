using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Internals.Fibers;
using Zummer.Models.Search;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Net.Http;
using System.Net.Http.Headers;
using System;
using Newtonsoft.Json;
using System.Text;
using System.Linq;
using System.Diagnostics;

namespace Zummer.Services
{
    /// <summary>
    /// Responsible for calling ADS
    /// </summary>
    internal sealed class AzureDiagnosticService : IAzureDiagnosticService
    {
        private const string ADSEndpoint = "https://api.diagnosticstest.msftcloudes.com/";

        private const string TokenResource = "https://microsoft.onmicrosoft.com/azurediagnostic";

        private const string Authority = "https://login.windows.net/microsoft.onmicrosoft.com";

        private const string ClientId = "XXX";

        private const string ClientKey = "XXX";

        private AuthenticationResult authenticationResult;

        private static readonly Dictionary<string, string> Headers = new Dictionary<string, string>();

        private readonly HttpClient httpClient;

        public AzureDiagnosticService()
        {
            this.httpClient = new HttpClient() { BaseAddress = new Uri(ADSEndpoint) };
            this.httpClient.DefaultRequestHeaders.Accept.Clear();
            this.httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            this.httpClient.DefaultRequestHeaders.Add("User-Agent", "AzsBot");
        }

        public async Task<string> GetDiagnosticResultWith(string requestId)
        {
            var urlBuilder = new DiagnosticServiceUrlBuilder(DiagnosticServiceUrlBuilder.Service.DiagnosticData);
            urlBuilder.AddParam("id", requestId);
            var httpRequest = await this.CreateHttpRequestMessage(HttpMethod.Get, urlBuilder.ToString());
            var responseString = await this.SendMessageAndGetResponse(httpRequest);
            return responseString;
        }

        public async Task<string> StartDiagnosticRequest(string symptom, Dictionary<string, string> parameters)
        {
            var response = await this.GetRequestId(symptom, parameters);
            return response.RequestId;
        }

        public async Task<DiagnosticResponse> GetDataByPollModeAsync(string symptom, Dictionary<string, string> parameters, int maxTimeInMs = 2000000, int retryTime = 500, bool usingCachedData = true, int maxCachedDuration = 600)
        {
            var responseData = new DiagnosticResponse();
            var sw = Stopwatch.StartNew();
            var response = await this.GetRequestId(symptom, parameters);
            var requestId = response.RequestId;

            while (sw.ElapsedMilliseconds < maxTimeInMs)
            {
                responseData = await this.GetDataByRequestId(requestId);
                if (responseData?.Status != null
                    && responseData.Status != ProcessingStatus.Running
                    && responseData.Status != ProcessingStatus.Queued)
                {
                    break;
                }

                if (sw.ElapsedMilliseconds > maxTimeInMs)
                {
                    break;
                }

                await Task.Delay(retryTime);
            }

            if (responseData.Status != ProcessingStatus.Succeeded)
            {
                throw new Exception("Error from ADS. " + (responseData.Message ?? responseData.Error ?? $"Status of {symptom} run is {responseData.Status}."));
            }

            return responseData;
            
        }

        private async Task<HttpRequestMessage> CreateHttpRequestMessage(HttpMethod method, string api, DiagnosticRequest dsRequest = null)
        {
            var message = new HttpRequestMessage(method, api);
            var activityId = new Guid();
            if (dsRequest != null)
            {
                message.Content = new StringContent(JsonConvert.SerializeObject(dsRequest), Encoding.UTF8, "application/json");
            }

            message.Headers.Add("X-TraceSession-Id", activityId.ToString());
            await this.GetTokenForDiagnosticCall();

            message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", this.authenticationResult.AccessToken);

            return message;
        }

        private async Task<string> SendMessageAndGetResponse(HttpRequestMessage message)
        {
            var response = await this.httpClient.SendAsync(message);
            var responseString = await response.Content.ReadAsStringAsync();
            return responseString;
        }

        private async Task<DiagnosticResponse> GetDataByRequestId(string requestId)
        {
            var urlBuilder = new DiagnosticServiceUrlBuilder(DiagnosticServiceUrlBuilder.Service.DiagnosticData);
            urlBuilder.AddParam("id", requestId);
            var httpRequest = await this.CreateHttpRequestMessage(HttpMethod.Get, urlBuilder.ToString());
            var responseString = await this.SendMessageAndGetResponse(httpRequest);
            return JsonConvert.DeserializeObject<DiagnosticResponse>(responseString);
        }

        private async Task<DiagnosticResponse> GetRequestId(string symptom, Dictionary<string, string> parameters, List<string> tags = null, bool usingCachedData = true, int maxCachedDuration = 600)
        {
            var urlBuilder = new DiagnosticServiceUrlBuilder(DiagnosticServiceUrlBuilder.Service.DiagnosticData);
            var httpRequest = await this.CreateHttpRequestMessage(HttpMethod.Post, urlBuilder.ToString(), BuildRequest(symptom, parameters, tags, usingCachedData, maxCachedDuration));
            var responseString = await this.SendMessageAndGetResponse(httpRequest);
            return JsonConvert.DeserializeObject<DiagnosticResponse>(responseString);
        }

        private static DiagnosticRequest BuildRequest(string symptom, Dictionary<string, string> parameters, List<string> tags = null, bool usingCachedData = true, int maxCachedDuration = 600)
        {
            return new DiagnosticRequest { SymptomId = symptom, Parameters = parameters, Tags = tags, AllowExistingResult = usingCachedData, ResultLifetimeInSeconds = maxCachedDuration };
        }

        private static string DictionaryToDebugString(Dictionary<string, string> dict)
        {
            return dict != null ? string.Join(",", dict.Select(x => x.Key + "=" + x.Value)) : string.Empty;
        }

        private static string ListToDebugString(List<string> list)
        {
            return list != null ? string.Join(",", list) : string.Empty;
        }

        private async Task GetTokenForDiagnosticCall()
        {
            var authenticationContext = new AuthenticationContext(Authority);
            this.authenticationResult = await authenticationContext.AcquireTokenAsync(TokenResource, new ClientCredential(ClientId, ClientKey));
        }
    }
}