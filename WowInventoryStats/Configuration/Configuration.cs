using System.Text.Json;
using System.Text.Json.Serialization;
using WowInventoryStats.Authentication;

namespace WowInventoryStats.Configuration
{
    public class AppParameters
    {
        [property: JsonPropertyName("credentials")]
        public TokenCredentials Credentials { get; set; } = new();

        [property: JsonPropertyName("logging")]
        public bool Logging { get; set; } = false;
    }

    public class AppConfiguration
    {
        public AppParameters Parameters { get; private set; }

        private static readonly string AppDataConfigFolder = "WowInventoryStats";

        private static readonly string AppDataConfigFileName = "WowInventoryStatsConfig.json";

        public AppConfiguration()
        {
            try
            {
                // Check if folder exists. Create one and copy over template config if not.
                string appDataConfigFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AppDataConfigFolder);
                string appDataConfigFilePath = Path.Combine(appDataConfigFolderPath, AppDataConfigFileName);
                if (!Directory.Exists(appDataConfigFolderPath) || !File.Exists(appDataConfigFilePath))
                {
                    Directory.CreateDirectory(appDataConfigFolderPath);
                    CreateDefaultConfig(appDataConfigFilePath);
                    throw new Exception("please provide access token credentials");
                }
                else
                {
                    Parameters = JsonSerializer.Deserialize<AppParameters>(File.ReadAllText(appDataConfigFilePath))!;
                }
            }
            catch (Exception ex)
            {
                throw new AppConfigurationException(ex.Message);
            }
        }

        private void CreateDefaultConfig(string path)
        {
            var json = JsonSerializer.Serialize(new AppParameters(), new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(path, json);
        }
    }

    public class AppConfigurationException : Exception
    {
        public AppConfigurationException(string message) : base(message)
        {
        }
    }
}
