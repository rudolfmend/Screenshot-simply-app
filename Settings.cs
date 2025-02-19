using System.IO;
using System.Text.Json;

public enum ImageFormat
{
    Png,
    Jpeg
}

public class Settings
{
    private static readonly string DefaultFolderName = "Screenshot Simply App";
    private static readonly string SettingsFileName = "settings.json";
    private static readonly string SettingsFilePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "Screenshot Simply App",
        SettingsFileName);

    // Properties
    public string SavePath { get; set; }
    public ImageFormat ImageFormat { get; set; }
    public int JpegQuality { get; set; }
    public string FileNamePrefix { get; set; }
    public bool MinimizeOnStartup { get; set; }
    public bool PlaySoundOnCapture { get; set; }

    public Settings()
    {
        // Default values
        SavePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
            DefaultFolderName);
        ImageFormat = ImageFormat.Png;
        JpegQuality = 90;
        FileNamePrefix = "Screenshot";
        MinimizeOnStartup = false;
        PlaySoundOnCapture = true;
    }

    public string GenerateFileName()
    {
        string extension = ImageFormat == ImageFormat.Png ? ".png" : ".jpg";
        return $"{FileNamePrefix}_{DateTime.Now:yyyyMMdd_HHmmss}{extension}";
    }

    public static Settings Load()
    {
        try
        {
            if (File.Exists(SettingsFilePath))
            {
                string json = File.ReadAllText(SettingsFilePath);
                var settings = JsonSerializer.Deserialize<Settings>(json);
                return settings ?? new Settings();
            }
        }
        catch
        {
            // If there's any error reading the settings, return defaults
        }
        return new Settings();
    }

    public void Save()
    {
        try
        {
            string? directoryPath = Path.GetDirectoryName(SettingsFilePath);
            if (string.IsNullOrEmpty(directoryPath))
            {
                throw new Exception("Invalid settings file path");
            }

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            string json = JsonSerializer.Serialize(this, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            File.WriteAllText(SettingsFilePath, json);
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to save settings", ex);
        }
    }

    public void EnsureSavePathExists()
    {
        if (!Directory.Exists(SavePath))
        {
            Directory.CreateDirectory(SavePath);
        }
    }
}