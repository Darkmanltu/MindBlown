using Xunit;
using Moq;
using System;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using MindBlown.Interfaces;
using MindBlown.Types;
using System.Collections.Generic;
using System.Reflection;
using MindBlown.Pages;
using MindBlown.Services;
using System.Linq;

public class TestMnemonicTests
{
    private readonly Mock<IActiveUserClient> _mockActiveUserClient = new Mock<IActiveUserClient>();
    private readonly Mock<IMnemonicService> _mockMnemonicService = new Mock<IMnemonicService>();
    private readonly Mock<IAuthService> _mockAuthService = new Mock<IAuthService>();
    private readonly Mock<ILWARecordService> _mockLWARecordService = new Mock<ILWARecordService>();
    private readonly Mock<IJSRuntime> _mockJSRuntime = new Mock<IJSRuntime>();
    private readonly Mock<IAnswerStatService> _mockAnswerStatService = new Mock<IAnswerStatService>();

    private readonly TestMnemonic _testMnemonic;
    
    
    public TestMnemonicTests()
    {
        _testMnemonic = new TestMnemonic
        {
            ActiveUserClient = _mockActiveUserClient.Object,
            MnemonicService = _mockMnemonicService.Object,
            AuthService = _mockAuthService.Object,
            LWARecordService = _mockLWARecordService.Object,
            JS = _mockJSRuntime.Object,
            AnswerStatService = _mockAnswerStatService.Object,
            displayingStat = new List<AnswerSessionType>(),
            answerSessionType = new AnswerSessionType() {UserName = "testUser"}
        };
    }

    [Fact]
    public async Task OnInitializedAsync_ShouldInitializeProperties()
    {
       
        var testMnemonic = new MnemonicsType { Id = Guid.NewGuid(), MnemonicText = "Test Mnemonic" };

        _mockAuthService.Setup(x => x.GetUsername()).ReturnsAsync("TestUser");
        _mockJSRuntime.Setup(x => x.InvokeAsync<Guid>("sessionStorage.getItem", It.IsAny<object[]>()))
            .ReturnsAsync(Guid.NewGuid());
        _mockActiveUserClient.Setup(x => x.IsSessionIdUniqueAsync(It.IsAny<Guid>()))
            .ReturnsAsync(true);

   
        _testMnemonic.testingMnemonic = testMnemonic;

     
        var onInitializedAsyncMethod = typeof(TestMnemonic).GetMethod("OnInitializedAsync", BindingFlags.Instance | BindingFlags.NonPublic);

        if (onInitializedAsyncMethod == null)
            throw new InvalidOperationException("OnInitializedAsync method not found.");

       
        var task = (Task?)onInitializedAsyncMethod.Invoke(_testMnemonic, null);
        if (task != null)
            await task;

        
        Assert.NotEqual(Guid.Empty, _testMnemonic.userId);
        _mockActiveUserClient.Verify(x => x.AddUserAsync(It.IsAny<Guid>()), Times.Once);
    }

    [Fact]
    public async Task LoadMnemonics_ShouldLoadMnemonicsFromService()
    {
       
        var mnemonics = new List<MnemonicsType>
        {
            new MnemonicsType { Id = Guid.NewGuid(), MnemonicText = "Mnemonic1" },
            new MnemonicsType { Id = Guid.NewGuid(), MnemonicText = "Mnemonic2" }
        };

        _mockMnemonicService.Setup(x => x.GetMnemonicsAsync()).ReturnsAsync(mnemonics);

       
        await _testMnemonic.LoadMnemonics();

      
        Assert.Equal(mnemonics.Count, _testMnemonic.mnemonicsList.Count());
    }

    [Fact]
    public void GetRandomMnemonic_ShouldReturnRandomMnemonic()
    {
       
        var mnemonics = new List<MnemonicsType>
        {
            new MnemonicsType { Id = Guid.NewGuid(), MnemonicText = "Mnemonic1" },
            new MnemonicsType { Id = Guid.NewGuid(), MnemonicText = "Mnemonic2" }
        };

        _testMnemonic.mnemonicsList = new Repository<MnemonicsType>(mnemonics);

      
        var randomMnemonic = _testMnemonic.getRandomMnemonic();

      
        Assert.Contains(randomMnemonic, mnemonics);
    }
    

    [Fact]
    public void ToggleDropdown_ShouldToggleVisibility()
    {
        _testMnemonic.ToggleDropdown();

   
        Assert.True(_testMnemonic.isDropdownVisible);
    }
    
    [Fact]
    public void GetRandomMnemonic_ShouldReturnValidMnemonicFromList()
    {
      
        var mnemonics = new List<MnemonicsType>
        {
            new MnemonicsType { Id = Guid.NewGuid(), MnemonicText = "Mnemonic1" },
            new MnemonicsType { Id = Guid.NewGuid(), MnemonicText = "Mnemonic2" }
        };
        _testMnemonic.mnemonicsList = new Repository<MnemonicsType>(mnemonics);

       
        var randomMnemonic = _testMnemonic.getRandomMnemonic();

   
        Assert.Contains(randomMnemonic, mnemonics);
        Assert.Equal(randomMnemonic, _testMnemonic.testingMnemonic);
    }
    
    [Fact]
    public void ToggleDropdown_ShouldToggleVisibilityCorrectly()
    {
     
        _testMnemonic.isDropdownVisible = false;

      
        _testMnemonic.ToggleDropdown();

        Assert.True(_testMnemonic.isDropdownVisible);

    
        _testMnemonic.ToggleDropdown();

        Assert.False(_testMnemonic.isDropdownVisible);
    }
    
    

}
