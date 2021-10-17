using System;
using System.Diagnostics.CodeAnalysis;
using Aspose.Imaging;

namespace sentinel_index
{
    class Program
    {
        [SuppressMessage("ReSharper.DPA", "DPA0001: Memory allocation issues")]
        static void Main(string[] args)
        {
            Image image = Image.Load(@"T33TWM_20211009T095029_B01.jp2");
            RasterImage raster = (RasterImage) image;

            var height = image.Height;
            var width = image.Width;
            Console.WriteLine(raster.GetPixel(544, 547));
            
            // for (int i = 0; i < height; i++)
            // {   
                // Console.WriteLine(raster.GetPixel(i, 0).ToString());
            // }
            
            var options = new Aspose.Imaging.ImageOptions.PngOptions();
            image.Save(@"output.png", options);
        }
    }
}