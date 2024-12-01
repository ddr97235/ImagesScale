using CommunityToolkit.Mvvm.ComponentModel;
using ImageScaleModels;
using System.Drawing;

namespace ImagesScale.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        public MainWindowViewModel()
        {
            camera = new(true);
            //camera.FrameChanged += OnNewFrame;
            camera.IsCameraAvailableChanged += OnIsCameraAvailable;
        }

        private void OnIsCameraAvailable(object? sender, EventArgs e)
        {
            ErrorText = camera.IsCameraAvailable ? string.Empty : "Камера недоступна: отсутсвует или занята.";
        }

        public async Task Start()
        {
            await camera.Start();
        }

        private void OnNewFrame(object? sender, FrameEventArgs e)
        {
            FrameData = e;
        }

        public readonly CameraWinRT camera;

        private static readonly FrameEventArgs emptyFrame = new([], Size.Empty, -1);
        [ObservableProperty]
        private FrameEventArgs _frameData = emptyFrame;

        [ObservableProperty]
        private string _errorText = string.Empty;
    }
}
