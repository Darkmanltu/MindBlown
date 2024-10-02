    public record LastWrongAnswerRecord
    {
        public Guid Id { get; set; }
        public required string? textM { get; set; }
        public required string? textW { get; set; }
        public required string? wrongTextW { get; set; }
    }