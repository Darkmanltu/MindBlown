using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MindBlown.Server.Data;
using MindBlown.Server.Models;
using MindBlown.Server.Singleton;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

public class ActiveUserControllerTests
{
    private AppDbContext CreateInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()) // Unique DB for each test
            .Options;

        return new AppDbContext(options);
    }

    private ActiveUserController CreateController(AppDbContext context)
    {
        var sessionTrackingService = new SessionTrackingService(context);
        return new ActiveUserController(sessionTrackingService);
    }
    
    [Fact]
    public async Task RemoveUser_ShouldRemoveUserFromDatabase()
    {
        using var context = CreateInMemoryDbContext();
        var controller = CreateController(context);

        var sessionId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        await context.ActiveUserSession.AddAsync(new User
        {
            SessionId = sessionId,
            UserId = userId,
            IsActive = true,
            LastActive = DateTime.UtcNow
        });
        await context.SaveChangesAsync();

        var response = await controller.RemoveUser(sessionId);
        Assert.IsType<OkResult>(response);

        var userInDb = await context.ActiveUserSession.FirstOrDefaultAsync(s => s.SessionId == sessionId);
        Assert.Null(userInDb);
    }

    [Fact]
    public async Task GetActiveUsers_ShouldReturnActiveUsers()
    {
        using var context = CreateInMemoryDbContext();
        var controller = CreateController(context);

        var user1 = new User
        {
            SessionId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            IsActive = true,
            LastActive = DateTime.UtcNow
        };

        var user2 = new User
        {
            SessionId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            IsActive = false, // Inactive user
            LastActive = DateTime.UtcNow.AddMinutes(-10)
        };

        await context.ActiveUserSession.AddRangeAsync(user1, user2);
        await context.SaveChangesAsync();

        var response = await controller.GetActiveUsers() as OkObjectResult;
        Assert.NotNull(response);

        var activeUsers = Assert.IsType<List<string>>(response.Value);
        Assert.Single(activeUsers);
        Assert.Contains(user1.SessionId.ToString(), activeUsers);
    }

    [Fact]
    public async Task GetDictionary_ShouldReturnConcurrentDictionary()
    {
        using var context = CreateInMemoryDbContext();
        var controller = CreateController(context);

        var user1 = new User
        {
            SessionId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            IsActive = true,
            LastActive = DateTime.UtcNow
        };

        var user2 = new User
        {
            SessionId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            IsActive = true,
            LastActive = DateTime.UtcNow.AddMinutes(-1)
        };

        await context.ActiveUserSession.AddRangeAsync(user1, user2);
        await context.SaveChangesAsync();

        var dictionary = await controller.GetDictionary();
        Assert.NotNull(dictionary);
        Assert.Equal(2, dictionary.Count);
        Assert.True(dictionary.ContainsKey(user1.SessionId));
        Assert.True(dictionary.ContainsKey(user2.SessionId));
    }

    [Fact]
    public async Task RemoveInactiveUsersAsync_ShouldRemoveUsersInactiveForFiveMinutes()
    {
        using var context = CreateInMemoryDbContext();
        var controller = CreateController(context);

        var activeUser = new User
        {
            SessionId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            IsActive = true,
            LastActive = DateTime.UtcNow
        };

        var inactiveUser = new User
        {
            SessionId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            IsActive = true,
            LastActive = DateTime.UtcNow.AddMinutes(-10) // Inactive
        };

        await context.ActiveUserSession.AddRangeAsync(activeUser, inactiveUser);
        await context.SaveChangesAsync();

        var response = await controller.RemoveInactiveUsersAsync();
        Assert.IsType<OkResult>(response);

        var remainingUsers = await context.ActiveUserSession.ToListAsync();
        Assert.Single(remainingUsers);
        Assert.Equal(activeUser.SessionId, remainingUsers.First().SessionId);
    }
}
