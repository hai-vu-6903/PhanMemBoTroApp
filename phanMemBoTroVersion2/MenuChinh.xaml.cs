using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace phanMemBoTroVersion2
{
    /// <summary>
    /// Interaction logic for MenuChinh.xaml
    /// </summary>
    public partial class MenuChinh : UserControl
    {
        public MenuChinh()
        {
            InitializeComponent();
        }

        private void BtnBaiHat_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = (MainWindow)Application.Current.MainWindow;

            main.MainContent.Content = new BaiHat();
        }

        private void BtnVuSinhHoat_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = (MainWindow)Application.Current.MainWindow;

            main.MainContent.Content = new DieuVuSinhHoat();
        }

        private void BtnTracNghiem_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = (MainWindow)Application.Current.MainWindow;

            main.MainContent.Content = new TracNghiem();
        }

        private void BtnBaoTang_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string url = "https://vr360.yoolife.vn/bao-tang-lich-su-quan-su-viet-nam-zmuseumc118u26724";

                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true // bắt buộc mở trình duyệt mặc định
                };

                Process.Start(psi);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Không thể mở web: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
