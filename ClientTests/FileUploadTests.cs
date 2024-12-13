using System.Text.Json;
using System.IO;
using Microsoft.AspNetCore.Components.Forms;
using Moq;
using Services;
using MindBlown.Types;
using MindBlown.Pages;
using Xunit;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MindBlown.Interfaces;

public class FileUploadTests
{
    private readonly Mock<IMnemonicService> _mnemonicServiceMock;
    private readonly FileUpload _fileUpload;
    private readonly Mock<NavigationManager> _navigationMock;

    public FileUploadTests()
    {
        _mnemonicServiceMock = new Mock<IMnemonicService>();
        _navigationMock = new Mock<NavigationManager>(MockBehavior.Strict);

        _fileUpload = new FileUpload
        {
            MnemonicService = _mnemonicServiceMock.Object,
            Navigation = _navigationMock.Object
        };
    }

    [Fact]
    public void HandleFileSelected_Should_SetFileInfo()
    {
        
        var fileMock = new Mock<IBrowserFile>();
        var inputFileChangeEventArgs = new InputFileChangeEventArgs(new[] { fileMock.Object });

        
        _fileUpload.HandleFileSelected(inputFileChangeEventArgs);

        
        Assert.Equal(fileMock.Object, _fileUpload.fileInfo);
        Assert.Equal(string.Empty, _fileUpload.message);
    }

    [Fact]
    public void ReturnToSetup_Should_NavigateToAddMnemonicPage()
    {
        
        var testNavigationManager = new TestNavigationManager();
        _fileUpload.Navigation = testNavigationManager;

        
        _fileUpload.ReturnToSetup();

        
        Assert.Equal("/addMnemonic", testNavigationManager.NavigatedTo);
    }
    
    [Fact]
    public async Task UploadFile_Should_ProcessValidJsonAndCallService()
    {
        
        var mnemonicJson = JsonSerializer.Serialize(new List<MnemonicsType>
        {
            new MnemonicsType { HelperText = "Test", MnemonicText = "T", Category = MnemonicCategory.Other }
        });

        var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(mnemonicJson));
        var fileMock = new Mock<IBrowserFile>();
        fileMock
            .Setup(f => f.OpenReadStream(It.IsAny<long>(), default))
            .Returns(() => stream);
        
        _fileUpload.fileInfo = fileMock.Object;

        var existingMnemonics = new List<MnemonicsType>(); // Simulate no existing mnemonics
        _mnemonicServiceMock
            .Setup(service => service.GetMnemonicsAsync())
            .ReturnsAsync(existingMnemonics);

        _mnemonicServiceMock
            .Setup(service => service.CreateMnemonicAsync(It.IsAny<MnemonicsType>()))
            .ReturnsAsync((MnemonicsType mnemonic) => mnemonic);

        _mnemonicServiceMock
            .Setup(service => service.LogErrorToServerAsync(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        
        await _fileUpload.UploadFile();

        
        _mnemonicServiceMock.Verify(service => service.CreateMnemonicAsync(It.Is<MnemonicsType>(m => m.HelperText == "Test")), Times.Once);
        Assert.Equal("File uploaded and mnemonics unboxed successfully!", _fileUpload.message);
    }
    [Fact]
    public async Task UploadFile_Should_HandleInvalidJsonGracefully()
    {
        
        var invalidJson = "{ invalid json }";
        var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(invalidJson));
        var fileMock = new Mock<IBrowserFile>();
        fileMock
            .Setup(f => f.OpenReadStream(It.IsAny<long>(), default))
            .Returns(() => stream);

        _fileUpload.fileInfo = fileMock.Object;

        
        await _fileUpload.UploadFile();

        
        Assert.Equal("Invalid JSON format.", _fileUpload.message);
    }

    [Fact]
    public async Task UploadFile_Should_NotDuplicateMnemonics()
    {
        // Arrange
        var existingMnemonics = new List<MnemonicsType>
        {
            new MnemonicsType { HelperText = "Duplicate", MnemonicText = "D", Category = MnemonicCategory.Other }
        };

        var mnemonicJson = JsonSerializer.Serialize(new List<MnemonicsType>
        {
            new MnemonicsType { HelperText = "Duplicate", MnemonicText = "D", Category = MnemonicCategory.Other }
        });

        var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(mnemonicJson));
        var fileMock = new Mock<IBrowserFile>();
        fileMock
            .Setup(f => f.OpenReadStream(It.IsAny<long>(), default))
            .Returns(() => stream);
        _fileUpload.fileInfo = fileMock.Object;

        _mnemonicServiceMock
            .Setup(service => service.GetMnemonicsAsync())
            .ReturnsAsync(existingMnemonics);

        
        await _fileUpload.UploadFile();


        _mnemonicServiceMock.Verify(service => service.CreateMnemonicAsync(It.IsAny<MnemonicsType>()), Times.Never);
    }
}

public class TestNavigationManager : NavigationManager
{
    public string? NavigatedTo { get; private set; }

    public TestNavigationManager()
    {
        Initialize("http://localhost/", "http://localhost/");
    }

    protected override void NavigateToCore(string uri, bool forceLoad)
    {
        NavigatedTo = uri; // Capture the navigation URI for assertion
    }
}