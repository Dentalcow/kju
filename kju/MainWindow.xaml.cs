using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
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
        // Add a field for the video window
        private VideoWindow? videoWindow;
        public ObservableCollection<AudioCue> Cues { get; } = new ObservableCollection<AudioCue>();
        public ApplicationSettings AppSettings { get; private set; }
        private MediaPlayer mediaPlayer;
        private int currentCueIndex = -1;
        private IntPtr m_windowHandle;
        private DispatcherQueue m_dispatcherQueue = null!;

        public MainWindow()
        {
            this.InitializeComponent();

            // Initialize settings
            AppSettings = new ApplicationSettings();

            // Initialize window handle and dispatcher queue
            InitializeWindowHandling();

            mediaPlayer = new MediaPlayer();
            mediaPlayer.PlaybackSession.PositionChanged += UpdateProgressBar;

            // IMPORTANT: Explicitly set initial cue index
            currentCueIndex = Cues.Count > 0 ? 0 : -1;

            // Optional: Ensure video window is closed when main window closes
            Closed += (s, e) =>
            {
                videoWindow?.Close();
            };
        }

        private void InitializeWindowHandling()
        {
            // Get the window handle
            m_windowHandle = WindowNative.GetWindowHandle(this);

            // Initialize dispatcher queue
            m_dispatcherQueue = DispatcherQueue.GetForCurrentThread();
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
            defaultTypeComboBox.Items.Add("Music");
            defaultTypeComboBox.Items.Add("SFX");
            defaultTypeComboBox.Items.Add("Voice");
            defaultTypeComboBox.Items.Add("Other");
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

        private async void ImportFiles_Click(object sender, RoutedEventArgs e)
        {
            await ImportFilesWithPicker();
        }

        private async Task ImportFilesWithPicker()
        {
            try
            {
                // Create file picker
                var picker = new FileOpenPicker();
                picker.SuggestedStartLocation = PickerLocationId.MusicLibrary;
                picker.FileTypeFilter.Add(".mp3");
                picker.FileTypeFilter.Add(".wav");
                picker.FileTypeFilter.Add(".flac");
                picker.ViewMode = PickerViewMode.List;

                // Initialize the picker with the window handle
                WinRT.Interop.InitializeWithWindow.Initialize(picker, m_windowHandle);

                // Pick files
                var files = await picker.PickMultipleFilesAsync();
                if (files != null && files.Count > 0)
                {
                    await ImportFilesWithTypeSelection(files);
                }
            }
            catch (Exception ex)
            {
                await DisplayErrorMessage($"File import error: {ex.Message}");
            }
        }

        private async Task ImportFilesWithTypeSelection(IReadOnlyList<StorageFile> files)
        {
            var dialog = new ContentDialog
            {
                Title = "Select Cue Type",
                PrimaryButtonText = "Import",
                CloseButtonText = "Cancel",
                XamlRoot = this.Content.XamlRoot
            };

            var typeComboBox = new ComboBox
            {
                PlaceholderText = "Select Cue Type"
            };
            typeComboBox.Items.Add("Audio");
            typeComboBox.Items.Add("Music");
            typeComboBox.Items.Add("SFX");
            typeComboBox.Items.Add("Voice");
            typeComboBox.Items.Add("Video");  // Added Video type
            typeComboBox.Items.Add("Other");

            dialog.Content = new StackPanel
            {
                Children = {
            new TextBlock { Text = "Choose a type for these files:" },
            typeComboBox
        }
            };

            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary && typeComboBox.SelectedItem != null)
            {
                string selectedType = typeComboBox.SelectedItem.ToString() ?? "Audio";

                foreach (var file in files.OrderBy(f => f.Name))
                {
                    AddCue(file.Path, selectedType);
                }
            }
        }

        private async void AddCue_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await AddCueWithFilePicker();
            }
            catch (Exception ex)
            {
                await DisplayErrorMessage($"An error occurred: {ex.Message}");
            }
        }

        // Update AddCueWithFilePicker to include Video type
        private async Task AddCueWithFilePicker()
        {
            var dialog = new ContentDialog
            {
                Title = "Add New Cue",
                PrimaryButtonText = "Add",
                CloseButtonText = "Cancel",
                XamlRoot = this.Content.XamlRoot
            };

            var typeComboBox = new ComboBox
            {
                PlaceholderText = "Select Cue Type",
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
            typeComboBox.Items.Add("Audio");
            typeComboBox.Items.Add("Music");
            typeComboBox.Items.Add("SFX");
            typeComboBox.Items.Add("Voice");
            typeComboBox.Items.Add("Video");  // Added Video type
            typeComboBox.Items.Add("Other");

            var filePickerButton = new Button
            {
                Content = "Select File",
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
            var selectedFileTextBlock = new TextBlock
            {
                Text = "No file selected",
                TextWrapping = TextWrapping.Wrap
            };

            StorageFile? selectedFile = null;
            filePickerButton.Click += async (s, args) =>
            {
                try
                {
                    var picker = new FileOpenPicker();
                    picker.SuggestedStartLocation = PickerLocationId.MusicLibrary;
                    // Update file type filters to include video and audio
                    picker.FileTypeFilter.Add(".mp3");
                    picker.FileTypeFilter.Add(".wav");
                    picker.FileTypeFilter.Add(".flac");
                    picker.FileTypeFilter.Add(".mp4");
                    picker.FileTypeFilter.Add(".avi");
                    picker.FileTypeFilter.Add(".mov");
                    picker.FileTypeFilter.Add(".mkv");

                    WinRT.Interop.InitializeWithWindow.Initialize(picker, m_windowHandle);

                    selectedFile = await picker.PickSingleFileAsync();
                    if (selectedFile != null)
                    {
                        selectedFileTextBlock.Text = selectedFile.Name;
                    }
                }
                catch (Exception ex)
                {
                    selectedFileTextBlock.Text = $"Error selecting file: {ex.Message}";
                    selectedFile = null;
                }
            };

            var stackPanel = new StackPanel { Spacing = 10 };
            stackPanel.Children.Add(new TextBlock { Text = "Cue Type:" });
            stackPanel.Children.Add(typeComboBox);
            stackPanel.Children.Add(filePickerButton);
            stackPanel.Children.Add(selectedFileTextBlock);

            dialog.Content = stackPanel;

            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary
                && typeComboBox.SelectedItem != null
                && selectedFile != null)
            {
                AddCue(
                    selectedFile.Path,
                    typeComboBox.SelectedItem.ToString() ?? "Audio"
                );
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

        // Modify AddCue to set a default type
        private void AddCue(string filePath, string? type = null, string? name = null)
        {
            // Determine type based on file extension if not provided
            string[] videoExtensions = { ".mp4", ".avi", ".mov", ".mkv", ".wmv" };
            string[] audioExtensions = { ".mp3", ".wav", ".flac" };

            if (type == null)
            {
                if (videoExtensions.Any(ext => filePath.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
                {
                    type = "Video";
                }
                else if (audioExtensions.Any(ext => filePath.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
                {
                    type = "Audio";
                }
                else
                {
                    type = "Other";
                }
            }

            var cue = new AudioCue
            {
                CueNumber = Cues.Count + 1,
                Type = type,
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

        // Update PlayPause_Click to handle video window
        private void PlayPause_Click(object sender, RoutedEventArgs e)
        {
            if (Cues.Count == 0 || currentCueIndex < 0 || currentCueIndex >= Cues.Count) return;

            var currentCue = Cues[currentCueIndex];

            if (currentCue.IsVideo)
            {
                // Ensure video window exists
                if (videoWindow == null)
                {
                    videoWindow = new VideoWindow();
                    videoWindow.PlayVideo(currentCue.FilePath);
                    videoWindow.Activate(); // Use Activate instead of Show
                    PlayPauseButton.Content = "Pause";
                    return;
                }

                // Toggle play/pause for video
                var playbackState = videoWindow.MediaPlayer.PlaybackSession.PlaybackState;
                if (playbackState == MediaPlaybackState.Playing)
                {
                    videoWindow.PauseVideo();
                    PlayPauseButton.Content = "Play";
                }
                else
                {
                    videoWindow.ResumeVideo();
                    PlayPauseButton.Content = "Pause";
                }
            }
            else
            {
                // Existing audio playback toggle logic
                var playbackState = mediaPlayer.PlaybackSession.PlaybackState;

                if (playbackState == MediaPlaybackState.Playing)
                {
                    mediaPlayer.Pause();
                    PlayPauseButton.Content = "Play";
                }
                else
                {
                    mediaPlayer.Play();
                    PlayPauseButton.Content = "Pause";
                }
            }
        }

        private void GoMinus_Click(object sender, RoutedEventArgs e)
        {
            if (Cues.Count > 0) // Ensure cues exist
            {
                currentCueIndex = Math.Max(0, currentCueIndex - 1);
                PlayAudio();
            }
        }

        private void GoPlus_Click(object sender, RoutedEventArgs e)
        {
            if (Cues.Count > 0) // Ensure cues exist
            {
                currentCueIndex = Math.Min(Cues.Count - 1, currentCueIndex + 1);
                PlayAudio();
            }
        }


        private void PlayAudio()
        {
            if (Cues == null || Cues.Count == 0)
            {
                ShowErrorMessage("No cues available to play.");
                return;
            }

            // Ensure valid index
            if (currentCueIndex < 0) currentCueIndex = 0;
            if (currentCueIndex >= Cues.Count) currentCueIndex = Cues.Count - 1;

            // Add the missing closing brace here
        }

        // Helper method to show error messages
        private async void ShowErrorMessage(string message)
        {
            try
            {
                var errorDialog = new ContentDialog
                {
                    Title = "Playback Error",
                    Content = message,
                    CloseButtonText = "OK",
                    XamlRoot = this.Content.XamlRoot
                };

                await errorDialog.ShowAsync();
            }
            catch (Exception ex)
            {
                // Fallback error logging if dialog fails
                System.Diagnostics.Debug.WriteLine($"ERROR showing error dialog: {ex.Message}");
            }
        }

        // Update the media playback position when the progress bar value changes
        private void ProgressBar_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (mediaPlayer != null && mediaPlayer.PlaybackSession != null)
            {
                mediaPlayer.PlaybackSession.Position = TimeSpan.FromSeconds(e.NewValue);
            }
        }

        // Update the progress bar when the media playback position changes
        private void UpdateProgressBar(MediaPlaybackSession sender, object args)
        {
            var position = sender.Position.TotalSeconds;
            var duration = sender.NaturalDuration.TotalSeconds;

            if (duration > 0)
            {
                m_dispatcherQueue.TryEnqueue(() =>
                {
                    ProgressBar.Minimum = 0;
                    ProgressBar.Maximum = duration;
                    ProgressBar.Value = position;
                    ElapsedTimeText.Text = TimeSpan.FromSeconds(position).ToString(@"mm\:ss");
                    RemainingTimeText.Text = TimeSpan.FromSeconds(duration - position).ToString(@"mm\:ss");
                });
            }
        }
        // You might want to add a method to close the video window explicitly
        private void CloseVideoWindow()
        {
            videoWindow?.Close();
            videoWindow = null;
        }
    }

    public class AudioCue
    {
        public int CueNumber { get; set; }
        public string Type { get; set; } = "Audio";
        public string Name { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public double Duration { get; set; }

        // Add this property to determine if the cue is a video
        public bool IsVideo
        {
            get
            {
                // Add video file extensions
                string[] videoExtensions = { ".mp4", ".avi", ".mov", ".mkv", ".wmv" };
                return videoExtensions.Any(ext => FilePath.EndsWith(ext, StringComparison.OrdinalIgnoreCase));
            }
        }
    }

    public class ApplicationSettings
    {
        public ObservableCollection<string> AudioDirectories { get; } = new ObservableCollection<string>();
        public string DefaultCueType { get; set; } = "Audio";

        // You can add more settings properties here
    }
}
