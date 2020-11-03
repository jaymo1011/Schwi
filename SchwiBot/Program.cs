using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Reflection;

namespace SchwiBot
{
    class Program
    {
        // Very basic application host for enabling standalone operation of SchwiBot from the command line via `dotnet run`
        // Source: https://docs.microsoft.com/en-us/dotnet/core/extensions/generic-host
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseEnvironment("Development")
                .ConfigureAppConfiguration(AppConfigurationHandler.Delegate)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<SchwiBot>();
                });
    }
}
