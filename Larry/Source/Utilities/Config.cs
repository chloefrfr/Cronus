﻿using Newtonsoft.Json;

namespace Larry.Source.Utilities
{
    public class Config
    {
        public string ConnectionUrl { get; set; } 
        public string Token {  get; set; }
        public string GuildId { get; set; }

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
    }
}
