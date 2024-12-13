using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;
using FluentAssertions;
using MindBlown.Types;
using Services;
using Xunit;

namespace Services.Tests
{
    public class LWARecordServiceTests
    {
        private readonly Mock<HttpClient> _mockHttpClient;
        private readonly LWARecordService _service;

        public LWARecordServiceTests()
        {

            _mockHttpClient = new Mock<HttpClient>(MockBehavior.Strict);
            _service = new LWARecordService(_mockHttpClient.Object);
        }

        [Fact]
        public async Task GetRecordAsync_ReturnsRecord_WhenRecordFound()
        {

            var testGuid = Guid.NewGuid();
            var testRecord = new LastWrongAnswerRecord
            {
                Id = testGuid,
                helperText = "Test Helper Text",
                mnemonicText = "Test Mnemonic",
                wrongTextMnemonic = "Test Wrong Mnemonic",
                category = MnemonicCategory.Other
            };

            var mockResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(testRecord)
            };

            
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();


            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Get &&
                        req.RequestUri.ToString().Contains($"api/lwarecord?id={testGuid}")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(mockResponse);

            var httpClient = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("http://localhost/") 
            };

            var service = new LWARecordService(httpClient);

            
            var result = await service.GetRecordAsync(testGuid);


            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(testRecord);
        }

        [Fact]
        public async Task GetRecordAsync_ReturnsNull_WhenRecordNotFound()
        {

            var testGuid = Guid.NewGuid();
            var mockResponse = new HttpResponseMessage(HttpStatusCode.NotFound);

            // Create a mock HttpMessageHandler
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);

            // Mock SendAsync to return 404 Not Found
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Get &&
                        req.RequestUri.ToString().Contains($"api/lwarecord?id={testGuid}")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(mockResponse);

            // Create HttpClient with the mocked handler

            var httpClient = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("http://localhost/") 
            };


            var service = new LWARecordService(httpClient);

            
            var result = await service.GetRecordAsync(testGuid);


            result.Should().BeNull(); 
        }

        [Fact]
        public async Task UpdateRecordAsync_ReturnsUpdatedRecord_WhenRequestIsSuccessful()
        {

            var testGuid = Guid.NewGuid();
            var testRecord = new LastWrongAnswerRecord
            {
                Id = testGuid,
                helperText = "Test Helper Text",
                mnemonicText = "Test Mnemonic",
                wrongTextMnemonic = "Test Wrong Mnemonic",
                category = MnemonicCategory.Other
            };

            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();

            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Post &&
                        req.RequestUri.ToString().Contains("api/lwarecord")), 
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = JsonContent.Create(testRecord)
                });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("https://example.com/") 
            };

            var service = new LWARecordService(httpClient);

            
            var result = await service.UpdateRecordAsync(testGuid, testRecord);

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(testRecord);

            mockHttpMessageHandler.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Post &&
                    req.RequestUri.ToString().Contains("api/lwarecord")), 
                ItExpr.IsAny<CancellationToken>());
        }


        [Fact]
        public async Task UpdateRecordAsync_ThrowsException_WhenRequestFails()
        {
            var testGuid = Guid.NewGuid();
            var testRecord = new LastWrongAnswerRecord
            {
                Id = testGuid,
                helperText = "Test Helper Text",
                mnemonicText = "Test Mnemonic",
                wrongTextMnemonic = "Test Wrong Mnemonic",
                category = MnemonicCategory.Other
            };

            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();

            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Post),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.BadRequest));

            var httpClient = new HttpClient(mockHttpMessageHandler.Object);
            var service = new LWARecordService(httpClient);

            await Assert.ThrowsAsync<InvalidOperationException>(() => service.UpdateRecordAsync(testGuid, testRecord));
        }

        [Fact]
        public async Task LogErrorToServerAsync_LogsErrorSuccessfully()
        {
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();

            // Mock SendAsync to simulate successful POST request
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Post && req.RequestUri.ToString() == "https://example.com/api/logs"),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

            var httpClient = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("https://example.com/") 
            };

            var service = new LWARecordService(httpClient);

            await service.LogErrorToServerAsync("Test message", "Test details");

            mockHttpMessageHandler.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Post && req.RequestUri.ToString() == "https://example.com/api/logs"),
                ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public async Task LogErrorToServerAsync_HandlesLoggingFailure()
        {
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();

            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Post && req.RequestUri.ToString().Contains("api/logs")),
                    ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new Exception("Logging failed"));

            var httpClient = new HttpClient(mockHttpMessageHandler.Object);
            var service = new LWARecordService(httpClient);

            
            var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);

            
            await service.LogErrorToServerAsync("Test message", "Test details");

            
            consoleOutput.ToString().Should().Contain("Failed to log to server");
        }

    }
}
