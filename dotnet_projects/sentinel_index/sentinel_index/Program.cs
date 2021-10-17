using System;
using System.Diagnostics.CodeAnalysis;
using Aspose.Imaging;
using Aspose.Imaging.FileFormats.Core.VectorPaths;
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
            Color[] band2Pxls = band2.LoadPixels(band2.Bounds);
            Color[] band4Pxls = band4.LoadPixels(band4.Bounds);
            Color[] band8Pxls = band8.LoadPixels(band8.Bounds);
            var width = band2.Size.Width;
            var height = band2.Size.Height;
            Color[] EVI = new Color[width * height];
            for (int i = 0; i < band2Pxls.Length; i++)
            {
                byte R = (byte) (band8Pxls[i].R - band4Pxls[i].R);
                byte G = (byte) (band8Pxls[i].G - band4Pxls[i].G);
                byte B = (byte) (band8Pxls[i].B - band4Pxls[i].B);
                EVI[i] = Color.FromArgb(R, G, B);
            }
            Console.WriteLine("EVI calculated");
            // var options = new Aspose.Imaging.ImageOptions.PngOptions();
            // image.Save(@"output.png", options);
        }
    }
}