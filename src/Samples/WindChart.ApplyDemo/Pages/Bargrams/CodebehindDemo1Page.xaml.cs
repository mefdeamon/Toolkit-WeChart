using System;
using System.Collections.Generic;
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

namespace WindChart.ApplyDemo.Pages.Bargrams
{
    /// <summary>
    /// CodebehindDemo1Page.xaml 的交互逻辑
    /// </summary>
    public partial class CodebehindDemo1Page : Page
    {
        public CodebehindDemo1Page()
        {
            InitializeComponent();

            // Y轴范围
            bar.YMin = 0;
            bar.XMax = 100;
            bar.YMax = 100;

            bar.NeedInterval = true;
            bar.ValueLabelLocation = BarValueLocation.Follow;
            bar.Direction = BarDirection.Horizontal;

            // 实时数据模拟
            List<Bar> bars = new List<Bar>();

            bars.Add(new Bar() { Fill = Brushes.CornflowerBlue, Label = "C#", Value = 54 });
            bars.Add(new Bar() { Fill = Brushes.CornflowerBlue, Label = "C++", Value = 46 });
            bars.Add(new Bar() { Fill = Brushes.CornflowerBlue, Label = "JavaScript", Value = 84 });
            bars.Add(new Bar() { Fill = Brushes.CornflowerBlue, Label = "Java", Value = 16 });
            bars.Add(new Bar() { Fill = Brushes.CornflowerBlue, Label = "Go", Value = 25 });
            bars.Add(new Bar() { Fill = Brushes.CornflowerBlue, Label = "Rust", Value = 26 });
            bars.Add(new Bar() { Fill = Brushes.CornflowerBlue, Label = "C", Value = 49 });
            bars.Add(new Bar() { Fill = Brushes.CornflowerBlue, Label = "Ruby", Value = 83 });
            bars.Add(new Bar() { Fill = Brushes.CornflowerBlue, Label = "TypeScript", Value = 41 });
            bars.Add(new Bar() { Fill = Brushes.CornflowerBlue, Label = "Node.js", Value = 12 });
            bars.Add(new Bar() { Fill = Brushes.CornflowerBlue, Label = "SQL", Value = 45 });
            bars.Add(new Bar() { Fill = Brushes.CornflowerBlue, Label = "Python", Value = 23 });

            bar.Draw(bars);
        }
    }
}
