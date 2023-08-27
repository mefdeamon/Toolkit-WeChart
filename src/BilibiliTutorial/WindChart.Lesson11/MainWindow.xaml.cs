using System;
using System.Collections.Generic;
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

namespace WindChart.Lesson11
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

        private void FlashEveryPoint()
        {
            // 设置X轴开始显示范围
            line.XMin = DateTime.Now.ToOADate();
            line.XMax = DateTime.Now.AddSeconds(60).ToOADate();
            // 设置X轴刻度文本格式
            line.XAxisTextFormatString = "HH: mm:ss";
            // X轴刻度按照日期显示
            line.IsXAxisTextDateTimeFormat = true;
            line.IsGraph = false;

            // 轴跟随数据变化
            line.IsAxisFollowData = true;
            // Y轴范围
            line.YMin = 0;
            line.YMax = 80;

            // 实时数据模拟
            Task.Run(() =>
            {
                Random rand = new Random();
                double y = 10;
                while (true)
                {
                    var ran = rand.Next(-150, 150);
                    if (ran > 0)
                    {
                        y -= 1;
                    }
                    else
                    {
                        y += 1;
                    }
                    this.Dispatcher.Invoke(() =>
                    {
                        line.Add(new Point(DateTime.Now.ToOADate(), y));
                    });
                    Thread.Sleep(30);
                }
            });
        }


        private void FlashOnce()
        {
            // 设置X轴开始显示范围
            DateTime dtStart = DateTime.Now.AddMinutes(-3);
            line.XMin = dtStart.ToOADate();
            line.XMax = DateTime.Now.AddSeconds(60).ToOADate();
            // 设置X轴刻度文本格式
            line.XAxisTextFormatString = "HH:mm:ss";
            // X轴刻度按照日期显示
            line.IsXAxisTextDateTimeFormat = true;

            // 轴跟随数据变化
            line.IsAxisFollowData = true;
            // Y轴范围
            line.YMin = 0;
            line.YMax = 80;

            // 数据模拟
            Random rand = new Random();
            List<Point> points = new List<Point>();
            double y = 10;
            while (dtStart < DateTime.Now)
            {
                var ran = rand.Next(-150, 150);
                if (ran > 0)
                {
                    y -= 1;
                }
                else
                {
                    y += 1;
                }
                points.Add(new Point(dtStart.ToOADate(), y));
                dtStart = dtStart.AddMilliseconds(500);
            }
            line.DrawLine(points);
        }

    }
}
