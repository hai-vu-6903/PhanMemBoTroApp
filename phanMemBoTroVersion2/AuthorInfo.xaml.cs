using phanMemBoTroVersion2.models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace phanMemBoTroVersion2
{
    public partial class AuthorInfo : UserControl
    {
        BaiHat parentPage;

        public AuthorInfo(BaiHat parent, models.BaiHat bai)
        {
            InitializeComponent();

            parentPage = parent;

            TxtAuthor.Text = bai.author;
            if (!string.IsNullOrEmpty(bai.authorImage))
            {
                ImgAuthor.Source = new BitmapImage(
                    new System.Uri(bai.authorImage, System.UriKind.Relative));
            }

            AuthorInfoList.ItemsSource = bai.authorInfo;
            SongInfoList.ItemsSource = bai.description;
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            var sb = (Storyboard)parentPage.Resources["ClosePanel"];
            sb.Begin();
        }
    }
}