using Wox.Plugin;
using System.Xml.Serialization;
using Wox.Infrastructure.Storage;

namespace Wox.Plugin.TimeCheck.Models
{
    [XmlType("row")]
    public class Time
    {
        [XmlElement("code")]
        public string Code { get; set; }
        [XmlElement("name")]
        public string Name { get; set; }
        [XmlElement("country")]
        public string Country { get; set; }
        [XmlElement("timeChange")]
        public int TimeChange { get; set; }
    }
}
