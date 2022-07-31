using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WindChart.ApplyDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void NaviClick(object sender, RoutedEventArgs e)
        {
            var el = (ToggleButton)sender;
            if (el.IsChecked == true)
            {
                var att = el.Tag.ToString();
                if (att != null)
                {
                    frame.Navigate(new Uri(att, UriKind.Relative));
                }
                else
                {
                    frame.Content = null;
                }
            }
        }
    }
}
