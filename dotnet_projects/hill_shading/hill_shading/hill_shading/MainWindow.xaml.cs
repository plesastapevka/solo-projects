using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace hill_shading
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        float[,] output;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void open_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.InitialDirectory = @"C:\Users\urban\OneDrive - Univerza v Mariboru\3_letnik\2_semester\GIS\vaja04_hill_shading";
            dialog.Filter = "Tiff, tif images (*.tiff;*.tif)|*.tiff;*.tif";
            if (dialog.ShowDialog() == true)
            {
                Stream imageSource = new FileStream(dialog.FileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                TiffBitmapDecoder decoder = new TiffBitmapDecoder(imageSource, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
                BitmapSource source = decoder.Frames[0];

                byte[] bytes = new byte[source.PixelHeight * source.PixelWidth * 4];
                float[] visine = new float[source.PixelWidth * source.PixelHeight];

                source.CopyPixels(bytes, source.PixelWidth * 4, 0);

                int counter = 0;
                for (int i = 0; i < bytes.Length;)
                {
                    byte[] tmp = new byte[4];
                    tmp[0] = bytes[i++];
                    tmp[1] = bytes[i++];
                    tmp[2] = bytes[i++];
                    tmp[3] = bytes[i++];
                    visine[counter++] = BitConverter.ToSingle(tmp, 0);
                }
                image.Source = source;
                output = new float[source.PixelHeight, source.PixelWidth];
                counter = 0;
                for (int i = 0; i < source.PixelHeight; i++)
                {
                    for (int j = 0; j < source.PixelWidth; j++)
                    {
                        output[i, j] = visine[counter++];
                    }
                }
            }
        }

        private void start_Click(object sender, RoutedEventArgs e)
        {
            if (!Double.TryParse(azimuthBox.Text, out double azimuth) || !Double.TryParse(zenithBox.Text, out double zenith))
            {
                MessageBox.Show("Azimuth or Zenith not a valid number");
                return;
            }
            azimuth = (Math.PI / 180) * azimuth;
            zenith = (Math.PI / 180) * zenith;

            float dzdx = 0, dzdy = 0;
            double slope = 0, aspect = 0;
            float min = 0, max = 0;
            int height = output.GetLength(0);
            int width = output.GetLength(1);
            float[,] newImg = new float[height, width];
            for (int i = 1; i < height - 1; i++)
            {
                for (int j = 1; j < width - 1; j++)
                {
                    dzdx = ((output[i + 1, j - 1] + 2 * output[i + 1, j] + output[i + 1, j + 1]) - (output[i - 1, j - 1] + 2 * output[i - 1, j] + output[i - 1, j + 1])) / 8;
                    dzdy = ((output[i - 1, j + 1] + 2 * output[i, j + 1] + output[i + 1, j + 1]) - (output[i - 1, j - 1] + 2 * output[i, j - 1] + output[i + 1, j - 1])) / 8;
                    slope = Math.Max(Math.Atan(Math.Sqrt(Math.Pow(dzdx, 2) + Math.Pow(dzdy, 2))), 0);

                    if (slope > 0)
                    {
                        aspect = (Math.Atan2(dzdy, -dzdx) + 2 * Math.PI) % (2 * Math.PI);
                    }
                    else
                    {
                        if (dzdy > 0) aspect = Math.PI / 2;
                        else aspect = (3 * Math.PI) / 2;
                    }
                    float val = (float)(Math.Cos(-zenith) * Math.Cos(slope) + (Math.Cos(azimuth - aspect) * Math.Sin(-zenith) * Math.Sin(slope)));
                    newImg[i, j] = val;
                    if (val > max) max = val;
                    if (val < min) min = val;
                }
            }

            saveImage(newImg, min, max);
            label.Content = "Image exported.";
            label.Visibility = Visibility.Visible;

            // normalize
            //for (int i = 0; i < height; i++)
            //{
            //    for (int j = 0; j < width; j++)
            //    {
            //        newImg[i, j] = ((newImg[i, j] - min) / (max - min)) * float.MaxValue;
            //    }
            //}

            //int[,] nwimg = new int[height, width];

            //for (int x = 0; x < height; ++x)
            //{
            //    for (int y = 0; y < width; ++y)
            //    {
            //        nwimg[x, y] = BitConverter.ToInt32(BitConverter.GetBytes(newImg[x, y]), 0);
            //    }
            //}
        }

        void saveImage(float[,] data, float min, float max)
        {
            double range = max - min;
            byte v;

            Bitmap bm = new Bitmap(data.GetLength(0), data.GetLength(1));
            BitmapData bd = bm.LockBits(new System.Drawing.Rectangle(0, 0, bm.Width, bm.Height), ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            // This is much faster than calling Bitmap.SetPixel() for each pixel.
            unsafe
            {
                byte* ptr = (byte*)bd.Scan0;
                for (int j = 0; j < bd.Height; j++)
                {
                    for (int i = 0; i < bd.Width; i++)
                    {
                        v = (byte)(255 * (data[i, bd.Height - 1 - j] - min) / range);
                        ptr[0] = v;
                        ptr[1] = v;
                        ptr[2] = v;
                        ptr[3] = (byte)255;
                        ptr += 4;
                    }
                    ptr += (bd.Stride - (bd.Width * 4));
                }
            }

            bm.UnlockBits(bd);
            bm.Save("output.tiff", ImageFormat.Tiff);
        }
    }
}
