using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Xunit;
using Microsoft.JSInterop;
using System.Net;
using MindBlown.Types;
using Moq.Protected;

public class AuthServiceTests
{
    [Fact]
    public async Task IsUserLoggedInAsync_ReturnsTrue_WhenTokenExists()
    {

        var jsRuntimeMock = new Mock<IJSRuntime>();
        jsRuntimeMock.Setup(js => js.InvokeAsync<string>("localStorage.getItem", It.IsAny<object[]>())).ReturnsAsync("dummyToken");

        var httpClientMock = new HttpClient(); // Not used here
        var authService = new AuthService(jsRuntimeMock.Object, httpClientMock);

        
        var result = await authService.IsUserLoggedInAsync();

        
        Assert.True(result);
    }

    [Fact]
    public async Task IsUserLoggedInAsync_ReturnsFalse_WhenNoTokenExists()
    {
        
        var jsRuntimeMock = new Mock<IJSRuntime>();
        jsRuntimeMock.Setup(js => js.InvokeAsync<string>("localStorage.getItem", It.IsAny<object[]>())).ReturnsAsync((string?)null);

        var httpClientMock = new HttpClient(); // Not used here
        var authService = new AuthService(jsRuntimeMock.Object, httpClientMock);


        var result = await authService.IsUserLoggedInAsync();


        Assert.False(result);
    }

    [Fact]
    public async Task LoginAsync_ReturnsUsername_WhenLoginSuccessful()
    {

        var loginRequest = new AccRequest { Username = "testuser", Password = "password" };
        var tokenResponse = new TokenResponse { Token = "dummyToken" };

        var jsRuntimeMock = new Mock<IJSRuntime>();

        // Mock HttpMessageHandler to mock SendAsync method
        var httpClientMock = new Mock<HttpMessageHandler>();
        httpClientMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = JsonContent.Create(tokenResponse)
            });

        var httpClient = new HttpClient(httpClientMock.Object)
        {
            BaseAddress = new Uri("https://example.com/") // Set the BaseAddress for HttpClient
        };

        var authService = new AuthService(jsRuntimeMock.Object, httpClient);


        var result = await authService.LoginAsync(loginRequest);
        
        Assert.Equal(loginRequest.Username, result);
    }

    [Fact]
    public async Task LoginAsync_ReturnsErrorMessage_WhenLoginFails()
    {

        var loginRequest = new AccRequest { Username = "testuser", Password = "wrongpassword" };
        
        var jsRuntimeMock = new Mock<IJSRuntime>();

        // Mock HttpMessageHandler to mock SendAsync method
        var httpClientMock = new Mock<HttpMessageHandler>();

        httpClientMock
            .Protected()  // Protected() to access protected methods like SendAsync
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest,
                Content = new StringContent("Invalid credentials")
            });

        var httpClient = new HttpClient(httpClientMock.Object)
        {
            BaseAddress = new Uri("https://example.com/") // Set the BaseAddress
        };

        var authService = new AuthService(jsRuntimeMock.Object, httpClient);

        
        var result = await authService.LoginAsync(loginRequest);

        
        Assert.Equal("Invalid credentials", result);
    }
    

    [Fact]
    public async Task GetUsername_ReturnsUsername_WhenValidTokenExists()
    {
        // Arrange
        var validToken = new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(claims: new[]
        {
            new Claim(ClaimTypes.Name, "testuser")
        }));

        var jsRuntimeMock = new Mock<IJSRuntime>();
        jsRuntimeMock.Setup(js => js.InvokeAsync<string>("localStorage.getItem", It.IsAny<object[]>())).ReturnsAsync(validToken);

        var httpClientMock = new HttpClient(); // Not used here
        var authService = new AuthService(jsRuntimeMock.Object, httpClientMock);

        // Act
        var result = await authService.GetUsername();

        // Assert
        Assert.Equal("testuser", result);
    }

    [Fact]
    public async Task GetUsername_ReturnsNull_WhenTokenIsInvalid()
    {
        // Arrange
        var invalidToken = "invalid.token.string";

        var jsRuntimeMock = new Mock<IJSRuntime>();
        jsRuntimeMock.Setup(js => js.InvokeAsync<string>("localStorage.getItem", It.IsAny<object[]>())).ReturnsAsync(invalidToken);

        var httpClientMock = new HttpClient(); // Not used here
        var authService = new AuthService(jsRuntimeMock.Object, httpClientMock);

        // Act
        var result = await authService.GetUsername();

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task SomeMethod_ReturnsMnemonic_WhenSuccessful()
    {
        // Arrange
        var mnemonic = new MnemonicsType { /* Initialize your mnemonic object here */ };

        var jsRuntimeMock = new Mock<IJSRuntime>();

        // Mock HttpMessageHandler to mock SendAsync method
        var httpClientMock = new Mock<HttpMessageHandler>();

        // Set up the SendAsync method on HttpMessageHandler to return a successful response
        httpClientMock
            .Protected()  // Use Protected() to access protected methods like SendAsync
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = JsonContent.Create(mnemonic)  // Return the mnemonic object as content
            });
        
        
        var httpClient = new HttpClient(httpClientMock.Object)
        {
            BaseAddress = new Uri("https://example.com/") // Set the BaseAddress for HttpClient
        };

        var authService = new AuthService(jsRuntimeMock.Object, httpClient);

        // Act
        var result = await authService.UpdateUserWithMnemonic("testuser", mnemonic);

        // Assert
        Assert.Equal(mnemonic, result);  // Check if the result is the same mnemonic object returned
    }

    [Fact]
    public async Task SomeMethod_ReturnsExpectedGuids_WhenSuccessful()
    {
        // Arrange
        var expectedGuids = new List<Guid>
        {
            Guid.NewGuid(),
            Guid.NewGuid()
        };

        var jsRuntimeMock = new Mock<IJSRuntime>();

        // Mock HttpMessageHandler to mock SendAsync method
        var httpClientMock = new Mock<HttpMessageHandler>();

        // Set up the SendAsync method on HttpMessageHandler to return a successful response
        httpClientMock
            .Protected()  // Use Protected() to access protected methods like SendAsync
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = JsonContent.Create(expectedGuids)  // Return the expected Guids as content
            });
        
        
        var httpClient = new HttpClient(httpClientMock.Object)
        {
            BaseAddress = new Uri("https://example.com/") // Set the BaseAddress for HttpClient
        };

        var authService = new AuthService(jsRuntimeMock.Object, httpClient);

        // Act
        var result = await authService.GetMnemonicsGuids("testuser");

        // Assert
        Assert.Equal(expectedGuids, result);  // Check if the result matches the expected list of Guids
    }

    [Fact]
    public async Task SomeMethod_ReturnsNewId_WhenSuccessful()
    {
        // Arrange
        var newId = Guid.NewGuid();

        var jsRuntimeMock = new Mock<IJSRuntime>();

        // Mock HttpMessageHandler to mock SendAsync method
        var httpClientMock = new Mock<HttpMessageHandler>();

        // Set up the SendAsync method on HttpMessageHandler to return a successful response
        httpClientMock
            .Protected()  // Use Protected() to access protected methods like SendAsync
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = JsonContent.Create(newId)  // Return the newId as content
            });
        
        var httpClient = new HttpClient(httpClientMock.Object)
        {
            BaseAddress = new Uri("https://example.com/") // Set the BaseAddress for HttpClient
        };

        var authService = new AuthService(jsRuntimeMock.Object, httpClient);

        // Act
        var result = await authService.GetLWARecordId("testuser");

        // Assert
        Assert.Equal(newId, result);  // Check if the result matches the newId returned
    }

}
