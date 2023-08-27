using System;
using System.Windows;
using System.Windows.Media;

namespace WindChart
{
    public class Gram : FrameworkElement
    {
        protected VisualCollection _children;

        public Gram()
        {
            _children = new VisualCollection(this);

            axisVisual = new DrawingVisual();
            _children.Add(axisVisual);
        }

        private DrawingVisual axisVisual;


        /// <summary>
        /// 获取X轴刻度上的文本格式化
        /// </summary>
        protected Func<Double, String> GetXAxisTextFormat = new Func<double, string>(oaDatetime => oaDatetime.ToString());


        /// <summary>
        /// 绘制原点
        /// </summary>
        protected void DrawAxis()
        {
            var drawingContext = axisVisual.RenderOpen();

            Point org = new Point(0, 0);
            ConvertToPixcel(ref org);
            drawingContext.DrawEllipse(Brushes.Black, new Pen(Brushes.OrangeRed, 1), org, 5, 5);

            Pen pen = new Pen(Brushes.Black, 1);
            // X轴
            Point xStart = new Point(XMin, 0);
            Point xEnd = new Point(XMax, 0);
            ConvertToPixcel(ref xStart);
            ConvertToPixcel(ref xEnd);
            drawingContext.DrawLine(pen, xStart, xEnd);
            int interval = (int)(XWidth / 10);
            for (double i = XMin; i <= XMax; i += interval)
            {
                Point xPstart = new Point(i, 0);
                ConvertToPixcel(ref xPstart);
                Point xPend = new Point(xPstart.X, xPstart.Y - 5);
                drawingContext.DrawLine(pen, xPstart, xPend);

                FormattedText text = new FormattedText(GetXAxisTextFormat.Invoke(i),
                    System.Globalization.CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface("Microsoft Yahei"),
                    12,
                    Brushes.Black, VisualTreeHelper.GetDpi(this).PixelsPerDip);

                Point loca = new Point(xPstart.X - text.Width / 2, xPstart.Y);
                drawingContext.DrawText(text, loca);
            }


            // Y轴
            Point yStart = new Point(XMin, YMin);
            Point yEnd = new Point(XMin, YMax);
            ConvertToPixcel(ref yStart);
            ConvertToPixcel(ref yEnd);
            drawingContext.DrawLine(pen, yStart, yEnd);
            interval = (int)(YHeight / 10);
            for (double i = YMin; i <= YMax; i += interval)
            {
                Point yPstart = new Point(XMin, i);
                ConvertToPixcel(ref yPstart);
                Point yPend = new Point(yPstart.X + 5, yPstart.Y);
                drawingContext.DrawLine(pen, yPstart, yPend);

                FormattedText text = new FormattedText(i.ToString(),
                    System.Globalization.CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface("Microsoft Yahei"),
                    12,
                    Brushes.Black, VisualTreeHelper.GetDpi(this).PixelsPerDip);

                Point loca = new Point(yPstart.X - text.Width - 2, yPstart.Y - text.Height / 2);
                drawingContext.DrawText(text, loca);
            }




            drawingContext.Close();
        }

        #region 场景大小

        /// <summary>
        /// Y轴最小值
        /// </summary>
        public double YMin { get; set; } = -100;
        /// <summary>
        /// Y轴最大值
        /// </summary>
        public double YMax { get; set; } = 100;
        /// <summary>
        /// X轴最小值
        /// </summary>
        public double XMin { get; set; } = -100;
        /// <summary>
        /// X轴最大值
        /// </summary>
        public double XMax { get; set; } = 100;

        protected double XWidth => XMax - XMin;
        protected double YHeight => YMax - YMin;

        #endregion

        #region 像素比

        public double xPixelRatio => RenderSize.Width / XWidth;
        public double yPixelRatio => RenderSize.Height / YHeight;

        #endregion

        #region 坐标点转换

        public double ConvertYToPixcel(double y)
        {
            double distY = y - YMin;
            return RenderSize.Height - distY * yPixelRatio;
        }

        public double ConvertXToPixcel(double x)
        {
            double distX = x - XMin;
            return distX * xPixelRatio;
        }

        public Point ConvertToPixcel(ref Point p)
        {
            p.X = ConvertXToPixcel((double)p.X);
            p.Y = ConvertYToPixcel((double)p.Y);
            return p;
        }

        public Point ConvertToPixcel(Point p)
        {
            p.X = ConvertXToPixcel((double)p.X);
            p.Y = ConvertYToPixcel((double)p.Y);
            return p;
        }

        #endregion

        protected override int VisualChildrenCount
        {
            get { return _children.Count; }
        }

        protected override Visual GetVisualChild(int index)
        {
            if (index < 0 || index >= _children.Count)
            {
                throw new ArgumentOutOfRangeException();
            }

            return _children[index];
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            DrawAxis();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            drawingContext.DrawRectangle(Brushes.White, null, new Rect(0, 0, RenderSize.Width, RenderSize.Height));
        }

    }

}
