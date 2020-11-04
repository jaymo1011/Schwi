using System.Collections.Generic;

namespace SchwiBot.Abstractions
{
    public interface IGuildUserStorage : IDictionary<string, string>
    {
        public ulong UserSnowflake { get; }
        public ulong GuildSnowflake { get; }
    }
}
