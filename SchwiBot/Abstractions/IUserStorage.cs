using System.Collections.Generic;

namespace SchwiBot.Abstractions
{
    public interface IUserStorage : IDictionary<string, string>
    {
        public ulong Snowflake { get; }
    }
}
