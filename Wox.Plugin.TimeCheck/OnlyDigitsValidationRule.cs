using System.Globalization;
using System.Resources;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Wox.Plugin.TimeCheck
{
    [ContentProperty("ErrorMessage")]
    public class OnlyDigitsValidationRule : ValidationRule
    {
        public ErrorMessageValue ErrorMessage { get; set; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var validationResult = new ValidationResult(true, null);

            if (value != null)
            {
                if (!string.IsNullOrEmpty(value.ToString()))
                {
                    var regex = new Regex("[^0-9]+"); //regex that matches disallowed text
                    var parsingOk = !regex.IsMatch(value.ToString());
                    if (!parsingOk)
                    {
                        validationResult = new ValidationResult(false, ErrorMessage);
                    }
                }
            }

            return validationResult;
        }
    }

    public class ErrorMessageValue : DependencyObject
    {
        public string Value
        {
            get { return (string)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            nameof(Value),
            typeof(string),
            typeof(ErrorMessageValue),
            new PropertyMetadata(default(string)));
    }
}