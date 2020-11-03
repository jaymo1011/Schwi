using Microsoft.Extensions.Logging;
using Discord;

namespace SchwiBot.Extensions
{
    public static class DiscordNetLogSeverityExtensions
    {
        /*
         * Discord.NET has a non-standard "log severity" system attached to it's log messages.
         * It would be ideal that the "severity" of these messages be translated to log levels which ILogger can understand.
         * Such is the purpose of this extension class.
         */

        public static LogLevel GetLogLevel(this LogSeverity logSeverity)
        {
            return logSeverity switch
            {
                LogSeverity.Verbose => LogLevel.Trace,
                LogSeverity.Debug => LogLevel.Debug,
                LogSeverity.Critical => LogLevel.Critical,
                LogSeverity.Error => LogLevel.Error,
                LogSeverity.Warning => LogLevel.Warning,
                _ => LogLevel.Information,
            };
        }
    }
}
