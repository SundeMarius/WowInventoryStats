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

    public record TokenCredentials
    {
        [property: JsonPropertyName("client_id")]
        public string? ClientId { get; init; } = "";

        [property: JsonPropertyName("client_secret")]
        public string? ClientSecret { get; init; } = "";

        public TokenCredentials()
        {
        } 
        public TokenCredentials(string? clientId, string? clientSecret)
        {
            ClientId = clientId;
            ClientSecret = clientSecret;
        }
        public bool Populated()
        {
            return !string.IsNullOrEmpty(ClientId) && !string.IsNullOrEmpty(ClientSecret);
        }
    }

    public class TokenAuthenticator
    {
        public OAuthToken Token { get; private set; } = new();

        public bool IsAuthenticated { get; private set; } = false;

        public async Task Authenticate(TokenCredentials credentials)
        {
            try
            {
                // Authentication is successful if no exceptions are thrown
                Token = await RequestAccessToken(credentials);
                IsAuthenticated = true;
            }
            catch (Exception ex)
            {
                throw new AuthenticationException(ex.Message, ex.GetBaseException());
            }
        }
        private static async Task<OAuthToken> RequestAccessToken(TokenCredentials credentials)
        {
            if (string.IsNullOrEmpty(credentials.ClientSecret))
            {
                throw new ArgumentNullException(nameof(credentials));
            }
            if (string.IsNullOrEmpty(credentials.ClientId))
            {
                throw new ArgumentNullException(nameof(credentials));
            }
            var query = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                {"grant_type", "client_credentials" },
                {"client_id", credentials.ClientId},
                {"client_secret", credentials.ClientSecret}
            });
            var response = await new HttpClient().PostAsync("https://us.battle.net/oauth/token", query);
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new AuthenticationException("invalid credentials");
            }
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<OAuthToken>(json)!;
        }
    }
    public class AuthenticationException : Exception
    {
        public AuthenticationException() : base()
        {
        }
        public AuthenticationException(string message) : base(message)
        {
        }
        public AuthenticationException(string message, Exception ex) : base(message, ex)
        {
        }
    }
}
