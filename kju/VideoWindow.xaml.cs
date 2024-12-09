using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Media.Playback;
using System.IO;
using System;

namespace kju
{
    public sealed partial class VideoWindow : Window
    {
        public MediaPlayer MediaPlayer => VideoPlayer.MediaPlayer;

        public VideoWindow()
        {
            this.InitializeComponent();
        }

        public void ShowImage(string imagePath)
        {
            // Hide video player
            VideoPlayer.Visibility = Visibility.Collapsed;
            ImageViewer.Visibility = Visibility.Visible;

            // Load the image
            var bitmapImage = new BitmapImage(new Uri(imagePath));
            ImageViewer.Source = bitmapImage;
        }

        public void ShowVideo()
        {
            // Show video player
            VideoPlayer.Visibility = Visibility.Visible;
            ImageViewer.Visibility = Visibility.Collapsed;
        }
    }
}