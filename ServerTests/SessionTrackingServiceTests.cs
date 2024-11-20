using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Xunit;
using MindBlown.Server.Singleton;
using MindBlown.Server.Data;
using MindBlown.Server.Models;

namespace MindBlown.Server.Tests
{
    public class SessionTrackingServiceTests
    {
        private AppDbContext CreateInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }

        [Fact]
        public async Task AddSessionAsync_ShouldAddSessionToDatabase()
        {
            using var context = CreateInMemoryDbContext();
            var service = new SessionTrackingService(context);

            var sessionId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            await service.AddSessionAsync(sessionId, userId);

            var session = context.ActiveUserSession.FirstOrDefault(s => s.SessionId == sessionId);
            Assert.NotNull(session);
            Assert.Equal(userId, session.UserId);
            Assert.True(session.IsActive);
        }

        [Fact]
        public async Task UpdateSessionActivityAsync_ShouldUpdateLastActive()
        {
            using var context = CreateInMemoryDbContext();
            var service = new SessionTrackingService(context);

            var sessionId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            
            await service.AddSessionAsync(sessionId, userId);

            
            await Task.Delay(1000); 
            await service.UpdateSessionActivityAsync(sessionId);

            var session = context.ActiveUserSession.FirstOrDefault(s => s.SessionId == sessionId);
            Assert.NotNull(session);
            Assert.True(session.LastActive > DateTime.UtcNow.AddSeconds(-2));
        }

        [Fact]
        public async Task RemoveSessionAsync_ShouldRemoveSessionFromDatabase()
        {
            using var context = CreateInMemoryDbContext();
            var service = new SessionTrackingService(context);

            var sessionId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            
            await service.AddSessionAsync(sessionId, userId);

            
            await service.RemoveSessionAsync(sessionId);

            var session = context.ActiveUserSession.FirstOrDefault(s => s.SessionId == sessionId);
            Assert.Null(session);
        }

        [Fact]
        public async Task GetActiveUserCountAsync_ShouldReturnCorrectCount()
        {
            using var context = CreateInMemoryDbContext();
            var service = new SessionTrackingService(context);

            await service.AddSessionAsync(Guid.NewGuid(), Guid.NewGuid());
            await service.AddSessionAsync(Guid.NewGuid(), Guid.NewGuid());

            var activeCount = await service.GetActiveUserCountAsync();
            Assert.Equal(2, activeCount);
        }

        [Fact]
        public async Task GetActiveUserIdsAsync_ShouldReturnCorrectIds()
        {
            using var context = CreateInMemoryDbContext();
            var service = new SessionTrackingService(context);

            var sessionId1 = Guid.NewGuid();
            var sessionId2 = Guid.NewGuid();
            await service.AddSessionAsync(sessionId1, Guid.NewGuid());
            await service.AddSessionAsync(sessionId2, Guid.NewGuid());

            var activeIds = await service.GetActiveUserIdsAsync();
            Assert.Contains(sessionId1.ToString(), activeIds);
            Assert.Contains(sessionId2.ToString(), activeIds);
        }

        [Fact]
        public async Task GetActiveUsersAsync_ShouldReturnCorrectUsers()
        {
            using var context = CreateInMemoryDbContext();
            var service = new SessionTrackingService(context);

            var sessionId1 = Guid.NewGuid();
            var sessionId2 = Guid.NewGuid();
            var userId1 = Guid.NewGuid();
            var userId2 = Guid.NewGuid();

            await service.AddSessionAsync(sessionId1, userId1);
            await service.AddSessionAsync(sessionId2, userId2);

            var users = await service.GetActiveUsersAsync();

            Assert.Equal(2, users.Count);
            Assert.Contains(users, u => u.SessionId == sessionId1 && u.UserId == userId1);
            Assert.Contains(users, u => u.SessionId == sessionId2 && u.UserId == userId2);
        }
    }
}
