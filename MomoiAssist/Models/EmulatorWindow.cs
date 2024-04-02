using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MomoiAssist.Helpers;

namespace MomoiAssist.Models
{
    public class EmulatorWindow
    {
        public string Title { get; set; }
        public nint Handle { get; set; }
        public Bitmap Screenshot = null;


        public EmulatorWindow(string title, nint handle)
        {
            Title = title;
            Handle = handle;
        }

        public void UpdateScreenshot()
        {
            Screenshot = EmulatorWindowHelper.CaptureWindow(Handle);
            if (Screenshot != null)
            {
                Screenshot.Save("screenshot.png", System.Drawing.Imaging.ImageFormat.Png);
                // print the path of the screenshot
                Console.WriteLine("Current Directory: " + System.IO.Directory.GetCurrentDirectory());
                Console.WriteLine("Screenshot saved.");
            }

        }




    }
}
