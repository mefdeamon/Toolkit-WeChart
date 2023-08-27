using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
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
            YAxisLineAlignment = YAxisLineAlignment.Left;
            XAxisLineAlignment = XAxisLineAlignment.Bottom;
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

        #region X 轴刻度格式化文本

        /// <summary>
        /// X轴刻度 格式化字符串 
        /// </summary>
        public String XAxisTextFormatString
        {
            get { return (String)GetValue(XAxisTextFormatStringProperty); }
            set { SetValue(XAxisTextFormatStringProperty, value); }
        }
        public static readonly DependencyProperty XAxisTextFormatStringProperty =
            DependencyProperty.Register("XAxisTextFormatString", typeof(String), typeof(Linegram),
                new FrameworkPropertyMetadata(String.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// X轴刻度使用日期格式化显示
        /// </summary>
        public Boolean IsXAxisTextDateTimeFormat
        {
            get { return (Boolean)GetValue(IsXAxisTextDateTimeFormatProperty); }
            set { SetValue(IsXAxisTextDateTimeFormatProperty, value); }
        }
        public static readonly DependencyProperty IsXAxisTextDateTimeFormatProperty =
            DependencyProperty.Register("IsXAxisTextDateTimeFormat", typeof(Boolean), typeof(Linegram),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (d, e) =>
                {
                    if (d is Linegram line)
                    {
                        if (e.NewValue is bool need)
                        {
                            if (need)
                            {
                                line.SetDateTimeStringFormat();
                            }
                            else
                            {
                                line.SetNumericStringFormat();
                            }
                        }
                    }
                }));

        private void SetNumericStringFormat()
        {
            GetXAxisTextFormat = new Func<double, string>(d =>
            {
                return Math.Round(d).ToString(CultureInfo.CurrentCulture);
            });
        }

        private void SetDateTimeStringFormat()
        {
            try
            {
                TimeSpan timeSpan = DateTime.FromOADate(XMin) - DateTime.FromOADate(XMax);
                if (String.IsNullOrEmpty(XAxisTextFormatString))
                {
                    XAxisTextFormatString = ((timeSpan.TotalDays > 1825.0) ? "yyyy" : ((timeSpan.TotalDays > 365.0) ? "yyyy-MM" : ((timeSpan.TotalDays > 0.5) ? "yyyy-MM-dd" : ((!(timeSpan.TotalMinutes > 0.5)) ? "yyyy-MM-dd\nHH:mm:ss" : "yyyy-MM-dd\nHH:mm"))));
                }
            }
            catch
            {
            }

            GetXAxisTextFormat = new Func<double, string>(d =>
            {
                DateTime dateTime = DateTime.FromOADate(d);
                string text = string.Format(CultureInfo.CurrentCulture, "{0:" + XAxisTextFormatString + "}", dateTime);
                return text;
            });
        }

        #endregion

        /// <summary>
        /// 界面根据最新范围<see cref="FlashRangePointCount"/>刷新数据
        /// </summary>
        public Boolean IsFlashRange
        {
            get { return (Boolean)GetValue(IsFlashRangeProperty); }
            set { SetValue(IsFlashRangeProperty, value); }
        }

        public static readonly DependencyProperty IsFlashRangeProperty =
            DependencyProperty.Register("IsFlashRange", typeof(Boolean), typeof(Linegram), new PropertyMetadata(false));

        /// <summary>
        /// 最新刷新范围，默认200
        /// </summary>
        public int FlashRangePointCount
        {
            get { return (int)GetValue(FlashRangePointCountProperty); }
            set { SetValue(FlashRangePointCountProperty, value); }
        }
        public static readonly DependencyProperty FlashRangePointCountProperty =
            DependencyProperty.Register("FlashRangePointCount", typeof(int), typeof(Linegram), new PropertyMetadata(200));

        /// <summary>
        /// 界面的刻度根据数据的范围刷新
        /// </summary>
        public Boolean IsAxisFollowData
        {
            get { return (Boolean)GetValue(IsAxisFollowDataProperty); }
            set { SetValue(IsAxisFollowDataProperty, value); }
        }
        public static readonly DependencyProperty IsAxisFollowDataProperty =
            DependencyProperty.Register("IsAxisFollowData", typeof(Boolean), typeof(Linegram), new PropertyMetadata(true));

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
        /// 绘制一个新点
        /// 连续调用这个方法绘制图像前
        /// </summary>
        /// <param name="p"></param>
        public void Add(Point p)
        {
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
            Draw();
        }


        private void UpdateAxis()
        {
            if (!IsAxisFollowData) { return; }

            IsRenderAxis = false;
            var xs = points.Select(x => x.X);
            var xmin = xs.Min();
            var xmax = xs.Max();

            var ys = points.Select(y => y.Y);
            var ymin = ys.Min();
            var ymax = ys.Max();

            if (ymin < YMin)
            {
                YMin = ymin;
            }

            if (ymax > YMax)
            {
                YMax = ymax;
            }

            if (IsFlashRange)
            {
                if (points.Count > FlashRangePointCount)
                {
                    var list = points.Skip(points.Count - FlashRangePointCount).ToList();
                    XMin = list.Select(x => x.X).Min();
                    points = list;
                }
            }
            else
            {
                if (xmin < XMin)
                {
                    XMin = xmin;
                }
            }

            if (xmax > XMax)
            {
                XMax = xmax;
            }

            // SetDateTimeStringFormat();
            // 更新图
            UpdatePixelRatio();
            DrawXAxisScale();
            DrawYAxisScale();

            IsRenderAxis = true;
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
                    UpdateAxis();
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
