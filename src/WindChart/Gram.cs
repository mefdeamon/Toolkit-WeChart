using System.Globalization;
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
        }
        #region 画笔资源
        
        /// <summary>
        /// 用于画X轴/刻度
        /// </summary>
        Pen xAxisPen;
        /// <summary>
        /// 用于画Y轴/刻度
        /// </summary>
        Pen yAxisPen;

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

        #endregion

        #region 对外提供的参数设置

        private static void YRangeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Gram)d).UpdatePixelRatio();

            ((Gram)d).DrawYAxisScale();
        }

        private static void XRangeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Gram)d).UpdatePixelRatio();

            ((Gram)d).DrawXAxisScale();
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

        #region X/Y Max/Min

        /// <summary>
        /// Y轴最大值
        /// </summary>
        public double YMax
        {
            get { return (double)GetValue(YMaxProperty); }
            set { SetValue(YMaxProperty, value); }
        }

        // Using a DependencyProperty as the backing store for YMax.  This enables animation, styling, binding, etc...
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

        // Using a DependencyProperty as the backing store for YMin.  This enables animation, styling, binding, etc...
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

        // Using a DependencyProperty as the backing store for XMax.  This enables animation, styling, binding, etc...
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

        // Using a DependencyProperty as the backing store for XMin.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty XMinProperty =
            DependencyProperty.Register("XMin", typeof(double), typeof(Gram),
                new FrameworkPropertyMetadata(-100D, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, XRangeChanged));

        #endregion


        #region X/Y轴

        #region 间隔

        /// <summary>
        /// X轴刻度之间间隔
        /// 默认 20，单位：米（m）
        /// </summary>
        public double XAxisScaleInterval
        {
            get { return (double)GetValue(XAxisScaleIntervalProperty); }
            set { SetValue(XAxisScaleIntervalProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AxisXScaleInterval.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty XAxisScaleIntervalProperty =
            DependencyProperty.Register("XAxisScaleInterval", typeof(double), typeof(Gram),
                new FrameworkPropertyMetadata(20D, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, XRangeChanged));

        /// <summary>
        /// Y轴刻度之间的间隔
        /// 默认 20，单位：米（m）
        /// </summary>
        public double YAxisScaleInterval
        {
            get { return (double)GetValue(YAxisScaleIntervalProperty); }
            set { SetValue(YAxisScaleIntervalProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AxisYScaleInterval.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty YAxisScaleIntervalProperty =
            DependencyProperty.Register("YAxisScaleInterval", typeof(double), typeof(Gram),
                new FrameworkPropertyMetadata(20D, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, YRangeChanged));

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

        // Using a DependencyProperty as the backing store for NeedXAxisText.  This enables animation, styling, binding, etc...
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

        // Using a DependencyProperty as the backing store for NeedXAxisLine.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NeedXAxisLineProperty =
            DependencyProperty.Register("NeedXAxisLine", typeof(bool), typeof(Gram),
                new FrameworkPropertyMetadata(true, XRangeChanged));

        /// <summary>
        /// X轴刻度线样式
        /// </summary>
        public AxisLineMode XAxisLineMode
        {
            get { return (AxisLineMode)GetValue(XAxisLineModeProperty); }
            set { SetValue(XAxisLineModeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for XAxisLineMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty XAxisLineModeProperty =
            DependencyProperty.Register("XAxisLineMode", typeof(AxisLineMode), typeof(Gram),
                new FrameworkPropertyMetadata(AxisLineMode.Center, XRangeChanged));

        /// <summary>
        /// 需要Y轴刻度文本
        /// </summary>
        public bool NeedYAxisText
        {
            get { return (bool)GetValue(NeedYAxisTextProperty); }
            set { SetValue(NeedYAxisTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NeedYAxisText.  This enables animation, styling, binding, etc...
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

        // Using a DependencyProperty as the backing store for NeedYAxisLine.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NeedYAxisLineProperty =
            DependencyProperty.Register("NeedYAxisLine", typeof(bool),
                typeof(Gram),
                new FrameworkPropertyMetadata(true, YRangeChanged));

        /// <summary>
        /// Y轴刻度线样式
        /// </summary>
        public AxisLineMode YAxisLineMode
        {
            get { return (AxisLineMode)GetValue(YAxisLineModeProperty); }
            set { SetValue(YAxisLineModeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for YAxisLineMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty YAxisLineModeProperty =
            DependencyProperty.Register("YAxisLineMode", typeof(AxisLineMode),
                typeof(Gram),
                new FrameworkPropertyMetadata(AxisLineMode.Center, YRangeChanged));

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

        // Using a DependencyProperty as the backing store for AxisYScaleLocation.  This enables animation, styling, binding, etc...
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

        // Using a DependencyProperty as the backing store for AxisXScaleLocation.  This enables animation, styling, binding, etc...
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

        // Using a DependencyProperty as the backing store for AxisScaleFontSize.  This enables animation, styling, binding, etc...
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

        // Using a DependencyProperty as the backing store for ScaleBrush.  This enables animation, styling, binding, etc...
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

        // Using a DependencyProperty as the backing store for YAxisBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty YAxisBrushProperty =
            DependencyProperty.Register("YAxisBrush", typeof(Brush), typeof(Gram),
                new FrameworkPropertyMetadata(new SolidColorBrush(Color.FromArgb(0xFF, 29, 14, 17)), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, YAxisBrushChanged));



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
        /// 更新绘制坐标轴相关的内容
        /// </summary>
        private void DrawXAxisScale()
        {
            var dc = xAxisVisual.RenderOpen();

            // 显示X轴
            if (NeedXAxisScale)
            {
                UpdatePixelRatio();

                double y1 = 0;
                double y2 = 0;

                switch (XAxisLineMode)
                {
                    case AxisLineMode.Grid:
                        y1 = YAxisConvertYToPixel(YMin);
                        y2 = YAxisConvertYToPixel(YMax);
                        break;
                    case AxisLineMode.TopLeft:
                        y1 = YAxisConvertYToPixel(YMax);
                        y2 = y1 + scalelinePixel;
                        break;
                    case AxisLineMode.BottmRight:
                        y1 = YAxisConvertYToPixel(YMin);
                        y2 = y1 - scalelinePixel;
                        break;
                    case AxisLineMode.Location:
                        y1 = YAxisConvertYToPixel(XAxisLocation);
                        y2 = y1 - scalelinePixel;
                        break;
                    case AxisLineMode.Center:
                        y1 = YAxisConvertYToPixel((YMax + YMin) / 2);
                        break;
                    default:
                        break;
                }

                // 画刻度线条
                if (NeedXAxisLine)
                {
                    for (double i = XMin; i <= XMax; i += XAxisScaleInterval)
                    {
                        var x = XAxisConvertXToPixel(i);

                        switch (XAxisLineMode)
                        {
                            case AxisLineMode.Grid:
                            case AxisLineMode.TopLeft:
                            case AxisLineMode.BottmRight:
                            case AxisLineMode.Location:
                                dc.DrawLine(xAxisPen, new Point(x, y1), new Point(x, y2));
                                break;
                            case AxisLineMode.Center:
                                dc.DrawLine(xAxisPen, new Point(x, y1 - scalelinePixel), new Point(x, y1 + scalelinePixel));
                                break;
                            default:
                                break;
                        }

                    }
                    var x1 = XAxisConvertXToPixel(XMin);
                    var x2 = XAxisConvertXToPixel(XMax);
                    if (XAxisLineMode == AxisLineMode.Grid)
                    {
                        dc.DrawLine(xAxisPen, new Point(x1, y2), new Point(x2, y2));
                    }
                    dc.DrawLine(xAxisPen, new Point(x1, y1), new Point(x2, y1));
                }

                // 是否需要绘制文本
                if (NeedXAxisText)
                {
                    FormattedText text;
                    for (double i = XMin; i <= XMax; i += XAxisScaleInterval)
                    {
                        var x = XAxisConvertXToPixel(i);

                        // 画刻度文本
                        var textcontent = i.ToString();
                        text = new FormattedText(textcontent, CultureInfo.GetCultureInfo("en-us"),
                                                                FlowDirection.LeftToRight, new Typeface("Verdana"), AxisFontSize, XAxisBrush,
                                                                VisualTreeHelper.GetDpi(this).PixelsPerDip);

                        var offsetLeft = text.Width / 2;

                        switch (XAxisLineMode)
                        {
                            case AxisLineMode.Grid:
                                dc.DrawText(text, new Point(x - offsetLeft, y1));
                                dc.DrawText(text, new Point(x - offsetLeft, y2 - (text.Height)));
                                break;
                            case AxisLineMode.TopLeft:
                                dc.DrawText(text, new Point(x - offsetLeft, y1 - (text.Height)));
                                break;
                            case AxisLineMode.BottmRight:
                            case AxisLineMode.Location:
                            case AxisLineMode.Center:
                                dc.DrawText(text, new Point(x - offsetLeft, y1));
                                break;
                            default:
                                break;
                        }

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
        private void DrawYAxisScale()
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

                switch (YAxisLineMode)
                {
                    case AxisLineMode.Grid:
                        // 画满
                        x1 = XAxisConvertXToPixel(XMin);
                        x2 = XAxisConvertXToPixel(XMax);
                        break;
                    case AxisLineMode.TopLeft:
                        // 底部
                        x1 = XAxisConvertXToPixel(XMin);
                        x2 = x1 + scalelinePixel;

                        break;
                    case AxisLineMode.BottmRight:
                        // 顶部
                        x1 = XAxisConvertXToPixel(XMax);
                        x2 = x1 - scalelinePixel;
                        break;

                    case AxisLineMode.Location:
                        // 在定位上画
                        x1 = XAxisConvertXToPixel(YAxisLocation);
                        x2 = x1 + scalelinePixel;
                        break;
                    case AxisLineMode.Center:
                        // 中心
                        x1 = XAxisConvertXToPixel((XMax + XMin) / 2);
                        break;
                    default:
                        break;
                }

                if (NeedYAxisLine)
                {
                    for (double i = YMin; i <= YMax; i += YAxisScaleInterval)
                    {
                        var y = YAxisConvertYToPixel(i);

                        switch (YAxisLineMode)
                        {
                            case AxisLineMode.Grid:
                            case AxisLineMode.TopLeft:
                            case AxisLineMode.BottmRight:
                            case AxisLineMode.Location:
                                dc.DrawLine(yAxisPen, new Point(x1, y), new Point(x2, y));
                                break;
                            case AxisLineMode.Center:
                                dc.DrawLine(yAxisPen, new Point(x1 - scalelinePixel, y), new Point(x1 + scalelinePixel, y));
                                break;
                            default:
                                break;
                        }
                    }

                    // 绘制轴线
                    var y1 = YAxisConvertYToPixel(YMin);
                    var y2 = YAxisConvertYToPixel(YMax);
                    if (YAxisLineMode == AxisLineMode.Grid)
                    {
                        dc.DrawLine(yAxisPen, new Point(x2, y1), new Point(x2, y2));
                    }
                    dc.DrawLine(yAxisPen, new Point(x1, y1), new Point(x1, y2));
                }

                // 是否需要绘制文本
                if (NeedYAxisText)
                {
                    // 文本和线之间的间隔
                    int lineTextGap = 2;

                    FormattedText text;
                    for (double i = YMin; i <= YMax; i += YAxisScaleInterval)
                    {
                        var y = YAxisConvertYToPixel(i);

                        // 画刻度文本
                        var textcontent = i.ToString();
                        text = new FormattedText(textcontent, CultureInfo.GetCultureInfo("en-us"),
                                                                FlowDirection.LeftToRight, new Typeface("Verdana"), AxisFontSize, YAxisBrush,
                                                                VisualTreeHelper.GetDpi(this).PixelsPerDip);

                        switch (YAxisLineMode)
                        {
                            case AxisLineMode.Grid:
                                dc.DrawText(text, new Point(x1 - text.Width - lineTextGap, y - text.Height / 2 - 1));
                                dc.DrawText(text, new Point(x2 + lineTextGap, y - text.Height / 2 - 1));
                                break;
                            case AxisLineMode.TopLeft:
                                dc.DrawText(text, new Point(x1 - text.Width - lineTextGap, y - text.Height / 2 - 1));
                                break;
                            case AxisLineMode.BottmRight:
                                dc.DrawText(text, new Point(x1 + lineTextGap, y - text.Height / 2 - 1));
                                break;
                            case AxisLineMode.Location:
                                dc.DrawText(text, new Point(x1 - text.Width - lineTextGap, y - text.Height / 2 - 1));
                                break;
                            case AxisLineMode.Center:
                                dc.DrawText(text, new Point(x1 - text.Width - lineTextGap, y - text.Height / 2 - 1));
                                break;
                            default:
                                break;
                        }

                    }
                }
            }

            dc.Close();

            // 显示图形
            InvalidateVisual();
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
