using System;
using System.Collections.Generic;
using System.Linq;
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
}
