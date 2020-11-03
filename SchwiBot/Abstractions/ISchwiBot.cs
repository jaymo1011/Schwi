using Discord;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace SchwiBot.Abstractions
{
    public interface ISchwiBot
    {
        public IServiceProvider Services { get; }
        
        public IDiscordClient DiscordClient { get; }
    }
}