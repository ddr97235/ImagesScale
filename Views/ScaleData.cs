using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace ImagesScale.Views
{
    public partial class ScaleData : Freezable
    {
        protected override Freezable CreateInstanceCore() => new ScaleData();

        public FrameworkElement Visual
        {
            get { return (FrameworkElement)GetValue(VisualProperty); }
            set { SetValue(VisualProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Visual.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VisualProperty =
            DependencyProperty.Register(nameof(Visual),
                                        typeof(FrameworkElement),
                                        typeof(ScaleData),
                                        new PropertyMetadata(null)
                                        {
                                            PropertyChangedCallback = OnVisualChanged
                                        });

        private static void OnVisualChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ScaleData sd = (ScaleData)d;
            if (e.OldValue is FrameworkElement old)
            {
                old.MouseLeftButtonDown -= sd.OnMouseClick;
                widthDescriptor.RemoveValueChanged(old, sd.OnSizeChanged);
                heightDescriptor.RemoveValueChanged(old, sd.OnSizeChanged);
            }
            if (e.NewValue is FrameworkElement @new)
            {
                @new.MouseLeftButtonDown += sd.OnMouseClick;
                widthDescriptor.AddValueChanged(@new, sd.OnSizeChanged);
                heightDescriptor.AddValueChanged(@new, sd.OnSizeChanged);
                sd.OnRenderViewBox();
            }
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
                                                typeof(ScaleData),
                                                new PropertyMetadata(new Point())
                                                {
                                                    PropertyChangedCallback = (d, _) => ((ScaleData)d).OnRenderViewBox()
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
                                        typeof(ScaleData),
                                        new PropertyMetadata(double.NaN)
                                        {
                                            PropertyChangedCallback = (d, _) => ((ScaleData)d).OnRenderViewBox(),
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
            DependencyProperty.RegisterReadOnly(nameof(ViewboxAbsolute), typeof(Rect), typeof(ScaleData), new PropertyMetadata(new Rect()));
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
                                                typeof(ScaleData),
                                                new PropertyMetadata(new Rect())
                                                {
                                                    CoerceValueCallback = (d, _) =>
                                                    {
                                                        ScaleData sd = (ScaleData)d;

                                                        if (sd.Visual is null)
                                                            return new Rect();

                                                        double width = sd.Visual.ActualWidth;
                                                        double height = sd.Visual.ActualHeight;

                                                        Rect abs = sd.ViewboxAbsolute;

                                                        Rect rel = new Rect(abs.X / width, abs.Y / height, abs.Width / width, abs.Height / height);
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
            DependencyProperty.RegisterAttachedReadOnly(nameof(ViewportProperty)[0..^8], typeof(Rect), typeof(ScaleData), new PropertyMetadata(new Rect(0, 0, 1, 1)));
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
                                                typeof(ScaleData),
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
    }
}
