using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace SmartVault.Program
{
    public class OAuthIntegrationImplementation
    {
        private readonly HttpClient _httpClient;

        public OAuthIntegrationImplementation(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GetAccessTokenAsync(string oauthProvider, string clientId, string clientSecret, string redirectUri, string code)
        {
            // Build the OAuth token request
            var tokenRequest = new HttpRequestMessage(HttpMethod.Post, oauthProvider + "/oauth/token")
            {
                Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "grant_type", "authorization_code" },
                { "code", code },
                { "client_id", clientId },
                { "client_secret", clientSecret },
                { "redirect_uri", redirectUri }
            })
            };

            // Send the token request and get the access token
            var tokenResponse = await _httpClient.SendAsync(tokenRequest);
            tokenResponse.EnsureSuccessStatusCode();
            var tokenJson = await tokenResponse.Content.ReadAsStringAsync();
            var tokenData = JsonSerializer.Deserialize<Dictionary<string, string>>(tokenJson);
            return tokenData["access_token"];
        }
    }
}
