using Godot;
using System;
using System.Collections.Generic;

public class GameData
{
    public string Character { get; set; } = "res://characters/character_0.res";
    public int ElapsedTime { get; set; }
    public int Money { get; set; } = 10000;
    public StatueItem? Statue { get; set; }
    public float BaseWinRate { get; set; } = .25f;
    // public List<Item> Inventory { get; } = [];
    // public List<int> Transactions { get; } = [];
}
