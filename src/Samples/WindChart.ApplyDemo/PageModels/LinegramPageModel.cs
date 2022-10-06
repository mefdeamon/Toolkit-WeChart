using MeiMvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace WindChart.ApplyDemo.PageModels
{

    public class LinegramPageModel : NotifyPropertyChanged
    {
        public LinegramPageModel()
        {
            LinePoints = new ObservableCollection<Point>();
            SimulateCommand?.Execute(null);
        }

        private Boolean needAiming = true;
        /// <summary>
        /// 显示瞄点
        /// </summary>
        public Boolean NeedAiming
        {
            get { return needAiming; }
            set { Set(ref needAiming, value); }
        }

        private Boolean keepAllPoints = true;

        public Boolean KeepAllPoints
        {
            get { return keepAllPoints; }
            set { Set(ref keepAllPoints, value); }
        }

        private Boolean isGraph = true;

        public Boolean IsGraph
        {
            get { return isGraph; }
            set { Set(ref isGraph, value); }
        }

        private Brush lineBrush = Brushes.Black;

        public Brush LineBrush
        {
            get { return lineBrush; }
            set { Set(ref lineBrush, value); }
        }

        private double lineThinkness = 1;

        public double LineThinkness
        {
            get { return lineThinkness; }
            set { Set(ref lineThinkness, value); }
        }

        private Brush fillBrush = Brushes.DarkSeaGreen;

        public Brush FillBrush
        {
            get { return fillBrush; }
            set { Set(ref fillBrush, value); }
        }

        public List<Brush> FillBrushes => new List<Brush>() { Brushes.Black, Brushes.OrangeRed, Brushes.DarkSeaGreen, Brushes.CornflowerBlue,
            new LinearGradientBrush() { EndPoint = new Point(0, 1),
                GradientStops=new GradientStopCollection (){new GradientStop() { Color= Colors.OrangeRed, Offset=0} ,
                new GradientStop() { Color= Color.FromArgb(0x40,0xFF,0x45,0x00), Offset=1} }},
             new LinearGradientBrush() { EndPoint = new Point(0, 1),
                GradientStops=new GradientStopCollection (){new GradientStop() { Color= Colors.DarkSeaGreen, Offset=0} ,
                new GradientStop() { Color= Color.FromArgb(0x40,0x8F,0xBC,0x8F), Offset=1} } },
             new LinearGradientBrush() { EndPoint = new Point(0, 1),
                GradientStops=new GradientStopCollection (){new GradientStop() { Color= Colors.CornflowerBlue, Offset=0} ,
                new GradientStop() { Color= Color.FromArgb(0x40,0x64,0x95,0xED), Offset=1} } }
        };

        private ObservableCollection<Point> linePoints = new ObservableCollection<Point>();

        public ObservableCollection<Point> LinePoints
        {
            get { return linePoints; }
            set { Set(ref linePoints, value); }
        }

        private bool isSimulating = false;

        public bool IsSimulating
        {
            get { return isSimulating; }
            set
            {
                Set(ref isSimulating, value);
                RaisePropertyChanged(nameof(CanSimulate));
            }
        }

        public bool CanSimulate => !IsSimulating;

        private int sleepTime = 0;
        /// <summary>
        /// 模拟，点与点绘制间隔时间
        /// 单位毫秒
        /// </summary>
        public int SleepTime
        {
            get { return sleepTime; }
            set { Set(ref sleepTime, value); }
        }

        public RelayCommand SimulateCommand => new RelayCommand(() =>
        {
            if (IsSimulating)
            {
                IsSimulating = false;
                return;
            }
            IsSimulating = true;
            LinePoints = new ObservableCollection<Point>();

            Task.Delay(10).ContinueWith(t =>
            {
                double x = 0;
                double y = 0;
                List<Point> list = new List<Point>();
                while (IsSimulating)
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
                    if (SleepTime > 0)
                    {
                        LinePoints.Add(p);

                        // 延迟效果
                        Thread.Sleep(SleepTime);
                    }

                    //x += Random.Shared.NextDouble();
                    x++;
                }

                IsSimulating = false;
            });
        });

    }
}
