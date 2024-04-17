using MomoiAssist.Helpers;
using MomoiAssist.Helpers.PInvoke;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;

namespace MomoiAssist.Models
{
    public class EmulatorWindow
    {
        public string Title { get; set; }
        public nint Handle { get; set; }

        public Rectangle WindowRect { get; set; }
        public Rectangle Rect { get; set; }

        public Bitmap? Screenshot = null;
        public BitmapImage? ScreenshotImage = null;

        public EmulatorWindow(string title, nint handle)
        {
            Title = title;
            Handle = handle;
            //TODO: Cancel this task when the window is closed
            
            Task.Run(() =>
            {
                while (true)
                {
                    UpdateScreenshot();
                }
            });
        }


        public void UpdateRect()
        {
            WindowRect = EmulatorWindowHelper.GetWindowRect(Handle);
            Rect=  EmulatorWindowHelper.GetClientRect(Handle);
        }



        public void UpdateScreenshot()
        {
            UpdateRect();
            Screenshot = EmulatorWindowHelper.CaptureWindowGDI(Handle, Rect.Left-WindowRect.Left, Rect.Top-WindowRect.Top, Rect.Width, Rect.Height);

            UpdateScreenshotImage();
            if (lastTime == 0)
            {
                lastTime = Environment.TickCount;
            }
            else
            {
                int currentTime = Environment.TickCount;
                if (currentTime - lastTime == 0)
                {
                    return;
                }
                //Console.WriteLine("FPS: " + 1000 / (currentTime - lastTime));
                lastTime = currentTime;
            }
        }

        int lastTime = 0;
        public void UpdateScreenshotImage()
        {
            ScreenshotImage = BitmapToImageSource(Screenshot);

        }


        public static BitmapImage? BitmapToImageSource(Bitmap? bitmap)
        {
            if (bitmap == null)
            {
                return null;
            }

            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();
                bitmapimage.Freeze();
                return bitmapimage;
            }
        }


    }
}
