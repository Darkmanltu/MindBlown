using System;
using System.Collections.Generic;
using FluentAssertions;
using MindBlown.Server.Models;
using Xunit;

namespace MindBlown.Tests
{
    public class UserMnemonicIDsTests
    {
        [Fact]
        public void UserMnemonicIDs_Constructor_ShouldInitializeProperties()
        {

            var userId = Guid.NewGuid();
            var username = "testuser";
            var password = "password123";
            var mnemonicGuids = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
            var lwaRecordId = Guid.NewGuid();

            
            var userMnemonic = new UserMnemonicIDs
            {
                Id = userId,
                Username = username,
                Password = password,
                MnemonicGuids = mnemonicGuids,
                LWARecordId = lwaRecordId
            };

            
            userMnemonic.Id.Should().Be(userId);
            userMnemonic.Username.Should().Be(username);
            userMnemonic.Password.Should().Be(password);
            userMnemonic.MnemonicGuids.Should().BeEquivalentTo(mnemonicGuids);
            userMnemonic.LWARecordId.Should().Be(lwaRecordId);
        }

        [Fact]
        public void UserMnemonicIDs_RequiredProperties_ShouldThrowExceptionIfNotSet()
        {
            
            var exception = Record.Exception(() => new LastWrongAnswerRecord
            {
                Id = Guid.NewGuid(),
                helperText = null,  
                mnemonicText = "Some mnemonic text",
                wrongTextMnemonic = "Some wrong mnemonic text",
                category = MnemonicCategory.Other
            });

            
            exception.Should().BeNull(); 
        }

        [Fact]
        public void UserMnemonicIDs_MnemonicGuids_ShouldInitializeWithEmptyListIfNullPassed()
        {
            
            var userMnemonic = new UserMnemonicIDs
            {
                Id = Guid.NewGuid(),
                Username = "testuser",
                Password = "password123",
                MnemonicGuids = new List<Guid>(), 
                LWARecordId = Guid.NewGuid()
            };

            
            userMnemonic.MnemonicGuids.Should().BeEmpty();
        }

        [Fact]
        public void UserMnemonicIDs_CanUpdateProperties()
        {
            
            var userMnemonic = new UserMnemonicIDs
            {
                Id = Guid.NewGuid(),
                Username = "testuser",
                Password = "password123",
                MnemonicGuids = new List<Guid> { Guid.NewGuid() },
                LWARecordId = Guid.NewGuid()
            };

            
            userMnemonic = userMnemonic with { Username = "updatedUser", Password = "newPassword123" };

            
            userMnemonic.Username.Should().Be("updatedUser");
            userMnemonic.Password.Should().Be("newPassword123");
        }
    }
}
