using System.Configuration;
using WowInventoryStats.Authentication;

namespace WowInventoryStats
{
    public class Program
    {
        static TokenAuthenticator wowAuth = new();

        static async Task Main()
        {
            try
            {
                // Get Access Token for Battlenet API access
                var clientId = ConfigurationManager.AppSettings["client_id"]!;
                var clientSecret = ConfigurationManager.AppSettings["client_secret"]!;
                OAuthToken token = await wowAuth.Authenticate(clientId, clientSecret);
                Console.WriteLine($"Successful token request, here it is: {token}");
            }
            catch (AuthenticationException ex)
            {
                Console.WriteLine($"Authentication error: {ex.Message}");
            }
            // TODO: Menu
            // TODO: Game data requests
            // TODO: Game data statistics
        }
    }
}