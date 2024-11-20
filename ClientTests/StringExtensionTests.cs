public class StringExtensionsTests
{
    [Fact]
    public void CapitalizeWords_WithLowerCaseWords_ReturnsCapitalizedWords()
    {
        
        var input = "hello world";
        var expected = "Hello World";

        
        var result = input.CapitalizeWords();

        
        Assert.Equal(expected, result);
    }

    [Fact]
    public void CapitalizeWords_WithMixedCaseWords_ReturnsCapitalizedWords()
    {
        
        var input = "heLLo wOrLd";
        var expected = "HeLLo WOrLd";

        
        var result = input.CapitalizeWords();

        
        Assert.Equal(expected, result);
    }

    [Fact]
    public void CapitalizeWords_WithEmptyString_ReturnsEmptyString()
    {
        
        var input = "";
        var expected = "";

        
        var result = input.CapitalizeWords();

        
        Assert.Equal(expected, result);
    }

    [Fact]
    public void CapitalizeWords_WithNullString_ReturnsNull()
    {
        
        string input = null;

        
        var result = input.CapitalizeWords();

        
        Assert.Null(result);
    }

    [Fact]
    public void CapitalizeWords_WithSingleCharacterWord_ReturnsCapitalizedCharacter()
    {
        
        var input = "a";
        var expected = "A";

        
        var result = input.CapitalizeWords();

        
        Assert.Equal(expected, result);
    }

    [Fact]
    public void CapitalizeWords_WithMultipleSpaces_ReturnsCorrectlyCapitalizedWords()
    {
        
        var input = "  multiple   spaces between words ";
        var expected = "  Multiple   Spaces Between Words ";

        
        var result = input.CapitalizeWords();

        
        Assert.Equal(expected, result);
    }
}