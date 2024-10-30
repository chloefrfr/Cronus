using Newtonsoft.Json;

namespace ShopGenerator.Storefront.Utilities
{
    public class Config
    {
        public string ConnectionUrl { get; set; }
        public string Token { get; set; }
        public string GuildId { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string GameDirectory { get; set; }
        public string CurrentVersion { get; set; }

        public static Config Load()
        {
            var configFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config.json");
            if (!File.Exists(configFile))
            {
                throw new FileNotFoundException($"Config file '{configFile}' not found.");
            }

            var json = File.ReadAllText(configFile);
            return JsonConvert.DeserializeObject<Config>(json);
        }

        public static Config GetConfig()
        {
            Config config;

            try
            {
                config = Config.Load();
                return config;
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to load config: {ex.Message}");
                throw new Exception();
            }
        }
    }
}
