using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace WindChart
{
    /// <summary>
    /// ��������ͼ
    /// </summary>
    public class Linegram : Gram
    {
        private DrawingVisual lineVisual;

        public Linegram()
        {
            lineVisual = new DrawingVisual();
            Visuals.Add(lineVisual);

            linePen = new Pen(YAxisBrush, LineThickness);
            linePen.Freeze();

            YAxisLineMode = AxisLineMode.TopLeft;
            XAxisLineMode = AxisLineMode.BottmRight;
            XMin = 0;
            YMin = 0;
        }

        /// <summary>
        /// �������
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

            ClearLine();
        }

        public void ClearLine()
        {
            // ���
            var dc = lineVisual.RenderOpen();

            dc.Close();

            // ��ʾͼ��
            InvalidateVisual();
        }

        /// <summary>
        /// ������ɫ�ʹ�ϸ
        /// </summary>
        Pen linePen;

        List<Point> points = new List<Point>();
        /// <summary>
        /// ������ʾ�ĵ���
        /// </summary>
        private int xMaxPoint = 200;
        /// <summary>
        /// X��Ծ����
        /// </summary>
        int xgap = 20;


        double maxY = 0;
        double minY = 0;


        /// <summary>
        /// ��X�����˵�ʱ��������ƶ�һ�ξ��룬Ȼ����������ƶ�
        /// </summary>
        /// <param name="p"></param>
        public void DrawLine2(Point p)
        {
            // ȷ��X��ķ�Χ
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

            // ȷ��Y�᷶Χ
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
        /// ��X���������ˣ����仯XMIN
        /// </summary>
        /// <param name="p"></param>
        public void DrawLine3(Point p)
        {
            // ȷ��X��ķ�Χ
            if (p.X > XMax)
            {
                XMax = p.X + xMaxPoint + xgap;
            }
            if (points.Count < 1)
            {
                XMin = p.X;
                XMax = XMin + xMaxPoint + xgap + xgap;
            }

            // ȷ��Y�᷶Χ
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
        /// ֱ��һ�°����ݻ���
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
        /// ��X�����˵�ʱ��������ƶ�һ��
        /// </summary>
        /// <param name="p"></param>
        public void DrawLine(Point p)
        {
            // ȷ��X��ķ�Χ
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

            // ȷ��Y�᷶Χ
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
        /// ��ͼ
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

            // ��ʾͼ��
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
