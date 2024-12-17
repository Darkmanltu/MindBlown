namespace MindBlown.Server.Models
{
public class AnsweredMnemonic
{
 public Guid AnsweredMnemonicId { get; set; } // Primary Key
    public Guid AnswerSessionId { get; set; } // Foreign Key to AnswerSession

    public Guid MnemonicId { get; set; } // Ties to a specific mnemonic
    public bool IsCorrect { get; set; } // Whether the answer was correct

    // Navigation property to parent AnswerSession
    public  AnswerSession AnswerSession { get; set; }
}
}