using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MindBlown.Server.Controllers;
using MindBlown.Server.Data;
using MindBlown.Server.Models;
using MindBlown.Types;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MindBlow.Server.Controllers;
using Xunit;

public class UserInfoControllerTests
{
    private readonly AppDbContext _context;
    private readonly UserInfoController _controller;

    public UserInfoControllerTests()
    {
        // Setup the in-memory database
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;

        _context = new AppDbContext(options);

        // Seed initial data
        SeedDatabase();

        // Initialize the controller with the in-memory database context
        _controller = new UserInfoController(_context);
    }

    private void SeedDatabase()
    {
        var user = new UserMnemonicIDs
        {
            Id = Guid.NewGuid(),
            Username = "existingUser",
            Password = "password123",
            MnemonicGuids = new List<Guid> { Guid.NewGuid() },
            LWARecordId = Guid.NewGuid()
        };

        _context.UserWithMnemonicsIDs.Add(user);
        _context.SaveChanges();
    }

    [Fact]
    public async Task PostUser_ValidNewUser_ReturnsOk()
    {
        var newUser = new UserCredentials
        {
            Username = "newUser",
            Password = "securePassword"
        };

        var result = await _controller.PostUser(newUser);

        var actionResult = Assert.IsType<OkResult>(result);
        Assert.NotNull(actionResult);

        Assert.Contains(_context.UserWithMnemonicsIDs, u => u.Username == "newUser");
    }

    [Fact]
    public async Task PostUser_UsernameTaken_ReturnsBadRequest()
    {
        var existingUser = new UserCredentials
        {
            Username = "existingUser",
            Password = "password123"
        };

        var result = await _controller.PostUser(existingUser);

        var actionResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Username is already taken.", ((BadRequestObjectResult)result).Value);
    }

    [Fact]
    public async Task LoginUser_ValidCredentials_ReturnsToken()
    {
        
        var credentials = new UserCredentials
        {
            Username = "existingUser",
            Password = "password123"
        };

        
        var result = await _controller.LoginUser(credentials);

        
        var actionResult = Assert.IsType<ActionResult<TokenResponse>>(result); // Check the base ActionResult type
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result); // Unwrap and check the specific result type
        var tokenResponse = Assert.IsType<TokenResponse>(okResult.Value); // Ensure the value inside OkObjectResult is TokenResponse
        Assert.NotNull(tokenResponse.Token); // Verify the token is not null
    }

    [Fact]
    public async Task LoginUser_InvalidCredentials_ReturnsBadRequest()
    {

        var credentials = new UserCredentials
        {
            Username = "nonexistentUser",
            Password = "wrongPassword"
        };

        
        var result = await _controller.LoginUser(credentials);

        
        var actionResult = Assert.IsType<ActionResult<TokenResponse>>(result); // Check the base ActionResult type
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult.Result); // Unwrap and check the specific result type
        Assert.Equal("Invalid login credentials", badRequestResult.Value); // Verify the error message
    }

    [Fact]
    public async Task UpdateUserMnemonicGuids_AddValidMnemonic_ReturnsOk()
    {
        var request = new MnemonicUpdateRequest
        {
            Username = "existingUser",
            MnemonicToAdd = new MnemonicsType
            {
                Id = Guid.NewGuid(),
                MnemonicText = "Sample mnemonic",
                HelperText = "Sample helper text"
            }
        };

        var result = await _controller.UpdateUserMnemonicGuids(request);

        var actionResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Mnemonic GUIDs updated successfully.", actionResult.Value);

        Assert.Contains(_context.UserWithMnemonicsIDs.First().MnemonicGuids, g => g == request.MnemonicToAdd.Id);
    }

    [Fact]
    public async Task UpdateUserMnemonicGuids_UserNotFound_ReturnsNotFound()
    {
        var request = new MnemonicUpdateRequest
        {
            Username = "nonexistentUser",
            MnemonicToAdd = new MnemonicsType
            {
                Id = Guid.NewGuid(),
                MnemonicText = "Sample mnemonic",
                HelperText = "Sample helper text"
            }
        };

        var result = await _controller.UpdateUserMnemonicGuids(request);

        var actionResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("User not found.", actionResult.Value);
    }

    [Fact]
    public async Task UpdateUserLWARecord_ValidUpdate_ReturnsOk()
    {
        var request = new LWARecordUpdateRequest
        {
            Username = "existingUser",
            NewId = Guid.NewGuid()
        };

        var result = await _controller.UpdateUserLWARecord(request);

        var actionResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("LWARecord updated successfully.", actionResult.Value);

        Assert.Equal(request.NewId, _context.UserWithMnemonicsIDs.First().LWARecordId);
    }

    [Fact]
    public async Task UpdateUserLWARecord_EmptyGuid_ReturnsBadRequest()
    {
        var request = new LWARecordUpdateRequest
        {
            Username = "existingUser",
            NewId = Guid.Empty
        };

        var result = await _controller.UpdateUserLWARecord(request);

        var actionResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Invalid mnemonic provided.", actionResult.Value);
    }

    [Fact]
    public async Task GetLWARecordId_ValidUser_ReturnsId()
    {
        
        var result = await _controller.GetLWARecordId("existingUser");

        
        var actionResult = Assert.IsType<ActionResult<Guid>>(result); // Check the base ActionResult type
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result); // Check that the result is OkObjectResult
        var lwaRecordId = Assert.IsType<Guid>(okResult.Value); // Ensure the value inside OkObjectResult is a Guid
        Assert.NotEqual(Guid.Empty, lwaRecordId); // Validate the GUID is not empty
    }

    [Fact]
    public async Task GetLWARecordId_UserNotFound_ReturnsBadRequest()
    {
        
        var result = await _controller.GetLWARecordId("nonexistentUser");

        
        var actionResult = Assert.IsType<ActionResult<Guid>>(result); // Check the base ActionResult type
        Assert.IsType<BadRequestObjectResult>(actionResult.Result);  // Check the specific result type
        Assert.Equal("User not found", ((BadRequestObjectResult)actionResult.Result).Value);
    }

    [Fact]
    public async Task GetMnemonicsGuids_ValidUser_ReturnsList()
    {
        
        var result = await _controller.GetMnemonicsGuids("existingUser");

        
        var actionResult = Assert.IsType<ActionResult<List<Guid>>>(result); // Check base ActionResult type
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result); // Unwrap and check the specific result type
        var guids = Assert.IsType<List<Guid>>(okResult.Value); // Ensure the value inside OkObjectResult is a List<Guid>
        Assert.NotEmpty(guids); // Validate the GUID list is not empty
    }

    [Fact]
    public async Task GetMnemonicsGuids_UserNotFound_ReturnsBadRequest()
    {
        
        var result = await _controller.GetMnemonicsGuids("nonexistentUser");

        
        var actionResult = Assert.IsType<ActionResult<List<Guid>>>(result); 
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult.Result); 
        Assert.Equal("User not found", badRequestResult.Value); 
    }
    [Fact]
    public void GenerateJwtToken_ValidInput_ReturnsTokenWithCorrectExpiry()
    {
        var username = "testUser";
        
        var method = _controller.GetType().GetMethod("GenerateJwtToken", BindingFlags.NonPublic | BindingFlags.Instance);
        var token = (string)method.Invoke(_controller, new object[] { username });
        

        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        Assert.Equal("MindBlownWeb", jwtToken.Issuer);
        Assert.Equal("user-service", jwtToken.Audiences.First());
        Assert.True(jwtToken.ValidTo > DateTime.UtcNow);
    }
}
