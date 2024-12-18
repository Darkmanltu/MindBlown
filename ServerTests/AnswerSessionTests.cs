using System;
using System.Collections.Generic;
using Xunit;
using MindBlown.Server.Models;

namespace MindBlown.Tests.Models
{
    public class AnswerSessionTests
    {
        [Fact]
        public void AnswerSession_CreatesSuccessfully_WhenAllPropertiesAreSet()
        {
           
            var answerSession = new AnswerSession
            {
                AnswerSessionId = Guid.NewGuid(),
                UserName = "testuser",
                LastAnswerTime = DateTime.UtcNow,
                CorrectCount = 5,
                IncorrectCount = 3,
                AnsweredMnemonics = new List<AnsweredMnemonic>
                {
                    new AnsweredMnemonic
                    {
                        AnsweredMnemonicId = Guid.NewGuid(),
                        AnswerSessionId = Guid.NewGuid(),
                        MnemonicId = Guid.NewGuid(),
                        IsCorrect = true,
                        AnswerSession = null! 
                    }
                }
            };

           
            Assert.NotNull(answerSession);
            Assert.Equal("testuser", answerSession.UserName);
            Assert.Equal(5, answerSession.CorrectCount);
            Assert.Equal(3, answerSession.IncorrectCount);
            Assert.Single(answerSession.AnsweredMnemonics);
        }
        

        [Fact]
        public void AnswerSession_DefaultValues_AreAsExpected()
        {
            
            var answerSession = new AnswerSession
            {
                AnswerSessionId = Guid.NewGuid(),
                UserName = "testuser",
                LastAnswerTime = DateTime.MinValue 
            };

           
            Assert.NotNull(answerSession.AnsweredMnemonics);
            Assert.Empty(answerSession.AnsweredMnemonics);
            Assert.Equal(0, answerSession.CorrectCount);
            Assert.Equal(0, answerSession.IncorrectCount);
            Assert.Equal(DateTime.MinValue, answerSession.LastAnswerTime);
        }

        [Fact]
        public void AnswerSession_CanAddAnsweredMnemonics()
        {
          
            var answerSession = new AnswerSession
            {
                AnswerSessionId = Guid.NewGuid(),
                UserName = "testuser",
                LastAnswerTime = DateTime.UtcNow
            };

            var answeredMnemonic = new AnsweredMnemonic
            {
                AnsweredMnemonicId = Guid.NewGuid(),
                MnemonicId = Guid.NewGuid(),
                IsCorrect = true,
                AnswerSessionId = answerSession.AnswerSessionId,
                AnswerSession = answerSession
            };

           
            answerSession.AnsweredMnemonics.Add(answeredMnemonic);

           
            Assert.Single(answerSession.AnsweredMnemonics);
            Assert.Equal(answeredMnemonic, answerSession.AnsweredMnemonics.First());
            Assert.Equal(answerSession, answeredMnemonic.AnswerSession);
        }

        [Fact]
        public void AnswerSession_ShouldMaintainCorrectAndIncorrectCounts()
        {
           
            var answerSession = new AnswerSession
            {
                AnswerSessionId = Guid.NewGuid(),
                UserName = "testuser",
                LastAnswerTime = DateTime.UtcNow,
                CorrectCount = 5,
                IncorrectCount = 3
            };

        
            answerSession.CorrectCount += 1;  
            answerSession.IncorrectCount += 2; 

          
            Assert.Equal(6, answerSession.CorrectCount);
            Assert.Equal(5, answerSession.IncorrectCount);
        }

        [Fact]
        public void AnswerSession_ShouldUpdateLastAnswerTime()
        {
         
            var answerSession = new AnswerSession
            {
                AnswerSessionId = Guid.NewGuid(),
                UserName = "testuser",
                LastAnswerTime = DateTime.UtcNow
            };

            var newTime = DateTime.UtcNow.AddMinutes(5);
            
            answerSession.LastAnswerTime = newTime;

            
            Assert.Equal(newTime, answerSession.LastAnswerTime);
        }
    }
}
