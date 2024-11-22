using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MindBlown.Interfaces;
using MindBlown.Types;
using Services;

namespace MindBlown.Pages
{
    public partial class TestMnemonic
    {
        [Inject]
        public required IMnemonicService MnemonicService { get; set; }


        public Repository<MnemonicsType> mnemonicsList = new Repository<MnemonicsType>();
        public string userGivenMnemonicText = "";

        // Initializes to 0 both values
        public AnsweringStatsStruct answeringStats = new AnsweringStatsStruct();

        public MnemonicsType? testingMnemonic;

        // LastWrongAnswerRecord is defined below
        public LastWrongAnswerRecord? lastWrongAnswer;

        public bool nextMnemonic = false;


        // Checks whether testingMnemonic is no longer null every 1s
        protected override async Task OnInitializedAsync()
        {
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

            
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await LoadMnemonics();
                lastWrongAnswer = await LWARecordService.GetRecordAsync();

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

                    await LWARecordService.UpdateRecordAsync(lastWrongAnswer);
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