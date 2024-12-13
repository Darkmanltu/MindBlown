using MindBlown.Types;

namespace MindBlown.Interfaces;
public interface IAuthService
{
    Task<bool> IsUserLoggedInAsync();
    Task LogoutAsync();
    Task<string?> LoginAsync(AccRequest loginRequest);
    Task<string?> SignupAsync(AccRequest signupRequest);
    Task<MnemonicsType?> UpdateUserWithMnemonic(string? username, MnemonicsType newMnemonic, bool toAdd);
    Task<List<Guid>> GetMnemonicsGuids(string? username);
    Task<Guid> GetLWARecordId(string? username);
    Task<Guid?> UpdateLWARecord(string? username, Guid newId);
    Task<string?> GetUsername();
}