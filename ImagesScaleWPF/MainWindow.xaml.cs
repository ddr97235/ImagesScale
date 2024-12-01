using ImagesScale.ViewModels;
using ImagesScale.Views;
using System.Windows;
using System.Windows.Media;

namespace ImagesScale
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            Loaded += OnLoaded;
            InitializeComponent();
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            MainWindowViewModel vm = (MainWindowViewModel)DataContext;
            await vm.Start();
            if (Application.Current.Resources["sd"] is ScaleData scaleData)
            {
                vm.camera.FrameChanged += scaleData!.OnNewFrame;
            }
        }
    }

    internal class MainWindowViewModelDesign : MainWindowViewModel
    {
        public MainWindowViewModelDesign()
        {
            _ = Start();
        }
    }
}