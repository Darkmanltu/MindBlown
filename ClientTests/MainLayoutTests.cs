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
    
}
