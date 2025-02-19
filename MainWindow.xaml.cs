using Microsoft.Win32;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
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
                        "Nepodarilo sa nájsť primárnu obrazovku.", "Chyba",
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

                    // Restore window first
                    this.WindowState = WindowState.Normal;
                    this.Activate(); // Ensure the window is active
                    this.Focus();    // Ensure the window has focus

                    // Then show message box
                    MessageBox.Show(this,
                        "Screenshot was successfully saved.",
                        "Success",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
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

        // Pomocná metóda pre získanie JPEG encodera
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
                    $"Chyba pri otváraní priečinka: {ex.Message}",
                    "Chyba",
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
