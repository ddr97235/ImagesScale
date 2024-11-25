using System.Drawing;

namespace ImagesScale.Models
{
    internal class DataController
    {
        private Size _ImageSize = new Size(0,0);
        private Size _ImageScaleSize = new Size(0, 0);
        public Size ImageScaleSize { get => _ImageScaleSize; }
        /// <summary>Содержит желаемый центр. Не всегда соответсвует реальному.</summary>
        private Point _CentrePoint = new Point(0,0);
        /// <summary> Левый верхний угол</summary>
        private Point _StartBorderPoint = new Point(0, 0);
        public Point StartBorderPoint { get => _StartBorderPoint; }
        public Point Image2_Offset 
        {
            get => new((int)(-_StartBorderPoint.X * ScaleIndexToScale(_ScaleIndex)),
                       (int)(-_StartBorderPoint.Y * ScaleIndexToScale(_ScaleIndex)));
        }
        public double UpScale { get => 1 / ScaleIndexToScale(_ScaleIndex); }
        private int _ScaleIndex = 0;
        private event Action? changeData;

        public DataController(Action? ChangeData)
        {
            this.changeData = ChangeData;
        }

        public void UpdateImageSize(Size size)
        {
            bool isChanged = !size.Equals(this._ImageSize);
            _ImageSize = size;
            if (isChanged)
            {
                UpdateScale(_ScaleIndex);
            }
        }

        public void UpdateScale(int scaleIndex)
        {
            _ScaleIndex = scaleIndex;
            double k = ScaleIndexToScale(_ScaleIndex);
            _ImageScaleSize = new((int)(_ImageSize.Width * k), (int)(_ImageSize.Height*k));
            UpdateCenterBorder(_CentrePoint);
        }

        public void UpdateCenterBorder(Point point)
        {
            _CentrePoint = point;
            int HalfWidth = _ImageScaleSize.Width / 2;
            int HalfHeigth = _ImageScaleSize.Height / 2;
            _StartBorderPoint = new(_CentrePoint.X + HalfWidth > _ImageSize.Width ? _ImageSize.Width - _ImageScaleSize.Width :
                                       _CentrePoint.X - HalfWidth < 0 ? 0 : _CentrePoint.X - HalfWidth,
                                    _CentrePoint.Y + HalfHeigth > _ImageSize.Height ? _ImageSize.Height - _ImageScaleSize.Height :
                                       _CentrePoint.Y - HalfHeigth < 0 ? 0 : _CentrePoint.Y - HalfHeigth);
            changeData?.Invoke();
        }    

        private double ScaleIndexToScale(int index)
        {
            return index switch
            {
                0 => 1d,
                1 => 0.5d,
                2 => 0.25d,
                3 => 0.1d,
                _ => throw new NotImplementedException()
            };
        }
    }
}
