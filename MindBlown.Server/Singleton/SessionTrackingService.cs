using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MindBlown.Server.Data;
using MindBlown.Server.Models;

namespace MindBlown.Server.Singleton
{
    public class SessionTrackingService
    {
        private readonly AppDbContext _context;

        public SessionTrackingService(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddSessionAsync(Guid sessionId, Guid userId)
        {
            var session = new User
            {
                SessionId = sessionId,
                UserId = userId,
                LastActive = DateTime.UtcNow,
                IsActive = true
            };

            _context.ActiveUserSession.Add(session);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateSessionActivityAsync(Guid sessionId)
        {
            var session = await _context.ActiveUserSession
                .FirstOrDefaultAsync(s => s.SessionId == sessionId);
            
            if (session != null)
            {
                session.LastActive = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task RemoveSessionAsync(Guid sessionId)
        {
            var session = await _context.ActiveUserSession
                .FirstOrDefaultAsync(s => s.SessionId == sessionId);

            if (session != null)
            {
                _context.ActiveUserSession.Remove(session);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<int> GetActiveUserCountAsync()
        {
            return await _context.ActiveUserSession
                .CountAsync(s => s.IsActive);
        }

        public async Task<List<string>> GetActiveUserIdsAsync()
        {
            return await _context.ActiveUserSession
                .Where(s => s.IsActive)
                .Select(s => s.SessionId.ToString())
                .ToListAsync();
        }
        public async Task<List<User>> GetActiveUsersAsync()
{
    // Select only the fields you need from ActiveUserSession
    var activeSessions = await _context.ActiveUserSession
        .Where(s => s.IsActive)
        .Select(s => new 
        {
            userId = s.UserId,
            sessionId = s.SessionId,
            lastActive = s.LastActive,
            isActive = s.IsActive
        })
        .ToListAsync();

    // Create a new User object for each record and add to a list
    var users = new List<User>();

    foreach (var session in activeSessions)
    {
        var user = new User
        {
            UserId = session.userId,  // Assuming the User object has a UserId property
            SessionId = session.sessionId,  // Assuming you want to store sessionId in User
            LastActive = session.lastActive,
            IsActive = session.isActive
        };
        users.Add(user);  
    }

    return users;
}
    }
}
