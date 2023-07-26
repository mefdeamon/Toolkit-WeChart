using MeiMvvm;
using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace WindChart.ApplyDemo.PageModels
{
    /// <summary>
    /// 单线图示例页面数据模型
    /// </summary>
    public class DotgramPageModel : NotifyPropertyChanged
    {
        private readonly Random random = new Random();

        public DotgramPageModel()
        {
            DotSource = new ObservableCollection<EllipseDot>();
            FlashFrame();
        }

        private bool isUpdatedDot;

        public bool IsUpdatedDot
        {
            get { return isUpdatedDot; }
            set { Set(ref isUpdatedDot, value); }
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

        private int sleepTime = 1000;
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
            DotSource = new ObservableCollection<EllipseDot>();

            Task.Delay(10).ContinueWith(t =>
            {
                while (IsSimulating)
                {
                    FlashFrame();
                    Thread.Sleep(SleepTime);
                }
                IsSimulating = false;
            });
        }


        private int dotCount = 40;

        public int DotCount
        {
            get { return dotCount; }
            set { Set(ref dotCount, value); }
        }

        private Double dotSize = 5;

        public Double DotSize
        {
            get { return dotSize; }
            set { Set(ref dotSize, value); }
        }

        private void FlashFrame()
        {
            double x = 0;
            double y = 0;

            int count = 0;
            var ss = new ObservableCollection<EllipseDot>();
            while (count < DotCount)
            {
                y = random.Next(-50, 50);
                x = random.Next(-50, 50);
                var size = random.Next(1, (int)DotSize);
                ss.Add(new EllipseDot() { X = x, Y = y, Type = random.Next(0, 3), Height = size, Width = size });

                count++;
            }
            DotSource = new ObservableCollection<EllipseDot>(ss);
        }

    }
}
