using phanMemBoTroVersion2.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
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
using System.IO;

namespace phanMemBoTroVersion2
{
    /// <summary>
    /// Interaction logic for DieuVuSinhHoat.xaml
    /// </summary>
    public partial class DieuVuSinhHoat : UserControl
    {
        List<DieuVu> danhSach = new List<DieuVu>();
        int index = 0;
        bool isPlaying = false;

        DispatcherTimer timer = new DispatcherTimer();


        public DieuVuSinhHoat()
        {
            InitializeComponent();

            this.Focus();

            timer.Interval = TimeSpan.FromMilliseconds(500);
            timer.Tick += Timer_Tick;

            LoadData();

            if (danhSach.Count > 0)
            {
                SidebarMenu.SelectedIndex = 0;
            }
        }

        void LoadData()
        {
            string json = File.ReadAllText("data/dieuvu.json");

            danhSach = JsonSerializer.Deserialize<List<DieuVu>>(json);

            SidebarMenu.ItemsSource = danhSach;

            SidebarMenu.DisplayMemberPath = "TenBai";
        }

        void LoadVideo(int i)
        {
            index = i;

            var item = danhSach[index];

            TxtTenVuDieu.Text = item.TenBai;

            VideoPlayer.Source = new Uri(item.VideoPath, UriKind.Relative);

            VideoPlayer.Play();

            IconPlayPause.Symbol = Wpf.Ui.Controls.SymbolRegular.Pause24;

            isPlaying = true;

            timer.Start();
        }

        void Timer_Tick(object sender, EventArgs e)
        {
            if (VideoPlayer.Source == null) return;

            if (VideoPlayer.NaturalDuration.HasTimeSpan)
            {
                var total = VideoPlayer.NaturalDuration.TimeSpan;
                var current = VideoPlayer.Position;

                TxtCurrentTime.Text = current.ToString(@"mm\:ss");

                MusicProgress.Value = current.TotalSeconds;
            }
        }

        private void SidebarMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SidebarMenu.SelectedItem is DieuVu item)
            {
                index = SidebarMenu.SelectedIndex;

                TxtTenVuDieu.Text = item.TenBai;

                VideoPlayer.Source = new Uri(item.VideoPath, UriKind.Relative);

                VideoPlayer.Stop();

                // reset thời gian
                TxtCurrentTime.Text = "00:00";
                TxtTotalTime.Text = "00:00";

                // reset slider
                MusicProgress.Value = 0;
                MusicProgress.Maximum = 100;

                IconPlayPause.Symbol = Wpf.Ui.Controls.SymbolRegular.Play24;

                isPlaying = false;

                timer.Stop();

                Keyboard.Focus(this);
            }
        }

        private void BtnPlayPause_Click(object sender, RoutedEventArgs e)
        {
            if (VideoPlayer.Source == null) return;

            if (isPlaying)
            {
                VideoPlayer.Pause();
                IconPlayPause.Symbol = Wpf.Ui.Controls.SymbolRegular.Play24;
                timer.Stop();
            }
            else
            {
                VideoPlayer.Play();
                IconPlayPause.Symbol = Wpf.Ui.Controls.SymbolRegular.Pause24;
                timer.Start();
            }

            isPlaying = !isPlaying;
        }

        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            index++;

            if (index >= danhSach.Count)
                index = 0;

            SidebarMenu.SelectedIndex = index;
        }

        private void BtnPrev_Click(object sender, RoutedEventArgs e)
        {
            index--;

            if (index < 0)
                index = danhSach.Count - 1;

            SidebarMenu.SelectedIndex = index;
        }

        private void MusicProgress_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (VideoPlayer.NaturalDuration.HasTimeSpan)
            {
                VideoPlayer.Position = TimeSpan.FromSeconds(MusicProgress.Value);
            }
        }

        private void VideoPlayer_MediaOpened(object sender, RoutedEventArgs e)
        {
            if (VideoPlayer.NaturalDuration.HasTimeSpan)
            {
                var total = VideoPlayer.NaturalDuration.TimeSpan;

                TxtTotalTime.Text = total.ToString(@"mm\:ss");

                MusicProgress.Minimum = 0;
                MusicProgress.Maximum = total.TotalSeconds;
            }
        }

        private void VideoPlayer_MediaEnded(object sender, RoutedEventArgs e)
        {
            timer.Stop();

            VideoPlayer.Stop();

            VideoPlayer.Position = TimeSpan.Zero;

            TxtCurrentTime.Text = "00:00";

            MusicProgress.Value = 0;

            IconPlayPause.Symbol = Wpf.Ui.Controls.SymbolRegular.Play24;

            isPlaying = false;
        }


        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = (MainWindow)Application.Current.MainWindow;

            main.MainContent.Content = new MenuChinh();
        }

        private void UserControl_KeyDown(object sender, KeyEventArgs e)
        {
            
            if (VideoPlayer.Source == null) return;

            if (!VideoPlayer.NaturalDuration.HasTimeSpan) return;

            if (e.Key == Key.Right)
            {
                VideoPlayer.Position += TimeSpan.FromSeconds(5);
            }

            if (e.Key == Key.Left)
            {
                if (VideoPlayer.Position.TotalSeconds > 5)
                    VideoPlayer.Position -= TimeSpan.FromSeconds(5);
                else
                    VideoPlayer.Position = TimeSpan.Zero;
            }

            if (e.Key == Key.Space)
            {
                BtnPlayPause_Click(null, null);
                return;
            }

        }
    }
}
