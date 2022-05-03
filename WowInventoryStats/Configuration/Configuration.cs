using System.Text.Json;
using System.Text.Json.Serialization;

namespace WowInventoryStats.Configuration
{
    public record UserSecret
    (
        [property: JsonPropertyName("client_id")] string ClientId = "",
        [property: JsonPropertyName("client_secret")] string ClientSecret = ""
    );

    public class AppConfiguration
    {
        public UserSecret Secret = new();

        private Dictionary<string, string> Parameters = new();

        public AppConfiguration(string path)
        {
            try
            {
                Parameters = JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText(path))!;
                string credentialsPath = Path.GetFullPath(this["credentials_path"]);
                Secret = JsonSerializer.Deserialize<UserSecret>(File.ReadAllText(credentialsPath))!;
            }
            catch (Exception ex)
            {
                throw new AppConfigurationException(ex.Message);
            }
        }

        public string this[string key]
        {
            get { return Parameters[key]; }
            set { Parameters[key] = value; }
        }
    }

    public class AppConfigurationException : Exception
    {
        public AppConfigurationException(string message) : base(message)
        {
        }
    }
}
