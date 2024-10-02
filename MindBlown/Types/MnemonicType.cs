public enum MnemonicCategory
{
    Acronym,
    Rhyme,
    StoryMethod,
}
public class MnemonicsType : IEquatable<MnemonicsType>
{
    public Guid Id { get; init; }
    public string? TextM { get; set; }
    public string? TextW { get; set; }

    public MnemonicCategory Category { get; set; }

    // public bool Equals(MnemonicsType? mnemonic)
    // {
    //     return (this.Id, this.TextM, this.TextW, this.Category) ==
    //         (mnemonic?.Id, mnemonic?.TextM, mnemonic?.TextW, mnemonic?.Category);
    // } 

    public MnemonicsType() { }

    public MnemonicsType(String newTextW)
    {
        this.TextW = newTextW;
    }

    public bool Equals(MnemonicsType? mnemonic)
    {
        return (this.TextW) == (mnemonic?.TextW);
    }

}
