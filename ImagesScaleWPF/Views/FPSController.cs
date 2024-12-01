using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImagesScale.Views
{
    internal class FPSController
    {
        private int _CurrentFPS = 0;
        private int _CurrentShowFPS = 0;
        private readonly TimeSpan OneSecond = new TimeSpan(0, 0, 1);
        private DateTime _StartSecondTime = DateTime.Now;
        /// <summary>
        /// Требуется вызывать для каждого пришедшего с камеры кадра.
        /// </summary>
        /// <param name="framenumber">Номер кадра</param>
        /// <param name="isNeedEven">Какой тип кадров(true - четный, false - нечетный, null - все кадры) считать в _CurrentShowFPS </param>
        /// <param name="showFPS"> Раз в секунду обратнвый вызов с результами FPS</param>
        public bool AddFrame(int framenumber, bool? isNeedEven, Action<int, int>? showFPS)
        {
            DateTime CurrentdateTime = DateTime.Now;
            if (CurrentdateTime - _StartSecondTime >= OneSecond)
            {
                showFPS?.Invoke(_CurrentFPS, _CurrentShowFPS); // раз в секунду отображаем FPSы
                _CurrentFPS = 0;
                _CurrentShowFPS = 0;
                _StartSecondTime = CurrentdateTime;
            }
            _CurrentFPS++;
            bool IsCurrentFrameEven = framenumber % 2 == 0;
            _CurrentShowFPS = isNeedEven switch
            {
                true => IsCurrentFrameEven ? _CurrentShowFPS + 1 : _CurrentShowFPS,
                false => IsCurrentFrameEven ? _CurrentShowFPS : _CurrentShowFPS + 1,
                null => _CurrentShowFPS + 1
            };
            return isNeedEven==null || IsCurrentFrameEven== isNeedEven;
        }
    }
}
