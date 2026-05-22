using Microsoft.Extensions.Logging;

namespace ESCOLA_API.Logging
{
    public static class FileLoggerExtensions
    {
        public static ILoggingBuilder AddDailyFileLogger(
            this ILoggingBuilder builder,
            Action<FileLoggerOptions> configure)
        {
            var options = new FileLoggerOptions();
            configure(options);

            builder.AddProvider(new DailyFileLoggerProvider(options));
            return builder;
        }
    }
}
