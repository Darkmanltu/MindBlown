using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using MindBlown.Interfaces;
using MindBlown.Exceptions;
using MindBlown.Types;

namespace Services
{
    public class MnemonicService : IMnemonicService
    {
        public readonly HttpClient _httpClient;

        public MnemonicService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Fetch all mnemonics
        public async Task<List<MnemonicsType>?> GetMnemonicsAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<MnemonicsType>>("api/mnemonics");
            }
            catch (HttpRequestException ex)
            {
                await LogErrorToServerAsync("Error fetching mnemonics", ex.Message);
                throw new MnemonicServiceException("An error occurred while fetching mnemonics.", ex);
            }
        }

        // Fetch a specific mnemonic by ID
        public async Task<MnemonicsType?> GetMnemonicAsync(Guid id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<MnemonicsType>($"api/mnemonics/{id}");
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
            catch (HttpRequestException ex)
            {
                await LogErrorToServerAsync($"Error fetching mnemonic with ID: {id}", ex.Message);
                throw new MnemonicServiceException($"An error occurred while fetching the mnemonic with ID {id}.", ex);
            }
        }

        // Test to catch an exception
        // public async Task<MnemonicsType?> GetMnemonicAsync(Guid id)
        // {
        //     try
        //     {
        //         // Use an invalid endpoint to trigger an error
        //         return await _httpClient.GetFromJsonAsync<MnemonicsType>($"api/invalid/{id}");
        //     }
        //     catch (HttpRequestException ex)
        //     {
        //         await LogErrorToServerAsync($"Error fetching mnemonic with ID: {id}", ex.Message);
        //         throw new MnemonicServiceException($"An error occurred while fetching the mnemonic with ID {id}.", ex);
        //     }
        // }
        // Create a new mnemonic
        public async Task<MnemonicsType?> CreateMnemonicAsync(MnemonicsType mnemonic)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/mnemonics", mnemonic);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<MnemonicsType>();
            }
            catch (HttpRequestException ex)
            {
                await LogErrorToServerAsync("Error creating mnemonic", ex.Message);
                throw new MnemonicServiceException("An error occurred while creating the mnemonic.", ex);
            }
        }

        // Update an existing mnemonic
        public async Task<MnemonicsType?> UpdateMnemonicAsync(MnemonicsType mnemonic)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/mnemonics/{mnemonic.Id}", mnemonic);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<MnemonicsType>();
            }
            catch (HttpRequestException ex)
            {
                await LogErrorToServerAsync($"Error updating mnemonic with ID: {mnemonic.Id}", ex.Message);
                throw new MnemonicServiceException(
                    $"An error occurred while updating the mnemonic with ID {mnemonic.Id}.", ex);
            }
        }

        // Delete a mnemonic by ID
        public async Task DeleteMnemonicAsync(Guid id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/mnemonics/{id}");
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                await LogErrorToServerAsync($"Error deleting mnemonic with ID: {id}", ex.Message);
                throw new MnemonicServiceException($"An error occurred while deleting the mnemonic with ID {id}.", ex);
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
