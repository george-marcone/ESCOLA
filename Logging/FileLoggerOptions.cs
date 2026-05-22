using Microsoft.Extensions.Logging;

namespace ESCOLA_API.Logging
{
    public sealed class FileLoggerOptions
    {
        public string DirectoryPath { get; set; } = "logs";
        public string FileNamePrefix { get; set; } = "escola-api";
        public LogLevel MinimumLevel { get; set; } = LogLevel.Information;
    }
}
