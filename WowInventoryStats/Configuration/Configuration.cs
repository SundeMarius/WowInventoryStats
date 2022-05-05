using System.Text.Json;
using System.Text.Json.Serialization;
using WowInventoryStats.Authentication;

namespace WowInventoryStats.Configuration
{
    public class AppParameters
    {
        [property: JsonPropertyName("logging")]
        public bool Logging { get; set; } = false;
        
        [property: JsonPropertyName("credentials")]
        public TokenCredentials Credentials { get; set; } = new();

        public AppParameters()
        {
        }
        public AppParameters(TokenCredentials credentials)
        {
            Credentials = credentials;
        }
    }

    public class AppConfiguration
    {
        public AppParameters Parameters { get; }

        private static readonly string AppDataConfigFolder = "WowInventoryStats";

        private static readonly string AppDataConfigFileName = "config.json";

        public AppConfiguration()
        {
            try
            {
                // Check if folder exists. Create one and copy over template config if not.
                string appDataConfigFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AppDataConfigFolder);
                string appDataConfigFilePath = Path.Combine(appDataConfigFolderPath, AppDataConfigFileName);
                if (File.Exists(appDataConfigFilePath))
                {
                    Parameters = JsonSerializer.Deserialize<AppParameters>(File.ReadAllText(appDataConfigFilePath))!;
                }
                else
                {
                    Directory.CreateDirectory(appDataConfigFolderPath);
                    CreateDefaultConfig(appDataConfigFilePath);
                    Parameters = new();
                }
            }
            catch (Exception ex)
            {
                throw new AppConfigurationException(ex.Message, ex);
            }
        }

        static private void CreateDefaultConfig(string path)
        {
            var json = JsonSerializer.Serialize(new AppParameters(), new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(path, json);
        }
    }

    public class AppConfigurationException : Exception
    {
        public AppConfigurationException() : base()
        {
        }
        public AppConfigurationException(string message) : base(message)
        {
        }
        public AppConfigurationException(string message, Exception ex) : base(message, ex)
        {
        }
    }
}
