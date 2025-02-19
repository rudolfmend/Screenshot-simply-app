using System.Windows;
using System.Windows.Controls;
using WinForms = System.Windows.Forms;

namespace Screenshot_2_WpfApp
{
    public partial class SettingsWindow : Window
    {
        private readonly Settings _settings;
        private bool _isInitialized = false;

        public SettingsWindow(Settings settings)
        {
            _settings = settings;
            InitializeComponent();
            _isInitialized = true;  // Nastavíme flag PRED LoadSettings
            LoadSettings();
            UpdatePreview();
        }

        private void FormatComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isInitialized || _settings == null || QualitySlider == null)
            {
                System.Diagnostics.Debug.WriteLine("Format change ignored - not initialized");
                return;
            }

            bool isJpeg = FormatComboBox.SelectedIndex == 1;
            QualitySlider.IsEnabled = isJpeg;
            System.Diagnostics.Debug.WriteLine($"Format changed to JPEG: {isJpeg}, Slider enabled: {QualitySlider.IsEnabled}");

            _settings.ImageFormat = isJpeg ? ImageFormat.Jpeg : ImageFormat.Png;
            UpdatePreview();
        }

        private void QualitySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            System.Diagnostics.Debug.WriteLine($"Value change - IsInitialized: {_isInitialized}, Slider enabled: {QualitySlider.IsEnabled}, New value: {e.NewValue}");

            if (!_isInitialized || _settings == null || QualitySlider == null)
            {
                System.Diagnostics.Debug.WriteLine("Slider value change ignored - not initialized");
                return;
            }

            _settings.JpegQuality = (int)e.NewValue;
            System.Diagnostics.Debug.WriteLine($"Quality updated to: {_settings.JpegQuality}");
            UpdatePreview();
        }

        private void UpdatePreview()
        {
            try
            {
                if (!_isInitialized || PreviewTextBox == null || _settings == null) return;
                PreviewTextBox.Text = _settings.GenerateFileName();
            }
            catch
            {
                // Ignorujeme chyby počas inicializácie
            }
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_isInitialized || _settings == null) return;

            using var dialog = new WinForms.FolderBrowserDialog
            {
                Description = "Select folder for saving screenshots",
                SelectedPath = _settings.SavePath
            };

            if (dialog.ShowDialog() == WinForms.DialogResult.OK)
            {
                PathTextBox.Text = dialog.SelectedPath;
                UpdatePreview();
            }
        }

        private void LoadSettings()
        {
            if (_settings == null) return;

            // Najprv nastavíme formát, ktorý ovplyvňuje enabled stav slidera
            FormatComboBox.SelectedIndex = _settings.ImageFormat == ImageFormat.Png ? 0 : 1;

            // Potom nastavíme slider
            QualitySlider.IsEnabled = _settings.ImageFormat == ImageFormat.Jpeg;
            QualitySlider.Value = _settings.JpegQuality;

            // Ostatné nastavenia
            PathTextBox.Text = _settings.SavePath;
            PrefixTextBox.Text = _settings.FileNamePrefix;
            MinimizeCheckBox.IsChecked = _settings.MinimizeOnStartup;
            SoundCheckBox.IsChecked = _settings.PlaySoundOnCapture;
        }

        private void PrefixTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isInitialized || _settings == null || PrefixTextBox == null) return;

            _settings.FileNamePrefix = PrefixTextBox.Text;
            UpdatePreview();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_isInitialized || _settings == null) return;

            try
            {
                _settings.SavePath = PathTextBox.Text;
                _settings.ImageFormat = FormatComboBox.SelectedIndex == 0 ? ImageFormat.Png : ImageFormat.Jpeg;
                _settings.JpegQuality = (int)QualitySlider.Value;
                _settings.FileNamePrefix = PrefixTextBox.Text;
                _settings.MinimizeOnStartup = MinimizeCheckBox.IsChecked ?? false;
                _settings.PlaySoundOnCapture = SoundCheckBox.IsChecked ?? false;

                _settings.EnsureSavePathExists();
                _settings.Save();

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(
                    this,
                    $"Error saving settings: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}
