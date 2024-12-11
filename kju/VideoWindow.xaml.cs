using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Windows.Media.Core;
using Windows.Media.Playback;
using System; // For Uri

namespace kju
{
    public sealed partial class VideoWindow : Window
    {
        public MediaPlayer MediaPlayer { get; private set; }

        public VideoWindow()
        {
            InitializeComponent(); // Make sure this is called

            // Create and configure the media player
            MediaPlayer = new MediaPlayer();
            VideoPlayerElement.SetMediaPlayer(MediaPlayer);

            // Set window to be a normal window
            var appWindow = GetAppWindow();
            if (appWindow != null)
            {
                appWindow.Title = "Video Player";
                appWindow.Resize(new Windows.Graphics.SizeInt32(800, 600));
            }
        }

        private AppWindow? GetAppWindow()
        {
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            var wndId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
            return AppWindow.GetFromWindowId(wndId);
        }

        public void PlayVideo(string filePath)
        {
            var mediaSource = MediaSource.CreateFromUri(new Uri(filePath));
            MediaPlayer.Source = mediaSource;
            MediaPlayer.Play();
        }

        public void PauseVideo()
        {
            MediaPlayer.Pause();
        }

        public void ResumeVideo()
        {
            MediaPlayer.Play();
        }

        public void CloseVideo()
        {
            MediaPlayer.Pause();
            MediaPlayer.Source = null;
        }
    }
}