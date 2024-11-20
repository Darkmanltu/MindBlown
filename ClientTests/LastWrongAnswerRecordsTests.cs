using Xunit;
using MindBlown.Types;
using System;

namespace MindBlown.Tests
{
    public class LastWrongAnswerRecordTests
    {
        [Fact]
        public void Constructor_ShouldInitializePropertiesCorrectly()
        {
            
            var id = Guid.NewGuid();
            string helperText = "Example helper";
            string mnemonicText = "Example mnemonic";
            string wrongTextMnemonic = "Example wrong mnemonic";
            MnemonicCategory category = MnemonicCategory.Chemistry;

            
            var record = new LastWrongAnswerRecord
            {
                Id = id,
                helperText = helperText,
                mnemonicText = mnemonicText,
                wrongTextMnemonic = wrongTextMnemonic,
                category = category
            };

            
            Assert.Equal(id, record.Id);
            Assert.Equal(helperText, record.helperText);
            Assert.Equal(mnemonicText, record.mnemonicText);
            Assert.Equal(wrongTextMnemonic, record.wrongTextMnemonic);
            Assert.Equal(category, record.category);
        }

        [Fact]
        public void DefaultId_ShouldBeEmptyGuid()
        {
            
            string helperText = "Example helper";
            string mnemonicText = "Example mnemonic";
            string wrongTextMnemonic = "Example wrong mnemonic";

            
            var record = new LastWrongAnswerRecord
            {
                helperText = helperText,
                mnemonicText = mnemonicText,
                wrongTextMnemonic = wrongTextMnemonic,
                category = null
            };

            
            Assert.Equal(Guid.Empty, record.Id);
            Assert.Equal(helperText, record.helperText);
            Assert.Equal(mnemonicText, record.mnemonicText);
            Assert.Equal(wrongTextMnemonic, record.wrongTextMnemonic);
            Assert.Null(record.category);
        }
    }
}
