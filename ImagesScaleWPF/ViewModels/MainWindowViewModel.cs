using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ImagesScale.Models;
using System.Windows;
using System.Windows.Media.Imaging;
using Windows.Graphics.Imaging;
using ImageScaleModels;

namespace ImagesScale.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        public MainWindowViewModel()
        {
            camera = new(true);
            camera.FrameChanged += OnNewFrame;
            //camera.Start();

            //if (!camera.IsCameraAvailable)
            //{
            //    ErrorText = "Камере недоступна. Отсутсвует. Или занята.";
            //}
        }

        public async Task Start()
        {
            await camera.Start();

            if (!camera.IsCameraAvailable)
            {
                ErrorText = "Камере недоступна. Отсутсвует. Или занята.";
            }
        }

        private void OnNewFrame(object? sender, FrameEventArgs e)
        {
            FrameData = e;
        }

        private CameraWinRT camera;
        //private void OnNewFrame(/*SoftwareBitmap softwareBitmap*/byte[] data, System.Drawing.Size imagesize, int frameID) => FrameData = (data, imagesize, frameID) ;
        //{
        //    dataController.UpdateImageSize(imagesize);
        //    Application.Current?.Dispatcher.Invoke((ThreadStart)delegate
        //    {
        //        ImageSource = camera.SoftwareBitmapToWriteableBitmap(softwareBitmap);
        //    });
        //}

        [ObservableProperty]
        private FrameEventArgs? _frameData;

        [ObservableProperty]
        private string _errorText = string.Empty;
    }
}
