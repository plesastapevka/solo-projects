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
    /// Interaction logic for settings.xaml
    /// </summary>
    public partial class settings : Window
    {
        ListBoxItem itm;
        string[] genres;
        public settings()
        {
            InitializeComponent();
            genres = Properties.Settings.Default.genres.ToString().Split('/');
            string[] tmp;
            foreach(string s in genres)
            {
                tmp = s.Split(' ');
                if (tmp.Count() > 1)
                {
                    itm = new ListBoxItem();
                    itm.Content = tmp[1];
                    listBox.Items.Add(itm);
                }
            }
        }

        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void apply_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder sb = new StringBuilder();

            foreach (object s in listBox.Items)
            {
                sb.Append(s.ToString() + "/");
            }
            string genresOut = sb.ToString();
            Console.WriteLine(genresOut);
            Properties.Settings.Default.genres = sb.ToString();
            Properties.Settings.Default.Save(); //to shrani settingse
            this.Close();
        }

        private void addgenre_Click(object sender, RoutedEventArgs e)
        {
            ListBoxItem itm = new ListBoxItem();
            itm.Content = genre.Text;

            listBox.Items.Add(itm);
        }

        private void remgenre_Click(object sender, RoutedEventArgs e)
        {
            listBox.Items.RemoveAt(listBox.SelectedIndex);
        }
    }
}
