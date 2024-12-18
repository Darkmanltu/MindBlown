using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MindBlown.Server.Data;
using MindBlown.Server.Models;
using Xunit;

namespace MindBlown.Tests
{
    public class DbContextTests
    {
        
        private AppDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) 
                .Options;

            return new AppDbContext(options);
        }

        [Fact]
        public void CanInsertAnswerSessionIntoDatabase()
        {
            using var context = GetInMemoryDbContext();
            var answerSession = new AnswerSession
            {
                AnswerSessionId = Guid.NewGuid(), 
                UserName = "TestUser"
            };

            context.AnswerSessions.Add(answerSession);
            context.SaveChanges();

            var result = context.AnswerSessions.FirstOrDefault(a => a.UserName == "TestUser");

            Assert.NotNull(result);
            Assert.Equal("TestUser", result.UserName);
        }

        [Fact]
        public void CanInsertAnsweredMnemonicIntoDatabase()
        {
            using var context = GetInMemoryDbContext();

            var answerSession = new AnswerSession
            {
                AnswerSessionId = Guid.NewGuid(),
                UserName = "TestUser"
            };

            var answeredMnemonic = new AnsweredMnemonic
            {
                AnsweredMnemonicId = Guid.NewGuid(),
                AnswerSessionId = answerSession.AnswerSessionId,
                AnswerSession = answerSession
            };

            context.AnswerSessions.Add(answerSession);
            context.AnsweredMnemonics.Add(answeredMnemonic);
            context.SaveChanges();

            var result = context.AnsweredMnemonics
                .Include(am => am.AnswerSession) // Include related AnswerSession
                .FirstOrDefault(am => am.AnsweredMnemonicId == answeredMnemonic.AnsweredMnemonicId);

            Assert.NotNull(result);
            Assert.NotNull(result.AnswerSession);
            Assert.Equal("TestUser", result.AnswerSession.UserName);
        }

        [Fact]
        public void CanEstablishOneToManyRelationshipBetweenAnswerSessionAndAnsweredMnemonics()
        {
            using var context = GetInMemoryDbContext();

            var sessionId = Guid.NewGuid();
            var answerSession = new AnswerSession
            {
                AnswerSessionId = sessionId,
                UserName = "SessionOwner"
            };

            var answeredMnemonic1 = new AnsweredMnemonic
            {
                AnsweredMnemonicId = Guid.NewGuid(),
                AnswerSessionId = sessionId,
                AnswerSession = answerSession
            };

            var answeredMnemonic2 = new AnsweredMnemonic
            {
                AnsweredMnemonicId = Guid.NewGuid(),
                AnswerSessionId = sessionId,
                AnswerSession = answerSession
            };

            context.AnswerSessions.Add(answerSession);
            context.AnsweredMnemonics.AddRange(answeredMnemonic1, answeredMnemonic2);
            context.SaveChanges();

            var result = context.AnswerSessions
                .Include(a => a.AnsweredMnemonics)
                .FirstOrDefault(a => a.AnswerSessionId == sessionId);

            Assert.NotNull(result);
            Assert.Equal(2, result.AnsweredMnemonics.Count);
        }

        [Fact]
        public void CascadeDelete_RemovesAnsweredMnemonics_WhenAnswerSessionDeleted()
        {
            using var context = GetInMemoryDbContext();

            var sessionId = Guid.NewGuid();
            var answerSession = new AnswerSession
            {
                AnswerSessionId = sessionId,
                UserName = "SessionOwner"
            };

            var answeredMnemonic = new AnsweredMnemonic
            {
                AnsweredMnemonicId = Guid.NewGuid(),
                AnswerSessionId = sessionId,
                AnswerSession = answerSession
            };

            context.AnswerSessions.Add(answerSession);
            context.AnsweredMnemonics.Add(answeredMnemonic);
            context.SaveChanges();

            
            Assert.Equal(1, context.AnswerSessions.Count());
            Assert.Equal(1, context.AnsweredMnemonics.Count());

          
            context.AnswerSessions.Remove(answerSession);
            context.SaveChanges();

           
            Assert.Equal(0, context.AnswerSessions.Count());
            Assert.Equal(0, context.AnsweredMnemonics.Count());
        }

        [Fact]
        public void CascadeDelete_DoesNotRemoveUnrelatedAnsweredMnemonics()
        {
            using var context = GetInMemoryDbContext();

            var sessionId1 = Guid.NewGuid();
            var sessionId2 = Guid.NewGuid();

            var answerSession1 = new AnswerSession
            {
                AnswerSessionId = sessionId1,
                UserName = "Session1"
            };

            var answerSession2 = new AnswerSession
            {
                AnswerSessionId = sessionId2,
                UserName = "Session2"
            };

            var answeredMnemonic1 = new AnsweredMnemonic
            {
                AnsweredMnemonicId = Guid.NewGuid(),
                AnswerSessionId = sessionId1,
                AnswerSession = answerSession1
            };

            var answeredMnemonic2 = new AnsweredMnemonic
            {
                AnsweredMnemonicId = Guid.NewGuid(),
                AnswerSessionId = sessionId2,
                AnswerSession = answerSession2
            };

            context.AnswerSessions.AddRange(answerSession1, answerSession2);
            context.AnsweredMnemonics.AddRange(answeredMnemonic1, answeredMnemonic2);
            context.SaveChanges();

    
            context.AnswerSessions.Remove(answerSession1);
            context.SaveChanges();

            
            Assert.Single(context.AnswerSessions);
            Assert.Single(context.AnsweredMnemonics);
            Assert.Equal(answeredMnemonic2.AnsweredMnemonicId, context.AnsweredMnemonics.First().AnsweredMnemonicId);
        }
    }
}
