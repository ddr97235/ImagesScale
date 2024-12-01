using ImagesScale.ViewModels;
using System.Windows;

namespace ImagesScale
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            new Window1().Show();

        }
    }

    public class Locator
    {
        public MainWindowViewModel? MainVM { get; set; }
    }
}
