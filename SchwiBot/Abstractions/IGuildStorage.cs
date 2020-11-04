using Discord;
using System.Collections.Generic;

namespace SchwiBot.Abstractions
{
    public interface IGuildStorage : IDictionary<string, string>
    {
        public ulong Snowflake { get; }

        public IGuildUserStorage GetUserStorage(ulong snowflake);

        public IGuildUserStorage GetUserStorage(IUser user);

        public IGuildUserStorage GetUserStorage(IUserStorage userStorage);
    }
}
