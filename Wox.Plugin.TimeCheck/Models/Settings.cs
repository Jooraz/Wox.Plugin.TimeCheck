using System;
using Newtonsoft.Json;

namespace TimeCheck.Models
{
    public class Settings
    {
        [JsonProperty]
        public TimeSpan Delay { get; set; }

        [JsonProperty]
        public string GoogleKey { get; set; }

        public Settings()
        {
            Delay = TimeSpan.FromSeconds(1);
        }
    }
}
