using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MindBlown.Server.Data;
using MindBlown.Server.Models;
using Xunit;

namespace MindBlown.Tests
{
    public class AppDbContextTests
    {
        private DbContextOptions<AppDbContext> GetInMemoryOptions()
        {
            return new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
                .Options;
        }

        [Fact]
        public void Can_Add_And_Retrieve_Mnemonic()
        {
            
            var options = GetInMemoryOptions();
            var mnemonic = new Mnemonic
            {
                Id = Guid.NewGuid(),
                HelperText = "Test Helper",
                MnemonicText = "Test Mnemonic",
                Category = MnemonicCategory.Science
            };

            
            using (var context = new AppDbContext(options))
            {
                context.Mnemonics.Add(mnemonic);
                context.SaveChanges();
            }

            
            using (var context = new AppDbContext(options))
            {
                var retrievedMnemonic = context.Mnemonics.FirstOrDefault();
                Assert.NotNull(retrievedMnemonic);
                Assert.Equal(mnemonic.Id, retrievedMnemonic.Id);
                Assert.Equal("Test Helper", retrievedMnemonic.HelperText);
                Assert.Equal("Test Mnemonic", retrievedMnemonic.MnemonicText);
                Assert.Equal(MnemonicCategory.Science, retrievedMnemonic.Category);
            }
        }

        [Fact]
        public void Can_Update_Mnemonic()
        {
            
            var options = GetInMemoryOptions();
            var mnemonic = new Mnemonic
            {
                Id = Guid.NewGuid(),
                HelperText = "Old Helper",
                MnemonicText = "Old Mnemonic",
                Category = MnemonicCategory.Math
            };

            
            using (var context = new AppDbContext(options))
            {
                context.Mnemonics.Add(mnemonic);
                context.SaveChanges();
            }

            using (var context = new AppDbContext(options))
            {
                var toUpdate = context.Mnemonics.First();
                toUpdate.HelperText = "Updated Helper";
                toUpdate.MnemonicText = "Updated Mnemonic";
                context.SaveChanges();
            }

            
            using (var context = new AppDbContext(options))
            {
                var updatedMnemonic = context.Mnemonics.First();
                Assert.Equal("Updated Helper", updatedMnemonic.HelperText);
                Assert.Equal("Updated Mnemonic", updatedMnemonic.MnemonicText);
            }
        }

        [Fact]
        public void Can_Delete_Mnemonic()
        {
            
            var options = GetInMemoryOptions();
            var mnemonic = new Mnemonic
            {
                Id = Guid.NewGuid(),
                HelperText = "To Be Deleted",
                MnemonicText = "To Be Deleted",
                Category = MnemonicCategory.Geography
            };

            
            using (var context = new AppDbContext(options))
            {
                context.Mnemonics.Add(mnemonic);
                context.SaveChanges();
            }

            using (var context = new AppDbContext(options))
            {
                var toDelete = context.Mnemonics.First();
                context.Mnemonics.Remove(toDelete);
                context.SaveChanges();
            }

            
            using (var context = new AppDbContext(options))
            {
                Assert.Empty(context.Mnemonics);
            }
        }
    }
}
