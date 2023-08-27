---
typora-root-url: res
---

# <img src="/windchart.png" style="zoom:30%;" />Toolkit-WeChart-WindChart
构建自定义&简洁的 Windows Presentation Foundation (WPF) 图表库。

Build Custom and Simple WPF Charts  Library



## 介绍

基于FrameworkElement，使用DrawingVisual实现的WPF 图表控件库

源码地址：[mefdeamon/Toolkit-WeChart: Build Custom and Simple WPF Charts Library (github.com)](https://github.com/mefdeamon/Toolkit-WeChart)

仓库名：Toolkit.WeChart

类库名：WindChart



## 目的

分享WindChart设计开发过程。



## Lesson1 浅识DrawingVisual

微软官网示例：[使用 DrawingVisual 对象 - WPF .NET Framework | Microsoft Docs](https://docs.microsoft.com/zh-cn/dotnet/desktop/wpf/graphics-multimedia/using-drawingvisual-objects?view=netframeworkdesktop-4.8)

值得关注的重要类和方法

- DrawingVisual
  - RenderOpen
- DrawingContext
  - DrawRectangle
    - Brush
    - Pen
  - DrawLine
  - DrawEllipse
  - DrawText
    - FormattedText
  - DrawRoundedRectangle
  - DrawGeometry
    - Geometry
  - DrawImage
    - ImageSource
  - ...
  - PushOpacity
  - PushTransform
  - ..
  - Pop
  - Close
- FrameworkElement
  - VisualChildrenCount
  - GetVisualChild
  - ...  



## Lesson2 建立坐标系

**坐标系**

*Windows图像坐标系*

在Windows中，图像坐标系的原点在左上角，像素没有负数，坐标以水平向右为X轴的正方向，垂直向下为Y轴正方向。

![](/axis.png)

*数学坐标系*

我们在数学应用中常用的二维坐标系是：水平向右为X轴的正方向，垂直向上为Y轴的正方向。这个坐标系也是我们一般可视化绘图的坐标系。

<img src="mathaxis.png" style="zoom:100%;" />

**坐标范围**

绘图时需要将实际场景（例如X Y）映射到窗口的绘图区域（例如长高）。因此需要明确场景的大小以及绘图区域的大小，为了便于开发，这里通过定义实际场景的大小和获取控件绘图区域的大小来控制坐标的范围。绘图区域的大小可以通过RenderSize中的Width和Height获取，实际场景的坐标范围定义如下：

```c#
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
        /// <summary>
        /// 实际场景的绝对宽度
        /// </summary>
        protected double XWidth => XMax - XMin;
        /// <summary>
        /// 实际场景的绝对高度
        /// </summary>
        protected double YHeight => YMax - YMin;
```

**像素比**

1单位数据对应多少像素。像素与实际的比例：例如控件长高的像素值分别是：200 Pixel * 150 Pixel。实际长高值分别是20m * 30m。那么长代表的1m就对应界面上200/20=10个像素，这就是长（X轴/水平方向）的像素比。同样，高的像素比就等于150/30=5，1m对应界面上5个像素。由此可以得出如下公式：

```c#
        /// <summary>
        /// X
        /// 水平方向
        /// 像素比
        /// 1单位数据对应多少像素
        /// 根据实际场景宽度和控件宽度得到
        /// </summary>
        protected double xPixelRatio => RenderSize.Width / XWidth;
        /// <summary>
        /// Y
        /// 垂直方向
        /// 像素比
        /// 1单位数据对应多少像素
        /// 根据实际场景长(高)度和控件高度得到
        /// </summary>
        protected double yPixelRatio => RenderSize.Height / YHeight;
```

**坐标转换**

将实际坐标点转换成图像坐标。

```c#
        /// <summary>
        /// 转换实际坐标的Y点为图像坐标值
        /// </summary>
        /// <param name="y"></param>
        /// <returns></returns>
        protected double ConvertYToPixel(double y)
        {
            var distY = y - YMin;
            return RenderSize.Height - distY * yPixelRatio;
        }

        /// <summary>
        /// 转换实际坐标的X点为图像坐标值
        /// 水平坐标轴（X轴）值转换，X
        /// 将实际值转换成像素
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        protected double ConvertXToPixel(double x)
        {
            var distX = x - XMin;
            return distX * xPixelRatio;
        }

        /// <summary>
        /// 坐标点转换
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        protected Point ConvertToPixel(Point p)
        {
            p.X = ConvertXToPixel(p.X);
            p.Y = ConvertYToPixel(p.Y);
            return p;
        }
```



## Lesson3 绘制坐标系

坐标系上的刻度线和刻度文本绘制

```c#
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
            double interval = XWidth / 10;
            for (double i = XMin; i <= XMax; i += interval)
            {
                Point xPstart = new Point(i, 0);
                ConvertToPixcel(ref xPstart);
                Point xPend = new Point(xPstart.X, xPstart.Y - 5);
                drawingContext.DrawLine(pen, xPstart, xPend);

                FormattedText text = new FormattedText(i.ToString(),
                    System.Globalization.CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface("Microsoft Yahei"),
                    12,
                    Brushes.Black, VisualTreeHelper.GetDpi(this).PixelsPerDip);

                Point loca = new Point(xPstart.X - text.Width / 2, xPstart.Y);
                drawingContext.DrawText(text, loca);
            }

            // Y轴
            Point yStart = new Point(0, YMin);
            Point yEnd = new Point(0, YMax);
            ConvertToPixcel(ref yStart);
            ConvertToPixcel(ref yEnd);
            drawingContext.DrawLine(pen, yStart, yEnd);
            interval = YHeight / 10;
            for (double i = YMin; i <= YMax; i += interval)
            {
                Point yPstart = new Point(0, i);
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

```



## Lesson4 绘制单线图1

尝试绘制单线图

```c#
public class LineGram : Gram
    {
        private DrawingVisual lineVisual;

        public LineGram()
        {
            lineVisual = new DrawingVisual();
            _children.Add(lineVisual);
            pen = new Pen(Brushes.Black, 1);
            pen.Freeze();

            XMax = 1000;
            DrawAxis();
        }


        private List<Point> Points = new List<Point>();

        private Pen pen = new Pen();

        public void InitLine(Brush lineBrus, double lineThickness = 1)
        {
            Points.Clear();
            pen = new Pen(lineBrus, lineThickness);
            pen.Freeze();
        }

        public void DrawLine(Point p)
        {
            Points.Add(p);

            var drawingContext = lineVisual.RenderOpen();

            if (Points.Count > 1)
            {
                for (int i = 1; i < Points.Count; i++)
                {
                    Point p0 = ConvertToPixcel(Points[i - 1]);
                    Point p1 = ConvertToPixcel(Points[i]);

                    drawingContext.DrawLine(pen, p0, p1);
                }
            }
            drawingContext.Close();
        }
    }
```



## Lesson5 绘制单线图2

单线面积图

- PathGeometry的构建
  - Figures(PathFigureCollection)
    - PathFigure
      - IsClosed
      - StartPoint
      - Segments（PathSegmentCollection）
        - LineSegment
          - Point
- DrawingGemotry

[如何：使用 PathGeometry 创建形状 - WPF .NET Framework | Microsoft Learn](https://learn.microsoft.com/zh-cn/dotnet/desktop/wpf/graphics-multimedia/how-to-create-a-shape-by-using-a-pathgeometry?view=netframeworkdesktop-4.8)



```c#
	public class LineGram : Gram
    {
        private DrawingVisual lineVisual;

        public LineGram()
        {
            lineVisual = new DrawingVisual();
            _children.Add(lineVisual);
            pen = new Pen(Brushes.Black, 1);
            pen.Freeze();

            XMax = 1000;
            DrawAxis();

            var fillBrush = new LinearGradientBrush() { StartPoint = new Point(0, 0), EndPoint = new Point(0, 1) };
            fillBrush.GradientStops = new GradientStopCollection();
            fillBrush.GradientStops.Add(new GradientStop() { Color = Brushes.CornflowerBlue.Color, Offset = 0 });
            fillBrush.GradientStops.Add(new GradientStop() { Color = Brushes.Transparent.Color, Offset = 1 });
            FillBrush = fillBrush;
        }

        public Brush FillBrush { get; set; }

        public Boolean IsGraph { get; set; } = true;


        private List<Point> Points = new List<Point>();

        private Pen pen = new Pen();

        public void InitLine(Brush lineBrus, double lineThickness = 1)
        {
            Points.Clear();
            pen = new Pen(lineBrus, lineThickness);
            pen.Freeze();
        }

        public void DrawLine(Point p)
        {
            Points.Add(p);

            var drawingContext = lineVisual.RenderOpen();

            if (Points.Count > 1)
            {

                if (IsGraph)
                {
                    PathGeometry pathGeometry = new PathGeometry();
                    pathGeometry.Figures = new PathFigureCollection();


                    var pathFigure = new PathFigure() { IsClosed = true, StartPoint = ConvertToPixcel(new Point(Points[0].X, YMin)) };
                    pathFigure.Segments = new PathSegmentCollection();

                    for (int i = 0; i < Points.Count; i++)
                    {
                        Point p1 = ConvertToPixcel(Points[i]);

                        pathFigure.Segments.Add(new LineSegment() { Point = p1 });
                    }

                    pathFigure.Segments.Add(new LineSegment() { Point = ConvertToPixcel(new Point(Points[Points.Count - 1].X, YMin)) });
                    pathGeometry.Figures.Add(pathFigure);

                    drawingContext.DrawGeometry(FillBrush, null, pathGeometry);
                }

                for (int i = 1; i < Points.Count; i++)
                {
                    Point p0 = ConvertToPixcel(Points[i - 1]);
                    Point p1 = ConvertToPixcel(Points[i]);

                    drawingContext.DrawLine(pen, p0, p1);
                }
            }
            drawingContext.Close();
        }
    }
```





## Lesson6 绘制单线图3

依赖属性/数据绑定

[依赖属性概述 - WPF .NET | Microsoft Learn](https://learn.microsoft.com/zh-cn/dotnet/desktop/wpf/properties/dependency-properties-overview?view=netdesktop-6.0)

为什么要引入依赖属性（支持MVVM数据绑定）

是否显示面积图

线条资源（ObservableCollection）

[ObservableCollection 类 (System.Collections.ObjectModel) | Microsoft Learn](https://learn.microsoft.com/zh-cn/dotnet/api/system.collections.objectmodel.observablecollection-1?view=net-7.0)



```c#

    public class LineGram : Gram
    {
        private DrawingVisual lineVisual;

        public LineGram()
        {
            lineVisual = new DrawingVisual();
            _children.Add(lineVisual);
            pen = new Pen(Brushes.Black, 1);
            pen.Freeze();

            XMax = 1000;
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

        private void Draw()
        {

            var drawingContext = lineVisual.RenderOpen();

            if (LineSource.Count > 1)
            {

                if (IsGraph)
                {
                    PathGeometry pathGeometry = new PathGeometry();
                    pathGeometry.Figures = new PathFigureCollection();


                    var pathFigure = new PathFigure() { IsClosed = true, StartPoint = ConvertToPixcel(new Point(LineSource[0].X, YMin)) };
                    pathFigure.Segments = new PathSegmentCollection();

                    for (int i = 0; i < LineSource.Count; i++)
                    {
                        Point p1 = ConvertToPixcel(LineSource[i]);

                        pathFigure.Segments.Add(new LineSegment() { Point = p1 });
                    }

                    pathFigure.Segments.Add(new LineSegment() { Point = ConvertToPixcel(new Point(LineSource[LineSource.Count - 1].X, YMin)) });
                    pathGeometry.Figures.Add(pathFigure);

                    drawingContext.DrawGeometry(FillBrush, null, pathGeometry);
                }

                for (int i = 1; i < LineSource.Count; i++)
                {
                    Point p0 = ConvertToPixcel(LineSource[i - 1]);
                    Point p1 = ConvertToPixcel(LineSource[i]);

                    drawingContext.DrawLine(pen, p0, p1);
                }
            }
            drawingContext.Close();
        }

    }
```



## Lesson7 绘制单线图4

鼠标选点显示

鼠标按钮事件重写

- **鼠标左键按下**绘制最近点的信息，
- **鼠标移动**时如果鼠标左键按下状态绘制最近点的信息，

- **鼠标右键按下**清空



像素坐标转实际坐标函数

```c#
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


```

查找最近的点

```c#
        Point FindNearPoint(Point poi)
        {
            var p2 = ConvertToActual(poi);
            var xx = points.Select(t => t.X).OrderBy(x => Math.Abs(x - p2.X)).First();

            var point = points.Where(t => t.X == xx).FirstOrDefault();
            return point;
        }
```

完整代码

```c#
 
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

            XMax = 1000;
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

        private void Draw()
        {

            var drawingContext = lineVisual.RenderOpen();

            if (LineSource.Count > 1)
            {

                if (IsGraph)
                {
                    PathGeometry pathGeometry = new PathGeometry();
                    pathGeometry.Figures = new PathFigureCollection();


                    var pathFigure = new PathFigure() { IsClosed = true, StartPoint = ConvertToPixcel(new Point(LineSource[0].X, YMin)) };
                    pathFigure.Segments = new PathSegmentCollection();

                    for (int i = 0; i < LineSource.Count; i++)
                    {
                        Point p1 = ConvertToPixcel(LineSource[i]);

                        pathFigure.Segments.Add(new LineSegment() { Point = p1 });
                    }

                    pathFigure.Segments.Add(new LineSegment() { Point = ConvertToPixcel(new Point(LineSource[LineSource.Count - 1].X, YMin)) });
                    pathGeometry.Figures.Add(pathFigure);

                    drawingContext.DrawGeometry(FillBrush, null, pathGeometry);
                }

                for (int i = 1; i < LineSource.Count; i++)
                {
                    Point p0 = ConvertToPixcel(LineSource[i - 1]);
                    Point p1 = ConvertToPixcel(LineSource[i]);

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
```





## Lesson8 绘制单线图5

保留所有点

- 更新绘图范围











## Lesson9 绘制单线图6

滚屏刷新

- 设置滚屏范围




```C#
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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


        public Boolean IsFlashRange { get; set; } = false;

        public int FlashRangePointCount { get; set; } = 200;

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

```

 



## Lesson10 绘制单线图7

刻度文本日期格式化

[String.Format 方法 (System) | Microsoft Learn](https://learn.microsoft.com/zh-cn/dotnet/api/system.string.format?view=net-7.0)

[DateTime.FromOADate(Double) 方法 (System) | Microsoft Learn](https://learn.microsoft.com/zh-cn/dotnet/api/system.datetime.fromoadate?view=net-7.0)

格式化字符串

日期与OLE Automation (double)日期的转换

```c#
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
```



```c#

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


```





## Lesson11 单线图应用

NuGet搜索**WindChart**

- 在后台代码直接使用

  MainWindow.xaml

  ```xaml
  <Window x:Class="WindChart.Lesson11.MainWindow"
          xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
          xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
          xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
          xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
          xmlns:local="clr-namespace:WindChart.Lesson11"
          xmlns:windchart="clr-namespace:WindChart;assembly=WindChart"
          mc:Ignorable="d"
          Title="MainWindow" Height="450" Width="800">
      <Grid>
          <Border Padding="24">
              <windchart:Linegram x:Name="line" />
          </Border>
      </Grid>
  </Window>
  
  ```

  - 一次性更新

    ```C#
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Navigation;
    using System.Windows.Shapes;
    
    namespace WindChart.Lesson11
    {
        /// <summary>
        /// Interaction logic for MainWindow.xaml
        /// </summary>
        public partial class MainWindow : Window
        {
            public MainWindow()
            {
                InitializeComponent();
    
                // 设置X轴开始显示范围
                line.XMin = DateTime.Now.ToOADate();
                line.XMax = DateTime.Now.AddSeconds(60).ToOADate();
                // 设置X轴刻度文本格式
                line.XAxisTextFormatString = "HH: mm:ss";
                // X轴刻度按照日期显示
                line.IsXAxisTextDateTimeFormat = true;
                line.IsGraph = false;
    
                // 轴跟随数据变化
                line.IsAxisFollowData = true;
                // Y轴范围
                line.YMin = 0;
                line.YMax = 80;
    
                // 实时数据模拟
                Task.Run(() =>
                {
                    Random rand = new Random();
                    double y = 10;
                    while (true)
                    {
                        var ran = rand.Next(-150, 150);
                        if (ran > 0)
                        {
                            y -= 1;
                        }
                        else
                        {
                            y += 1;
                        }
                        this.Dispatcher.Invoke(() =>
                        {
                            line.Add(new Point(DateTime.Now.ToOADate(), y));
                        });
                        Thread.Sleep(30);
                    }
                });
            }
        }
    }
    
    ```

  - 单点实时更新

    MainWindow.xaml.cs

    ```c#
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Navigation;
    using System.Windows.Shapes;
    
    namespace WindChart.Lesson11
    {
        /// <summary>
        /// Interaction logic for MainWindow.xaml
        /// </summary>
        public partial class MainWindow : Window
        {
            public MainWindow()
            {
                InitializeComponent();
    
                // 设置X轴开始显示范围
                DateTime dtStart = DateTime.Now.AddMinutes(-3);
                line.XMin = dtStart.ToOADate();
                line.XMax = DateTime.Now.AddSeconds(60).ToOADate();
                // 设置X轴刻度文本格式
                line.XAxisTextFormatString = "HH:mm:ss";
                // X轴刻度按照日期显示
                line.IsXAxisTextDateTimeFormat = true;
    
                // 轴跟随数据变化
                line.IsAxisFollowData = true;
                // Y轴范围
                line.YMin = 0;
                line.YMax = 80;
    
                // 数据模拟
                Random rand = new Random();
                List<Point> points = new List<Point>();
                double y = 10;
                while (dtStart < DateTime.Now)
                {
                    var ran = rand.Next(-150, 150);
                    if (ran > 0)
                    {
                        y -= 1;
                    }
                    else
                    {
                        y += 1;
                    }
                    points.Add(new Point(dtStart.ToOADate(), y));
                    dtStart = dtStart.AddMilliseconds(500);
                }
                line.DrawLine(points);
            }
    
        }
    }
    ```

- MVVM绑定

  MainWindow.xaml

  ```c#
  <Window x:Class="WindChart.Lesson11.MainWindow"
          xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
          xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
          xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
          xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
          xmlns:local="clr-namespace:WindChart.Lesson11"
          xmlns:windchart="clr-namespace:WindChart;assembly=WindChart"
          mc:Ignorable="d"
          Title="MainWindow" Height="450" Width="800">
      <Window.Resources>
          <local:MainViewModel x:Key="MainVm"/>
      </Window.Resources>
      <Grid DataContext="{Binding Source={StaticResource MainVm}}">
          <Border Padding="24">
              <windchart:Linegram x:Name="line" XMin="0" XMax="500" YMin="0"
                                  XAxisTextFormatString="yyyy-MM-dd"
                                  IsAxisFollowData="True"
                                  IsXAxisTextDateTimeFormat="True"
                                  LineSource="{Binding LinePoints}"
                                  YMax="80" IsGraph="True" />
          </Border>
      </Grid>
  </Window>
  ```

  MainViewModel.cs

  ```c#
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.ComponentModel;
  using System.Threading;
  using System.Threading.Tasks;
  using System.Windows;
  
  namespace WindChart.Lesson11
  {
      public class MainViewModel : INotifyPropertyChanged
      {
          public event PropertyChangedEventHandler PropertyChanged;
  
          public MainViewModel()
          {
              LinePoints = new ObservableCollection<Point>();
  
  
              // 模拟
              Task.Delay(1000).ContinueWith((Action<Task>)(t =>
              {
  
                  Application.Current.Dispatcher.Invoke(() =>
                  {
                      LinePoints = new ObservableCollection<Point>();
                  });
  
                  double x = 0;
                  double y = 0;
  
                  Random random = new Random();
  
                  y = 50;
  
  
                  //  while (true)
                  {
                      List<Point> points = new List<Point>();
                      while (true)
                      {
  
                          Application.Current.Dispatcher.Invoke((Action)(() =>
                          {
                              points.Add(new Point(x, y));
                          }));
  
                          x++;
                          y = random.Next() % 2 == 0 ? ++y : --y;
  
                          Application.Current.Dispatcher.Invoke(() =>
                          {
                              LinePoints = new ObservableCollection<Point>(points);
                          });
  
                          Thread.Sleep(20);
  
                      }
                      Application.Current.Dispatcher.Invoke(() =>
                      {
                          LinePoints = new ObservableCollection<Point>(points);
                          x = 0;
                          y = 50;
                      });
  
                      Thread.Sleep(2000);
                  }
              }));
          }
  
          private Boolean isGraph;
  
          public Boolean IsGraph
          {
              get { return isGraph; }
              set
              {
                  isGraph = value;
                  PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsGraph)));
              }
          }
  
          private ObservableCollection<Point> linePoints;
  
          public ObservableCollection<Point> LinePoints
          {
              get { return linePoints; }
              set
              {
                  linePoints = value;
                  PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LinePoints)));
              }
          }
      }
  }
  ```

  















































Lesson



















