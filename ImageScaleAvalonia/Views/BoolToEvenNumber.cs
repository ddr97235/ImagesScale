using Avalonia.Data;
using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace ImageScaleAvalonia.Views
{
    public class BoolToEvenNumber : IValueConverter
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
            {
                throw new NotSupportedException();
            }
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {

            if (value is not bool isEvent || !isEvent)
            {
                return BindingOperations.DoNothing;
            }

            if (parameter is not string param)
            {
                throw new NotImplementedException();
            }

            return param switch
            {
                "Even" => isEvent,
                "NotEven" => !isEvent,
                "All" => isEvent ? null : false,
                _ => throw new NotImplementedException(),
            };

        }
    }
}
