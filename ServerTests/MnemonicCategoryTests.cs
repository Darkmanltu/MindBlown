using MindBlown.Server.Models;
using System;
using Xunit;

namespace MindBlown.Tests
{
    public class MnemonicCategoryTests
    {
        [Fact]
        public void MnemonicCategory_Enum_ContainsAllExpectedValues()
        {
            
            var expectedValues = new[]
            {
                "Chemistry", "History", "Math", "Science", "Geography",
                "Physics", "Biology", "Astronomy", "Literature", "Language",
                "Art", "Music", "Technology", "Engineering", "Medicine",
                "Psychology", "Philosophy", "Sociology", "Economics", "Politics",
                "Law", "Business", "Accounting", "Marketing", "Education",
                "Architecture", "ComputerScience", "EnvironmentalScience", "Agriculture", "Sports",
                "Health", "Nutrition", "Anthropology", "Archaeology", "Theology",
                "Ethics", "Logic", "Linguistics", "Zoology", "Other"
            };

            
            var actualValues = Enum.GetNames(typeof(MnemonicCategory));

            
            Assert.Equal(expectedValues.Length, actualValues.Length);
            foreach (var value in expectedValues)
            {
                Assert.Contains(value, actualValues);
            }
        }
    }
}