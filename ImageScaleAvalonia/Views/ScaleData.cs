using Avalonia.Controls;
using Avalonia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Data;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System.IO;
using Windows.Graphics.Imaging;

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

        private static void OnVisualChanged(ScaleData element, AvaloniaPropertyChangedEventArgs e){}

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
            }
        }

        private void OnDataContextChanged(object? sender, EventArgs e) => 
            Bind(FrameProperty, new Binding { Source = (sender as Control).DataContext, Path = "FrameData" });
 
        private static readonly StyledProperty<object> FrameProperty =
            AvaloniaProperty.Register<ScaleData, object>(nameof(FrameProperty));

        private object Frame
        {
            get => GetValue(FrameProperty);
            set => SetValue(FrameProperty, value);
        }

        private static void OnFrameChanged(ScaleData element, AvaloniaPropertyChangedEventArgs e)
        {
            if (e.NewValue is ValueTuple<byte[], System.Drawing.Size, int> framedata)
            {
                var size = framedata.Item2;
                if (framedata.Item1.Length == size.Width * size.Height)
                { // byte[] в формате bitmap GRAY8
                  // не поддерживает в данной реализации
                }
                else
                { // вариант для byte[] содержит изображение в формате файла JPG
                    element.fPSController.AddFrame(framedata.Item3, element.IsEvenNumberId, (fpsCam, fpsUI) =>
                    { 
                        element.FPS_UI = fpsUI; element.FPSCamera = fpsCam; 
                    });
                    bool IsCurrentFrameEven = framedata.Item3 % 2 == 0;
                    if (element.IsEvenNumberId == null || IsCurrentFrameEven == element.IsEvenNumberId)
                    {
                        using (var stream = new MemoryStream(framedata.Item1))
                        {
                            // Загружаем изображение в Bitmap из MemoryStream
                            var bitmap = new Bitmap(stream);
                            element.BitmapData = bitmap;
                        }
                        element.FrameID = framedata.Item3;
                    }                            
                }
            }
        }

        public static readonly StyledProperty<Bitmap> BitmapDataProperty =
        AvaloniaProperty.Register<ScaleData, Bitmap>("BitmapData");

        public Bitmap BitmapData // источник изображения
        {
            get => GetValue(BitmapDataProperty);
            set => SetValue(BitmapDataProperty, value);
        }

        private FPSController fPSController = new();

        public static readonly StyledProperty<int> FPSCameraProperty =
            AvaloniaProperty.Register<ScaleData, int>("FPSCamera", defaultValue: 0);
        public int FPSCamera
        {
            get => GetValue(FPSCameraProperty);
            set => SetValue(FPSCameraProperty, value);
        }
        public static readonly StyledProperty<int> FPS_UIProperty =
           AvaloniaProperty.Register<ScaleData, int>("FPS_UI", defaultValue: 0);
        public int FPS_UI
        {
            get => GetValue(FPS_UIProperty);
            set => SetValue(FPS_UIProperty, value);
        }
        public static readonly StyledProperty<bool?> IsEvenNumberIdProperty =
            AvaloniaProperty.Register<ScaleData, bool?>("IsEvenNumberId", defaultValue: null);
        /// <summary> true - четный, false - нечетные, null - все и четные и нечетные.</summary>
        public bool? IsEvenNumberId
        {
            get => GetValue(IsEvenNumberIdProperty);
            set => SetValue(IsEvenNumberIdProperty, value);
        }

        public static readonly StyledProperty<int> FrameIDProperty =
            AvaloniaProperty.Register<ScaleData, int>("FrameID", defaultValue: 8);
        public int FrameID
        {
            get => GetValue(FrameIDProperty);
            set => SetValue(FrameIDProperty, value);
        }
        public ScaleData()
        {
            MyProperty.Changed.AddClassHandler<ScaleData>(OnMyChanged);
            VisualProperty.Changed.AddClassHandler<ScaleData>(OnVisualChanged);
            FrameProperty.Changed.AddClassHandler<ScaleData>(OnFrameChanged);
        }
        static ScaleData()
        {
            ScaleDataValueProperty.Changed.AddClassHandler<Control>(OnScaleDataPropertyChanged);
        }
    }
}
