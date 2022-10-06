using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;

namespace WindChart.ApplyDemo.PageModels
{
    /// <summary>
    /// 欢迎页面数据模型
    /// </summary>
    internal class WelcomePageModel : MeiMvvm.NotifyPropertyChanged
    {
        private readonly Random random = new Random();

        /// <summary>
        /// 单线图的点
        /// </summary>
        public ObservableCollection<Point> LinePoints { get; set; } = new ObservableCollection<Point>();

        public WelcomePageModel()
        {
            Task.Delay(10).ContinueWith(t =>
            {
                double x = 0;
                double y = 0;
                List<Point> list = new List<Point>();
                while (true)
                {
                    if (x > 500)
                    {
                        LinePoints = new ObservableCollection<Point>(list);
                        break;
                    }

                    var ran = random.Next(-150, 150);

                    if (ran > 0)
                    {
                        y -= Math.Abs(ran) / 10;
                    }
                    else
                    {
                        y += Math.Abs(ran) / 10;
                    }
                    var p = new Point(x, y);

                    list.Add(p);

                    x++;
                }
            });
        }
    }
}
