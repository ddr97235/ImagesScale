using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace ImagesScale.Views
{
    public class BoolToEvenNumberConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (parameter is string param)
            {
                return param switch
                {
                    "Even" => value is bool _value && _value,
                    "NotEven" => value is bool _value && !_value,
                    "All" => value == null,
                    _ => throw new NotImplementedException(),
                };
            }
            else
                throw new NotImplementedException();
        }
        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (parameter is string param)
            {
                return param switch
                {
                    "Even" => (bool)value!,
                    "NotEven" => !(bool)value!,
                    "All" => (bool)value! ? null : false,
                    _ => throw new NotImplementedException(),
                };
            }
            else
                throw new NotImplementedException();
            // return index ? parameter : Binding.DoNothing;
        }

        public static readonly BoolToEvenNumberConverter Instance = new();
        private BoolToEvenNumberConverter() { }
    }

    [MarkupExtensionReturnType(typeof(BoolToEvenNumberConverter))]
    public class BoolToEvenNumberExtension : MarkupExtension
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return BoolToEvenNumberConverter.Instance;
        }
    }
}
