using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace ImagesScale.Views
{
    public partial class ScaleData : Freezable
    {
        public static VisualBrush GetVisualBrush(DependencyObject obj)
        {
            return (VisualBrush)obj.GetValue(VisualBrushProperty);
        }

        private static void SetVisualBrush(DependencyObject obj, VisualBrush value)
        {
            obj.SetValue(VisualBrushPropertyKey, value);
        }

        private static readonly VisualBrush emptyBrush = new VisualBrush(null);
        static ScaleData()
        {
            emptyBrush.Freeze();
        }
        // Using a DependencyProperty as the backing store for VisualBrush.  This enables animation, styling, binding, etc...
        private static readonly DependencyPropertyKey VisualBrushPropertyKey =
            DependencyProperty.RegisterAttachedReadOnly(nameof(VisualBrushProperty)[0..^8], typeof(VisualBrush), typeof(ScaleData), new PropertyMetadata(null));
        public static readonly DependencyProperty VisualBrushProperty = VisualBrushPropertyKey.DependencyProperty;

    }
}
