using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;
using System.Windows.Input;

namespace ImagesScale.Views
{
    public class ScaleValueToIndex : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is int index && parameter is int scale)
            {
                return scale == index;
            }
            else
            {
                throw new NotSupportedException();
            }
        }
        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool index && parameter is int scale)
            {
                if (!index) return Binding.DoNothing;
                return scale;
            }
            else
            {
                throw new NotSupportedException();
            }
        }
    }
}
