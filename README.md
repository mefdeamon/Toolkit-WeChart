# Toolkit-WeChart
构建自定义&简洁的 Windows Presentation Foundation (WPF) 图表库

Build Custom and Simple WPF Charts  Library



## 概要（Summary）

**Wind Chart **是面向 Windows Presentation Foundation (WPF)的一套图表库。由 **Meiliyong** 自主设计开发完成，最初基于.NET Core 3.1开发，如需其他框架版本请下载源码重新编译，如需查看示例请下载源代码。

源码地址：[mefdeamon/Toolkit-WeChart(github.com)](https://github.com/mefdeamon/Toolkit-WeChart)

图表控件包含：

- 单线图（Linegram）



## 快速使用（Quick Start）

1. 命名空间namespace引入

   ```xaml
         xmlns:windchart="clr-namespace:WindChart;assembly=WindChart"
   ```

2. 单线图应用示例

   单线图控件**Linegram**引入

   ```xaml
   <Page x:Class="WindChart.ApplyDemo.Pages.LinegramPage"
         xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
         xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
         xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006" 
         xmlns:d="http://schemas.microsoft.com/expression/blend/2008" 
         xmlns:local="clr-namespace:WindChart.ApplyDemo.Pages"
         xmlns:windchart="clr-namespace:WindChart;assembly=WindChart"
         mc:Ignorable="d" 
         d:DesignHeight="450" d:DesignWidth="800"
         Title="LinegramPage">
       <Grid>
           <Grid.RowDefinitions>
               <RowDefinition Height="Auto"/>
               <RowDefinition Height="349*"/>
           </Grid.RowDefinitions>
           <ToggleButton Content="开始模拟" Margin="5,5,5,5"
                         Click="Simulate_Click"  />
           <Grid Grid.Row="1">
               <windchart:Linegram x:Name="line" Margin="20,20,20,20" XAxisBrush="Gray" YAxisScaleCount="4" Grid.Row="0"/>
           </Grid>
       </Grid>
   </Page>
   
   ```

   在*code behind*调用**DrawLine**绘图

   ```c#
   using System;
   using System.Collections.Generic;
   using System.Threading;
   using System.Threading.Tasks;
   using System.Windows;
   using System.Windows.Controls;
   
   namespace WindChart.ApplyDemo.Pages
   {
       /// <summary>
       /// Interaction logic for LinegramPage.xaml
       /// </summary>
       public partial class LinegramPage : Page
       {
           public LinegramPage()
           {
               InitializeComponent();
           }
   
           /// <summary>
           /// 模拟状态
           /// </summary>
           bool IsSimulated = false;
   
           /// <summary>
           /// 开始/停止模拟
           /// </summary>
           /// <param name="sender"></param>
           /// <param name="e"></param>
           private void Simulate_Click(object sender, RoutedEventArgs e)
           {
               if (IsSimulated)
               {
                   IsSimulated = false;
               }
               else
               {
                   IsSimulated = true;
                   Task.Run(() =>
                   {
                       int x = 0;
                       int y = 0;
                       while (IsSimulated)
                       {
                           // 初始化图形
                           this.Dispatcher.Invoke(() =>
                           {
                               line.Init();
                           });
                           while (IsSimulated)
                           {
                               if (x > 500)
                               {
                                   x = 0;
                                   break;
                               }
                               
                               // 生产一个随机点
                               var ran = Random.Shared.Next(-150, 150);
                               if (ran > 0)
                               {
                                   y -= Math.Abs(ran) / 10;
                               }
                               else
                               {
                                   y += Math.Abs(ran) / 10;
                               }
                               var p = new Point(x, y);
   
                               // 绘制一个随机点
                               this.Dispatcher.Invoke(() =>
                               {
                                   line.DrawLine(p);
                               });
                               x++;
                           }
                           Thread.Sleep(5000);
                       }
                   });
               }
           }
       }
   }
   ```




## 应用（Apply）

### 单线图（Linegram）

单线图应用示例

View中引入单线图控件**Linegram**，DataContext与ViewModel绑定。

```xaml
<Page x:Class="WindChart.ApplyDemo.Pages.WelcomePage"
      xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
      xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
      xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006" 
      xmlns:d="http://schemas.microsoft.com/expression/blend/2008" 
      xmlns:local="clr-namespace:WindChart.ApplyDemo.Pages"
      xmlns:windchart="clr-namespace:WindChart;assembly=WindChart"
      mc:Ignorable="d" 
      DataContext="{Binding WelcomePageModel, Source={StaticResource Locator}}"
      d:DesignHeight="450" d:DesignWidth="800"
      Title="WelcomePage">
    <Grid>
        <Grid.ColumnDefinitions>
            <ColumnDefinition Width="*"/>
            <ColumnDefinition Width="*"/>
        </Grid.ColumnDefinitions>
        <Grid.RowDefinitions>
            <RowDefinition Height="*"/>
            <RowDefinition Height="*"/>
        </Grid.RowDefinitions>
        <Border Padding="24 12" Background="WhiteSmoke"
                Grid.Row="0" Grid.Column="0">
            <!-- 图标控件 -->
            <windchart:Linegram 
                                XAxisBrush="Gray" YAxisBrush="Black"
                                YAxisScaleCount="4" XAxisScaleCount="10"
                                YMax="100" YMin="40" XMin="10" XMax="100"
                                LineSource="{Binding LinePoints}"
                                LineThickness="1"
                                LineBrush="Black"
                                IsGraph="True" 
                                KeepAllPoints="True"
                                NeedAiming="True"
                                >
                <windchart:Linegram.Fill>
                    <LinearGradientBrush>
                        <GradientStop Color="#8FBC8F"/>
                        <GradientStop Color="#408FBC8F"/>
                    </LinearGradientBrush>
                </windchart:Linegram.Fill>
            </windchart:Linegram>
        </Border>
    </Grid>
</Page>

```

ViewModel中定义

```c#
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;

namespace WindChart.ApplyDemo.PageModels
{
    /// <summary>
    /// 欢迎页面数据模型
    /// </summary>
    internal class WelcomePageModel : MeiMvvm.NotifyPropertyChanged
    {
        private readonly Random random = new Random();

        /// <summary>
        /// 单线图的点
        /// </summary>
        public ObservableCollection<Point> LinePoints { get; set; } = new ObservableCollection<Point>();

        public WelcomePageModel()
        {
            Task.Delay(10).ContinueWith(t =>
            {
                double x = 0;
                double y = 0;
                List<Point> list = new List<Point>();
                while (true)
                {
                    if (x > 500)
                    {
                        LinePoints = new ObservableCollection<Point>(list);
                        break;
                    }

                    var ran = random.Next(-150, 150);

                    if (ran > 0)
                    {
                        y -= Math.Abs(ran) / 10;
                    }
                    else
                    {
                        y += Math.Abs(ran) / 10;
                    }
                    var p = new Point(x, y);

                    list.Add(p);

                    x++;
                }
            });
        }
    }
}

```



