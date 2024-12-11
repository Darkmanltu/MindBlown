using System;
using FluentAssertions;
using MindBlown.Server.Models;
using Xunit;

namespace MindBlown.Tests
{
    public class LastWrongAnswerRecordTests
    {
        [Fact]
        public void LastWrongAnswerRecord_Constructor_ShouldInitializeProperties()
        {

            var id = Guid.NewGuid();
            var helperText = "This is helper text";
            var mnemonicText = "Mnemonic text example";
            var wrongTextMnemonic = "Wrong text example";
            var category = MnemonicCategory.Other;


            var record = new LastWrongAnswerRecord
            {
                Id = id,
                helperText = helperText,
                mnemonicText = mnemonicText,
                wrongTextMnemonic = wrongTextMnemonic,
                category = category
            };


            record.Id.Should().Be(id);
            record.helperText.Should().Be(helperText);
            record.mnemonicText.Should().Be(mnemonicText);
            record.wrongTextMnemonic.Should().Be(wrongTextMnemonic);
            record.category.Should().Be(MnemonicCategory.Other);
        }

        [Fact]
        public void LastWrongAnswerRecord_RequiredProperties_ShouldThrowExceptionIfNotSet()
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
        public void LastWrongAnswerRecord_OptionalCategory_ShouldBeNullIfNotSet()
        {

            var record = new LastWrongAnswerRecord
            {
                Id = Guid.NewGuid(),
                helperText = "Helper text",
                mnemonicText = "Mnemonic text",
                wrongTextMnemonic = "Wrong mnemonic text",
                category = null 
            };


            record.category.Should().BeNull();
        }

        [Fact]
        public void LastWrongAnswerRecord_CanUpdateProperties()
        {
            var record = new LastWrongAnswerRecord
            {
                Id = Guid.NewGuid(),
                helperText = "Helper text",
                mnemonicText = "Mnemonic text",
                wrongTextMnemonic = "Wrong mnemonic text",
                category = MnemonicCategory.Other
            };


            record = record with
            {
                helperText = "Updated helper text",
                mnemonicText = "Updated mnemonic text"
            };


            record.helperText.Should().Be("Updated helper text");
            record.mnemonicText.Should().Be("Updated mnemonic text");
        }
    }
}
