using Microsoft.Extensions.Logging;

namespace ESCOLA_API.Logging
{
    public sealed class DailyFileLogger : ILogger
    {
        private readonly string _categoryName;
        private readonly DailyFileLoggerProvider _provider;

        public DailyFileLogger(string categoryName, DailyFileLoggerProvider provider)
        {
            _categoryName = categoryName;
            _provider = provider;
        }

        public IDisposable? BeginScope<TState>(TState state)
            where TState : notnull
        {
            return _provider.BeginScope(state);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return _provider.IsEnabled(logLevel);
        }

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            var message = formatter(state, exception);
            if (string.IsNullOrWhiteSpace(message) && exception == null)
            {
                return;
            }

            _provider.Write(_categoryName, logLevel, eventId, message, exception);
        }
    }
}
