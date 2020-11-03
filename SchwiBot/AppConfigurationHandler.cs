using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

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
            // Set the base path to the configuration value stored at SchwiBasePath and get the (potentially) new file provider for it.
            builder.SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), host.Configuration.GetValue<string>("SchwiBasePath") ?? ""));
            var configDir = builder.GetFileProvider();

            var configFile = configDir.GetFileInfo(SchwiConfigurationFileName);
            if (!configFile.Exists)
            {
                CopySampleConfigFileTo(configFile.PhysicalPath);
                Console.WriteLine($"New configuration file created at {configFile.PhysicalPath}");
            }

            builder.AddJsonFile(configFile.PhysicalPath);
        };
    }
}
