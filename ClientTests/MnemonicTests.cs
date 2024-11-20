using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MindBlown;
using MindBlown.Interfaces;
using MindBlown.Types;
using MindBlown.Exceptions;
using MindBlown.Pages;
using Xunit;
using Xunit.Abstractions;

namespace MindBlown.Tests
{
    public class MnemonicsTests : Mnemonics
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly Mock<IMnemonicService> _mnemonicServiceMock;
        private readonly Mock<ILoggingService> _loggingServiceMock;
        private readonly Mock<IActiveUserClient> _activeUserClientMock;


        public MnemonicsTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            _mnemonicServiceMock = new Mock<IMnemonicService>();
            _loggingServiceMock = new Mock<ILoggingService>();
            _activeUserClientMock = new Mock<IActiveUserClient>();
         }

        [Fact]
        public async Task OnSubmit_ShouldAddNewMnemonic_WhenNotExists()
        {
            
            var model = new MnemonicsType
            {
                HelperText = "Test Helper",
                MnemonicText = "Test Mnemonic",
                Category = MnemonicCategory.Other
            };
        
            var existingMnemonics = new List<MnemonicsType>();
            _mnemonicServiceMock.Setup(s => s.GetMnemonicsAsync()).ReturnsAsync(existingMnemonics);
            _mnemonicServiceMock.Setup(s => s.CreateMnemonicAsync(It.IsAny<MnemonicsType>())).ReturnsAsync(model);
            
            var mnemonics = new Mnemonics
            {
                MnemonicService = _mnemonicServiceMock.Object,
                LoggingService = _loggingServiceMock.Object,
                ActiveUserClient = _activeUserClientMock.Object,
                Model = model
            };

            
            await mnemonics.OnSubmit();

            
            Assert.Contains(mnemonics.mnemonicsList, m => m.HelperText == model.HelperText);
        }
        

        [Fact]
        public async Task LoadMnemonics_ShouldLoadDataFromService()
        {
            
            var mockData = new List<MnemonicsType>
            {
                new MnemonicsType { Id = Guid.NewGuid(), HelperText = "Helper", MnemonicText = "Mnemonic", Category = MnemonicCategory.Other }
            };
            _mnemonicServiceMock.Setup(s => s.GetMnemonicsAsync()).ReturnsAsync(mockData);

            var mnemonics = new Mnemonics
            {
                MnemonicService = _mnemonicServiceMock.Object,
                LoggingService = _loggingServiceMock.Object,
                ActiveUserClient = _activeUserClientMock.Object,

            };

            
            await mnemonics.LoadMnemonics();

            
            Assert.Equal(mockData.Count, mnemonics.mnemonicsList.Count);
            Assert.True(mnemonics.showMnemonics);
        }
        
         [Fact]
        public async Task LoadMnemonics_ShouldLoadEmptyList_WhenNoDataInService()
        {
            
            _mnemonicServiceMock.Setup(s => s.GetMnemonicsAsync()).ReturnsAsync(new List<MnemonicsType>());

            var mnemonics = new Mnemonics
            {
                MnemonicService = _mnemonicServiceMock.Object,
                LoggingService = _loggingServiceMock.Object,
                ActiveUserClient = _activeUserClientMock.Object
            };

            
            await mnemonics.LoadMnemonics();

            
            Assert.Empty(mnemonics.mnemonicsList);
            Assert.True(mnemonics.showMnemonics);  
        }
        
        // [Fact]
        // public async Task OnSubmit_ShouldShowError_WhenMnemonicAlreadyExists()
        // {
        //     // Arrange: Adding a duplicate mnemonic
        //     var model = new MnemonicsType
        //     {
        //         HelperText = "Test Helper",
        //         MnemonicText = "Test Mnemonic",
        //         Category = MnemonicCategory.Other
        //     };
        //
        //     var existingMnemonics = new List<MnemonicsType>
        //     {
        //         new MnemonicsType { HelperText = "Test Helper" }
        //     };
        //     _mnemonicServiceMock.Setup(s => s.GetMnemonicsAsync()).ReturnsAsync(existingMnemonics);
        //
        //     var mnemonics = new Mnemonics
        //     {
        //         MnemonicService = _mnemonicServiceMock.Object,
        //         LoggingService = _loggingServiceMock.Object,
        //         ActiveUserClient = _activeUserClientMock.Object,
        //         Model = model
        //     };
        //
        //     // Act: Attempting to add a duplicate
        //     await mnemonics.OnSubmit();
        //
        //     // Assert: Error message should be shown and logged
        //     _loggingServiceMock.Verify(l => l.LogAsync(It.IsAny<LogEntry>()), Times.Once);
        //     Assert.True(mnemonics.errorMessageIsVisible);
        //     Assert.Equal("Mnemonic already exists.", mnemonics.errorMessage);
        // }
        //
        // [Fact]
        // public async Task RemoveMnemonic_ShouldRemoveMnemonic_WhenExists()
        // {
        //     // Arrange
        //     var mnemonicId = Guid.NewGuid();
        //     var existingMnemonics = new List<MnemonicsType>
        //     {
        //         new MnemonicsType { Id = mnemonicId, HelperText = "Helper", MnemonicText = "Mnemonic", Category = MnemonicCategory.Other }
        //     };
        //     _mnemonicServiceMock.Setup(s => s.GetMnemonicsAsync()).ReturnsAsync(existingMnemonics);
        //     _mnemonicServiceMock.Setup(s => s.DeleteMnemonicAsync(It.IsAny<Guid>())).Returns(Task.CompletedTask);
        //
        //     var mnemonics = new Mnemonics
        //     {
        //         MnemonicService = _mnemonicServiceMock.Object,
        //         LoggingService = _loggingServiceMock.Object,
        //         ActiveUserClient = _activeUserClientMock.Object
        //     };
        //
        //     // Act: Remove the mnemonic
        //     await mnemonics.RemoveMnemonic(mnemonicId);
        //
        //     // Assert: Mnemonic should be removed from the list
        //     Assert.DoesNotContain(mnemonics.mnemonicsList, m => m.Id == mnemonicId);
        //     _loggingServiceMock.Verify(l => l.LogAsync(It.IsAny<LogEntry>()), Times.Once);
        // }

       
        
    }
}
