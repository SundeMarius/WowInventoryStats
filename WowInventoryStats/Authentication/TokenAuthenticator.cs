using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WowInventoryStats.Authentication
{
    public record OAuthToken
    {
        [property: JsonPropertyName("access_token")]
        public string? AccessToken { get; init; } = "";

        [property: JsonPropertyName("token_type")]
        public string? TokenType { get; init; } = "";

        [property: JsonPropertyName("expires_in")]
        public int ExpiresIn { get; init; } = 0;
    }
    public class TokenCredentials
    {
        [property: JsonPropertyName("client_id")]
        public string? ClientId { get; set; } = "";

        [property: JsonPropertyName("client_secret")]
        public string? ClientSecret { get; set; } = "";

        public TokenCredentials(string? clientId, string? clientSecret)
        {
            ClientId = clientId;
            ClientSecret = clientSecret;
        }

        public TokenCredentials()
        {
        }
    }

    public class TokenAuthenticator
    {
        public OAuthToken Token { get; private set; } = new();

        public bool IsAuthenticated { get; private set; } = false;

        public async Task Authenticate(TokenCredentials credentials)
        {
            // Authentication is successful if no exceptions are thrown
            Token = await RequestAccessToken(credentials);
            IsAuthenticated = true;
        }
        private static async Task<OAuthToken> RequestAccessToken(TokenCredentials credentials)
        {
            if (string.IsNullOrEmpty(credentials.ClientSecret))
            {
                throw new ArgumentNullException(nameof(credentials.ClientSecret));
            }
            if (string.IsNullOrEmpty(credentials.ClientId))
            {
                throw new ArgumentNullException(nameof(credentials.ClientId));
            }
            var client = new HttpClient();
            var query = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                {"grant_type", "client_credentials" },
                {"client_id", credentials.ClientId},
                {"client_secret", credentials.ClientSecret}
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
