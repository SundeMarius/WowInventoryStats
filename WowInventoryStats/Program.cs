using WowInventoryStats.Authentication;
using WowInventoryStats.Configuration;

namespace WowInventoryStats
{
    public class Program
    {
        static async Task Main()
        {
            AppConfiguration? AppConfig;
            TokenAuthenticator wowAuth = new();
            // Read application config and get access token from Blizzard
            try
            {
                AppConfig = new AppConfiguration();
                while (!AppConfig.Parameters.Credentials.Populated())
                {
                    Console.Write("Enter client id: ");
                    var clientId = Console.ReadLine();
                    Console.Write("Enter client secret: ");
                    var clientSecret = Console.ReadLine();
                }
                await wowAuth.Authenticate(AppConfig.Parameters.Credentials);
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
                Console.WriteLine($"Error: {ex}");
            }
            // TODO: Menu
            // TODO: Game data requests
            // TODO: Game data statistics
        }
    }
}