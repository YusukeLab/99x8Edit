using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;

namespace _99x8Edit
{
    public class ConfigRoot
    {
        public IndivisualSettings Settings { get; set; } = new IndivisualSettings();
    }
    public class IndivisualSettings
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
        internal static IndivisualSettings Setting
        {
            get => _config.Settings;
        }
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
            catch {}
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
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _filename);
            File.WriteAllText(path, json);
        }
    }
}
