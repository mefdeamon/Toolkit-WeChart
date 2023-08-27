using System;
using System.Globalization;
using System.Net;
using System.Windows;
using System.Windows.Media;

namespace WindChart
{
    /// <summary>
    /// 基础图形，包含坐标系
    /// </summary>
    public class Gram : FrameworkElement
    {
        /// <summary>
        /// 刻度线长度，单位像素
        /// </summary>
        protected readonly int scalelinePixel = 5;

        /// <summary>
        /// 默认文本字体
        /// </summary>
        protected readonly Typeface defaultTypeface = new Typeface("Verdana");

        /// <summary>
        /// 图像
        /// </summary>
        protected VisualCollection Visuals;

        /// <summary>
        /// 用于画X轴信息
        /// </summary>
        DrawingVisual xAxisVisual;

        /// <summary>
        /// 用于画Y轴信息
        /// </summary>
        DrawingVisual yAxisVisual;

        /// <summary>
        /// 背景色
        /// </summary>
        public Brush Background { get; set; }

        public Gram()
        {
            Background = Brushes.WhiteSmoke;

            Visuals = new VisualCollection(this);
            xAxisVisual = new DrawingVisual();
            Visuals.Add(xAxisVisual);
            yAxisVisual = new DrawingVisual();
            Visuals.Add(yAxisVisual);

            xAxisPen = new Pen(XAxisBrush, 1);
            yAxisPen = new Pen(YAxisBrush, 1);
            xAxisPen.Freeze();
            yAxisPen.Freeze();
            axisLinePen = new Pen(AxisLineBrush, 1);
            axisLinePen.Freeze();
        }

        #region 画笔资源

        /// <summary>
        /// 用于画X轴/刻度文本
        /// </summary>
        Pen xAxisPen;
        /// <summary>
        /// 用于画Y轴/刻度文本
        /// </summary>
        Pen yAxisPen;

        /// <summary>
        /// 用于绘制刻度线
        /// </summary>
        Pen axisLinePen;

        #endregion

        #region 实际-像素比转换方法

        /// <summary>
        /// 实际场景的绝对宽度
        /// </summary>
        protected double absoluteActualX = 200;
        /// <summary>
        /// 实际场景的绝对高度
        /// </summary>
        protected double absoluteActualY = 200;

        /// <summary>
        /// X
        /// 水平方向
        /// 像素比
        /// 1单位数据对应多少像素
        /// 根据实际场景宽度和控件宽度得到
        /// </summary>
        protected double xPixelRatio = 10;
        /// <summary>
        /// Y
        /// 垂直方向
        /// 像素比
        /// 1单位数据对应多少像素
        /// 根据实际场景长(高)度和控件高度得到
        /// </summary>
        protected double yPixelRatio = 10;

        /// <summary>
        /// 转换实际坐标的Y点为图像坐标值
        /// </summary>
        /// <param name="actY"></param>
        /// <returns></returns>
        protected double YAxisConvertYToPixel(double actY)
        {
            var orgY = actY - YMin;
            var orgYToPX = RenderSize.Height - orgY * yPixelRatio;
            return orgYToPX;
        }

        /// <summary>
        /// 转换实际坐标的X点为图像坐标值
        /// 水平坐标轴（X轴）值转换，X
        /// 将实际值转换成像素
        /// </summary>
        /// <param name="actX"></param>
        /// <returns></returns>
        protected double XAxisConvertXToPixel(double actX)
        {
            var orgX = actX - XMin;
            var orgXToPX = orgX * xPixelRatio;
            return orgXToPX;
        }

        /// <summary>
        /// 坐标点转换
        /// </summary>
        /// <param name="actP"></param>
        /// <returns></returns>
        protected Point ConvertToPixel(Point actP)
        {
            actP.X = XAxisConvertXToPixel(actP.X);
            actP.Y = YAxisConvertYToPixel(actP.Y);
            return actP;
        }

        /// <summary>
        /// 更新像素比
        /// </summary>
        protected void UpdatePixelRatio()
        {
            // 场景实际大小情况
            absoluteActualX = XMax - XMin;
            absoluteActualY = YMax - YMin;

            // 实际场景中1m对应多少像素点
            xPixelRatio = RenderSize.Width / absoluteActualX;
            yPixelRatio = RenderSize.Height / absoluteActualY;
        }

