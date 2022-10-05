using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
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
            DependencyProperty.Register("Fill", typeof(Brush), typeof(Linegram), new PropertyMetadata(Brushes.CornflowerBlue));

        /// <summary>
        /// 是否显示面积区域
        /// </summary>
        public Boolean IsGraph
        {
            get { return (Boolean)GetValue(IsGraphProperty); }
            set { SetValue(IsGraphProperty, value); }
        }
        public static readonly DependencyProperty IsGraphProperty =
            DependencyProperty.Register("IsGraph", typeof(Boolean), typeof(Linegram), new PropertyMetadata(true));

        /// <summary>
        /// 显示瞄点
        /// </summary>
        public Boolean NeedAiming
        {
            get { return (Boolean)GetValue(NeedAimingProperty); }
            set { SetValue(NeedAimingProperty, value); }
        }
        public static readonly DependencyProperty NeedAimingProperty =
            DependencyProperty.Register("NeedAiming", typeof(Boolean), typeof(Linegram), new PropertyMetadata(true));

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
                                new FrameworkPropertyMetadata(new ObservableCollection<Point>(), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (d, e) =>
                                {
                                    if (d is Linegram l)
                                    {
                                        if (e.NewValue is ObservableCollection<Point> pos)
                                        {
                                            if (pos.Count > 1)
                                            {
                                                l.DrawLine(pos.ToList());
                                            }
                                            else
                                            {
                                                l.InitLine();
                                                // TODO需要对事件进行删除-=
                                                l.LineSource.CollectionChanged += (sender, e) =>
                                                {
                                                    switch (e.Action)
                                                    {
                                                        case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                                                            if (e.NewItems != null && e.NewItems.Count == 1)
                                                            {
                                                                if (e.NewItems[0] is Point p)
                                                                    l.DrawLine(p);
                                                            }
                                                            break;
                                                        case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                                                            break;
                                                        case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                                                            break;
                                                        case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
                                                            break;
                                                        case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                                                            break;
                                                        default:
                                                            break;
                                                    }
                                                };
                                            }
                                        }
                                    }
                                }));

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
        /// 连续调用这个方法绘制图像前，需要先调用<see cref="InitLine"/>函数
        /// </summary>
        /// <param name="p"></param>
        public void DrawLine(Point p)
        {
            if (points.Count > 1)
            {
                ResetAxis(p);
            }
            points.Add(p);
            Draw();
        }

        /// <summary>
        /// 初始化绘图点的集合
        /// </summary>
        public void InitLine() => points.Clear();

        /// <summary>
        /// 绘制的路径
        /// </summary>
        PathGeometry path = new PathGeometry();

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
                    var last = ConvertToPixel(points[0]);

                    if (NeedAiming)
                    {
                        dc.DrawRectangle(Brushes.Black, null, new Rect(last.X - 2, last.Y - 2, 4, 4));
                    }

                    path.Figures = new PathFigureCollection();
                    PathFigure p = new PathFigure() { IsClosed = IsGraph };
                    p.Segments = new PathSegmentCollection();

                    if (IsGraph)
                    {
                        p.StartPoint = new Point(last.X, RenderSize.Height);
                        p.Segments.Add(new LineSegment() { Point = last });
                    }
                    else
                    {
                        p.StartPoint = last;
                    }

                    for (int i = 1; i < points.Count; i++)
                    {
                        var location = ConvertToPixel(points[i]);
                        // dc.DrawLine(linePen, last, location);

                        p.Segments.Add(new LineSegment() { Point = location });
                        last = location;
                    }

                    if (IsGraph)
                    {
                        p.Segments.Add(new LineSegment() { Point = new Point(last.X, RenderSize.Height) });
                    }

                    path.Figures.Add(p);

                    if (IsGraph)
                    {
                        dc.DrawGeometry(Fill, linePen, path);
                    }
                    else
                    {
                        dc.DrawGeometry(null, linePen, path);
                    }

                    if (NeedAiming)
                    {
                        dc.DrawEllipse(Brushes.OrangeRed, null, last, 2, 2);
                    }
                }

                dc.Close();

                // 显示图形
                InvalidateVisual();
            });
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

        #region override

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            Draw();
        }

        #endregion
    }
}
