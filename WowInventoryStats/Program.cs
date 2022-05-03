using WowInventoryStats.Authentication;
using WowInventoryStats.Configuration;

namespace WowInventoryStats
{
    public class Program
    {
        static AppConfiguration? Config;

        static async Task Main()
        {
            try
            {
                // Read application config
                Config = new AppConfiguration("config.json");
                // Get Access Token for Battlenet API access
                TokenAuthenticator wowAuth = new();
                await wowAuth.Authenticate(Config.Secret.ClientId, Config.Secret.ClientSecret);
                Console.WriteLine($"Successful token request, here it is: {wowAuth.Token}");
            }
            catch (AppConfigurationException ex)
            {
                Console.WriteLine($"Configuration error: {ex.Message}");
            }
            catch (AuthenticationException ex)
            {
                Console.WriteLine($"Authentication error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            // TODO: Menu
            // TODO: Game data requests
            // TODO: Game data statistics
        }
    }
}