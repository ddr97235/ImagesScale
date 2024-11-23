using System;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace WpfCustomControlsCore
{
    public partial class DecoratorAdoner
    {
        public static UIElement GetChild(FrameworkElement fe)
        {
            return (UIElement)fe.GetValue(ChildProperty);
        }

        public static void SetChild(FrameworkElement fe, UIElement value)
        {
            fe.SetValue(ChildProperty, value);
        }

        // Using a DependencyProperty as the backing store for Child.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ChildProperty =
            DependencyProperty.RegisterAttached(
                "Child",
                typeof(UIElement),
                typeof(DecoratorAdoner),
                new PropertyMetadata(null)
                {
                    PropertyChangedCallback = OnChildChanged
                });

        private static void OnChildChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement fe = (FrameworkElement)d;
            if (!decorators.TryGetValue(fe, out DecoratorAdoner? decorator))
            {
                decorator = new(fe) { root = fe };
                decorators.Add(fe, decorator);
            }


            UIElement? element = (UIElement)e.NewValue;

            decorator.Child = element;

            fe.Loaded += OnLoaded;
            if (fe.IsLoaded)
                OnLoaded(fe, EventArgs.Empty);
        }

        private static void OnLoaded(object sender, EventArgs e)
        {
            FrameworkElement fe = (FrameworkElement)sender;

            if (decorators.TryGetValue(fe, out DecoratorAdoner? decorator))
            {
                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(fe);
                adornerLayer.Remove(decorator);
                adornerLayer.Add(decorator);
            }
        }
        private static readonly ConditionalWeakTable<Visual, DecoratorAdoner> decorators = new();
    }
}