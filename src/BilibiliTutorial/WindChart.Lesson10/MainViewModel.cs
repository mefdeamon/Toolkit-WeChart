using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace WindChart.Lesson10
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public MainViewModel()
        {
            LinePoints = new ObservableCollection<Point>();


            // 模拟
            Task.Delay(1000).ContinueWith((Action<Task>)(t =>
            {

                Application.Current.Dispatcher.Invoke(() =>
                {
                    LinePoints = new ObservableCollection<Point>();
                });

                double x = 0;
                double y = 0;

                Random random = new Random();

                y = 50;


                //  while (true)
                {
                    List<Point> points = new List<Point>();
                    while (true)
                    {

                        Application.Current.Dispatcher.Invoke((Action)(() =>
                        {
                            points.Add(new Point(x, y));
                        }));

                        x++;
                        y = random.Next() % 2 == 0 ? ++y : --y;

                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            LinePoints = new ObservableCollection<Point>(points);
                        });

                        Thread.Sleep(20);

                    }
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        LinePoints = new ObservableCollection<Point>(points);
                        x = 0;
                        y = 50;
                    });

                    Thread.Sleep(2000);
                }

            }));



        }

        private Boolean isGraph;

        public Boolean IsGraph
        {
            get { return isGraph; }
            set
            {
                isGraph = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsGraph)));
            }
        }

        private ObservableCollection<Point> linePoints;

        public ObservableCollection<Point> LinePoints
        {
            get { return linePoints; }
            set
            {
                linePoints = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LinePoints)));
            }
        }


    }

}
