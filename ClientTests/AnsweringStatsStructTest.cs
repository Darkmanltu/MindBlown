using Xunit;
using MindBlown.Types;

namespace MindBlown.Tests
{
    public class AnsweringStatsStructTests
    {
        [Fact]
        public void DefaultConstructor_ShouldInitializePrecisionToTwo()
        {
            var stats = new AnsweringStatsStruct();
            
            Assert.Equal(2, stats.precision);
            Assert.Equal(0, stats.correctAnswerCount);
            Assert.Equal(0, stats.allAnswerCount);
        }

        [Fact]
        public void Constructor_WithParameter_ShouldSetPrecisionCorrectly()
        {

            int expectedPrecision = 5;

            
            var stats = new AnsweringStatsStruct(expectedPrecision);

            
            Assert.Equal(expectedPrecision, stats.precision);
            Assert.Equal(0, stats.correctAnswerCount);
            Assert.Equal(0, stats.allAnswerCount);
        }

        [Fact]
        public void Properties_ShouldSetAndGetValuesCorrectly()
        {
            
            var stats = new AnsweringStatsStruct
            {
                correctAnswerCount = 10,
                allAnswerCount = 20
            };

            
            Assert.Equal(10, stats.correctAnswerCount);
            Assert.Equal(20, stats.allAnswerCount);
        }
    }
}