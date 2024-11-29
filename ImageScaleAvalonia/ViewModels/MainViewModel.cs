using CommunityToolkit.Mvvm.ComponentModel;
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
    private void OnNewFrame(byte[] data, System.Drawing.Size imagesize, int frameID) => FrameData = (data, imagesize, frameID);    

    [ObservableProperty]
    private (byte[] Data, System.Drawing.Size Imagesize, int FrameID)? _FrameData;
}
