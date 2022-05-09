using WowInventoryStats.Authentication;
using WowInventoryStats.Configuration;

namespace WowInventoryStats
{
    public class Program
    {
        public static readonly string ConfigFolderPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "WowInventoryStats");

        public static readonly string ConfigFilePath = Path.Combine(ConfigFolderPath, "config.json");

        static async Task Main()
        {
            AppConfiguration AppConfig;
            TokenAuthenticator wowAuth = new();
            try
            {
                // Create application config and get access token from Blizzard
                AppConfig = new AppConfiguration(ConfigFilePath);
                if (!AppConfig.Parameters.Credentials.Populated())
                {
                    string? answer = "Y";
                    Console.WriteLine("Please provide your client ID and client secret credentials from your battle.net account.");
                    Console.Write("Enter credentials? (Y/n): ");
                    answer = Console.ReadLine();
                    if (string.IsNullOrEmpty(answer) || answer.ToLower() == "y")
                    {
                        Console.Write("Client ID: ");
                        var clientId = Console.ReadLine();
                        Console.Write("Client secret: ");
                        var clientSecret = Console.ReadLine();
                        AppConfig.Parameters.Credentials = new TokenCredentials{ ClientId = clientId, ClientSecret = clientSecret};
                        AppConfig.SaveConfig();
                    }
                    else
                    {
                        return;
                    }
                }
                Console.WriteLine("Authenticating...");
                await wowAuth.Authenticate(AppConfig.Parameters.Credentials);
                Console.WriteLine($"Authentication successful.");
                Console.WriteLine($"Token: {wowAuth.Token}");
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