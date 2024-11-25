using Avalonia.Controls;
using Avalonia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageScaleAvalonia.Views
{
    public partial class ScaleData : Control/*: Freezable*/
    {
        public static readonly StyledProperty<string> MyProperty =
        AvaloniaProperty.Register<ScaleData, string>(
            nameof(MyProperty),
            defaultValue: "По умолчанию",
            coerce: CoerceMyProperty);

        public string My
        {
            get => GetValue(MyProperty);
            set => SetValue(MyProperty, value);
        }

        private static string CoerceMyProperty(AvaloniaObject sender, string value)
        {
            // Принуждение значения: 
            return value;
        }

        private static void OnMyChanged(Control element, AvaloniaPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
            }
        }

        private static void OnVisualChanged(Control element, AvaloniaPropertyChangedEventArgs e)
        {
            if (e.NewValue is UserControl frame && element is ScaleData datahelper)
            {
                var image = frame.GetControl<Image>("MainCameraImage");
                datahelper.Visual = image;
            }
        }

        public static readonly StyledProperty<Visual> VisualProperty =
            AvaloniaProperty.Register<ScaleData, Visual>(nameof(VisualProperty));

        public Visual Visual
        {
            get => GetValue(VisualProperty);
            set => SetValue(VisualProperty, value);
        }



        public ScaleData()
        {
            MyProperty.Changed.AddClassHandler<Control>(OnMyChanged);
            VisualProperty.Changed.AddClassHandler<Control>(OnVisualChanged);

        }
    }
}
