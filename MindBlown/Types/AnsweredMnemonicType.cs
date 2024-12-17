using MindBlown.Types;
namespace MindBlown.Types 
{
    public class AnsweredMnemonicType {



    public Guid AnsweredMnemonicId { get; set; } // Primary Key
    public Guid AnswerSessionId { get; set; } // Foreign Key to AnswerSession

    public Guid MnemonicId { get; set; } // Ties to a specific mnemonic
    public bool IsCorrect { get; set; } // Whether the answer was correct

    // Navigation property to parent AnswerSession
    public required AnswerSessionType AnswerSession { get; set; }
    }



}