using System.Text.Json;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MindBlown.Types;
using MindBlown.Exceptions;
using Microsoft.AspNetCore.Components;
using System.Collections.Concurrent;



namespace MindBlown.Pages
{
    public partial class Mnemonics : IDisposable
    {

        // private TimedRemovalService TimedRemovalService { get; set; } = default!;
        private MnemonicsType Model { get; set; } = new MnemonicsType();

        private int ActiveUserCount {get; set;}
        
       

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
            try
            {
                var existingMnemonics = await MnemonicService.GetMnemonicsAsync() ?? new List<MnemonicsType>();
                // mnemonicAlreadyExists = existingMnemonics.Any(m => m.HelperText == Model.HelperText);
                mnemonicAlreadyExists = existingMnemonics.Where(m => m.HelperText == Model.HelperText).Count() > 0;

                if (mnemonicAlreadyExists)
                {
                    throw new MnemonicAlreadyExistsException(invalidInputMessage);
                }

                var newMnemonic = new MnemonicsType
                {
                    Id = Guid.NewGuid(),
                    HelperText = Model.HelperText,
                    MnemonicText = Model.MnemonicText,
                    Category = Model.Category
                };

                var addedMnemonic = await MnemonicService.CreateMnemonicAsync(newMnemonic);
                mnemonicsList.Add(newMnemonic);

                if (addedMnemonic == null)
                    await ShowErrorMessage("Mnemonic could not be added to the database");

                Model = new MnemonicsType();

                if (loadMnemonicsButtonWasPressed)
                    StateHasChanged();
            }
            catch (MnemonicAlreadyExistsException ex)
            {
                await LoggingService.LogAsync(new LogEntry
                {
                    Message = ex.Message,
                    Details = ex.ToString()
                });
                await ShowErrorMessage(ex.Message);
                Model = new MnemonicsType();
            }
            catch (Exception ex)
            {
                await LoggingService.LogAsync(new LogEntry
                {
                    Message = "Unexpected error",
                    Details = ex.ToString()
                });
                await ShowErrorMessage($"An unexpected error occurred: {ex.Message}");
            }
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
                if (mnemonicInDB != null)
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
       protected override async Task OnInitializedAsync()
{
    var chechifnull = await JS.InvokeAsync<string>("sessionStorage.getItem", "userId");

    // If userId is null or empty, it means it doesn't exist
    if (string.IsNullOrEmpty(chechifnull))
    {
        // Generate a new Guid for the userId
        chechifnull = Guid.NewGuid().ToString();

        // Store the new userId in sessionStorage
        await JS.InvokeVoidAsync("sessionStorage.setItem", "userId", chechifnull);
    }

    // Retrieve user ID from session storage or generate a new one if it doesn't exist
    var userId = await JS.InvokeAsync<Guid>("sessionStorage.getItem", "userId");
    
    
    System.Console.WriteLine("User ID: " + userId);
    // Add the user to ActiveUserClient
   bool isUnique = await ActiveUserClient.IsSessionIdUniqueAsync(userId);

    if (isUnique)
    {
        // Add the user to ActiveUserClient only if the sessionId is unique
        await ActiveUserClient.AddUserAsync(userId);

        // Update the active user count
        
    }
    else
    {
        // If the sessionId is a duplicate, log or handle the error as needed
        await ShowErrorMessage("This session ID is already in use.");
    }
    

          
    
    await ActiveUserClient.RemoveInnactive();
    var activeUserDict = await ActiveUserClient.GetDictionary();
    //ActiveUserCount = await ActiveUserClient.GetActiveUserCountAsync();
    ActiveUserCount = await ActiveUserClient.GetActiveUserCountAsync(activeUserDict);
   //await ActiveUserClient.RemoveUserAsync(userId);
}

public void Dispose()
{
    try
    {
        // Blocking call for asynchronous logic in Dispose()
        DisposeAsync().GetAwaiter().GetResult();
    }
    catch (Exception )
    {   
        // throws an exception but still executes it fine idk why
        //Console.WriteLine($"Error during Dispose: {ex.Message}");
    }
}

private async Task DisposeAsync()
{
    // Perform async cleanup
    var userId = await JS.InvokeAsync<Guid>("sessionStorage.getItem", "userId");
    await ActiveUserClient.RemoveUserAsync(userId);
    var activeUserDict = await ActiveUserClient.GetDictionary();
    //ActiveUserCount = await ActiveUserClient.GetActiveUserCountAsync();
    ActiveUserCount = await ActiveUserClient.GetActiveUserCountAsync(activeUserDict);
}


        // On Users button press Enter, submit the form cause its cringe to click with mouse
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
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await LoadMnemonics();
            }
        }
    }

}