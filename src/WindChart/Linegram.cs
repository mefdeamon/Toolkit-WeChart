using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using static System.Net.Mime.MediaTypeNames;

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
        /// 鼠标线
        /// </summary>
        private DrawingVisual mouseVisual;

        /// <summary>
        /// 线条颜色和粗细
        /// </summary>
        Pen linePen;

        public Linegram()
        {
            lineVisual = new DrawingVisual();
            Visuals.Add(lineVisual);
            mouseVisual = new DrawingVisual();
            Visuals.Add(mouseVisual);


            linePen = new Pen(LineBrush, LineThickness);
            linePen.Freeze();

            // 初始化画板刻度信息
            YAxisLineMode = AxisLineMode.TopLeft;
            XAxisLineMode = AxisLineMode.BottmRight;
            XMin = 0;
            YMin = 0;
        }


        #region 鼠标

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            DrawMouse(e.GetPosition(this));
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (e.MouseDevice.LeftButton == MouseButtonState.Pressed)
            {
                // 获取鼠标位置，检查周围是否存在目标，标记目标ID
                var poi = e.GetPosition(this);
                DrawMouse(poi);
            }
        }

        private void DrawMouse(Point poi)
        {
            var dc = mouseVisual.RenderOpen();
            if (points.Count > 0)
            {
                // 获取当前对应的目标点
                var point = FindNearPoint(poi);

                // 将目标点转换成像素坐标
                poi = ConvertToPixel(point);

                // 绘制鼠标原点和XY
                dc.DrawEllipse(Brushes.Red, null, poi, 2, 2);
                dc.DrawLine(new Pen(Brushes.Red, 1), new Point(poi.X, 0), new Point(poi.X, RenderSize.Height));
                dc.DrawLine(new Pen(Brushes.Red, 1), new Point(0, poi.Y), new Point(RenderSize.Width, poi.Y));

                // 画刻度文本
                var textcontent = (Math.Round(point.Y, 2)).ToString();
                FormattedText text = new FormattedText(textcontent, CultureInfo.CurrentCulture,
                                                        FlowDirection.LeftToRight, defaultTypeface, AxisFontSize, XAxisBrush,
                                                        VisualTreeHelper.GetDpi(this).PixelsPerDip);
                var wordWidth = (text.Width / textcontent.Length);
                poi.X = poi.X + wordWidth / 2;
                poi.Y = poi.Y + wordWidth / 2;
                dc.DrawRectangle(Brushes.White, null, new Rect(poi, new Size(text.Width + wordWidth * 2, text.Height)));
                poi.X = poi.X + wordWidth;
                dc.DrawText(text, poi);
            }

            dc.Close();
        }

        private void ClearMouse()
        {
            var dc = mouseVisual.RenderOpen();
            dc.Close();
        }

        /// <summary>
        /// 找到对应位置的对应点
        /// </summary>
        /// <param name="xvv"></param>
        /// <returns></returns>
        Point FindNearPoint(Point poi)
        {
            var p2 = ConvertToActual(poi);
            var xx = points.Select(t => t.X).OrderBy(x => Math.Abs(x - p2.X)).First();

            var point = points.Where(t => t.X == xx).FirstOrDefault();
            return point;
        }



        protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseRightButtonDown(e);
            ClearMouse();
        }




        #endregion


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

            if (maxX > YMax)
            {
                YMax = maxY;
            }
            if (minX < YMin)
            {
                YMin = minY;
            }

            if (minY < XMin)
            {
                XMin = minX;
            }

            if (maxX > XMax)
            {
                XMax = maxX;
            }
            

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
            ClearMouse();
        }

        #endregion
    }
}
