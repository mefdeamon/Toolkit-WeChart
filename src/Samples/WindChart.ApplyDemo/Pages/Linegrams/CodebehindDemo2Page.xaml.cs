using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Automation;
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
    /// CodebehindDemo2Page.xaml 的交互逻辑
    /// </summary>
    public partial class CodebehindDemo2Page : Page
    {
        public CodebehindDemo2Page()
        {
            InitializeComponent();
            Draw();
        }

        private void Draw()
        {
            // 参数配置
            double amplitude = 1.0; // 振幅
            double frequency = 1.0; // 频率 (Hz)
            double phase = 0.0;     // 相位 (弧度)
            double samplingRate = 100.0; // 采样率 (每秒采样点数)
            double duration = 4.0;  // 持续时间 (秒)

            // 生成正弦曲线
            // 生成正弦曲线
            List<Point> sineWave = GenerateSineWave(amplitude, frequency, phase, samplingRate, duration);

            // 设置X轴开始显示范围
            // X轴刻度按照日期显示
            line.IsXAxisTextDateTimeFormat = false;
            line.IsGraph = false;

            // 轴跟随数据变化
            line.IsAxisFollowData = true;
            // Y轴范围
            line.YMin = -2;
            line.YMax = 2;
            line.XMin = 0;
            line.XMax = 4;

            line.YAxisScaleCount = 4;
            line.XAxisScaleCount = 4;


            line.DrawLine(sineWave);
        }


        static List<Point> GenerateSineWave(double amplitude, double frequency, double phase, double samplingRate, double duration)
        {
            List<Point> points = new List<Point>();
            int totalPoints = (int)(samplingRate * duration); // 总点数
            double deltaTime = 1.0 / samplingRate;            // 时间步长

            for (int i = 0; i < totalPoints; i++)
            {
                double x = i * deltaTime; // 当前时间点
                double y = amplitude * Math.Sin(2 * Math.PI * frequency * x + phase); // 正弦函数
                points.Add(new Point(x, y));
            }

            return points;
        }
    }
}
