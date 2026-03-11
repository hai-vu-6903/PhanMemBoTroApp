using phanMemBoTroVersion2.models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using BaiHatModel = phanMemBoTroVersion2.models.BaiHat;

namespace phanMemBoTroVersion2
{
    public partial class BaiHat : UserControl
    {

        List<BaiHatModel> danhSach = new List<BaiHatModel>();
        List<LyricLine> lyrics = new List<LyricLine>();

        DispatcherTimer timer = new DispatcherTimer();

        int currentIndex = 0;
        bool isPlaying = false;

        public BaiHat()
        {
            InitializeComponent();

            this.Focus();

            LoadDanhSach();

            timer.Interval = TimeSpan.FromMilliseconds(300);
            timer.Tick += Timer_Tick;
        }



        void LoadDanhSach()
        {
            string path = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "data",
                "baihat.json");

            string json = File.ReadAllText(path);

            danhSach = JsonSerializer.Deserialize<List<BaiHatModel>>(json);

            SidebarMenu.ItemsSource = danhSach;

            SidebarMenu.SelectedIndex = 0;
        }

        void LoadBaiHat(int index)
        {
            if (index < 0 || index >= danhSach.Count) return;

            // dừng bài cũ
            ResetPlayer();

            currentIndex = index;

            var bai = danhSach[index];

            TxtTenBaiHat.Text = bai.title;
            TxtTacGia.Text = "Tác giả: " + bai.author;

            // đường dẫn nhạc
            string musicPath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                bai.fileVoice);

            Player.Source = new Uri(musicPath);

            // load lyric
            string lrcPath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                bai.fileLrc);

            lyrics = LoadLyrics(lrcPath);

            //LyricsList.ItemsSource = lyrics.Select(l => l.Text);
            LyricsList.ItemsSource = lyrics;

            // ===== RESET TIME =====
            Player.Position = TimeSpan.Zero;
            MusicProgress.Value = 0;

            TxtCurrentTime.Text = "00:00";
            TxtTotalTime.Text = "00:00";

            // reset trạng thái play
            IconPlayPause.Symbol = Wpf.Ui.Controls.SymbolRegular.Play24;
            isPlaying = false;

            if (InfoPanel.Width > 0)
            {
                var info = new AuthorInfo(this, bai);
                InfoContent.Content = info;
            }
        }

        List<LyricLine> LoadLyrics(string path)
        {
            var result = new List<LyricLine>();

            if (!File.Exists(path)) return result;

            foreach (var line in File.ReadAllLines(path))
            {
                if (!line.StartsWith("[")) continue;

                int close = line.IndexOf("]");
                if (close < 0) continue;

                string timeText = line.Substring(1, close - 1);
                string lyric = line.Substring(close + 1);

                if (TimeSpan.TryParse("00:" + timeText, out TimeSpan time))
                {
                    result.Add(new LyricLine
                    {
                        Time = time,
                        Text = lyric
                    });
                }
            }

            return result.OrderBy(l => l.Time).ToList();
        }



        private void SidebarMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadBaiHat(SidebarMenu.SelectedIndex);
        }


        private void BtnPlayPause_Click(object sender, RoutedEventArgs e)
        {
            if (!isPlaying)
            {
                Player.Play();
                timer.Start();

                IconPlayPause.Symbol = Wpf.Ui.Controls.SymbolRegular.Pause24;

                isPlaying = true;
            }
            else
            {
                Player.Pause();

                IconPlayPause.Symbol = Wpf.Ui.Controls.SymbolRegular.Play24;

                isPlaying = false;
            }
        }

        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            int next = currentIndex + 1;

            if (next >= danhSach.Count) next = 0;

            SidebarMenu.SelectedIndex = next;
        }



        private void BtnPrev_Click(object sender, RoutedEventArgs e)
        {
            int prev = currentIndex - 1;

            if (prev < 0) prev = danhSach.Count - 1;

            SidebarMenu.SelectedIndex = prev;
        }

        private void MusicProgress_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (Math.Abs(Player.Position.TotalSeconds - MusicProgress.Value) > 1)
            {
                Player.Position = TimeSpan.FromSeconds(MusicProgress.Value);
            }
        }

        void Timer_Tick(object sender, EventArgs e)
        {
            if (Player.NaturalDuration.HasTimeSpan)
            {
                var total = Player.NaturalDuration.TimeSpan;
                var current = Player.Position;

                MusicProgress.Maximum = total.TotalSeconds;
                MusicProgress.Value = current.TotalSeconds;

                TxtCurrentTime.Text = current.ToString(@"mm\:ss");
                TxtTotalTime.Text = total.ToString(@"mm\:ss");
            }

            var time = Player.Position;

            //for (int i = 0; i < lyrics.Count; i++)
            //{
            //    if (lyrics[i].Time > time) break;

            //    LyricsList.SelectedIndex = i;
            //    LyricsList.ScrollIntoView(LyricsList.SelectedItem);
            //}

            int index = lyrics.FindLastIndex(l => l.Time <= time);

            if (index >= 0 && LyricsList.SelectedIndex != index)
            {
                LyricsList.SelectedIndex = index;
                LyricsList.ScrollIntoView(LyricsList.SelectedItem);
                LyricsList.UpdateLayout();

                var item = LyricsList.ItemContainerGenerator
                            .ContainerFromIndex(LyricsList.SelectedIndex) as ListBoxItem;

                if (item != null)
                {
                    item.BringIntoView();
                }
            }
        }

        private void Player_MediaEnded(object sender, RoutedEventArgs e)
        {
            ResetPlayer();
        }

        void ResetPlayer()
        {
            Player.Stop();
            timer.Stop();

            Player.Position = TimeSpan.Zero;

            MusicProgress.Value = 0;

            TxtCurrentTime.Text = "00:00";
            TxtTotalTime.Text = "00:00";

            LyricsList.SelectedIndex = -1;

            IconPlayPause.Symbol = Wpf.Ui.Controls.SymbolRegular.Play24;

            isPlaying = false;
        }

        private void LyricsList_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (LyricsList.SelectedItem is LyricLine line)
            {
                Player.Position = line.Time;

                LyricsList.SelectedItem = line;
                LyricsList.ScrollIntoView(line);
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = (MainWindow)Application.Current.MainWindow;

            main.MainContent.Content = new MenuChinh();
        }

        private void TxtInfo_Click(object sender, RoutedEventArgs e)
        {
            var bai = danhSach[currentIndex];

            AuthorInfo info = new AuthorInfo(this, bai);

            InfoContent.Content = info;

            var sb = (System.Windows.Media.Animation.Storyboard)
                this.Resources["OpenPanel"];

            sb.Begin();
        }

        private void UserControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                BtnPlayPause_Click(null, null);
            }

            if (e.Key == Key.Escape)
            {
                BtnBack_Click(null, null);
                return;
            }
        }
    }
}