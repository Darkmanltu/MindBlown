﻿using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MindBlown.Interfaces;
using MindBlown.Types;
using Services;
using Microsoft.JSInterop;
using MindBlown.Services;

namespace MindBlown.Pages
{
    public partial class TestMnemonic:IDisposable
    {

        // injecting for counter
        [Inject]
        public IActiveUserClient ActiveUserClient { get; set; }
        [Inject]
        public IMnemonicService MnemonicService { get; set; }
        [Inject]
        public IAuthService AuthService { get; set; }
        [Inject]
        public ILWARecordService LWARecordService { get; set; }
        [Inject]
        public IJSRuntime JS { get; set; }
        
        
        public Guid userId { get; set; }
        public int ActiveUserCount {get; set;}
        public Repository<MnemonicsType> mnemonicsList = new Repository<MnemonicsType>();
        public string userGivenMnemonicText = "";

        // Initializes to 0 both values
        public AnsweringStatsStruct answeringStats = new AnsweringStatsStruct();

        public MnemonicsType? testingMnemonic;

        // LastWrongAnswerRecord is defined below
        public LastWrongAnswerRecord? lastWrongAnswer;

        public bool nextMnemonic = false;

        private Timer? _timer;

        // Checks whether testingMnemonic is no longer null every 1s
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
            userId = await JS.InvokeAsync<Guid>("sessionStorage.getItem", "userId");


            // Console.WriteLine("User ID: " + userId);
            // Add the user to ActiveUserClient
            bool isUnique = await ActiveUserClient.IsSessionIdUniqueAsync(userId);


            if (isUnique)
            {
                // Add the user to ActiveUserClient only if the sessionId is unique
                await ActiveUserClient.AddUserAsync(userId);

                // Update the active user count

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

            while (testingMnemonic == null)
            {
                await Task.Delay(50);
            }

            // testingMnemonic is no longer null
        }

        /*
        Loads mnemonicsList and gets random mnemonic from mnemonicsList after going into this website.
        Also updates what mnemonic is being tested now and shows it on website.
        */

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

            
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await JS.InvokeVoidAsync("detectTabCloseF", DotNetObjectReference.Create(this));
                await LoadMnemonics();
                var username = await AuthService.GetUsername();
                var recordId = await AuthService.GetLWARecordId(username);
                lastWrongAnswer = await LWARecordService.GetRecordAsync(recordId);

                if (mnemonicsList.Count() != 0)
                {
                    getRandomMnemonic();
                }
            }

            if (nextMnemonic)
            {
                nextMnemonic = false;
                getRandomMnemonic();
                StateHasChanged();
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
            // Console.WriteLine($"Active user recounter: {activeUserCountCheck}");

            // Update the state if the count has changed
            if (ActiveUserCount != activeUserCountCheck)
            {
                ActiveUserCount = activeUserCountCheck;
                StateHasChanged(); // Trigger re-render
            }
        }

        // Loading from local storage
        public async Task LoadMnemonics()
        {
            // Load the mnemonics from database
            mnemonicsList = new Repository<MnemonicsType>(await MnemonicService.GetMnemonicsAsync() ?? new List<MnemonicsType>());
            if(await AuthService.IsUserLoggedInAsync())
            {
                var username = await AuthService.GetUsername();
                mnemonicsList = new Repository<MnemonicsType>(await MnemonicService.GetMnemonicsByIdsAsync(await AuthService.GetMnemonicsGuids(username)) ?? new List<MnemonicsType>());
            }
        }

        // Task when Check button is pressed
        public async Task CheckMnemonic()
        {
            object userMnemonic = new MnemonicsType(newMnemonicText: userGivenMnemonicText);

            if (testingMnemonic != null && testingMnemonic.Equals((MnemonicsType)userMnemonic))
            {
                answeringStats.correctAnswerCount++;
            }
            else
            {
                // Use record for last wrong answered mnemonic
                if (testingMnemonic?.HelperText != null && testingMnemonic?.MnemonicText != null )
                {
                    lastWrongAnswer = new LastWrongAnswerRecord
                    {
                        Id = testingMnemonic.Id,
                        helperText = testingMnemonic?.HelperText,
                        mnemonicText =
                        testingMnemonic?.MnemonicText,
                        wrongTextMnemonic = userGivenMnemonicText,
                        category = testingMnemonic?.Category
                    };

                    var username = await AuthService.GetUsername();
                    var oldRecordId = await AuthService.GetLWARecordId(username);
                    var newRecord = await LWARecordService.UpdateRecordAsync(oldRecordId, lastWrongAnswer);
                    if(newRecord != null)
                        await AuthService.UpdateLWARecord(username, newRecord.Id);
                    // else: Error occured when updating record
                }
            }
            nextMnemonic = true;

            answeringStats.allAnswerCount++;
            userGivenMnemonicText = string.Empty;
            StateHasChanged();
            await Task.Delay(1); // Fixes the warning (async methods must have await inside them)
        }


        public MnemonicsType getRandomMnemonic()
        {
            System.Random random = new System.Random();
            int randomNumber = random.Next(0, mnemonicsList.Count());
            MnemonicsType randomMnemonic = mnemonicsList[randomNumber];
            return testingMnemonic = randomMnemonic;
        }
        public async Task Enter(KeyboardEventArgs e)
        {
            if (e.Key == "Enter" || e.Key == "NumpadEnter")
            {
                await CheckMnemonic();
            }
        }


    }
}
