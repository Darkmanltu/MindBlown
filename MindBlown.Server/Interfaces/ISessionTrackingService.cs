using MindBlown.Server.Models;

namespace MindBlown.Server.Interfaces;

public interface ISessionTrackingService
{
    Task AddSessionAsync(Guid userId, Guid sessionId);
    Task RemoveSessionAsync(Guid userId);
    Task<List<User>> GetActiveUsersAsync();
    Task<List<String>> GetActiveUserIdsAsync();
}