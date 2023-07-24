using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

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

        private ObservableCollection<Bar> bars=new ObservableCollection<Bar> ();

        public ObservableCollection<Bar> Bars
        {
            get { return bars; }
            set { Set(ref bars, value); }
        }

        public WelcomePageModel()
        {
            Task.Delay(10).ContinueWith(t =>
            {
                double x = 0;
                double y = 50;
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

            var lis = new List<Bar>() {
                new Bar() {Value = 8, Label= "Sunday" },
                new Bar() {Value = 7.5, Label= "Monday"},
                new Bar() {Value = 8.6, Label = "Tuesday"}, 
                new Bar() {Value = 8.2, Label = "Wednesday"}, 
                new Bar() {Value = 7.6, Label = "Thursday"},
                new Bar() {Value = 7.1, Label = "Friday"},
                new Bar() {Value = 7.1, Label = "Saturday" } };
            Bars = new ObservableCollection<Bar>(lis);


            Task.Delay(10).ContinueWith(t =>
            {
                double x = 0;
                double y = 0;

                int count = 0;
                var ss = new ObservableCollection<EllipseDot>();
                while (count < 50)
                {
                    y = random.Next(-50, 50);
                    x = random.Next(0, 350);
                    var size = random.Next(1, 5);
                    ss.Add(new EllipseDot() { X = x, Y = y, Type = random.Next(0, 3), Height = size, Width = size });

                    count++;
                }
                DotSource = new ObservableCollection<EllipseDot>(ss);
            });
        }

        private ObservableCollection<EllipseDot> dotSource = new ObservableCollection<EllipseDot>();
        /// <summary>
        /// 线点数据
        /// </summary>
        public ObservableCollection<EllipseDot> DotSource
        {
            get { return dotSource; }
            set { Set(ref dotSource, value); }
        }

    }
}
