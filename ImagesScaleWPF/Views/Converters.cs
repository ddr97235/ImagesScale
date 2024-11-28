using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Windows.Graphics.Imaging;
using static System.Runtime.InteropServices.JavaScript.JSType;
using BitmapFrame = System.Windows.Media.Imaging.BitmapFrame;

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
                    "All" => (bool)value!? null:false,
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

    public class BytesToWriteableBitmapConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if(value is ValueTuple<byte[], System.Drawing.Size, int> framedata)
            {                
                var size = framedata.Item2;
                if (framedata.Item1.Length == size.Width * size.Height)
                { // byte[] в формате bitmap GRAY8
                    WriteableBitmap writeableBitmap = new WriteableBitmap(size.Width, size.Height, 96, 96, PixelFormats.Gray8, null);
                    Int32Rect rect = new Int32Rect(0, 0, size.Width, size.Height);
                    writeableBitmap.WritePixels(rect, framedata.Item1, writeableBitmap.BackBufferStride, 0);
                    return writeableBitmap;
                }
                else
                { // вариант для byte[] содержит изображение в формате файла JPG
                    BitmapSource bitmapSource;
                    using (MemoryStream stream = new MemoryStream(framedata.Item1))
                    {
                        bitmapSource = BitmapFrame.Create(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
                    }
                    return bitmapSource;
                }                
            }
            return null;
        }
        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public static readonly BytesToWriteableBitmapConverter Instance = new();
        private BytesToWriteableBitmapConverter() { }
    }

    [MarkupExtensionReturnType(typeof(BytesToWriteableBitmapConverter))]
    public class BytesToWriteableBitmapExtension : MarkupExtension
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return BytesToWriteableBitmapConverter.Instance;
        }
    }
}
