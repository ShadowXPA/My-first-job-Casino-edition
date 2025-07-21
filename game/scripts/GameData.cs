using ProjectGJ.Scripts.Items;
using System.Collections.Generic;

namespace ProjectGJ.Scripts;

public class GameData
{
    public string CharacterResource { get; set; } = $"{Constants.CHARACTER_RESOURCE_BASE_PATH}/{Constants.DEFAULT_CHARACTER_RESOURCE}";
    public int ElapsedTime { get; set; }
    public int Money { get; set; } = 10000;
    public StatueItem? Statue { get; set; }
    public Inventory Inventory { get; set; } = new();
    public List<Transaction> Transactions { get; } = [];
}
