using MeiMvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace WindChart.ApplyDemo.PageModels
{
    /// <summary>
    /// 单线图示例页面数据模型
    /// </summary>
    public class LinegramPageModel : NotifyPropertyChanged
    {
        private readonly Random random = new Random();

        public LinegramPageModel()
        {
            LinePoints = new ObservableCollection<Point>();
            FillBrush = FillBrushes[6];

            // 初始化
            double x = 0;
            double y = 0;
            List<Point> list = new List<Point>();
            while (x < 100)
            {
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
            LinePoints = new ObservableCollection<Point>(list);
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

        private Boolean isAxisFollowData = true;
        /// <summary>
        /// 界面是否跟随数据（保留所有数据）
        /// </summary>
        public Boolean IsAxisFollowData
        {
            get { return isAxisFollowData; }
            set { Set(ref isAxisFollowData, value); }
        }

        private Boolean isFlashRange = false;
        /// <summary>
        /// 是否刷新最新最大范围<see cref="FlashRangePointCount"/>
        /// </summary>
        public Boolean IsFlashRange
        {
            get { return isFlashRange; }
            set { Set(ref isFlashRange, value); }
        }

        private int flashRangePointCount = 200;
        /// <summary>
        /// 刷新范围点个数
        /// </summary>
        public int FlashRangePointCount
        {
            get { return flashRangePointCount; }
            set { Set(ref flashRangePointCount, value); }
        }



        private Boolean isGraph = true;
        /// <summary>
        /// 是否是面积图
        /// </summary>
        public Boolean IsGraph
        {
            get { return isGraph; }
            set { Set(ref isGraph, value); }
        }

        private Brush lineBrush = Brushes.Black;
        /// <summary>
        /// 线颜色
        /// </summary>
        public Brush LineBrush
        {
            get { return lineBrush; }
            set { Set(ref lineBrush, value); }
        }

        private double lineThinkness = 1;
        /// <summary>
        /// 线宽
        /// </summary>
        public double LineThinkness
        {
            get { return lineThinkness; }
            set { Set(ref lineThinkness, value); }
        }

        private Brush fillBrush = Brushes.DarkSeaGreen;
        /// <summary>
        /// 填充颜色
        /// </summary>
        public Brush FillBrush
        {
            get { return fillBrush; }
            set { Set(ref fillBrush, value); }
        }
        /// <summary>
        /// 面积区域填充颜色选择项
        /// </summary>
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
        /// <summary>
        /// 线点数据
        /// </summary>
        public ObservableCollection<Point> LinePoints
        {
            get { return linePoints; }
            set { Set(ref linePoints, value); }
        }

        private bool isSimulating = false;
        /// <summary>
        /// 是否处于模拟状态
        /// </summary>
        public bool IsSimulating
        {
            get { return isSimulating; }
            set
            {
                Set(ref isSimulating, value);
                RaisePropertyChanged(nameof(CanSimulate));
            }
        }
        /// <summary>
        /// 是否可以再次模拟
        /// </summary>
        public bool CanSimulate => !IsSimulating;

        private int sleepTime = 30;
        /// <summary>
        /// 模拟，点与点绘制间隔时间
        /// 单位毫秒
        /// </summary>
        public int SleepTime
        {
            get { return sleepTime; }
            set { Set(ref sleepTime, value); }
        }

        /// <summary>
        /// 数据模拟
        /// </summary>
        public RelayCommand SimulateCommand => new RelayCommand(Simulate);

        /// <summary>
        /// 数据模拟
        /// </summary>
        private void Simulate()
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
                    if (SleepTime > 0)
                    {
                        LinePoints.Add(p);

                        // 延迟效果
                        Thread.Sleep(SleepTime);
                    }

                    x++;
                }

                IsSimulating = false;
            });
        }
    }
}