        /// <summary>
        /// 确认点是否在图表范围中
        /// </summary>
        /// <param name="location">实际场景的点</param>
        /// <returns></returns>
        protected bool IsInGram(Point location)
        {
            if (XMin <= location.X && location.X <= XMax)
            {
                if (YMin <= location.Y && location.Y <= YMax)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 判断绘制的点是否在控件范围内
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        protected bool IsInRenderSize(Point location)
        {
            if (0 <= location.X && location.X <= RenderSize.Width)
            {
                if (0 <= location.Y && location.Y <= RenderSize.Height)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }


        protected Point ConvertToActual(Point pixcel)
        {
            pixcel.X = XAxisConvertToActual(pixcel.X);
            pixcel.Y = YAxisConvertToActual(pixcel.Y);
            return pixcel;
        }

        protected double YAxisConvertToActual(double pixelY)
        {
            var cur = (RenderSize.Height - pixelY) / yPixelRatio;
            return cur + YMin;
        }
        protected double XAxisConvertToActual(double pixelX)
        {
            var cur = (pixelX) / xPixelRatio;
            return cur + XMin;
        }

        #endregion

        #region 对外提供的参数设置

        private static void YRangeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Gram g)
            {
                if (g.IsRenderAxis)
                {
                    g.UpdatePixelRatio();
                    g.DrawYAxisScale();
                }
            }
        }

        private static void XRangeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Gram g)
            {
                if (g.IsRenderAxis)
                {
                    g.UpdatePixelRatio();
                    g.DrawXAxisScale();
                }
            }
        }

