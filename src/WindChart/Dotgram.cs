using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace WindChart
{
    /// <summary>
    /// 点阵图
    /// </summary>
    public class Dotgram : BaseDotgram<EllipseDot>
    {
        public Dotgram() : base()
        {
            // 初始化画板刻度信息
            YAxisLineAlignment = YAxisLineAlignment.Location;
            XAxisLineAlignment = XAxisLineAlignment.Bottom;
            XMin = 0;
            XMax = 400;
            YMin = -100;
            YMax = 100;
            NeedXAxisLine = true;
            NeedXAxisText = true;

            // XMin = "0" XMax = "300" NeedXAxisText = "True" YMin = "-100" YMax = "300"

        }

        #region override

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            Draw();
        }

        public override void Draw()
        {
            var dc = dotVisual.RenderOpen();

            if (DotSource.Count > 0)
            {
                Point p0 = new Point();
                foreach (var item in DotSource)
                {
                    p0.X = XAxisConvertXToPixel(item.X);
                    p0.Y = YAxisConvertYToPixel(item.Y);

                    dc.DrawEllipse(EllipseDot.TypeBrushes[item.Type], null, p0, item.Width * xPixelRatio, item.Height * yPixelRatio);
                }
            }

            dc.Close();

            // 显示图形
            InvalidateVisual();
        }

        public override void Draw(List<EllipseDot> dots)
        {
            var dc = dotVisual.RenderOpen();

            if (dots.Count > 0)
            {
                Point p0 = new Point();
                foreach (var item in dots)
                {
                    p0.X = XAxisConvertXToPixel(item.X);
                    p0.Y = YAxisConvertYToPixel(item.Y);

                    dc.DrawEllipse(EllipseDot.TypeBrushes[item.Type], null, p0, item.Width * xPixelRatio, item.Height * yPixelRatio);
                }
            }

            dc.Close();

            // 显示图形
            InvalidateVisual();
        }

        public override void Clear()
        {
            var dc = dotVisual.RenderOpen();

            dc.Close();

            // 显示图形
            InvalidateVisual();
        }


        #endregion
    }



    public abstract class BaseDotgram<T> : Gram where T : Dot
    {
        /// <summary>
        /// 画板
        /// </summary>
        protected DrawingVisual dotVisual;

        public BaseDotgram()
        {
            dotVisual = new DrawingVisual();
            Visuals.Add(dotVisual);
        }

        public abstract void Draw();
        public abstract void Draw(List<T> dots);
        public abstract void Clear();

        /// <summary>
        /// 使用绑定的时候，直接用New
        /// </summary>
        public ObservableCollection<T> DotSource
        {
            get { return (ObservableCollection<T>)GetValue(DotSourceProperty); }
            set { SetValue(DotSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Dots.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DotSourceProperty =
            DependencyProperty.Register("DotSource", typeof(ObservableCollection<T>), typeof(BaseDotgram<T>),
                new FrameworkPropertyMetadata(new ObservableCollection<T>(), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, DotSourceChanged));

        /// <summary>
        /// 线条集合发送变化时，更新图形
        /// 只有在New的时候，再会被触发
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void DotSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is BaseDotgram<T> l)
            {
                if (e.NewValue is ObservableCollection<T> pos)
                {
                    // 对集合进行New操作，在构造函数中带有默认集合
                    if (pos.Count > 1)
                    {
                        l.Draw(pos.ToList());
                    }
                    else
                    {
                        // 对集合进行new操作时，没有参数的构造函数
                        // 清理界面
                        l.Clear();

                        // 绑定集合变化事件
                        l.DotSource.CollectionChanged += (sender, e) =>
                        {
                            switch (e.Action)
                            {
                                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                                    break;
                                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                                    break;
                                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                                    break;
                                case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
                                    break;
                                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                                    // 集合调用Clear函数时
                                    l.Clear();
                                    break;
                                default:
                                    break;
                            }
                        };
                    }
                }
            }
        }



    }


    public class Dot
    {
        public Dot() { }

        public double X { get; set; }
        public double Y { get; set; }

        public double Width { get; set; }
        public double Height { get; set; }
    }

    public class EllipseDot : Dot
    {
        public int Type { get; set; }

        public static Brush[] TypeBrushes = new Brush[]
        {
            Brushes.Red,
            Brushes.Green,
            Brushes.Blue,
            Brushes.Purple,
        };
    }

}
