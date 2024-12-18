using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MindBlow.Server.Controllers;
using MindBlown.Server.Models;
using MindBlown.Server.Data;
using Xunit;

public class MnemonicsControllerTests
{
    private AppDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task GetMnemonics_ShouldReturnAllMnemonics()
    {
        using var context = GetInMemoryDbContext();
        context.Mnemonics.AddRange(
            new Mnemonic { HelperText = "Mnemonic1" },
            new Mnemonic { HelperText = "Mnemonic2" }
        );
        await context.SaveChangesAsync();

        var controller = new MnemonicsController(context);

        var result = await controller.GetMnemonics();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var mnemonics = Assert.IsType<List<Mnemonic>>(okResult.Value);
        Assert.Equal(2, mnemonics.Count);
    }

    [Fact]
    public async Task GetMnemonic_ShouldReturnMnemonic_WhenExists()
    {
        using var context = GetInMemoryDbContext();
        var mnemonic = new Mnemonic { Id = Guid.NewGuid(), HelperText = "Sample Mnemonic" };
        context.Mnemonics.Add(mnemonic);
        await context.SaveChangesAsync();

        var controller = new MnemonicsController(context);

        var result = await controller.GetMnemonic(mnemonic.Id);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedMnemonic = Assert.IsType<Mnemonic>(okResult.Value);
        Assert.Equal(mnemonic.Id, returnedMnemonic.Id);
    }

