using System.Text.Json;
using Microsoft.AspNetCore.Components.Forms;

namespace MindBlown.Pages
{
    public partial class FileUpload
    {
        private IBrowserFile? fileInfo;

        private string? message;

        private void HandleFileSelected(InputFileChangeEventArgs e)
        {
            fileInfo = e.GetMultipleFiles(1).FirstOrDefault(); // Get the first selected file
            message = string.Empty;
        }

        private void ReturnToSetup()
        {
            Navigation.NavigateTo("/addMnemonic");
        }

        private async Task UploadFile()
        {
            if (fileInfo != null)
            {
                using var stream = fileInfo.OpenReadStream(maxAllowedSize: 10_000_000);
                using var reader = new StreamReader(stream);
                var jsonString = await reader.ReadToEndAsync();

                //List<MnemonicTypeNoGuid>? listFromJson = JsonSerializer.Deserialize<List<MnemonicTypeNoGuid>>(jsonString); *@ 

                List<object>? boxedMnemonicsList = JsonSerializer.Deserialize<List<object>>(jsonString);

                if (boxedMnemonicsList != null)
                {
                    foreach (var boxedMnemonic in boxedMnemonicsList)
                    {
                        var boxedString = boxedMnemonic?.ToString();
                        if (!string.IsNullOrEmpty(boxedString))
                        {
                            // Unbox the object back to MnemonicsType by casting
                            var mnemonic = JsonSerializer.Deserialize<MnemonicsType>(boxedString);

                            if (mnemonic != null && mnemonic.HelperText != null)
                            {
                                var existingMnemonics = await localStorage.GetItemAsync<List<MnemonicsType>>("userMnemonics") ?? new List<MnemonicsType>();

                                var mnemonicAlreadyExists = existingMnemonics.Any(m => m.HelperText == mnemonic.HelperText);

                                if (!mnemonicAlreadyExists)
                                {
                                    MnemonicCategory jsonCategory;
                                    if (Enum.IsDefined<MnemonicCategory>((MnemonicCategory)mnemonic.Category))
                                    {
                                        jsonCategory = (MnemonicCategory)mnemonic.Category;
                                    }
                                    else
                                    {
                                        jsonCategory = MnemonicCategory.Other;
                                    }

                                    var mnemonicFromList = new MnemonicsType
                                    {
                                        Id = Guid.NewGuid(),
                                        HelperText = mnemonic.HelperText,
                                        MnemonicText = mnemonic.MnemonicText,
                                        Category = jsonCategory,
                                    };

                                    existingMnemonics.Add(mnemonicFromList);
                                }

                                await localStorage.SetItemAsync("userMnemonics", existingMnemonics);
                            }
                        }
                    }

                    message = "File uploaded and mnemonics unboxed successfully!";
                    ResetFileInput();
                }
            }
        }

        private void ResetFileInput()
        {
            fileInfo = null;
        }
    }
}