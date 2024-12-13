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
            new MnemonicsType
            {
                Id = Guid.NewGuid(), HelperText = "Helper 1", MnemonicText = "Mnemonic 1",
                Category = MnemonicCategory.Chemistry
            },
            new MnemonicsType
            {
                Id = Guid.NewGuid(), HelperText = "Helper 2", MnemonicText = "Mnemonic 2",
                Category = MnemonicCategory.Art
            }
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

        var result = await _mnemonicService.GetMnemonicsAsync();

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

        var newMnemonic = new MnemonicsType
        {
            Id = Guid.NewGuid(), HelperText = "New Helper", MnemonicText = "New Mnemonic",
            Category = MnemonicCategory.Art
        };

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

    [Fact]
    public async Task GetMnemonicsByIdsAsync_ReturnsMnemonicsList()
    {
        var ids = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
        var mnemonics = new List<MnemonicsType>
        {
            new MnemonicsType
            {
                Id = ids[0], HelperText = "Helper 1", MnemonicText = "Mnemonic 1", Category = MnemonicCategory.Chemistry
            },
            new MnemonicsType
                { Id = ids[1], HelperText = "Helper 2", MnemonicText = "Mnemonic 2", Category = MnemonicCategory.Art }
        };

        foreach (var mnemonic in mnemonics)
        {
            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(r => r.RequestUri.AbsolutePath.Contains(mnemonic.Id.ToString())),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = JsonContent.Create(mnemonic)
                });
        }

        var result = await _mnemonicService.GetMnemonicsByIdsAsync(ids);

        Assert.NotNull(result);
        Assert.Equal(mnemonics.Count, result.Count);
        foreach (var mnemonic in mnemonics)
        {
            Assert.Contains(result, r => r.Id == mnemonic.Id);
        }
    }

    [Fact]
    public async Task UpdateMnemonicAsync_ReturnsUpdatedMnemonic()
    {
        var updatedMnemonic = new MnemonicsType
        {
            Id = Guid.NewGuid(), HelperText = "Updated Helper", MnemonicText = "Updated Mnemonic",
            Category = MnemonicCategory.Chemistry
        };

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(r => r.RequestUri.AbsolutePath.Contains(updatedMnemonic.Id.ToString())),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = JsonContent.Create(updatedMnemonic)
            });

        var result = await _mnemonicService.UpdateMnemonicAsync(updatedMnemonic);

        Assert.NotNull(result);
        Assert.Equal(updatedMnemonic.HelperText, result.HelperText);
        Assert.Equal(updatedMnemonic.MnemonicText, result.MnemonicText);
        Assert.Equal(updatedMnemonic.Category, result.Category);
    }

    [Fact]
    public async Task LogErrorToServerAsync_SendsLogEntry()
    {
        var logMessage = "Test Error Message";
        var logDetails = "Error details here";

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(r =>
                    r.Method == HttpMethod.Post &&
                    r.RequestUri.AbsolutePath.Contains("api/logs")
                ),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK
            });

        await _mnemonicService.LogErrorToServerAsync(logMessage, logDetails);

        _httpMessageHandlerMock.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(r =>
                r.Method == HttpMethod.Post &&
                r.RequestUri.AbsolutePath.Contains("api/logs")
            ),
            ItExpr.IsAny<CancellationToken>()
        );
    }
    
    [Fact]
    public async Task GetMnemonicsByIdsAsync_EmptyIdsList_ReturnsEmptyList()
    {
        var ids = new List<Guid>();

        var result = await _mnemonicService.GetMnemonicsByIdsAsync(ids);

        Assert.NotNull(result);
        Assert.Empty(result);
    }
    
    [Fact]
    public async Task GetMnemonicsByIdsAsync_OneIdNotFound_ReturnsPartialList()
    {
        var ids = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
        var foundMnemonic = new MnemonicsType { Id = ids[0], HelperText = "Helper", MnemonicText = "Mnemonic", Category = MnemonicCategory.Chemistry };

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(r => r.RequestUri.AbsolutePath.Contains(ids[0].ToString())),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = JsonContent.Create(foundMnemonic)
            });

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(r => r.RequestUri.AbsolutePath.Contains(ids[1].ToString())),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NotFound
            });

        var result = await _mnemonicService.GetMnemonicsByIdsAsync(ids);

        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(foundMnemonic.Id, result[0].Id);
    }
    
    [Fact]
    public async Task GetMnemonicsByIdsAsync_AllIdsNotFound_ReturnsEmptyList()
    {
        var ids = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NotFound
            });

        var result = await _mnemonicService.GetMnemonicsByIdsAsync(ids);

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task UpdateMnemonicAsync_NotFound_ThrowsException()
    {
        var mnemonicToUpdate = new MnemonicsType { Id = Guid.NewGuid(), HelperText = "Helper", MnemonicText = "Mnemonic", Category = MnemonicCategory.Chemistry };

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(r => r.RequestUri.AbsolutePath.Contains(mnemonicToUpdate.Id.ToString())),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NotFound
            });

        var exception = await Assert.ThrowsAsync<MnemonicServiceException>(() => _mnemonicService.UpdateMnemonicAsync(mnemonicToUpdate));
        Assert.Contains(mnemonicToUpdate.Id.ToString(), exception.Message);
    }

    [Fact]
    public async Task UpdateMnemonicAsync_ServerError_ThrowsException()
    {
        var mnemonicToUpdate = new MnemonicsType { Id = Guid.NewGuid(), HelperText = "Helper", MnemonicText = "Mnemonic", Category = MnemonicCategory.Chemistry };

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(r => r.RequestUri.AbsolutePath.Contains(mnemonicToUpdate.Id.ToString())),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.InternalServerError
            });

        var exception = await Assert.ThrowsAsync<MnemonicServiceException>(() => _mnemonicService.UpdateMnemonicAsync(mnemonicToUpdate));
        Assert.Contains("An error occurred while updating the mnemonic", exception.Message);
    }

    [Fact]
    public async Task LogErrorToServerAsync_ServerError_DoesNotThrowException()
    {
        var logMessage = "Test Error Message";
        var logDetails = "Error details here";

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(r =>
                    r.Method == HttpMethod.Post &&
                    r.RequestUri.AbsolutePath.Contains("api/logs")
                ),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.InternalServerError
            });

        await _mnemonicService.LogErrorToServerAsync(logMessage, logDetails);

        _httpMessageHandlerMock.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(r =>
                r.Method == HttpMethod.Post &&
                r.RequestUri.AbsolutePath.Contains("api/logs")
            ),
            ItExpr.IsAny<CancellationToken>()
        );
    }
    
}