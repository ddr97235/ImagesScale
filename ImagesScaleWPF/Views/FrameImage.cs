using ImageScaleModels;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

namespace ImagesScale.Views
{
    public partial class FrameImage : Freezable
    {

        //private class BitmapSourceConverter : ImageSourceConverter
        //{
        //    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        //    {
        //        bool ddd =  base.CanConvertFrom(context, sourceType);
        //        return ddd;
        //    }

        //    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        //    {
        //        object ddd = base.ConvertFrom(context, culture, value);
        //        return ddd;
        //    }
        //}


        protected override Freezable CreateInstanceCore() => new FrameImage();

        protected override bool FreezeCore(bool isChecking) => false;


        //public FrameImage()
        //{
        //    CroppedBitmap cropped = new();

        //    _ = BindingOperations.SetBinding(cropped, CroppedBitmap.SourceProperty,
        //        new Binding()
        //        {
        //            Path = new PropertyPath(SourceProperty),
        //            Source = this
        //        });

        //    _ = BindingOperations.SetBinding(cropped, CroppedBitmap.SourceRectProperty,
        //        new Binding()
        //        {
        //            Path = new PropertyPath(FrameRectProperty),
        //            Source = this
        //        });

        //    Frame = cropped;
        //}

        //[TypeConverter(typeof(BitmapSourceConverter))]
        public BitmapSource? Source
        {
            get { return (BitmapSource?)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Source.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register(nameof(Source),
                                        typeof(BitmapSource),
                                        typeof(FrameImage),
                                        new PropertyMetadata(null)
                                        {
                                            PropertyChangedCallback = (d, _) => ((FrameImage)d).InvalidateProperty(FrameProperty)
                                        });

        public ImageSource? Frame
        {
            get { return (ImageSource?)GetValue(FrameProperty); }
            private set { SetValue(FramePropertyKey, value); }
        }

        // Using a DependencyProperty as the backing store for Frame.  This enables animation, styling, binding, etc...
        private static readonly DependencyPropertyKey FramePropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(Frame),
                                                typeof(ImageSource),
                                                typeof(FrameImage),
                                                new PropertyMetadata(null)
                                                {
                                                    CoerceValueCallback = (d, value) =>
                                                    {
                                                        FrameImage frame = (FrameImage)d;

                                                        BitmapSource? source = frame.Source;
                                                        if (source is null)
                                                        {
                                                            return null;
                                                        }

                                                        Rect rect = frame.ViewboxRelative;

                                                        if (rect.IsEmpty ||
                                                            IsNotNormal(rect.X) ||
                                                            IsNotNormal(rect.Y) ||
                                                            IsNotNormal(rect.Width) ||
                                                            IsNotNormal(rect.Height))
                                                        {
                                                            return null;
                                                        }

                                                        double width = source.Width;
                                                        double height = source.Height;

                                                        Int32Rect intRect = new(
                                                            (int)(rect.X * width),
                                                            (int)(rect.Y * height),
                                                            (int)(rect.Width * width),
                                                            (int)(rect.Height * height));
                                                        return new CroppedBitmap(source, intRect);

                                                        bool IsNotNormal(double num)
                                                        {
                                                            return !(num == 0.0 || double.IsNormal(num));
                                                        }
                                                    }
                                                });
        public static readonly DependencyProperty FrameProperty = FramePropertyKey.DependencyProperty;

        public Int32Rect FrameRect
        {
            get { return (Int32Rect)GetValue(FrameRectProperty); }
            private set { SetValue(FrameRectProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FrameRect.  This enables animation, styling, binding, etc...
        private static readonly DependencyPropertyKey FrameRectPropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(FrameRect),
                                                typeof(Int32Rect),
                                                typeof(FrameImage),
                                                new PropertyMetadata(Int32Rect.Empty)
                                                {
                                                    CoerceValueCallback = (d, value) =>
                                                    {
                                                        FrameImage frame = (FrameImage)d;
                                                        BitmapSource? source = frame.Source;
                                                        if (source is null)
                                                        {
                                                            return Int32Rect.Empty;
                                                        }
                                                        Rect rect = frame.ViewboxRelative;
                                                        if (rect.IsEmpty)
                                                        {
                                                            return Int32Rect.Empty;
                                                        }

                                                        double width = source.Width;
                                                        double height = source.Height;

                                                        Int32Rect intRect = new(
                                                            (int)(rect.X * width),
                                                            (int)(rect.Y * height),
                                                            (int)(rect.Width * width),
                                                            (int)(rect.Height * height));
                                                        return intRect;
                                                    }
                                                });
        public static readonly DependencyProperty FrameRectProperty = FrameRectPropertyKey.DependencyProperty;

        public FrameworkElement Visual
        {
            get { return (FrameworkElement)GetValue(VisualProperty); }
            set { SetValue(VisualProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Visual.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VisualProperty =
            DependencyProperty.Register(nameof(Visual),
                                        typeof(FrameworkElement),
                                        typeof(FrameImage),
                                        new PropertyMetadata(null)
                                        {
                                            PropertyChangedCallback = OnVisualChanged
                                        });

        private static void OnVisualChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FrameImage sd = (FrameImage)d;
            if (e.OldValue is FrameworkElement old)
            {
                old.MouseLeftButtonDown -= sd.OnMouseClick;
                widthDescriptor.RemoveValueChanged(old, sd.OnSizeChanged);
                heightDescriptor.RemoveValueChanged(old, sd.OnSizeChanged);
            }
            if (e.NewValue is FrameworkElement @new)
            {
                @new.DataContextChanged += sd.OnDataContextChanged;


                @new.MouseLeftButtonDown += sd.OnMouseClick;
                widthDescriptor.AddValueChanged(@new, sd.OnSizeChanged);
                heightDescriptor.AddValueChanged(@new, sd.OnSizeChanged);
                sd.OnRenderViewBox();
            }
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            // Создаем привязку            
        }

        private void OnSizeChanged(object? sender, EventArgs e)
        {
            OnRenderViewBox();
        }

        private static DependencyPropertyDescriptor widthDescriptor = DependencyPropertyDescriptor.FromProperty(FrameworkElement.ActualWidthProperty, typeof(FrameworkElement));
        private static DependencyPropertyDescriptor heightDescriptor = DependencyPropertyDescriptor.FromProperty(FrameworkElement.ActualHeightProperty, typeof(FrameworkElement));

        private void OnMouseClick(object sender, MouseButtonEventArgs e)
        {
            Position = e.GetPosition(Visual);
        }


        private void OnRenderViewBox()
        {
            FrameworkElement visual = Visual;
            if (visual is null)
            {
                return;
            }
            double width = Visual.ActualWidth;
            double height = Visual.ActualHeight;
            double scale = Scale;

            double swidth = width / scale;
            double sheight = height / scale;

            var www = LayoutInformation.GetLayoutSlot(Visual);
            var ggg = LayoutInformation.GetLayoutClip(Visual);

            Point position = Position;

            Rect rect = new Rect(position.X - swidth / 2, position.Y - sheight / 2, swidth, sheight);
            if (rect.Left < 0) rect.X = 0;
            if (rect.Top < 0) rect.Y = 0;
            if (rect.Right > width) rect.X = width - rect.Width;
            if (rect.Bottom > height) rect.Y = height - rect.Height;

            ViewboxAbsolute = rect;
            InvalidateProperty(ViewboxRelativeProperty);
        }


        public Point Position
        {
            get => (Point)GetValue(PositionProperty);
            private set => SetValue(PositionPropertyKey, value);
        }

        // Using a DependencyProperty as the backing store for Position.  This enables animation, styling, binding, etc...
        private static readonly DependencyPropertyKey PositionPropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(Position),
                                                typeof(Point),
                                                typeof(FrameImage),
                                                new PropertyMetadata(new Point())
                                                {
                                                    PropertyChangedCallback = (d, _) => ((FrameImage)d).OnRenderViewBox()
                                                });

        public static readonly DependencyProperty PositionProperty = PositionPropertyKey.DependencyProperty;



        public double Scale
        {
            get => (double)GetValue(ScaleProperty);
            set => SetValue(ScaleProperty, value);
        }

        // Using a DependencyProperty as the backing store for Scale.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ScaleProperty =
            DependencyProperty.Register(nameof(Scale),
                                        typeof(double),
                                        typeof(FrameImage),
                                        new PropertyMetadata(double.NaN)
                                        {
                                            PropertyChangedCallback = (d, _) => ((FrameImage)d).OnRenderViewBox(),
                                            CoerceValueCallback = (_, value) =>
                                            {
                                                double scale = (double)value;
                                                if (scale > 1)
                                                    return scale;
                                                return 1.0;
                                            }
                                        });
        public Rect ViewboxAbsolute
        {
            get => (Rect)GetValue(ViewboxAbsoluteProperty);
            private set => SetValue(ViewboxAbsolutePropertyKey, value);
        }

        // Using a DependencyProperty as the backing store for ViewBox.  This enables animation, styling, binding, etc...
        private static readonly DependencyPropertyKey ViewboxAbsolutePropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(ViewboxAbsolute), typeof(Rect), typeof(FrameImage), new PropertyMetadata(new Rect()));
        public static readonly DependencyProperty ViewboxAbsoluteProperty = ViewboxAbsolutePropertyKey.DependencyProperty;

        public Rect ViewboxRelative
        {
            get => (Rect)GetValue(ViewboxRelativeProperty);
            private set => SetValue(ViewboxRelativePropertyKey, value);
        }

        // Using a DependencyProperty as the backing store for ViewBox.  This enables animation, styling, binding, etc...
        private static readonly DependencyPropertyKey ViewboxRelativePropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(ViewboxRelative),
                                                typeof(Rect),
                                                typeof(FrameImage),
                                                new PropertyMetadata(new Rect())
                                                {
                                                    PropertyChangedCallback = (d, _) => ((FrameImage)d).InvalidateProperty(FrameProperty),
                                                    CoerceValueCallback = (d, _) =>
                                                    {
                                                        FrameImage sd = (FrameImage)d;

                                                        if (sd.Visual is null)
                                                            return new Rect();

                                                        double width = sd.Visual.ActualWidth;
                                                        double height = sd.Visual.ActualHeight;

                                                        Rect abs = sd.ViewboxAbsolute;

                                                        Rect rel = new(abs.X / width, abs.Y / height, abs.Width / width, abs.Height / height);
                                                        return rel;
                                                    }
                                                });
        public static readonly DependencyProperty ViewboxRelativeProperty = ViewboxRelativePropertyKey.DependencyProperty;



        public static Rect GetViewport(FrameworkElement obj)
        {
            return (Rect)obj.GetValue(ViewportProperty);
        }

        private static void SetViewport(FrameworkElement obj, Rect value)
        {
            obj.SetValue(ViewportPropertyKey, value);
        }

        // Using a DependencyProperty as the backing store for Viewport.  This enables animation, styling, binding, etc...
        private static readonly DependencyPropertyKey ViewportPropertyKey =
            DependencyProperty.RegisterAttachedReadOnly(nameof(ViewportProperty)[0..^8], typeof(Rect), typeof(FrameImage), new PropertyMetadata(new Rect(0, 0, 1, 1)));
        public static readonly DependencyProperty ViewportProperty = ViewportPropertyKey.DependencyProperty;



        public static Rect GetScaleBox(FrameworkElement fe)
        {
            return (Rect)fe.GetValue(ScaleBoxProperty);
        }

        public static void SetScaleBox(FrameworkElement fe, Rect box)
        {
            fe.SetValue(ScaleBoxProperty, box);
        }

        // Using a DependencyProperty as the backing store for IsProportionalScale.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ScaleBoxProperty =
            DependencyProperty.RegisterAttached(nameof(ScaleBoxProperty)[0..^8],
                                                typeof(Rect),
                                                typeof(FrameImage),
                                                new PropertyMetadata(new Rect())
                                                {
                                                    PropertyChangedCallback = OnScaleBoxChanged
                                                });
        private static readonly object empty = "пусто";
        private static void OnScaleBoxChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement fe = (FrameworkElement)d;
            if (!elements.TryGetValue(fe, out object? v))
            {
                elements.Add(fe, empty);
                fe.Loaded += OnElementLoaded;
                fe.Unloaded += OnElementUnloaded;
                if (fe.IsLoaded)
                {
                    OnElementLoaded(fe, new RoutedEventArgs());
                }
            }

            OnElementChanged(fe, EventArgs.Empty);
        }

        private static void OnElementLoaded(object sender, RoutedEventArgs e)
        {
            FrameworkElement fe = (FrameworkElement)sender!;
            widthDescriptor.AddValueChanged(fe, OnElementChanged);
            heightDescriptor.AddValueChanged(fe, OnElementChanged);
        }
        private static void OnElementUnloaded(object sender, RoutedEventArgs e)
        {
            FrameworkElement fe = (FrameworkElement)sender!;
            widthDescriptor.RemoveValueChanged(fe, OnElementChanged);
            heightDescriptor.RemoveValueChanged(fe, OnElementChanged);
        }


        private static readonly ConditionalWeakTable<FrameworkElement, object> elements = [];

        private static void OnElementChanged(object? sender, EventArgs e)
        {
            FrameworkElement fe = (FrameworkElement)sender!;
            double width = fe.ActualWidth;
            double height = fe.ActualHeight;
            Rect box = GetScaleBox(fe);

            double b_w_h = box.Width / box.Height;
            double w_h = width / height;

            Rect port = new();
            if (box.Width > box.Height * w_h)
            {
                port.Width = 1;
                port.Height = (box.Height * (width / box.Width)) / height;
            }
            else
            {
                port.Height = 1;
                port.Width = (box.Width * (height / box.Height)) / width;
            }

            port.X = 0.5 * (1 - port.Width);
            port.Y = 0.5 * (1 - port.Height);

            SetViewport(fe, port);

        }

        public static FrameImage GetTargetBox(DependencyObject obj)
        {
            return (FrameImage)obj.GetValue(TargetBoxProperty);
        }

        public static void SetTargetBox(DependencyObject obj, FrameImage value)
        {
            obj.SetValue(TargetBoxProperty, value);
        }

        // Using a DependencyProperty as the backing store for TargetBox.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TargetBoxProperty =
            DependencyProperty.RegisterAttached(nameof(TargetBoxProperty)[0..^8],
                                        typeof(FrameImage),
                                        typeof(FrameImage),
                                        new PropertyMetadata(null)
                                        {
                                            PropertyChangedCallback = (d, e) =>
                                            {
                                                FrameworkElement fe = (FrameworkElement)d;
                                                if (e.NewValue is FrameImage sd)
                                                {
                                                    SetScaleBox(fe, sd.ViewboxAbsolute);
                                                }
                                            }
                                        });



        public static FrameImage GetFrameImage(FrameworkElement obj)
        {
            return (FrameImage)obj.GetValue(FrameImageProperty);
        }

        public static void SetFrameImage(FrameworkElement obj, FrameImage value)
        {
            obj.SetValue(FrameImageProperty, value);
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FrameImageProperty =
            DependencyProperty.RegisterAttached(nameof(FrameImageProperty)[0..^8],
                                                typeof(FrameImage),
                                                typeof(FrameImage),
                                                new PropertyMetadata(null)
                                                {
                                                    PropertyChangedCallback = (d, e) =>
                                                    {
                                                        FrameworkElement fe = (FrameworkElement)d;
                                                        if (e.NewValue is FrameImage sd)
                                                        {
                                                            sd.Visual = fe;
                                                        }
                                                    }
                                                });
        public bool? IsEvenNumberId
        {
            get { return (bool?)GetValue(IsEvenNumberIdProperty); }
            set { SetValue(IsEvenNumberIdProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Visual.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsEvenNumberIdProperty =
            DependencyProperty.Register(nameof(IsEvenNumberId),
                                        typeof(bool?),
                                        typeof(FrameImage),
                                        new PropertyMetadata(null)
                                        {
                                            PropertyChangedCallback = (d, e) =>
                                            {

                                            }
                                        });


        public void OnNewFrame(object? sender, FrameEventArgs e)
        {
            if (Application.Current is not null && e.Frame != null)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (fPSController.AddFrame(e.FrameId, IsEvenNumberId, (fpsCam, fpsUI) => { FPS_UI = fpsUI; FPSCamera = fpsCam; }))
                    {
                        FrameData = e.Frame;
                        FrameID = e.FrameId;
                    }
                });
            }
        }

        public byte[] FrameData
        {
            get { return (byte[])GetValue(FrameDataProperty); }
            set { SetValue(FrameDataProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Visual.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FrameDataProperty =
            DependencyProperty.Register(nameof(FrameData),
                                        typeof(byte[]),
                                        typeof(FrameImage),
                                        new PropertyMetadata(null) { });
        public int FrameID
        {
            get { return (int)GetValue(FrameIDProperty); }
            set { SetValue(FrameIDProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Visual.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FrameIDProperty =
            DependencyProperty.Register(nameof(FrameID),
                                        typeof(int),
                                        typeof(FrameImage),
                                        new PropertyMetadata(null) { });
        private FPSController fPSController = new();

        private int FPS_UI
        {
            get { return (int)GetValue(FPS_UIProperty); }
            set { SetValue(FPS_UIProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Visual.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FPS_UIProperty =
            DependencyProperty.Register(nameof(FPS_UI),
                                        typeof(int),
                                        typeof(FrameImage),
                                        new PropertyMetadata(null) { });
        private int FPSCamera
        {
            get { return (int)GetValue(FPSCameraProperty); }
            set { SetValue(FPSCameraProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Visual.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FPSCameraProperty =
            DependencyProperty.Register(nameof(FPSCamera),
                                        typeof(int),
                                        typeof(FrameImage),
                                        new PropertyMetadata(null) { });
    }


}
