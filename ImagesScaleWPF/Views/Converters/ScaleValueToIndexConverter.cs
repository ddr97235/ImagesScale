using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace ImagesScale.Views
{
    public class ScaleValueToIndexConverter : IValueConverter
    {
        private static readonly DoubleConverter numconverter = new();
        private static readonly BooleanConverter boolconverter = new();
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (parameter is not double scale)
            {
                try
                {
                    scale = (double)numconverter.ConvertFrom(null, culture, parameter!)!;
                }
                catch (Exception)
                {
                    scale = 1;
                }
            }
            if (value is not double index)
            {
                index = (double)numconverter.ConvertFrom(null, culture, value!)!;
            }

            return scale == index;
        }
        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is not bool index)
            {
                index = (bool)boolconverter.ConvertFrom(null, culture, value!)!;
            }

            return index ? parameter : Binding.DoNothing;
        }

        public static readonly ScaleValueToIndexConverter Instance = new();
        private ScaleValueToIndexConverter() { }
    }
    
    [MarkupExtensionReturnType(typeof(ScaleValueToIndexConverter))]
    public class ScaleValueToIndexExtension : MarkupExtension
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return ScaleValueToIndexConverter.Instance;
        }
    }
}
