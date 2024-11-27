using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using System.Windows.Media.Imaging;
using System.Runtime.InteropServices;
using WinRT;
using System.Windows;
using System.Windows.Media;
using Windows.Media.Capture.Frames;
using Windows.Media.Capture;
using Windows.Devices.Enumeration;
using System.Diagnostics;
using System.Windows.Controls;
using Windows.Media.MediaProperties;
using Windows.Gaming.Input;

namespace ImagesScale.Models
{
    internal class CameraWinRT
    {
        private MediaCapture? mediaCapture;
        private MediaFrameReader? mediaFrameReader;
        private event Action<byte[], System.Drawing.Size, int>? newFrame;
        private bool isColor = false;
        private int frameId = 0;
        public bool IsCameraAvailable { get; private set; } = false;
        private async Task<List<(string Name, string ID, Guid Guid)>> GetCameraList() => (await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture)).Select(x => (x.Name, x.Id, (Guid)x.Properties["System.Devices.ContainerId"])).ToList();
        
        public CameraWinRT(Action<byte[], System.Drawing.Size, int>? NewFrame, bool IsColor = false)
        {
            newFrame = NewFrame;
            isColor = IsColor;
        }

        public async void Start()
        {
            var ListCamera = await GetCameraList();
            if (ListCamera.Count > 0)
            {
                try
                {
                    mediaCapture = new();
                    await InitializeCameraAsync(ListCamera[0].ID);
                    //await AForge.AForgeCamera.SetMinGainAndGammaAsync(CurrentCamera.ID); // выполняется ассинхронно за ~100мсек                        
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Ошибка инициализации камеры: " + ex.Message);                    
                }
                finally { }
                await InitializeFrameReaderAsync();

                IsCameraAvailable = true;
            }
            else
            {
                IsCameraAvailable = false;
            }
        }

        private async Task InitializeCameraAsync(string ID)
        {
            var settings = new MediaCaptureInitializationSettings
            {
                VideoDeviceId = ID,
                //  MemoryPreference = MediaCaptureMemoryPreference.Cpu,
                SharingMode = MediaCaptureSharingMode.ExclusiveControl,
                StreamingCaptureMode = StreamingCaptureMode.Video
            };
            try // Initialize MediaCapture
            {
                await mediaCapture?.InitializeAsync(settings);
                //mediaCapture.CaptureDeviceExclusiveControlStatusChanged += CaptureDeviceExclusiveControlStatusChanged;// только после инициализации. ДО-нельзя.
            }
            catch (UnauthorizedAccessException)
            { // если нет доступа камере при запуски приложения
           
                Debug.WriteLine("Нет доступа к камере.");
            }
            finally{}
        }

        private async Task InitializeFrameReaderAsync()
        {
            try
            {
                await mediaCapture?.VideoDeviceController.SetMediaStreamPropertiesAsync(MediaStreamType.VideoPreview, GetResolutionOrDefault(1280, 720));
                mediaFrameReader = await mediaCapture?.CreateFrameReaderAsync(GetMediaFrameSource(), MediaEncodingSubtypes.Nv12);
                mediaFrameReader.FrameArrived += ColorFrameReader_FrameArrived;
                mediaFrameReader.AcquisitionMode = MediaFrameReaderAcquisitionMode.Realtime;
                var status = await mediaFrameReader.StartAsync();
                switch (status)
                {
                    case MediaFrameReaderStartStatus.ExclusiveControlNotAvailable:
                        Debug.WriteLine("Камера занята другой программой");
                        await StopmediaFrameReader();
                        break;
                    case MediaFrameReaderStartStatus.Success:
                        //mediaCapture.CaptureDeviceExclusiveControlStatusChanged -= CaptureDeviceExclusiveControlStatusChanged; // отсюда не убирать!                        
                        break;
                    default:
                        Debug.WriteLine("Неизвестная ошибка инициализации видеопотока камеры. Ошибка: " + status.ToString());
                        break;
                }
            }
            catch (UnauthorizedAccessException)
            {
                 Debug.WriteLine("The app was denied access to the camera");
            }
            finally{}
        }

        private IMediaEncodingProperties GetResolutionOrDefault(int width, int heigth, string mediaEncodingSubtypes = /*"MJPG"*/"NV12")
        {
            var resolutions = mediaCapture?.VideoDeviceController.GetAvailableMediaStreamProperties(MediaStreamType.Photo).ToList();
            var resolution = resolutions!.Where(p => ((VideoEncodingProperties)p).Width == width && ((VideoEncodingProperties)p).Height == heigth && ((VideoEncodingProperties)p).Subtype == mediaEncodingSubtypes).ToList();
            return resolution.Count == 1 ? resolution[0] : resolutions![0];
        }

        private MediaFrameSource GetMediaFrameSource()
        {
            if (mediaCapture == null)
            {
                throw new Exception("Не своевременный вызов GetMediaFrameSource");
            }
            var videoFrameSources = mediaCapture.FrameSources.Where(x => x.Value.Info.MediaStreamType == MediaStreamType.VideoRecord);
            if (videoFrameSources.Count() == 0)
            {
                //Debug.WriteLine("No ");
            }
            return videoFrameSources.FirstOrDefault().Value;
        }

