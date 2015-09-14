using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace UserControls
{
    public partial class ChatHead : UserControl
    {
        const double thickness = 1.5;
        public int Radius { get; set; }
        public string Label {
            get { return txtNotification.Text; }
            set { txtNotification.Text = value; }
        }

        private string _imgPath;
        public string ImagePath {
            get {
                return _imgPath; 
            }
            set{
                _imgPath = value;
                Uri uri = new Uri(_imgPath, UriKind.RelativeOrAbsolute);
                BitmapImage bmp = new BitmapImage(uri);
                image.Source = bmp;         
            }
        }

        /// <summary>
        /// Class constructor
        /// </summary>
        public ChatHead(){
            InitializeComponent();
        }

        /// <summary>
        /// This creates the circular clip around this control
        /// </summary>
        private void invalidate() {

            if (Radius >= 0 && Radius * 2 <= Width) {
                
                //decide the center of the circular region
                double centerX = this.Height / 2;
                double centerY = this.Width / 2;

                //create the clip geometry
                EllipseGeometry geom = new EllipseGeometry();
                geom.RadiusY = Radius - thickness;
                geom.RadiusX = Radius - thickness;
                geom.Center = new Point(centerX, centerY);

                //apply the clip to this control
                image.Clip = geom;

                //now setup the shape outline
                outline.Width = Radius * 2;
                outline.Height = Radius * 2;

                //set the location of the notification
                double coord = (Width / 1.732) - Radius;
                notification.Margin = new Thickness(coord, coord, 0, 0);
            }
        }

        /// <summary>
        /// Invoked when the control loads
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_Loaded_1(object sender, RoutedEventArgs e) {
            invalidate();
        }

        /// <summary>
        /// Invoked when thie control is resized
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_SizeChanged_1(object sender, SizeChangedEventArgs e) {
            Height = Width;
            invalidate();
        }
    }
}