        private static void XAxisBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Gram)d).xAxisPen = new Pen((Brush)e.NewValue, 1);
            ((Gram)d).xAxisPen.Freeze();
            ((Gram)d).UpdatePixelRatio();
            ((Gram)d).DrawXAxisScale();
        }

        private static void YAxisBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Gram)d).yAxisPen = new Pen((Brush)e.NewValue, 1);
            ((Gram)d).yAxisPen.Freeze();
            ((Gram)d).UpdatePixelRatio();
            ((Gram)d).DrawYAxisScale();
        }
        private static void AxisLineBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Gram)d).axisLinePen = new Pen((Brush)e.NewValue, 1);
            ((Gram)d).axisLinePen.Freeze();
            ((Gram)d).UpdatePixelRatio();
            ((Gram)d).DrawYAxisScale();
            ((Gram)d).DrawXAxisScale();
        }

        #region X/Y Max/Min

        /// <summary>
        /// Y轴最大值
        /// </summary>
        public double YMax
        {
            get { return (double)GetValue(YMaxProperty); }
            set { SetValue(YMaxProperty, value); }
        }
        private static readonly DependencyProperty YMaxProperty =
            DependencyProperty.Register("YMax", typeof(double), typeof(Gram),
                new FrameworkPropertyMetadata(100D, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, YRangeChanged));

        /// <summary>
        /// Y轴最小值
        /// </summary>
        public double YMin
        {
            get { return (double)GetValue(YMinProperty); }
            set { SetValue(YMinProperty, value); }
        }
        public static readonly DependencyProperty YMinProperty =
            DependencyProperty.Register("YMin", typeof(double), typeof(Gram),
                new FrameworkPropertyMetadata(-100D, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, YRangeChanged));

        /// <summary>
        /// X轴最大值
        /// </summary>
        public double XMax
        {
            get { return (double)GetValue(XMaxProperty); }
            set { SetValue(XMaxProperty, value); }
        }
        public static readonly DependencyProperty XMaxProperty =
            DependencyProperty.Register("XMax", typeof(double), typeof(Gram),
                new FrameworkPropertyMetadata(100D, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, XRangeChanged));

        /// <summary>
        /// X轴最小值
        /// </summary>
        public double XMin
        {
            get { return (double)GetValue(XMinProperty); }
            set { SetValue(XMinProperty, value); }
        }
        public static readonly DependencyProperty XMinProperty =
            DependencyProperty.Register("XMin", typeof(double), typeof(Gram),
                new FrameworkPropertyMetadata(-100D, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, XRangeChanged));

        /// <summary>
        /// 当X/Y的Min/Max发生变化时，是否需要渲染轴信息
        /// 提供给外部是否更新轴刻度的功能，用于节省性能
        /// 默认值TRUE
        /// </summary>
        public bool IsRenderAxis { get; set; } = true;

        #endregion


        #region X/Y轴

        #region 刻度间隔&数量

        /// <summary>
        /// X轴刻度间隔
        /// </summary>
        public double XAxisScaleInterval => absoluteActualX / XAxisScaleCount;
        /// <summary>
        /// Y轴刻度间隔
        /// </summary>
        public double YAxisScaleInterval => absoluteActualY / YAxisScaleCount;

        /// <summary>
        /// X轴刻度个数
        /// </summary>
        public int XAxisScaleCount
        {
            get { return (int)GetValue(XAxisScaleCountProperty); }
            set { SetValue(XAxisScaleCountProperty, value); }
        }
        public static readonly DependencyProperty XAxisScaleCountProperty =
            DependencyProperty.Register("XAxisScaleCount", typeof(int), typeof(Gram),
                new FrameworkPropertyMetadata(5, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, XRangeChanged));
        /// <summary>
        /// Y轴刻度个数
        /// </summary>
        public int YAxisScaleCount
        {
            get { return (int)GetValue(YAxisScaleCountProperty); }
            set { SetValue(YAxisScaleCountProperty, value); }
        }
        public static readonly DependencyProperty YAxisScaleCountProperty =
            DependencyProperty.Register("YAxisScaleCount", typeof(int), typeof(Gram),
                new FrameworkPropertyMetadata(5, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, YRangeChanged));

        #endregion

        #region 文本/线/样式显示设置

        /// <summary>
        /// 需要X轴刻度文本
        /// </summary>
        public bool NeedXAxisText
        {
            get { return (bool)GetValue(NeedXAxisTextProperty); }
            set { SetValue(NeedXAxisTextProperty, value); }
        }
        public static readonly DependencyProperty NeedXAxisTextProperty =
            DependencyProperty.Register("NeedXAxisText", typeof(bool), typeof(Gram),
                new FrameworkPropertyMetadata(true, XRangeChanged));

        /// <summary>
        /// 需要X轴刻度线
        /// </summary>
        public bool NeedXAxisLine
        {
            get { return (bool)GetValue(NeedXAxisLineProperty); }
            set { SetValue(NeedXAxisLineProperty, value); }
        }
        public static readonly DependencyProperty NeedXAxisLineProperty =
            DependencyProperty.Register("NeedXAxisLine", typeof(bool), typeof(Gram),
                new FrameworkPropertyMetadata(true, XRangeChanged));

        /// <summary>
        /// X轴刻度线样式
        /// </summary>
        public XAxisLineAlignment XAxisLineAlignment
        {
            get { return (XAxisLineAlignment)GetValue(XAxisLineAlignmentProperty); }
            set { SetValue(XAxisLineAlignmentProperty, value); }
        }
        public static readonly DependencyProperty XAxisLineAlignmentProperty =
            DependencyProperty.Register("XAxisLineAlignment", typeof(XAxisLineAlignment), typeof(Gram),
                new FrameworkPropertyMetadata(XAxisLineAlignment.Center, XRangeChanged));


        /// <summary>
        /// X轴刻度文本样式
        /// </summary>
        public XAxisTextAlignment XAxisTextAlignment
        {
            get { return (XAxisTextAlignment)GetValue(XAxisTextAlignmentProperty); }
            set { SetValue(XAxisTextAlignmentProperty, value); }
        }
        /// <summary>
        /// <see cref="XAxisTextAlignment"/>
        /// </summary>
        public static readonly DependencyProperty XAxisTextAlignmentProperty =
            DependencyProperty.Register("XAxisTextAlignment", typeof(XAxisTextAlignment), typeof(Gram),
                new FrameworkPropertyMetadata(XAxisTextAlignment.Bottom, XRangeChanged));

        /// <summary>
        /// 需要Y轴刻度文本
        /// </summary>
        public bool NeedYAxisText
        {
            get { return (bool)GetValue(NeedYAxisTextProperty); }
            set { SetValue(NeedYAxisTextProperty, value); }
        }
        public static readonly DependencyProperty NeedYAxisTextProperty =
            DependencyProperty.Register("NeedYAxisText", typeof(bool), typeof(Gram),
                new FrameworkPropertyMetadata(true, YRangeChanged));

        /// <summary>
        /// 需要Y轴刻度线
        /// </summary>
        public bool NeedYAxisLine
        {
            get { return (bool)GetValue(NeedYAxisLineProperty); }
            set { SetValue(NeedYAxisLineProperty, value); }
        }
        public static readonly DependencyProperty NeedYAxisLineProperty =
            DependencyProperty.Register("NeedYAxisLine", typeof(bool),
                typeof(Gram),
                new FrameworkPropertyMetadata(true, YRangeChanged));

        /// <summary>
        /// Y轴刻度线样式
        /// </summary>
        public YAxisLineAlignment YAxisLineAlignment
        {
            get { return (YAxisLineAlignment)GetValue(YAxisLineAlignmentProperty); }
            set { SetValue(YAxisLineAlignmentProperty, value); }
        }
        public static readonly DependencyProperty YAxisLineAlignmentProperty =
            DependencyProperty.Register("YAxisLineAlignment", typeof(YAxisLineAlignment),
                typeof(Gram),
                new FrameworkPropertyMetadata(YAxisLineAlignment.Center, YRangeChanged));


        /// <summary>
        /// Y轴刻度文本样式
        /// </summary>
        public YAxisTextAlignment YAxisTextAlignment
        {
            get { return (YAxisTextAlignment)GetValue(YAxisTextAlignmentProperty); }
            set { SetValue(YAxisTextAlignmentProperty, value); }
        }
        /// <summary>
        /// <see cref="YAxisTextAlignment"/>
        /// </summary>
        public static readonly DependencyProperty YAxisTextAlignmentProperty =
            DependencyProperty.Register("YAxisTextAlignment", typeof(YAxisTextAlignment),
                typeof(Gram),
                new FrameworkPropertyMetadata(YAxisTextAlignment.Left, YRangeChanged));


        #endregion

        #region AxisLineMode == Location 的位置

        /// <summary>
        /// Y轴位置，默认在0点，单位与YMin/YMax一致
        /// </summary>
        public double YAxisLocation
        {
            get { return (double)GetValue(YAxisLocationProperty); }
            set { SetValue(YAxisLocationProperty, value); }
        }
        public static readonly DependencyProperty YAxisLocationProperty =
            DependencyProperty.Register("YAxisLocation", typeof(double), typeof(Gram),
                new FrameworkPropertyMetadata(0D, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, YRangeChanged));

        /// <summary>
        /// X轴位置，默认在0点，单位与XMin/XMax一致
        /// </summary>
        public double XAxisLocation
        {
            get { return (double)GetValue(XAxisLocationProperty); }
            set { SetValue(XAxisLocationProperty, value); }
        }
        public static readonly DependencyProperty XAxisLocationProperty =
            DependencyProperty.Register("XAxisLocation", typeof(double), typeof(Gram),
                new FrameworkPropertyMetadata(0D, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, XRangeChanged));


        #endregion

        /// <summary>
        /// 刻度 字体大小
        /// 默认12，单位：像素
        /// </summary>
        public double AxisFontSize
        {
            get { return (double)GetValue(AxisFontSizeProperty); }
            set { SetValue(AxisFontSizeProperty, value); }
        }
        public static readonly DependencyProperty AxisFontSizeProperty =
            DependencyProperty.Register("AxisFontSize", typeof(double), typeof(Gram),
                new FrameworkPropertyMetadata(10D, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, XRangeChanged));

        /// <summary>
        /// X轴 颜色
        /// 默认黑色
        /// </summary>
        public Brush XAxisBrush
        {
            get { return (Brush)GetValue(XAxisBrushProperty); }
            set { SetValue(XAxisBrushProperty, value); }
        }
        public static readonly DependencyProperty XAxisBrushProperty =
            DependencyProperty.Register("XAxisBrush", typeof(Brush), typeof(Gram),
                new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, XAxisBrushChanged));

        /// <summary>
        /// Y轴 颜色
        /// 默认黑色
        /// </summary>
        public Brush YAxisBrush
        {
            get { return (Brush)GetValue(YAxisBrushProperty); }
            set { SetValue(YAxisBrushProperty, value); }
        }
        public static readonly DependencyProperty YAxisBrushProperty =
            DependencyProperty.Register("YAxisBrush", typeof(Brush), typeof(Gram),
                new FrameworkPropertyMetadata(new SolidColorBrush(Color.FromArgb(0xFF, 29, 14, 17)), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, YAxisBrushChanged));

        /// <summary>
        /// X 轴箭头显示
        /// </summary>
        public Boolean NeedXAxisArrow
        {
            get { return (Boolean)GetValue(NeedXAxisArrowProperty); }
            set { SetValue(NeedXAxisArrowProperty, value); }
        }

        public static readonly DependencyProperty NeedXAxisArrowProperty =
            DependencyProperty.Register("NeedXAxisArrow", typeof(Boolean), typeof(Gram),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (d, e) =>
                {
                    if (d is Gram gram)
                    {
                        gram.DrawXAxisScale();
                    }
                }));

        /// <summary>
        /// Y 轴箭头显示
        /// </summary>
        public Boolean NeedYAxisArrow
        {
            get { return (Boolean)GetValue(NeedYAxisArrowProperty); }
            set { SetValue(NeedYAxisArrowProperty, value); }
        }

        public static readonly DependencyProperty NeedYAxisArrowProperty =
            DependencyProperty.Register("NeedYAxisArrow", typeof(Boolean), typeof(Gram),
                new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (d, e) =>
                {
                    if (d is Gram gram)
                    {
                        gram.DrawYAxisScale();
                    }
                }));

        /// <summary>
        /// 轴线颜色
        /// </summary>
        public Brush AxisLineBrush
        {
            get { return (Brush)GetValue(AxisLineBrushProperty); }
            set { SetValue(AxisLineBrushProperty, value); }
        }
        public static readonly DependencyProperty AxisLineBrushProperty =
            DependencyProperty.Register("AxisLineBrush", typeof(Brush), typeof(Gram),
                new FrameworkPropertyMetadata(new SolidColorBrush(Color.FromArgb(0xFF, 0x33, 0x33, 0x33)), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, AxisLineBrushChanged));

        #endregion

        #endregion

        /// <summary>
        /// 是否显示X轴刻度信息
        /// </summary>
        public bool NeedXAxisScale => NeedXAxisText | NeedXAxisLine;

        /// <summary>
        /// 是否显示Y轴刻度信息
        /// </summary>
        public bool NeedYAxisScale => NeedYAxisText | NeedYAxisLine;

        /// <summary>
        /// 获取X轴刻度显示格式化文本
        /// </summary>
        protected Func<double, string> GetXAxisTextFormat = d => Math.Round(d).ToString();

        /// <summary>
        /// 更新绘制坐标轴相关的内容
        /// </summary>
        protected void DrawXAxisScale()
        {
            var dc = xAxisVisual.RenderOpen();

            // 显示X轴
            if (NeedXAxisScale)
            {
                UpdatePixelRatio();

                double y1 = 0;
                double y2 = 0;

                switch (XAxisLineAlignment)
                {
                    case XAxisLineAlignment.Grid:
                        y1 = YAxisConvertYToPixel(YMin);
                        y2 = YAxisConvertYToPixel(YMax);
                        break;
                    case XAxisLineAlignment.Top:
                        y1 = YAxisConvertYToPixel(YMax);
                        y2 = y1 - scalelinePixel;
                        break;
                    case XAxisLineAlignment.Bottom:
                        y1 = YAxisConvertYToPixel(YMin);
                        y2 = y1 + scalelinePixel;
                        break;
                    case XAxisLineAlignment.Location:
                        y1 = YAxisConvertYToPixel(XAxisLocation);
                        y2 = y1 + scalelinePixel;
                        break;
                    case XAxisLineAlignment.Center:
                        y1 = YAxisConvertYToPixel((YMax + YMin) / 2);
                        break;
                    default:
                        break;
                }

                // 画刻度线条
                if (NeedXAxisLine)
                {
                    for (double i = XMin; i < XMax; i += XAxisScaleInterval)
                    {
                        var x = XAxisConvertXToPixel(i);

                        switch (XAxisLineAlignment)
                        {
                            case XAxisLineAlignment.Grid:
                            case XAxisLineAlignment.Top:
                            case XAxisLineAlignment.Bottom:
                            case XAxisLineAlignment.Location:
                                dc.DrawLine(axisLinePen, new Point(x, y1), new Point(x, y2));
                                break;
                            case XAxisLineAlignment.Center:
                                dc.DrawLine(axisLinePen, new Point(x, y1 - scalelinePixel), new Point(x, y1 + scalelinePixel));
                                break;
                            default:
                                break;
                        }
                    }

                    // 绘制最后一个刻度
                    switch (XAxisLineAlignment)
                    {
                        case XAxisLineAlignment.Top:
                        case XAxisLineAlignment.Bottom:
                        case XAxisLineAlignment.Location:
                            var x = XAxisConvertXToPixel(XMax);
                            if (NeedXAxisArrow)
                            {
                                var startPoint = new Point(XAxisConvertXToPixel(XMin), y1);
                                var endPoint = new Point(x + scalelinePixel * 2, y1);
                                var geo = CreateArrowGeometry(startPoint, endPoint, scalelinePixel * 2, scalelinePixel);
                                dc.DrawGeometry(axisLinePen.Brush, null, geo);
                            }
                            else
                            {
                                dc.DrawLine(axisLinePen, new Point(x, y1), new Point(x, y2));
                            }
                            break;
                        case XAxisLineAlignment.Grid:
                            x = XAxisConvertXToPixel(XMax);
                            dc.DrawLine(axisLinePen, new Point(x, y1), new Point(x, y2));
                            break;
                        case XAxisLineAlignment.Center:
                            x = XAxisConvertXToPixel(XMax);
                            if (NeedXAxisArrow)
                            {
                                var startPoint = new Point(XAxisConvertXToPixel(XMin), y1);
                                var endPoint = new Point(x + scalelinePixel * 2, y1);
                                var geo = CreateArrowGeometry(startPoint, endPoint, scalelinePixel * 2, scalelinePixel);
                                dc.DrawGeometry(axisLinePen.Brush, null, geo);
                            }
                            else
                            {
                                dc.DrawLine(axisLinePen, new Point(x, y1 - scalelinePixel), new Point(x, y1 + scalelinePixel));
                            }
                            break;
                        default:
                            break;
                    }

                    var x1 = XAxisConvertXToPixel(XMin);
                    var x2 = XAxisConvertXToPixel(XMax);
                    if (XAxisLineAlignment == XAxisLineAlignment.Grid)
                    {
                        dc.DrawLine(axisLinePen, new Point(x1, y2), new Point(x2, y2));
                    }
                    dc.DrawLine(axisLinePen, new Point(x1, y1), new Point(x2, y1));
                }

                // 是否需要绘制文本
                if (NeedXAxisText)
                {
                    FormattedText text;
                    for (double i = XMin; i < XMax; i += XAxisScaleInterval)
                    {
                        var x = XAxisConvertXToPixel(i);

                        // 画刻度文本
                        var textcontent = GetXAxisTextFormat.Invoke(i);
                        text = new FormattedText(textcontent, CultureInfo.CurrentCulture,
                                                                FlowDirection.LeftToRight, defaultTypeface, AxisFontSize, XAxisBrush,
                                                                VisualTreeHelper.GetDpi(this).PixelsPerDip);

                        var offsetLeft = text.Width / 2;

                        switch (XAxisTextAlignment)
                        {
                            case XAxisTextAlignment.Both:
                                dc.DrawText(text, new Point(x - offsetLeft, y1));
                                dc.DrawText(text, new Point(x - offsetLeft, y2 - (text.Height)));
                                break;
                            case XAxisTextAlignment.Top:
                                if (XAxisLineAlignment == XAxisLineAlignment.Grid)
                                {
                                    dc.DrawText(text, new Point(x - offsetLeft, y2 - (text.Height) - scalelinePixel));
                                }
                                else
                                {
                                    dc.DrawText(text, new Point(x - offsetLeft, y1 - (text.Height) - scalelinePixel));
                                }
                                break;
                            case XAxisTextAlignment.Bottom:
                                if (XAxisLineAlignment == XAxisLineAlignment.Grid)
                                {
                                    dc.DrawText(text, new Point(x - offsetLeft, y1 + scalelinePixel));
                                }
                                else
                                {
                                    dc.DrawText(text, new Point(x - offsetLeft, y1 + scalelinePixel));
                                }
                                break;
                            default:
                                break;
                        }

                    }

                    // 画最后一个文本
                    var xx = XAxisConvertXToPixel(XMax);
                    // 画刻度文本
                    text = new FormattedText(GetXAxisTextFormat.Invoke(XMax), CultureInfo.CurrentCulture,
                                                            FlowDirection.LeftToRight, defaultTypeface, AxisFontSize, XAxisBrush,
                                                            VisualTreeHelper.GetDpi(this).PixelsPerDip);
                    var textOffsetLeft = text.Width / 2;
                    switch (XAxisTextAlignment)
                    {
                        case XAxisTextAlignment.Both:

                            dc.DrawText(text, new Point(xx - textOffsetLeft, y1));
                            dc.DrawText(text, new Point(xx - textOffsetLeft, y2 - (text.Height)));
                            break;
                        case XAxisTextAlignment.Top:

                            if (XAxisLineAlignment == XAxisLineAlignment.Grid)
                            {
                                dc.DrawText(text, new Point(xx - textOffsetLeft, y2 - (text.Height) - scalelinePixel));
                            }
                            else
                            {
                                dc.DrawText(text, new Point(xx - textOffsetLeft, y1 - (text.Height) - scalelinePixel));
                            }
                            break;
                        case XAxisTextAlignment.Bottom:
                            if (XAxisLineAlignment == XAxisLineAlignment.Grid)
                            {
                                dc.DrawText(text, new Point(xx - textOffsetLeft, y1 + scalelinePixel));
                            }
                            else
                            {
                                dc.DrawText(text, new Point(xx - textOffsetLeft, y1 + scalelinePixel));
                            }
                            break;
                        default:
                            break;
                    }
                }
            }

            dc.Close();
            // 显示图形
            InvalidateVisual();
        }

        /// <summary>
        /// 绘制纵轴Y轴的刻度信息
        /// </summary>
        protected void DrawYAxisScale()
        {
            var dc = yAxisVisual.RenderOpen();

            // 显示X轴
            if (NeedYAxisScale)
            {
                UpdatePixelRatio();

                double x1 = 0;
                double x2 = 0;

                // 刻度线高度
                var scaleWidth = absoluteActualX / 60;

                switch (YAxisLineAlignment)
                {
                    case YAxisLineAlignment.Grid:
                        x1 = XAxisConvertXToPixel(XMin);
                        x2 = XAxisConvertXToPixel(XMax);
                        break;
                    case YAxisLineAlignment.Left:
                        x1 = XAxisConvertXToPixel(XMin);
                        x2 = x1 - scalelinePixel;

                        break;
                    case YAxisLineAlignment.Right:
                        // 顶部
                        x1 = XAxisConvertXToPixel(XMax);
                        x2 = x1 + scalelinePixel;
                        break;

                    case YAxisLineAlignment.Location:
                        // 在定位上画
                        x1 = XAxisConvertXToPixel(YAxisLocation);
                        x2 = x1 - scalelinePixel;
                        break;
                    case YAxisLineAlignment.Center:
                        // 中心
                        x1 = XAxisConvertXToPixel((XMax + XMin) / 2);
                        break;
                    default:
                        break;
                }

                if (NeedYAxisLine)
                {
                    for (double i = YMin; i < YMax; i += YAxisScaleInterval)
                    {
                        var y = YAxisConvertYToPixel(i);

                        switch (YAxisLineAlignment)
                        {
                            case YAxisLineAlignment.Grid:
                            case YAxisLineAlignment.Left:
                            case YAxisLineAlignment.Right:
                            case YAxisLineAlignment.Location:
                                dc.DrawLine(axisLinePen, new Point(x1, y), new Point(x2, y));
                                break;
                            case YAxisLineAlignment.Center:
                                dc.DrawLine(axisLinePen, new Point(x1 - scalelinePixel, y), new Point(x1 + scalelinePixel, y));
                                break;
                            default:
                                break;
                        }
                    }

                    // 绘制最后一个刻度
                    switch (YAxisLineAlignment)
                    {
                        case YAxisLineAlignment.Left:
                        case YAxisLineAlignment.Right:
                        case YAxisLineAlignment.Location:
                            var y = YAxisConvertYToPixel(YMax);
                            if (NeedYAxisArrow)
                            {
                                var startPoint = new Point(x1, y);
                                var endPoint = new Point(x1, y - scalelinePixel * 2);
                                var geo = CreateArrowGeometry(startPoint, endPoint, scalelinePixel * 2, scalelinePixel);
                                dc.DrawGeometry(axisLinePen.Brush, null, geo);
                            }
                            else
                            {
                                dc.DrawLine(axisLinePen, new Point(x1, y), new Point(x2, y));
                            }
                            break;
                        case YAxisLineAlignment.Grid:
                            y = YAxisConvertYToPixel(YMax);
                            dc.DrawLine(axisLinePen, new Point(x1, y), new Point(x2, y));
                            break;
                        case YAxisLineAlignment.Center:
                            y = YAxisConvertYToPixel(YMax);
                            if (NeedYAxisArrow)
                            {
                                var startPoint = new Point(x1, y);
                                var endPoint = new Point(x1, y - scalelinePixel * 2);
                                var geo = CreateArrowGeometry(startPoint, endPoint, scalelinePixel * 2, scalelinePixel);
                                dc.DrawGeometry(axisLinePen.Brush, null, geo);
                            }
                            else
                            {
                                dc.DrawLine(axisLinePen, new Point(x1 - scalelinePixel, y), new Point(x1 + scalelinePixel, y));
                            }
                            break;
                        default:
                            break;
                    }

                    // 绘制轴线
                    var y1 = YAxisConvertYToPixel(YMin);
                    var y2 = YAxisConvertYToPixel(YMax);
                    if (YAxisLineAlignment == YAxisLineAlignment.Grid)
                    {
                        dc.DrawLine(axisLinePen, new Point(x2, y1), new Point(x2, y2));
                    }
                    dc.DrawLine(axisLinePen, new Point(x1, y1), new Point(x1, y2));
                }

                // 是否需要绘制文本
                if (NeedYAxisText)
                {
                    // 文本和线之间的间隔
                    int lineTextGap = 2;

                    FormattedText text;
                    for (double i = YMin; i < YMax; i += YAxisScaleInterval)
                    {
                        var y = YAxisConvertYToPixel(i);

                        // 画刻度文本
                        var textcontent = ((int)i).ToString();
                        text = new FormattedText(textcontent, CultureInfo.CurrentCulture,
                                                                FlowDirection.LeftToRight, defaultTypeface, AxisFontSize, YAxisBrush,
                                                                VisualTreeHelper.GetDpi(this).PixelsPerDip);

                        switch (YAxisTextAlignment)
                        {
                            case YAxisTextAlignment.Both:
                                dc.DrawText(text, new Point(x1 - text.Width - lineTextGap, y - text.Height / 2 - 1));
                                dc.DrawText(text, new Point(x2 + lineTextGap, y - text.Height / 2 - 1));
                                break;
                            case YAxisTextAlignment.Left:
                                if (YAxisLineAlignment == YAxisLineAlignment.Grid)
                                {
                                    dc.DrawText(text, new Point(x1 - text.Width - lineTextGap - scalelinePixel, y - text.Height / 2 - 1));
                                }
                                else
                                {
                                    dc.DrawText(text, new Point(x1 - text.Width - lineTextGap - scalelinePixel, y - text.Height / 2 - 1));
                                }
                                break;
                            case YAxisTextAlignment.Right:
                                if (YAxisLineAlignment == YAxisLineAlignment.Grid)
                                {
                                    dc.DrawText(text, new Point(x2 + lineTextGap + scalelinePixel, y - text.Height / 2 - 1));
                                }
                                else
                                {
                                    dc.DrawText(text, new Point(x1 + lineTextGap + scalelinePixel, y - text.Height / 2 - 1));
                                }
                                break;
                            default:
                                break;
                        }

                    }

                    text = new FormattedText(((int)YMax).ToString(), CultureInfo.CurrentCulture,
                                                            FlowDirection.LeftToRight, defaultTypeface, AxisFontSize, YAxisBrush,
                                                            VisualTreeHelper.GetDpi(this).PixelsPerDip);
                    var yy = YAxisConvertYToPixel(YMax);

                    switch (YAxisTextAlignment)
                    {
                        case YAxisTextAlignment.Both:
                            dc.DrawText(text, new Point(x1 - text.Width - lineTextGap, yy - text.Height / 2 - 1));
                            dc.DrawText(text, new Point(x2 + lineTextGap, yy - text.Height / 2 - 1));
                            break;
                        case YAxisTextAlignment.Left:
                            if (YAxisLineAlignment == YAxisLineAlignment.Grid)
                            {
                                dc.DrawText(text, new Point(x1 - text.Width - lineTextGap - scalelinePixel, yy - text.Height / 2 - 1));
                            }
                            else
                            {
                                dc.DrawText(text, new Point(x1 - text.Width - lineTextGap - scalelinePixel, yy - text.Height / 2 - 1));
                            }
                            break;
                        case YAxisTextAlignment.Right:
                            if (YAxisLineAlignment == YAxisLineAlignment.Grid)
                            {
                                dc.DrawText(text, new Point(x2 + lineTextGap + scalelinePixel, yy - text.Height / 2 - 1));
                            }
                            else
                            {
                                dc.DrawText(text, new Point(x1 + lineTextGap + scalelinePixel, yy - text.Height / 2 - 1));
                            }
                            break;
                        default:
                            break;
                    }
                }
            }

            dc.Close();

            // 显示图形
            InvalidateVisual();
        }



        /// <summary>
        /// 获取箭头图形
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="endPoint"></param>
        /// <param name="arrowLength"></param>
        /// <param name="arrowWidth"></param>
        /// <returns></returns>
        private PathGeometry CreateArrowGeometry(Point startPoint, Point endPoint, double arrowLength = 10, double arrowWidth = 5)
        {
            Vector arrowDirection = endPoint - startPoint;
            arrowDirection.Normalize();

            Point arrowTip = endPoint - arrowDirection * arrowLength;

            Vector arrowSide = new Vector(arrowDirection.Y, arrowDirection.X) * arrowWidth;
            Point arrowBase1 = arrowTip + arrowSide;
            Point arrowBase2 = arrowTip - arrowSide;

            PathFigure arrowHead = new PathFigure();
            arrowHead.StartPoint = endPoint;
            arrowHead.Segments.Add(new LineSegment(arrowBase1, isStroked: true));
            arrowHead.Segments.Add(new LineSegment(arrowBase2, isStroked: true));
            arrowHead.IsClosed = true;
            arrowHead.IsFilled = true;

            PathGeometry arrowGeometry = new PathGeometry();
            arrowGeometry.Figures.Add(arrowHead);
            return arrowGeometry;
        }


        #region override

        protected override int VisualChildrenCount => Visuals.Count;

        protected override Visual GetVisualChild(int index)
        {
            return Visuals[index];
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            DrawXAxisScale();
            DrawYAxisScale();
            base.OnRenderSizeChanged(sizeInfo);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            drawingContext.DrawRectangle(Background, null, new Rect(0, 0, RenderSize.Width, RenderSize.Height));
            base.OnRender(drawingContext);
        }

        #endregion
    }
}
