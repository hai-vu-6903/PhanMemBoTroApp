using Newtonsoft.Json;
using phanMemBoTroVersion2.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace phanMemBoTroVersion2
{
    public partial class TracNghiem : UserControl
    {
        List<ExamContainer> exams;
        List<Question> currentQuestions;

        int currentIndex = 0;
        int correctCount = 0;
        bool answered = false;

        public TracNghiem()
        {
            InitializeComponent();
            LoadQuestions();
        }

        void LoadQuestions()
        {
            string path = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Data",
                "questions.json"
            );

            if (!File.Exists(path))
            {
                MessageBox.Show("Không tìm thấy file: " + path);
                return;
            }

            string json = File.ReadAllText(path);

            var root = JsonConvert.DeserializeObject<List<Root>>(json);

            exams = root[0].exams;

            SidebarMenu.ItemsSource = exams;
            SidebarMenu.DisplayMemberPath = "title";

            SidebarMenu.SelectedIndex = 0;
        }

        private void SidebarMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SidebarMenu.SelectedItem == null) return;

            var exam = SidebarMenu.SelectedItem as ExamContainer;

            currentQuestions = exam.questions;

            foreach (var q in currentQuestions)
                q.selected = -1;

            currentIndex = 0;
            correctCount = 0;

            ResultPanel.Visibility = Visibility.Collapsed;

            BuildNavigator();
            ShowQuestion();
        }

        void ShowQuestion()
        {
            answered = false;

            var q = currentQuestions[currentIndex];

            TxtQuestion.Text = q.question;

            BtnA.Content = q.options[0];
            BtnB.Content = q.options[1];
            BtnC.Content = q.options[2];

            if (q.options.Count > 3)
            {
                BtnD.Content = q.options[3];
                BtnD.Visibility = Visibility.Visible;
            }
            else
            {
                BtnD.Visibility = Visibility.Collapsed;
            }

            ResetButtons();

            TxtProgress.Text = $"Câu {currentIndex + 1} / {currentQuestions.Count}";

            QuizProgress.Maximum = currentQuestions.Count;
            QuizProgress.Value = currentIndex + 1;

            for (int i = 0; i < QuestionNavigator.Children.Count; i++)
            {
                var b = QuestionNavigator.Children[i] as Button;

                if (i == currentIndex)
                    b.BorderBrush = Brushes.DarkGreen;
                else
                    b.BorderBrush = Brushes.Transparent;
            }

            UpdateProgressBar();
            UpdateScore();
            AnimateQuestion();

            var selected = q.selected;

            if (selected != -1)
            {
                answered = true;

                if (selected == q.answer)
                {
                    GetButton(selected).Background = correctColor;
                }
                else
                {
                    GetButton(selected).Background = wrongColor;
                    HighlightCorrect(q.answer);
                }
            }

            Button GetButton(int index)
            {
                if (index == 0) return BtnA;
                if (index == 1) return BtnB;
                if (index == 2) return BtnC;
                return BtnD;
            }
        }

        void ResetButtons()
        {
            BtnA.Background = Brushes.White;
            BtnB.Background = Brushes.White;
            BtnC.Background = Brushes.White;
            BtnD.Background = Brushes.White;
        }

        private void Answer_Click(object sender, RoutedEventArgs e)
        {
            if (answered) return;

            answered = true;

            Button btn = sender as Button;

            int selected = int.Parse(btn.Tag.ToString());

            var q = currentQuestions[currentIndex];

            if (q.selected == -1)
            {
                currentQuestions[currentIndex].selected = selected;

                if (selected == q.answer)
                    correctCount++;
            }

            var navBtn = QuestionNavigator.Children[currentIndex] as Button;

            if (selected == q.answer)
            {
                btn.Background = Brushes.LightGreen;
                navBtn.Background = Brushes.LightGreen;
                //correctCount++;
            }
            else
            {
                btn.Background = Brushes.IndianRed;
                navBtn.Background = Brushes.IndianRed;

                HighlightCorrect(q.answer);
            }

            UpdateScore();
        }

        void HighlightCorrect(int answer)
        {
            if (answer == 0) BtnA.Background = Brushes.LightGreen;
            if (answer == 1) BtnB.Background = Brushes.LightGreen;
            if (answer == 2) BtnC.Background = Brushes.LightGreen;
            if (answer == 3) BtnD.Background = Brushes.LightGreen;
        }

        private void BtnPrev_Click(object sender, RoutedEventArgs e)
        {
            if (currentIndex > 0)
            {
                currentIndex--;
                ShowQuestion();
            }
        }

        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            if (!answered) return;

            currentIndex++;

            if (currentIndex >= currentQuestions.Count)
            {
                ShowResult();
                return;
            }

            ShowQuestion();
        }

        private void BtnRestart_Click(object sender, RoutedEventArgs e)
        {
            foreach (var q in currentQuestions)
            {
                q.selected = -1;
            }

            correctCount = 0;
            currentIndex = 0;

            ResultPanel.Visibility = Visibility.Collapsed;

            BuildNavigator();
            ShowQuestion();
        }

        void ShowResult()
        {
            int correct = 0;

            foreach (var q in currentQuestions)
            {
                if (q.selected == q.answer)
                    correct++;
            }

            TxtScore.Text = $"Bạn đúng {correct} / {currentQuestions.Count} câu";
            ResultPanel.Visibility = Visibility.Visible;

        }

        private void BtnFinish_Click(object sender, RoutedEventArgs e)
        {
            ResultPanel.Visibility = Visibility.Collapsed;
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = (MainWindow)Application.Current.MainWindow;

            main.MainContent.Content = new MenuChinh();
        }
        void BuildNavigator()
        {
            QuestionNavigator.Children.Clear();

            for (int i = 0; i < currentQuestions.Count; i++)
            {
                Button btn = new Button();

                btn.Content = i + 1;
                //btn.Width = 35;
                //btn.Height = 35;
                btn.HorizontalAlignment = HorizontalAlignment.Stretch;
                btn.VerticalAlignment = VerticalAlignment.Stretch;
                btn.Margin = new Thickness(5);

                btn.Background = Brushes.LightGray;
                btn.BorderBrush = Brushes.Transparent;

                int index = i;

                btn.Click += (s, e) =>
                {
                    currentIndex = index;
                    ShowQuestion();
                };

                QuestionNavigator.Children.Add(btn);
            }
        }

        private void BtnNextExam_Click(object sender, RoutedEventArgs e)
        {
            if (SidebarMenu.SelectedIndex < exams.Count - 1)
                SidebarMenu.SelectedIndex++;
        }

        Brush correctColor = new SolidColorBrush(Color.FromRgb(120, 200, 120));
        Brush wrongColor = new SolidColorBrush(Color.FromRgb(220, 120, 120));
        Brush defaultColor = new SolidColorBrush(Color.FromRgb(240, 240, 240));

        void UpdateScore()
        {
            int correct = 0;
            int answered = 0;

            foreach (var q in currentQuestions)
            {
                if (q.selected != -1)
                {
                    answered++;

                    if (q.selected == q.answer)
                        correct++;
                }
            }

            double percent = (double)correct / currentQuestions.Count * 100;

            TxtPercent.Text = $"Điểm: {correct}/{currentQuestions.Count} ({percent:0}%)";
        }

        void UpdateProgressBar()
        {
            double percent = (double)(currentIndex + 1) / currentQuestions.Count;

            double width = percent * 250;

            DoubleAnimation anim = new DoubleAnimation
            {
                To = width,
                Duration = TimeSpan.FromMilliseconds(300)
            };

            ProgressFill.BeginAnimation(WidthProperty, anim);
        }

        void AnimateQuestion()
        {
            DoubleAnimation fade = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromMilliseconds(250)
            };

            TxtQuestion.BeginAnimation(OpacityProperty, fade);
        }

        public class GridLengthAnimation : AnimationTimeline
        {
            public override Type TargetPropertyType => typeof(GridLength);

            public GridLength From { get; set; }
            public GridLength To { get; set; }

            public override object GetCurrentValue(object defaultOriginValue, object defaultDestinationValue, AnimationClock clock)
            {
                double fromVal = From.Value;
                double toVal = To.Value;

                double progress = clock.CurrentProgress.Value;

                double value = fromVal + (toVal - fromVal) * progress;

                return new GridLength(value);
            }

            protected override Freezable CreateInstanceCore()
            {
                return new GridLengthAnimation();
            }
        }
    }
}