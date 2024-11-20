using Microsoft.AspNetCore.Mvc;
using MindBlown.Server.Singleton;
using MindBlown.Server.Models;
using System.Collections.Concurrent;

[ApiController]
[Route("api/activeUser")]
public class ActiveUserController : ControllerBase
{
    private readonly ConcurrentDictionary<Guid, User> _activeUsers = new ConcurrentDictionary<Guid, User>();

    private readonly SessionTrackingService _sessionTrackingService;

    public ActiveUserController(SessionTrackingService sessionTrackingService)
    {
        _sessionTrackingService = sessionTrackingService;
    }
    [HttpGet("getdict")]
    public async Task<ConcurrentDictionary<Guid, User>> GetDictionary()
    {  
         await UpdateDictionary();
        return  _activeUsers;
    }

    public async  Task<IActionResult> UpdateDictionary()
    {  
        var users = await _sessionTrackingService.GetActiveUsersAsync();
        _activeUsers.Clear();       //clear for posibility that users are removed from db in other thread
        foreach(var uz in users){
            _activeUsers[uz.SessionId]= uz;
        }

        return Ok();
    }
    [HttpPost("add")]
    public async Task<IActionResult> AddUser([FromBody] Guid userId, Guid sessionId)
    {
        //System.Console.WriteLine("Adding user:");
        await _sessionTrackingService.AddSessionAsync(userId, sessionId);
        return Ok();
    }

    [HttpPost("remove")]
    public async Task<IActionResult> RemoveUser([FromBody] Guid userId)
    {
       await _sessionTrackingService.RemoveSessionAsync(userId);
        return Ok();
    }

    
    
    [HttpGet("usersId")]
public async Task<IActionResult> GetActiveUsers()
{
    try
    {
        var users = await _sessionTrackingService.GetActiveUserIdsAsync();
        return Ok(users);
    }
    catch (Exception ex)
    {
        // Log the error (make sure logging is configured in your app)
        Console.WriteLine($"Error in GetActiveUsers: {ex.Message}");
        return StatusCode(500, "An error occurred while fetching active users.");
    }
}
 [HttpGet("users")]
public async Task<IActionResult> GetActiveUsersFull()
{
    try
    {
        var users = await _sessionTrackingService.GetActiveUsersAsync();
        return Ok(users);
    }
    catch (Exception ex)
    {
        // Log the error (make sure logging is configured in your app)
        Console.WriteLine($"Error in GetActiveUsers: {ex.Message}");
        return StatusCode(500, "An error occurred while fetching active users.");
    }
}
[HttpPost("removeInnactive")]
 public async Task<IActionResult> RemoveInactiveUsersAsync()
    {
        var activeUsers = await _sessionTrackingService.GetActiveUserIdsAsync();
        var users = await _sessionTrackingService.GetActiveUsersAsync();

        foreach (var u in users){
            Console.WriteLine($"Removing user with ID: {u.SessionId}");
            if (u.LastActive < DateTime.UtcNow.AddMinutes(-5)){
                await RemoveUser(u.SessionId);
                // might help with thread race condition
                await Task.Delay(100);
            }
        }

        //await Task.Wait(500);
    

    return Ok();
    }
}

