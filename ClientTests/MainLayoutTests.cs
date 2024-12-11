using System;
using System.Reflection;
using System.Threading.Tasks;
using Bunit;
using Moq;
using Xunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using MindBlown.Interfaces;
using MindBlown.Layout;
using MindBlown.Types;

public class MainLayoutTests
{
    private readonly Mock<IAuthService> _authServiceMock;
    private readonly Mock<NavigationManager> _navigationManagerMock;
    private readonly MainLayout _mainLayout;

    public MainLayoutTests()
    {
        _authServiceMock = new Mock<IAuthService>();
        _navigationManagerMock = new Mock<NavigationManager>();
        _mainLayout = new MainLayout
        {
            AuthService = _authServiceMock.Object,
            NavigationManager = _navigationManagerMock.Object
        };
    }

    [Fact]
    public async Task OnInitializedAsync_UserLoggedIn_SetsIsLoggedInAndUsername()
    {
        _authServiceMock.Setup(x => x.IsUserLoggedInAsync()).ReturnsAsync(true);
        _authServiceMock.Setup(x => x.GetUsername()).ReturnsAsync("TestUser");

        var method = typeof(MainLayout).GetMethod("OnInitializedAsync", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        if (method == null)
        {
            throw new InvalidOperationException("OnInitializedAsync method not found.");
        }
        await (Task)method.Invoke(_mainLayout, null);

        Assert.True(_mainLayout.isLoggedIn);
        Assert.Equal("TestUser", _mainLayout.username);
    }

    [Fact]
    public async Task OnInitializedAsync_UserNotLoggedIn_SetsIsLoggedInToFalse()
    {
        _authServiceMock.Setup(x => x.IsUserLoggedInAsync()).ReturnsAsync(false);

        var method = typeof(MainLayout).GetMethod("OnInitializedAsync", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        if (method == null)
        {
            throw new InvalidOperationException("OnInitializedAsync method not found.");
        }
        await (Task)method.Invoke(_mainLayout, null);

        Assert.False(_mainLayout.isLoggedIn);
        Assert.Null(_mainLayout.username);
    }

    // [Fact]
    // public async Task HandleLogout_LogsOutUser_AndNavigatesToHome()
    // {
    //     // Arrange
    //     _authServiceMock.Setup(x => x.LogoutAsync()).Returns(Task.CompletedTask);
    //     string baseUri = "http://localhost/";
    //     _navigationManagerMock.SetupGet(x => x.BaseUri).Returns(baseUri);
    //
    //     // Act
    //     await _mainLayout.HandleLogout();
    //
    //     // Assert
    //     Assert.False(_mainLayout.isLoggedIn);
    //     Assert.Null(_mainLayout.username);
    //
    //     // Use explicit arguments for NavigateTo
    //     _navigationManagerMock.Verify(x => x.NavigateTo(baseUri, false, true), Times.Once);
    // }
    
    [Fact]
    public void ShowSignupModal_SetsSignupModalVisibilityToTrue()
    {
        _mainLayout.ShowSignupModal();

        Assert.True(_mainLayout.isSignupModalVisible);
    }

    [Fact]
    public void ShowLoginModal_SetsLoginModalVisibilityToTrue()
    {
        _mainLayout.ShowLoginModal();

        Assert.True(_mainLayout.isLoginModalVisible);
    }

    [Fact]
    public void OnSignupModalVisibilityChanged_UpdatesSignupModalVisibility()
    {
        _mainLayout.OnSignupModalVisibilityChanged(true);

        Assert.True(_mainLayout.isSignupModalVisible);

        _mainLayout.OnSignupModalVisibilityChanged(false);

        Assert.False(_mainLayout.isSignupModalVisible);
    }

    [Fact]
    public void OnLoginModalVisibilityChanged_UpdatesLoginModalVisibility()
    {
        _mainLayout.OnLoginModalVisibilityChanged(true);

        Assert.True(_mainLayout.isLoginModalVisible);

        _mainLayout.OnLoginModalVisibilityChanged(false);


        Assert.False(_mainLayout.isLoginModalVisible);
    }

    // [Fact]
    // public async Task HandleSignup_SuccessfulSignup_ClosesSignupModal()
    // {
    //     // Arrange
    //     _authServiceMock.Setup(x => x.SignupAsync(It.IsAny<AccRequest>())).ReturnsAsync("Success");
    //
    //     // Act
    //     await _mainLayout.HandleSignup();
    //
    //     // Assert
    //     Assert.False(_mainLayout.isSignupModalVisible);
    // }

    // [Fact]
    // public async Task HandleSignup_FailedSignup_ShowsErrorMessage()
    // {
    //     // Arrange
    //     _authServiceMock.Setup(x => x.SignupAsync(It.IsAny<AccRequest>())).ReturnsAsync("Error: Signup failed");
    //
    //     // Act
    //     await _mainLayout.HandleSignup();
    //
    //     // Assert
    //     Assert.NotNull(_mainLayout.errorMessage);
    //     Assert.Equal("Error: Signup failed", _mainLayout.errorMessage);
    // }

    // [Fact]
    // public async Task HandleLogin_SuccessfulLogin_SetsIsLoggedInAndClosesLoginModal()
    // {
    //     // Arrange
    //     _authServiceMock.Setup(x => x.LoginAsync(It.IsAny<AccRequest>())).ReturnsAsync("TestUser");
    //
    //     // Act
    //     await _mainLayout.HandleLogin();
    //
    //     // Assert
    //     Assert.True(_mainLayout.isLoggedIn);
    //     Assert.Equal("TestUser", _mainLayout.username);
    //     Assert.False(_mainLayout.isLoginModalVisible);
    // }

    // [Fact]
    // public async Task HandleLogin_FailedLogin_ShowsErrorMessage()
    // {
    //     using var context = new TestContext();
    //
    //     // Arrange
    //     var authServiceMock = new Mock<IAuthService>();
    //     authServiceMock.Setup(x => x.LoginAsync(It.IsAny<AccRequest>())).ReturnsAsync("Invalid credentials");
    //
    //     // Register the mocked AuthService in the test context's service collection
    //     context.Services.AddSingleton(authServiceMock.Object);
    //
    //     // Render the MainLayout component using the new Render method
    //     var mainLayout = context.Render<MainLayout>();
    //
    //     // Act
    //     await mainLayout.Instance.HandleLogin();
    //
    //     // Assert
    //     Assert.Equal("Invalid credentials", mainLayout.Instance.errorMessage);
    // }

        // [Fact]
        // public async Task ShowErrorMessage_SetsAndClearsErrorMessage()
        // {
        //     // Act
        //     await _mainLayout.ShowErrorMessage("Test Error Message");
        //
        //     // Assert
        //     Assert.Null(_mainLayout.errorMessage);
        // }
}
