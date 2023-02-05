using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;

namespace OurTeamViewer.Helpers
{
    public class ImageHelper
    {
        // Create a folder to store images
        public void CreateFolder(string path)
        {
            int number = 0;
            while (Directory.Exists(path))
            {
                // Add number to folder name if folder already exists
                if (number != 0)
                {
                    path.Replace((number - 1).ToString(), "");
                    path += number;
                }
                number++;
            }

            Directory.CreateDirectory(path);
        }


        //Convert a byte array to an image and save it to the folder
        public string GetImagePath(byte[] buffer, string path)
        {
            // Convert byte array to image
            ImageConverter ic = new ImageConverter();
            var data = ic.ConvertFrom(buffer);
            Image img = data as Image;

            // If the conversion was successful
            if (img != null)
            {
                Bitmap bitmap = new Bitmap(img);
                var strGuid = Guid.NewGuid().ToString();
                var imagePath = $@"{path}\image{strGuid}.png";
                bitmap.Save(imagePath);
                return imagePath;
            }
            else
            {
                return String.Empty;
            }
        }

        //// Take a screenshot and save it to the folder
        //public async Task<string> TakeScreenShotAsync()
        //{
        //    // Create a bitmap with the size of the screen
        //    Bitmap bmp = new Bitmap(1920, 1080);
        //    var id = Guid.NewGuid().ToString();
        //    var source = Path.Combine(FolderPath, "screenshot" + id + ".png");

        //    // Copy the screen to the bitmap
        //    using (Graphics g = Graphics.FromImage(bmp))
        //    {
        //        g.CopyFromScreen(0, 0, 0, 0, new Size(1920, 1080));
        //        bmp.Save(source);  // saves the image
        //    }
        //    return source;
        //}
    }
}
