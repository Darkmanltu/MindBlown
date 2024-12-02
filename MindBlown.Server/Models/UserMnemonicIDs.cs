namespace MindBlown.Server.Models
{
    public record UserMnemonicIDs
    {
        public Guid Id { get; init; }
        public required string Username { get; set; }
        public required string Password { get; set; }
        public required List<Guid> MnemonicGuids { get; set; }
        public required Guid LWARecordId { get; set; }
    }
}