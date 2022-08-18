using System;
using System.Collections.Generic;
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

            linePen = new Pen(YAxisBrush, LineThickness);
            linePen.Freeze();

            // 初始化画板刻度信息
            YAxisLineMode = AxisLineMode.TopLeft;
            XAxisLineMode = AxisLineMode.BottmRight;
            XMin = 0;
            YMin = 0;
        }

        /// <summary>
        /// 线条宽度
        /// </summary>
        public double LineThickness { get; set; } = 1;

        public void Init(double xMin = 0, double xMax = 200, double yMin = -50, double yMax = 50)
        {
            linePen = new Pen(YAxisBrush, LineThickness);
            linePen.Freeze();

            points.Clear();

            XMin = xMin;
            XMax = xMax;

            YMin = yMin;
            YMax = yMax;

            maxY = YMax;
            minY = YMin;

            Draw();
        }



        List<Point> points = new List<Point>();
        /// <summary>
        /// 界面显示的点数
        /// </summary>
        private int xMaxPoint = 200;
        /// <summary>
        /// X跳跃距离
        /// </summary>
        int xgap = 20;


        double maxY = 0;
        double minY = 0;


        /// <summary>
        /// 当X轴满了的时候，往左边移动一段距离，然后等满了再移动
        /// </summary>
        /// <param name="p"></param>
        public void DrawLine2(Point p)
        {
            // 确定X轴的范围
            if (points.Count > xMaxPoint)
            {
                points.RemoveAt(0);
                XMin = points[0].X;
                XMax = points[0].X + xMaxPoint + xgap;
            }
            if (p.X > XMax)
            {
                XMax = p.X + xMaxPoint + xgap;
            }
            if (points.Count < 1)
            {
                XMin = p.X;
                XMax = XMin + xMaxPoint + xgap + xgap;
            }

            // 确定Y轴范围
            if (p.Y > maxY)
            {
                maxY = p.Y + xgap;
                YMax = maxY;
            }
            if (p.Y < minY)
            {
                minY = p.Y - xgap;
                YMin = minY;
            }

            points.Add(p);

            Draw();
        }

        /// <summary>
        /// 当X轴数据满了，不变化XMIN
        /// </summary>
        /// <param name="p"></param>
        public void DrawLine3(Point p)
        {
            // 确定X轴的范围
            if (p.X > XMax)
            {
                XMax = p.X + xMaxPoint + xgap;
            }
            if (points.Count < 1)
            {
                XMin = p.X;
                XMax = XMin + xMaxPoint + xgap + xgap;
            }

            // 确定Y轴范围
            if (p.Y > maxY)
            {
                maxY = p.Y + xgap;
                YMax = maxY;
            }
            if (p.Y < minY)
            {
                minY = p.Y - xgap;
                YMin = minY;
            }

            points.Add(p);

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

            YMax = maxY + xgap;
            YMin = minY - xgap;

            XMax = maxX + xgap;
            XMin = minX;

            Draw();

            //
            //points.Clear();
        }

        /// <summary>
        /// 当X轴满了的时候，往左边移动一步
        /// </summary>
        /// <param name="p"></param>
        public void DrawLine(Point p)
        {
            // 确定X轴的范围
            if (points.Count > xMaxPoint)
            {
                points.RemoveRange(0, xgap);
                XMin = points[0].X;
                XMax = points[0].X + xMaxPoint + xgap + xgap;
            }
            if (p.X > XMax)
            {
                XMax = p.X + xMaxPoint + xgap;
            }
            if (points.Count < 1)
            {
                XMin = p.X;
                XMax = XMin + xMaxPoint + xgap + xgap;
            }

            // 确定Y轴范围
            if (p.Y > maxY)
            {
                maxY = p.Y + xgap;
                YMax = maxY;
            }
            if (p.Y < minY)
            {
                minY = p.Y - xgap;
                YMin = minY;
            }

            points.Add(p);

            Draw();
        }

        /// <summary>
        /// 绘图
        /// </summary>
        private void Draw()
        {
            var dc = lineVisual.RenderOpen();

            if (points.Count > 0)
            {
                var last = ConvertToPixel(points[0]);
                for (int i = 0; i < points.Count; i++)
                {
                    var location = ConvertToPixel(points[i]);
                    dc.DrawLine(linePen, last, location);
                    last = location;
                }
            }

            dc.Close();

            // 显示图形
            InvalidateVisual();
        }

        #region override

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            Draw();
            base.OnRenderSizeChanged(sizeInfo);
        }
        #endregion
    }

}
