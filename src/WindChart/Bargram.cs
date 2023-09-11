using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace WindChart
{
    /// <summary>
    /// 条形图
    /// </summary>
    public class Bargram : Gram
    {
        /// <summary>
        /// 折线画板
        /// </summary>
        private DrawingVisual barVisual;


        public Bargram()
        {
            barVisual = new DrawingVisual();
            Visuals.Add(barVisual);

            // 初始化画板刻度信息
            YAxisLineAlignment = YAxisLineAlignment.Left;
            XAxisLineAlignment = XAxisLineAlignment.Bottom;
            XMin = 0;
            YMin = 0;
            XMax = 10;
            YMax = 10;
            NeedXAxisLine = false;
            NeedXAxisText = false;
        }

        private List<Bar> Bars = new List<Bar>();
        public void Draw(List<Bar> bars)
        {
            Bars = bars;
            Draw();
        }

        FormattedText text = null;
        Typeface typeface = new Typeface("微软雅黑");

        public double BarFontSize { get; set; } = 12;

        #region 依赖属性

        /// <summary>
        /// 需要间隔
        /// </summary>
        public bool NeedInterval
        {
            get { return (bool)GetValue(NeedIntervalProperty); }
            set { SetValue(NeedIntervalProperty, value); }
        }
        /// <summary>
        /// <see cref="NeedInterval"/>
        /// </summary>
        public static readonly DependencyProperty NeedIntervalProperty =
            DependencyProperty.Register("NeedInterval", typeof(bool), typeof(Bargram), new PropertyMetadata(true, (d, e) => ((Bargram)d).Draw()));

        /// <summary>
        /// 值标签位置
        /// </summary>
        public BarValueLocation ValueLabelLocation
        {
            get { return (BarValueLocation)GetValue(ValueLabelLocationProperty); }
            set { SetValue(ValueLabelLocationProperty, value); }
        }
        /// <summary>
        /// <see cref="ValueLabelLocation"/>
        /// </summary>
        public static readonly DependencyProperty ValueLabelLocationProperty =
            DependencyProperty.Register("ValueLabelLocation", typeof(BarValueLocation), typeof(Bargram), new PropertyMetadata(BarValueLocation.Follow, (d, e) => ((Bargram)d).Draw()));

        /// <summary>
        /// 值标签颜色
        /// </summary>
        public Brush ValueLabelBrush
        {
            get { return (Brush)GetValue(ValueLabelBrushProperty); }
            set { SetValue(ValueLabelBrushProperty, value); }
        }
        /// <summary>
        /// <see cref="ValueLabelBrush"/>
        /// </summary>
        public static readonly DependencyProperty ValueLabelBrushProperty =
            DependencyProperty.Register("ValueLabelBrush", typeof(Brush), typeof(Bargram), new PropertyMetadata(Brushes.Black, (d, e) => ((Bargram)d).Draw()));

        /// <summary>
        /// 条形布局方向
        /// </summary>
        public BarDirection Direction
        {
            get { return (BarDirection)GetValue(DirectionProperty); }
            set { SetValue(DirectionProperty, value); }
        }
        /// <summary>
        /// <see cref="Direction"/>
        /// </summary>
        public static readonly DependencyProperty DirectionProperty =
            DependencyProperty.Register("Direction", typeof(BarDirection), typeof(Bargram), new PropertyMetadata(BarDirection.Vertical, (d, e) =>
            {
                if (d is Bargram g)
                {
                    g.IsRenderAxis = false;
                    switch (g.Direction)
                    {
                        case BarDirection.Vertical:
                            g.NeedXAxisArrow = false;
                            g.NeedXAxisText = false;
                            g.NeedXAxisLine = false;
                            g.NeedYAxisLine = true;
                            g.NeedYAxisText = true;
                            g.NeedYAxisArrow = true;
                            break;
                        case BarDirection.Horizontal:
                            g.NeedXAxisArrow = true;
                            g.NeedXAxisText = true;
                            g.NeedXAxisLine = true;
                            g.NeedYAxisLine = false;
                            g.NeedYAxisText = false;
                            g.NeedYAxisArrow = false;
                            break;
                        default:
                            break;
                    }
                    g.IsRenderAxis = true;
                    g.DrawXAxisScale();
                    g.DrawYAxisScale();
                    g.Draw();
                }
            }));

        /// <summary>
        /// 数据
        /// </summary>
        public ObservableCollection<Bar> BarSource
        {
            get { return (ObservableCollection<Bar>)GetValue(BarSourceProperty); }
            set { SetValue(BarSourceProperty, value); }
        }
        /// <summary>
        /// <see cref="BarSource"/>
        /// </summary>
        public static readonly DependencyProperty BarSourceProperty =
            DependencyProperty.Register("BarSource", typeof(ObservableCollection<Bar>), typeof(Bargram),
                new FrameworkPropertyMetadata(default(ObservableCollection<Bar>), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (d, e) => ((Bargram)d).Draw(((ObservableCollection<Bar>)e.NewValue).ToList())));


        /// <summary>
        /// 条形边框的颜色
        /// </summary>
        public Brush BarBorderBrush
        {
            get { return (Brush)GetValue(BarBorderBrushProperty); }
            set { SetValue(BarBorderBrushProperty, value); }
        }
        public static readonly DependencyProperty BarBorderBrushProperty =
            DependencyProperty.Register("BarBorderBrush", typeof(Brush), typeof(Bargram),
                new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (d, e) =>
                {
                    if (d is Bargram g)
                    {
                        g.rectBorderPen = new Pen((Brush)e.NewValue, 1);
                        g.Draw();
                    }
                }));

        Pen rectBorderPen = new Pen(Brushes.Black, 1);

        #endregion

        #region 根据方向不同绘制

        /// <summary>
        /// 绘图
        /// </summary>
        private void Draw()
        {
            if (Bars.Count > 0)
            {
                switch (Direction)
                {
                    case BarDirection.Vertical:
                        DrawVertical();
                        break;
                    case BarDirection.Horizontal:
                        DrawHorizontal();
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// 像素边界偏移
        /// </summary>
        const double xOffset = 2;

        /// <summary>
        /// 水平
        /// </summary>
        private void DrawHorizontal()
        {
            var maxX = Bars.Max(t => t.Value);
            if (maxX > XMax)
            {
                IsRenderAxis = false;
                XMax = maxX + maxX / 10;

                IsRenderAxis = true;
                UpdatePixelRatio();
                DrawXAxisScale();
            }

            if (YAxisScaleCount != Bars.Count)
            {
                YAxisScaleCount = Bars.Count;
            }

            this.Dispatcher.Invoke(() =>
            {
                var dc = barVisual.RenderOpen();

                if (Bars.Count > 0)
                {
                    var barHeight = (RenderSize.Height) / Bars.Count;
                    double barYLocation = 0;

                    if (NeedInterval)
                    {
                        barHeight = barHeight / 2.0;
                        barYLocation = barHeight / 2.0;
                    }
                    foreach (var item in Bars)
                    {
                        var barWidth = XAxisConvertXToPixel(item.Value);

                        if (!NeedInterval)
                        {
                            if (rectBorderPen.Brush != item.Fill)
                            {
                                rectBorderPen = new Pen(item.Fill, 1);
                            }
                        }
                        dc.DrawRectangle(item.Fill, rectBorderPen, new Rect(0, barYLocation, barWidth, barHeight));

                        // 值文本
                        text = new FormattedText(item.Value.ToString(), CultureInfo.CurrentCulture,
                                                              FlowDirection.LeftToRight, typeface, BarFontSize, ValueLabelBrush,
                                                              VisualTreeHelper.GetDpi(this).PixelsPerDip);
                        switch (ValueLabelLocation)
                        {
                            case BarValueLocation.Follow:
                                if (barWidth + text.Width > RenderSize.Width)
                                {
                                    dc.DrawText(text, new Point(RenderSize.Width - text.Width, barYLocation + barHeight / 2 - text.Height / 2));
                                }
                                else
                                {
                                    dc.DrawText(text, new Point(barWidth + xOffset, barYLocation + barHeight / 2 - text.Height / 2));
                                }
                                break;
                            case BarValueLocation.TopLeft:
                                dc.DrawText(text, new Point(xOffset, barYLocation + barHeight / 2 - text.Height / 2));
                                break;
                            case BarValueLocation.BottomRight:
                                dc.DrawText(text, new Point(RenderSize.Width - text.Width, barYLocation + barHeight / 2 - text.Height / 2));
                                break;
                            default:
                                break;
                        }

                        // 标签文本
                        text = new FormattedText(item.Label.ToString(), CultureInfo.CurrentCulture,
                                                            FlowDirection.LeftToRight, typeface, AxisFontSize, YAxisBrush,
                                                            VisualTreeHelper.GetDpi(this).PixelsPerDip);

                        Point labelLocation = new Point(-xOffset - text.Width, barYLocation + barHeight / 2 - text.Height / 2);
                        dc.DrawText(text, labelLocation);

                        barYLocation += barHeight;
                        if (NeedInterval)
                        {
                            barYLocation += barHeight;
                        }
                    }
                }

                dc.Close();

                // 显示图形
                InvalidateVisual();
            });
        }

        /// <summary>
        /// 垂直
        /// </summary>
        private void DrawVertical()
        {
            var maxY = Bars.Max(t => t.Value);
            if (maxY > YMax)
            {
                IsRenderAxis = false;
                YMax = maxY + maxY / 10;
                IsRenderAxis = true;
                UpdatePixelRatio();
                DrawYAxisScale();
            }
            if (XAxisScaleCount != Bars.Count)
            {
                XAxisScaleCount = Bars.Count;
            }

            this.Dispatcher.Invoke(() =>
            {
                var dc = barVisual.RenderOpen();

                if (Bars.Count > 0)
                {
                    var barWidth = (RenderSize.Width) / Bars.Count;
                    double barXLocation = 0;

                    if (NeedInterval)
                    {
                        barWidth = barWidth / 2.0;
                        barXLocation = barWidth / 2.0;
                    }
                    foreach (var item in Bars)
                    {
                        var barHeight = YAxisConvertYToPixel(item.Value);
                        if (!NeedInterval)
                        {
                            if (rectBorderPen.Brush != item.Fill)
                            {
                                rectBorderPen = new Pen(item.Fill, 1);
                            }
                        }
                        dc.DrawRectangle(item.Fill, rectBorderPen, new Rect(barXLocation, barHeight, barWidth, RenderSize.Height - barHeight));

                        // 值文本
                        text = new FormattedText(item.Value.ToString(), CultureInfo.CurrentCulture,
                                                              FlowDirection.LeftToRight, typeface, BarFontSize, ValueLabelBrush,
                                                              VisualTreeHelper.GetDpi(this).PixelsPerDip);
                        switch (ValueLabelLocation)
                        {
                            case BarValueLocation.Follow:
                                dc.DrawText(text, new Point(barXLocation + barWidth / 2 - text.Width / 2, barHeight - text.Height));
                                break;
                            case BarValueLocation.TopLeft:
                                dc.DrawText(text, new Point(barXLocation + barWidth / 2 - text.Width / 2, 0));
                                break;
                            case BarValueLocation.BottomRight:
                                dc.DrawText(text, new Point(barXLocation + barWidth / 2 - text.Width / 2, RenderSize.Height - text.Height));
                                break;
                            default:
                                break;
                        }

                        // 标签文本
                        text = new FormattedText(item.Label.ToString(), CultureInfo.CurrentCulture,
                                                            FlowDirection.LeftToRight, typeface, AxisFontSize, XAxisBrush,
                                                            VisualTreeHelper.GetDpi(this).PixelsPerDip);

                        Point labelLocation = new Point(barXLocation + barWidth / 2 - text.Width / 2, RenderSize.Height);
                        dc.DrawText(text, labelLocation);

                        barXLocation += barWidth;
                        if (NeedInterval)
                        {
                            barXLocation += barWidth;
                        }
                    }
                }

                dc.Close();

                // 显示图形
                InvalidateVisual();
            });
        }
        #endregion

        #region override

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            Draw();
        }

        #endregion
    }

    /// <summary>
    /// 条形图的方向
    /// </summary>
    public enum BarDirection
    {
        /// <summary>
        /// 由下至上 垂直
        /// </summary>
        Vertical,
        /// <summary>
        /// 由左至右 水平
        /// </summary>
        Horizontal
    }

    /// <summary>
    /// 值文本位置
    /// </summary>
    public enum BarValueLocation
    {
        /// <summary>
        /// 不显示文本
        /// </summary>
        None,
        /// <summary>
        /// 跟随值直接显示到最顶上
        /// </summary>
        Follow,
        /// <summary>
        /// 显示到顶上或者左边
        /// </summary>
        TopLeft,
        /// <summary>
        /// 显示到底部或者右边
        /// </summary>
        BottomRight,
    }

    /// <summary>
    /// 条形
    /// </summary>
    public class Bar
    {
        /// <summary>
        /// 标签
        /// </summary>
        public string Label { get; set; }

        private double _value;
        /// <summary>
        /// 值
        /// </summary>
        public double Value
        {
            get { return _value; }
            set
            {
                _value = value;
            }
        }
        /// <summary>
        /// 填充颜色
        /// </summary>
        public Brush Fill { get; set; } = Brushes.CornflowerBlue;
    }
}
