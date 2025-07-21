using System.Collections.Generic;
using ProjectGJ.Scripts.Items;

namespace ProjectGJ.Scripts;

public static class GameItems
{
    public static readonly List<CasinoGameItem> CasinoGames = [
        new() {
            Name = "Slot machine",
            // Description = "",
            Price = 1500,
            Resource = $"{Constants.ASSET_SPRITE_CASINO_BASE_PATH}/slot_machine.png",
        },
        new() {
            Name = "Roulette",
            Description = "Requires a dealer",
            Price = 6000,
            Resource = $"{Constants.ASSET_SPRITE_CASINO_BASE_PATH}/roulette.png",
        },
        new() {
            Name = "Blackjack",
            Description = "Requires a dealer",
            Price = 10000,
            Resource = $"{Constants.ASSET_SPRITE_CASINO_BASE_PATH}/blackjack.png",
        },
    ];

    public static readonly List<StatueItem> Statues = [
        new() {
            Name = "Dice",
            Description = "+10% Customer average time spent",
            Price = 50000,
            Resource = $"{Constants.ASSET_SPRITE_CASINO_BASE_PATH}/statue1.png",
            CustomerAvgTimeSpentMultiplier = 1.0f + 0.1f,
        },
        new() {
            Name = "Ace",
            Description = "+10% Customer addicts\n-5% Customer cheaters",
            Price = 52000,
            Resource = $"{Constants.ASSET_SPRITE_CASINO_BASE_PATH}/statue2.png",
            CustomerAddictsMultiplier = 1.0f + 0.1f,
            CustomerCheatersMultiplier = 1.0f - 0.05f,
        },
        new() {
            Name = "Club",
            Description = "-25% Shop prices",
            Price = 55000,
            Resource = $"{Constants.ASSET_SPRITE_CASINO_BASE_PATH}/statue3.png",
            ShopPricesMultiplier = 1.0f - 0.25f,
        },
        new() {
            Name = "Diamond",
            Description = "-50% Chance of a machine breaking",
            Price = 60000,
            Resource = $"{Constants.ASSET_SPRITE_CASINO_BASE_PATH}/statue4.png",
            ChanceMachineBreakMultiplier = 1.0f - 0.5f,
        },
        new() {
            Name = "Heart",
            Description = "-20% Customer base win rate",
            Price = 70000,
            Resource = $"{Constants.ASSET_SPRITE_CASINO_BASE_PATH}/statue5.png",
            CustomerBaseWinRateMultiplier = 1.0f - 0.2f,
        },
    ];

    public static readonly Dictionary<(Gender, Profession), List<string>> AvailableCharacters = new()
    {
        { (Gender.Unknown, Profession.Player), [
            "character_34.res", "character_35.res", "character_36.res", "character_37.res",
            "character_38.res", "character_39.res",
        ] },
        { (Gender.Male, Profession.Player), [
            "character_1.res", "character_2.res", "character_3.res", "character_4.res",
            "character_8.res", "character_16.res", "character_18.res", "character_19.res",
            "character_20.res", "character_22.res", "character_24.res", "character_25.res",
            "character_28.res", "character_32.res",
        ] },
        { (Gender.Female, Profession.Player), [
            "character_0.res", "character_12.res", "character_13.res", "character_14.res",
            "character_23.res", "character_27.res", "character_33.res",
        ]},
        { (Gender.Male, Profession.Customer), [
            "character_1.res", "character_2.res", "character_3.res", "character_4.res",
            "character_8.res", "character_16.res", "character_18.res", "character_19.res",
            "character_20.res", "character_22.res", "character_24.res", "character_25.res",
            "character_28.res", "character_30.res", "character_32.res",
        ] },
        { (Gender.Female, Profession.Customer), [
            "character_0.res", "character_12.res", "character_13.res", "character_14.res",
            "character_23.res", "character_27.res", "character_31.res", "character_33.res",
        ] },
        { (Gender.Male, Profession.Security), [ "character_5.res", "character_6.res", "character_7.res", ] },
        { (Gender.Female, Profession.Security), [ "character_26.res", ] },
        { (Gender.Male, Profession.Bartender), [ "character_21.res", ] },
        { (Gender.Female, Profession.Bartender), [ "character_17.res", ] },
        { (Gender.Male, Profession.Dealer), [ "character_9.res", "character_11.res", ] },
        { (Gender.Female, Profession.Dealer), [ "character_10.res", "character_15.res", ] },
    };

    public static Dictionary<Gender, List<string>> Names = new()
    {
        { Gender.Male, [ "Elliot", "Branden", "Linwood", "Hugo", "Archer", "Rolland", "Garret", "John", "Paul", "Phillis", "Silas", "Jeffrey", "Jordan", "Dixon", "Derrick", "Russel", "Cooper", "Lee", "Bryan", "David", "Jefferson" ] },
        { Gender.Female, [ "Nena", "Denice", "Brianna", "Loraine", "Abi", "Shanna", "Samantha", "Lynne", "Janna", "Annette", "Lizette", "Eleanor" ] },
        { Gender.Unknown, [ "Chanda", "Meztli", "Fedlimid", "Khordad", "Feidlimid", "Sushila" ] },
    };

    public static List<string> Surnames = [
        "Fiddler", "Coleman", "Dawson", "Mathewson", "Horton", "Moss", "Barr", "Hathway",
        "Evelyn", "Baron", "Lum", "Godfrey", "Knowles", "Stone", "Hollins",
        "Rowe", "Wayne", "Hall", "Darnell", "Ayers", "Spurling", "Gates",
        "Haden", "Michaelson", "Peck"
    ];
}
