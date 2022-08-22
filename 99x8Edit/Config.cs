using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;

namespace _99x8Edit
{
    // For appsettings.json
    public class ConfigRoot
    {
        public IndividualSettings Settings { get; set; } = new IndividualSettings();
    }
    public class IndividualSettings
    {
        public string ProjectFile { get; set; } = "";
        public string ImportDirectory { get; set; } = "";
        public string ExportDirectory { get; set; } = "";
        public string PCGFileDirectory { get; set; } = "";
        public string MapFileDirectory { get; set; } = "";
        public string SpriteFileDirectory { get; set; } = "";
        public string PaletteDirectory { get; set; } = "";
        public string PeekDirectory { get; set; } = "";
        public EditType EditControlType { get; set; } = EditType.Current;
    }
    public enum EditType
    {
        Current,
        Toggle
    }
    internal class Config
    {
        private static readonly string _filename = "appsettings.json";
        private static ConfigRoot _config = new ConfigRoot();
        internal static IndividualSettings Setting => _config.Settings;
        internal static void Load()
        {
            // Open the configuration file
            try
            {
                _config = new ConfigurationBuilder()
                    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                    .AddJsonFile(_filename)
                    .Build()
                    .Get<ConfigRoot>();
            }
            catch (Exception)
            {
                // Do nothing cause this may occur by invalid app setting file
            }
        }
        internal static void Save()
        {
            // Save the configuration file
            var jsonWriteOptions = new JsonSerializerOptions()
            {
                WriteIndented = true
            };
            jsonWriteOptions.Converters.Add(new JsonStringEnumConverter());
            string json = JsonSerializer.Serialize(_config, jsonWriteOptions);
            string appdir = AppDomain.CurrentDomain.BaseDirectory;
            if (appdir != null)
            {
                string path = Path.Combine(appdir, _filename);
                File.WriteAllText(path, json);
            }
        }
    }
}
