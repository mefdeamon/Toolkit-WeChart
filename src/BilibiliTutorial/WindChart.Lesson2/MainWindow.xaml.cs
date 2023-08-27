using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace WindChart.Lesson2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
    }


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
        /// 绘制原点
        /// </summary>
        private void DrawAxis()
        {
            var drawingContext = axisVisual.RenderOpen();

            Point org = new Point(0, 0);
            org = ConvertToPixcel(org);

            drawingContext.DrawEllipse(Brushes.Black, new Pen(Brushes.OrangeRed, 1), org, 5, 5);

            drawingContext.Close();
        }

        #region 场景大小

        public double XMin { get; set; } = -10;
        public double YMin { get; set; } = -10;
        public double XMax { get; set; } = 100;
        public double YMax { get; set; } = 100;

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

    }

}
