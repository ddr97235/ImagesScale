using System.Drawing;

//using System.Windows.Media.Imaging;
//using System.Windows.Media;
//using System.Windows.Controls;
//using System.Windows.Navigation;
//using System.Windows.Shapes;

namespace ImageScaleModels
{
    public class FrameEventArgs(byte[] frame, Size size, int frameId) : EventArgs
    {
        public byte[] Frame { get; } = frame;
        public Size Size { get; } = size;

        public int FrameId { get; } = frameId;
    }
}
