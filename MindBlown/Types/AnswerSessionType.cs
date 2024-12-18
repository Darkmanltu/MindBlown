using MindBlown.Types;


namespace MindBlown.Types
{
    public class AnswerSessionType{
    public Guid AnswerSessionId { get; set; } // Primary Key
    public required string UserName { get; set; } // Tracks user name
    public DateTime LastAnswerTime { get; set; } // DateTime of the last answer
    public int CorrectCount { get; set; } // Number of correct answers
    public int IncorrectCount { get; set; } // Number of incorrect answers

    }
}