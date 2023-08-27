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

namespace WindChart.ApplyDemo.Pages.Linegrams
{
    /// <summary>
    /// CodebehindDemo1Page.xaml 的交互逻辑
    /// </summary>
    public partial class CodebehindDemo1Page : Page
    {
        public CodebehindDemo1Page()
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

            code.Text = @"    // 设置X轴开始显示范围
    DateTime dtStart = DateTime.Now.AddMinutes(-3);
    line.XMin = dtStart.ToOADate();
    line.XMax = DateTime.Now.AddSeconds(60).ToOADate();
    // 设置X轴刻度文本格式
    line.XAxisTextFormatString = ""HH: mm: ss"";
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
    line.DrawLine(points);";

            xaml.Text = @"<Page x:Class=""WindChart.ApplyDemo.Pages.Linegrams.CodebehindDemo1Page""
      xmlns = ""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
      xmlns: x = ""http://schemas.microsoft.com/winfx/2006/xaml""
      xmlns: mc = ""http://schemas.openxmlformats.org/markup-compatibility/2006""
      xmlns: d = ""http://schemas.microsoft.com/expression/blend/2008""
      xmlns: local = ""clr-namespace:WindChart.ApplyDemo.Pages.Linegrams""
      xmlns: windchart = ""clr-namespace:WindChart;assembly=WindChart""
      mc: Ignorable = ""d""
      d: DesignHeight = ""450"" d: DesignWidth = ""800""
      Title = ""CodebehindDemo1Page"" >
     <Grid>
        < Border Padding = ""24"" Background = ""WhiteSmoke"" >
            <windchart:Linegram x:Name = ""line"" />
         </Border >
     </Grid >
 </Page > ";
        }
    }
}
