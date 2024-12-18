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
using MindBlown;
using Services;

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
        
        var jsonResponse = "{\"Token\": \"dummyToken\"}";

        var jsRuntimeMock = new Mock<IJSRuntime>();
        
        var httpClientMock = new Mock<HttpMessageHandler>();
        httpClientMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonResponse) 
            });

        var httpClient = new HttpClient(httpClientMock.Object)
        {
            BaseAddress = new Uri("https://example.com/") 
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
        
        var httpClientMock = new Mock<HttpMessageHandler>();

        httpClientMock
            .Protected()  
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest,
                Content = new StringContent("Invalid credentials")
            });

        var httpClient = new HttpClient(httpClientMock.Object)
        {
            BaseAddress = new Uri("https://example.com/") 
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

        var httpClientMock = new HttpClient();
        var authService = new AuthService(jsRuntimeMock.Object, httpClientMock);

        // Act
        var result = await authService.GetUsername();

        // Assert
        Assert.Equal("testuser", result);
    }

    [Fact]
    public async Task GetUsername_ReturnsNull_WhenTokenIsInvalid()
    {
     
        var invalidToken = "invalid.token.string";

        var jsRuntimeMock = new Mock<IJSRuntime>();
        jsRuntimeMock.Setup(js => js.InvokeAsync<string>("localStorage.getItem", It.IsAny<object[]>())).ReturnsAsync(invalidToken);

        var httpClientMock = new HttpClient(); 
        var authService = new AuthService(jsRuntimeMock.Object, httpClientMock);

       
        var result = await authService.GetUsername();

   
        Assert.Null(result);
    }

    [Fact]
    public async Task SomeMethod_ReturnsMnemonic_WhenSuccessful()
    {
       
        var mnemonic = new MnemonicsType
        {
            
        };
        var toAdd = true;

        var jsRuntimeMock = new Mock<IJSRuntime>();

       
        var httpClientMock = new Mock<HttpMessageHandler>();

    
        httpClientMock
            .Protected() 
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = JsonContent.Create(mnemonic) 
            });

        var httpClient = new HttpClient(httpClientMock.Object)
        {
            BaseAddress = new Uri("https://example.com/") 
        };

        var authService = new AuthService(jsRuntimeMock.Object, httpClient);

    
        var result = await authService.UpdateUserWithMnemonic("testuser", mnemonic, toAdd);

      
        Assert.Equal(mnemonic, result);
    }

    [Fact]
    public async Task SomeMethod_ReturnsExpectedGuids_WhenSuccessful()
    {
  
        var expectedGuids = new List<Guid>
        {
            Guid.NewGuid(),
            Guid.NewGuid()
        };

        var jsRuntimeMock = new Mock<IJSRuntime>();

      
        var httpClientMock = new Mock<HttpMessageHandler>();

        
        httpClientMock
            .Protected()  
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = JsonContent.Create(expectedGuids) 
            });
        
        
        var httpClient = new HttpClient(httpClientMock.Object)
        {
            BaseAddress = new Uri("https://example.com/") 
        };

        var authService = new AuthService(jsRuntimeMock.Object, httpClient);

   
        var result = await authService.GetMnemonicsGuids("testuser");

 
        Assert.Equal(expectedGuids, result);  
    }

    [Fact]
    public async Task SomeMethod_ReturnsNewId_WhenSuccessful()
    {
     
        var newId = Guid.NewGuid();

        var jsRuntimeMock = new Mock<IJSRuntime>();

       
        var httpClientMock = new Mock<HttpMessageHandler>();

      
        httpClientMock
            .Protected()  
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = JsonContent.Create(newId)  
            });
        
        var httpClient = new HttpClient(httpClientMock.Object)
        {
            BaseAddress = new Uri("https://example.com/") 
        };

        var authService = new AuthService(jsRuntimeMock.Object, httpClient);

     
        var result = await authService.GetLWARecordId("testuser");

      
        Assert.Equal(newId, result);  
    }
    
    
    [Fact]
    public async Task SignupAsync_ReturnsSuccess_WhenSignupSuccessful()
    {
      
        var signupRequest = new AccRequest { Username = "testuser", Password = "password" };
        var jsonResponse = "Success";

        var jsRuntimeMock = new Mock<IJSRuntime>();

        var httpClientMock = new Mock<HttpMessageHandler>();
        httpClientMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonResponse)
            });

        var httpClient = new HttpClient(httpClientMock.Object)
        {
            BaseAddress = new Uri("https://example.com/")
        };

        var authService = new AuthService(jsRuntimeMock.Object, httpClient);

    
        var result = await authService.SignupAsync(signupRequest);

        Assert.Equal("Success", result);
    }
    
    [Fact]
    public async Task SignupAsync_ReturnsError_WhenSignupFails()
    {
   
        var signupRequest = new AccRequest { Username = "testuser", Password = "wrongpassword" };
        var errorMessage = "Invalid input";

        var jsRuntimeMock = new Mock<IJSRuntime>();

        var httpClientMock = new Mock<HttpMessageHandler>();
        httpClientMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest,
                Content = new StringContent(errorMessage)
            });

        var httpClient = new HttpClient(httpClientMock.Object)
        {
            BaseAddress = new Uri("https://example.com/")
        };

        var authService = new AuthService(jsRuntimeMock.Object, httpClient);

       
        var result = await authService.SignupAsync(signupRequest);

     
        Assert.Equal(errorMessage, result);
    }
    
    [Fact]
    public async Task UpdateLWARecord_ReturnsNewId_WhenSuccessful()
    {
      
        var username = "testuser";
        var newId = Guid.NewGuid();
    
        var jsRuntimeMock = new Mock<IJSRuntime>();
    
        var httpClientMock = new Mock<HttpMessageHandler>();
        httpClientMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = JsonContent.Create(newId)
            });
    
        var httpClient = new HttpClient(httpClientMock.Object)
        {
            BaseAddress = new Uri("https://example.com/")
        };
    
        var authService = new AuthService(jsRuntimeMock.Object, httpClient);
    
       
        var result = await authService.UpdateLWARecord(username, newId);
    
  
        Assert.Equal(newId, result);
    }
    
    [Fact]
    public async Task UpdateLWARecord_ReturnsNull_WhenUpdateFails()
    {
  
        var username = "testuser";
        var newId = Guid.NewGuid();

        var jsRuntimeMock = new Mock<IJSRuntime>();

        var httpClientMock = new Mock<HttpMessageHandler>();
        httpClientMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest,
                Content = new StringContent("Error updating LWA record")
            });

        var httpClient = new HttpClient(httpClientMock.Object)
        {
            BaseAddress = new Uri("https://example.com/")
        };

        var authService = new AuthService(jsRuntimeMock.Object, httpClient);

     
        var result = await authService.UpdateLWARecord(username, newId);

     
        Assert.Null(result);
    }

    

}
