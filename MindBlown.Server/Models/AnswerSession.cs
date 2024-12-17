namespace MindBlown.Server.Models
{
public class AnswerSession
{
    public Guid AnswerSessionId { get; set; } // Primary Key
    public required string UserName { get; set; } // Tracks user name
    public DateTime LastAnswerTime { get; set; } // DateTime of the last answer
    public int CorrectCount { get; set; } // Number of correct answers
    public int IncorrectCount { get; set; } // Number of incorrect answers

    // Navigation property for related AnsweredMnemonics
    public ICollection<AnsweredMnemonic> AnsweredMnemonics { get; set; } = new List<AnsweredMnemonic>();
}
}
