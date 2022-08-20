using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace _99x8Edit
{
    public class Config
    {
        // Application configuratoins
        public static void Init()
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true);
            IConfigurationRoot root = builder.Build();

            string str = root["ProjectFile"];
            System.Diagnostics.Debug.WriteLine(str);
            //IConfigurationSection section = root.GetSection("TestValues");
            //string str = section["Key1"];
            //System.Diagnostics.Debug.WriteLine(str);
        }
        public static void Save()
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        //    var ret = config.AppSettings.Settings["hogehoge"].Value;
            config.AppSettings.Settings["TestValues:Key1"].Value = "3";
            config.Save();
        }

    }
}
