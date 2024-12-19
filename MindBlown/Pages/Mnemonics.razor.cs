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
        [Inject]
        public required IAuthService AuthService { get; set; }
        public MnemonicsType Model { get; set; } = new MnemonicsType();

        public int ActiveUserCount {get; set;}
        
        public Guid userId { get; set; }

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
            await LoadMnemonics();
            StateHasChanged();

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
            userId = await JS.InvokeAsync<Guid>("sessionStorage.getItem", "userId");

            // Add the user to ActiveUserClient
            bool isUnique = await ActiveUserClient.IsSessionIdUniqueAsync(userId);


            if (isUnique)
            {
                // Add the user to ActiveUserClient only if the sessionId is unique
                await ActiveUserClient.AddUserAsync(userId);
            }




            await ActiveUserClient.RemoveInnactive();
            var activeUserDict = await ActiveUserClient.GetDictionary();
            ActiveUserCount = await ActiveUserClient.GetActiveUserCountAsync(activeUserDict);


            _timer = new Timer(async async =>
            {
                await CheckActiveUserCountAsync();
            }, null, TimeSpan.Zero, TimeSpan.FromSeconds(30));

        }


        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await JS.InvokeVoidAsync("detectTabCloseF", DotNetObjectReference.Create(this));
                await LoadMnemonics();
            }
        }
        
        [JSInvokable]
        public async Task OnTabClosing()
        {
            await ActiveUserClient.RemoveUserAsync(userId);
            
        }

        private async Task CheckActiveUserCountAsync()
        {
            // Retrieve the active user count
            var activeUserCountCheck = await ActiveUserClient.GetActiveUserCountAsync(await ActiveUserClient.GetDictionary());

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
                // Get mnemonics from database. Returns a list of MnemonicsType
                var existingMnemonics = new List<MnemonicsType>();

                if(await AuthService.IsUserLoggedInAsync())
                {    
                    // var username = await AuthService.GetUsername();

                    // Get user's ids
                    var guids = await AuthService.GetMnemonicsGuids(await AuthService.GetUsername());

                    if (guids.Count == 0)
                    {
                        existingMnemonics = new List<MnemonicsType>();
                    }
                    else
                    {
                        existingMnemonics = await MnemonicService.GetMnemonicsByIdsAsync(guids);
                    }
                }

             
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
                await AuthService.UpdateUserWithMnemonic(username, newMnemonic, true);
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
                    var username = await AuthService.GetUsername();
                    
                    await MnemonicService.DeleteMnemonicAsync(mnemonicToRemove.Id);
                    await AuthService.UpdateUserWithMnemonic(username, mnemonicToRemove, false);

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
            }


            // Disposing timer for checking whether active user count updated
            _timer?.Dispose(); 
        }

        public async Task DisposeAsync()

        {
            // Perform async cleanup
            var userId = await JS.InvokeAsync<Guid>("sessionStorage.getItem", "userId");
            var activeUserDict = await ActiveUserClient.GetDictionary();
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
            var user = await AuthService.GetUsername();
            if (user != null) {
                var usersMnemonicsGuids = await AuthService.GetMnemonicsGuids(user);
                var usersMnemonics = await MnemonicService.GetMnemonicsByIdsAsync(usersMnemonicsGuids) ?? new List<MnemonicsType>();

                object boxedMnemonics = usersMnemonics;

                var fileName = "mnemonics.json";
                var jsonFormattedData = JsonSerializer.Serialize(boxedMnemonics, options: new JsonSerializerOptions { WriteIndented = true });

                await JS.InvokeVoidAsync("downloadFile", fileName, jsonFormattedData);
            }
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
