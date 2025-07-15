using System.Collections.Generic;

public static class GameItems
{
    /**
    * TODO: Available characters for:
    *  - Player
    *  - Customers
    *  - Workers
    *    - Security
    *    - Game tables
    *    - Bar
    */

    public static readonly List<Item> ShopItems = [
        new Item() {
            Name = "Slot machine",
            Price = 1500,
            Resource = "res://assets/sprites/casino/slot_machine.png"
        },
        new Item() {
            Name = "Roulette",
            Price = 6000,
            Resource = "res://assets/sprites/casino/roulette.png"
        },
        new Item() {
            Name = "Poker",
            Price = 10000,
            Resource = "res://assets/sprites/casino/poker.png"
        }
    ];

    public static readonly List<Item> ShopStatueItems = [
        new Item() {
            Name = "Dice",
            Price = 15000,
            Resource = "res://assets/sprites/casino/statue1.png"
        }
    ];

    public static readonly List<StatueItem> Statues = [
        new StatueItem() {
            Name = "Dice",
            Description = "-5% Customer base win rate",
            Resource = "res://assets/sprites/casino/statue1.png"
        },
        new StatueItem() {
            Name = "Ace",
            Description = "-5% Customer base win rate",
            Resource = "res://assets/sprites/casino/statue2.png"
        },
        new StatueItem() {
            Name = "Club",
            Description = "+5% Customer addicts\n-5% Customer cheaters",
            Resource = "res://assets/sprites/casino/statue3.png"
        },
        new StatueItem() {
            Name = "Diamond",
            Description = "-5% Customer base win rate",
            Resource = "res://assets/sprites/casino/statue4.png"
        },
        new StatueItem() {
            Name = "Heart",
            Description = "-5% Customer base win rate",
            Resource = "res://assets/sprites/casino/statue5.png"
        }
    ];
}
