using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using MindBlown.Types;
using Moq;
using Moq.Protected;
using Xunit;

public class ActiveUserClientTests
{
    private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private readonly HttpClient _httpClient;
    private readonly ActiveUserClient _activeUserClient;

    public ActiveUserClientTests()
    {
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        _httpClient = new HttpClient(_httpMessageHandlerMock.Object)
        {
            BaseAddress = new Uri("http://localhost")
        };
        _activeUserClient = new ActiveUserClient(_httpClient);
    }

    [Fact]
    public async Task IsSessionIdUniqueAsync_UniqueSessionId_ReturnsTrue()
    {
        
        var activeUsers = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
        var newSessionId = Guid.NewGuid();

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = JsonContent.Create(activeUsers)
            });

        
        var result = await _activeUserClient.IsSessionIdUniqueAsync(newSessionId);

        
        Assert.True(result);
    }

    [Fact]
    public async Task IsSessionIdUniqueAsync_NonUniqueSessionId_ReturnsFalse()
    {
        
        var commonSessionId = Guid.NewGuid();
        var activeUsers = new List<Guid> { commonSessionId, Guid.NewGuid() };

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = JsonContent.Create(activeUsers)
            });

        
        var result = await _activeUserClient.IsSessionIdUniqueAsync(commonSessionId);

        
        Assert.False(result);
    }

    [Fact]
    public async Task GetActiveUsersAsync_ReturnsListOfActiveUsers()
    {
        
        var activeUsers = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = JsonContent.Create(activeUsers)
            });

        
        var result = await _activeUserClient.GetActiveUsersAsync();

        
        Assert.NotNull(result);
        Assert.Equal(activeUsers.Count, result.Count);
    }

    [Fact]
    public async Task AddUserAsync_SendsPostRequest()
    {
        
        var userId = Guid.NewGuid();

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(r =>
                    r.Method == HttpMethod.Post &&
                    r.RequestUri.AbsolutePath == "/api/activeUser/add" &&
                    r.Content.ReadAsStringAsync().Result.Contains(userId.ToString())
                ),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK
            });

        
        await _activeUserClient.AddUserAsync(userId);

        
        _httpMessageHandlerMock.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>()
        );
    }

    [Fact]
    public async Task RemoveUserAsync_SendsPostRequest()
    {
        
        var userId = Guid.NewGuid();

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(r =>
                    r.Method == HttpMethod.Post &&
                    r.RequestUri.AbsolutePath == "/api/activeUser/remove" &&
                    r.Content.ReadAsStringAsync().Result.Contains(userId.ToString())
                ),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK
            });

        
        await _activeUserClient.RemoveUserAsync(userId);

        
        _httpMessageHandlerMock.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>()
        );
    }

    [Fact]
    public async Task GetActiveUserCountAsync_ReturnsCount()
    {
        
        var activeUserCount = 5;

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = JsonContent.Create(activeUserCount)
            });

        
        var result = await _activeUserClient.GetActiveUserCountAsync();

        
        Assert.Equal(activeUserCount, result);
    }

    [Fact]
    public async Task RemoveInnactive_SendsPostRequest()
    {
        
        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(r =>
                    r.Method == HttpMethod.Post &&
                    r.RequestUri.AbsolutePath == "/api/activeUser/removeInnactive"
                ),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK
            });

        
        await _activeUserClient.RemoveInnactive();

        
        _httpMessageHandlerMock.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>()
        );
    }

    [Fact]
    public async Task GetDictionary_ReturnsConcurrentDictionary()
    {
        
        var userDictionary = new ConcurrentDictionary<Guid, User>();
        userDictionary.TryAdd(Guid.NewGuid(), new User { isActive = true });
        userDictionary.TryAdd(Guid.NewGuid(), new User { isActive = false });

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = JsonContent.Create(userDictionary)
            });

        
        var result = await _activeUserClient.GetDictionary();

        
        Assert.NotNull(result);
        Assert.Equal(userDictionary.Count, result.Count);
    }
    
    [Fact]
    public async Task GetActiveUserCountAsync_ReturnsCorrectCount()
    {
        
        var activeUserCount = 10;

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = JsonContent.Create(activeUserCount)
            });

        
        var result = await _activeUserClient.GetActiveUserCountAsync();

        
        Assert.Equal(activeUserCount, result);
    }

    [Fact]
    public async Task GetActiveUserCountAsync_ServerError_ThrowsException()
    {
        
        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.InternalServerError
            });

        
        await Assert.ThrowsAsync<HttpRequestException>(() => _activeUserClient.GetActiveUserCountAsync());
    }

    [Fact]
    public async Task GetActiveUserCountAsync_EmptyResponse_ReturnsZero()
    {
        
        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = JsonContent.Create<int>(0)
            });

        
        var result = await _activeUserClient.GetActiveUserCountAsync();

        
        Assert.Equal(0, result);
    }

}
