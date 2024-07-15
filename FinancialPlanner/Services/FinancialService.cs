using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace FinancialPlanner.Services
{
    public class FinancialService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string AccessTokenSessionKey = "AccessToken";

        public FinancialService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }

        private string AccessToken
        {
            get => _httpContextAccessor.HttpContext.Session.GetString(AccessTokenSessionKey);
            set => _httpContextAccessor.HttpContext.Session.SetString(AccessTokenSessionKey, value);
        }

        public async Task<string> GetTokenAsync(string secretId, string secretKey)
        {
            var requestUrl = "https://bankaccountdata.gocardless.com/api/v2/token/new/";
            var requestData = new
            {
                secret_id = secretId,
                secret_key = secretKey
            };
            var content = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(requestUrl, content);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(responseContent);
            AccessToken = tokenResponse.Access; // Store the access token in session

            return responseContent;
        }

        public async Task EnsureTokenAsync(string secretId, string secretKey)
        {
            if (string.IsNullOrEmpty(AccessToken))
            {
                await GetTokenAsync(secretId, secretKey);
            }
        }

        public async Task<string> GetInstitutionsAsync()
        {
            if (string.IsNullOrEmpty(AccessToken))
            {
                throw new System.InvalidOperationException("Access token is not set. Call GetTokenAsync first.");
            }

            var requestUrl = "https://bankaccountdata.gocardless.com/api/v2/institutions/?country=gb";

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {AccessToken}");
            _httpClient.DefaultRequestHeaders.Add("accept", "application/json");

            var response = await _httpClient.GetAsync(requestUrl);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            return responseContent;
        }

        public async Task<string> CreateAgreementAsync(string institutionId)
        {
            if (string.IsNullOrEmpty(AccessToken))
            {
                throw new System.InvalidOperationException("Access token is not set. Call GetTokenAsync first.");
            }

            var requestUrl = "https://bankaccountdata.gocardless.com/api/v2/agreements/enduser/";
            var requestData = new
            {
                institution_id = institutionId,
                max_historical_days = 365,
                access_valid_for_days = 30,
                access_scope = new[] { "balances", "details", "transactions" }
            };
            var content = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {AccessToken}");
            _httpClient.DefaultRequestHeaders.Add("accept", "application/json");

            var response = await _httpClient.PostAsync(requestUrl, content);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            return responseContent;
        }
    }

    public class TokenResponse
    {
        [JsonProperty("access")]
        public string Access { get; set; }

        [JsonProperty("access_expires")]
        public int AccessExpires { get; set; }

        [JsonProperty("refresh")]
        public string Refresh { get; set; }

        [JsonProperty("refresh_expires")]
        public int RefreshExpires { get; set; }
    }
}
