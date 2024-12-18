using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using MindBlow.Server.Controllers;
using MindBlown.Server.Data;
using MindBlown.Server.Models;

public class UserInfoControllerTests
{
    private readonly AppDbContext _context;

    public UserInfoControllerTests()
    {
        
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _context = new AppDbContext(options);
    }

    [Fact]
    public async Task PostUser_SuccessfulSignup_ReturnsOk()
    {
       
        var user = new UserCredentials { Username = "testuser", Password = "password123" };
        var controller = new UserInfoController(_context);

        
        var result = await controller.PostUser(user);

        
        var okResult = Assert.IsType<OkResult>(result);
        Assert.NotNull(okResult);
    }

    [Fact]
    public async Task PostUser_UsernameAlreadyExists_ReturnsBadRequest()
    {
       
        var existingUser = new UserMnemonicIDs
        {
            Id = Guid.NewGuid(),
            Username = "testuser",
            Password = "hashedPassword",
            MnemonicGuids = new List<Guid>(),
            LWARecordId = Guid.NewGuid()
        };
        _context.UserWithMnemonicsIDs.Add(existingUser);
        await _context.SaveChangesAsync();

        var newUser = new UserCredentials { Username = "testuser", Password = "password123" };
        var controller = new UserInfoController(_context);

       
        var result = await controller.PostUser(newUser);

        
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Username is already taken.", badRequestResult.Value);
    }

