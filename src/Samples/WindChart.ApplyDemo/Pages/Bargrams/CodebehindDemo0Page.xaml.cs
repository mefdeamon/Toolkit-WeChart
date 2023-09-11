using System;
using System.Collections.Generic;
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

namespace WindChart.ApplyDemo.Pages.Bargrams
{
    /// <summary>
    /// CodebehindDemo0Page.xaml 的交互逻辑
    /// </summary>
    public partial class CodebehindDemo0Page : Page
    {
        public CodebehindDemo0Page()
        {
            InitializeComponent();

            // Y轴范围
            bar.YMin = 0;
            bar.YMax = 24;

            bar.NeedInterval = true;
            bar.ValueLabelLocation = BarValueLocation.None;

            // 实时数据模拟
            Task.Run(() =>
            {
                Random rand = new Random();
                double y = 10;
                List<Bar> bars = new List<Bar>();
                DateTime dt = DateTime.Now.AddMonths(-2);
                while (true)
                {
                    y = rand.Next(5, 16);

                    this.Dispatcher.Invoke(() =>
                    {
                        bars.Add(new Bar() { Fill = y > 8 ? Brushes.CornflowerBlue : Brushes.OrangeRed, Label = dt.Day.ToString(), Value = y });
                        bar.Draw(bars);
                    });
                    dt = dt.AddDays(1);
                    if (dt > DateTime.Now)
                    {
                        break;
                    }
                }
            });

            code.Text = @"    // Y轴范围
    bar.YMin = 0;
    bar.YMax = 24;

    bar.NeedInterval = true;
    bar.ValueLabelLocation = BarValueLocation.None;

    // 实时数据模拟
    Task.Run(() =>
    {
        Random rand = new Random();
        double y = 10;
        List<Bar> bars = new List<Bar>();
        DateTime dt = DateTime.Now.AddMonths(-2);
        while (true)
        {
            y = rand.Next(5, 16);

            this.Dispatcher.Invoke(() =>
            {
                bars.Add(new Bar() { Fill = y > 8 ? Brushes.CornflowerBlue : Brushes.OrangeRed, Label = dt.Day.ToString(), Value = y });
                bar.Draw(bars);
            });
            Thread.Sleep(30);
            dt = dt.AddDays(1);
            if (dt > DateTime.Now)
            {
                break;
            }
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
      	<Grid Grid.Row=""1""  Background=""WhiteSmoke"">
            <StackPanel Orientation=""Horizontal"" Margin=""5"" VerticalAlignment=""Top"">
                <TextBlock Text=""近2月睡眠时长分布（小时）""  FontWeight=""Black""/>
                <TextBlock Text=""不足8小时"" Margin=""10 0 5 0""/>
                <Rectangle HorizontalAlignment=""Left"" VerticalAlignment=""Center"" Fill=""OrangeRed"" Height=""10"" Width=""30""/>
                 <TextBlock Text=""睡眠充足"" Margin=""10 0 5 0""/>
                <Rectangle HorizontalAlignment=""Left"" VerticalAlignment=""Center"" Fill=""CornflowerBlue"" Height=""10"" Width=""30""/>
            </StackPanel>

            <Border Padding=""24 36 24 24""
                Grid.Row=""1"" >
                <windchart:Bargram x:Name=""bar""/>
            </Border>
        </Grid>
  </Page > ";
        }
    }
}
