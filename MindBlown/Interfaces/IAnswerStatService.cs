using System.Collections.Generic;
using System.Threading.Tasks;
using MindBlown.Types;

namespace MindBlown.Interfaces
{
    public interface IAnswerStatService
    {

        Task<List<AnswerSessionType>> GetList(string username);
        
        Task<bool> CreateAnswerSessionAsync(AnswerSessionType answerSession);
        
        Task<bool> AddAnsweredMnemonicAsync(AnsweredMnemonicType answeredMnemonic);
        
        Task<bool> AddAnswerSessionAsync(AnswerSessionType answerSession, ICollection<AnsweredMnemonicType> answerMnemonics);
    }
}