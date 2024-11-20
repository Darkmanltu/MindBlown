using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using MindBlown.Server.Controllers;

public class LoggingControllerTests
{
    [Fact]
    public async Task Log_ShouldWriteLogEntryToFile_WhenLogEntryIsValid()
    {
        
        var controller = new LoggingController();
        var logEntry = new LogEntry
        {
            Message = "Test log message",
            Details = "This is a test log",
            Timestamp = DateTime.UtcNow,
            LogLevel = "Info"
        };

        string logFilePath = "logs/log.txt";
        if (File.Exists(logFilePath))
        {
            File.Delete(logFilePath); 
        }

        
        var result = await controller.Log(logEntry);

        
        Assert.IsType<OkResult>(result);
        Assert.True(File.Exists(logFilePath)); // Ensure the file was created
        var logContent = await File.ReadAllTextAsync(logFilePath);
        Assert.Contains(logEntry.Message, logContent);
        Assert.Contains(logEntry.Details, logContent);
        Assert.Contains(logEntry.LogLevel, logContent);
    }

    // [Fact]
    // public async Task Log_ShouldReturn500_WhenExceptionIsThrown()
    // {
    //     // Arrange
    //     var controller = new LoggingController();
    //     var logEntry = new LogEntry { Message = "Test", Details = "Testing exception" };
    //
    //     // Simulate a failure by providing an invalid path
    //     string originalPath = "logs/log.txt";
    //     string invalidPath = ":invalid_path/log.txt";
    //     typeof(LoggingController)
    //         .GetField("logFilePath", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
    //         .SetValue(null, invalidPath);
    //
    //     // Act
    //     var result = await controller.Log(logEntry) as ObjectResult;
    //
    //     // Assert
    //     Assert.NotNull(result);
    //     Assert.Equal(500, result.StatusCode);
    //     Assert.Contains("Internal server error", result.Value.ToString());
    //
    //     // Cleanup
    //     typeof(LoggingController)
    //         .GetField("logFilePath", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
    //         .SetValue(null, originalPath);
    // }

    [Fact]
    public async Task Log_ShouldCreateDirectory_WhenDirectoryDoesNotExist()
    {
        
        var controller = new LoggingController();
        var logEntry = new LogEntry { Message = "Directory test", Details = "Testing directory creation" };
        string directoryPath = "logs";
        if (Directory.Exists(directoryPath))
        {
            Directory.Delete(directoryPath, true); 
        }

        
        var result = await controller.Log(logEntry);

        
        Assert.IsType<OkResult>(result);
        Assert.True(Directory.Exists(directoryPath)); 
    }
}
