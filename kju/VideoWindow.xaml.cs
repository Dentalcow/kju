using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Media.Playback;

namespace kju
{
    public sealed partial class VideoWindow : Window
    {
        public MediaPlayer MediaPlayer => VideoPlayer.MediaPlayer;

        public VideoWindow()
        {
            this.InitializeComponent();
        }
    }
}
