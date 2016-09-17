using System.Drawing;

namespace MustafaUğuz.PES2017.Image
{
    public class DDS
    {
        public static System.Drawing.Image GetImage(string inputFilePath)
        {
            return DevIL.DevIL.LoadBitmap(inputFilePath);
        }

        public static bool SaveImage(string outputFilePath, System.Drawing.Image image)
        {
            return DevIL.DevIL.SaveBitmap(outputFilePath, new Bitmap(image));
        }
    }
}
