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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WindChart.Lesson13
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();


            Task.Delay(1000).ContinueWith(t =>
            {
                bar.Draw(new List<Bar>()
                {
                    new Bar (){ Fill=Brushes.Purple, Label="星期天", Value=8},
                    new Bar (){ Fill=Brushes.PaleGoldenrod , Label="星期一", Value=8.35},
                    new Bar (){ Fill=Brushes.Green, Label="星期二", Value=9.35}
                });
            });
        }
    }
}
