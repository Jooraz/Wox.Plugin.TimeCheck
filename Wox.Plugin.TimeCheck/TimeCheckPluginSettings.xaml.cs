using System;
using System.Reflection;
using System.Windows;
using TimeCheck.Models;
using Wox.Plugin;

namespace TimeCheck
{
    /// <summary>
    /// Interaction logic for TimeCheckPluginSettings.xaml
    /// </summary>
    public partial class TimeCheckPluginSettings
    {
        private IPublicAPI woxAPI;
        private Settings _settings;

        public string GoogleApiKey { get; set; } = "";

        public TimeCheckPluginSettings(IPublicAPI woxAPI, Settings settings)
        {
            this.woxAPI = woxAPI;
            InitializeComponent();
            _settings = settings;
            this.DataContext = _settings;
        }
    }
}
