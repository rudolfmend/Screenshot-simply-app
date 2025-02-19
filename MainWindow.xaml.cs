using Microsoft.Win32;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Windows.Forms;
using MessageBox = System.Windows.MessageBox;
using WinForms = System.Windows.Forms; // Alias Windows.Forms

namespace Screenshot_2_WpfApp
{
    public partial class MainWindow : Window
    {
        private Settings _settings;
        public MainWindow()
        {
            InitializeComponent();
            _settings = Settings.Load();
            _settings.EnsureSavePathExists();

            if (_settings.MinimizeOnStartup)
            {
                this.WindowState = WindowState.Minimized;
            }
        }

        private async void CaptureButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Minimize window to not capture it
                this.WindowState = WindowState.Minimized;
                await Task.Delay(500);

                var screen = WinForms.Screen.PrimaryScreen;
                if (screen == null)
                {
                    this.WindowState = WindowState.Normal; // Restore window first
                    MessageBox.Show(this,
                        "Unable to find primary screen.", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Capture screen
                using (Bitmap bitmap = new Bitmap(
                    screen.Bounds.Width,
                    screen.Bounds.Height))
                {
                    using (Graphics graphics = Graphics.FromImage(bitmap))
                    {
                        graphics.CopyFromScreen(0, 0, 0, 0, bitmap.Size);
                    }

                    string fileName = Path.Combine(_settings.SavePath, _settings.GenerateFileName());

                    if (_settings.ImageFormat == ImageFormat.Png)
                    {
                        bitmap.Save(fileName, System.Drawing.Imaging.ImageFormat.Png);
                    }
                    else
                    {
                        // set JPEG quality
                        var jpegEncoder = GetEncoderInfo("image/jpeg");
                        var encoderParameters = new EncoderParameters(1);
                        encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, _settings.JpegQuality);
                        bitmap.Save(fileName, jpegEncoder, encoderParameters);
                    }

                    if (_settings.PlaySoundOnCapture)
                    {
                        System.Media.SystemSounds.Asterisk.Play();
                    }

                    // The window will be refreshed first
                    this.WindowState = WindowState.Normal;
                    this.Activate(); // Ensure the window is active
                    this.Focus();    // Ensure the window has focus

                    // custom non-modal MessageBox and auto-close
                    var messageWindow = new System.Windows.Window
                    {
                        Title = "Success",
                        Width = 340,
                        Height = 240,
                        WindowStartupLocation = WindowStartupLocation.CenterOwner,
                        Owner = this,
                        ResizeMode = ResizeMode.NoResize,
                        ShowInTaskbar = false,
                        WindowStyle = WindowStyle.ToolWindow
                    };

                    var messagePanel = new System.Windows.Controls.StackPanel
                    {
                        Margin = new Thickness(20)
                    };

                    var messageText = new System.Windows.Controls.TextBlock
                    {
                        Text = "Screenshot was successfully saved.",
                        TextWrapping = TextWrapping.Wrap,
                        HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                        VerticalAlignment = System.Windows.VerticalAlignment.Center,
                        Margin = new Thickness(0, 10, 0, 0) // Zmenšil som spodný okraj
                    };

                    // TextBlock for countdown
                    var countdownText = new System.Windows.Controls.TextBlock
                    {
                        Text = "The window will close in 5 seconds.",
                        HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                        Margin = new Thickness(0, 5, 0, 20),
                        FontSize = 11,
                        Foreground = System.Windows.Media.Brushes.Gray
                    };

                    var okButton = new System.Windows.Controls.Button
                    {
                        Content = "OK",
                        Width = 180,
                        Height = 48,
                        HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                    };

                    // Rounded button style
                    var buttonStyle = new System.Windows.Style(typeof(System.Windows.Controls.Button));
                    var buttonTemplate = new System.Windows.Controls.ControlTemplate(typeof(System.Windows.Controls.Button));

                    // Rounded border
                    var border = new System.Windows.FrameworkElementFactory(typeof(System.Windows.Controls.Border));
                    border.SetValue(System.Windows.Controls.Border.CornerRadiusProperty, new System.Windows.CornerRadius(8));
                    border.SetValue(System.Windows.Controls.Border.BackgroundProperty, System.Windows.SystemColors.ControlBrush);

                    // Content of button (ContentPresenter)
                    var contentPresenter = new System.Windows.FrameworkElementFactory(typeof(System.Windows.Controls.ContentPresenter));
                    contentPresenter.SetValue(System.Windows.Controls.ContentPresenter.HorizontalAlignmentProperty, System.Windows.HorizontalAlignment.Center);
                    contentPresenter.SetValue(System.Windows.Controls.ContentPresenter.VerticalAlignmentProperty, System.Windows.VerticalAlignment.Center);
                    border.AppendChild(contentPresenter);

                    buttonTemplate.VisualTree = border;
                    buttonStyle.Setters.Add(new System.Windows.Setter(System.Windows.Controls.Control.TemplateProperty, buttonTemplate));
                    okButton.Style = buttonStyle;

                    okButton.Click += (s, args) => messageWindow.Close();

                    // Add controls to the panel
                    messagePanel.Children.Add(messageText);
                    messagePanel.Children.Add(countdownText);
                    messagePanel.Children.Add(okButton);
                    messageWindow.Content = messagePanel;

                    // Seconds counter
                    int secondsLeft = 5;

                    // countdown timer
                    var autoCloseTimer = new System.Windows.Threading.DispatcherTimer
                    {
                        Interval = TimeSpan.FromSeconds(1)
                    };
                    autoCloseTimer.Tick += (s, args) =>
                    {
                        secondsLeft--;
                        if (secondsLeft <= 0)
                        {
                            autoCloseTimer.Stop();
                            messageWindow.Close();
                        }
                        else
                        {
                            // Update countdown text
                            countdownText.Text = $"The window will close in {secondsLeft} {(secondsLeft == 1 ? "second" : (secondsLeft > 1 && secondsLeft < 5 ? "seconds" : "seconds"))}";
                        }
                    };
                    autoCloseTimer.Start();

                    messageWindow.Show();
                }
            }
            catch (Exception ex)
            {
                this.WindowState = WindowState.Normal; // Restore window first
                this.Activate(); // Ensure the window is active
                this.Focus();    // Ensure the window has focus

                MessageBox.Show(this,
                    $"Failed to capture screenshot: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        // Auxiliary method for obtaining a JPEG encoder
        private ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            var codecs = ImageCodecInfo.GetImageEncoders();
            return codecs.First(codec => codec.MimeType == mimeType);
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            var settingsWindow = new SettingsWindow(_settings)
            {
                Owner = this
            };

            if (settingsWindow.ShowDialog() == true)
            {
                // Settings was saved successfully - reload them to update UI if needed
                _settings = Settings.Load();
            }
        }

        private async void OpenFolderButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await Windows.System.Launcher.LaunchFolderPathAsync(_settings.SavePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    this,
                    $"Error opening folder: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void AboutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "Screenshot Simple App\n\n" +
                "A simple screenshot capture app for quickly saving screenshots.\n\n" +

                "© 2025 Rudolf Mendzezof",
                "About the app",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
    }
}
