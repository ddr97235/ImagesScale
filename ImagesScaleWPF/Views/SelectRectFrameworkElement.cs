using System.Windows;
using System.Windows.Input;

namespace ImagesScale.Views
{
    /// <summary>Класс с логикой выделения прямоугольника <see cref="FrameworkElement"/>
    /// по клику мыши (событию <see cref="UIElement.MouseLeftButtonDown"/>).</summary>
    public partial class SelectRectFrameworkElement : Freezable
    {
        /// <summary>Перепопределение метода создания экземпляра.</summary>
        /// <returns>Возвращает новый экземпляр <see cref="SelectRectFrameworkElement"/>.</returns>
        /// <remarks>Этот метод используется в логике <see cref="Freezable"/> при создании клона.
        /// Это бывает необходимо, например, для изменения замороженного <see cref="Freezable"/>
        /// при анимировании свойства.</remarks>
        protected override Freezable CreateInstanceCore() => new SelectRectFrameworkElement();

        /// <summary>Разрешение на заморозку.</summary>
        /// <param name="isChecking"><see langword="true"/>, если это только проверка без последующего замораживания.<br/>
        /// <see langword="false"/>, если это проверка перед заморозкой.</param>
        /// <returns><see langword="true"/> - объект можно заморозить, <see langword="false"/> - нельзя заморозить.<br/>
        /// В даной реализации возвращает всегда <see langword="false"/>.</returns>
        protected override bool FreezeCore(bool isChecking) => false;


        /// <summary>Элемент на котром нужно веделять прямоугольник.</summary>
        public FrameworkElement Visual
        {
            get { return (FrameworkElement)GetValue(VisualProperty); }
            set { SetValue(VisualProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Visual.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VisualProperty =
            DependencyProperty.Register(nameof(Visual),
                                        typeof(FrameworkElement),
                                        typeof(SelectRectFrameworkElement),
                                        new PropertyMetadata(null)
                                        {
                                            PropertyChangedCallback = OnVisualChanged
                                        });

        private static void OnVisualChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SelectRectFrameworkElement sd = (SelectRectFrameworkElement)d;
            if (e.OldValue is FrameworkElement old)
            {
                old.RemoveHandler(UIElement.MouseLeftButtonDownEvent, (MouseButtonEventHandler)sd.OnMouseClick);
                //old.MouseLeftButtonDown -= sd.OnMouseClick;

                old.SizeChanged -= sd.OnSizeChanged;
                //widthDescriptor.RemoveValueChanged(old, sd.OnSizeChanged);
                //heightDescriptor.RemoveValueChanged(old, sd.OnSizeChanged);
            }
            if (e.NewValue is FrameworkElement @new)
            {
                // Вызов обработчика даже для уже обработанного события. Нужно, например, для ButtonBase.
                @new.AddHandler(UIElement.MouseLeftButtonDownEvent, (MouseButtonEventHandler)sd.OnMouseClick, true);
                //@new.MouseLeftButtonDown += sd.OnMouseClick;

                @new.SizeChanged += sd.OnSizeChanged;
                //widthDescriptor.AddValueChanged(@new, sd.OnSizeChanged);
                //heightDescriptor.AddValueChanged(@new, sd.OnSizeChanged);

                sd.VisualSize = new Size(@new.ActualWidth, @new.ActualHeight);
                //sd.OnRenderViewBox();
            }
            else
            {
                sd.ClearValue(VisualProperty);
                sd.ClearValue(VisualSizeProperty);
            }
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            VisualSize = e.NewSize;
            //OnRenderViewBox();
        }

        //private void OnSizeChanged(object? sender, EventArgs e)
        //{
        //    OnRenderViewBox();
        //}

        //private static DependencyPropertyDescriptor widthDescriptor = DependencyPropertyDescriptor.FromProperty(FrameworkElement.ActualWidthProperty, typeof(FrameworkElement));
        //private static DependencyPropertyDescriptor heightDescriptor = DependencyPropertyDescriptor.FromProperty(FrameworkElement.ActualHeightProperty, typeof(FrameworkElement));

        private void OnMouseClick(object sender, MouseButtonEventArgs e)
        {
            Position = e.GetPosition(Visual);
        }



        /// <summary>Размер <see cref="Visual"/>.</summary>
        public Size VisualSize
        {
            get { return (Size)GetValue(VisualSizeProperty); }
            private set { SetValue(VisualSizePropertyKey, value); }
        }

        // Using a DependencyProperty as the backing store for VisualSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyPropertyKey VisualSizePropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(VisualSize),
                                                typeof(Size),
                                                typeof(SelectRectFrameworkElement),
                                                new PropertyMetadata(Size.Empty)
                                                {
                                                    PropertyChangedCallback = (d, _) => ((SelectRectFrameworkElement)d).OnRenderViewBox()
                                                });
        public static readonly DependencyProperty VisualSizeProperty = VisualSizePropertyKey.DependencyProperty;

        /// <summary>Обновление прямоугольников выделения.</summary>
        private void OnRenderViewBox()
        {
            FrameworkElement visual = Visual;
            if (visual is null)
            {
                return;
            }
            Size visualSize = VisualSize;
            double width = visualSize.Width;
            double height = visualSize.Height;
            //double width = Visual.ActualWidth;
            //double height = Visual.ActualHeight;
            double scale = Scale;

            double swidth = width / scale;
            double sheight = height / scale;

            Point position = Position;

            Rect rect = new Rect(position.X - swidth / 2, position.Y - sheight / 2, swidth, sheight);
            if (rect.Left < 0) rect.X = 0;
            if (rect.Top < 0) rect.Y = 0;
            if (rect.Right > width) rect.X = width - rect.Width;
            if (rect.Bottom > height) rect.Y = height - rect.Height;

            ViewboxAbsolute = rect;
            InvalidateProperty(ViewboxRelativeProperty);
        }

        /// <summary>Позиция последнего клика мыши.</summary>
        public Point Position
        {
            get => (Point)GetValue(PositionProperty);
            private set => SetValue(PositionPropertyKey, value);
        }

        // Using a DependencyProperty as the backing store for Position.  This enables animation, styling, binding, etc...
        private static readonly DependencyPropertyKey PositionPropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(Position),
                                                typeof(Point),
                                                typeof(SelectRectFrameworkElement),
                                                new PropertyMetadata(new Point())
                                                {
                                                    PropertyChangedCallback = (d, _) => ((SelectRectFrameworkElement)d).OnRenderViewBox()
                                                });

