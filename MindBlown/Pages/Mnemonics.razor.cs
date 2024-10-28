using System.Text.Json;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MindBlown.Types;

namespace MindBlown.Pages
{
    public partial class Mnemonics
    {
        private MnemonicsType Model { get; set; } = new MnemonicsType();
        private List<MnemonicsType> mnemonicsList = new List<MnemonicsType>();
        //gali neprireikt private List<MnemonicsType> loadedMnemonicsList = new List<MnemonicsType>();
        private bool showMnemonics = false;
        private bool mnemonicAlreadyExists;
        private string invalidInputMessage = "Mnemonic with given Helper text already exists.";
        private string? errorMessage { get; set; }
        private bool errorMessageIsVisible { get; set; }
        private string? successMessage { get; set; }
        private bool successMessageIsVisible { get; set; }

        private async Task OnSubmit()
        {
            // Getting mnemonics from local storage
            var existingMnemonics = await localStorage.GetItemAsync<List<MnemonicsType>>("userMnemonics") ?? new List<MnemonicsType>();

            mnemonicAlreadyExists = existingMnemonics.Where(m => m.HelperText == Model.HelperText).Count() > 0;

            if (mnemonicAlreadyExists)
            {
                await localStorage.SetItemAsync("userMnemonics", existingMnemonics);
                Model = new MnemonicsType();
                return;
            }

            var newMnemonic = new MnemonicsType
            {
                Id = Guid.NewGuid(),
                HelperText = Model.HelperText,
                MnemonicText = Model.MnemonicText,
                Category = Model.Category
            };

            existingMnemonics.Add(newMnemonic);

            // save the values to local storage
            await localStorage.SetItemAsync("userMnemonics", existingMnemonics);

            // clear for new input
            Model = new MnemonicsType();
        }

        private async Task LoadMnemonics()
        {
            // read the value as list
            mnemonicsList = await localStorage.GetItemAsync<List<MnemonicsType>>("userMnemonics") ?? new List<MnemonicsType>();

            // Show the mnemonics after loading
            showMnemonics = true;
        }

        private async Task RemoveMnemonic(Guid mnemonicId)
        {
            var existingMnemonics = await localStorage.GetItemAsync<List<MnemonicsType>>("userMnemonics") ?? new List<MnemonicsType>();

            var mnemonicToRemove = existingMnemonics.FirstOrDefault(m => m.Id == mnemonicId);
            if (mnemonicToRemove != null)
            {
                existingMnemonics.Remove(mnemonicToRemove);
                await localStorage.SetItemAsync("userMnemonics", existingMnemonics);

                var mnemonicInDB = await MnemonicService.GetMnemonicAsync(mnemonicToRemove.Id);
                if(mnemonicInDB != null)
                {
                    await MnemonicService.DeleteMnemonicAsync(mnemonicToRemove.Id);
                }

                // Refresh the mnemonics list after removal
                mnemonicsList = existingMnemonics;
            }
        }
        // On Users button press Enter, submit the form couse its cringe to click with mouse
        public async Task Enter(KeyboardEventArgs e)
        {
            if (e.Key == "Enter" || e.Key == "NumpadEnter")
            {
                await OnSubmit();
            }
        }

        private void RedirectToUpload()
        {
            Navigation.NavigateTo("/json-upload");
        }

        private async Task DownloadJson()
        {

            var existingMnemonics = await localStorage.GetItemAsync<List<MnemonicsType>>("userMnemonics") ?? new List<MnemonicsType>();

            object boxedMnemonics = existingMnemonics;

            var fileName = "mnemonics.json";
            var jsonFormattedData = JsonSerializer.Serialize(boxedMnemonics, options: new JsonSerializerOptions { WriteIndented = true });

            await JS.InvokeVoidAsync("downloadFile", fileName, jsonFormattedData);
        }

        private async Task GetMnemonicsFromDB()
        {
            List<MnemonicsType>? mnemonicListFromDB = await MnemonicService.GetMnemonicsAsync() ?? new List<MnemonicsType>();

            if (mnemonicListFromDB.Count() == 0)
            {
                await ShowErrorMessage("Database is empty");
            }
            else
            {
                await ShowSuccessMessage("Upload from database successful");
            }

            mnemonicsList = mnemonicsList.Union(mnemonicListFromDB).ToList();
            
            await localStorage.SetItemAsync("userMnemonics", mnemonicsList);
        }

        private async Task UpdateDatabase()
        {
            if(mnemonicsList.Any())
            {
                await MnemonicService.CreateMnemonicsAsync(mnemonicsList);
                await ShowSuccessMessage("Database updated");
            }
            else
                await ShowErrorMessage("Cannot update database. Mnemonic list is empty");
        }

        public async Task ShowErrorMessage(string givenErrorMessage)
        {
            errorMessage = givenErrorMessage;
            errorMessageIsVisible = true;
            StateHasChanged();

            await Task.Delay(3000);
            
            errorMessageIsVisible = false;
            StateHasChanged();
        }

        public async Task ShowSuccessMessage(string givenSuccessMessage)
        {
            successMessage = givenSuccessMessage;
            successMessageIsVisible = true;
            StateHasChanged();

            await Task.Delay(3000);
            
            successMessageIsVisible = false;
            StateHasChanged();
        }
    }
}