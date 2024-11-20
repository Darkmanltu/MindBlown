using System.Collections.Concurrent;
using MindBlown.Types;

public interface IActiveUserClient
{
    Task<bool> IsSessionIdUniqueAsync(Guid sessionId);
    Task<List<Guid>> GetActiveUsersAsync();
    Task AddUserAsync(Guid userId);
    Task RemoveUserAsync(Guid userId);
    Task<int> GetActiveUserCountAsync();
    Task<int> GetActiveUserCountAsync(ConcurrentDictionary<Guid, User> activeUsers);
    Task RemoveInnactive();
    Task<ConcurrentDictionary<Guid, User>> GetDictionary();
}