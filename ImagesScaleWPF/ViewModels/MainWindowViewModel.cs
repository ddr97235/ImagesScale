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
            dataController = new(OnChangeData);
            dataController.UpdateScale(_ScaleIndex); // ..скорее нужно перенести в конструктор.
            camera = new(OnNewFrame);
            camera.Start();
            if(!camera.IsCameraAvailable)
            {
                ErrorText = "Камере недоступна. Отсутсвует. Или занята.";
            }
        }
        private CameraWinRT camera;
        private DataController dataController;

        private  void OnNewFrame(SoftwareBitmap softwareBitmap, System.Drawing.Size imagesize)
        {
            dataController.UpdateImageSize(imagesize);
            Application.Current?.Dispatcher.Invoke((ThreadStart)delegate
            {
                ImageSource = camera.SoftwareBitmapToWriteableBitmap(softwareBitmap);
            });
        }
        
        [ObservableProperty]
        private WriteableBitmap? _ImageSource;
        
        [ObservableProperty]
        private int _ScaleIndex = 2;

        [ObservableProperty]
        private double _BorderStartX = 0d;
        [ObservableProperty]
        private double _BorderStartY = 0d;
        [ObservableProperty]
        private double _BorderWidth = 0d;
        [ObservableProperty]
        private double _BorderHeigth = 0d;

        [ObservableProperty]
        private double _Image2_OffsetX = 0d;
        [ObservableProperty]
        private double _Image2_OffsetY = 0d;
        [ObservableProperty]
        private double _UpScale = 2d;

        [RelayCommand]
        private void ImageClick(object point) => dataController.UpdateCenterBorder((System.Drawing.Point)point);       
        private void OnChangeData()
        {
            BorderStartX = dataController.StartBorderPoint.X;
            BorderStartY = dataController.StartBorderPoint.Y;
            BorderWidth = dataController.ImageScaleSize.Width;
            BorderHeigth = dataController.ImageScaleSize.Height;
            Image2_OffsetX = dataController.Image2_Offset.X;
            Image2_OffsetY = dataController.Image2_Offset.Y;
            UpScale = dataController.UpScale;
        }
        partial void OnScaleIndexChanged(int value) => dataController.UpdateScale(value);
        [ObservableProperty]
        private string _ErrorText = String.Empty;
    }
}
