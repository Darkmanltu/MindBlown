using Xunit;
using Moq;
using MindBlown.Server.Controllers;
using MindBlown.Server.Data;
using MindBlown.Server.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using MindBlow.Server.Controllers;

public class LWARecordControllerTests
{
    private readonly DbContextOptions<AppDbContext> _dbOptions;
    private readonly LWARecordController _controller;

    public LWARecordControllerTests()
    {
        
        _dbOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new AppDbContext(_dbOptions);
        _controller = new LWARecordController(context);
    }

    [Fact]
    public async Task GetRecord_ReturnsRecord_WhenFound()
    {
        
        var record = new LastWrongAnswerRecord
        {
            Id = Guid.NewGuid(),
            helperText = "Sample helper text.",
            mnemonicText = "Sample mnemonic.",
            wrongTextMnemonic = "Sample wrong mnemonic.",
            category = MnemonicCategory.Science
        };

        using (var context = new AppDbContext(_dbOptions))
        {
            context.Record.Add(record);
            await context.SaveChangesAsync();
        }

        
        var result = await _controller.GetRecord(record.Id);

        
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedRecord = Assert.IsType<LastWrongAnswerRecord>(okResult.Value);
        Assert.Equal(record.Id, returnedRecord.Id);
    }

    [Fact]
    public async Task GetRecord_ReturnsNull_WhenNotFound()
    {
      
        var result = await _controller.GetRecord(Guid.NewGuid());

       
        Assert.Null(result.Result); 
    }

    [Fact]
    public async Task PostRecord_AddsNewRecord_AndDeletesExisting()
    {
        
        var existingRecord = new LastWrongAnswerRecord
        {
            Id = Guid.NewGuid(),
            helperText = "Old helper text.",
            mnemonicText = "Old mnemonic.",
            wrongTextMnemonic = "Old wrong mnemonic.",
            category = MnemonicCategory.History
        };

        var newRecord = new LastWrongAnswerRecord
        {
            Id = Guid.NewGuid(),
            helperText = "New helper text.",
            mnemonicText = "New mnemonic.",
            wrongTextMnemonic = "New wrong mnemonic.",
            category = MnemonicCategory.Math
        };

        using (var context = new AppDbContext(_dbOptions))
        {
            context.Record.Add(existingRecord);
            await context.SaveChangesAsync();
        }

        var request = new IdLWARecordRequest
        {
            IdToChange = existingRecord.Id,
            RecordToSet = newRecord
        };

        
        var result = await _controller.PostRecord(request);

        
        using (var context = new AppDbContext(_dbOptions))
        {
            Assert.Null(await context.Record.FindAsync(existingRecord.Id)); 
            Assert.NotNull(await context.Record.FindAsync(newRecord.Id)); 
        }

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedRecord = Assert.IsType<LastWrongAnswerRecord>(okResult.Value);
        Assert.Equal(newRecord.Id, returnedRecord.Id);
    }

    [Fact]
    public async Task DeleteRecord_RemovesRecord_WhenFound()
    {
        
        var record = new LastWrongAnswerRecord
        {
            Id = Guid.NewGuid(),
            helperText = "Helper text for deletion.",
            mnemonicText = "Mnemonic for deletion.",
            wrongTextMnemonic = "Wrong mnemonic for deletion.",
            category = MnemonicCategory.Science
        };

        using (var context = new AppDbContext(_dbOptions))
        {
            context.Record.Add(record);
            await context.SaveChangesAsync();
        }

        
        var result = await _controller.DeleteRecord(record.Id);

        
        using (var context = new AppDbContext(_dbOptions))
        {
            Assert.Null(await context.Record.FindAsync(record.Id));
        }

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteRecord_ReturnsNotFound_WhenNotFound()
    {
        
        var result = await _controller.DeleteRecord(Guid.NewGuid());

        
        Assert.IsType<NotFoundResult>(result);
    }
}
