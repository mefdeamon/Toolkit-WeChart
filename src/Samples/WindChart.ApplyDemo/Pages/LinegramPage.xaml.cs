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

namespace WindChart.ApplyDemo.Pages
{
    /// <summary>
    /// Interaction logic for LinegramPage.xaml
    /// </summary>
    public partial class LinegramPage : Page
    {
        public LinegramPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 模拟状态
        /// </summary>
        bool IsSimulated = false;

        /// <summary>
        /// 开始/停止模拟
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Simulate_Click(object sender, RoutedEventArgs e)
        {
            if (IsSimulated)
            {
                IsSimulated = false;
            }
            else
            {
                IsSimulated = true;
                Task.Run(() =>
                {
                    int x = 0;
                    int y = 0;
                    while (IsSimulated)
                    {

                        this.Dispatcher.Invoke(() =>
                        {
                            line.Init();
                            line2.Init();
                            line3.Init();
                            line4.Init();
                        });

                        List<Point> points = new List<Point>();

                        while (IsSimulated)
                        {

                            if (x > 500)
                            {
                                x = 0;
                                break;
                            }

                            var ran = Random.Shared.Next(-150, 150);

                            if (ran > 0)
                            {
                                y -= Math.Abs(ran) / 10;
                            }
                            else
                            {
                                y += Math.Abs(ran) / 10;
                            }
                            var p = new Point(x, y);

                            this.Dispatcher.Invoke(() =>
                            {
                                line.DrawLine(p);
                                line2.DrawLine2(p);
                                line3.DrawLine3(p);

                            });
                            points.Add(p);
                            x++;
                           // Thread.Sleep(50);
                        }

                        this.Dispatcher.Invoke(() =>
                        {
                            line4.DrawLine(points);

                        });
                        Thread.Sleep(5000);
                    }
                });
            }


        }
    }
}
