using System;
using System.Diagnostics.CodeAnalysis;
using Aspose.Imaging;
using OpenJpegDotNet;
using Image = Aspose.Imaging.Image;

namespace sentinel_index
{
    class Program
    {
        static void Main(string[] args)
        {
            RasterImage band2 = (RasterImage)Image.Load(@"T33TWM_20211009T095029_B02.jp2");
            RasterImage band4 = (RasterImage)Image.Load(@"T33TWM_20211009T095029_B04.jp2");
            RasterImage band8 = (RasterImage)Image.Load(@"T33TWM_20211009T095029_B08.jp2");
            // Color[] band4Pxls = band4.LoadPixels(band4.Bounds);
            // Color[] band8Pxls = band8.LoadPixels(band8.Bounds);
            // Color[] EVI = new Color[band2Pxls.Length];
            for (int i = 0; i < band2.Height; i++)
            {
                Console.WriteLine(band2.GetPixel(i, 0).R);
                break;
                // byte R = (band8Pxls[i].R - band4Pxls[i].R)
                // EVI[i] = new Color();
            }

            // var options = new Aspose.Imaging.ImageOptions.PngOptions();
            // image.Save(@"output.png", options);
        }
    }
}