using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MindBlown.Interfaces;
using MindBlown.Types;
using MindBlown.Pages;
using MindBlown.Services;
using Moq;
using Xunit;

namespace MindBlown.Tests
{
    public class TestMnemonicTests
    {
        private readonly Mock<IActiveUserClient> _mockActiveUserClient;
        private readonly Mock<IMnemonicService> _mockMnemonicService;
        private readonly Mock<IAuthService> _mockAuthService;
        private readonly Mock<ILWARecordService> _mockLWARecordService;
        private readonly Mock<IJSRuntime> _mockJSRuntime;

        public TestMnemonicTests()
        {
            _mockActiveUserClient = new Mock<IActiveUserClient>();
            _mockMnemonicService = new Mock<IMnemonicService>();
            _mockAuthService = new Mock<IAuthService>();
            _mockLWARecordService = new Mock<ILWARecordService>();
            _mockJSRuntime = new Mock<IJSRuntime>();
        }

// [Fact]
// public async Task OnInitializedAsync_ShouldInitializeUserIdAndActiveUser_UsingReflection()
// {
//     // Arrange
//     var testComponent = new TestMnemonic
//     {
//         ActiveUserClient = _mockActiveUserClient.Object,
//         MnemonicService = _mockMnemonicService.Object,
//         AuthService = _mockAuthService.Object,
//         LWARecordService = _mockLWARecordService.Object,
//         JS = _mockJSRuntime.Object
//     };
//
//     _mockJSRuntime
//         .Setup(js => js.InvokeAsync<string>("sessionStorage.getItem", It.Is<object[]>(args => args[0].ToString() == "userId")))
//         .ReturnsAsync(string.Empty);
//
//     _mockJSRuntime
//         .Setup(js => js.InvokeVoidAsync(
//             It.Is<string>(method => method == "sessionStorage.setItem"),
//             It.Is<object[]>(args =>
//             {
//                 var userIdString = args[1].ToString();
//                 return args[0].ToString() == "userId" && Guid.TryParse(userIdString, out var parsedGuid);
//             })
//         ));
//     _mockActiveUserClient
//         .Setup(client => client.IsSessionIdUniqueAsync(It.IsAny<Guid>()))
//         .ReturnsAsync(true);
//
//     _mockActiveUserClient
//         .Setup(client => client.AddUserAsync(It.IsAny<Guid>()))
//         .Returns(Task.CompletedTask);
//
//     var mockUserDictionary = new ConcurrentDictionary<Guid, User>();
//     _mockActiveUserClient
//         .Setup(client => client.GetDictionary())
//         .ReturnsAsync(mockUserDictionary);
//
//     _mockActiveUserClient
//         .Setup(client => client.GetActiveUserCountAsync(It.IsAny<ConcurrentDictionary<Guid, User>>()))
//         .ReturnsAsync(1);
//
//     // Act
//     var method = typeof(TestMnemonic).GetMethod("OnInitializedAsync",
//         System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
//     if (method == null)
//         throw new InvalidOperationException("OnInitializedAsync method not found");
//
//     var task = (Task)method.Invoke(testComponent, null);
//     await task;
//
//     // Assert
//     _mockJSRuntime.Verify(js => js.InvokeVoidAsync(
//         "sessionStorage.setItem",
//         It.Is<object[]>(args => args[0].ToString() == "userId" && Guid.TryParse(args[1].ToString(), out _))
//     ), Times.Once);
//
//     _mockActiveUserClient.Verify(client => client.AddUserAsync(It.IsAny<Guid>()), Times.Once);
// }

        [Fact]
        public async Task LoadMnemonics_ShouldLoadMnemonicsCorrectly()
        {
            
            var mnemonicsList = new List<MnemonicsType>
            {
                new MnemonicsType { MnemonicText = "Sample Mnemonic" }
            };

            _mockMnemonicService
                .Setup(service => service.GetMnemonicsAsync())
                .ReturnsAsync(mnemonicsList);

            var testComponent = new TestMnemonic
            {
                MnemonicService = _mockMnemonicService.Object,
                AuthService = _mockAuthService.Object
            };

            
            await testComponent.LoadMnemonics();

            
            Assert.NotNull(testComponent.mnemonicsList);
            Assert.Equal(1, testComponent.mnemonicsList.Count());
        }

