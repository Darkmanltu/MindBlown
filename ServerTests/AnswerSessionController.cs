using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MindBlown.Server.Controllers;
using MindBlown.Server.Data;
using MindBlown.Server.Models;

public class AnswerSessionControllerTests
{
    private AppDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task CreateAnswerSession_ShouldReturnCreatedResult_WhenValidAnswerSession()
    {
        using var context = GetInMemoryDbContext();
        var controller = new AnswerSessionController(context);
        var answerSession = new AnswerSession { UserName = "testuser" };

        var result = await controller.CreateAnswerSession(answerSession);

        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var createdSession = Assert.IsType<AnswerSession>(createdAtActionResult.Value);
        Assert.Equal("testuser", createdSession.UserName);
    }

    [Fact]
    public async Task CreateAnswerSession_ShouldReturnBadRequest_WhenNullAnswerSession()
    {
        using var context = GetInMemoryDbContext();
        var controller = new AnswerSessionController(context);

        var result = await controller.CreateAnswerSession(null);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetAnswerSessionList_ShouldReturnListOfSessions_WhenValidUser()
    {
        using var context = GetInMemoryDbContext();
        var controller = new AnswerSessionController(context);

        var answerSession1 = new AnswerSession { AnswerSessionId = Guid.NewGuid(), UserName = "testuser" };
        var answerSession2 = new AnswerSession { AnswerSessionId = Guid.NewGuid(), UserName = "testuser" };
        context.AnswerSessions.AddRange(answerSession1, answerSession2);
        await context.SaveChangesAsync();

        var result = await controller.GetAnswerSessionList("testuser");

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var sessions = Assert.IsType<List<AnswerSession>>(okResult.Value);
        Assert.Equal(2, sessions.Count);
    }

    [Fact]
    public async Task GetAnswerSessionList_ShouldReturnBadRequest_WhenNoUserProvided()
    {
        using var context = GetInMemoryDbContext();
        var controller = new AnswerSessionController(context);

        var result = await controller.GetAnswerSessionList(null);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetAnswerSession_ShouldReturnAnswerSession_WhenValidId()
    {
        using var context = GetInMemoryDbContext();
        var controller = new AnswerSessionController(context);

        var answerSession = new AnswerSession { AnswerSessionId = Guid.NewGuid(), UserName = "testuser" };
        context.AnswerSessions.Add(answerSession);
        await context.SaveChangesAsync();

        var result = await controller.GetAnswerSession(answerSession.AnswerSessionId);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedSession = Assert.IsType<AnswerSession>(okResult.Value);
        Assert.Equal(answerSession.AnswerSessionId, returnedSession.AnswerSessionId);
    }

    [Fact]
    public async Task GetAnswerSession_ShouldReturnNotFound_WhenInvalidId()
    {
        using var context = GetInMemoryDbContext();
        var controller = new AnswerSessionController(context);

        var result = await controller.GetAnswerSession(Guid.NewGuid());

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task AddAnsweredMnemonic_ShouldReturnOk_WhenValidData()
    {
        using var context = GetInMemoryDbContext();
        var controller = new AnswerSessionController(context);

        var answerSession = new AnswerSession { AnswerSessionId = Guid.NewGuid(), UserName = "testuser" };
        context.AnswerSessions.Add(answerSession);
        await context.SaveChangesAsync();

        var answeredMnemonic = new AnsweredMnemonic
        {
            AnswerSessionId = answerSession.AnswerSessionId,
            MnemonicId = Guid.NewGuid(),
            IsCorrect = true,
            AnswerSession = new AnswerSession { UserName = "testuser" }
        };

        var result = await controller.AddAnsweredMnemonic(answeredMnemonic);

        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task AddAnsweredMnemonic_ShouldReturnNotFound_WhenInvalidSessionId()
    {
        using var context = GetInMemoryDbContext();
        var controller = new AnswerSessionController(context);

        var answeredMnemonic = new AnsweredMnemonic
        {
            AnswerSessionId = Guid.NewGuid(),
            MnemonicId = Guid.NewGuid(),
            IsCorrect = true,
            AnswerSession = new AnswerSession
            {
            UserName = "testuser"
            }
            
            
        };

        var result = await controller.AddAnsweredMnemonic(answeredMnemonic);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task RemoveAnswerSession_ShouldReturnOk_WhenValidId()
    {
        using var context = GetInMemoryDbContext();
        var controller = new AnswerSessionController(context);

        var answerSession = new AnswerSession { AnswerSessionId = Guid.NewGuid(), UserName = "testuser" };
        context.AnswerSessions.Add(answerSession);
        await context.SaveChangesAsync();

        var result = await controller.RemoveAnswerSession(answerSession.AnswerSessionId);

        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task RemoveAnswerSession_ShouldReturnNotFound_WhenInvalidId()
    {
        using var context = GetInMemoryDbContext();
        var controller = new AnswerSessionController(context);

        var result = await controller.RemoveAnswerSession(Guid.NewGuid());

        Assert.IsType<NotFoundResult>(result);
    }
}
