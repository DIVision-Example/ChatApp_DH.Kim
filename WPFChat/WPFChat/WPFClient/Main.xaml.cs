using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
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

namespace WPFClient {
    /// <summary>
    /// Main.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Main : Window {
        public Main() {
            InitializeComponent();
        }

        private void btnNewWindow_Click(object sender, RoutedEventArgs e) {
            UiClient newWindow = new UiClient();
            newWindow.Show();
        }



            // return output;
    }
}