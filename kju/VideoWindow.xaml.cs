using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Media.Playback;
using System;
using Microsoft.UI.Windowing;

namespace kju
{
    public partial class VideoWindow : Window
    {
        public MediaPlayer MediaPlayer { get; private set; }

        private bool isFullScreen = false;

        public VideoWindow()
        {
            InitializeComponent();

            // Initialize the MediaPlayer
            MediaPlayer = new MediaPlayer();

            // Use the SetMediaPlayer method to assign the MediaPlayer
            VideoPlayer.SetMediaPlayer(MediaPlayer);
        }

        public void ToggleFullScreen()
        {
            if (!isFullScreen)
            {
                // Set to fullscreen
                this.AppWindow.SetPresenter(AppWindowPresenterKind.FullScreen);
                isFullScreen = true;
            }
            else
            {
                // Return to normal state
                this.AppWindow.SetPresenter(AppWindowPresenterKind.Overlapped);
                isFullScreen = false;
            }
        }

        public void DisplayImage(string imagePath)
        {
            if (!System.IO.File.Exists(imagePath))
            {
                throw new System.IO.FileNotFoundException("Image file not found", imagePath);
            }

            var bitmapImage = new BitmapImage(new Uri(imagePath));
            ImageDisplay.Source = bitmapImage;

            // Make the image visible and hide the video player
            ImageDisplay.Visibility = Visibility.Visible;
            VideoPlayer.Visibility = Visibility.Collapsed;
        }

        public double GetCurrentPosition()
        {
            return MediaPlayer.PlaybackSession.Position.TotalSeconds;
        }

        public double GetDuration()
        {
            return MediaPlayer.PlaybackSession.NaturalDuration.TotalSeconds;
        }

        public void SetPosition(double seconds)
        {
            MediaPlayer.PlaybackSession.Position = TimeSpan.FromSeconds(seconds);
        }
    }
}
