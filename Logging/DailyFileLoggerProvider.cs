using System.Globalization;
using System.Text;
using Microsoft.Extensions.Logging;

namespace form_API.Logging
{
    public sealed class DailyFileLoggerProvider : ILoggerProvider, ISupportExternalScope
    {
        private readonly object _lock = new();
        private readonly FileLoggerOptions _options;
        private IExternalScopeProvider _scopeProvider = new LoggerExternalScopeProvider();

        public DailyFileLoggerProvider(FileLoggerOptions options)
        {
            _options = options;
            Directory.CreateDirectory(_options.DirectoryPath);
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new DailyFileLogger(categoryName, this);
        }

        public void Dispose()
        {
        }

        public void SetScopeProvider(IExternalScopeProvider scopeProvider)
        {
            _scopeProvider = scopeProvider;
        }

        internal bool IsEnabled(LogLevel logLevel)
        {
            return logLevel != LogLevel.None && logLevel >= _options.MinimumLevel;
        }

        internal IDisposable? BeginScope<TState>(TState state)
            where TState : notnull
        {
            return _scopeProvider.Push(state);
        }

        internal void Write(
            string categoryName,
            LogLevel logLevel,
            EventId eventId,
            string message,
            Exception? exception)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            var filePath = Path.Combine(
                _options.DirectoryPath,
                $"{_options.FileNamePrefix}-{DateTimeOffset.Now:yyyyMMdd}.log");

            var log = new StringBuilder()
                .Append(DateTimeOffset.Now.ToString("yyyy-MM-dd HH:mm:ss.fff zzz", CultureInfo.InvariantCulture))
                .Append(" [")
                .Append(logLevel)
                .Append("] ")
                .Append(categoryName);

            if (eventId.Id != 0 || !string.IsNullOrWhiteSpace(eventId.Name))
            {
                log.Append(" (")
                    .Append(eventId.Id);

                if (!string.IsNullOrWhiteSpace(eventId.Name))
                {
                    log.Append(':')
                        .Append(eventId.Name);
                }

                log.Append(')');
            }

            var scopes = GetScopes();
            if (scopes.Count > 0)
            {
                log.Append(" => ")
                    .Append(string.Join(" => ", scopes));
            }

            log.Append(": ")
                .AppendLine(message);

            if (exception != null)
            {
                log.AppendLine(exception.ToString());
            }

            lock (_lock)
            {
                File.AppendAllText(filePath, log.ToString());
            }
        }

        private List<string> GetScopes()
        {
            var scopes = new List<string>();
            _scopeProvider.ForEachScope((scope, values) =>
            {
                if (scope != null)
                {
                    values.Add(scope.ToString() ?? string.Empty);
                }
            }, scopes);

            return scopes;
        }
    }
}
