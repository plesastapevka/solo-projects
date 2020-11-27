using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices.ComTypes;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using System.Drawing.Imaging;
using System.IO;

namespace vezne_tocke
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        static string path1;
        static string path2;

        static bool image1Uploaded = false;
        static int image1Count = 0;

        static bool image2Uploaded = false;
        static int image2Count = 0;

        static int pointCounter = 0;

        List<int> newPosPix;

        public List<Point> pic1;
        public List<Point> pic2;

        Bitmap transformedImg;

        public MainWindow()
        {
            InitializeComponent();
            pic1 = new List<Point>();
            pic2 = new List<Point>();
            label.Content = "Currently placed points: " + pointCounter;
            imagefinal.Visibility = System.Windows.Visibility.Hidden;
        }

        public List<int> calculateNewPos(Bitmap img1, Bitmap img2, int imgpos1, int imgpos2, int mainImg)
        {
            List<int> newPos = new List<int>();
            List<Point> tockeMain;
            List<Point> tockeSec;
            if (mainImg == 1)
            {
                tockeMain = pic1;
                tockeSec = pic2;
            }
            else
            {
                tockeMain = pic2;
                tockeSec = pic1;
            }

            int posX = imgpos1 + tockeMain[0].X;
            int posY = imgpos2 + tockeMain[0].Y;

            newPos.Add(posX - tockeSec[0].X);
            newPos.Add(posY - tockeSec[0].Y);

            System.Windows.Vector v1 = new System.Windows.Vector(tockeMain[0].X, tockeMain[0].Y);
            System.Windows.Vector v12 = new System.Windows.Vector(tockeMain[1].X, tockeMain[1].Y);
            System.Windows.Vector v2 = new System.Windows.Vector(tockeSec[0].X, tockeSec[0].Y);
            System.Windows.Vector v22 = new System.Windows.Vector(tockeSec[1].X, tockeSec[1].Y);

            System.Windows.Vector f1 = v22 - v2;
            System.Windows.Vector f2 = v12 - v1;
            newPos.Add((int)System.Windows.Vector.AngleBetween(f1, f2));

            return newPos;
        }

        private void GetPointBounds(PointF[] points,
        out float xmin, out float xmax,
        out float ymin, out float ymax)
        {
            xmin = points[0].X;
            xmax = xmin;
            ymin = points[0].Y;
            ymax = ymin;
            foreach (PointF point in points)
            {
                if (xmin > point.X) xmin = point.X;
                if (xmax < point.X) xmax = point.X;
                if (ymin > point.Y) ymin = point.Y;
                if (ymax < point.Y) ymax = point.Y;
            }

            if (ymin < 0)
            {
                ymax = ymax + Math.Abs(ymin);
            }
            else if (ymin > 0)
            {
                ymax = ymax - ymin;
            }

            if (xmin < 0)
            {
                xmax = xmax + Math.Abs(xmin);
            }
            else if (xmin > 0)
            {
                xmax = xmax - xmin;
            }
        }

        private void startButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Bitmap img1 = new Bitmap(path1);
            Bitmap img2 = new Bitmap(path2);
            img1.MakeTransparent();
            img2.MakeTransparent();

            int mainImg;
            if (image1.Height >= image2.Height && image1.Width >= image2.Width)
            {
                mainImg = 1;
            }
            else
            {
                mainImg = 2;
            }

            int width = (int)(image1.Width + image2.Height);
            int height = (int)(image1.Height + image2.Height);

            Bitmap bitmap = new Bitmap(width, height);

            //Draw first image
            using (var g = Graphics.FromImage(bitmap))
            {
                if (mainImg == 1)
                    g.DrawImage(img1, (width - img1.Width) / 2, (height - img1.Height) / 2, img1.Width, img1.Height);
                else
                    g.DrawImage(img2, (width - img2.Width) / 2, (height - img2.Height) / 2, img2.Width, img2.Height);
            }

            //Calculate starting (X,Y) for second image
            if (mainImg == 1)
                newPosPix = calculateNewPos(img1, img2, (width - img1.Width) / 2, (height - img1.Height) / 2, mainImg);
            else
                newPosPix = calculateNewPos(img2, img1, (width - img2.Width) / 2, (height - img2.Height) / 2, mainImg);


            //Rotation
            if (mainImg == 1)
            {

                int angle = newPosPix[2];

                Matrix rotate_at_center = new Matrix();

                Matrix rotate_at_origin = new Matrix();
                rotate_at_origin.Rotate((float)angle);
                PointF[] points =
                {
                    new PointF(0, 0),
                    new PointF(img2.Width, 0),
                    new PointF(img2.Width, img2.Height),
                    new PointF(0, img2.Height),
                };
                rotate_at_origin.TransformPoints(points);
                float xmin, xmax, ymin, ymax;
                GetPointBounds(points, out xmin, out xmax, out ymin, out ymax);

                PointF[] newPts =
                {
                    new PointF(pic2[0].X, pic2[0].Y),
                    new PointF(pic2[1].X, pic2[1].Y),
                };



                int wid = (int)Math.Round(xmax);
                int hgt = (int)Math.Round(ymax);

                transformedImg = new Bitmap(wid, hgt);

                rotate_at_center.RotateAt(angle,
                    new PointF(wid / 2f, hgt / 2f));

                rotate_at_origin.TransformPoints(newPts);

                newPts[0].X += Math.Abs(xmin);
                newPts[0].Y += Math.Abs(ymin);
                newPts[1].X += Math.Abs(xmin);
                newPts[1].Y += Math.Abs(ymin);

                //swap original points with new points
                pic2.Clear();
                pic2.Add(new Point((int)newPts[0].X, (int)newPts[0].Y));
                pic2.Add(new Point((int)newPts[1].X, (int)newPts[1].Y));


                using (Graphics g = Graphics.FromImage(transformedImg))
                {
                    // Use smooth image interpolation.
                    g.InterpolationMode = InterpolationMode.High;
                    //g.Clear(Color.LightBlue);
                    g.Transform = rotate_at_center;

                    int x = (wid - img2.Width) / 2;
                    int y = (hgt - img2.Height) / 2;
                    g.DrawImage(img2, x, y, img2.Width, img2.Height);
                    //g.DrawImage(img1, wid - img1.Width, hgt - img1.Height, img1.Width, img1.Height);
                }

                if (mainImg == 1)
                    newPosPix = calculateNewPos(img1, img2, (width - img1.Width) / 2, (height - img1.Height) / 2, mainImg);
                else
                    newPosPix = calculateNewPos(img2, img1, (width - img2.Width) / 2, (height - img2.Height) / 2, mainImg);
               
            }
            else
            {
                int angle = newPosPix[2];

                Matrix rotate_at_center = new Matrix();

                Matrix rotate_at_origin = new Matrix();
                rotate_at_origin.Rotate((float)angle);
                PointF[] points =
                {
                    new PointF(0, 0),
                    new PointF(img1.Width, 0),
                    new PointF(img1.Width, img1.Height),
                    new PointF(0, img1.Height),
                };
                rotate_at_origin.TransformPoints(points);
                float xmin, xmax, ymin, ymax;
                GetPointBounds(points, out xmin, out xmax, out ymin, out ymax);

                PointF[] newPts =
                {
                    new PointF(pic1[0].X, pic1[0].Y),
                    new PointF(pic1[1].X, pic1[1].Y),
                };



                int wid = (int)Math.Round(xmax);
                int hgt = (int)Math.Round(ymax);

                transformedImg = new Bitmap(wid, hgt);

                rotate_at_center.RotateAt(angle, new PointF(wid / 2f, hgt / 2f));


                // calc new pooints
                rotate_at_origin.TransformPoints(newPts);

                newPts[0].X += Math.Abs(xmin);
                newPts[0].Y += Math.Abs(ymin);
                newPts[1].X += Math.Abs(xmin);
                newPts[1].Y += Math.Abs(ymin);

                // switch points
                pic1.Clear();
                pic1.Add(new Point((int)newPts[0].X, (int)newPts[0].Y));
                pic1.Add(new Point((int)newPts[1].X, (int)newPts[1].Y));


                using (Graphics g = Graphics.FromImage(transformedImg))
                {
                    // image interpol
                    g.InterpolationMode = InterpolationMode.High;
                    g.Transform = rotate_at_center;

                    int x = (wid - img1.Width) / 2;
                    int y = (hgt - img1.Height) / 2;
                    g.DrawImage(img1, x, y, img1.Width, img1.Height);
                }

                if (mainImg == 1)
                    newPosPix = calculateNewPos(img1, img2, (width - img1.Width) / 2, (height - img1.Height) / 2, mainImg);
                else
                    newPosPix = calculateNewPos(img2, img1, (width - img2.Width) / 2, (height - img2.Height) / 2, mainImg);

            }

            // last step drawing
            using (var g = Graphics.FromImage(bitmap))
            {
                g.DrawImage(transformedImg, newPosPix[0], newPosPix[1], transformedImg.Width, transformedImg.Height);
            }

            // displaying image on app
            bitmap.Save("bitmap.png", ImageFormat.Png); // ImageFormat.Jpeg, etc
            MemoryStream memory = new MemoryStream();
            bitmap.Save(memory, ImageFormat.Png);
            memory.Position = 0;
            BitmapImage bitmapimage = new BitmapImage();
            bitmapimage.BeginInit();
            bitmapimage.StreamSource = memory;
            bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapimage.EndInit();

            imagefinal.Height = height;
            imagefinal.Width = width;
            imagefinal.Source = bitmapimage;

            image1.Visibility = System.Windows.Visibility.Hidden;
            image2.Visibility = System.Windows.Visibility.Hidden;

            imagefinal.Visibility = System.Windows.Visibility.Visible;
        }

        private void uploadImage1(object sender, MouseEventArgs e)
        {
            if (!image1Uploaded)
            {
                OpenFileDialog op = new OpenFileDialog
                {
                    Title = "Select a picture",
                    Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
                  "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
                  "Portable Network Graphic (*.png)|*.png"
                };
                if (op.ShowDialog() == true)
                {
                    path1 = op.FileName;
                    BitmapImage img = new BitmapImage(new Uri(op.FileName));
                    
                    double x = img.Width;
                    image1.Width = img.PixelWidth;
                    image1.Height = img.PixelHeight;
                    image1.Source = img;
                    image1Uploaded = true;
                }
            }
            else
            {
                if (image1Count < 2)
                {
                    System.Windows.Point p = e.GetPosition(image1);
                    
                    pic1.Add(new Point((int)p.X, (int)p.Y));
                    image1Count++;
                    pointCounter++;
                    label.Content = "Currently placed points: " + pointCounter;
                }
            }
        }

        private void uploadImage2(object sender, MouseEventArgs e)
        {
            if (!image2Uploaded)
            {
                OpenFileDialog op = new OpenFileDialog
                {
                    Title = "Select a picture",
                    Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
                  "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
                  "Portable Network Graphic (*.png)|*.png"
                };
                if (op.ShowDialog() == true)
                {
                    path2 = op.FileName;
                    BitmapImage img = new BitmapImage(new Uri(op.FileName));
                    image2.Width = img.PixelWidth;
                    image2.Height = img.PixelHeight;
                    image2.Source = img;
                    image2Uploaded = true;
                }
            }
            else
            {
                if (image2Count < 2)
                {
                    System.Windows.Point p = e.GetPosition(image2);
                    pic2.Add(new Point((int)p.X, (int)p.Y));
                    image2Count++;
                    pointCounter++;
                    label.Content = "Currently placed points: " + pointCounter;
                }
            }
        }
    }
}
