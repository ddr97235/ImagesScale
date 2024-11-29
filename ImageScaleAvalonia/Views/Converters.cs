using Avalonia.Data.Converters;
using Avalonia.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if(!(bool)value)
            {
                return BindingOperations.DoNothing;
            }

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
        }
    }
}
