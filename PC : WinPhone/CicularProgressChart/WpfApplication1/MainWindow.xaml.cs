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

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        public MainWindow() {
            InitializeComponent();
            pieChart.FgBrush = new SolidColorBrush(Color.FromRgb(58,8,120));
            pieChart.FontBrush = new SolidColorBrush(Color.FromRgb(58, 8, 120)); 
            pieChart.BgBrush = new SolidColorBrush(Color.FromArgb(100,142,51,255));
            pieChart.HoleBrush = this.Background;
        }

        private void Slider_ValueChanged_1(object sender, RoutedPropertyChangedEventArgs<double> e) {
            double value = e.NewValue;           
            pieChart.Value = value;            
        }
                
    }
}
