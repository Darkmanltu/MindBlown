using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;
using Xunit;
using MindBlown.Types;
using MindBlown.Exceptions;
using Services;

namespace ClientTests;

public class MnemonicServiceTests
    {
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private readonly HttpClient _httpClient;
        private readonly MnemonicService _mnemonicService;

        public MnemonicServiceTests()
        {
            
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            _httpClient = new HttpClient(_httpMessageHandlerMock.Object)
            {
                BaseAddress = new Uri("http://localhost")
            };
            _mnemonicService = new MnemonicService(_httpClient);
        }

        [Fact]
        public async Task GetMnemonicsAsync_ReturnsMnemonicsList()
        {
            
            var expectedMnemonics = new List<MnemonicsType>
            {
                new MnemonicsType { Id = Guid.NewGuid(), HelperText = "Helper 1", MnemonicText = "Mnemonic 1", Category = MnemonicCategory.Chemistry },
                new MnemonicsType { Id = Guid.NewGuid(), HelperText = "Helper 2", MnemonicText = "Mnemonic 2", Category = MnemonicCategory.Art }
            };

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = JsonContent.Create(expectedMnemonics)
                });

            // Act
            var result = await _mnemonicService.GetMnemonicsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedMnemonics.Count, result.Count);
        }

        [Fact]
        public async Task GetMnemonicAsync_NotFound_ReturnsNull()
        {
            
            var mnemonicId = Guid.NewGuid();

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(r => r.RequestUri.AbsolutePath.Contains(mnemonicId.ToString())),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NotFound
                });

            
            var result = await _mnemonicService.GetMnemonicAsync(mnemonicId);

            
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateMnemonicAsync_ReturnsCreatedMnemonic()
        {
            
            var newMnemonic = new MnemonicsType { Id = Guid.NewGuid(), HelperText = "New Helper", MnemonicText = "New Mnemonic", Category = MnemonicCategory.Art };

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.Created,
                    Content = JsonContent.Create(newMnemonic)
                });

            
            var result = await _mnemonicService.CreateMnemonicAsync(newMnemonic);

            
            Assert.NotNull(result);
            Assert.Equal(newMnemonic.HelperText, result.HelperText);
        }

        [Fact]
        public async Task DeleteMnemonicAsync_WhenCalled_EnsuresSuccessStatusCode()
        {
            
            var mnemonicId = Guid.NewGuid();

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NoContent
                });

            
            await _mnemonicService.DeleteMnemonicAsync(mnemonicId);
            _httpMessageHandlerMock.VerifyAll();
        }
    }