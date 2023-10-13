using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace WindChart.Lesson14
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContext = new MainWindowModel();

            //// Y轴范围
            //bar.YMin = 0;
            //bar.YMax = 24;

            //bar.ValueLabelLocation = BarValueLocation.None;

            //// 实时数据模拟
            //Task.Run(() =>
            //{
            //    Random rand = new Random();
            //    double y = 10;
            //    List<Bar> bars = new List<Bar>();
            //    DateTime dt = DateTime.Now.AddMonths(-2);
            //    while (true)
            //    {
            //        y = rand.Next(5, 16);

            //        this.Dispatcher.Invoke(() =>
            //        {
            //            bars.Add(new Bar() { Fill = y > 8 ? Brushes.CornflowerBlue : Brushes.OrangeRed, Label = dt.Day.ToString(), Value = y });
            //            bar.DrawBar(bars);
            //        });
            //        dt = dt.AddDays(1);
            //        if (dt > DateTime.Now)
            //        {
            //            break;
            //        }
            //    }
            //});
        }
    }


    public class MainWindowModel : ObservableObject
    {
        public MainWindowModel()
        {

            // 实时数据模拟
            Random rand = new Random();
            double y = 10;
            List<Bar> bars = new List<Bar>();
            DateTime dt = DateTime.Now.AddMonths(-2);
            while (true)
            {
                y = rand.Next(5, 16);

                bars.Add(new Bar() { Fill = y > 8 ? Brushes.CornflowerBlue : Brushes.OrangeRed, Label = dt.Day.ToString(), Value = y });
                dt = dt.AddDays(1);
                if (dt > DateTime.Now)
                {
                    break;
                }
            }

            Bars = new ObservableCollection<Bar>(bars);
        }


        public ObservableCollection<Bar> Bars { get; set; } 

    }

}
