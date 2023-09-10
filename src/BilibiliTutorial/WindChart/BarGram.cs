using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace WindChart
{

    public class BarGram : Gram
    {
        private DrawingVisual barVisual;

        public BarGram()
        {
            barVisual = new DrawingVisual();
            _children.Add(barVisual);

            XMin = 0;
            YMin = 0;
            YMax = 100;
            XMax = 200;
            NeedXAxisText = false;
            NeedXAxisLine = false;
            DrawAxis();
        }


        public void Draw(List<Bar> bars)
        {
            _bars = bars;
            Draw();
        }


        private List<Bar> _bars = new List<Bar>();

        private void Draw()
        {
            this.Dispatcher.Invoke(() =>
            {
                var dc = barVisual.RenderOpen();

                if (_bars.Count > 0)
                {
                    double barWidth = RenderSize.Width / _bars.Count;
                    double barLocation = 0;

                    for (int i = 0; i < _bars.Count; i++)
                    {
                        var item = _bars[i];

                        var barHeight = ConvertYToPixcel(item.Value);

                        dc.DrawRectangle(item.Fill, null, new System.Windows.Rect(barLocation, barHeight, barWidth, RenderSize.Height - barHeight));


                        FormattedText text = new FormattedText(item.Value.ToString(),
                                  System.Globalization.CultureInfo.CurrentCulture,
                                 System.Windows.FlowDirection.LeftToRight,
                                  new Typeface("Microsoft Yahei"),
                                  12,
                                 item.Fill, VisualTreeHelper.GetDpi(this).PixelsPerDip);

                        var x = barLocation + barWidth / 2 - text.Width / 2;
                        dc.DrawText(text, new System.Windows.Point(x, barHeight - text.Height));




                        FormattedText label = new FormattedText(item.Label.ToString(),
                                System.Globalization.CultureInfo.CurrentCulture,
                               System.Windows.FlowDirection.LeftToRight,
                                new Typeface("Microsoft Yahei"),
                                12,
                               Brushes.Black, VisualTreeHelper.GetDpi(this).PixelsPerDip);

                        x = barLocation + barWidth / 2 - label.Width / 2;
                        dc.DrawText(label, new System.Windows.Point(x, RenderSize.Height));



                        barLocation += barWidth;
                    }
                }

                dc.Close();
            });
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            Draw();
        }

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
