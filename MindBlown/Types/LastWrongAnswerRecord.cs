    public record LastWrongAnswerRecord
    {
        public Guid Id { get; init; }
        public required string? helperText { get; init; }
        public required string? mnemonicText { get; init; }
        public required string? wrongTextMnemonic { get; init; }
        public MnemonicCategory? category { get; init; }
    }