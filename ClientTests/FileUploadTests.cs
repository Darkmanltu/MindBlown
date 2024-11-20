// using System;
// using System.Collections.Generic;
// using System.IO;
// using System.Text;
// using System.Threading.Tasks;
// using Microsoft.AspNetCore.Components.Web;
// using MindBlown.Interfaces;
// using MindBlown.Pages;
// using MindBlown.Types;
// using Xunit;
// using Microsoft.Extensions.DependencyInjection;
// using System.Net.Http;
// using System.Net;
// using Microsoft.AspNetCore.Components.Forms;
// using Services;
//
// namespace MindBlown.Tests
// {
//     public class FileUploadTests
//     {
//         // Real HttpClient (or fake it with a test server)
//         public class FakeHttpClient : HttpClient
//         {
//             public FakeHttpClient() : base(new HttpMessageHandlerStub())
//             {
//                 // Set a base address so that HttpClient can make requests
//                 this.BaseAddress = new Uri("https://example.com");
//             }
//         }
//
//         // Simulating an HTTP response for MnemonicService
//         public class HttpMessageHandlerStub : HttpMessageHandler
//         {
//             protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
//             {
//                 var response = new HttpResponseMessage(HttpStatusCode.OK)
//                 {
//                     Content = new StringContent("[{\"HelperText\": \"Test Helper\", \"MnemonicText\": \"Test Mnemonic\", \"Category\": 0}]")
//                 };
//                 return Task.FromResult(response);
//             }
//         }
//
//         // Mock implementation of IBrowserFile for file upload
//         public class MockBrowserFile : IBrowserFile
//         {
//             private readonly byte[] _fileContent;
//
//             public MockBrowserFile(string content)
//             {
//                 _fileContent = Encoding.UTF8.GetBytes(content);
//                 LastModified = DateTimeOffset.UtcNow; // Simulating a last modified timestamp
//             }
//
//             public string Name => "testFile.json";
//             public long Size => _fileContent.Length;
//             public string ContentType => "application/json";
//
//             public DateTimeOffset LastModified { get; }
//
//             public Stream OpenReadStream(long maxAllowedSize = 0, CancellationToken cancellationToken = default)
//             {
//                 var memoryStream = new MemoryStream(_fileContent);
//                 return memoryStream;
//             }
//         }
//
//         [Fact]
//         public async Task Should_LoadMnemonics_OnFileUpload()
//         {
//             // Arrange: Set up mocked services and data
//             var fakeHttpClient = new FakeHttpClient(); // Use the fake HttpClient
//             var mockService = new MnemonicService(fakeHttpClient); // Provide the fake HttpClient to the MnemonicService
//
//             var fileContent = "[{\"HelperText\": \"Test Helper\", \"MnemonicText\": \"Test Mnemonic\", \"Category\": 0}]";
//             var mockFile = new MockBrowserFile(fileContent); // Mocking a JSON file
//
//             // Instantiate the FileUpload component directly
//             var fileUpload = new FileUpload { MnemonicService = mockService };
//
//             // Simulate file selection
//             var inputFileChangeEventArgs = new InputFileChangeEventArgs(new[] { mockFile });
//             fileUpload.HandleFileSelected(inputFileChangeEventArgs);
//
//             // Act: Simulate file upload
//             await fileUpload.UploadFile();
//
//             // Assert: Check if the mnemonic was successfully added
//             var mnemonics = await mockService.GetMnemonicsAsync();
//             Assert.Single(mnemonics); // Ensure one mnemonic was added
//             Assert.Equal("Test Helper", mnemonics.First().HelperText);
//             Assert.Equal("Test Mnemonic", mnemonics.First().MnemonicText);
//             Assert.Equal(MnemonicCategory.Other, mnemonics.First().Category);
//         }
//
//         [Fact]
//         public async Task Should_ShowSuccessMessage_AfterFileUpload()
//         {
//             // Arrange: Set up mocked services and data
//             var fakeHttpClient = new FakeHttpClient(); // Use the fake HttpClient
//             var mockService = new MnemonicService(fakeHttpClient); // Provide the fake HttpClient to the MnemonicService
//
//             var fileContent = "[{\"HelperText\": \"Success Helper\", \"MnemonicText\": \"Success Mnemonic\", \"Category\": 0}]";
//             var mockFile = new MockBrowserFile(fileContent);
//
//             // Instantiate the FileUpload component directly
//             var fileUpload = new FileUpload { MnemonicService = mockService };
//
//             // Simulate file selection
//             var inputFileChangeEventArgs = new InputFileChangeEventArgs(new[] { mockFile });
//             fileUpload.HandleFileSelected(inputFileChangeEventArgs);
//
//             // Act: Simulate file upload
//             await fileUpload.UploadFile();
//
//             // Assert: Check if success message is set
//             Assert.Equal("File uploaded and mnemonics unboxed successfully!", fileUpload.message);
//         }
//
//         [Fact]
//         public void Should_ResetFileInput_AfterUpload()
//         {
//             // Arrange: Set up mocked services and data
//             var fakeHttpClient = new FakeHttpClient(); // Use the fake HttpClient
//             var mockService = new MnemonicService(fakeHttpClient); // Provide the fake HttpClient to the MnemonicService
//
//             var fileContent = "[{\"HelperText\": \"HelperText\", \"MnemonicText\": \"MnemonicText\", \"Category\": 0}]";
//             var mockFile = new MockBrowserFile(fileContent);
//
//             // Instantiate the FileUpload component directly
//             var fileUpload = new FileUpload { MnemonicService = mockService };
//
//             // Simulate file selection
//             var inputFileChangeEventArgs = new InputFileChangeEventArgs(new[] { mockFile });
//             fileUpload.HandleFileSelected(inputFileChangeEventArgs);
//
//             // Act: Reset file input after selection
//             fileUpload.ResetFileInput();
//
//             // Assert: Ensure that the fileInfo is reset to null
//             Assert.Null(fileUpload.fileInfo);
//         }
//     }
// }
