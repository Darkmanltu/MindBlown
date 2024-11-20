using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;
using Services;
using MindBlown.Types;
using Xunit;

namespace ClientTests
{
    public class LoggingServiceTest
    {
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private readonly HttpClient _httpClient;
        private readonly LoggingService _loggingService;

        public LoggingServiceTest()
        {
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            _httpClient = new HttpClient(_httpMessageHandlerMock.Object)
            {
                BaseAddress = new Uri("http://localhost")
            };
            _loggingService = new LoggingService(_httpClient);
        }

        [Fact]
        public async Task LogAsync_SendsLogEntryToApi()
        {
            
            var logEntry = new LogEntry
            {
                Message = "Test log message",
                Details = "Detailed information about the log entry",
                Timestamp = DateTime.UtcNow,
                LogLevel = "Info"
            };

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Post &&
                        req.RequestUri == new Uri("http://localhost/api/logs") &&
                        req.Content.Headers.ContentType.MediaType == "application/json"),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK
                })
                .Verifiable();

            
            await _loggingService.LogAsync(logEntry);

            
            _httpMessageHandlerMock.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Post &&
                    req.RequestUri == new Uri("http://localhost/api/logs")),
                ItExpr.IsAny<CancellationToken>());
        }
    }
}
