using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace WindChart
{
    /// <summary>
    /// 单条线型图
    /// </summary>
    public class Linegram : Gram
    {
        /// <summary>
        /// 折线画板
        /// </summary>
        private DrawingVisual lineVisual;

        /// <summary>
        /// 线条颜色和粗细
        /// </summary>
        Pen linePen;

        public Linegram()
        {
            lineVisual = new DrawingVisual();
            Visuals.Add(lineVisual);

            linePen = new Pen(LineBrush, LineThickness);
            linePen.Freeze();

            // 初始化画板刻度信息
            YAxisLineMode = AxisLineMode.TopLeft;
            XAxisLineMode = AxisLineMode.BottmRight;
            XMin = 0;
            YMin = 0;
        }

        #region 场景信息配置设置

        /// <summary>
        /// 是否显示绘制所有的点
        /// </summary>
        public Boolean KeepAllPoints
        {
            get { return (Boolean)GetValue(KeepAllPointsProperty); }
            set { SetValue(KeepAllPointsProperty, value); }
        }
        public static readonly DependencyProperty KeepAllPointsProperty =
            DependencyProperty.Register("KeepAllPoints", typeof(Boolean), typeof(Linegram), new PropertyMetadata(false));

        /// <summary>
        /// 线条画刷
        /// </summary>
        public Brush LineBrush
        {
            get { return (Brush)GetValue(LineBrushProperty); }
            set { SetValue(LineBrushProperty, value); }
        }
        public static readonly DependencyProperty LineBrushProperty =
            DependencyProperty.Register("LineBrush", typeof(Brush), typeof(Linegram),
                new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (d, e) =>
                {
                    if (d is Linegram l)
                    {
                        l.linePen = new Pen((Brush)e.NewValue, l.LineThickness);
                        l.linePen.Freeze();
                        l.Draw();
                    }
                }));

        /// <summary>
        /// 线条宽度
        /// </summary>
        public double LineThickness
        {
            get { return (double)GetValue(LineThicknessProperty); }
            set { SetValue(LineThicknessProperty, value); }
        }
        public static readonly DependencyProperty LineThicknessProperty =
            DependencyProperty.Register("LineThickness", typeof(double), typeof(Linegram),
                new FrameworkPropertyMetadata(1D, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (d, e) =>
                {
                    if (d is Linegram l)
                    {
                        l.linePen = new Pen(l.LineBrush, (double)e.NewValue);
                        l.linePen.Freeze();
                        l.Draw();
                    }
                }));

        /// <summary>
        /// 填充颜色
        /// 面积区域显示时才有使用
        /// </summary>
        public Brush Fill
        {
            get { return (Brush)GetValue(FillProperty); }
            set { SetValue(FillProperty, value); }
        }
        public static readonly DependencyProperty FillProperty =
            DependencyProperty.Register("Fill", typeof(Brush), typeof(Linegram),
                new FrameworkPropertyMetadata(Brushes.CornflowerBlue, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, LinePropertyChangedCallback));


        /// <summary>
        /// 是否显示面积区域
        /// </summary>
        public Boolean IsGraph
        {
            get { return (Boolean)GetValue(IsGraphProperty); }
            set { SetValue(IsGraphProperty, value); }
        }
        public static readonly DependencyProperty IsGraphProperty =
            DependencyProperty.Register("IsGraph", typeof(Boolean), typeof(Linegram),
                new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, LinePropertyChangedCallback));

        /// <summary>
        /// 线条的状态发送变化时，重新绘制一次图形
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void LinePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Linegram l)
            {
                l.Draw();
            }
        }

        /// <summary>
        /// 显示瞄点
        /// </summary>
        public Boolean NeedAiming
        {
            get { return (Boolean)GetValue(NeedAimingProperty); }
            set { SetValue(NeedAimingProperty, value); }
        }
        public static readonly DependencyProperty NeedAimingProperty =
            DependencyProperty.Register("NeedAiming", typeof(Boolean), typeof(Linegram),
                new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, LinePropertyChangedCallback));

        #endregion

        #region 绘制曲线图，对外提供的绑定属性

        /// <summary>
        /// 线条数据信息（一下绘制完）
        /// </summary>
        public ObservableCollection<Point> LineSource
        {
            get { return (ObservableCollection<Point>)GetValue(LineSourceProperty); }
            set { SetValue(LineSourceProperty, value); }
        }
        public static readonly DependencyProperty LineSourceProperty =
            DependencyProperty.Register("LineSource", typeof(ObservableCollection<Point>), typeof(Linegram),
                                new FrameworkPropertyMetadata(new ObservableCollection<Point>(), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, LineSourceChanged));

        /// <summary>
        /// 线条集合发送变化时，更新图形
        /// 只有在New的时候，再会被触发
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void LineSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Linegram l)
            {
                if (e.NewValue is ObservableCollection<Point> pos)
                {
                    // 对集合进行New操作，在构造函数中带有默认集合
                    if (pos.Count > 1)
                    {
                        l.DrawLine(pos.ToList());
                    }
                    else
                    {
                        // 对集合进行new操作时，没有参数的构造函数
                        // 清理界面
                        l.ClearLine();

                        // 绑定集合变化事件
                        l.LineSource.CollectionChanged += (sender, e) =>
                        {
                            switch (e.Action)
                            {
                                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                                    if (e.NewItems != null && e.NewItems.Count == 1)
                                    {
                                        if (e.NewItems[0] is Point p)
                                            l.Add(p);
                                    }
                                    break;
                                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                                    break;
                                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                                    break;
                                case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
                                    break;
                                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                                    // 集合调用Clear函数时
                                    l.ClearLine();
                                    break;
                                default:
                                    break;
                            }
                        };
                    }
                }
            }
        }

        #endregion

        /// <summary>
        /// 绘制的所有点
        /// </summary>
        List<Point> points = new List<Point>();

        /// <summary>
        /// 重置坐标刻度
        /// </summary>
        /// <param name="p"></param>
        private void ResetAxis(Point p)
        {
            IsRenderAxis = false;
            bool yChanged = false, xChanged = false;
            this.Dispatcher.Invoke(() =>
            {
                // X 轴
                if (p.X > XMax)
                {
                    var changed = p.X - XMax;
                    XMax += changed;

                    if (KeepAllPoints)
                    {
                        if (points[0].X != XMin)
                        {
                            XMin = points[0].X;
                        }
                    }
                    else
                    {
                        XMin += changed;

                        if (points[0].X < XMin)
                        {
                            points.Remove(points[0]);
                            XMin = points[0].X;
                        }
                    }
                    xChanged = true;
                }
                else if (p.X < XMin)
                {
                    var changed = XMin - p.X;
                    XMax -= changed;

                    if (KeepAllPoints)
                    {
                        if (points[0].X != XMax)
                        {
                            XMax = points[0].X;
                        }
                    }
                    else
                    {
                        XMin -= changed;
                        if (points[0].X > XMax)
                        {
                            points.Remove(points[0]);
                            XMax = points[0].X;
                        }
                    }
                    xChanged = true;
                }

                // Y 轴
                if (p.Y > YMax)
                {
                    YMax = p.Y;
                    yChanged = true;
                }
                else if (p.Y < YMin)
                {
                    YMin = p.Y;
                    yChanged = true;
                }

                IsRenderAxis = true;
                UpdatePixelRatio();

                if (xChanged)
                {
                    DrawXAxisScale();
                }
                if (yChanged)
                {
                    DrawYAxisScale();
                }
            });
        }

        /// <summary>
        /// 绘制一个新点
        /// 连续调用这个方法绘制图像前
        /// </summary>
        /// <param name="p"></param>
        public void Add(Point p)
        {
            if (points.Count > 1)
            {
                ResetAxis(p);
            }
            points.Add(p);
            Draw();
        }

        /// <summary>
        /// 清理界面
        /// </summary>
        private void ClearLine()
        {
            points.Clear();
            Draw();
        }

        /// <summary>
        /// 直接一下把数据画完
        /// </summary>
        /// <param name="newPoints"></param>
        public void DrawLine(List<Point> newPoints)
        {
            points.Clear();
            points.AddRange(newPoints);

            var maxX = points.Max(t => t.X);
            var minX = points.Min(t => t.X);

            var maxY = points.Max(t => t.Y);
            var minY = points.Min(t => t.Y);

            IsRenderAxis = false;

            YMax = maxY;
            YMin = minY;

            XMax = maxX;
            XMin = minX;

            // 更新图
            UpdatePixelRatio();
            DrawXAxisScale();
            DrawYAxisScale();

            IsRenderAxis = false;

            Draw();
        }

        /// <summary>
        /// 绘制的路径
        /// </summary>
        PathGeometry path = new PathGeometry() { Figures = new PathFigureCollection() };

        /// <summary>
        /// 绘图
        /// </summary>
        private void Draw()
        {
            this.Dispatcher.Invoke(() =>
            {
                var dc = lineVisual.RenderOpen();

                if (points.Count > 0)
                {
                    Point p0 = ConvertToPixel(points[0]);

                    if (NeedAiming)
                    {
                        dc.DrawRectangle(Brushes.Black, null, new Rect(p0.X - 2, p0.Y - 2, 4, 4));
                    }

                    if (IsGraph)
                    {
                        path.Figures.Clear();
                        PathFigure p = new PathFigure() { IsClosed = IsGraph };
                        p.Segments = new PathSegmentCollection();

                        p.StartPoint = new Point(p0.X, RenderSize.Height);
                        p.Segments.Add(new LineSegment() { Point = p0 });

                        for (int i = 1; i < points.Count; i++)
                        {
                            var p1 = ConvertToPixel(points[i]);

                            dc.DrawLine(linePen, p0, p1);

                            p.Segments.Add(new LineSegment() { Point = p1 });
                            p0 = p1;
                        }

                        p.Segments.Add(new LineSegment() { Point = new Point(p0.X, RenderSize.Height) });

                        path.Figures.Add(p);

                        dc.DrawGeometry(Fill, null, path);
                    }
                    else
                    {
                        for (int i = 1; i < points.Count; i++)
                        {
                            Point p1 = ConvertToPixel(points[i]);
                            dc.DrawLine(linePen, p0, p1);

                            p0 = p1;
                        }
                    }

                    if (NeedAiming)
                    {
                        dc.DrawEllipse(Brushes.OrangeRed, null, p0, 2, 2);
                    }
                }

                dc.Close();

                // 显示图形
                InvalidateVisual();
            });
        }

        #region override

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            Draw();
        }

        #endregion
    }
}
