using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;

namespace SchwiBot
{
    public static class AppConfigurationHandler
    {
        public const string SchwiConfigurationFileName = "SchwiConfig.json";
        public const string SchwiConfigurationSampleFileName = "SchwiConfig.sample.json";

        internal static void CopySampleConfigFileTo(string path)
        {
            // If the path is null (most likely the result of a broken base path) then do nothing.
            if (path == null)
                return;

            var assemblyDir = AppContext.BaseDirectory;
            var sampleConfigPath = Path.Combine(assemblyDir, SchwiConfigurationSampleFileName);

            if (File.Exists(sampleConfigPath))
            {
                // when we have a valid sample config, create a new configuration file at the specified path
                File.Copy(sampleConfigPath, path);
            }
            else
            {
                // otherwise, throw an exception as the user has most likely broken something
                throw new FileNotFoundException("Could not initialise configuration as the sample configuration was not found!", sampleConfigPath);
            }
        }

        public static Action<HostBuilderContext, IConfigurationBuilder> Delegate = (HostBuilderContext host, IConfigurationBuilder builder) =>
        {
            // Get the config from the specified location or use the current working directory to find the configuration file.
            var configFilePath = host.Configuration.GetValue<string>("SchwiConfigPath") ?? Path.Combine(Directory.GetCurrentDirectory(), SchwiConfigurationFileName);

            // If the specified path was relative then it should be relative to the current directory.
            configFilePath = Path.Combine(Directory.GetCurrentDirectory(), configFilePath);

            // If the specified path is missing a file name (and extension), use the default config name.
            if (!Path.HasExtension(configFilePath))
                configFilePath = Path.Combine(configFilePath, SchwiConfigurationFileName);
            
            // If the config file does not exist, create it by copying the sample config to it.
            if (!File.Exists(configFilePath))
            {
                CopySampleConfigFileTo(configFilePath);
                Console.WriteLine($"New configuration file created at {configFilePath}");
            }

            // Now that we know that it should exist, load the config from the file.
            builder.AddJsonFile(configFilePath);
        };
    }
}
