using System.IO;
using System.Drawing;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Tesseract;

// Install: "Install-Package Tesseract" using Package Manager Console with Nuget
namespace MomoiAssist.ViewModels {

    public class ImageRecognizer {

        private readonly TesseractEngine engine;

        public ImageRecognizer() { 
            this.engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.LstmOnly);
            this.engine.SetVariable("tessedit_char_whitelist", "0123456789:.");
        }

        public async Task<string> RecognizeTextAsync() {
            return await Task.Run(RecognizeText);
        }

        public string RecognizeText() {
            byte[] screenshot = ImageRecognizer.GetScreenShot(1640, 34, 160, 45);
            Pix image = Pix.LoadFromMemory(screenshot);

            var page = this.engine.Process(image);
            var text = page.GetText().Trim();
            page.Dispose();

            return text;
        }

        public static byte[] GetScreenShot(int x, int y, int width, int height) {
            Bitmap bitmap = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(bitmap);

            g.CopyFromScreen(x, y, 0, 0, new System.Drawing.Size(width, height));
            
            MemoryStream memoryStream = new MemoryStream();
            bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);

            return memoryStream.ToArray();
        }

        public static void SaveImage(byte[] imageBytes, string filePath, System.Drawing.Imaging.ImageFormat format) {
            MemoryStream ms = new MemoryStream(imageBytes);
            Image image = Image.FromStream(ms);
            
            image.Save(filePath, format);
            Trace.WriteLine("Image saved");
        }

        public static bool IsValidDurationFormat(string text) {
            string pattern = @"^\d{2}:\d{2}\.\d{3}$";
            return Regex.IsMatch(text, pattern);
        }
    }

}
