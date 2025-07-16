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

    private static long _idCount;
    private long _id;

    public Item()
    {
        _id = _idCount++;
    }

    public override bool Equals(object? obj)
    {
        if (obj == this) return true;
        if (obj is null || obj is not Item item) return false;
        return item._id == _id;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
