using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Web;
using MindBlown.Pages;
using MindBlown.Types;
using MindBlown.Interfaces; 
using Xunit;

public class TestMnemonicTests : TestMnemonic
{
    private Mock<IMnemonicService> _mnemonicServiceMock; 
    private TestMnemonic _testMnemonic;

    public TestMnemonicTests()
    {
        _mnemonicServiceMock = new Mock<IMnemonicService>();
        _testMnemonic = new TestMnemonic
        {
            MnemonicService = _mnemonicServiceMock.Object 
        };
    }

    [Fact]
    public async Task OnInitializedAsync_ShouldNotBlock_WhenTestingMnemonicIsSet()
    {

        var mnemonicList = new List<MnemonicsType> 
        { 
            new MnemonicsType { HelperText = "Test", MnemonicText = "TestMnemonic" } 
        };
        _mnemonicServiceMock.Setup(s => s.GetMnemonicsAsync()).ReturnsAsync(mnemonicList); 

        
        var methodInfo = typeof(TestMnemonic).GetMethod("OnInitializedAsync", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var initializedTask = (Task)methodInfo.Invoke(_testMnemonic, null);

        await Task.Delay(100); 

        
        _testMnemonic.testingMnemonic = mnemonicList.First();
        await initializedTask; 

        
        Assert.NotNull(_testMnemonic.testingMnemonic);
    }

    [Fact]
    public async Task OnAfterRenderAsync_ShouldLoadMnemonicsAndPickRandomMnemonic()
    {
        
        var mnemonicList = new List<MnemonicsType>
        {
            new MnemonicsType { Id = Guid.NewGuid(), HelperText = "Test1", MnemonicText = "TestMnemonic1" },
            new MnemonicsType { Id = Guid.NewGuid(), HelperText = "Test2", MnemonicText = "TestMnemonic2" }
        };
        _mnemonicServiceMock.Setup(s => s.GetMnemonicsAsync()).ReturnsAsync(mnemonicList); 
        _testMnemonic.testingMnemonic = null; 

        
        var methodInfo = typeof(TestMnemonic).GetMethod("OnAfterRenderAsync", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        await (Task)methodInfo.Invoke(_testMnemonic, new object[] { true });

        
        Assert.NotNull(_testMnemonic.testingMnemonic);
        Assert.Contains(mnemonicList, mnemonic => mnemonic.Equals(_testMnemonic.testingMnemonic));
    }
    
    // public async Task CheckMnemonic_ShouldUpdateAnsweringStats_WhenAnswerIsCorrect()
    // {
    //     // Arrange
    //     var correctMnemonic = new MnemonicsType { HelperText = "Test", MnemonicText = "TestMnemonic" };
    //     _testMnemonic.testingMnemonic = correctMnemonic; // Set the correct mnemonic for testing
    //     _testMnemonic.userGivenMnemonicText = "TestMnemonic"; // User provides correct text
    //
    //     // Act
    //     await _testMnemonic.CheckMnemonic();
    //
    //     // Assert: Verify that the correct answer count is incremented
    //     Assert.Equal(1, _testMnemonic.answeringStats.correctAnswerCount);
    //     Assert.Equal(1, _testMnemonic.answeringStats.allAnswerCount);
    // }

    // [Fact]
    // public async Task CheckMnemonic_ShouldRecordLastWrongAnswer_WhenAnswerIsIncorrect()
    // {
    //     // Arrange
    //     var correctMnemonic = new MnemonicsType { HelperText = "Test", MnemonicText = "TestMnemonic" };
    //     _testMnemonic.testingMnemonic = correctMnemonic; // Set the correct mnemonic for testing
    //     _testMnemonic.userGivenMnemonicText = "WrongAnswer"; // User provides incorrect text
    //
    //     // Act
    //     await _testMnemonic.CheckMnemonic();
    //
    //     // Assert: Verify that the wrong answer is recorded
    //     Assert.NotNull(_testMnemonic.lastWrongAnswer);
    //     Assert.Equal("WrongAnswer", _testMnemonic.lastWrongAnswer.wrongTextMnemonic);
    // }

    [Fact]
    public void getRandomMnemonic_ShouldReturnRandomMnemonic()
    {
        
        var mnemonicList = new List<MnemonicsType>
        {
            new MnemonicsType { Id = Guid.NewGuid(), HelperText = "Test1", MnemonicText = "TestMnemonic1" },
            new MnemonicsType { Id = Guid.NewGuid(), HelperText = "Test2", MnemonicText = "TestMnemonic2" }
        };
        _testMnemonic.mnemonicsList = new Repository<MnemonicsType>(mnemonicList); // Setup the list of mnemonics

        
        var randomMnemonic = _testMnemonic.getRandomMnemonic();

        
        Assert.Contains(randomMnemonic, mnemonicList);
    }

    // [Fact]
    // public async Task Enter_ShouldCallCheckMnemonic_WhenEnterKeyIsPressed()
    // {
    //     // Arrange
    //     var mnemonicList = new List<MnemonicsType>
    //     {
    //         new MnemonicsType { Id = Guid.NewGuid(), HelperText = "Test", MnemonicText = "TestMnemonic" }
    //     };
    //     _mnemonicServiceMock.Setup(s => s.GetMnemonicsAsync()).ReturnsAsync(mnemonicList); // Setup mock method
    //     _testMnemonic.testingMnemonic = mnemonicList.First(); // Set testingMnemonic for the test
    //
    //     // Act: Use reflection to call the protected method OnInitializedAsync
    //     var methodInfo = typeof(TestMnemonic).GetMethod("OnInitializedAsync", BindingFlags.NonPublic | BindingFlags.Instance);
    //     var initializedTask = (Task)methodInfo.Invoke(_testMnemonic, null);
    //
    //     await Task.Delay(100); // Let the component initialize
    //
    //     // Set testingMnemonic
    //     _testMnemonic.testingMnemonic = mnemonicList.First();
    //     await initializedTask; // Ensure the task completes
    //
    //     // Act: Use reflection to call the protected method OnAfterRenderAsync
    //     methodInfo = typeof(TestMnemonic).GetMethod("OnAfterRenderAsync", BindingFlags.NonPublic | BindingFlags.Instance);
    //     await (Task)methodInfo.Invoke(_testMnemonic, new object[] { true });  // Trigger OnAfterRenderAsync
    //
    //     // Act: Simulate pressing the Enter key
    //     var keyboardEventArgs = new KeyboardEventArgs { Key = "Enter" };
    //     await _testMnemonic.Enter(keyboardEventArgs);
    //
    //     // Assert: Ensure that CheckMnemonic is called (i.e., allAnswerCount is incremented)
    //     Assert.Equal(1, _testMnemonic.answeringStats.allAnswerCount); // This means CheckMnemonic was called
    // }
}
