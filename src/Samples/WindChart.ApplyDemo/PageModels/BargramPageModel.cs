using MeiMvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;

namespace WindChart.ApplyDemo.PageModels
{
    internal class BargramPageModel : NotifyPropertyChanged
    {
        private readonly Random random = new Random();

        private ObservableCollection<Bar> bars = new ObservableCollection<Bar>();

        public ObservableCollection<Bar> Bars
        {
            get { return bars; }
            set { Set(ref bars, value); }
        }

        public BargramPageModel()
        {
            var lis = new List<Bar>() {
                new Bar() {Value = 8, Label= "Sunday" },
                new Bar() {Value = 7.5, Label= "Monday"},
                new Bar() {Value = 8.6, Label = "Tuesday"},
                new Bar() {Value = 8.2, Label = "Wednesday"},
                new Bar() {Value = 7.6, Label = "Thursday"},
                new Bar() {Value = 7.1, Label = "Friday"},
                new Bar() {Value = 7.1, Label = "Saturday" } };
            Bars = new ObservableCollection<Bar>(lis);
        }



        private bool isAutoInterval = true;

        public bool IsAutoInterval
        {
            get { return isAutoInterval; }
            set { Set(ref isAutoInterval, value); }
        }

        private BarDirection direction = BarDirection.Vertical;

        public BarDirection Direction
        {
            get { return direction; }
            set { Set(ref direction, value); }
        }

        private BarValueLocation valueLabelLocation = BarValueLocation.Follow;

        public BarValueLocation ValueLabelLocation
        {
            get { return valueLabelLocation; }
            set { Set(ref valueLabelLocation, value); }
        }

        private Brush valueLabelBrush = Brushes.LimeGreen;

        public Brush ValueLabelBrush
        {
            get { return valueLabelBrush; }
            set { Set(ref valueLabelBrush, value); }
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

                while (IsSimulating)
                {
                    foreach (var item in lis)
                    {
                        var baseValue = random.Next(6, 9);
                        item.Value = Math.Round(baseValue % 2 == 0 ? baseValue + random.NextDouble() : baseValue - random.NextDouble(), 1);

                        if (item.Value < 7.5)
                        {
                            item.Fill = Brushes.OrangeRed;
                        }
                        else
                        {
                            item.Fill = Brushes.CornflowerBlue;
                        }
                    }

                    if (SleepTime > 0)
                    {
                        // 延迟效果
                        Thread.Sleep(SleepTime);
                    }

                    Bars = new ObservableCollection<Bar>(lis);
                }

                IsSimulating = false;
            });
        }
    }
}
