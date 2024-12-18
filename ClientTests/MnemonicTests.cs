// MnemonicsTests.cs

using MindBlown.Pages;
using MindBlown.Types;
using MindBlown.Interfaces;
using MindBlown.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Moq;
using Xunit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MindBlown.Tests
{
    public class MnemonicsTests
    {
        private readonly Mock<IMnemonicService> _mnemonicServiceMock;
        private readonly Mock<ILoggingService> _loggingServiceMock;
        private readonly Mock<IActiveUserClient> _activeUserClientMock;
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly Mock<IJSRuntime> _jsRuntimeMock;

        private readonly Mnemonics _mnemonicsPage;

        public MnemonicsTests()
        {
            _mnemonicServiceMock = new Mock<IMnemonicService>();
            _loggingServiceMock = new Mock<ILoggingService>();
            _activeUserClientMock = new Mock<IActiveUserClient>();
            _authServiceMock = new Mock<IAuthService>();
            _jsRuntimeMock = new Mock<IJSRuntime>();

            _mnemonicsPage = new Mnemonics
            {
                MnemonicService = _mnemonicServiceMock.Object,
                LoggingService = _loggingServiceMock.Object,
                ActiveUserClient = _activeUserClientMock.Object,
                AuthService = _authServiceMock.Object,
                JS = _jsRuntimeMock.Object
            };
        }

        [Fact]
        public async Task LoadMnemonics_LoadsMnemonicsForUser()
        {
            
            _authServiceMock.Setup(auth => auth.IsUserLoggedInAsync()).ReturnsAsync(true);
            _authServiceMock.Setup(auth => auth.GetUsername()).ReturnsAsync("testUser");
            _authServiceMock.Setup(auth => auth.GetMnemonicsGuids("testUser"))
                .ReturnsAsync(new List<Guid> { Guid.NewGuid() });

            _mnemonicServiceMock.Setup(service => service.GetMnemonicsByIdsAsync(It.IsAny<List<Guid>>()))
                .ReturnsAsync(new List<MnemonicsType>
                    { new MnemonicsType { Id = Guid.NewGuid(), HelperText = "Test", MnemonicText = "Test Mnemonic" } });

            
            await _mnemonicsPage.LoadMnemonics();

            
            Assert.True(_mnemonicsPage.showMnemonics);
            Assert.Single(_mnemonicsPage.mnemonicsList);
        }

        [Fact]
        public async Task RemoveMnemonic_RemovesMnemonicFromList()
        {
            
            var mnemonicId = Guid.NewGuid();
            _mnemonicServiceMock.Setup(service => service.GetMnemonicsAsync())
                .ReturnsAsync(new List<MnemonicsType> { new MnemonicsType { Id = mnemonicId } });

            _mnemonicServiceMock.Setup(service => service.GetMnemonicAsync(mnemonicId))
                .ReturnsAsync(new MnemonicsType { Id = mnemonicId });

            _authServiceMock.Setup(auth => auth.GetUsername()).ReturnsAsync("testUser");

            
            await _mnemonicsPage.RemoveMnemonic(mnemonicId);

          
            _mnemonicServiceMock.Verify(service => service.DeleteMnemonicAsync(mnemonicId), Times.Once);
            Assert.Empty(_mnemonicsPage.mnemonicsList);
        }

        [Fact]
        public async Task LoadMnemonics_ShouldLoadMnemonics_ForLoggedInUser()
        {
            
            var userGuid = Guid.NewGuid();
            var userMnemonics = new List<MnemonicsType>
            {
                new MnemonicsType { Id = Guid.NewGuid(), HelperText = "Test Helper", MnemonicText = "Test Mnemonic" }
            };

            
            _authServiceMock.Setup(auth => auth.IsUserLoggedInAsync()).ReturnsAsync(true);
            _authServiceMock.Setup(auth => auth.GetUsername()).ReturnsAsync("testUser");
            _authServiceMock.Setup(auth => auth.GetMnemonicsGuids("testUser"))
                .ReturnsAsync(new List<Guid> { userGuid });

            
            _mnemonicServiceMock.Setup(service => service.GetMnemonicsByIdsAsync(It.IsAny<List<Guid>>()))
                .ReturnsAsync(userMnemonics);

            
            await _mnemonicsPage.LoadMnemonics();

           
            Assert.True(_mnemonicsPage.showMnemonics); 
            Assert.Single(_mnemonicsPage.mnemonicsList); 
            Assert.Equal("Test Helper",
                _mnemonicsPage.mnemonicsList[0].HelperText);
        }

        [Fact]
        public async Task RemoveMnemonic_ShouldRemoveMnemonic_WhenValidMnemonicIdIsProvided()
        {
            
            var mnemonicId = Guid.NewGuid();
            var userGuid = Guid.NewGuid();
            var mnemonicsList = new List<MnemonicsType>
            {
                new MnemonicsType { Id = mnemonicId, HelperText = "Test Helper" }
            };

       
            _authServiceMock.Setup(auth => auth.GetUsername()).ReturnsAsync("testUser");

          
            _mnemonicServiceMock.Setup(service => service.GetMnemonicsAsync())
                .ReturnsAsync(mnemonicsList);
            _mnemonicServiceMock.Setup(service => service.GetMnemonicAsync(mnemonicId))
                .ReturnsAsync(new MnemonicsType { Id = mnemonicId, HelperText = "Test Helper" });

            
            _mnemonicServiceMock.Setup(service => service.DeleteMnemonicAsync(mnemonicId))
                .Returns(Task.CompletedTask);

            
            await _mnemonicsPage.RemoveMnemonic(mnemonicId);

            
            _mnemonicServiceMock.Verify(service => service.DeleteMnemonicAsync(mnemonicId),
                Times.Once); 
            Assert.Empty(_mnemonicsPage.mnemonicsList); 
        }

        [Fact]
        public async Task OnSubmit_ShouldCreateNewMnemonic_WhenValidInput()
        {
          
            _authServiceMock.Setup(auth => auth.IsUserLoggedInAsync()).ReturnsAsync(true);
            _authServiceMock.Setup(auth => auth.GetUsername()).ReturnsAsync("testUser");
            _authServiceMock.Setup(auth => auth.GetMnemonicsGuids("testUser"))
                .ReturnsAsync(new List<Guid>());

            _mnemonicServiceMock.Setup(service => service.GetMnemonicsByIdsAsync(It.IsAny<List<Guid>>()))
                .ReturnsAsync(new List<MnemonicsType>());

            _mnemonicServiceMock.Setup(service => service.CreateMnemonicAsync(It.IsAny<MnemonicsType>()))
                .ReturnsAsync(new MnemonicsType
                    { Id = Guid.NewGuid(), HelperText = "New Helper", MnemonicText = "New Mnemonic" });

            _authServiceMock.Setup(auth =>
                    auth.UpdateUserWithMnemonic(It.IsAny<string>(), It.IsAny<MnemonicsType>(), It.IsAny<bool>()))
                .Returns(Task.FromResult<MnemonicsType?>(new MnemonicsType
                    { Id = Guid.NewGuid(), HelperText = "Helper", MnemonicText = "Mnemonic" }));

            var newMnemonic = new MnemonicsType { HelperText = "New Helper", MnemonicText = "New Mnemonic" };
            _mnemonicsPage.Model = newMnemonic;

           
            await _mnemonicsPage.OnSubmit();

         
            _mnemonicServiceMock.Verify(service => service.CreateMnemonicAsync(It.IsAny<MnemonicsType>()), Times.Once);
            _authServiceMock.Verify(auth => auth.UpdateUserWithMnemonic("testUser", It.IsAny<MnemonicsType>(), true),
                Times.Once);
            Assert.Single(_mnemonicsPage.mnemonicsList);
            Assert.Null(_mnemonicsPage.errorMessage);
        }

        [Fact]
        public void Dispose_ShouldDisposeTimer()
        {
           
            _mnemonicsPage.Dispose(); 

       
            Assert.Null(_mnemonicsPage._timer);
        }

        [Fact]
        public async Task RemoveMnemonic_ShouldRemoveMnemonicFromListAndDatabase()
        {
            
            var mnemonicId = Guid.NewGuid();
            var existingMnemonic = new MnemonicsType
                { Id = mnemonicId, HelperText = "Test", MnemonicText = "Test Mnemonic" };
            _mnemonicServiceMock.Setup(service => service.GetMnemonicsAsync())
                .ReturnsAsync(new List<MnemonicsType> { existingMnemonic });

            _mnemonicServiceMock.Setup(service => service.GetMnemonicAsync(mnemonicId))
                .ReturnsAsync(existingMnemonic);

            _mnemonicServiceMock.Setup(service => service.DeleteMnemonicAsync(mnemonicId))
                .Returns(Task.CompletedTask);

            _authServiceMock.Setup(auth => auth.GetUsername()).ReturnsAsync("testUser");

           
            await _mnemonicsPage.RemoveMnemonic(mnemonicId);

          
            _mnemonicServiceMock.Verify(service => service.DeleteMnemonicAsync(mnemonicId), Times.Once);
            Assert.Empty(_mnemonicsPage.mnemonicsList);
        }

        [Fact]
        public async Task OnSubmit_ThrowsInvalidOperationException_WhenMnemonicAlreadyExists()
        {
            // Arrange
            _authServiceMock.Setup(auth => auth.IsUserLoggedInAsync()).ReturnsAsync(true);
            _authServiceMock.Setup(auth => auth.GetUsername()).ReturnsAsync("testUser");
            _authServiceMock.Setup(auth => auth.GetMnemonicsGuids("testUser"))
                .ReturnsAsync(new List<Guid> { Guid.NewGuid() });

            _mnemonicServiceMock.Setup(service => service.GetMnemonicsByIdsAsync(It.IsAny<List<Guid>>()))
                .ReturnsAsync(new List<MnemonicsType> { new MnemonicsType { HelperText = "Test Helper" } });

            _mnemonicsPage.Model.HelperText = "Test Helper"; 

           
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _mnemonicsPage.OnSubmit());
           
        }

        [Fact]
        public async Task OnTabClosing_ShouldRemoveUserFromActiveUserClient()
        {
            
            var userGuid = Guid.Empty; 

           
            _jsRuntimeMock.Setup(js => js.InvokeAsync<Guid>("sessionStorage.getItem", It.IsAny<object[]>()))
                .ReturnsAsync(userGuid);

         
            _activeUserClientMock.Setup(client => client.RemoveUserAsync(userGuid))
                .Returns(Task.CompletedTask);

           
            await _mnemonicsPage.OnTabClosing();

   
            _activeUserClientMock.Verify(client => client.RemoveUserAsync(Guid.Empty), Times.Once);
        }

        [Fact]
        public async Task LoadMnemonics_ShouldHandleNoMnemonics_ForLoggedInUser()
        {
           
            var userGuid = Guid.NewGuid();

        
            _authServiceMock.Setup(auth => auth.IsUserLoggedInAsync()).ReturnsAsync(true);
            _authServiceMock.Setup(auth => auth.GetUsername()).ReturnsAsync("testUser");
            _authServiceMock.Setup(auth => auth.GetMnemonicsGuids("testUser"))
                .ReturnsAsync(new List<Guid>());
            
            _mnemonicServiceMock.Setup(service => service.GetMnemonicsByIdsAsync(It.IsAny<List<Guid>>()))
                .ReturnsAsync(new List<MnemonicsType>());

            
            await _mnemonicsPage.LoadMnemonics();

            
            Assert.True(_mnemonicsPage.showMnemonics); 
            Assert.Empty(_mnemonicsPage.mnemonicsList); 
        }

 
        
    }
}
