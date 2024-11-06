using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MindBlown.Server.Controllers
{
    [ApiController]
    [Route("api/logs")]
    public class LoggingController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Log([FromBody] LogEntry logEntry)
        {
            try
            {
                string logFilePath = "logs/log.txt";
                Directory.CreateDirectory(Path.GetDirectoryName(logFilePath)!);

                var logMessage = $"{logEntry.Timestamp} [{logEntry.LogLevel}]: {logEntry.Message} - {logEntry.Details}{Environment.NewLine}";
                await System.IO.File.AppendAllTextAsync(logFilePath, logMessage);

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }

    public class LogEntry
    {
        public string Message { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string LogLevel { get; set; } = "Error"; // Default log level
    }
}
