using System.Text.Json;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

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
            var existingMnemonics = await localStorage.GetItemAsync<List<MnemonicsType>>("userMnemonics") ?? new
            List<MnemonicsType>();

            var mnemonicToRemove = existingMnemonics.FirstOrDefault(m => m.Id == mnemonicId);
            if (mnemonicToRemove != null)
            {
                existingMnemonics.Remove(mnemonicToRemove);
                await localStorage.SetItemAsync("userMnemonics", existingMnemonics);

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

            object boxedMnemonics = (object)existingMnemonics;

            var fileName = "mnemonics.json";
            var jsonFormattedData = JsonSerializer.Serialize(boxedMnemonics, options: new JsonSerializerOptions { WriteIndented = true });

            await JS.InvokeVoidAsync("downloadFile", fileName, jsonFormattedData);
        }
    }
}