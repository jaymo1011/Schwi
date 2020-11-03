using Microsoft.Extensions.Logging;
using Discord;

namespace SchwiBot
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
            switch (logSeverity)
            {
                case LogSeverity.Verbose:
                    return LogLevel.Trace;
                case LogSeverity.Debug:
                    return LogLevel.Debug;
                case LogSeverity.Critical:
                    return LogLevel.Critical;
                case LogSeverity.Error:
                    return LogLevel.Error;
                case LogSeverity.Warning:
                    return LogLevel.Warning;
                case LogSeverity.Info:
                default:
                    return LogLevel.Information;
            }
        }
    }
}
