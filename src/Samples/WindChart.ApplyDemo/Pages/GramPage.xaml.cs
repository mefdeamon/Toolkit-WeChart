using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace WindChart.ApplyDemo.Pages
{
    /// <summary>
    /// Interaction logic for GramPage.xaml
    /// </summary>
    public partial class GramPage : Page
    {
        public GramPage()
        {
            InitializeComponent();
            XAxisLineCheckBox.IsChecked = true;
            XAxisTextCheckBox.IsChecked = true;
            YAxisTextCheckBox.IsChecked = true;
            YAxisLineCheckBox.IsChecked = true;
        }
    }

    /// <summary>
    /// <see cref="AxisLineMode"/> To <see cref="Visibility"/>值转换器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AxisLineModeToVisibilityConverter : IValueConverter
    {
        public AxisLineModeToVisibilityConverter()
        {
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            AxisLineMode mode = (AxisLineMode)value;
            if (mode == AxisLineMode.Location)
            {
                return Visibility.Visible;
            }
            else
            { return Visibility.Collapsed; }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => Binding.DoNothing;
    }
}
