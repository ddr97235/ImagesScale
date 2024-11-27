using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ImagesScale.Models;
using System.Windows;
using System.Windows.Media.Imaging;
using Windows.Graphics.Imaging;

namespace ImagesScale.ViewModels
{
    public partial class MainWindowViewModel: ObservableObject
    {       
        public MainWindowViewModel()
        {           
            camera = new(OnNewFrame);
            camera.Start();
            if(!camera.IsCameraAvailable)
            {
                ErrorText = "Камере недоступна. Отсутсвует. Или занята.";
            }
        }
        private CameraWinRT camera;
        private void OnNewFrame(/*SoftwareBitmap softwareBitmap*/byte[] data, System.Drawing.Size imagesize, int frameID) => FrameData = (data, imagesize, frameID) ;
        //{
        //    dataController.UpdateImageSize(imagesize);
        //    Application.Current?.Dispatcher.Invoke((ThreadStart)delegate
        //    {
        //        ImageSource = camera.SoftwareBitmapToWriteableBitmap(softwareBitmap);
        //    });
        //}

        [ObservableProperty]
        private (byte[] Data, System.Drawing.Size Imagesize, int FrameID)? _FrameData;
       
        [ObservableProperty]
        private string _ErrorText = String.Empty;
    }
}
