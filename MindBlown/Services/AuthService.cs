using System.Net.Http.Json;
using Microsoft.JSInterop;
using MindBlown.Types;
using System.IdentityModel.Tokens.Jwt; // For JWT handling
using System.Security.Claims;  // For Claims and ClaimTypes
using System;
using System.Text;
using System.Text.Json;
using MindBlown.Interfaces;

namespace Services;
public class AuthService : IAuthService
{
    private readonly IJSRuntime _jsRuntime;
    private readonly HttpClient _httpClient;

    public AuthService(IJSRuntime jsRuntime, HttpClient httpClient)
    {
        _jsRuntime = jsRuntime;
        _httpClient = httpClient;
    }

    // Check if the user is logged in by checking the presence of a token in localStorage
    public async Task<bool> IsUserLoggedInAsync()
    {
        var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");
        return !string.IsNullOrEmpty(token);
    }

    // Log out the user by removing the token from localStorage
    public async Task LogoutAsync()
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "authToken");
    }

    // Perform login and return a token or an error message
    public async Task<string?> LoginAsync(AccRequest loginRequest)
    {
        var lastUsername = loginRequest.Username;
        var response = await _httpClient.PostAsJsonAsync("api/userinfo/login", loginRequest);

        if (response.IsSuccessStatusCode)
        {
            var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();
            if (tokenResponse != null)
            {
                // Store the token in localStorage for persistence
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "authToken", tokenResponse.Token);
                return lastUsername;
            }
        }
        else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
        {
            var error = await response.Content.ReadAsStringAsync();
           
            return error;
        }
        
        return null;
    }

    // Perform signup and return a token or an error message
    public async Task<string?> SignupAsync(AccRequest signupRequest)
    {
        var response = await _httpClient.PostAsJsonAsync("api/userinfo/signup", signupRequest);

        if (response.IsSuccessStatusCode)
        {
            return "Success";
 
        }
        else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
        {
            var error = await response.Content.ReadAsStringAsync();
            
            return error;
        }

        return null; 
    }

    public async Task<MnemonicsType?> UpdateUserWithMnemonic(string? username, MnemonicsType newMnemonic, bool toAdd)
    {
        if (username != null)
        {
            var request = new MnemonicUpdateRequest()
            {
                Username = username,
                MnemonicToAdd = newMnemonic,
                ToAdd = toAdd
            };
            var response = await _httpClient.PutAsJsonAsync("api/userinfo", request);
            if (response.IsSuccessStatusCode)
            {
                return newMnemonic;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Error: " + error);
            }
        }
        
        return null;
    }

    public async Task<List<Guid>> GetMnemonicsGuids(string? username)
    {
        if (username != null)
        {
            return await _httpClient.GetFromJsonAsync<List<Guid>>($"api/userinfo/guids?username={username}") ?? new List<Guid>();
        }
        else
        {
            return new List<Guid>();
        }
    }

    public async Task<Guid> GetLWARecordId(string? username)
    {
        if (username != null)
        {
            return await _httpClient.GetFromJsonAsync<Guid>($"api/userinfo/userlwarecord?username={username}");
        }
        else
        {
            return new Guid();
        }
    }

    public async Task<Guid?> UpdateLWARecord(string? username, Guid newId)
    {
        if (username != null)
        {
            var request = new LWARecordUpdateRequest()
            {
                Username = username,
                NewId = newId
            };
            var response = await _httpClient.PutAsJsonAsync("api/userinfo/lwarecord_update", request);
            if (response.IsSuccessStatusCode)
            {
                return newId;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Error: " + error);
            }
        }
        
        return null;
    }

    

    public async Task<string?> GetUsername()
    {
        var jwtToken = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");
        
        if (string.IsNullOrEmpty(jwtToken))
        {
            return null;
        }
        
        // Create a JWT handler to decode the token
        var jwtHandler = new JwtSecurityTokenHandler();

        try
        {
            // Parse the JWT
            var token = jwtHandler.ReadJwtToken(jwtToken);

            // Retrieve the claim of type 'name' (this is the one you included when generating the token)
            var usernameClaim = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);

            return usernameClaim?.Value;
        }
        catch (Exception ex)
        {
            // Handle exceptions (e.g., invalid token format)
            Console.WriteLine("Error parsing JWT: " + ex.Message);
            return null;
        }
    }

}

public class MnemonicUpdateRequest
{
    public required string Username { get; set; }
    public required MnemonicsType MnemonicToAdd { get; set; }
    
    public required bool ToAdd { get; set; }
}

public class LWARecordUpdateRequest
{
    public required string Username { get; set; }
    public required Guid NewId { get; set; }
}


public class TokenResponse
{
    public required string Token { get; set; }
}