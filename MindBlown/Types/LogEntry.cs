namespace MindBlown.Types
{
    public class LogEntry
    {
        public string Message { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string LogLevel { get; set; } = "Error"; // Default log level
    }
}