    [Fact]
    public async Task GetMnemonic_ShouldReturnNotFound_WhenNotExists()
    {
        using var context = GetInMemoryDbContext();
        var controller = new MnemonicsController(context);

        var result = await controller.GetMnemonic(Guid.NewGuid());

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task PostMnemonic_ShouldCreateMnemonic()
    {
        using var context = GetInMemoryDbContext();
        var controller = new MnemonicsController(context);
        var mnemonic = new Mnemonic { HelperText = "New Mnemonic" };

        var result = await controller.PostMnemonic(mnemonic);

        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var createdMnemonic = Assert.IsType<Mnemonic>(createdAtActionResult.Value);
        Assert.Equal("New Mnemonic", createdMnemonic.HelperText);
    }

    [Fact]
    public async Task PutMnemonic_ShouldUpdateMnemonic_WhenExists()
    {
        using var context = GetInMemoryDbContext();
        var mnemonic = new Mnemonic { Id = Guid.NewGuid(), HelperText = "Original Text" };
        context.Mnemonics.Add(mnemonic);
        await context.SaveChangesAsync();

        var controller = new MnemonicsController(context);
        mnemonic.HelperText = "Updated Text";

        var result = await controller.PutMnemonic(mnemonic.Id, mnemonic);

        Assert.IsType<NoContentResult>(result);
        var updatedMnemonic = await context.Mnemonics.FindAsync(mnemonic.Id);
        Assert.Equal("Updated Text", updatedMnemonic.HelperText);
    }

    [Fact]
    public async Task PutMnemonic_ShouldReturnBadRequest_WhenIdsMismatch()
    {
        using var context = GetInMemoryDbContext();
        var controller = new MnemonicsController(context);
        var mnemonic = new Mnemonic
        {
            Id = Guid.NewGuid(),
            HelperText = "Original Mnemonic"
        };

        var result = await controller.PutMnemonic(Guid.NewGuid(), mnemonic);

        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task DeleteMnemonic_ShouldRemoveMnemonic()
    {
        using var context = GetInMemoryDbContext();
        var mnemonic = new Mnemonic { Id = Guid.NewGuid(), HelperText = "Mnemonic to Delete" };
        context.Mnemonics.Add(mnemonic);
        await context.SaveChangesAsync();

        var controller = new MnemonicsController(context);

        var result = await controller.DeleteMnemonic(mnemonic.Id);

        Assert.IsType<NoContentResult>(result);
        Assert.Null(await context.Mnemonics.FindAsync(mnemonic.Id));
    }

    [Fact]
    public void Test_ShouldReturnOk()
    {
        var controller = new MnemonicsController(null!); 

        var result = controller.Test();

        Assert.IsType<OkObjectResult>(result);
        var okResult = result as OkObjectResult;
        Assert.Equal("API is reachable", okResult.Value);
    }

    [Fact]
    public async Task MnemonicExists_ShouldReturnTrue_WhenMnemonicExists()
    {
        using var context = GetInMemoryDbContext();
        var controller = new MnemonicsController(context);
        var mnemonic = new Mnemonic
        {
            Id = Guid.NewGuid(),
            HelperText = "Test Mnemonic"
        };
        context.Mnemonics.Add(mnemonic);
        await context.SaveChangesAsync();

        var result = controller.MnemonicExists(mnemonic.Id);

        Assert.True(result);
    }

    [Fact]
    public async Task MnemonicExists_ShouldReturnFalse_WhenMnemonicDoesNotExist()
    {
        using var context = GetInMemoryDbContext();
        var controller = new MnemonicsController(context);

        var result = controller.MnemonicExists(Guid.NewGuid());

        Assert.False(result);
    }
    
    [Fact]
    public async Task PostMnemonics_ShouldAddUniqueMnemonics_Bulk()
    {
        using var context = GetInMemoryDbContext();
        var controller = new MnemonicsController(context);

        var mnemonics = new List<Mnemonic>
        {
            new Mnemonic { Id = Guid.NewGuid(), HelperText = "Helper 1", MnemonicText = "Mnemonic 1" },
            new Mnemonic { Id = Guid.NewGuid(), HelperText = "Helper 2", MnemonicText = "Mnemonic 2" }
        };

        var result = await controller.PostMnemonics(mnemonics);

        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
        var createdMnemonics = Assert.IsType<List<Mnemonic>>(createdAtActionResult.Value);
        Assert.Equal(2, createdMnemonics.Count);

        var savedMnemonics = await context.Mnemonics.ToListAsync();
        Assert.Equal(2, savedMnemonics.Count);  
        Assert.Contains(savedMnemonics, m => m.HelperText == "Helper 1");
        Assert.Contains(savedMnemonics, m => m.HelperText == "Helper 2");
    }
    
    [Fact]
    public async Task PostMnemonics_ShouldReturnBadRequest_WhenNoMnemonicsProvided_Bulk()
    {
        using var context = GetInMemoryDbContext();
        var controller = new MnemonicsController(context);

        var result = await controller.PostMnemonics(new List<Mnemonic>());

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("No mnemonics provided.", badRequestResult.Value);
    }
    
    [Fact]
    public async Task PostMnemonics_ShouldReturnBadRequest_WhenNullMnemonicsProvided_Bulk()
    {
        using var context = GetInMemoryDbContext();
        var controller = new MnemonicsController(context);

        var result = await controller.PostMnemonics(null);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("No mnemonics provided.", badRequestResult.Value);
    }
    
    [Fact]
    public async Task DeleteMnemonic_ShouldReturnNotFound_WhenMnemonicDoesNotExist()
    {
        
        using var context = GetInMemoryDbContext();
        var controller = new MnemonicsController(context);
        
        var result = await controller.DeleteMnemonic(Guid.NewGuid());
        
        Assert.IsType<NotFoundResult>(result);
    }
    
    [Fact]
    public async Task PutMnemonic_ShouldReturnNotFound_WhenMnemonicDoesNotExist()
    {
        using var context = GetInMemoryDbContext();
        var controller = new MnemonicsController(context);
        var mnemonic = new Mnemonic
        {
            Id = Guid.NewGuid(),
            HelperText = "Non-existent Mnemonic"
        };

  
        var result = await controller.PutMnemonic(mnemonic.Id, mnemonic);
        
        Assert.IsType<NotFoundResult>(result);
    }
    
    
}
