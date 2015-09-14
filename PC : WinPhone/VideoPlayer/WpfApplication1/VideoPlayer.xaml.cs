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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace CustomControls
{
    /// <summary>
    /// Interaction logic for VideoPlayer.xaml
    /// </summary>
    public partial class VideoPlayer : UserControl
    {
        public bool isPlaying { get; private set; }
        public event EventHandler fillComplete, mediaEnded;
        private DispatcherTimer timer;
        private bool isDragging;
        private bool isControlPanelVisible, isAnimating;
        private string lastPlayed;

        /// <summary>
        /// class constructor
        /// </summary>
        public VideoPlayer() {
            InitializeComponent();
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(200);
            timer.Tick += timer_Tick;
            isDragging = false;
            isControlPanelVisible = true;
            isAnimating = false;
            polyPause.Visibility = System.Windows.Visibility.Collapsed;
            polyPlay.Visibility = System.Windows.Visibility.Visible;
            seeker.IsEnabled = false;
        }

        /// <summary>
        /// sets the source for the media
        /// </summary>
        /// <param name="path"></param>
        public void setSource(string path) {
            mediaPlayer.Source = new Uri(path, UriKind.RelativeOrAbsolute);
            lastPlayed = path;
        }

        /// <summary>
        /// animation for showing the controls
        /// </summary>
        private void showControls() {
            if (isAnimating == false) {
                DoubleAnimation anim = new DoubleAnimation() {
                    To = 0,
                    From = controls.ActualHeight,
                    Duration = TimeSpan.FromMilliseconds(200),
                    FillBehavior = FillBehavior.HoldEnd
                };

                anim.Completed += animShow_Completed;
                isControlPanelVisible = true;
                isAnimating = true;
                TranslateTransform tt = new TranslateTransform();
                controls.RenderTransform = tt;
                tt.BeginAnimation(TranslateTransform.YProperty, anim);
            }
        }

        void animShow_Completed(object sender, EventArgs e) {
            isAnimating = false;
        }

        /// <summary>
        /// animation for hiding the controls
        /// </summary>
        private void hideControls() {
            if (isAnimating == false) {
                DoubleAnimation anim = new DoubleAnimation() {
                    From = 0,
                    To = controls.ActualHeight,
                    Duration = TimeSpan.FromMilliseconds(200),
                    FillBehavior = FillBehavior.HoldEnd
                };

                anim.Completed += animHide_Completed;
                isAnimating = true;
                TranslateTransform tt = new TranslateTransform();
                controls.RenderTransform = tt;
                tt.BeginAnimation(TranslateTransform.YProperty, anim);
            }
        }

        void animHide_Completed(object sender, EventArgs e) {
            isControlPanelVisible = false;
            isAnimating = false;
        }

        /// <summary>
        /// plays the media
        /// </summary>
        public void play() {
            if (mediaPlayer.Source == null) setSource(lastPlayed);
            mediaPlayer.Play();
            isPlaying = true;            
        }

        /// <summary>
        /// stops the playback
        /// </summary>
        public void stop() {
            mediaPlayer.Stop();
            isPlaying = false;
            if(mediaEnded != null) mediaEnded(this, new EventArgs());
        }

        /// <summary>
        /// pauses the playback
        /// </summary>
        public void pause() {
            mediaPlayer.Pause();
            isPlaying = false;
        }

        private void adjustControlsWidth() {
            seeker.Width = controls.ActualWidth - 3*(btnPlay.ActualWidth + btnPlay.Margin.Left + btnPlay.Margin.Right) - 50;
        }
        
        
        /// <summary>
        /// filling animation completed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void animFiller_Completed(object sender, EventArgs e) {
            if(fillComplete != null) fillComplete(this, new EventArgs());
        }
        
        /// <summary>
        /// Play Button clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPlay_Click(object sender, RoutedEventArgs e) {
            if (!isPlaying) {
                this.play();
                polyPause.Visibility = System.Windows.Visibility.Visible;
                polyPlay.Visibility = System.Windows.Visibility.Collapsed;
            }
            else{
                this.pause();
                polyPlay.Visibility = System.Windows.Visibility.Visible;
                polyPause.Visibility = System.Windows.Visibility.Collapsed;                
            }

            seeker.IsEnabled = true;
        }

        /// <summary>
        /// Stop Button clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStop_Click(object sender, RoutedEventArgs e) {
            this.stop();
            polyPlay.Visibility = System.Windows.Visibility.Visible;
            polyPause.Visibility = System.Windows.Visibility.Collapsed;
            seeker.IsEnabled = false;
            mediaPlayer.Source = null;
        }

        /// <summary>
        /// event handler for media opened event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mediaPlayer_MediaOpened_1(object sender, RoutedEventArgs e) {
            if (mediaPlayer.NaturalDuration.HasTimeSpan) {
                TimeSpan ts = mediaPlayer.NaturalDuration.TimeSpan;
                seeker.Maximum = ts.TotalSeconds;
                seeker.SmallChange = 1;
                seeker.LargeChange = Math.Min(10, ts.Seconds / 10);
            }

            timer.Start();
        }

        /// <summary>
        /// event handler for the media ended event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mediaPlayer_MediaEnded_1(object sender, RoutedEventArgs e) {
            if(mediaEnded!=null) mediaEnded(this, new EventArgs());
        }

        /// <summary>
        /// event handler for the timer's tick event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void timer_Tick(object sender, EventArgs e) {
            if (!isDragging) seeker.Value = mediaPlayer.Position.TotalSeconds;
        }

        /// <summary>
        /// event for when the thumb on the seeker is dragged
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void seeker_DragStarted_1(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e) {
            isDragging = true;
        }

        /// <summary>
        /// event for when the thumb stops dragging
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void seeker_DragCompleted_1(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e) {
            isDragging = false;
            mediaPlayer.Position = TimeSpan.FromSeconds(seeker.Value);
        }

        /// <summary>
        /// control loaded event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_Loaded_1(object sender, RoutedEventArgs e) {
            adjustControlsWidth();
        }

        /// <summary>
        /// size changed event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_SizeChanged_1(object sender, SizeChangedEventArgs e) {
            adjustControlsWidth();
        }

        /// <summary>
        /// when the mouse leaves the control panel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void controls_MouseLeave_1(object sender, MouseEventArgs e) {
            hideControls();
        }

        /// <summary>
        /// Mouse is moved on this control. If the mouse is moved to the bottom
        /// then show control panel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_MouseMove_1(object sender, MouseEventArgs e) {
            Point pos = e.GetPosition(this);
            if (pos.Y > this.ActualHeight - 30) {
                if(!isControlPanelVisible) showControls();
            }
        }

        /// <summary>
        /// Mute button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnMute_Click_1(object sender, RoutedEventArgs e) {
            mediaPlayer.Volume = (mediaPlayer.Volume == 0) ? 1 : 0;
            btnMute.Background = (mediaPlayer.Volume == 0) ? Brushes.Red : Brushes.Black;
        }


    }
}
