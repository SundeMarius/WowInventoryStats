using WowInventoryStats.Authentication;
using WowInventoryStats.Configuration;
using WowInventoryStats.Logger;

namespace WowInventoryStats
{
    public class Program
    {
        /*
            Cross platform user config path
        */
        public static string UserConfigFilePath
        {
            get
            {
                string ConfigFolderPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "WowInventoryStats");
                return Path.Combine(ConfigFolderPath, "config.json");
            }
        }

        private static readonly ConsoleLogger CLogger = new();
        
        private static AppConfiguration? AppConfig;

        static async Task Main()
        {
            TokenAuthenticator wowAuth = new();
            try
            {
                // Read application config
                CLogger.Log(LogType.Trace, "Loading user configuration...");
                AppConfig = new AppConfiguration(UserConfigFilePath);
                if (!AppConfig.Populated())
                {
                    CLogger.Log(LogType.Warn, $"No configuration found -- creating default config at {AppConfig.ConfigPath}");
                    AppConfig.Parameters.Credentials = CredentialsPrompt();
                    if (!AppConfig.Populated())
                    {
                        CLogger.Log(LogType.Failure, "Credentials not provided, exiting...");
                        Environment.Exit(-1);
                    }
                    AppConfig.SaveConfig();
                }
                CLogger.Log(LogType.Trace, "User configuration loaded!");
                CLogger.Log(LogType.Trace, "Authenticating...");
                //  Get access token from Blizzard
                await wowAuth.Authenticate(AppConfig.Parameters.Credentials);
                CLogger.Log(LogType.Success, "Access granted!");
                CLogger.Log(LogType.Warn, "Here is your access token");
                Console.WriteLine($"Token: {wowAuth.Token}");
            }
            catch (AppConfigurationException ex)
            {
                CLogger.Log(LogType.Error, $"Configuration error: {ex.Message}");
            }
            catch (AuthenticationException ex)
            {
                CLogger.Log(LogType.Error, $"Authentication error: {ex.Message}");
            }
            catch (Exception ex)
            {
                CLogger.Log(LogType.Error, $"Error: {ex.Message}");
            }
        }

        private static TokenCredentials CredentialsPrompt()
        {
            Console.WriteLine("Please provide your client ID and client secret credentials from your battle.net account.");
            Console.Write("Prompt credentials? (Y/n): ");
            string? answer = Console.ReadLine();
            TokenCredentials credentials = new();
            if (string.IsNullOrEmpty(answer) || answer.ToLower() == "y")
            {
                Console.Write("Client ID: ");
                var clientId = Console.ReadLine();
                Console.Write("Client secret: ");
                var clientSecret = Console.ReadLine();
                credentials = new TokenCredentials(clientId, clientSecret);
            }
            return credentials;
        }
    }
}