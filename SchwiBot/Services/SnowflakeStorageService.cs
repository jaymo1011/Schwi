using Discord;
using Microsoft.EntityFrameworkCore;
using SchwiBot.Abstractions;
using SchwiBot.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SchwiBot.Services
{
    public class SnowflakeStorageService : ISnowflakeStorage
    {
        private readonly SchwiDatabaseContext databaseContext;
        private readonly Dictionary<ulong, UserStorage> fetchedUsers = new Dictionary<ulong, UserStorage>();
        private readonly Dictionary<ulong, GuildStorage> fetchedGuilds = new Dictionary<ulong, GuildStorage>();

        public SnowflakeStorageService(SchwiDatabaseContext _databaseContext)
        {
            databaseContext = _databaseContext;

            // This is certainly not the best way to do it, but it works for now!
            // TODO: do this better :P
            databaseContext.Database.Migrate();
        }

        void ISnowflakeStorage.SaveChanges() => databaseContext.SaveChangesAsync(); // We don't want to block the thread that wants to save changes.

        IUserStorage ISnowflakeStorage.GetStorage(IUser user) => GetStorageForUser(user.Id);
        IGuildStorage ISnowflakeStorage.GetStorage(IGuild guild) => GetStorageForGuild(guild.Id);
        IGuildUserStorage ISnowflakeStorage.GetStorage(IGuildUser guildUser) => GetStorageForGuild(guildUser.GuildId).GetUserStorage(guildUser);

        public IUserStorage GetStorageForUser(ulong userSnowflake)
        {
            if (!fetchedUsers.ContainsKey(userSnowflake))
            {
                var dbStorageEntry = databaseContext.UserStorages.Find(userSnowflake) ?? databaseContext.UserStorages.Add(new DbUserStorage(userSnowflake)).Entity;

                fetchedUsers[userSnowflake] = new UserStorage(dbStorageEntry);
            }

            return fetchedUsers[userSnowflake];
        }

        public IGuildStorage GetStorageForGuild(ulong guildSnowflake)
        {
            if (!fetchedGuilds.ContainsKey(guildSnowflake))
            {
                var dbStorageEntry = databaseContext.GuildStorages.Find(guildSnowflake) ?? databaseContext.GuildStorages.Add(new DbGuildStorage(guildSnowflake)).Entity;

                fetchedGuilds[guildSnowflake] = new GuildStorage(dbStorageEntry);
            }

            return fetchedGuilds[guildSnowflake];
        }

        private class UserStorage : IUserStorage
        {
            private readonly Dictionary<string, string> userStorage;
            private readonly DbUserStorage dbEntry;

            public UserStorage(DbUserStorage _dbEntry)
            {
                dbEntry = _dbEntry;
                userStorage = dbEntry.Storage;
            }

            ulong IUserStorage.Snowflake => dbEntry.Snowflake;
            string IDictionary<string, string>.this[string key]
            {
                get
                {
                    // Avoid KeyNotFound exceptions
                    if (!userStorage.ContainsKey(key))
                        return null;

                    return userStorage[key];
                }

                set => userStorage[key] = value;
            }

            #region IDictionary Forwarder
            ICollection<string> IDictionary<string, string>.Keys => userStorage.Keys;
            ICollection<string> IDictionary<string, string>.Values => userStorage.Values;
            int ICollection<KeyValuePair<string, string>>.Count => userStorage.Count;
            bool ICollection<KeyValuePair<string, string>>.IsReadOnly => false;
            void IDictionary<string, string>.Add(string key, string value) => userStorage.Add(key, value);
            void ICollection<KeyValuePair<string, string>>.Add(KeyValuePair<string, string> item) => userStorage.Add(item.Key, item.Value);
            void ICollection<KeyValuePair<string, string>>.Clear() => userStorage.Clear();
            bool ICollection<KeyValuePair<string, string>>.Contains(KeyValuePair<string, string> item) => userStorage.Contains(item);
            bool IDictionary<string, string>.ContainsKey(string key) => userStorage.ContainsKey(key);
            void ICollection<KeyValuePair<string, string>>.CopyTo(KeyValuePair<string, string>[] array, int arrayIndex) => throw new NotImplementedException();
            IEnumerator<KeyValuePair<string, string>> IEnumerable<KeyValuePair<string, string>>.GetEnumerator() => userStorage.GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => userStorage.GetEnumerator();
            bool IDictionary<string, string>.Remove(string key) => userStorage.Remove(key);
            bool ICollection<KeyValuePair<string, string>>.Remove(KeyValuePair<string, string> item) => userStorage.Remove(item.Key);
            bool IDictionary<string, string>.TryGetValue(string key, out string value) => userStorage.TryGetValue(key, out value);
            #endregion
        }

        private class GuildStorage : IGuildStorage
        {
            private readonly Dictionary<string, string> guildStorage;
            private readonly Dictionary<ulong, GuildUserStorage> fetchedUserStorages;
            private readonly DbGuildStorage dbEntry;

            internal GuildStorage(DbGuildStorage _dbEntry)
            {
                dbEntry = _dbEntry;
                guildStorage = dbEntry.Storage;
                fetchedUserStorages = new Dictionary<ulong, GuildUserStorage>();
            }

            private IGuildUserStorage GetUserStorageInternal(ulong snowflake)
            {
                if (!fetchedUserStorages.ContainsKey(snowflake))
                {
                    if (!dbEntry.UserStorage.ContainsKey(snowflake))
                    {
                        dbEntry.UserStorage[snowflake] = new Dictionary<string, string>();
                    }

                    fetchedUserStorages[snowflake] = new GuildUserStorage(snowflake, dbEntry.Snowflake, dbEntry.UserStorage[snowflake]);
                }

                return fetchedUserStorages[snowflake];
            }

            ulong IGuildStorage.Snowflake => dbEntry.Snowflake;
            string IDictionary<string, string>.this[string key]
            {
                get
                {
                    if (!guildStorage.ContainsKey(key))
                        return null;

                    return guildStorage[key];
                }

                set => guildStorage[key] = value;
            }
            IGuildUserStorage IGuildStorage.GetUserStorage(ulong snowflake) => GetUserStorageInternal(snowflake);
            IGuildUserStorage IGuildStorage.GetUserStorage(IUser user) => GetUserStorageInternal(user.Id);
            IGuildUserStorage IGuildStorage.GetUserStorage(IUserStorage userStorage) => GetUserStorageInternal(userStorage.Snowflake);

            #region IDictionary Forwarder
            ICollection<string> IDictionary<string, string>.Keys => guildStorage.Keys;
            ICollection<string> IDictionary<string, string>.Values => guildStorage.Values;
            int ICollection<KeyValuePair<string, string>>.Count => guildStorage.Count;
            bool ICollection<KeyValuePair<string, string>>.IsReadOnly => false;
            void IDictionary<string, string>.Add(string key, string value) => guildStorage.Add(key, value);
            void ICollection<KeyValuePair<string, string>>.Add(KeyValuePair<string, string> item) => guildStorage.Add(item.Key, item.Value);
            void ICollection<KeyValuePair<string, string>>.Clear() => guildStorage.Clear();
            bool ICollection<KeyValuePair<string, string>>.Contains(KeyValuePair<string, string> item) => guildStorage.Contains(item);
            bool IDictionary<string, string>.ContainsKey(string key) => guildStorage.ContainsKey(key);
            void ICollection<KeyValuePair<string, string>>.CopyTo(KeyValuePair<string, string>[] array, int arrayIndex) => throw new NotImplementedException();
            IEnumerator<KeyValuePair<string, string>> IEnumerable<KeyValuePair<string, string>>.GetEnumerator() => guildStorage.GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => guildStorage.GetEnumerator();
            bool IDictionary<string, string>.Remove(string key) => guildStorage.Remove(key);
            bool ICollection<KeyValuePair<string, string>>.Remove(KeyValuePair<string, string> item) => guildStorage.Remove(item.Key);
            bool IDictionary<string, string>.TryGetValue(string key, out string value) => guildStorage.TryGetValue(key, out value);
            #endregion
        }

        private class GuildUserStorage : IGuildUserStorage
        {
            private readonly ulong userSnowflake;
            private readonly ulong guildSnowflake;
            private readonly Dictionary<string, string> guildUserStorage;

            internal GuildUserStorage(ulong _userSnowflake, ulong _guildSnowflake, Dictionary<string, string> _guildUserStorage)
            {
                userSnowflake = _userSnowflake;
                guildSnowflake = _guildSnowflake;
                guildUserStorage = _guildUserStorage;
            }

            ulong IGuildUserStorage.UserSnowflake => userSnowflake;
            ulong IGuildUserStorage.GuildSnowflake => guildSnowflake;
            string IDictionary<string, string>.this[string key]
            {
                get
                {
                    if (!guildUserStorage.ContainsKey(key))
                        return null;

                    return guildUserStorage[key];
                }

                set => guildUserStorage[key] = value;
            }

            #region IDictionary Forwarder
            ICollection<string> IDictionary<string, string>.Keys => guildUserStorage.Keys;
            ICollection<string> IDictionary<string, string>.Values => guildUserStorage.Values;
            int ICollection<KeyValuePair<string, string>>.Count => guildUserStorage.Count;
            bool ICollection<KeyValuePair<string, string>>.IsReadOnly => false;
            void IDictionary<string, string>.Add(string key, string value) => guildUserStorage.Add(key, value);
            void ICollection<KeyValuePair<string, string>>.Add(KeyValuePair<string, string> item) => guildUserStorage.Add(item.Key, item.Value);
            void ICollection<KeyValuePair<string, string>>.Clear() => guildUserStorage.Clear();
            bool ICollection<KeyValuePair<string, string>>.Contains(KeyValuePair<string, string> item) => guildUserStorage.Contains(item);
            bool IDictionary<string, string>.ContainsKey(string key) => guildUserStorage.ContainsKey(key);
            void ICollection<KeyValuePair<string, string>>.CopyTo(KeyValuePair<string, string>[] array, int arrayIndex) => throw new NotImplementedException();
            IEnumerator<KeyValuePair<string, string>> IEnumerable<KeyValuePair<string, string>>.GetEnumerator() => guildUserStorage.GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => guildUserStorage.GetEnumerator();
            bool IDictionary<string, string>.Remove(string key) => guildUserStorage.Remove(key);
            bool ICollection<KeyValuePair<string, string>>.Remove(KeyValuePair<string, string> item) => guildUserStorage.Remove(item.Key);
            bool IDictionary<string, string>.TryGetValue(string key, out string value) => guildUserStorage.TryGetValue(key, out value);
            #endregion
        }
    }
}
