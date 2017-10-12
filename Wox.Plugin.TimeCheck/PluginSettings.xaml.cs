using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using Wox.Plugin.TimeCheck.Models;
using Wox.Plugin;
using System.Windows.Controls;

namespace Wox.Plugin.TimeCheck
{
    /// <summary>
    /// Interaction logic for TimeCheckPluginSettings.xaml
    /// </summary>
    public partial class PluginSettings : UserControl
    {
        private IPublicAPI woxAPI;
        private Settings _settings;
        
        public PluginSettings(IPublicAPI woxAPI, Settings settings)
        {
            this.woxAPI = woxAPI;
            InitializeComponent();
            _settings = settings;
            this.DataContext = _settings;
        }

        private void PreviewDelayInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = false;
        }

        private static bool IsTextAllowed(string text)
        {
            Regex regex = new Regex("[^0-9]+"); //regex that matches disallowed text
            var match = !regex.IsMatch(text);

            if (!match)
            {
                return false;
            }

            long result = 0;
            return long.TryParse(text, out result);
        }
    }
}