        public static readonly DependencyProperty PositionProperty = PositionPropertyKey.DependencyProperty;


        /// <summary>"Масштаб".<br/>
        /// Определяет отношение размера элемента <see cref="Visual"/>
        /// к размерам выделяемого прямоугольника.</summary>
        public double Scale
        {
            get => (double)GetValue(ScaleProperty);
            set => SetValue(ScaleProperty, value);
        }

        // Using a DependencyProperty as the backing store for Scale.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ScaleProperty =
            DependencyProperty.Register(nameof(Scale),
                                        typeof(double),
                                        typeof(SelectRectFrameworkElement),
                                        new PropertyMetadata(double.NaN)
                                        {
                                            PropertyChangedCallback = (d, _) => ((SelectRectFrameworkElement)d).OnRenderViewBox(),
                                            CoerceValueCallback = (_, value) =>
                                            {
                                                double scale = (double)value;
                                                if (scale > 1)
                                                    return scale;
                                                return 1.0;
                                            }
                                        });
        /// <summary>Размер выделяемого прямоугольника в абсолютных единицах.</summary>
        public Rect ViewboxAbsolute
        {
            get => (Rect)GetValue(ViewboxAbsoluteProperty);
            private set => SetValue(ViewboxAbsolutePropertyKey, value);
        }

        // Using a DependencyProperty as the backing store for ViewBox.  This enables animation, styling, binding, etc...
        private static readonly DependencyPropertyKey ViewboxAbsolutePropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(ViewboxAbsolute), typeof(Rect), typeof(SelectRectFrameworkElement), new PropertyMetadata(new Rect()));
        public static readonly DependencyProperty ViewboxAbsoluteProperty = ViewboxAbsolutePropertyKey.DependencyProperty;

        /// <summary>Размер выделяемого прямоугольника в относительных единицах.</summary>
        public Rect ViewboxRelative
        {
            get => (Rect)GetValue(ViewboxRelativeProperty);
            private set => SetValue(ViewboxRelativePropertyKey, value);
        }

