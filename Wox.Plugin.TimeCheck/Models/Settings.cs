using System;
using Newtonsoft.Json;

namespace Wox.Plugin.TimeCheck.Models
{
    public class Settings
    {
        [JsonProperty]
        public long Delay { get; set; }

        [JsonProperty]
        public string GoogleKey { get; set; }

        public Settings()
        {
            Delay = (long)TimeSpan.FromSeconds(1).TotalMilliseconds;
        }
    }
}
