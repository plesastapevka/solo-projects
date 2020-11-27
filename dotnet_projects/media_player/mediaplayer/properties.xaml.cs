using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace mediaplayer
{
    /// <summary>
    /// Interaction logic for properties.xaml
    /// </summary>
    public partial class properties : Window
    {
        MainWindow.Song sel;

        public properties()
        {
            InitializeComponent();

            string[] genres = Properties.Settings.Default.genres.ToString().Split('/');
            string[] tmp;
            foreach (string s in genres)
            {
                tmp = s.Split(' ');
                if (tmp.Count() > 1)
                {
                    comboBox.Items.Add(tmp[1]);
                }
            }

            sel = ((MainWindow)Application.Current.MainWindow).selected;
            //lvid = ((MainWindow)Application.Current.MainWindow).dc.songs[id];
            naslov.Text = sel.Title;
            if (sel.Artist != null && sel.Artist != "")
            {
                author.Text = sel.Artist;
            }
            else
            {
                author.Text = "unknown";
            }
            comboBox.Text = sel.genre;
            path.Text = sel.PathLocation;
            if (sel.imageSource != null && sel.imageSource != "")
            {
                buttonimg.Source = new BitmapImage(new Uri(sel.ImageSource));
            }
        }

        public void change_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Select a picture";
            op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
              "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
              "Portable Network Graphic (*.png)|*.png";
            if (op.ShowDialog() == true)
            {
                buttonimg.Source = new BitmapImage(new Uri(op.FileName));
                sel.ImageSource = "@" + op.FileName;
            }

        }

        private void vredu_Click(object sender, RoutedEventArgs e)
        {
            sel.title = naslov.Text;
            sel.Artist = author.Text;
            sel.Genre = comboBox.Text;
            sel.imageSource = buttonimg.Source.ToString();
            ((MainWindow)Application.Current.MainWindow).Playlist.ItemsSource = ((MainWindow)Application.Current.MainWindow).ListViewItemsCollections;
            this.Close();
        }

        private void zapri_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
