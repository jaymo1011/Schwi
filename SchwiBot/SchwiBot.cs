﻿using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SchwiBot
{
    public class SchwiBot : ISchwiBot, IHostedService
    {
        private ServiceProvider services;
        private DiscordSocketClient discordClient;
        private readonly ILogger _logger;
        private IConfiguration Configuration;

        public SchwiBot(ILogger<SchwiBot> logger, IHostApplicationLifetime appLifetime, IConfiguration configuration)
        {
            _logger = logger;
            services = BuildServices();
            discordClient = services.GetRequiredService<DiscordSocketClient>();
            discordClient.Log += ProcessDiscordNetLog;

            Configuration = configuration; // development stuff, pending a proper implementation
        }

        internal ServiceProvider BuildServices()
            => new ServiceCollection()
                .AddSingleton(new DiscordSocketClient(new DiscordSocketConfig
                {
                    //AlwaysDownloadUsers = true,
                    //MessageCacheSize = 50,
                    ExclusiveBulkDelete = false
                }))
                .BuildServiceProvider();

        internal Task ProcessDiscordNetLog(Discord.LogMessage logMessage)
        {
            if (logMessage.Exception != null)
            {
                // Flag exceptions with a little more verbosity than regular logs
                _logger.Log(LogLevel.Trace, "EXCEPTION!\nSource: {0}\nMessage:{1}\n{2}", logMessage.Source, logMessage.Message, logMessage.Exception.ToString());
            }
            else
            {
                _logger.Log(logMessage.Severity.GetLogLevel(), "{0}: {1}", logMessage.Source, logMessage.Message);
            }

            // This function does not have any code that should be ran asynchronously
            return Task.CompletedTask;
        }

        // ISchwiBot Implementations
        IServiceProvider ISchwiBot.Services => services;
        IDiscordClient ISchwiBot.DiscordClient => discordClient;

        // IHostedService Implementations
        async Task IHostedService.StartAsync(CancellationToken cancellationToken)
        {
            await discordClient.LoginAsync(TokenType.Bot, Configuration["DiscordBotToken"], true);

            if (!cancellationToken.IsCancellationRequested)
                await discordClient.StartAsync();
        }

        Task IHostedService.StopAsync(CancellationToken cancellationToken)
        {
            return discordClient.StopAsync();
        }
    }
}
