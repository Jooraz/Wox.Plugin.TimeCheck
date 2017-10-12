using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wox.Plugin;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.IO;
using RestSharp;
using System.Net;
using Wox.Infrastructure.Storage;
using System.Globalization;
using System.Reflection;
using Wox.Plugin.TimeCheck.Helpers;
using Wox.Plugin.TimeCheck.Models;

namespace Wox.Plugin.TimeCheck
{

    public partial class TimeChecker : IPlugin, ISettingProvider, IPluginI18n, ISavable
    {
        private Settings _settings;
        private PluginJsonStorage<Settings> _storage;
        private Debouncer _debouncer;
        public List<Time> Times { get; set; }

        private PluginInitContext _context;

        public void Init(PluginInitContext context)
        {
            _context = context;
            _storage = new PluginJsonStorage<Settings>();
            _settings = _storage.Load();
            _debouncer = new Debouncer(_settings.Delay);

            LoadData();
        }

        private void LoadData()
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Wox.Plugin.TimeCheck.Resources.timezones.xml"))
            {
                XmlSerializer deserializer = new XmlSerializer(typeof(List<Time>), new XmlRootAttribute("root"));
                Times = (List<Time>)deserializer.Deserialize(stream);
            }
        }

        public List<Result> Query(Query query)
        {
            var key = _settings.GoogleKey;

            List<Result> results = new List<Result>();

            if (string.IsNullOrEmpty(query.Search))
            {
                return results;
            }

            string searchLowered = query.Search.ToLower();
            var found = Times.Where(x => x.Country.ToLower().Contains(searchLowered) || x.Name.ToLower().Contains(searchLowered));
            found = found.Take(5);
            foreach (var el in found)
            {
                var time = DateTime.Now;
                time = time.AddSeconds(el.TimeChange);

                var place = el.Name;

                var value = $"Local mean time for {place} is {time}";

                results.Add(new Result()
                {
                    Title = value,
                    SubTitle = "Copy to clipboard",
                    Action = _ =>
                    {
                        Clipboard.SetText(value);
                        return true;
                    }
                });
            }

            if (!string.IsNullOrEmpty(key))
            {
                var getData = new Action(() =>
                {
                    var client = new RestClient("https://maps.googleapis.com");
                    var name = query.Search;

                    var geoRequest = new RestRequest("maps/api/geocode/json");
                    geoRequest.AddQueryParameter("address", name);
                    geoRequest.AddQueryParameter("key", key);

                    IRestResponse<GeoOutput> response = client.Execute<GeoOutput>(geoRequest);

                    //if not ok, nothing else to do
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        return;
                    }

                    foreach (var el in response.Data.Results)
                    {
                        var address = el.FormattedAddress;
                        var location = el.Geometry.Location;

                        var request = new RestRequest("maps/api/timezone/json?location={lat},{lng}&key={key}", Method.GET);
                        request.AddUrlSegment("lat", location.Lat.ToString(CultureInfo.InvariantCulture));
                        request.AddUrlSegment("lng", location.Lng.ToString(CultureInfo.InvariantCulture));
                        request.AddUrlSegment("key", key);
                        request.AddQueryParameter("timestamp", ((long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds).ToString());

                        IRestResponse<TimeResult> oneTimeResult = client.Execute<TimeResult>(request);
                        var timeResult = oneTimeResult.Data;
                        if (oneTimeResult.StatusCode == HttpStatusCode.OK && timeResult.Status == "OK")
                        {
                            var time = DateTime.UtcNow.AddSeconds(timeResult.RawOffset + timeResult.DstOffset);

                            var value = $"Local time for {address} is {time}";
                            var result = new Result()
                            {
                                Title = value,
                                SubTitle = "Copy to clipboard",
                                Action = e =>
                                {
                                    Clipboard.SetText(value);
                                    return true;
                                }
                            };

                            results.Add(result);
                        }
                    }
                });

                _debouncer.AddAction(getData);

            }

            return results;
        }


        public System.Windows.Controls.Control CreateSettingPanel()
        {
            return new TimeCheckPluginSettings(_context.API, _settings);
        }

        public string GetTranslatedPluginTitle()
        {
            return _context.API.GetTranslation("wox_plugin_timecheck_plugin_name");
        }

        public string GetTranslatedPluginDescription()
        {
            return _context.API.GetTranslation("wox_plugin_timecheck_plugin_description");
        }

        public void Save()
        {
            _storage.Save();
        }
    }
}
