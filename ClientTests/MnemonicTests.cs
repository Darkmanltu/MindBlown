// using Moq;
// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;
// using MindBlown;
// using MindBlown.Interfaces;
// using MindBlown.Types;
// using MindBlown.Exceptions;
// using MindBlown.Pages;
// using Xunit;
// using Xunit.Abstractions;
//
// namespace MindBlown.Tests
// {
//     public class MnemonicsTests : Mnemonics
//     {
//         private readonly ITestOutputHelper _testOutputHelper;
//         private readonly Mock<IMnemonicService> _mnemonicServiceMock;
//         private readonly Mock<ILoggingService> _loggingServiceMock;
//         private readonly Mock<IActiveUserClient> _activeUserClientMock;
//
//
//         public MnemonicsTests(ITestOutputHelper testOutputHelper)
//         {
//             _testOutputHelper = testOutputHelper;
//             _mnemonicServiceMock = new Mock<IMnemonicService>();
//             _loggingServiceMock = new Mock<ILoggingService>();
//             _activeUserClientMock = new Mock<IActiveUserClient>();
//         }
//
//         [Fact]
//         public async Task OnSubmit_ShouldAddNewMnemonic_WhenNotExists()
//         {
//
//             var model = new MnemonicsType
//             {
//                 HelperText = "Test Helper",
//                 MnemonicText = "Test Mnemonic",
//                 Category = MnemonicCategory.Other
//             };
//
//             var existingMnemonics = new List<MnemonicsType>();
//             _mnemonicServiceMock.Setup(s => s.GetMnemonicsAsync()).ReturnsAsync(existingMnemonics);
//             _mnemonicServiceMock.Setup(s => s.CreateMnemonicAsync(It.IsAny<MnemonicsType>())).ReturnsAsync(model);
//
//             var mnemonics = new Mnemonics
//             {
//                 MnemonicService = _mnemonicServiceMock.Object,
//                 LoggingService = _loggingServiceMock.Object,
//                 ActiveUserClient = _activeUserClientMock.Object,
//                 Model = model
//             };
//
//
//             await mnemonics.OnSubmit();
//
//
//             Assert.Contains(mnemonics.mnemonicsList, m => m.HelperText == model.HelperText);
//         }
//
//
//         [Fact]
//         public async Task LoadMnemonics_ShouldLoadDataFromService()
//         {
//
//             var mockData = new List<MnemonicsType>
//             {
//                 new MnemonicsType
//                 {
//                     Id = Guid.NewGuid(), HelperText = "Helper", MnemonicText = "Mnemonic",
//                     Category = MnemonicCategory.Other
//                 }
//             };
//             _mnemonicServiceMock.Setup(s => s.GetMnemonicsAsync()).ReturnsAsync(mockData);
//
//             var mnemonics = new Mnemonics
//             {
//                 MnemonicService = _mnemonicServiceMock.Object,
//                 LoggingService = _loggingServiceMock.Object,
//                 ActiveUserClient = _activeUserClientMock.Object,
//
//             };
//
//
//             await mnemonics.LoadMnemonics();
//
//
//             Assert.Equal(mockData.Count, mnemonics.mnemonicsList.Count);
//             Assert.True(mnemonics.showMnemonics);
//         }
//
//         [Fact]
//         public async Task LoadMnemonics_ShouldLoadEmptyList_WhenNoDataInService()
//         {
//
//             _mnemonicServiceMock.Setup(s => s.GetMnemonicsAsync()).ReturnsAsync(new List<MnemonicsType>());
//
//             var mnemonics = new Mnemonics
//             {
//                 MnemonicService = _mnemonicServiceMock.Object,
//                 LoggingService = _loggingServiceMock.Object,
//                 ActiveUserClient = _activeUserClientMock.Object
//             };
//
//
//             await mnemonics.LoadMnemonics();
//
//
//             Assert.Empty(mnemonics.mnemonicsList);
//             Assert.True(mnemonics.showMnemonics);
//         }
//
//     }
// }
