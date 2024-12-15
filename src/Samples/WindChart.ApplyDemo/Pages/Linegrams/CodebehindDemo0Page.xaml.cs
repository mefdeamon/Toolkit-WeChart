using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WindChart.ApplyDemo.Pages.Linegrams
{
    /// <summary>
    /// CodebehindDemo0Page.xaml 的交互逻辑
    /// </summary>
    public partial class CodebehindDemo0Page : Page
    {
        public CodebehindDemo0Page()
        {
            InitializeComponent();

            DateTime dtLast = DateTime.Now.AddSeconds(60);

            // 设置X轴开始显示范围
            line.XMin = DateTime.Now.ToOADate();
            line.XMax = dtLast.ToOADate();
            // 设置X轴刻度文本格式
            line.XAxisTextFormatString = "HH:mm:ss";
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

                    if (DateTime.Now > dtLast)
                    {
                        break;
                    }
                }
            });


            code.Text = @"    // 设置X轴开始显示范围
    line.XMin = DateTime.Now.ToOADate();
    line.XMax = DateTime.Now.AddSeconds(60).ToOADate();
    // 设置X轴刻度文本格式
    line.XAxisTextFormatString = ""HH: mm:ss"";
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
    });";

            xaml.Text = @"<Page x:Class=""WindChart.ApplyDemo.Pages.Linegrams.CodebehindDemo0Page""
      xmlns = ""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
      xmlns: x = ""http://schemas.microsoft.com/winfx/2006/xaml""
      xmlns: mc = ""http://schemas.openxmlformats.org/markup-compatibility/2006""
      xmlns: d = ""http://schemas.microsoft.com/expression/blend/2008""
      xmlns: local = ""clr-namespace:WindChart.ApplyDemo.Pages.Linegrams""
      xmlns: windchart = ""clr-namespace:WindChart;assembly=WindChart""
      mc: Ignorable = ""d""
      d: DesignHeight = ""450"" d: DesignWidth = ""800""
      Title = ""CodebehindDemo0Page"" >
     <Grid>
        < Border Padding = ""24"" Background = ""WhiteSmoke"" >
            <windchart:Linegram x:Name = ""line"" />
         </Border >
     </Grid >
 </Page > ";
        }
    }
}
