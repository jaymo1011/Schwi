using Discord;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace SchwiBot
{
    public interface ISchwiBot
    {
        public IServiceProvider Services { get; }
        
        public IDiscordClient DiscordClient { get; }
    }
}