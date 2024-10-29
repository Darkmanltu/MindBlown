using System;

namespace MindBlown.Server.Models
{
    public class Mnemonic {
        public Guid Id { get; init; }
        public string? HelperText { get; set; }
        public string? MnemonicText { get; set; }
        public MnemonicCategory Category { get; set; }
    }

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
}