using Avalonia.Controls;
using Avalonia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageScaleAvalonia.Views
{
    public partial class ScaleData : AvaloniaObject
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

        private static void OnMyChanged(ScaleData element, AvaloniaPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
            }
        }

        private static void OnVisualChanged(ScaleData element, AvaloniaPropertyChangedEventArgs e)
        {

        }

        public static readonly StyledProperty<Visual> VisualProperty =
            AvaloniaProperty.Register<ScaleData, Visual>(nameof(VisualProperty));

        public Visual Visual
        {
            get => GetValue(VisualProperty);
            set => SetValue(VisualProperty, value);
        }


        public static readonly AttachedProperty<ScaleData> ScaleDataValueProperty =
            AvaloniaProperty.RegisterAttached<Grid, Control, ScaleData>("ScaleDataValue");

        public static void SetScaleDataValue(Control element, ScaleData value)
        {
            _ = element ?? throw new ArgumentNullException(nameof(element));
            element.SetValue(ScaleDataValueProperty, value);
        }
        public static ScaleData GetScaleDataValue(Control element)
        {
            _ = element ?? throw new ArgumentNullException(nameof(element));
            return element.GetValue(ScaleDataValueProperty);
        }

        private static void OnScaleDataPropertyChanged(AvaloniaObject d, AvaloniaPropertyChangedEventArgs e)
        {
            if (e.NewValue is ScaleData scaleData && d is Control control)
            {
                scaleData.Visual = control;
                control.DataContextChanged += scaleData.OnDataContextChanged;

                //control.AttachedToVisualTree += 
            }
        }

        private void OnDataContextChanged(object? sender, EventArgs e)
        {

        }

        /*static*/
        public ScaleData()
        {
            MyProperty.Changed.AddClassHandler<ScaleData>(OnMyChanged);
            VisualProperty.Changed.AddClassHandler<ScaleData>(OnVisualChanged);

        }

        static ScaleData()
        {
            ScaleDataValueProperty.Changed.AddClassHandler<Control>(OnScaleDataPropertyChanged);
        }
    }
}
