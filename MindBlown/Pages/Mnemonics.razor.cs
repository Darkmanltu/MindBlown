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
        private bool showMnemonics = false;
        private bool mnemonicAlreadyExists;
        private string invalidInputMessage = "Mnemonic with given Helper text already exists.";
        private string? errorMessage { get; set; }
        private bool errorMessageIsVisible { get; set; }
        private string? successMessage { get; set; }
        private bool successMessageIsVisible { get; set; }
        private bool loadMnemonicsButtonWasPressed { get; set; }

        private async Task OnSubmit()
        {
            // Getting mnemonics from database
            var existingMnemonics = await MnemonicService.GetMnemonicsAsync() ?? new List<MnemonicsType>();

            mnemonicAlreadyExists = existingMnemonics.Where(m => m.HelperText == Model.HelperText).Count() > 0;

            if (mnemonicAlreadyExists)
            {
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

            // Save new mnemonic to database
            var addedMnemonic = await MnemonicService.CreateMnemonicAsync(newMnemonic);

            mnemonicsList.Add(newMnemonic);

            // Check if succesfully added to database
            if (addedMnemonic == null)
                await ShowErrorMessage("Mnemonice could not be added to the database");

            // Clear for new input
            Model = new MnemonicsType();

            if (loadMnemonicsButtonWasPressed)
                StateHasChanged();
        }

        private async Task LoadMnemonics()
        {
            // Get mnemonics from database. Returns a list of MnemonicsType
            mnemonicsList = await MnemonicService.GetMnemonicsAsync() ?? new List<MnemonicsType>();

            // Show the mnemonics after loading
            showMnemonics = true;
        }

        private async Task RemoveMnemonic(Guid mnemonicId)
        {
            var existingMnemonics = await MnemonicService.GetMnemonicsAsync() ?? new List<MnemonicsType>();

            var mnemonicToRemove = existingMnemonics.FirstOrDefault(m => m.Id == mnemonicId);
            if (mnemonicToRemove != null)
            {
                existingMnemonics.Remove(mnemonicToRemove);

                var mnemonicInDB = await MnemonicService.GetMnemonicAsync(mnemonicToRemove.Id);
                if(mnemonicInDB != null)
                {
                    await MnemonicService.DeleteMnemonicAsync(mnemonicToRemove.Id);

                }
                else
                {
                    await ShowErrorMessage("Mnemonic to remove not found in database");
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

            var existingMnemonics = await MnemonicService.GetMnemonicsAsync() ?? new List<MnemonicsType>();

            object boxedMnemonics = existingMnemonics;

            var fileName = "mnemonics.json";
            var jsonFormattedData = JsonSerializer.Serialize(boxedMnemonics, options: new JsonSerializerOptions { WriteIndented = true });

            await JS.InvokeVoidAsync("downloadFile", fileName, jsonFormattedData);
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