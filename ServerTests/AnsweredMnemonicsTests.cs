using System;
using Xunit;
using MindBlown.Server.Models;

namespace MindBlown.Tests.Models
{
    public class AnsweredMnemonicTests
    {
        [Fact]
        public void AnsweredMnemonic_CreatesSuccessfully_WhenAllPropertiesAreSet()
        {
           
            var answerSession = new AnswerSession
            {
                AnswerSessionId = Guid.NewGuid(),
                UserName = "testuser"
            };

           
            var answeredMnemonic = new AnsweredMnemonic
            {
                AnsweredMnemonicId = Guid.NewGuid(),
                AnswerSessionId = Guid.NewGuid(),
                MnemonicId = Guid.NewGuid(),
                IsCorrect = true,
                AnswerSession = answerSession
            };

           
            Assert.NotNull(answeredMnemonic);
            Assert.Equal(answerSession, answeredMnemonic.AnswerSession);
            Assert.True(answeredMnemonic.IsCorrect);
        }

        [Fact]
        public void AnsweredMnemonic_DefaultValues_AreAsExpected()
        {
            
            var answeredMnemonic = new AnsweredMnemonic
            {
                AnsweredMnemonicId = Guid.NewGuid(),
                AnswerSessionId = Guid.NewGuid(),
                MnemonicId = Guid.NewGuid(),
                IsCorrect = false,
                AnswerSession = new AnswerSession
                {
                    AnswerSessionId = Guid.NewGuid(),
                    UserName = "testuser"
                }
            };

            
            Assert.NotNull(answeredMnemonic.AnsweredMnemonicId);
            Assert.False(answeredMnemonic.IsCorrect);
        }

        [Fact]
        public void AnsweredMnemonic_ShouldTieToCorrectAnswerSession()
        {
          
            var answerSession1 = new AnswerSession
            {
                AnswerSessionId = Guid.NewGuid(),
                UserName = "testuser1"
            };

            var answerSession2 = new AnswerSession
            {
                AnswerSessionId = Guid.NewGuid(),
                UserName = "testuser2"
            };

            var answeredMnemonic = new AnsweredMnemonic
            {
                AnsweredMnemonicId = Guid.NewGuid(),
                AnswerSessionId = answerSession1.AnswerSessionId,
                MnemonicId = Guid.NewGuid(),
                IsCorrect = true,
                AnswerSession = answerSession1
            };

           
            answeredMnemonic.AnswerSession = answerSession2;

            
            Assert.Equal(answerSession2, answeredMnemonic.AnswerSession);
        }
    }
}
