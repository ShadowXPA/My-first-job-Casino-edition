using Godot;

namespace ProjectGJ.Scripts.Items;

public class Item
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public int Price { get; set; }
    public float PriceMultiplier { get; set; } = 1.0f;
    public int FinalPrice { get { return Mathf.FloorToInt(Price * PriceMultiplier); } }
    public string? Resource { get; set; }
    public virtual string PriceString() => $"${FinalPrice}";
}
