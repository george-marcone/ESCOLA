using Microsoft.Extensions.Logging;

namespace form_API.Logging
{
    public sealed class FileLoggerOptions
    {
        public string DirectoryPath { get; set; } = "logs";
        public string FileNamePrefix { get; set; } = "backend-api";
        public LogLevel MinimumLevel { get; set; } = LogLevel.Information;
    }
}
