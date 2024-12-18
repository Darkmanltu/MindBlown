using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using Moq;
using Moq.Protected;
using Xunit;
using MindBlown.Types;
using MindBlown.Services;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

public class AnswerStatServiceTests
{
    private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private readonly HttpClient _httpClient;
    private readonly AnswerStatService _service;

    public AnswerStatServiceTests()
    {
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        _httpClient = new HttpClient(_httpMessageHandlerMock.Object)
        {
            BaseAddress = new Uri("https://testapi.local/")
        };
        _service = new AnswerStatService(_httpClient);
    }

    [Fact]
    public async Task GetList_ShouldReturnAnswerSessionList_OnSuccess()
    {
     
        var mockResponse = new List<AnswerSessionType>
        {
            new AnswerSessionType { AnswerSessionId = Guid.NewGuid(), UserName = "testuser" },
            new AnswerSessionType { AnswerSessionId = Guid.NewGuid(), UserName = "testuser2" }
        };
        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Get &&
                    req.RequestUri.ToString().Contains("api/answersession/list?user=testuser")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = JsonContent.Create(mockResponse)
            });

   
        var result = await _service.GetList("testuser");

   
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("testuser", result[0].UserName);
    }

    [Fact]
    public async Task GetList_ShouldReturnEmptyList_OnFailure()
    {
     
        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest
            });


        var result = await _service.GetList("testuser");

    
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task CreateAnswerSessionAsync_ShouldReturnTrue_OnSuccess()
    {
   
        var answerSession = new AnswerSessionType { AnswerSessionId = Guid.NewGuid(), UserName = "testuser" };
        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Post &&
                    req.RequestUri.ToString().Contains("api/answersession/add")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK
            });

  
        var result = await _service.CreateAnswerSessionAsync(answerSession);


        Assert.True(result);
    }

    [Fact]
    public async Task AddAnsweredMnemonicAsync_ShouldReturnTrue_OnSuccess()
    {
    
        var answeredMnemonic = new AnsweredMnemonicType
        {
            AnsweredMnemonicId = Guid.NewGuid(),
            MnemonicId = Guid.NewGuid(),
            IsCorrect = true,
            AnswerSessionId = Guid.NewGuid(),
            AnswerSession = new AnswerSessionType
            {
                AnswerSessionId = Guid.NewGuid(),
                UserName = "testuser"
            }
        };

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Post &&
                    req.RequestUri.ToString().Contains("api/answersession/addAnsweredMnemonic")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK
            });

   
        var result = await _service.AddAnsweredMnemonicAsync(answeredMnemonic);

       
        Assert.True(result);
    }

    [Fact]
    public async Task AddAnswerSessionAsync_ShouldReturnFalse_WhenNoAnswerMnemonics()
    {
 
        var answerSession = new AnswerSessionType { AnswerSessionId = Guid.NewGuid(), UserName = "testuser" };


        var result = await _service.AddAnswerSessionAsync(answerSession, null);

 
        Assert.False(result);
    }

    [Fact]
    public async Task AddAnswerSessionAsync_ShouldReturnTrue_OnSuccess()
    {
  
        var answerSession = new AnswerSessionType { AnswerSessionId = Guid.NewGuid(), UserName = "testuser" };
        var answeredMnemonics = new List<AnsweredMnemonicType>
        {
            new AnsweredMnemonicType
            {
                AnsweredMnemonicId = Guid.NewGuid(),
                MnemonicId = Guid.NewGuid(),
                IsCorrect = true,
                AnswerSessionId = Guid.NewGuid(),
                AnswerSession = new AnswerSessionType
                {
                    AnswerSessionId = Guid.NewGuid(),
                    UserName = "testuser"
                }
            }
        };
        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK
            });


        var result = await _service.AddAnswerSessionAsync(answerSession, answeredMnemonics);

        Assert.True(result);
    }

    [Fact]
    public async Task CreateAnswerSessionAsync_ShouldReturnFalse_OnException()
    {

        var answerSession = new AnswerSessionType { AnswerSessionId = Guid.NewGuid(), UserName = "testuser" };
        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new HttpRequestException("Simulated network error"));


        var result = await _service.CreateAnswerSessionAsync(answerSession);


        Assert.False(result);
    }

    [Fact]
    public async Task CreateAnswerSessionAsync_ShouldReturnFalse_OnHttpResponseFailure()
    {
        // Arrange
        var answerSession = new AnswerSessionType { AnswerSessionId = Guid.NewGuid(), UserName = "testuser" };
        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest 
            });

        
        var result = await _service.CreateAnswerSessionAsync(answerSession);

   
        Assert.True(result);
    }

    [Fact]
    public async Task AddAnswerSessionAsync_ShouldReturnFalse_OnAnswerSessionApiFailure()
    {

        var answerSession = new AnswerSessionType { AnswerSessionId = Guid.NewGuid(), UserName = "testuser" };
        var answeredMnemonics = new List<AnsweredMnemonicType>
        {
            new AnsweredMnemonicType
            {
                AnsweredMnemonicId = Guid.NewGuid(),
                MnemonicId = Guid.NewGuid(),
                IsCorrect = true,
                AnswerSessionId = answerSession.AnswerSessionId,
                AnswerSession = answerSession
            }
        };

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.RequestUri.ToString().Contains("api/answersession/add")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest // Simulating a failure in adding the session
            });


        var result = await _service.AddAnswerSessionAsync(answerSession, answeredMnemonics);


        Assert.False(result);
    }

    [Fact]
    public async Task AddAnswerSessionAsync_ShouldReturnFalse_OnMnemonicApiFailure()
    {
        // Arrange
        var answerSession = new AnswerSessionType { AnswerSessionId = Guid.NewGuid(), UserName = "testuser" };
        var answeredMnemonics = new List<AnsweredMnemonicType>
        {
            new AnsweredMnemonicType
            {
                AnsweredMnemonicId = Guid.NewGuid(),
                MnemonicId = Guid.NewGuid(),
                IsCorrect = true,
                AnswerSessionId = answerSession.AnswerSessionId,
                AnswerSession = answerSession
            }
        };

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.RequestUri.ToString().Contains("api/answersession/add")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK // Successful session addition
            });

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.RequestUri.ToString().Contains("api/answersession/addAnsweredMnemonic")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest // Simulating a failure in adding mnemonics
            });


        var result = await _service.AddAnswerSessionAsync(answerSession, answeredMnemonics);


        Assert.False(result);
    }

    [Fact]
    public async Task AddAnswerSessionAsync_ShouldReturnFalse_OnExceptionInSessionAddition()
    {

        var answerSession = new AnswerSessionType { AnswerSessionId = Guid.NewGuid(), UserName = "testuser" };
        var answeredMnemonics = new List<AnsweredMnemonicType>
        {
            new AnsweredMnemonicType
            {
                AnsweredMnemonicId = Guid.NewGuid(),
                MnemonicId = Guid.NewGuid(),
                IsCorrect = true,
                AnswerSessionId = answerSession.AnswerSessionId,
                AnswerSession = answerSession
            }
        };

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.RequestUri.ToString().Contains("api/answersession/add")),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new HttpRequestException("Simulated exception in session addition"));


        var result = await _service.AddAnswerSessionAsync(answerSession, answeredMnemonics);


        Assert.False(result);
    }

    [Fact]
    public async Task AddAnswerSessionAsync_ShouldReturnFalse_OnExceptionInMnemonicAddition()
    {

        var answerSession = new AnswerSessionType { AnswerSessionId = Guid.NewGuid(), UserName = "testuser" };
        var answeredMnemonics = new List<AnsweredMnemonicType>
        {
            new AnsweredMnemonicType
            {
                AnsweredMnemonicId = Guid.NewGuid(),
                MnemonicId = Guid.NewGuid(),
                IsCorrect = true,
                AnswerSessionId = answerSession.AnswerSessionId,
                AnswerSession = answerSession
            }
        };

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.RequestUri.ToString().Contains("api/answersession/add")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK 
            });

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.RequestUri.ToString().Contains("api/answersession/addAnsweredMnemonic")),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new HttpRequestException("Simulated exception in mnemonic addition"));


        var result = await _service.AddAnswerSessionAsync(answerSession, answeredMnemonics);
        
        Assert.False(result);
    }
}
