# Toolkit-WeChart
构建自定义&简洁的 Windows Presentation Foundation (WPF) 图表库

Build Custom and Simple WPF Charts  Library



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

   

   

