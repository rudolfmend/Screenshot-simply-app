using System.Diagnostics; // Alias Windows.Forms
using System.Diagnostics.CodeAnalysis;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using MessageBox = System.Windows.MessageBox;
using WinForms = System.Windows.Forms;

namespace Screenshot_2_WpfApp
{
    public partial class MainWindow : Window
    {
        // Win32 API constants
        private const int WM_HOTKEY = 0x0312;
        private const int MOD_SHIFT = 0x0004;
        private const int VK_C = 0x43; // Virtual key code for 'C'   (Shift + C)
        private const int VK_S = 0x53; // Virtual key code for 'S'   (Shift + S)
        private const int VK_F = 0x46; // Virtual key code for 'F'   (Shift + F)

        // Unique hotkey IDs
        private const int HOTKEY_ID_CAPTURE = 9000;
        private const int HOTKEY_ID_SETTINGS = 9001;
        private const int HOTKEY_ID_FOLDER = 9002;


        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private HwndSource? _source;
        private IntPtr _windowHandle;
        private Settings _settings;


        public MainWindow()
        {
            InitializeComponent();
            _settings = Settings.Load();
            _settings.EnsureSavePathExists();

            this.Loaded += MainWindow_Loaded;
            this.Closing += MainWindow_Closing;

            if (_settings.MinimizeOnStartup)
            {
                this.WindowState = WindowState.Minimized;
            }
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _windowHandle = new WindowInteropHelper(this).Handle;
            _source = HwndSource.FromHwnd(_windowHandle);
            _source?.AddHook(HwndHook);
            // Register Shift+C as a keyboard shortcut
            RegisterHotKey(_windowHandle, HOTKEY_ID_CAPTURE, MOD_SHIFT, VK_C);  // Shift + C "Capture - Shift + C"
            RegisterHotKey(_windowHandle, HOTKEY_ID_SETTINGS, MOD_SHIFT, VK_S); // Shift + S "Settings - Shift + S"
            RegisterHotKey(_windowHandle, HOTKEY_ID_FOLDER, MOD_SHIFT, VK_F);   // Shift + F "Open Folder - Shift + F"
        }

