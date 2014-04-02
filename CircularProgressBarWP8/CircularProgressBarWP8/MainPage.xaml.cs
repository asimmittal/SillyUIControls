using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using CircularProgressBarWP8.Resources;
using System.Windows.Media.Animation;

namespace CircularProgressBarWP8
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage() {
            InitializeComponent();
        }

        private void PhoneApplicationPage_Loaded_1(object sender, RoutedEventArgs e) {
            pgBar.Value = slider.Value;
        }

        private void slider_ValueChanged_1(object sender, RoutedPropertyChangedEventArgs<double> e) {
            pgBar.Value = slider.Value;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e) {
            DoubleAnimation animValue = new DoubleAnimation() {
                From = 0, To = pgBar.Value, 
                Duration = TimeSpan.FromMilliseconds(700)
            };

            Storyboard sb = new Storyboard();
            Storyboard.SetTarget(animValue, pgBar);
            Storyboard.SetTargetProperty(animValue, new PropertyPath("ValueProperty"));
            sb.Children.Add(animValue);
            sb.Begin();
        }

    }
}