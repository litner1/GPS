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

namespace GPS
{
    /// <summary>
    /// Interaction logic for Start.xaml
    /// </summary>
    public partial class Start : Window
    {
        public Start()
        {
            InitializeComponent();
            starting_image.Source = MainWindow.CreateBitmapSourceFromBitmap(Properties.Resources.StartWindow);

        }

        private void Window_Closed(object sender, EventArgs e)
        {
            MessageBox.Show("Kliknij na skrzyżowanie, pierwsze kliknięcie w skrzyżowanie stanie się punktem startowym, a drugie punktem końcowym.");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
