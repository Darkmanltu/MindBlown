    public record LastWrongAnswerRecord
    {
        public Guid Id { get; init; }
        public required string? textM { get; init; }
        public required string? textW { get; init; }
        public required string? wrongTextW { get; init; }
    }