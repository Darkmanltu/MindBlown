using System.Text.Json;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MindBlown.Types;
using MindBlown.Exceptions;
using Microsoft.AspNetCore.Components;
using System.Collections.Concurrent;
using MindBlown.Interfaces;
using Services;


namespace MindBlown.Pages
{
    public partial class Mnemonics : IDisposable
    {
        [Inject]
        public required IMnemonicService MnemonicService { get; set; }
        [Inject]
        public required ILoggingService LoggingService { get; set; }
        [Inject]
        public required IActiveUserClient ActiveUserClient { get; set; }


        // private TimedRemovalService TimedRemovalService { get; set; } = default!;
        public MnemonicsType Model { get; set; } = new MnemonicsType();

        public int ActiveUserCount {get; set;}
        
       

        public List<MnemonicsType> mnemonicsList = new List<MnemonicsType>();
        public bool showMnemonics = false;
        public bool mnemonicAlreadyExists;
        public string invalidInputMessage = "Mnemonic with given Helper text already exists.";
        public string? errorMessage { get; set; }
        public bool errorMessageIsVisible { get; set; }
        public string? successMessage { get; set; }
        public bool successMessageIsVisible { get; set; }
        public bool loadMnemonicsButtonWasPressed { get; set; }
        private Timer? _timer;


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


            Console.WriteLine("User ID: " + userId);
            // Add the user to ActiveUserClient
            bool isUnique = await ActiveUserClient.IsSessionIdUniqueAsync(userId);

            await Task.Delay(5000);

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


            _timer = new Timer(async async =>
            {
                await CheckActiveUserCountAsync();
            }, null, TimeSpan.Zero, TimeSpan.FromSeconds(30));

        }


        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await LoadMnemonics();
            }
        }

        private async Task CheckActiveUserCountAsync()
        {
            // Retrieve the active user count
            var activeUserCountCheck = await ActiveUserClient.GetActiveUserCountAsync(await ActiveUserClient.GetDictionary());
            // Console.WriteLine($"Active user recounter: {activeUserCountCheck}");

            // Update the state if the count has changed
            if (ActiveUserCount != activeUserCountCheck)
            {
                ActiveUserCount = activeUserCountCheck;
                StateHasChanged(); // Trigger re-render
            }
        }


        public async Task OnSubmit()
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
                var username = await AuthService.GetUsername();
                await AuthService.UpdateUserWithMnemonic(username, newMnemonic);
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
                // await ShowErrorMessage(ex.Message);
                Model = new MnemonicsType();
                StateHasChanged();
                await Task.Delay(3000);
                mnemonicAlreadyExists = false;
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

        public async Task LoadMnemonics()
        {
            // Get mnemonics from database. Returns a list of MnemonicsType
            mnemonicsList = await MnemonicService.GetMnemonicsAsync() ?? new List<MnemonicsType>();
            if(await AuthService.IsUserLoggedInAsync())
            {    
                var username = await AuthService.GetUsername();

                // Get user's ids
                var guids = await AuthService.GetMnemonicsGuids(username);
                
                if (guids.Count == 0)
                {
                    mnemonicsList = new List<MnemonicsType>();
                }
                else
                {
                    mnemonicsList = await MnemonicService.GetMnemonicsByIdsAsync(guids);
                }
            }
            // Show the mnemonics after loading
            showMnemonics = true;
        }

        public async Task RemoveMnemonic(Guid mnemonicId)
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


            // Disposing timer for checking whether active user count updated
            _timer?.Dispose(); 
        }

        public async Task DisposeAsync()

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

        public void RedirectToUpload()
        {
            Navigation.NavigateTo("/json-upload");
        }

        public async Task DownloadJson()
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