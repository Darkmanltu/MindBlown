using MindBlown.Types;

namespace MindBlown.Services
{
    public interface ILWARecordService
    {

        Task<LastWrongAnswerRecord?> GetRecordAsync(Guid id);
        
       Task<LastWrongAnswerRecord?> UpdateRecordAsync(Guid idToChange, LastWrongAnswerRecord record);
        
        Task LogErrorToServerAsync(string message, string details);
    }
}