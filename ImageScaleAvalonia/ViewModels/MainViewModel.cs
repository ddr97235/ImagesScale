using ImageScaleModels;

namespace ImageScaleAvalonia.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    public MainViewModel()
    {
        camera = new(OnNewFrame, true);
        camera.Start();
    }
    private CameraWinRT camera;
    private void OnNewFrame(/*SoftwareBitmap softwareBitmap*/byte[] data, System.Drawing.Size imagesize, int frameID) //=> FrameData = (data, imagesize, frameID);
    {

    }
    //{
    //    dataController.UpdateImageSize(imagesize);
    //    Application.Current?.Dispatcher.Invoke((ThreadStart)delegate
    //    {
    //        ImageSource = camera.SoftwareBitmapToWriteableBitmap(softwareBitmap);
    //    });
    //}
    public string Greeting => "Welcome to Avalonia!";
}
