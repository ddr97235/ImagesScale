using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using BitmapFrame = System.Windows.Media.Imaging.BitmapFrame;

namespace ImagesScale.Views
{
    public class BytesToWriteableBitmapConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is ValueTuple<byte[], System.Drawing.Size, int> framedata)
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
