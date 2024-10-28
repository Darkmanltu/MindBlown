using Microsoft.AspNetCore.Components.Web;
using MindBlown.Types;

namespace MindBlown.Pages
{
    public partial class TestMnemonic
    {

        private Repository<MnemonicsType> mnemonicsList = new Repository<MnemonicsType>();
        private string userGivenMnemonicText = "";

        // Initializes to 0 both values
        public AnsweringStatsStruct answeringStats = new AnsweringStatsStruct();

        private MnemonicsType? testingMnemonic;

        //LastWrongAnswerRecord is defined below
        public LastWrongAnswerRecord? lastWrongAnswer;

        private bool nextMnemonic = false;


        //Checks whether testingMnemonic is no longer null every 1s
        protected override async Task OnInitializedAsync()
        {
            while (testingMnemonic == null)
            {
                await Task.Delay(50);
            }

            //testingMnemonic is no longer null
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
                if (mnemonicsList.Count() != 0)
                {
                    // System.Console.WriteLine("new random mnemonic: " + getRandomMnemonic().HelperText);
                    getRandomMnemonic();
                }
            }

            if (nextMnemonic)
            {
                nextMnemonic = false;
                // System.Console.WriteLine("new random mnemonic: " + getRandomMnemonic().HelperText + " " + nextMnemonic);
                getRandomMnemonic();
                StateHasChanged();
            }
        }


        //Loading from local storage
        private async Task LoadMnemonics()
        {
            //Load the mnemonics from local storage
            mnemonicsList = new Repository<MnemonicsType>(await localStorage.GetItemAsync<List<MnemonicsType>>("userMnemonics") ?? new List<MnemonicsType>());
        }

        //Task when Check button is pressed
        private async Task CheckMnemonic()
        {

            object userMnemonic = new MnemonicsType(newMnemonicText: userGivenMnemonicText);

            // @* userMnemonic.TextW = userGivenTextW; *@
            // @* if(testingMnemonic?.TextW == userGivenTextW && testingMnemonic != null) *@
            if (testingMnemonic != null && testingMnemonic.Equals((MnemonicsType)userMnemonic))
            {
                answeringStats.correctAnswerCount++;
            }
            else
            {
                //Use record for last wrong answered mnemonic
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
                }
            }
            nextMnemonic = true;

            answeringStats.allAnswerCount++;
            userGivenMnemonicText = string.Empty;
            // userGivenTextW = "debug_123";
            StateHasChanged();
            await Task.Delay(1); // Fixes the warning (asyn methods must have await inside them)
        }


        private MnemonicsType getRandomMnemonic()
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
