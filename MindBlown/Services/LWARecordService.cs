using System.Net.Http.Json;
using MindBlown.Services;
using MindBlown.Types;

namespace Services
{
    public class LWARecordService : ILWARecordService
    {
        public readonly HttpClient _httpClient;

        public LWARecordService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<LastWrongAnswerRecord?> GetRecordAsync(Guid id)
        {
            try
            {
                var resp = await _httpClient.GetAsync($"api/lwarecord?id={id}");
                if (resp.IsSuccessStatusCode)
                {
                    var result = await resp.Content.ReadFromJsonAsync<LastWrongAnswerRecord>();
                    return result;
                }
                else return null;
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

        public async Task<LastWrongAnswerRecord?> UpdateRecordAsync(Guid idToChange, LastWrongAnswerRecord record)
        {
            try
            {
                var request = new IdLWARecordRequest()
                {
                    IdToChange = idToChange,
                    RecordToSet = record
                };
                var response = await _httpClient.PostAsJsonAsync($"api/lwarecord", request);
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
                var response = await _httpClient.PostAsJsonAsync("api/logs", logEntry);
                
                response.EnsureSuccessStatusCode(); // Throws exception if not 2xx
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to log to server: {ex.Message}");
                
                // Fallback logging (e.g., browser console)
            }
            
            
        }
        
    }
    
    public class IdLWARecordRequest
    {
        public required Guid IdToChange { get; set; }
        public required LastWrongAnswerRecord RecordToSet { get; set; }
    }
}
