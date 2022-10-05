using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

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
    }

    public class LinegramPageModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void RaisePropertyChanged(string PropertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }
        protected bool Set<T>(ref T Field, T Value, [CallerMemberName] string PropertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(Field, Value))
                return false;

            Field = Value;

            RaisePropertyChanged(PropertyName);

            return true;
        }

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

    // Remark：
    //     If you are using this class in WPF4.5 or above, you need to use the GalaSoft.MvvmLight.CommandWpf
    //     namespace (instead of GalaSoft.MvvmLight.Command). This will enable (or restore)
    //     the CommandManager class which handles automatic enabling/disabling of controls
    //     based on the CanExecute delegate.
    /// <summary>
    ///  A generic command whose sole purpose is to relay its functionality to other objects by invoking delegates. 
    ///  The default return value for the CanExecute method is 'true'. 
    ///  This class allows you to accept command parameters in the Execute and CanExecute callback methods.
    /// </summary>
    /// <typeparam name="T">The type of the command parameter.</typeparam>
    public class RelayCommand<T> : ICommand
    {
        /// <summary>
        /// The execution logic. 
        /// </summary>
        private Action<T> mExecute;

        /// <summary>
        /// The execution status logic. 
        /// </summary>
        private Func<T, bool> mCanExecute;

        /// <summary>
        /// Initializes a new instance of the RelayCommand class that can always execute.
        /// 
        /// Exception:
        ///     T:System.ArgumentNullException:
        ///         If the execute argument is null.
        /// </summary>
        /// <param name="execute">The execution logic. IMPORTANT: If the action causes a closure, you must set keepTargetAlive to true to avoid side effects.</param>
        /// <param name="keepTargetAlive">If true, the target of the Action will be kept as a hard reference, which might cause a memory leak. You should only set this parameter to true if the action is causing a closure. See http://galasoft.ch/s/mvvmweakaction.</param>
        public RelayCommand(Action<T> execute, bool keepTargetAlive = false)
        {
            // TODO: Check the T Type

            this.mExecute = execute;
        }

        /// <summary>
        /// Initializes a new instance of the RelayCommand class.
        /// 
        /// Exception:
        ///     T:System.ArgumentNullException:
        ///         If the execute argument is null.
        /// </summary>
        /// <param name="execute">The execution logic. IMPORTANT: If the action causes a closure, you must set keepTargetAlive to true to avoid side effects.</param>
        /// <param name="canExecute">The execution status logic. IMPORTANT: If the func causes a closure, you must set keepTargetAlive to true to avoid side effects.</param>
        /// <param name="keepTargetAlive">If true, the target of the Action will be kept as a hard reference, which might cause a memory leak. You should only set this parameter to true if the action is causing a closure.</param>
        public RelayCommand(Action<T> execute, Func<T, bool> canExecute, bool keepTargetAlive = false)
        {
            // TODO: Check the T Type 

            this.mExecute = execute;
            this.mCanExecute = canExecute;
        }

        /// <summary>
        ///  Occurs when changes occur that affect whether the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged = (sender, e) => { };

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed,this object can be set to a null reference</param>
        /// <returns>true if this command can be executed; otherwise, false.</returns>
        public bool CanExecute(object parameter)
        {
            if (mCanExecute != null)
                return mCanExecute.Invoke((T)parameter);
            else
                return true;
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed,this object can be set to a null reference</param>
        public virtual void Execute(object parameter)
        {
            mExecute.Invoke((T)parameter);
        }

        /// <summary>
        /// Raises the CanExecuteChanged event.
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged(this, null);
        }
    }

    // Remark：
    //     If you are using this class in WPF4.5 or above, you need to use the GalaSoft.MvvmLight.CommandWpf
    //     namespace (instead of GalaSoft.MvvmLight.Command). This will enable (or restore)
    //     the CommandManager class which handles automatic enabling/disabling of controls
    //     based on the CanExecute delegate.
    /// <summary>
    /// A command whose sole purpose is to relay its functionality to other objects by invoking delegates. 
    /// The default return value for the CanExecute method is 'true'.
    /// This class does not allow you to accept command parameters in the Execute and CanExecute callback methods.
    /// </summary>
    public class RelayCommand : ICommand
    {

        /// <summary>
        /// The execution logic. 
        /// </summary>
        private Action mExecute;

        /// <summary>
        /// The execution status logic. 
        /// </summary>
        private Func<bool> mCanExecute;

        /// <summary>
        /// Initializes a new instance of the RelayCommand class that can always execute.
        /// 
        /// Exception:
        ///     T:System.ArgumentNullException:
        ///         If the execute argument is null.
        /// </summary>
        /// <param name="execute">The execution logic. IMPORTANT: If the action causes a closure, you must set keepTargetAlive to true to avoid side effects.</param>
        /// <param name="keepTargetAlive">If true, the target of the Action will be kept as a hard reference, which might cause a memory leak. You should only set this parameter to true if the action is causing a closure.</param>
        public RelayCommand(Action execute, bool keepTargetAlive = false)
        {
            this.mExecute = execute;
        }

        /// <summary>
        /// Initializes a new instance of the RelayCommand class.
        /// 
        /// Exception:
        ///     T:System.ArgumentNullException:
        ///         If the execute argument is null.
        /// </summary>
        /// <param name="execute">The execution logic. IMPORTANT: If the action causes a closure, you must set keepTargetAlive to true to avoid side effects.</param>
        /// <param name="canExecute">The execution status logic. IMPORTANT: If the func causes a closure, you must set keepTargetAlive to true to avoid side effects.</param>
        /// <param name="keepTargetAlive">If true, the target of the Action will be kept as a hard reference, which might cause a memory leak. You should only set this parameter to true if the action is causing a closures.</param>
        public RelayCommand(Action execute, Func<bool> canExecute, bool keepTargetAlive = false)
        {
            this.mExecute = execute;
            this.mCanExecute = canExecute;
        }

        /// <summary>
        /// Occurs when changes occur that affect whether the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged = (sender, e) => { };

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter">This parameter will always be ignored.</param>
        /// <returns>true if this command can be executed; otherwise, false.</returns>
        public bool CanExecute(object parameter)
        {
            if (mCanExecute != null)
                return this.mCanExecute.Invoke();
            else
                return true;
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">This parameter will always be ignored.</param>
        public virtual void Execute(object parameter)
        {
            this.mExecute.Invoke();
        }

        /// <summary>
        /// Raises the CanExecuteChanged event.
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged.Invoke(this, null);
        }
    }
}
