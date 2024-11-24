using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;

namespace ImagesScale
{ // авторство  chatGPT. Убрал только в параметре команды анонимный тип в с которым вообще "не едет"
    public static class MouseClickCommandHelper
    {
        // Прикрепляемое свойство для команды клика
        public static readonly DependencyProperty MouseClickCommandProperty =
            DependencyProperty.RegisterAttached(
                "MouseClickCommand",
                typeof(ICommand),
                typeof(MouseClickCommandHelper),
                new PropertyMetadata(null, OnMouseClickCommandChanged));

        // Свойство для получения команды
        public static ICommand GetMouseClickCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(MouseClickCommandProperty);
        }

        // Свойство для установки команды
        public static void SetMouseClickCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(MouseClickCommandProperty, value);
        }

        // Обработчик изменения прикрепляемого свойства
        private static void OnMouseClickCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is UIElement uiElement)
            {
                if (e.OldValue == null && e.NewValue != null)
                {
                    // Подписываемся на событие клика, если команда назначена
                    uiElement.MouseLeftButtonDown += UiElement_MouseLeftButtonDown;
                }
                else if (e.OldValue != null && e.NewValue == null)
                {
                    // Отписываемся от события клика, если команда удалена
                    uiElement.MouseLeftButtonDown -= UiElement_MouseLeftButtonDown;
                }
            }
        }

        // Обработчик клика мыши
        private static void UiElement_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var uiElement = sender as UIElement;
            var command = GetMouseClickCommand(uiElement);

            if (command != null && command.CanExecute(null))
            {
                // Получаем координаты клика относительно окна
                var position = e.GetPosition(uiElement);
                //var commandParameter = new { X= position.X, Y = position.Y }; // Можно использовать анонимный тип или Point
                System.Drawing.Point commandParameter = new((int)position.X, (int)position.Y);

                // Выполняем команду с передачей координат
                command.Execute(commandParameter);
            }
        }
    }
}