        private void MainWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            // Unregister the keyboard shortcut when closing
            UnregisterHotKey(_windowHandle, HOTKEY_ID_CAPTURE);
            UnregisterHotKey(_windowHandle, HOTKEY_ID_SETTINGS);
            UnregisterHotKey(_windowHandle, HOTKEY_ID_FOLDER);
            _source?.RemoveHook(HwndHook);
        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_HOTKEY)
            {
                int hotkeyId = wParam.ToInt32();
                handled = true;

                switch (hotkeyId)
                {
                    case HOTKEY_ID_CAPTURE:
                        CaptureButton_Click(this, new RoutedEventArgs());
                        break;
                    // Replace the line causing the error
                    case HOTKEY_ID_SETTINGS:
                        SettingsButton_Click(this, new RoutedEventArgs());
                        break;
                    case HOTKEY_ID_FOLDER:
                        OpenFolderButton_Click(this, new RoutedEventArgs());
                        break;
                    default:
                        handled = false;
                        break;
                }
            }
            return IntPtr.Zero;
        }

        private async void CaptureButton_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("---------- CAPTURE SEQUENCE STARTED ----------");
            try
            {
                Debug.WriteLine("Minimizing window...");
                this.WindowState = WindowState.Minimized;
                await Task.Delay(500);
                Debug.WriteLine("Window minimized, proceeding with capture");

                var screen = WinForms.Screen.PrimaryScreen;
                if (screen == null)
                {
                    Debug.WriteLine("ERROR: Primary screen not found");
                    this.WindowState = WindowState.Normal;
                    MessageBox.Show(this,
                        "Unable to find primary screen.", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                Debug.WriteLine($"Screen detected: {screen.Bounds.Width}x{screen.Bounds.Height}");

                try
                {
                    Debug.WriteLine("Creating bitmap...");
                    using Bitmap bitmap = new(
                        screen.Bounds.Width,
                        screen.Bounds.Height);
                    using (Graphics graphics = Graphics.FromImage(bitmap))
                    {
                        Debug.WriteLine("Copying screen to bitmap...");
                        graphics.CopyFromScreen(0, 0, 0, 0, bitmap.Size);
                    }
                    Debug.WriteLine("Screen captured successfully");

                    // Ensure save directory exists
                    Debug.WriteLine($"Checking save directory: {_settings.SavePath}");
                    if (!Directory.Exists(_settings.SavePath))
                    {
                        Debug.WriteLine("Creating save directory...");
                        Directory.CreateDirectory(_settings.SavePath);
                    }

                    string fileName = Path.Combine(_settings.SavePath, _settings.GenerateFileName());
                    Debug.WriteLine($"Saving to file: {fileName}");

                    if (_settings.ImageFormat == ImageFormat.Png)
                    {
                        Debug.WriteLine("Saving as PNG...");
                        bitmap.Save(fileName, System.Drawing.Imaging.ImageFormat.Png);
                    }
                    else
                    {
                        Debug.WriteLine($"Saving as JPEG (quality: {_settings.JpegQuality})...");
                        var jpegEncoder = GetEncoderInfo("image/jpeg");
                        var encoderParameters = new EncoderParameters(1);
                        encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, _settings.JpegQuality);
                        bitmap.Save(fileName, jpegEncoder, encoderParameters);
                    }
                    Debug.WriteLine("File saved successfully");

                    if (_settings.PlaySoundOnCapture)
                    {
                        Debug.WriteLine("Playing notification sound");
                        System.Media.SystemSounds.Asterisk.Play();
                    }

                    Debug.WriteLine("Restoring window...");
                    this.WindowState = WindowState.Normal;
                    this.Activate();
                    this.Focus();
                    Debug.WriteLine("Window restored");

                    Debug.WriteLine("Showing success message");
                    ShowSuccessMessage();
                }
                catch (UnauthorizedAccessException ex)
                {
                    Debug.WriteLine($"PERMISSION ERROR: {ex.Message}");
                    Debug.WriteLine(ex.StackTrace);

                    this.WindowState = WindowState.Normal;
                    this.Activate();
                    this.Focus();

                    Debug.WriteLine("Showing permission request dialog");
                    var result = MessageBox.Show(this,
                        "Screenshot Simply App needs permission to capture your screen and save files. Would you like to grant these permissions?",
                        "Permission Required",
                        MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        Debug.WriteLine("User agreed to grant permissions, initializing permission requests...");
                        try
                        {
                            Debug.WriteLine("Requesting screen capture permission...");
                            var picker = new Windows.Graphics.Capture.GraphicsCapturePicker();
                            await picker.PickSingleItemAsync();

                            Debug.WriteLine("Testing file system access...");
                            string testPath = Path.Combine(_settings.SavePath, "test_permission.tmp");
                            File.WriteAllText(testPath, "test");
                            File.Delete(testPath);

                            Debug.WriteLine("Permissions successfully obtained");
                            MessageBox.Show(this,
                                "Permissions granted. Please try capturing the screen again.",
                                "Permissions Updated",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        catch (Exception permEx)
                        {
                            Debug.WriteLine($"PERMISSION REQUEST FAILED: {permEx.Message}");
                            Debug.WriteLine(permEx.StackTrace);

                            MessageBox.Show(this,
                                $"Could not obtain necessary permissions: {permEx.Message}\n\nPlease check your system settings and try again.",
                                "Permission Error",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else
                    {
                        Debug.WriteLine("User declined permission request");
                    }
                }
                catch (Exception primaryEx)
                {
                    Debug.WriteLine($"Primary screen capture failed: {primaryEx.Message}");
                    try
                    {
                        Debug.WriteLine("Attempting alternative capture method...");
                        using Bitmap bitmap2 = new(screen.Bounds.Width, screen.Bounds.Height);
                        using (Graphics graphics = Graphics.FromImage(bitmap2))
                        {
                            // Použitie iného prístupu
                            graphics.CopyFromScreen(screen.Bounds.X, screen.Bounds.Y, 0, 0, screen.Bounds.Size);
                        }

                        // Ensure save directory exists
                        Debug.WriteLine($"Checking save directory: {_settings.SavePath}");
                        if (!Directory.Exists(_settings.SavePath))
                        {
                            Debug.WriteLine("Creating save directory...");
                            Directory.CreateDirectory(_settings.SavePath);
                        }

                        string fileName = Path.Combine(_settings.SavePath, _settings.GenerateFileName());
                        Debug.WriteLine($"Saving to file using alternative method: {fileName}");

                        if (_settings.ImageFormat == ImageFormat.Png)
                        {
                            Debug.WriteLine("Saving as PNG (alternative)...");
                            bitmap2.Save(fileName, System.Drawing.Imaging.ImageFormat.Png);
                        }
                        else
                        {
                            Debug.WriteLine($"Saving as JPEG (alternative, quality: {_settings.JpegQuality})...");
                            var jpegEncoder = GetEncoderInfo("image/jpeg");
                            var encoderParameters = new EncoderParameters(1);
                            encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, _settings.JpegQuality);
                            bitmap2.Save(fileName, jpegEncoder, encoderParameters);
                        }
                        Debug.WriteLine("File saved successfully using alternative method");

                        // ► ♪ ♫ ♪ ♫ ◄
                        if (_settings.PlaySoundOnCapture)
                        {
                            Debug.WriteLine("Playing notification sound");
                            try
                            {
                                var dispatcher = System.Windows.Application.Current.Dispatcher;
                                dispatcher.Invoke(() => {
                                    var player = new System.Windows.Media.MediaPlayer();
                                    player.Open(new Uri("ms-winsoundevent:Notification.Default", UriKind.Absolute));
                                    player.Play();
                                });
                            }
                            catch (Exception soundEx)
                            {
                                Debug.WriteLine($"Failed to play sound: {soundEx.Message}");
                            }
                        }

                        Debug.WriteLine("Restoring window...");
                        this.WindowState = WindowState.Normal;
                        this.Activate();
                        this.Focus();
                        Debug.WriteLine("Window restored");

                        Debug.WriteLine("Showing success message");
                        ShowSuccessMessage();
                    }
                    catch (Exception secondaryEx)
                    {
                        
                        // When both methods fail, log the full stack trace for better diagnostics
                        Debug.WriteLine($"Secondary screen capture failed: {secondaryEx.Message}");
                        Debug.WriteLine(secondaryEx.StackTrace);

                        this.WindowState = WindowState.Normal;
                        this.Activate();
                        this.Focus();

                        MessageBox.Show(this,
                            $"All screen capture methods failed.\nPrimary error: {primaryEx.Message}\nSecondary error: {secondaryEx.Message}",
                            "Capture Error",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"CRITICAL ERROR: {ex.Message}");
                Debug.WriteLine(ex.StackTrace);

                this.WindowState = WindowState.Normal;
                this.Activate();
                this.Focus();

                MessageBox.Show(this,
                    $"Failed to capture screenshot: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            Debug.WriteLine("---------- CAPTURE SEQUENCE ENDED ----------");
        }

        // Auxiliary method for checking permissions
        private static async Task<bool> CheckGraphicsCapturePermission()
        {
            try
            {
                // Skontroluj, či systém podporuje zachytávanie obrazovky
                bool isSupported = Windows.Graphics.Capture.GraphicsCaptureSession.IsSupported();
                if (!isSupported)
                {
                    Debug.WriteLine("Graphics capture is not supported on this device");
                    return false;
                }

                // Skontroluj oprávnenie pre prístup k priečinku
                try
                {
                    Directory.CreateDirectory(Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
                        "ScreenshotSimplyApp"));
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Folder access error: {ex.Message}");
                    // Pokračujeme, pretože používateľ mohol zvoliť iný priečinok
                }

                // Na Windows potrebujeme spustiť testovací capture, aby sme inicializovali 
                // systémový dialóg pre povolenia, ak ešte nebol udelený
                try
                {
                    var picker = new Windows.Graphics.Capture.GraphicsCapturePicker();
                    await picker.PickSingleItemAsync();
                    // Aj keď používateľ zruší výber, znamená to, že dialóg sa zobrazil
                    // a oprávnenie bolo získané alebo odmietnuté
                    return true;
                }
                catch (Exception)
                {
                    // Ak tu nastane výnimka, možno používateľ zakázal oprávnenie
                    // alebo došlo k inému problému
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Permission check failed: {ex.Message}");
                return false;
            }
        }

        // Auxiliary method for obtaining a JPEG encoder
        private static ImageCodecInfo GetEncoderInfo(string mimeType)
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

        // Helper method to show success message
        private void ShowSuccessMessage()
        {
            // custom non-modal MessageBox and auto-close
            var messageWindow = new System.Windows.Window
            {
                Title = "Success",
                Width = 380,
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
                Margin = new Thickness(0, 10, 0, 0),
                FontSize = 20
            };

            // TextBlock for countdown
            var countdownText = new System.Windows.Controls.TextBlock
            {
                Text = "The window will close in 5 seconds.",
                HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                Margin = new Thickness(0, 5, 0, 20),
                FontSize = 16,
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
                    countdownText.Text = $"The window will close in {secondsLeft} {(secondsLeft == 1 ? "second" : "seconds")}";
                }
            };
            autoCloseTimer.Start();

            messageWindow.Show();
        }
    }
}
