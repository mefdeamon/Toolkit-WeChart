using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace WindChart
{
    public class LineGram : Gram
    {
        private DrawingVisual lineVisual;
        private DrawingVisual mouseVisual;

        public LineGram()
        {
            lineVisual = new DrawingVisual();
            _children.Add(lineVisual);
            mouseVisual = new DrawingVisual();
            _children.Add(mouseVisual);

            pen = new Pen(Brushes.Black, 1);
            pen.Freeze();
            XMin = 0;
            YMin = 0;
            YMax = 100;
            XMax = 200;
            DrawAxis();

            var fillBrush = new LinearGradientBrush() { StartPoint = new Point(0, 0), EndPoint = new Point(0, 1) };
            fillBrush.GradientStops = new GradientStopCollection();
            fillBrush.GradientStops.Add(new GradientStop() { Color = Brushes.CornflowerBlue.Color, Offset = 0 });
            fillBrush.GradientStops.Add(new GradientStop() { Color = Brushes.Transparent.Color, Offset = 1 });
            FillBrush = fillBrush;
        }

        public Brush FillBrush { get; set; }


        public Boolean IsGraph
        {
            get { return (Boolean)GetValue(IsGraphProperty); }
            set { SetValue(IsGraphProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsGraph.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsGraphProperty =
            DependencyProperty.Register("IsGraph", typeof(Boolean), typeof(LineGram),
                new PropertyMetadata(default(Boolean), new PropertyChangedCallback((d, e) =>
                {
                    if (d is LineGram gram)
                    {
                        gram.Draw();
                    }
                })));



        public ObservableCollection<Point> LineSource
        {
            get { return (ObservableCollection<Point>)GetValue(LineSourceProperty); }
            set { SetValue(LineSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LineSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LineSourceProperty =
            DependencyProperty.Register("LineSource", typeof(ObservableCollection<Point>), typeof(LineGram),
                new PropertyMetadata(default(ObservableCollection<Point>), new PropertyChangedCallback(LineSourceChanged)));

        private static void LineSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is LineGram gram)
            {
                gram.Draw();
            }
        }


        private Pen pen = new Pen();

        public void InitLine(Brush lineBrus, double lineThickness = 1)
        {
            //Points.Clear();
            pen = new Pen(lineBrus, lineThickness);
            pen.Freeze();
        }

        public void DrawLine(Point p)
        {
            LineSource.Add(p);
            Draw();
        }


        private void UpdateAxis()
        {
            var xs = LineSource.Select(x => x.X);
            var xmin = xs.Min();
            var xmax = xs.Max();

            var ys = LineSource.Select(y => y.Y);
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
                if (LineSource.Count > FlashRangePointCount)
                {
                    var list = LineSource.Skip(LineSource.Count - FlashRangePointCount).ToList();
                    XMin = list.Select(x => x.X).Min();
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

            DrawAxis();
        }


        public Boolean IsFlashRange { get; set; } = true;

        public int FlashRangePointCount { get; set; } = 200;



        public Boolean IsXAxisTextDateTimeForamt
        {
            get { return (Boolean)GetValue(IsXAxisTextDateTimeForamtProperty); }
            set { SetValue(IsXAxisTextDateTimeForamtProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsXAxisTextDateTimeForamt.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsXAxisTextDateTimeForamtProperty =
            DependencyProperty.Register("IsXAxisTextDateTimeForamt", typeof(Boolean), typeof(LineGram), new PropertyMetadata(false, new PropertyChangedCallback((d, e) =>
             {
                 if (d is LineGram lineGram)
                 {
                     if (e.NewValue is Boolean need)
                     {
                         if (need)
                         {
                             lineGram.SetDateTimeFormat();
                         }
                         else
                         { lineGram.SetNumericFormat(); }
                     }
                 }
             })));


        public String XAxisTextFormatString
        {
            get { return (String)GetValue(XAxisTextFormatStringProperty); }
            set { SetValue(XAxisTextFormatStringProperty, value); }
        }
        public static readonly DependencyProperty XAxisTextFormatStringProperty =
            DependencyProperty.Register("XAxisTextFormatString", typeof(String), typeof(LineGram), new PropertyMetadata(String.Empty));




        private void SetDateTimeFormat()
        {
            XAxisTextFormatString = "{0:yyyy-MM-dd}";
            GetXAxisTextFormat = new Func<double, string>(oaDate =>
            {
                DateTime dt = DateTime.FromOADate(oaDate);
                return String.Format(XAxisTextFormatString, dt);
            });
        }

        private void SetNumericFormat()
        {
            GetXAxisTextFormat = new Func<double, string>(oaDate =>
            {
                return Math.Round(oaDate, 10).ToString();
            });
        }




        List<Point> points = new List<Point>();

        private void Draw()
        {
            var drawingContext = lineVisual.RenderOpen();

            points = LineSource.ToList();
            if (IsFlashRange)
            {
                if (LineSource.Count > FlashRangePointCount)
                {
                    points = LineSource.Skip(LineSource.Count - FlashRangePointCount).ToList();
                }
            }


            if (points.Count > 1)
            {

                UpdateAxis();

                if (IsGraph)
                {
                    PathGeometry pathGeometry = new PathGeometry();
                    pathGeometry.Figures = new PathFigureCollection();


                    var pathFigure = new PathFigure() { IsClosed = true, StartPoint = ConvertToPixcel(new Point(points[0].X, YMin)) };
                    pathFigure.Segments = new PathSegmentCollection();

                    for (int i = 0; i < points.Count; i++)
                    {
                        Point p1 = ConvertToPixcel(points[i]);
                        pathFigure.Segments.Add(new LineSegment() { Point = p1 });
                    }

                    pathFigure.Segments.Add(new LineSegment() { Point = ConvertToPixcel(new Point(points[points.Count - 1].X, YMin)) });
                    pathGeometry.Figures.Add(pathFigure);

                    drawingContext.DrawGeometry(FillBrush, null, pathGeometry);
                }

                for (int i = 1; i < points.Count; i++)
                {
                    Point p0 = ConvertToPixcel(points[i - 1]);
                    Point p1 = ConvertToPixcel(points[i]);

                    drawingContext.DrawLine(pen, p0, p1);
                }
            }
            drawingContext.Close();
        }


        #region 鼠标事件

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
                DrawMouse(e.GetPosition(this));
            }
        }

        protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseRightButtonDown(e);

            var drawingContext = mouseVisual.RenderOpen();
            drawingContext.Close();

        }


        private void DrawMouse(Point location)
        {

            var drawingContext = mouseVisual.RenderOpen();

            if (LineSource.Count > 1)
            {

                var actPoint = FindNearPoint(location);

                location = ConvertToPixcel(actPoint);

                drawingContext.DrawEllipse(Brushes.Red, null, location, 5, 5);
                //drawingContext.DrawRectangle(Brushes.Red, null, new Rect(0, location.Y, RenderSize.Width, 1));
                //drawingContext.DrawRectangle(Brushes.Red, null, new Rect(location.X, 0, 1, RenderSize.Height));
                drawingContext.DrawLine(new Pen(Brushes.Purple, 1), new Point(0, location.Y), new Point(RenderSize.Width, location.Y));
                drawingContext.DrawLine(new Pen(Brushes.Purple, 1), new Point(location.X, 0), new Point(location.X, RenderSize.Height));

                var content = $"({actPoint.X},{actPoint.Y})";
                FormattedText text = new FormattedText(content,
                   System.Globalization.CultureInfo.CurrentCulture,
                   FlowDirection.LeftToRight,
                   new Typeface("Microsoft Yahei"),
                   12,
                   Brushes.Black, VisualTreeHelper.GetDpi(this).PixelsPerDip);
                drawingContext.DrawRectangle(Brushes.White, null, new Rect(new Point(location.X + text.Height, location.Y + text.Height), new Size(text.Width, text.Height)));
                drawingContext.DrawText(text, new Point(location.X + text.Height, location.Y + text.Height));


            }
            drawingContext.Close();
        }

        Point FindNearPoint(Point poi)
        {
            var p2 = ConvertToActual(poi);
            var xx = LineSource.Select(t => t.X).OrderBy(x => Math.Abs(x - p2.X)).First();

            var point = LineSource.Where(t => t.X == xx).FirstOrDefault();
            return point;
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

    }
}