    [Fact]
    public async Task LoginUser_ValidCredentials_ReturnsToken()
    {
        
        var passwordHasher = new Microsoft.AspNetCore.Identity.PasswordHasher<object>();
        var hashedPassword = passwordHasher.HashPassword(new object(), "password123");

        var user = new UserMnemonicIDs
        {
            Id = Guid.NewGuid(),
            Username = "testuser",
            Password = hashedPassword,
            MnemonicGuids = new List<Guid>(),
            LWARecordId = Guid.NewGuid()
        };
        _context.UserWithMnemonicsIDs.Add(user);
        await _context.SaveChangesAsync();

        var credentials = new UserCredentials { Username = "testuser", Password = "password123" };
        var controller = new UserInfoController(_context);

       
        var result = await controller.LoginUser(credentials);

       
        var actionResult = Assert.IsType<ActionResult<TokenResponse>>(result); 
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result); 
        var tokenResponse = Assert.IsType<TokenResponse>(okResult.Value); 
        Assert.NotNull(tokenResponse.Token); 
    }
    

    [Fact]
    public async Task UpdateUserMnemonicGuids_AddMnemonic_ReturnsOk()
    {
        
        var user = new UserMnemonicIDs
        {
            Id = Guid.NewGuid(),
            Username = "testuser",
            MnemonicGuids = new List<Guid>(),
            LWARecordId = Guid.NewGuid(),
            Password = "hashedPassword"
        };
        _context.UserWithMnemonicsIDs.Add(user);
        await _context.SaveChangesAsync();

        var mnemonicId = Guid.NewGuid();
        var request = new MnemonicUpdateRequest
        {
            Username = "testuser",
            MnemonicToAdd = new Mnemonic { Id = mnemonicId },
            ToAdd = true
        };

        var controller = new UserInfoController(_context);
        
        var result = await controller.UpdateUserMnemonicGuids(request);
        
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Mnemonic GUIDs updated successfully.", okResult.Value);
    }

    [Fact]
    public async Task GetLWARecordId_ValidUser_ReturnsLWARecordId()
    {
      
        var lwaRecordId = Guid.NewGuid();
        var user = new UserMnemonicIDs
        {
            Id = Guid.NewGuid(),
            Username = "testuser",
            MnemonicGuids = new List<Guid>(),
            LWARecordId = lwaRecordId,
            Password = "hashedPassword"
        };
        _context.UserWithMnemonicsIDs.Add(user);
        await _context.SaveChangesAsync();

        var controller = new UserInfoController(_context);

        
        var result = await controller.GetLWARecordId("testuser");

       
        var actionResult = Assert.IsType<ActionResult<Guid>>(result); 
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result); 
        Assert.Equal(lwaRecordId, okResult.Value);
    }

    [Fact]
    public async Task GetMnemonicsGuids_ValidUser_ReturnsGuidList()
    {
        var mnemonicIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
        var user = new UserMnemonicIDs
        {
            Id = Guid.NewGuid(),
            Username = "testuser",
            MnemonicGuids = mnemonicIds,
            LWARecordId = Guid.NewGuid(),
            Password = "hashedPassword"
        };
        _context.UserWithMnemonicsIDs.Add(user);
        await _context.SaveChangesAsync();

        var controller = new UserInfoController(_context);

       
        var result = await controller.GetMnemonicsGuids("testuser");

        
        var actionResult = Assert.IsType<ActionResult<List<Guid>>>(result); 
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result); 
        var resultIds = Assert.IsType<List<Guid>>(okResult.Value);
        Assert.Equal(mnemonicIds, resultIds);
    }
    
    [Fact]
    public async Task GetLWARecordId_InvalidUser_ReturnsBadRequest()
    {
      
        var controller = new UserInfoController(_context);

 
        var result = await controller.GetLWARecordId("nonexistentuser");

       
        var actionResult = Assert.IsType<ActionResult<Guid>>(result);
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
        Assert.Equal("User not found", badRequestResult.Value);
    }
    
    [Fact]
    public async Task GetMnemonicsGuids_InvalidUser_ReturnsBadRequest()
    {
        
        var controller = new UserInfoController(_context);

        
        var result = await controller.GetMnemonicsGuids("nonexistentuser");

        
        var actionResult = Assert.IsType<ActionResult<List<Guid>>>(result);
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
        Assert.Equal("User not found", badRequestResult.Value);
    }
    
    [Fact]
    public async Task LoginUser_InvalidPassword_ReturnsBadRequest()
    {
        
        var passwordHasher = new Microsoft.AspNetCore.Identity.PasswordHasher<object>();
        var hashedPassword = passwordHasher.HashPassword(new object(), "correctpassword");

        var user = new UserMnemonicIDs
        {
            Id = Guid.NewGuid(),
            Username = "testuser",
            Password = hashedPassword,
            MnemonicGuids = new List<Guid>(),
            LWARecordId = Guid.NewGuid()
        };
        _context.UserWithMnemonicsIDs.Add(user);
        await _context.SaveChangesAsync();

        var credentials = new UserCredentials { Username = "testuser", Password = "wrongpassword" };
        var controller = new UserInfoController(_context);

        
        var result = await controller.LoginUser(credentials);
        
        var actionResult = Assert.IsType<ActionResult<TokenResponse>>(result);
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
        Assert.Equal("Invalid login credentials", badRequestResult.Value);
    }
    
    [Fact]
    public async Task UpdateUserMnemonicGuids_InvalidUser_ReturnsNotFound()
    {
        
        var request = new MnemonicUpdateRequest
        {
            Username = "nonexistentuser",
            MnemonicToAdd = new Mnemonic { Id = Guid.NewGuid() },
            ToAdd = true
        };
        var controller = new UserInfoController(_context);

      
        var result = await controller.UpdateUserMnemonicGuids(request);
        
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("User not found.", notFoundResult.Value);
    }
    
    [Fact]
    public async Task UpdateUserMnemonicGuids_InvalidMnemonic_ReturnsBadRequest()
    {
        var user = new UserMnemonicIDs
        {
            Id = Guid.NewGuid(),
            Username = "testuser",
            MnemonicGuids = new List<Guid>(),
            LWARecordId = Guid.NewGuid(),
            Password = "hashedPassword"
        };
        _context.UserWithMnemonicsIDs.Add(user);
        await _context.SaveChangesAsync();

        var request = new MnemonicUpdateRequest
        {
            Username = "testuser",
            MnemonicToAdd = null, 
            ToAdd = true
        };
        var controller = new UserInfoController(_context);
        
        var result = await controller.UpdateUserMnemonicGuids(request);
        
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Invalid mnemonic provided.", badRequestResult.Value);
    }
    
    [Fact]
    public async Task UpdateUserMnemonicGuids_RemoveNonExistentMnemonic_ReturnsOk()
    {
        var user = new UserMnemonicIDs
        {
            Id = Guid.NewGuid(),
            Username = "testuser",
            MnemonicGuids = new List<Guid>(),
            LWARecordId = Guid.NewGuid(),
            Password = "hashedPassword"
        };
        _context.UserWithMnemonicsIDs.Add(user);
        await _context.SaveChangesAsync();

        var request = new MnemonicUpdateRequest
        {
            Username = "testuser",
            MnemonicToAdd = new Mnemonic { Id = Guid.NewGuid() }, // Non-existent Mnemonic
            ToAdd = false
        };
        var controller = new UserInfoController(_context);

       
        var result = await controller.UpdateUserMnemonicGuids(request);
        
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Mnemonic GUIDs updated successfully.", okResult.Value);
    }
}
