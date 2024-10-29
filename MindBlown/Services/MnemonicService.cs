using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using MindBlown.Types;
using System.Collections.Concurrent;

public class MnemonicService
{
    private readonly HttpClient _httpClient;

    public MnemonicService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<MnemonicsType>?> GetMnemonicsAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<MnemonicsType>>("api/mnemonics");
    }

    public async Task<MnemonicsType?> GetMnemonicAsync(Guid id)
    {
        // return await _httpClient.GetFromJsonAsync<MnemonicsType>($"api/mnemonics/{id}");

        try
        {
            return await _httpClient.GetFromJsonAsync<MnemonicsType>($"api/mnemonics/{id}");
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }

    }

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
            // Handle error (logging, rethrowing, etc.)
            Console.WriteLine($"Error creating mnemonic: {ex.Message}");
            return null;
        }
    }

    // Create a new list of mnemonics
    public async Task CreateMnemonicsAsync(List<MnemonicsType> mnemonics)
    {
        var response = await _httpClient.PostAsJsonAsync("api/mnemonics/bulk", mnemonics);
        response.EnsureSuccessStatusCode();
    }


    // Update an existing mnemonic
    public async Task<MnemonicsType?> UpdateMnemonicAsync(MnemonicsType mnemonic)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/mnemonics/{mnemonic.Id}", mnemonic);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<MnemonicsType>();
    }

    // Delete a mnemonic by ID
    public async Task DeleteMnemonicAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"api/mnemonics/{id}");
        response.EnsureSuccessStatusCode();
    }

}