        // Using a DependencyProperty as the backing store for ViewBox.  This enables animation, styling, binding, etc...
        private static readonly DependencyPropertyKey ViewboxRelativePropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(ViewboxRelative),
                                                typeof(Rect),
                                                typeof(SelectRectFrameworkElement),
                                                new PropertyMetadata(new Rect())
                                                {
                                                    CoerceValueCallback = (d, _) =>
                                                    {
                                                        SelectRectFrameworkElement sd = (SelectRectFrameworkElement)d;

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



        //public static Rect GetViewport(FrameworkElement obj)
        //{
        //    return (Rect)obj.GetValue(ViewportProperty);
        //}

        //private static void SetViewport(FrameworkElement obj, Rect value)
        //{
        //    obj.SetValue(ViewportPropertyKey, value);
        //}

        //// Using a DependencyProperty as the backing store for Viewport.  This enables animation, styling, binding, etc...
        //private static readonly DependencyPropertyKey ViewportPropertyKey =
        //    DependencyProperty.RegisterAttachedReadOnly(nameof(ViewportProperty)[0..^8], typeof(Rect), typeof(SelectRectFrameworkElement), new PropertyMetadata(new Rect(0, 0, 1, 1)));
        //public static readonly DependencyProperty ViewportProperty = ViewportPropertyKey.DependencyProperty;



        //public static Rect GetScaleBox(FrameworkElement fe)
        //{
        //    return (Rect)fe.GetValue(ScaleBoxProperty);
        //}

        //public static void SetScaleBox(FrameworkElement fe, Rect box)
        //{
        //    fe.SetValue(ScaleBoxProperty, box);
        //}

        //// Using a DependencyProperty as the backing store for IsProportionalScale.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty ScaleBoxProperty =
        //    DependencyProperty.RegisterAttached(nameof(ScaleBoxProperty)[0..^8],
        //                                        typeof(Rect),
        //                                        typeof(SelectRectFrameworkElement),
        //                                        new PropertyMetadata(new Rect())
        //                                        {
        //                                            PropertyChangedCallback = OnScaleBoxChanged
        //                                        });
        //private static readonly object empty = "пусто";
        //private static void OnScaleBoxChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    FrameworkElement fe = (FrameworkElement)d;
        //    if (!elements.TryGetValue(fe, out object? v))
        //    {
        //        elements.Add(fe, empty);
        //        fe.Loaded += OnElementLoaded;
        //        fe.Unloaded += OnElementUnloaded;
        //        if (fe.IsLoaded)
        //        {
        //            OnElementLoaded(fe, new RoutedEventArgs());
        //        }
        //    }

        //    OnElementChanged(fe, EventArgs.Empty);
        //}

        //private static void OnElementLoaded(object sender, RoutedEventArgs e)
        //{
        //    FrameworkElement fe = (FrameworkElement)sender!;
        //    widthDescriptor.AddValueChanged(fe, OnElementChanged);
        //    heightDescriptor.AddValueChanged(fe, OnElementChanged);
        //}
        //private static void OnElementUnloaded(object sender, RoutedEventArgs e)
        //{
        //    FrameworkElement fe = (FrameworkElement)sender!;
        //    widthDescriptor.RemoveValueChanged(fe, OnElementChanged);
        //    heightDescriptor.RemoveValueChanged(fe, OnElementChanged);
        //}


        //private static readonly ConditionalWeakTable<FrameworkElement, object> elements = [];

        //private static void OnElementChanged(object? sender, EventArgs e)
        //{
        //    FrameworkElement fe = (FrameworkElement)sender!;
        //    double width = fe.ActualWidth;
        //    double height = fe.ActualHeight;
        //    Rect box = GetScaleBox(fe);

        //    double b_w_h = box.Width / box.Height;
        //    double w_h = width / height;

        //    Rect port = new();
        //    if (box.Width > box.Height * w_h)
        //    {
        //        port.Width = 1;
        //        port.Height = (box.Height * (width / box.Width)) / height;
        //    }
        //    else
        //    {
        //        port.Height = 1;
        //        port.Width = (box.Width * (height / box.Height)) / width;
        //    }

        //    port.X = 0.5 * (1 - port.Width);
        //    port.Y = 0.5 * (1 - port.Height);

        //    SetViewport(fe, port);

        //}

        //public static SelectRectFrameworkElement GetTargetBox(DependencyObject obj)
        //{
        //    return (SelectRectFrameworkElement)obj.GetValue(TargetBoxProperty);
        //}

        //public static void SetTargetBox(DependencyObject obj, SelectRectFrameworkElement value)
        //{
        //    obj.SetValue(TargetBoxProperty, value);
        //}

        //// Using a DependencyProperty as the backing store for TargetBox.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty TargetBoxProperty =
        //    DependencyProperty.RegisterAttached(nameof(TargetBoxProperty)[0..^8],
        //                                typeof(SelectRectFrameworkElement),
        //                                typeof(SelectRectFrameworkElement),
        //                                new PropertyMetadata(null)
        //                                {
        //                                    PropertyChangedCallback = (d, e) =>
        //                                    {
        //                                        FrameworkElement fe = (FrameworkElement)d;
        //                                        if (e.NewValue is SelectRectFrameworkElement sd)
        //                                        {
        //                                            SetScaleBox(fe, sd.ViewboxAbsolute);
        //                                        }
        //                                    }
        //                                });



        public static SelectRectFrameworkElement GetSelectRectFrameworkElement(FrameworkElement obj)
        {
            return (SelectRectFrameworkElement)obj.GetValue(SelectRectFrameworkElementProperty);
        }

        public static void SetSelectRectFrameworkElement(FrameworkElement obj, SelectRectFrameworkElement value)
        {
            obj.SetValue(SelectRectFrameworkElementProperty, value);
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectRectFrameworkElementProperty =
            DependencyProperty.RegisterAttached(nameof(SelectRectFrameworkElementProperty)[0..^8],
                                                typeof(SelectRectFrameworkElement),
                                                typeof(SelectRectFrameworkElement),
                                                new PropertyMetadata(null)
                                                {
                                                    PropertyChangedCallback = (d, e) =>
                                                    {
                                                        FrameworkElement fe = (FrameworkElement)d;
                                                        if (e.NewValue is SelectRectFrameworkElement sd)
                                                        {
                                                            sd.Visual = fe;
                                                        }
                                                    }
                                                });

        //public bool? IsEvenNumberId
        //{
        //    get { return (bool?)GetValue(IsEvenNumberIdProperty); }
        //    set { SetValue(IsEvenNumberIdProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for Visual.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty IsEvenNumberIdProperty =
        //    DependencyProperty.Register(nameof(IsEvenNumberId),
        //                                typeof(bool?),
        //                                typeof(SelectRectFrameworkElement),
        //                                new PropertyMetadata(null)
        //                                {
        //                                    PropertyChangedCallback = (d, e) =>
        //                                    {

        //                                    }
        //                                });


        //public void OnNewFrame(object? sender, FrameEventArgs e)
        //{
        //    if (Application.Current is not null && e.Frame != null)
        //    {
        //        Application.Current.Dispatcher.Invoke(() =>
        //        {
        //            if (fPSController.AddFrame(e.FrameId, IsEvenNumberId, (fpsCam, fpsUI) => { FPS_UI = fpsUI; FPSCamera = fpsCam; }))
        //            {
        //                FrameData = e.Frame;
        //                FrameID = e.FrameId;
        //            }
        //        });
        //    }
        //}

        //public byte[] FrameData
        //{
        //    get { return (byte[])GetValue(FrameDataProperty); }
        //    set { SetValue(FrameDataProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for Visual.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty FrameDataProperty =
        //    DependencyProperty.Register(nameof(FrameData),
        //                                typeof(byte[]),
        //                                typeof(SelectRectFrameworkElement),
        //                                new PropertyMetadata(null) { });
        //public int FrameID
        //{
        //    get { return (int)GetValue(FrameIDProperty); }
        //    set { SetValue(FrameIDProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for Visual.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty FrameIDProperty =
        //    DependencyProperty.Register(nameof(FrameID),
        //                                typeof(int),
        //                                typeof(SelectRectFrameworkElement),
        //                                new PropertyMetadata(null) { });
        //private FPSController fPSController = new();

        //private int FPS_UI
        //{
        //    get { return (int)GetValue(FPS_UIProperty); }
        //    set { SetValue(FPS_UIProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for Visual.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty FPS_UIProperty =
        //    DependencyProperty.Register(nameof(FPS_UI),
        //                                typeof(int),
        //                                typeof(SelectRectFrameworkElement),
        //                                new PropertyMetadata(null) { });
        //private int FPSCamera
        //{
        //    get { return (int)GetValue(FPSCameraProperty); }
        //    set { SetValue(FPSCameraProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for Visual.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty FPSCameraProperty =
        //    DependencyProperty.Register(nameof(FPSCamera),
        //                                typeof(int),
        //                                typeof(SelectRectFrameworkElement),
        //                                new PropertyMetadata(null) { });
    }

}
