using System;
using System.ComponentModel.DataAnnotations;
using MindBlown.Types;
using Xunit;

namespace MindBlown.Tests
{
    public class UserValidationTests
    {
        [Fact]
        public void User_WithValidData_IsValid()
        {
            
            var user = new User
            {
                userId = Guid.NewGuid(),
                sessionId = Guid.NewGuid(),
                lastActive = DateTime.UtcNow,
                isActive = true
            };
            
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(user, new ValidationContext(user), validationResults, true);

            Assert.True(isValid);
            Assert.Empty(validationResults);
        }
    }
}