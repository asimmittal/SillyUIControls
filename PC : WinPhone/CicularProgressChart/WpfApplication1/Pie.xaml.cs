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
    /// Interaction logic for Pie.xaml
    /// </summary>
    public partial class Pie : UserControl
    {
        /// <summary>
        /// Set the text color of the value in the center
        /// </summary>
        public Brush FontBrush {
            set {
                lblValue.Foreground = value;
            }
        }

        /// <summary>
        /// Set the color for the total pie
        /// </summary>
        public Brush BgBrush {
            set {
                bgCircle.Fill = value;                
            }
        }

        /// <summary>
        /// Set the color for the hole
        /// </summary>
        public Brush HoleBrush {
            set {
                hole.Fill = value;
            }
        }

        /// <summary>
        /// Set the color for the fill for the sector
        /// </summary>
        public Brush FgBrush {
            set {
                path.Fill = value;
                path.Stroke = value;
            }
        }

        /// <summary>
        /// The value for this pie
        /// </summary>
        private double val=0;
        public double Value {
            get {
                return val;
            }

            set {
                value = Math.Truncate(value * 100) / 100;
                if (value > 1) value = 1; else if (value < 0) value = 0;                
                val = value;
                drawSector(val);
                lblValue.Content = "" + (val * 100) + "%";
            }
        }

        /// <summary>
        /// this method draws the sector based on the value
        /// </summary>
        /// <param name="value"></param>
        private void drawSector(double value){

            /*
             * The circular pie consists of three parts
             * a. the background
             * b. the pie / sector
             * c. the hole
             * 
             * The path object in the user control will be used to
             * create the pie/sector. Ellipses are used to create the other
             * two. This method draws a sector of a circle based on the value
             * passed into this function.
             * 
             * A sector has three parts -
             * a. a line segment from center to circumfrence
             * b. Arcs - parts of the circumfrence
             * c. a line segment from circumfrence to center
             */
            
            //Clean the path
            path.SetValue(Path.DataProperty, null);
            PathGeometry pg = new PathGeometry();
            PathFigure fig = new PathFigure();
            
            //if the value is zero, do nothing further
            if (value == 0) return;

            //a few variables for good measure
            double height = this.ActualHeight;              
            double width = this.ActualWidth;
            double radius = height / 2;
            double theta = (360 * value) - 90;  // <--- the coordinate system is flipped with 0,0 at top left. Hence the -90
            double xC = radius;
            double yC = radius;

            //this is to ensure that the fill is complete, otherwise a slight
            //gap is left in the fill of the sector. By incrementing it by one
            //we fill that gap as the angle is now 361 deg (-90 for the coord sys inversion)
            if (value == 1) theta += 1;

            //finalPoint represents the point along the circumfrence after
            //which the sector ends and the line back to the center begins
            //use parametric equations for a circule to figure that out
            // <--- PI / 180 = 0.0174

            double finalPointX = xC + (radius * Math.Cos(theta * 0.0174));      
            double finalPointY = yC + (radius * Math.Sin(theta * 0.0174));     

            //Now we have calculated all the points we need to start drawing the
            //segments for the figure. Drawing a figure in WPF is like using a pencil
            //without lifting the tip. Each segment specifies an end point and
            //is connected to the previous segment's end point. 

            fig.StartPoint = new Point(radius, radius);                                         //start by placing the pencil's tip at the center of the circle
            LineSegment firstLine = new LineSegment(new Point(radius, 0), true);                //the first line segment goes vertically upwards (radius,0)
            fig.Segments.Add(firstLine);                                                        //draw that line segment

            //Now we have to draw the arc for this sector. The way drawing works in wpf,
            //in order to get a proper and coherent circumfrence, we need to draw separate
            //arcs everytime the sector exceeds a quarter of the circle. 

            if (value > 0.25) {                                                                 //if the sector will exceed the first quarter
                ArcSegment firstQuart = new ArcSegment();                                       //first get ready to draw the first quarter's arc
                firstQuart.Point = new Point(width, radius);                                    //being a quarter, it ends at (width,radius)
                firstQuart.SweepDirection = SweepDirection.Clockwise;                           //we're drawing clockwise
                firstQuart.IsStroked = true;                                                    //yes, it is stroked
                firstQuart.Size = new Size(radius, radius);                                     //the size of this is for a circle with r = radius
                fig.Segments.Add(firstQuart);                                                   //now draw this arc (fill property would make it a sector)
            }

            if (value > 0.5) {                                                                  //if the secotr will exceed the second quarter
                ArcSegment secondQuart = new ArcSegment();                                      //get ready to draw the second quarter's arc too
                secondQuart.Point = new Point(radius, height);                                  //..
                secondQuart.SweepDirection = SweepDirection.Clockwise;                          //..
                secondQuart.IsStroked = true;                                                   //..
                secondQuart.Size = new Size(radius, radius);                                    //..
                fig.Segments.Add(secondQuart);                                                  //draw the second quarter's arc
            }

            if (value > 0.75) {                                                                 //Similarly if the sector will exceed three quarters
                ArcSegment thirdQuart = new ArcSegment();                                       //get ready to draw the third quarter's arc (starts at the end of the previous quarter)
                thirdQuart.Point = new Point(0, radius);                                        //ends at (0,radius) 
                thirdQuart.SweepDirection = SweepDirection.Clockwise;
                thirdQuart.IsStroked = true;
                thirdQuart.Size = new Size(radius, radius);
                fig.Segments.Add(thirdQuart);                                                   //draw this arc
            }

            ArcSegment finalQuart = new ArcSegment();                                           //finally, once we're done with the quarter arcs
            finalQuart.Point = new Point(finalPointX, finalPointY);                             //let's get ready to draw the last arc/sector
            finalQuart.SweepDirection = SweepDirection.Clockwise;                               //which will complete the remaining circumfrence and end
            finalQuart.IsStroked = true;                                                        // at (finalPointX, finalPointY)
            finalQuart.Size = new Size(radius, radius);
            fig.Segments.Add(finalQuart);                                                       //draw this arc

            //Now one line segment, arcs for the circumfrence - all that is done
            //let's draw a line segment from the end of the sector/arcs back to the
            //center of the circle

            LineSegment lastLine = new LineSegment(new Point(radius, radius), true);
            fig.Segments.Add(lastLine);

            //Now take these figures and add this to the path geometry
            //then add the path geometry to the path's data property
            pg.Figures.Add(fig);
            path.SetValue(Path.DataProperty, pg);
        }

        /// <summary>
        /// Class Constructor
        /// </summary>
        public Pie() {
            InitializeComponent();
            val = 0.1;
        }

        /// <summary>
        /// Loaded event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_Loaded_1(object sender, RoutedEventArgs e) {
            drawSector(val);
        }

        /// <summary>
        /// Size changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_SizeChanged_1(object sender, SizeChangedEventArgs e) {
            this.Height = this.Width;
            drawSector(val);
        }
    }
}
