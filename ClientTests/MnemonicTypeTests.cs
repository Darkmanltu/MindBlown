using Xunit;
using MindBlown.Types;
using System;

namespace MindBlown.Tests
{
    public class MnemonicsTypeTests
    {
        [Fact]
        public void Constructor_ShouldInitializeWithDefaultValues()
        {

            var mnemonic = new MnemonicsType();

            
            Assert.Equal(Guid.Empty, mnemonic.Id);
            Assert.Null(mnemonic.HelperText);
            Assert.Null(mnemonic.MnemonicText);
            Assert.Equal(MnemonicCategory.Other, mnemonic.Category);
        }

        [Fact]
        public void Constructor_WithMnemonicText_ShouldSetMnemonicText()
        {
            
            var mnemonicText = "Sample Mnemonic";

            
            var mnemonic = new MnemonicsType(mnemonicText);

            
            Assert.Equal(mnemonicText, mnemonic.MnemonicText);
            Assert.Null(mnemonic.HelperText);
        }

        [Fact]
        public void Constructor_WithMnemonicTextAndHelperText_ShouldSetBothProperties()
        {
            
            var mnemonicText = "Sample Mnemonic";
            var helperText = "Helper Text";

            
            var mnemonic = new MnemonicsType(mnemonicText, helperText);

            
            Assert.Equal(mnemonicText, mnemonic.MnemonicText);
            Assert.Equal(helperText, mnemonic.HelperText);
        }

        [Fact]
        public void GetHashCode_ShouldBeBasedOnHelperTextAndMnemonicText()
        {
            
            var mnemonic1 = new MnemonicsType { HelperText = "Helper", MnemonicText = "Mnemonic" };
            var mnemonic2 = new MnemonicsType { HelperText = "Helper", MnemonicText = "Mnemonic" };

            
            var hash1 = mnemonic1.GetHashCode();
            var hash2 = mnemonic2.GetHashCode();

            
            Assert.Equal(hash1, hash2);
        }

        [Fact]
        public void Equals_WithSameMnemonicText_ShouldReturnTrue()
        {
            
            var mnemonic1 = new MnemonicsType { MnemonicText = "Mnemonic" };
            var mnemonic2 = new MnemonicsType { MnemonicText = "Mnemonic" };

            
            var isEqual = mnemonic1.Equals(mnemonic2);

            
            Assert.True(isEqual);
        }

        [Fact]
        public void Equals_WithDifferentMnemonicText_ShouldReturnFalse()
        {
            
            var mnemonic1 = new MnemonicsType { MnemonicText = "Mnemonic1" };
            var mnemonic2 = new MnemonicsType { MnemonicText = "Mnemonic2" };

            
            var isEqual = mnemonic1.Equals(mnemonic2);

            
            Assert.False(isEqual);
        }

        [Fact]
        public void Equals_WithNull_ShouldReturnFalse()
        {
            
            var mnemonic = new MnemonicsType { MnemonicText = "Mnemonic" };

            
            var isEqual = mnemonic.Equals(null);

            
            Assert.False(isEqual);
        }

        [Fact]
        public void Equals_WithDifferentType_ShouldReturnFalse()
        {
            
            var mnemonic = new MnemonicsType { MnemonicText = "Mnemonic" };
            var otherObject = new object();

            
            var isEqual = mnemonic.Equals(otherObject as MnemonicsType);

            
            Assert.False(isEqual);
        }

        [Fact]
        public void Equals_WithSameReference_ShouldReturnTrue()
        {
            
            var mnemonic = new MnemonicsType { MnemonicText = "Mnemonic" };

            
            var isEqual = mnemonic.Equals(mnemonic);

            
            Assert.True(isEqual);
        }
    }
}
