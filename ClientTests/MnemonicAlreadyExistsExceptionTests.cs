using System;
using MindBlown.Exceptions;
using Xunit;

namespace MindBlown.Tests.Exceptions
{
    public class MnemonicAlreadyExistsExceptionTests
    {
        [Fact]
        public void MnemonicAlreadyExistsException_CanBeInstantiated_WithMessage()
        {
            
            string expectedMessage = "This mnemonic already exists.";

            
            var exception = new MnemonicAlreadyExistsException(expectedMessage);

            
            Assert.NotNull(exception);
            Assert.Equal(expectedMessage, exception.Message);
        }

        [Fact]
        public void MnemonicAlreadyExistsException_IsAssignableFromException()
        {
            
            string message = "This mnemonic already exists.";

            
            var exception = new MnemonicAlreadyExistsException(message);

            
            Assert.IsAssignableFrom<Exception>(exception);
        }
        
    }
}