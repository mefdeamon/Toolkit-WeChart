using System;
using System.Collections.Generic;
using System.Linq;
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

namespace WindChart.Lesson4
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // 模拟
            Task.Delay(1000).ContinueWith((Action<Task>)(t =>
            {

                this.Dispatcher.Invoke(() =>
                {
                    lineGram.InitLine(Brushes.Red);
                });

                double x = 0;
                double y = 0;

                Random random = new Random();

                y = 50;

                while (true)
                {

                    this.Dispatcher.Invoke((Action)(() =>
                    {
                        lineGram.DrawLine(new Point(x, y));
                    }));

                    x++;
                    y = random.Next() % 2 == 0 ? ++y : --y;

                    if (x > 500)
                    {
                        this.Dispatcher.Invoke(() =>
                        {
                            lineGram.InitLine(Brushes.DarkOliveGreen);
                            y = 50;
                            x = 0;
                        });
                    }

                    //Thread.Sleep(100);
                }



            }));

        }
    }
}
