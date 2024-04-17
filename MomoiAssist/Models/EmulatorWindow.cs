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
        public RECT WindowRect { get; set; }
        public RECT Rect { get; set; }

        public Bitmap? Screenshot = null;
        public BitmapImage? ScreenshotImage = null;

        public EmulatorWindow(string title, nint handle)
        {
            Title = title;
            Handle = handle;
            Task.Run(() =>
            {
                while (true)
                {
                    UpdateScreenshot();
                }
            });
        }


        public void UpdateRect(object sender, EventArgs e)
        {
            User32.GetWindowRect(Handle, out RECT WindowRect);
            this.WindowRect = WindowRect;

            User32.ClientToScreen(Handle, out POINT point);
            var horizontal_offset = point.X - WindowRect.Left;
            var vertical_offset = point.Y - WindowRect.Top;
            Rect = new RECT
            {
                Left = WindowRect.Left + horizontal_offset,
                Top = WindowRect.Top + vertical_offset,
                Right = WindowRect.Right - horizontal_offset,
                Bottom = WindowRect.Bottom - horizontal_offset
            };

        }


        public void UpdateScreenshot(object sender, EventArgs e)
        {
            UpdateScreenshot();
        }
        public void UpdateScreenshot()
        {
            Screenshot = EmulatorWindowHelper.CaptureWindowGDI(Handle);

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