        // [Fact]
        // public async Task CheckMnemonic_ShouldUpdateAnsweringStatsCorrectly_WhenAnswerIsCorrect()
        // {
        //     // Arrange
        //     var testMnemonic = new MnemonicsType { MnemonicText = "Correct Answer" };
        //     var testComponent = new TestMnemonic
        //     {
        //         testingMnemonic = testMnemonic,
        //         userGivenMnemonicText = "Correct Answer"
        //     };
        //
        //     // Act
        //     await testComponent.CheckMnemonic();
        //
        //     // Assert
        //     Assert.Equal(1, testComponent.answeringStats.correctAnswerCount);
        //     Assert.Equal(1, testComponent.answeringStats.allAnswerCount);
        // }

        // [Fact]
        // public async Task CheckMnemonic_ShouldUpdateLastWrongAnswerRecord_WhenAnswerIsIncorrect()
        // {
        //     // Arrange
        //     var testMnemonic = new MnemonicsType
        //     {
        //         MnemonicText = "Correct Answer",
        //         HelperText = "Help",
        //         Category = MnemonicCategory.Other,
        //         Id = Guid.NewGuid()
        //     };
        //
        //     var testComponent = new TestMnemonic
        //     {
        //         testingMnemonic = testMnemonic,
        //         userGivenMnemonicText = "Wrong Answer",
        //         LWARecordService = _mockLWARecordService.Object,
        //         AuthService = _mockAuthService.Object
        //     };
        //
        //     _mockAuthService.Setup(service => service.GetUsername()).ReturnsAsync("testuser");
        //     _mockAuthService.Setup(service => service.GetLWARecordId("testuser")).ReturnsAsync(Guid.NewGuid());
        //     _mockLWARecordService
        //         .Setup(service => service.UpdateRecordAsync(It.IsAny<Guid>(), It.IsAny<LastWrongAnswerRecord>()))
        //         .ReturnsAsync(new LastWrongAnswerRecord
        //         {
        //             Id = Guid.NewGuid(),
        //             helperText = "Default helper text",
        //             mnemonicText = "Default mnemonic text",
        //             wrongTextMnemonic = "Default wrong mnemonic text",
        //             category = MnemonicCategory.Other
        //         });
        //     // Act
        //     await testComponent.CheckMnemonic();
        //
        //     // Assert
        //     Assert.NotNull(testComponent.lastWrongAnswer);
        //     Assert.Equal("Wrong Answer", testComponent.lastWrongAnswer.wrongTextMnemonic);
        // }

        [Fact]
        public void GetRandomMnemonic_ShouldReturnRandomMnemonic()
        {
            
            var mnemonicsList = new List<MnemonicsType>
            {
                new MnemonicsType { MnemonicText = "Mnemonic 1" },
                new MnemonicsType { MnemonicText = "Mnemonic 2" }
            };

            var testComponent = new TestMnemonic
            {
                mnemonicsList = new Repository<MnemonicsType>(mnemonicsList)
            };

            
            var randomMnemonic = testComponent.getRandomMnemonic();

            
            Assert.Contains(randomMnemonic, mnemonicsList); 
        }

        // [Fact]
        // public async Task Enter_ShouldCallCheckMnemonic_OnEnterKey()
        // {
        //     // Arrange
        //     var wasCalled = false;
        //
        //     var testComponent = new TestMnemonic
        //     {
        //         CheckMnemonic = async () => { wasCalled = true; }
        //     };
        //
        //     var keyboardEventArgs = new KeyboardEventArgs { Key = "Enter" };
        //
        //     // Act
        //     await testComponent.Enter(keyboardEventArgs);
        //
        //     // Assert
        //     Assert.True(wasCalled);
        // }

    }
}