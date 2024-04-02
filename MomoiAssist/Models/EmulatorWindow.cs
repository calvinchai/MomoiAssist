﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using MomoiAssist.Helpers;
using Wpf.Ui.Controls;

namespace MomoiAssist.Models
{
    public class EmulatorWindow
    {
        public string Title { get; set; }
        public nint Handle { get; set; }
        public RECT Rect { get; set; }


        public Bitmap? Screenshot = null;
        public BitmapImage? ScreenshotImage = null;

        public EmulatorWindow(string title, nint handle)
        {
            Title = title;
            Handle = handle;
            StartScreenshotUpdateTimer();
        }
        private DispatcherTimer screenshotUpdateTimer = new DispatcherTimer();

        private void StartScreenshotUpdateTimer()
        {
            screenshotUpdateTimer.Interval = TimeSpan.FromSeconds(0.1); 
            screenshotUpdateTimer.Tick += UpdateScreenshot;
            screenshotUpdateTimer.Start();

        }
        public void UpdateScreenshot(object sender, EventArgs e)
        {
            UpdateScreenshot();
        }
        public void UpdateScreenshot()
        {
            Screenshot = EmulatorWindowHelper.CaptureWindowCopyScreen(Handle);
            //if (Screenshot != null)
            //{
            //    Screenshot.Save(Title+".png", System.Drawing.Imaging.ImageFormat.Png);

            //}
            UpdateScreenshotImage();
        }


        public void UpdateScreenshotImage()
        {
            ScreenshotImage = BitmapToImageSource(Screenshot);
        }


        BitmapImage? BitmapToImageSource(Bitmap? bitmap)
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

                return bitmapimage;
            }
        }


    }
}
