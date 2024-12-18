using System;
using MindBlown.Exceptions;
using Xunit;

    public class MnemonicServiceExceptionTests
    {
        [Fact]
        public void MnemonicServiceException_CanBeInstantiated_WithMessage()
        {
            
            string expectedMessage = "Service error occurred.";

            
            var exception = new MnemonicServiceException(expectedMessage);

            
            Assert.NotNull(exception);
            Assert.Equal(expectedMessage, exception.Message);
        }

        [Fact]
        public void MnemonicServiceException_CanBeInstantiated_WithMessageAndInnerException()
        {
            
            string expectedMessage = "Service error occurred.";
            var innerException = new InvalidOperationException("Inner exception message.");

            
            var exception = new MnemonicServiceException(expectedMessage, innerException);

            
            Assert.NotNull(exception);
            Assert.Equal(expectedMessage, exception.Message);
            Assert.Equal(innerException, exception.InnerException);
        }

        [Fact]
        public void MnemonicServiceException_IsAssignableFromException()
        {
            
            string message = "Service error occurred.";

            
            var exception = new MnemonicServiceException(message);

            
            Assert.IsAssignableFrom<Exception>(exception);
        }
    }
