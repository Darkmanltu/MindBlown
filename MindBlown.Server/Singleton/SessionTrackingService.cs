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
                sessionId = sessionId,
                userId = userId,
                lastActive = DateTime.UtcNow,
                isActive = true
            };

            _context.ActiveUserSession.Add(session);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateSessionActivityAsync(Guid sessionId)
        {
            var session = await _context.ActiveUserSession
                .FirstOrDefaultAsync(s => s.sessionId == sessionId);
            
            if (session != null)
            {
                session.lastActive = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task RemoveSessionAsync(Guid sessionId)
        {
            var session = await _context.ActiveUserSession
                .FirstOrDefaultAsync(s => s.sessionId == sessionId);

            if (session != null)
            {
                _context.ActiveUserSession.Remove(session);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<int> GetActiveUserCountAsync()
        {
            return await _context.ActiveUserSession
                .CountAsync(s => s.isActive);
        }

        public async Task<List<string>> GetActiveUserIdsAsync()
        {
            return await _context.ActiveUserSession
                .Where(s => s.isActive)
                .Select(s => s.sessionId.ToString())
                .ToListAsync();
        }
        public async Task<List<User>> GetActiveUsersAsync()
{
    // Select only the fields you need from ActiveUserSession
    var activeSessions = await _context.ActiveUserSession
        .Where(s => s.isActive)
        .Select(s => new 
        {
            s.userId,
            s.sessionId,
            s.lastActive,
            s.isActive
        })
        .ToListAsync();

    // Create a new User object for each record and add to a list
    var users = new List<User>();

    foreach (var session in activeSessions)
    {
        var user = new User
        {
            userId = session.userId,  // Assuming the User object has a UserId property
            sessionId = session.sessionId,  // Assuming you want to store sessionId in User
            lastActive = session.lastActive,
            isActive = session.isActive
        };
        System.Console.WriteLine("User ID: " + user.userId);
        users.Add(user);  
    }

    return users;
}
    }
}
