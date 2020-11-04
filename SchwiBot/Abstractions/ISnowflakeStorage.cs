using Discord;

namespace SchwiBot.Abstractions
{
    public interface ISnowflakeStorage
    {
        public IUserStorage GetStorage(IUser user);
        public IGuildStorage GetStorage(IGuild guild);
        public IGuildUserStorage GetStorage(IGuildUser guildUser);
        public IGuildStorage GetStorageForGuild(ulong guildSnowflake);
        public IUserStorage GetStorageForUser(ulong userSnowflake);
        public void SaveChanges();
    }
}
