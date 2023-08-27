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
    /// <see cref="YAxisLineAlignment"/> To <see cref="Visibility"/>值转换器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class YAxisLineAlignmentToVisibilityConverter : IValueConverter
    {
        public YAxisLineAlignmentToVisibilityConverter()
        {
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            YAxisLineAlignment mode = (YAxisLineAlignment)value;
            if (mode == YAxisLineAlignment.Location)
            {
                return Visibility.Visible;
            }
            else
            { return Visibility.Collapsed; }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => Binding.DoNothing;
    }

    /// <summary>
    /// <see cref="XAxisLineAlignment"/> To <see cref="Visibility"/>值转换器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class XAxisLineAlignmentToVisibilityConverter : IValueConverter
    {
        public XAxisLineAlignmentToVisibilityConverter()
        {
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            XAxisLineAlignment mode = (XAxisLineAlignment)value;
            if (mode == XAxisLineAlignment.Location)
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
