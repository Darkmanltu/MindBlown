using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using MindBlown.Types;
using System.Collections.Concurrent;
using MindBlown.Interfaces;

public class ActiveUserClient : IActiveUserClient
{
    private readonly HttpClient _httpClient;

    public ActiveUserClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<bool> IsSessionIdUniqueAsync(Guid sessionId)
    {
        var activeUsers = await GetActiveUsersAsync();
        //System.Console.WriteLine("Session ID: " + sessionId);
        //System.Console.WriteLine("Active Users: " );
        foreach (var userSessionId in activeUsers)
    {
        Console.WriteLine(userSessionId);
    }
        return !activeUsers.Contains(sessionId);
        
    }
   
    public async Task<List<Guid>> GetActiveUsersAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<Guid>>("api/activeUser/usersId") ?? new List<Guid>();
    }

    public async Task AddUserAsync(Guid userId)
    {
        await _httpClient.PostAsJsonAsync("api/activeUser/add", userId);
    }

    public async Task RemoveUserAsync(Guid userId)
    {
        await _httpClient.PostAsJsonAsync("api/activeUser/remove", userId);
    }

    public async Task<int> GetActiveUserCountAsync()
    {
        return await _httpClient.GetFromJsonAsync<int>("api/activeUser/count");
    }
    public Task<int> GetActiveUserCountAsync(ConcurrentDictionary<Guid, User> activeUsers)
    {
        int count = 0;
        foreach (var user in activeUsers)
        {
            if (user.Value.isActive)
            {
                count++;
            }
        }
        return Task.FromResult(count);
    }
    public async Task RemoveInnactive()
    {
        await _httpClient.PostAsJsonAsync("api/activeUser/removeInnactive", 1);
    }
    public async Task<ConcurrentDictionary<Guid, User>> GetDictionary()
    {
        return await _httpClient.GetFromJsonAsync<ConcurrentDictionary<Guid, User>>("api/activeUser/getdict") ?? new ConcurrentDictionary<Guid, User>();
    }
}

