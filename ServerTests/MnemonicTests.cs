using System;
using MindBlown.Server.Models;
using Xunit;

namespace MindBlown.Tests
{
    public class MnemonicTests
    {
        [Fact]
        public void Mnemonic_Initialization_WithDefaultValues()
        {
            
            var mnemonic = new Mnemonic();

            
            Assert.NotNull(mnemonic.Id); 
            Assert.Null(mnemonic.HelperText);
            Assert.Null(mnemonic.MnemonicText);
            Assert.Equal(MnemonicCategory.Other, mnemonic.Category); 
        }

        [Fact]
        public void Mnemonic_Properties_AreSetCorrectly()
        {
            
            var mnemonic = new Mnemonic
            {
                HelperText = "Helper",
                MnemonicText = "Text",
                Category = MnemonicCategory.Science
            };

            
            Assert.Equal("Helper", mnemonic.HelperText);
            Assert.Equal("Text", mnemonic.MnemonicText);
            Assert.Equal(MnemonicCategory.Science, mnemonic.Category);
        }

        [Fact]
        public void Mnemonic_Id_IsImmutable()
        {
            
            var id = Guid.NewGuid();
            var mnemonic = new Mnemonic { Id = id };

            
            Assert.Equal(id, mnemonic.Id);
        }
    }
}