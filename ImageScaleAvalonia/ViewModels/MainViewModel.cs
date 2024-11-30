using CommunityToolkit.Mvvm.ComponentModel;
using ImageScaleModels;
using System.Threading.Tasks;

namespace ImageScaleAvalonia.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    public MainViewModel()
    {
        camera = new(true);
        camera.FrameChanged += OnNewFrame;
    }
    private CameraWinRT camera;

    private void OnNewFrame(object? sender, FrameEventArgs e)
    {
        FrameData = e;
    }
    public async Task Start()
    {
        await camera.Start();

        if (!camera.IsCameraAvailable)
        {
            ErrorText = "Камере недоступна. Отсутсвует. Или занята.";
        }
    }

    [ObservableProperty]
    private FrameEventArgs? _frameData;
    
    [ObservableProperty]
    private string _errorText = string.Empty;
}
