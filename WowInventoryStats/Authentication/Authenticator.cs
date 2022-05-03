using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WowInventoryStats.Authentication
{
    public record OAuthToken
    (
        [property: JsonPropertyName("access_token")] string AccessToken = "",
        [property: JsonPropertyName("token_type")] string TokenType = "",
        [property: JsonPropertyName("expires_in")] int ExpiresIn = 0
    );

    public class TokenAuthenticator
    {
        public OAuthToken Token { get; private set; } = new();

        public bool IsAuthenticated = false;

        public async Task Authenticate(string clientID, string clientSecret)
        {
            // Authentication is successful if no exceptions are thrown
            Token = await RequestAccessToken(clientID, clientSecret);
            IsAuthenticated = true;
        }
        private static async Task<OAuthToken> RequestAccessToken(string clientID, string clientSecret)
        {
            if (string.IsNullOrEmpty(clientSecret))
            {
                throw new ArgumentNullException("client Secret can not be null or empty");
            }
            if (string.IsNullOrEmpty(clientID))
            {
                throw new ArgumentNullException("client ID can not be null or empty");
            }
            var client = new HttpClient();
            var query = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                {"grant_type", "client_credentials" },
                {"client_id", clientID},
                {"client_secret", clientSecret}
            });
            var response = await client.PostAsync("https://us.battle.net/oauth/token", query);
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new AuthenticationException("invalid credentials");
            }
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<OAuthToken>(result)!;
        }
    }
    public class AuthenticationException : Exception
    {
        public AuthenticationException(string message) : base(message)
        {
        }
    }
}
