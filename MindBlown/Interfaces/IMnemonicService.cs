using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MindBlown.Types;

namespace MindBlown.Interfaces
{
    public interface IMnemonicService
    {
        Task<List<MnemonicsType>?> GetMnemonicsAsync();
        Task<MnemonicsType?> GetMnemonicAsync(Guid id);
        Task<MnemonicsType?> CreateMnemonicAsync(MnemonicsType mnemonic);
        Task<MnemonicsType?> UpdateMnemonicAsync(MnemonicsType mnemonic);
        Task DeleteMnemonicAsync(Guid id);
        Task LogErrorToServerAsync(string message, string details);
    }
}