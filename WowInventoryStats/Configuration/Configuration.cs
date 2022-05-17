using System.Text.Json;
using System.Text.Json.Serialization;
using WowInventoryStats.Authentication;

namespace WowInventoryStats.Configuration
{
    public class AppParameters
    {
        [property: JsonPropertyName("credentials")]
        public TokenCredentials Credentials { get; set; } = new();

        // Checks that all fields are not null
        public bool Populated()
        {
            return Credentials.Populated();
        }
    }

    public class AppConfiguration
    {
        public AppParameters Parameters { get; set; }

        public AppConfiguration(string path)
        {
            ConfigPath = path;
            try
            {
                // Check if file exists. If not, create one and copy over template config.
                if (File.Exists(ConfigPath))
                {
                    Parameters = JsonSerializer.Deserialize<AppParameters>(File.ReadAllText(ConfigPath))!;
                }
                else
                {
                    Parameters = new();
                    Directory.CreateDirectory(Path.GetDirectoryName(ConfigPath)!);
                    SaveConfig();
                }
            }
            catch (Exception ex)
            {
                throw new AppConfigurationException(ex.Message, ex);
            }
        }

        public void SaveConfig()
        {
            var json = JsonSerializer.Serialize(Parameters, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(ConfigPath, json);
        }

        public bool Populated()
        {
            return Parameters.Populated();
        }

        internal readonly string ConfigPath;
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
