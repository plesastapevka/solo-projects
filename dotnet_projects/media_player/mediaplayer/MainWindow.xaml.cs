using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml.Serialization;

namespace mediaplayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int ID = 0;
        bool isPlay = false;
        public int selID { get; set; }
        public DataContainer dc;
        public Song selected;
        private DispatcherTimer _timer = null;
        private DispatcherTimer _duration = null;
        private TimeSpan _position;
        public Song current;
        bool repeatPlay;
        int currentTrack;
        bool shufflePlay;
        Random rnd;

        public ObservableCollection<Song> ListViewItemsCollections { get { return _ListViewItemsCollections; } }
        ObservableCollection<Song> _ListViewItemsCollections = new ObservableCollection<Song>();
        

        public MainWindow()
        {
            string lang = Properties.Settings.Default.lang.ToString();
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(lang);
            InitializeComponent();
            rnd = new Random();
            shufflePlay = false;
            repeatPlay = false;
            mediaElement.LoadedBehavior = MediaState.Manual;
            mediaElement.UnloadedBehavior = MediaState.Stop;
            dc = new DataContainer();
            current = new Song();
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(30);
            _timer.Tick += _timer_Tick;
            _timer.Start();

            _duration = new DispatcherTimer();
            _duration.Interval = TimeSpan.FromSeconds(1);
            _duration.Tick += _duration_Tick;
            _duration.Start();
            deserialize();
        }

        public void deserialize()
        {
            XmlSerializer ser = new XmlSerializer(typeof(PlaylistToExport));
            StreamReader reader = new StreamReader(@"..\..\podatki.xml");
            PlaylistToExport pl = new PlaylistToExport();
            pl = (PlaylistToExport)ser.Deserialize(reader);
            ID = 0;
            foreach (Song t in pl.songs)
            {
                ListViewItemsCollections.Add(t);
                dc.Add(t, ID);

                Playlist.ItemsSource = ListViewItemsCollections;
                ID++;
            }
        }

        public class Song : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;
            public bool IsPlaying
            {
                get { return isPlaying; }
                set
                {
                    isPlaying = value;
                    OnPropertyChanged(new PropertyChangedEventArgs(""));
                }
            }

            public void OnPropertyChanged(PropertyChangedEventArgs e)
            {
                if(PropertyChanged != null)
                {
                    PropertyChanged(this, e);
                }
            }

            public string imageSource
            {
                get { return ImageSource; }
                set
                {
                    ImageSource = value;
                    OnPropertyChanged(new PropertyChangedEventArgs(""));
                }
            }

            public string title
            {
                get { return Title; }
                set
                {
                    Title = value;
                    OnPropertyChanged(new PropertyChangedEventArgs(""));
                }
            }

            public string Artist
            {
                get { return artist; }
                set
                {
                    artist = value;
                    OnPropertyChanged(new PropertyChangedEventArgs(""));
                }
            }

            public string Desc
            {
                get { return desc; }
                set
                {
                    desc = value;
                    OnPropertyChanged(new PropertyChangedEventArgs(""));
                }
            }

            public string Duration
            {
                get { return duration; }
                set
                {
                    duration = value;
                    OnPropertyChanged(new PropertyChangedEventArgs(""));
                }
            }
            public string Genre
            {
                get { return genre; }
                set
                {
                    genre = value;
                    OnPropertyChanged(new PropertyChangedEventArgs(""));
                }
            }

            public int songID { get; set; }
            public string ImageSource { get; set; }
            public string Title { get; set; }
            public string xmlID { get; set; }
            public string PathLocation { get; set; }
            public string artist { get; set; }
            public string desc { get; set; }
            public string duration { get; set; }
            public string genre { get; set; }
            public bool isPlaying { get; set; }

        }

        public class DataContainer //playlist
        {
            public Dictionary<int, Song> songs;

            public DataContainer() { songs = new Dictionary<int, Song>(); }

            public void Add(Song lwd, int ID)
            {
                songs.Add(ID, lwd);
            }

            public void Remove(int ID)
            {
                songs.Remove(ID);
            }

            public Dictionary<int, Song> Songs
            {
                get { return songs; }
                set
                {
                    songs = value;
                }
            }
        }

        public class PlaylistToExport
        {
            [XmlElement(Order = 1, ElementName = "id")]
            public int id;

            [XmlElement(Order = 2, ElementName = "naslov")]
            public string naslov;

            [XmlElement(Order = 3, ElementName = "pesmi")]
            public List<Song> songs;

            public PlaylistToExport() { naslov = "default"; songs = new List<Song>(); }
            public PlaylistToExport(List<Song> pesmi, string ime) { naslov = ime; songs = pesmi; }
        }

        private void exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void valueChanged_Changed(object sender, RoutedEventArgs e)
        {
            mediaElement.Position = TimeSpan.FromSeconds(slider.Value);
            label.Content = TimeSpan.FromSeconds(slider.Value).ToString(@"hh\:mm\:ss");
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            serialize();
        }

        private void lang_Click(object sender, EventArgs e)
        {
            if(Properties.Settings.Default.lang.ToString() == "si")
            {
                Properties.Settings.Default.genres = "en";
                Properties.Settings.Default.Save(); //to shrani settingse
            }
            else if(Properties.Settings.Default.lang.ToString() == "en")
            {
                Properties.Settings.Default.genres = "si";
                Properties.Settings.Default.Save(); //to shrani settingse
            }
            Application.Current.Shutdown();
            System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
        }

        private void add_Click(object sender, RoutedEventArgs e)
        {
            Song lvid = new Song()
            {
                ImageSource = "",
                Title = "Naslov",
                xmlID = ID.ToString(),
                artist = "Urban",
                PathLocation = "..\\..\\restart.mp3",
                duration = "4:20"
            };
            ListViewItemsCollections.Add(lvid);
            dc.Add(lvid, ID);

            Playlist.ItemsSource = ListViewItemsCollections;
            ID++;
        }

        private void remove_Click(object sender, RoutedEventArgs e)
        {
            if (Playlist.SelectedItems.Count > 0)
            {
                while (Playlist.SelectedItems.Count > 0)
                {
                    ID--;
                    Song s = (Song)Playlist.SelectedItem;
                    ListViewItemsCollections.Remove((Song)Playlist.SelectedItem);
                    dc.Remove(ID);
                }
            }
            else
            {
                MessageBoxResult invalid = MessageBox.Show("Nobena datoteka ni izbrana.",
                                          "Napaka",
                                          MessageBoxButton.OK,
                                          MessageBoxImage.Error);
            }
        }

        private void import_Click(object sender, RoutedEventArgs e) //vnesemo izbran playlist
        {
            OpenFileDialog fdlg = new OpenFileDialog();
            fdlg.Multiselect = true;
            fdlg.Filter = "XML Files (.xml)|.xml";
            if (fdlg.ShowDialog() == true)
            {
                dc.songs.Clear();
                ID = 0;

                XmlSerializer ser = new XmlSerializer(typeof(PlaylistToExport));
                StreamReader reader = new StreamReader(fdlg.OpenFile());
                PlaylistToExport p = new PlaylistToExport();
                p = (PlaylistToExport)ser.Deserialize(reader);

                foreach (Song s in p.songs)
                {
                    ListViewItemsCollections.Add(s);
                    dc.Add(s, ID);
                    ID++;
                }
            }
            Playlist.ItemsSource = ListViewItemsCollections;
        }

        private void save_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog fldg = new SaveFileDialog();
            fldg.Filter = "XML Files (.xml)|.xml";
            if (fldg.ShowDialog() == true)
            {
                List<Song> tmp = new List<Song>();
                foreach (var x in dc.songs.Values)
                {
                    tmp.Add(x);
                }

                PlaylistToExport export = new PlaylistToExport(tmp, "playlist1");

                XmlSerializer serializer = new XmlSerializer(typeof(PlaylistToExport));
                TextWriter writer = new StreamWriter(fldg.FileName);
                serializer.Serialize(writer, export);
                writer.Close();
            }
        }

        private void showName_DoubleClick(object sender, RoutedEventArgs e)
        {
            current.IsPlaying = false;
            Song s = (Song)Playlist.SelectedItem;
            mediaElement.Source = new Uri(s.PathLocation);
            current = s;
            current.IsPlaying = true;
            mediaElement.Play();
            currentTrack = Playlist.SelectedIndex;

            //_position = TimeSpan.Parse(current.duration);
            //slider.Minimum = 0;
            //slider.Maximum = _position.TotalSeconds;
            //mediaElement.Play();
            play.Content = FindResource("Stop");
            isPlay = true;

        }

        public static bool GetDuration(string filename, out TimeSpan duration)
        {
            try
            {
                Shell32.Shell shl = new Shell32.Shell();
                var fldr = shl.NameSpace(System.IO.Path.GetDirectoryName(filename));
                var itm = fldr.ParseName(System.IO.Path.GetFileName(filename));

                var propValue = fldr.GetDetailsOf(itm, 27);

                return TimeSpan.TryParse(propValue, out duration);
            }
            catch (Exception)
            {
                duration = new TimeSpan();
                return false;
            }
        }

        private void play_Click(object sender, RoutedEventArgs e)
        {
            if (Playlist.SelectedItems.Count == 1)
            {
                current.IsPlaying = false;
                Song s = (Song)Playlist.SelectedItem;
                mediaElement.Source = new Uri(s.PathLocation);
                current = s;
                current.IsPlaying = true;
                currentTrack = Playlist.SelectedIndex;
                mediaElement.Play();
            }
            else
            {
                MessageBoxResult invalid = MessageBox.Show("Izberite eno datoteko.",
                                          "Napaka",
                                          MessageBoxButton.OK,
                                          MessageBoxImage.Error);
            }

            if (isPlay)
            {
                mediaElement.Pause();
                play.Content = FindResource("Play");
                isPlay = false;
            }
            else
            {
                mediaElement.Play();
                play.Content = FindResource("Stop");
                isPlay = true;
            }
        }

        void _duration_Tick(object sender, EventArgs e)
        {
            if (mediaElement.Source != null)
            {
                slider.Minimum = 0;
                TimeSpan ts;
                GetDuration(current.PathLocation, out ts);
                slider.Maximum = ts.TotalSeconds;
                slider.Value = mediaElement.Position.TotalSeconds;
                label.Content = mediaElement.Position.ToString(@"hh\:mm\:ss") + '/' + ts.ToString(@"hh\:mm\:ss");
            }
        }

        private void settings_Click(object sender, RoutedEventArgs e)
        {
            settings set = new settings();
            set.Show();
        }

        private void animation_Click(object sender, RoutedEventArgs e)
        {
            animation set = new animation();
            set.Show();
        }

        private void stop_Click_1(object sender, RoutedEventArgs e)
        {
            mediaElement.Stop();
            play.Content = FindResource("Play");
            current.IsPlaying = false;
            isPlay = false;
        }

        private void next_Click(object sender, RoutedEventArgs e)
        {
            if(currentTrack >= 0 && currentTrack < Playlist.Items.Count-1 && shufflePlay == false)
            {
                current.IsPlaying = false;
                currentTrack++;
                Console.WriteLine(Playlist.Items.Count);
                current = (Song)Playlist.Items[currentTrack];
                mediaElement.Source = new Uri(current.PathLocation);
                current.IsPlaying = true;
                mediaElement.Play();
            }
            else if (shufflePlay == true)
            {
                current.IsPlaying = false;
                currentTrack = rnd.Next(Playlist.Items.Count);
                current = (Song)Playlist.Items[currentTrack];
                mediaElement.Source = new Uri(current.PathLocation);
                current.IsPlaying = true;
                mediaElement.Play();
            }
        }

        private void mediaEnded_Ended(object sender, RoutedEventArgs e)
        {
            if(repeatPlay == true)
            {
                mediaElement.Source = new Uri(current.PathLocation);
                current.IsPlaying = true;
                mediaElement.Play();
            }
            else if(shufflePlay == true)
            {
                current.IsPlaying = false;
                currentTrack = rnd.Next(Playlist.Items.Count);
                current = (Song)Playlist.Items[currentTrack];
                mediaElement.Source = new Uri(current.PathLocation);
                current.IsPlaying = true;
                mediaElement.Play();
            }
            else
            {
                if (currentTrack >= 0 && currentTrack < Playlist.Items.Count - 1 && shufflePlay == false)
                {
                    current.IsPlaying = false;
                    currentTrack++;
                    Console.WriteLine(Playlist.Items.Count);
                    current = (Song)Playlist.Items[currentTrack];
                    mediaElement.Source = new Uri(current.PathLocation);
                    current.IsPlaying = true;
                    mediaElement.Play();
                }
                else
                {
                    current.IsPlaying = false;
                    currentTrack = 0;
                    current = (Song)Playlist.Items[currentTrack];
                    mediaElement.Source = new Uri(current.PathLocation);
                    current.IsPlaying = true;
                    mediaElement.Play();
                }
            }
        }

        private void prev_Click(object sender, RoutedEventArgs e)
        {
            if (currentTrack > 0 && currentTrack < Playlist.Items.Count+1 && shufflePlay == false)
            {
                current.IsPlaying = false;
                currentTrack--;
                Console.WriteLine(Playlist.Items.Count);
                current = (Song)Playlist.Items[currentTrack];
                mediaElement.Source = new Uri(current.PathLocation);
                current.IsPlaying = true;
                mediaElement.Play();
            }
            else if(shufflePlay == true)
            {
                current.IsPlaying = false;
                currentTrack = rnd.Next(Playlist.Items.Count);
                current = (Song)Playlist.Items[currentTrack];
                mediaElement.Source = new Uri(current.PathLocation);
                current.IsPlaying = true;
                mediaElement.Play();
            }
        }

        private void repeat_Click(object sender, RoutedEventArgs e)
        {
            if (repeatPlay == false)
            {
                repeatPlay = true;
                repeat.Background = new SolidColorBrush(Color.FromRgb(205, 205, 205));
            }
            else
            {
                repeatPlay = false;
                repeat.Background = new SolidColorBrush(Color.FromRgb(46, 46, 46));
            }
        }

        private void shuffle_Click(object sender, RoutedEventArgs e)
        {
            if(shufflePlay == false)
            {
                shufflePlay = true;
                shuffle.Background = new SolidColorBrush(Color.FromRgb(205, 205, 205));
            }
            else
            {
                shufflePlay = false;
                shuffle.Background = new SolidColorBrush(Color.FromRgb(46, 46, 46));
            }
        }

        private void addfile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fdlg = new OpenFileDialog();
            fdlg.Multiselect = true;
            fdlg.Filter = "Media files (*.mp3;*.mpg;*.mpeg)|*.mp3;*.mpg;*.mpeg|All files (*.*)|*.*";
            if (fdlg.ShowDialog() == true)
            {
                int i = 0;
                string name;
                foreach (string item in fdlg.FileNames)
                {
                    TimeSpan duration;
                    GetDuration(@item, out duration);
                    name = System.IO.Path.GetFileNameWithoutExtension(item);

                    Song lvid = new Song()
                    {
                        ImageSource = "",
                        Title = name,
                        xmlID = ID.ToString(),
                        PathLocation = item,
                        artist = "unknown",
                        desc = "unknown",
                        duration = duration.ToString()
                    };
                    ListViewItemsCollections.Add(lvid);
                    dc.Add(lvid, ID);

                    i++;
                    ID++;
                }
            }
            Playlist.ItemsSource = ListViewItemsCollections;
        }


        protected void stackpanel_LostFocus(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
        }

        private void properties_Click(object sender, RoutedEventArgs e)
        {
            if (Playlist.SelectedItems.Count == 1)
            {
                selected = (Song)Playlist.SelectedItem;
                properties prop = new properties();
                prop.Show();
            }
            else
            {
                MessageBoxResult invalid = MessageBox.Show("Izberite eno datoteko.",
                                          "Napaka",
                                          MessageBoxButton.OK,
                                          MessageBoxImage.Error);
            }
        }

        private void window_closed(object sender, System.ComponentModel.CancelEventArgs e)
        {
            current.IsPlaying = false;
            serialize();
        }

        private void serialize()
        {
            List<Song> tmp = new List<Song>();
            foreach (var x in dc.songs.Values)
            {
                tmp.Add(x);
            }

            PlaylistToExport export = new PlaylistToExport(tmp, "playlist1");
            export.id = ID;
            XmlSerializer serializer = new XmlSerializer(typeof(PlaylistToExport));
            TextWriter writer = new StreamWriter(@"..\..\podatki.xml");
            serializer.Serialize(writer, export);
            writer.Close();
        }

        private void Window_GotFocus(object sender, RoutedEventArgs e)
        {
            Playlist.ItemsSource = ListViewItemsCollections;
        }
    }
}