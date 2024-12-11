using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MindBlown.Interfaces;
using MindBlown.Types;
using Services;

namespace MindBlown.Pages
{
    public partial class FileUpload
    {
        [Inject] public required IMnemonicService MnemonicService { get; set; }
        [Inject] public required NavigationManager Navigation { get; set; }

        public IBrowserFile? fileInfo;

        public string? message;

        public void HandleFileSelected(InputFileChangeEventArgs e)
        {
            fileInfo = e.GetMultipleFiles(1).FirstOrDefault(); // Get the first selected file
            message = string.Empty;
        }

        public void ReturnToSetup()
        {
            Navigation.NavigateTo("/addMnemonic");
        }

        public async Task UploadFile()
        {
            if (fileInfo != null)
            {
                try
                {
                    using var stream = fileInfo.OpenReadStream(maxAllowedSize: 10_000_000);
                    using var reader = new StreamReader(stream);
                    var jsonString = await reader.ReadToEndAsync();

                    var boxedMnemonicsList = JsonSerializer.Deserialize<List<object>>(jsonString);

                    if (boxedMnemonicsList != null)
                    {
                        foreach (var boxedMnemonic in boxedMnemonicsList)
                        {
                            var boxedString = boxedMnemonic?.ToString();
                            if (!string.IsNullOrEmpty(boxedString))
                            {
                                var mnemonic = JsonSerializer.Deserialize<MnemonicsType>(boxedString);

                                if (mnemonic != null && mnemonic.HelperText != null)
                                {
                                    var existingMnemonics = await MnemonicService.GetMnemonicsAsync() ??
                                                            new List<MnemonicsType>();

                                    var mnemonicAlreadyExists =
                                        existingMnemonics.Any(m => m.HelperText == mnemonic.HelperText);

                                    if (!mnemonicAlreadyExists)
                                    {
                                        var jsonCategory =
                                            Enum.IsDefined<MnemonicCategory>((MnemonicCategory)mnemonic.Category)
                                                ? (MnemonicCategory)mnemonic.Category
                                                : MnemonicCategory.Other;

                                        var mnemonicFromList = new MnemonicsType
                                        {
                                            Id = Guid.NewGuid(),
                                            HelperText = mnemonic.HelperText,
                                            MnemonicText = mnemonic.MnemonicText,
                                            Category = jsonCategory,
                                        };

                                        existingMnemonics.Add(mnemonicFromList);
                                        await MnemonicService.CreateMnemonicAsync(mnemonicFromList);
                                    }
                                }
                            }
                        }

                        message = "File uploaded and mnemonics unboxed successfully!";
                        ResetFileInput();
                    }
                }
                catch (JsonException)
                {
                    message = "Invalid JSON format.";
                }
                catch (Exception ex)
                {
                    message = $"An error occurred: {ex.Message}";
                }
            }
        }

        public void ResetFileInput()
        {
            fileInfo = null;
        }
    }
}