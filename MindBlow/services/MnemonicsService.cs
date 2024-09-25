using System.Text.Json;
using System.IO;

public class MnemonicsService
{
    public readonly string _filePath = "/mnemonics.json";
    public async Task<List<MnemonicsType>> GetMnemonicsAsync()
    {
        if (!File.Exists(_filePath))
        {
            return new List<MnemonicsType>();
        }

        var existingData = await File.ReadAllTextAsync(_filePath);
        return JsonSerializer.Deserialize<List<MnemonicsType>>(existingData) ?? new List<MnemonicsType>();
    }

    public async Task SaveMnemonicsAsync(MnemonicsType model)
    {
        var mnemonicsList = await GetMnemonicsAsync();
        mnemonicsList.Add(model);
        
        var updatedJson = JsonSerializer.Serialize(mnemonicsList, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(_filePath, updatedJson);
    }
}

public class MnemonicsType
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string? TextM { get; set; }
    public string? TextW { get; set; }
}
