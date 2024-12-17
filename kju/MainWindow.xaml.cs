using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace kju
{
    public sealed partial class MainWindow : Window
    {
        public ObservableCollection<AudioCue> Cues { get; } = new ObservableCollection<AudioCue>();
        public ApplicationSettings AppSettings { get; private set; }
        private MediaPlayer mediaPlayer;
        private int currentCueIndex = -1;
        private IntPtr m_windowHandle;
        private DispatcherQueue m_dispatcherQueue = null!;
        private VideoWindow? videoWindow;

        public MainWindow()
        {
            this.InitializeComponent();

            // Initialize settings
            AppSettings = new ApplicationSettings();

            // Initialize window handle and dispatcher queue
            InitializeWindowHandling();

            mediaPlayer = new MediaPlayer();
            mediaPlayer.PlaybackSession.PositionChanged += UpdateProgressBar;

            // Initialize and show the video window
            InitializeVideoWindow();

            // Handle the Closed event to close all windows
            this.Closed += MainWindow_Closed;
        }

        private void ToggleVideoFullscreen()
        {
            if (videoWindow != null)
            {
                videoWindow.ToggleFullScreen();
            }
        }

        private void InitializeWindowHandling()
        {
            // Get the window handle
            m_windowHandle = WindowNative.GetWindowHandle(this);

            // Initialize dispatcher queue
            m_dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        }

        private void InitializeVideoWindow()
        {
            videoWindow = new VideoWindow();
            videoWindow.Closed += (s, e) => videoWindow = null;
            videoWindow.Activate(); // Use Activate instead of Show

            // Set the MediaPlayer source to a blank media to display a black screen
            videoWindow.MediaPlayer.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Assets/black.mp4"));
            videoWindow.MediaPlayer.Play();

            // Add MediaEnded event handler
            videoWindow.MediaPlayer.MediaEnded += MediaPlayer_MediaEnded;

            // Enable hardware acceleration
            videoWindow.MediaPlayer.RealTimePlayback = true;
        }

        private void Fullscreen_Click(object sender, RoutedEventArgs e)
        {
            ToggleVideoFullscreen();
        }

        private void MediaPlayer_MediaEnded(MediaPlayer sender, object args)
        {
            m_dispatcherQueue.TryEnqueue(() =>
            {
                if (videoWindow != null)
                {
                    // Set the MediaPlayer source to the splash screen
                    videoWindow.MediaPlayer.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Assets/splashscreen.mp4"));
                    videoWindow.MediaPlayer.Play();
                }
            });
        }

        private void MainWindow_Closed(object sender, WindowEventArgs e)
        {
            // Close the video window if it is open
            videoWindow?.Close();
        }

        private async void Settings_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ContentDialog
            {
                Title = "kju Settings",
                PrimaryButtonText = "Save",
                CloseButtonText = "Cancel",
                XamlRoot = this.Content.XamlRoot
            };

            var stackPanel = new StackPanel { Spacing = 10 };

            // Add button to reopen video window
            var reopenVideoWindowButton = new Button
            {
                Content = "Reopen Playback Window",
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
            reopenVideoWindowButton.Click += (s, args) =>
            {
                if (videoWindow == null)
                {
                    InitializeVideoWindow();
                }
                else
                {
                    videoWindow.Activate();
                }
            };

            stackPanel.Children.Add(reopenVideoWindowButton);

            // Audio File Directories
            var audioDirectoriesExpander = new Expander
            {
                Header = "Audio File Directories",
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
            var audioDirectoriesStack = new StackPanel { Spacing = 5 };

            // Existing directories list
            var directoriesListView = new ListView();
            directoriesListView.ItemsSource = AppSettings.AudioDirectories;

            // Add directory button
            var addDirectoryButton = new Button
            {
                Content = "Add Directory",
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
            addDirectoryButton.Click += async (s, args) =>
            {
                var folderPicker = new FolderPicker();
                folderPicker.SuggestedStartLocation = PickerLocationId.MusicLibrary;
                InitializeWithWindow.Initialize(folderPicker, m_windowHandle);

                StorageFolder? selectedFolder = await folderPicker.PickSingleFolderAsync();
                if (selectedFolder != null)
                {
                    AppSettings.AudioDirectories.Add(selectedFolder.Path);
                    directoriesListView.ItemsSource = null;
                    directoriesListView.ItemsSource = AppSettings.AudioDirectories;
                }
            };

            // Remove directory button
            var removeDirectoryButton = new Button
            {
                Content = "Remove Selected Directory",
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
            removeDirectoryButton.Click += (s, args) =>
            {
                if (directoriesListView.SelectedItem is string selectedPath)
                {
                    AppSettings.AudioDirectories.Remove(selectedPath);
                    directoriesListView.ItemsSource = null;
                    directoriesListView.ItemsSource = AppSettings.AudioDirectories;
                }
            };

            audioDirectoriesStack.Children.Add(directoriesListView);
            audioDirectoriesStack.Children.Add(addDirectoryButton);
            audioDirectoriesStack.Children.Add(removeDirectoryButton);
            audioDirectoriesExpander.Content = audioDirectoriesStack;

            // Default Cue Type
            var defaultTypeComboBox = new ComboBox
            {
                Header = "Default Cue Type",
                PlaceholderText = "Select Default Cue Type",
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
            defaultTypeComboBox.Items.Add("Audio");
            defaultTypeComboBox.Items.Add("Video");
            defaultTypeComboBox.SelectedItem = AppSettings.DefaultCueType;
            defaultTypeComboBox.SelectionChanged += (s, args) =>
            {
                if (defaultTypeComboBox.SelectedItem is string selectedType)
                {
                    AppSettings.DefaultCueType = selectedType;
                }
            };

            stackPanel.Children.Add(audioDirectoriesExpander);
            stackPanel.Children.Add(defaultTypeComboBox);

            dialog.Content = stackPanel;

            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                // Save settings logic would go here
                // For now, settings are automatically updated
            }
        }

        private async void Help_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ContentDialog
            {
                Title = "kju - Help",
                PrimaryButtonText = "Close",
                XamlRoot = this.Content.XamlRoot
            };

            var scrollViewer = new ScrollViewer();
            var stackPanel = new StackPanel { Spacing = 10 };

            // Add help sections
            var sections = new[]
            {
                    new {
                        Title = "Getting Started",
                        Content = "kju is an audio cueing application designed to help you manage and play audio cues seamlessly."
                    },
                    new {
                        Title = "Importing Files",
                        Content = "Use the 'Import Files' button to add audio files. You can select multiple files and assign them a cue type."
                    },
                    new {
                        Title = "Adding Cues",
                        Content = "Click 'Add Cue' to manually add a new cue. You can specify a name, type, and select an audio file."
                    },
                    new {
                        Title = "Playing Cues",
                        Content = "Use the 'Previous' and 'Next' buttons to navigate between cues. The 'Play/Pause' button controls audio playback."
                    },
                    new {
                        Title = "Settings",
                        Content = "In the settings, you can manage audio directories and set a default cue type."
                    }
                };

            foreach (var section in sections)
            {
                var sectionExpander = new Expander
                {
                    Header = section.Title,
                    HorizontalAlignment = HorizontalAlignment.Stretch
                };
                sectionExpander.Content = new TextBlock
                {
                    Text = section.Content,
                    TextWrapping = TextWrapping.Wrap
                };
                stackPanel.Children.Add(sectionExpander);
            }

            scrollViewer.Content = stackPanel;
            dialog.Content = scrollViewer;

            await dialog.ShowAsync();
        }

        private async void AddCue_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await AddCuesWithFilePicker();
            }
            catch (Exception ex)
            {
                await DisplayErrorMessage($"An error occurred: {ex.Message}");
            }
        }

        private async Task AddCuesWithFilePicker()
        {
            var picker = new FileOpenPicker();
            picker.SuggestedStartLocation = PickerLocationId.MusicLibrary;

            // Add file type filters for various media types
            var supportedTypes = new[] {
                ".mp3", ".wav", ".flac", ".aac", ".m4a",  // Audio
                ".mp4", ".mkv", ".avi", ".mov", ".wmv",   // Video
                ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp"  // Images
            };

            foreach (var ext in supportedTypes)
            {
                picker.FileTypeFilter.Add(ext);
            }

            // Initialize the picker with the window handle
            InitializeWithWindow.Initialize(picker, m_windowHandle);

            var pickedFiles = await picker.PickMultipleFilesAsync();
            if (pickedFiles != null && pickedFiles.Count > 0)
            {
                foreach (var file in pickedFiles)
                {
                    // Determine type based on file extension
                    string type = DetermineFileType(file.Path);
                    AddCue(file.Path, type);
                }
            }
        }

        private string DetermineFileType(string filePath)
        {
            string extension = System.IO.Path.GetExtension(filePath).ToLower();
            switch (extension)
            {
                case ".mp3":
                case ".wav":
                case ".flac":
                case ".aac":
                case ".m4a":
                    return "Audio";
                case ".mp4":
                case ".mkv":
                case ".avi":
                case ".mov":
                case ".wmv":
                    return "Video";
                case ".jpg":
                case ".jpeg":
                case ".png":
                case ".gif":
                case ".bmp":
                case ".webp":
                    return "Image";
                default:
                    return "Audio"; // Default to Audio if unknown
            }
        }

        private async Task DisplayErrorMessage(string message)
        {
            var errorDialog = new ContentDialog
            {
                Title = "Error",
                Content = message,
                CloseButtonText = "OK",
                XamlRoot = this.Content.XamlRoot
            };

            await errorDialog.ShowAsync();
        }

        private void AddCue(string filePath, string? type = null, string? name = null)
        {
            var cue = new AudioCue
            {
                CueNumber = Cues.Count + 1,
                Type = type ?? "Audio",
                Name = name ?? System.IO.Path.GetFileNameWithoutExtension(filePath),
                FilePath = filePath
            };

            Cues.Add(cue);
        }

        private void RemoveCue_Click(object sender, RoutedEventArgs e)
        {
            if (CuesTableView.SelectedItem is AudioCue selectedCue)
            {
                Cues.Remove(selectedCue);
                UpdateCueNumbers();
            }
            else
            {
                ShowErrorMessage("Please select a cue to remove.");
            }
        }

        private void UpdateCueNumbers()
        {
            for (int i = 0; i < Cues.Count; i++)
            {
                Cues[i].CueNumber = i + 1;
            }
        }

        private async void ShowErrorMessage(string message)
        {
            var errorDialog = new ContentDialog
            {
                Title = "Error",
                Content = message,
                CloseButtonText = "OK",
                XamlRoot = this.Content.XamlRoot
            };

            await errorDialog.ShowAsync();
        }

        private void PlayPause_Click(object sender, RoutedEventArgs e)
        {
            if (currentCueIndex >= 0 && currentCueIndex < Cues.Count)
            {
                var currentCue = Cues[currentCueIndex];
                var playbackState = currentCue.Type == "Video" ? videoWindow?.MediaPlayer.PlaybackSession.PlaybackState : mediaPlayer.PlaybackSession.PlaybackState;

                if (playbackState == MediaPlaybackState.Playing)
                {
                    if (currentCue.Type == "Video")
                    {
                        videoWindow?.MediaPlayer.Pause();
                    }
                    else
                    {
                        mediaPlayer.Pause();
                    }
                    PlayPauseButton.Content = "Play";
                }
                else if (playbackState == MediaPlaybackState.Paused)
                {
                    if (currentCue.Type == "Video")
                    {
                        videoWindow?.MediaPlayer.Play();
                    }
                    else
                    {
                        mediaPlayer.Play();
                    }
                    PlayPauseButton.Content = "Pause";
                }
                else
                {
                    PlayAudio();
                    PlayPauseButton.Content = "Pause";
                }
            }
        }

        private void GoMinus_Click(object sender, RoutedEventArgs e)
        {
            if (currentCueIndex > 0)
            {
                currentCueIndex--;
                PlayAudio();
            }
        }

        private void GoPlus_Click(object sender, RoutedEventArgs e)
        {
            if (currentCueIndex < Cues.Count - 1)
            {
                currentCueIndex++;
                PlayAudio();
            }
        }

        private async void PlayAudio()
        {
            if (Cues.Count > 0)
            {
                if (currentCueIndex == -1 && Cues.Count > 0)
                {
                    currentCueIndex = 0;
                }

                if (currentCueIndex >= 0 && currentCueIndex < Cues.Count)
                {
                    var currentCue = Cues[currentCueIndex];
                    try
                    {
                        // Stop previous video if it was playing
                        if (videoWindow != null)
                        {
                            videoWindow.MediaPlayer.Pause();
                        }

                        if (!System.IO.File.Exists(currentCue.FilePath))
                        {
                            await DisplayErrorMessage($"File not found: {currentCue.FilePath}");
                            return;
                        }

                        // Reset video window visibility
                        if (videoWindow != null)
                        {
                            videoWindow.VideoPlayer.Visibility = Visibility.Visible;
                            videoWindow.ImageDisplay.Visibility = Visibility.Collapsed;
                        }

                        switch (currentCue.Type)
                        {
                            case "Video":
                                await PlayVideoAsync(currentCue.FilePath);
                                break;
                            case "Image":
                                PlayImage(currentCue.FilePath);
                                break;
                            default: // Audio
                                var storageFile = await StorageFile.GetFileFromPathAsync(currentCue.FilePath);
                                mediaPlayer.Source = MediaSource.CreateFromStorageFile(storageFile);
                                mediaPlayer.Play();
                                PlayPauseButton.Content = "Pause";
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        await DisplayErrorMessage($"Error playing cue: {ex.Message}");
                    }
                }
            }
        }

        private async Task PlayVideoAsync(string filePath)
        {
            if (videoWindow == null)
            {
                videoWindow = new VideoWindow();
                videoWindow.Closed += (s, e) => videoWindow = null;
                videoWindow.Activate();

                // Hook up the PositionChanged event for video
                videoWindow.MediaPlayer.PlaybackSession.PositionChanged += UpdateProgressBar;

                // Also hook up MediaOpened event to set initial progress bar state
                videoWindow.MediaPlayer.MediaOpened += VideoMediaPlayer_MediaOpened;
            }

            var storageFile = await StorageFile.GetFileFromPathAsync(filePath);
            var mediaSource = MediaSource.CreateFromStorageFile(storageFile);

            videoWindow.MediaPlayer.Source = mediaSource;
            videoWindow.MediaPlayer.Play();
            PlayPauseButton.Content = "Pause";
        }

        private void PlayImage(string filePath)
        {
            if (videoWindow == null)
            {
                videoWindow = new VideoWindow();
                videoWindow.Closed += (s, e) => videoWindow = null;
                videoWindow.Activate();
            }

            try
            {
                // Hide video player and show image
                videoWindow.VideoPlayer.Visibility = Visibility.Collapsed;
                videoWindow.ImageDisplay.Visibility = Visibility.Visible;

                // Use the public method to display the image
                videoWindow.DisplayImage(filePath);

                // Disable playback controls for images
                PlayPauseButton.Content = "Play";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error displaying image: {ex.Message}");
            }
        }

        private void PlayVideo(string filePath)
        {
            if (videoWindow == null)
            {
                videoWindow = new VideoWindow();
                videoWindow.Closed += (s, e) => videoWindow = null;
                videoWindow.Activate(); // Use Activate instead of Show
            }

            videoWindow.MediaPlayer.Source = MediaSource.CreateFromUri(new Uri(filePath));
            videoWindow.MediaPlayer.Play();
            PlayPauseButton.Content = "Pause";

            // Update the progress bar for the video player
            videoWindow.MediaPlayer.PlaybackSession.PositionChanged += UpdateProgressBar;
        }

        private void ProgressBar_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (currentCueIndex >= 0 && currentCueIndex < Cues.Count)
            {
                var currentCue = Cues[currentCueIndex];
                var newPosition = TimeSpan.FromSeconds(e.NewValue);

                try
                {
                    if (currentCue.Type == "Video" && videoWindow?.MediaPlayer != null)
                    {
                        videoWindow.MediaPlayer.PlaybackSession.Position = newPosition;
                    }
                    else if (mediaPlayer != null)
                    {
                        mediaPlayer.PlaybackSession.Position = newPosition;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error changing media position: {ex.Message}");
                }
            }
        }

        // Update the progress bar when the media playback position changes
        private void UpdateProgressBar(MediaPlaybackSession sender, object args)
        {
            m_dispatcherQueue.TryEnqueue(() =>
            {
                var position = sender.Position.TotalSeconds;
                var duration = sender.NaturalDuration.TotalSeconds;

                if (duration > 0)
                {
                    ProgressBar.Minimum = 0;
                    ProgressBar.Maximum = duration;
                    ProgressBar.Value = position;
                    ElapsedTimeText.Text = TimeSpan.FromSeconds(position).ToString(@"mm\:ss");
                    RemainingTimeText.Text = TimeSpan.FromSeconds(duration - position).ToString(@"mm\:ss");
                }
            });
        }

        private void VideoMediaPlayer_MediaOpened(MediaPlayer sender, object args)
        {
            m_dispatcherQueue.TryEnqueue(() =>
            {
                var session = sender.PlaybackSession;
                var duration = session.NaturalDuration.TotalSeconds;

                if (duration > 0)
                {
                    ProgressBar.Minimum = 0;
                    ProgressBar.Maximum = duration;
                    ProgressBar.Value = 0; // Start at the beginning

                    // Update time displays
                    ElapsedTimeText.Text = "0:00";
                    RemainingTimeText.Text = TimeSpan.FromSeconds(duration).ToString(@"mm\:ss");
                }
            });
        }
    }

    public class AudioCue
    {
        public int CueNumber { get; set; }
        public string Type { get; set; } = "Audio";
        public string Name { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        // Remove Duration property
    }

    public class ApplicationSettings
    {
        public ObservableCollection<string> AudioDirectories { get; } = new ObservableCollection<string>();
        public string DefaultCueType { get; set; } = "Audio";

        // You can add more settings properties here
    }
}