        private async void ColorFrameReader_FrameArrived(MediaFrameReader sender, MediaFrameArrivedEventArgs args)
        {
            using (MediaFrameReference mediaFrameReference = sender.TryAcquireLatestFrame())
            {
                var videoMediaFrame = mediaFrameReference?.VideoMediaFrame;
                if (videoMediaFrame == null)
                {
                    return; // сюда попадаем только во время отладки
                }
                using (Windows.Graphics.DirectX.Direct3D11.IDirect3DSurface surface = videoMediaFrame!.Direct3DSurface)
                {                    
                    if (surface == null)
                    {
                        using (SoftwareBitmap colorSoftBitmap = SoftwareBitmap.Convert(videoMediaFrame.SoftwareBitmap /*softBitmap*/, BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied))
                        {
                            System.Drawing.Size imagesize = new(colorSoftBitmap.PixelWidth, colorSoftBitmap.PixelHeight);
                            //newFrame?.Invoke(colorSoftBitmap, imagesize);
                        }
                        return;
                    }


                    using (SoftwareBitmap softBitmap = await SoftwareBitmap.CreateCopyFromSurfaceAsync(surface))
                    {
                        if (isColor)
                        {
                            using (SoftwareBitmap colorSoftBitmap = SoftwareBitmap.Convert(softBitmap, BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied))
                            {
                                System.Drawing.Size imagesize = new(colorSoftBitmap.PixelWidth, colorSoftBitmap.PixelHeight);
                                //newFrame?.Invoke(colorSoftBitmap, imagesize);
                            }
                        }
                        else
                        {
                            System.Drawing.Size imagesize = new(softBitmap.PixelWidth, softBitmap.PixelHeight);
                            newFrame?.Invoke(GetFrame(softBitmap), imagesize, ++frameId);
                        }
                    }
                }
            }
        }
        private async Task StopmediaFrameReader()
        {
            if (mediaFrameReader != null)
            {
                await mediaFrameReader.StopAsync();
                mediaFrameReader.FrameArrived -= ColorFrameReader_FrameArrived;
                mediaFrameReader.Dispose();
                mediaFrameReader = null;
            }
        }
        public unsafe WriteableBitmap SoftwareBitmapToWriteableBitmap(SoftwareBitmap softwareBitmap)
        {
            WriteableBitmap writeableBitmap = new WriteableBitmap(softwareBitmap.PixelWidth, softwareBitmap.PixelHeight, 96, 96, BitmapPixelFormatToPixelFormat(softwareBitmap.BitmapPixelFormat) /*PixelFormats.Gray8*/, null);
            using (BitmapBuffer buffer = softwareBitmap.LockBuffer(BitmapBufferAccessMode.Read))
            using (var reference = buffer.CreateReference())
            {
                byte* data;
                uint capacity;
                reference.As<IMemoryBufferByteAccess>().GetBuffer(out data, out capacity);
                //var desc = buffer.GetPlaneDescription(0);
                byte[] pixels = new byte[softwareBitmap.PixelWidth * softwareBitmap.PixelHeight * (writeableBitmap.Format.BitsPerPixel / 8)];
                Marshal.Copy((IntPtr)data, pixels, 0, pixels.Length);
                Int32Rect rect = new Int32Rect(0, 0, softwareBitmap.PixelWidth, softwareBitmap.PixelHeight);
                writeableBitmap.WritePixels(rect, pixels, writeableBitmap.BackBufferStride, 0);
            }
            return writeableBitmap;
        }
        private static PixelFormat BitmapPixelFormatToPixelFormat(BitmapPixelFormat bitmapPixelFormat)
        {
            switch (bitmapPixelFormat)
            {
                case BitmapPixelFormat.Nv12:
                    return PixelFormats.Gray8;
                case BitmapPixelFormat.Bgra8:
                    return PixelFormats.Bgra32;
                default: throw new Exception("BitmapPixelFormatToPixelFormat не является универсальной функцией, и поддерживает только Bgra8 и Nv12(условно)");
            }
        }
        [ComImport]
        [Guid("5b0d3235-4dba-4d44-865e-8f1d0e4fd04d")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        unsafe interface IMemoryBufferByteAccess
        {
            void GetBuffer(out byte* buffer, out uint capacity);
        }

        private static unsafe byte[] GetFrame(SoftwareBitmap softwareBitmap)
        {
            byte[] res = new byte[softwareBitmap.PixelWidth * softwareBitmap.PixelHeight];
            using (BitmapBuffer buffer = softwareBitmap.LockBuffer(BitmapBufferAccessMode.Read))
            using (var reference = buffer.CreateReference())
            {
                byte* data;
                uint capacity;
                reference.As<IMemoryBufferByteAccess>().GetBuffer(out data, out capacity);
                fixed (byte* pSource = res)
                {
                    Buffer.MemoryCopy(data, pSource, res.Length, res.Length);
                }
                return res;
            }
        }
    }
}
