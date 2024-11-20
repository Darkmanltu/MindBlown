using Xunit;
using MindBlown.Types;
using System;

namespace MindBlown.Tests
{
    public class LogEntryTests
    {
        [Fact]
        public void Constructor_ShouldInitializeWithDefaults()
        {
            
            var logEntry = new LogEntry();

            
            Assert.Equal(string.Empty, logEntry.Message);
            Assert.Equal(string.Empty, logEntry.Details);
            Assert.Equal("Error", logEntry.LogLevel);
            Assert.True((DateTime.UtcNow - logEntry.Timestamp).TotalSeconds < 1, "Timestamp should be initialized to current UTC time.");
        }

        [Fact]
        public void Properties_ShouldAllowSettingValues()
        {
            
            var message = "Test message";
            var details = "Detailed log information";
            var timestamp = DateTime.UtcNow.AddMinutes(-5);
            var logLevel = "Warning";

            
            var logEntry = new LogEntry
            {
                Message = message,
                Details = details,
                Timestamp = timestamp,
                LogLevel = logLevel
            };

            
            Assert.Equal(message, logEntry.Message);
            Assert.Equal(details, logEntry.Details);
            Assert.Equal(timestamp, logEntry.Timestamp);
            Assert.Equal(logLevel, logEntry.LogLevel);
        }

        [Theory]
        [InlineData("Info")]
        [InlineData("Warning")]
        [InlineData("Error")]
        public void LogLevel_ShouldAcceptDifferentLevels(string logLevel)
        {
            
            var logEntry = new LogEntry { LogLevel = logLevel };

            
            Assert.Equal(logLevel, logEntry.LogLevel);
        }

        [Fact]
        public void Message_ShouldAllowEmptyString()
        {
            
            var logEntry = new LogEntry { Message = string.Empty };

            
            Assert.Equal(string.Empty, logEntry.Message);
        }

        [Fact]
        public void Details_ShouldAllowEmptyString()
        {
            
            var logEntry = new LogEntry { Details = string.Empty };

            
            Assert.Equal(string.Empty, logEntry.Details);
        }
    }
}
