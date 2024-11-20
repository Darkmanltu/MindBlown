using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using MindBlown.Interfaces;
using MindBlown.Types;

namespace Services
{
    public class LoggingService : ILoggingService
    {
        private readonly HttpClient _httpClient;

        public LoggingService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task LogAsync(LogEntry logEntry)
        {
            await _httpClient.PostAsJsonAsync("api/logs", logEntry);
        }
    }
}