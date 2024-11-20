using System.Net.Http.Json;
using MindBlown.Types;

namespace Services
{
    public class LWARecordService
    {
        public readonly HttpClient _httpClient;

        public LWARecordService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<LastWrongAnswerRecord?> GetRecordAsync()
        {
            try
            {
                var result = await _httpClient.GetFromJsonAsync<LastWrongAnswerRecord?>("api/lwarecord");
                return result;
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                // Do nothing if the error is a "Not Found" (404) situation
                return null;
            }
            catch (Exception ex)
            {
                await LogErrorToServerAsync($"Error updating record", ex.Message);
                throw;
            }
        }

        public async Task<LastWrongAnswerRecord?> UpdateRecordAsync(LastWrongAnswerRecord record)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"api/lwarecord", record);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<LastWrongAnswerRecord>();
            }
            // catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            // {
            //     // Do nothing if the error is a "Not Found" (404) situation
            //     return null;
            // }
            catch (HttpRequestException ex)
            {
                // await LogErrorToServerAsync($"Error updating record with ID: {id}", ex.Message);
                await LogErrorToServerAsync($"Error updating record", ex.Message);
                throw;
            }
        }

        // Log error to server-side logging API
        public async Task LogErrorToServerAsync(string message, string details)
        {
            var logEntry = new LogEntry
            {
                Message = message,
                Details = details,
                Timestamp = DateTime.UtcNow
            };

            try
            {
                await _httpClient.PostAsJsonAsync("api/logs", logEntry);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to log to server: {ex.Message}");
                // Fallback logging (e.g., browser console)
            }
        }
    }
}