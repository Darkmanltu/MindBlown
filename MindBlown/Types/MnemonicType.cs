public enum MnemonicCategory
{
    Chemistry,
    History,
    Math,
    Science,
    Geography,
    Physics,
    Biology,
    Astronomy,
    Literature,
    Language,
    Art,
    Music,
    Technology,
    Engineering,
    Medicine,
    Psychology,
    Philosophy,
    Sociology,
    Economics,
    Politics,
    Law,
    Business,
    Accounting,
    Marketing,
    Education,
    Architecture,
    ComputerScience,
    EnvironmentalScience,
    Agriculture,
    Sports,
    Health,
    Nutrition,
    Anthropology,
    Archaeology,
    Theology,
    Ethics,
    Logic,
    Linguistics,
    Zoology,
    Other
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
