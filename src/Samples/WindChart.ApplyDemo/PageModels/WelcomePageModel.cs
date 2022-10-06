using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WindChart.ApplyDemo.PageModels
{
    internal class WelcomePageModel : MeiMvvm.NotifyPropertyChanged
    {
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

                    list.Add(p);

                    //x += Random.Shared.NextDouble();
                    x++;
                }
            });
        }
    }
}
