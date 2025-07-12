public static class MaterialExtensions
{
    public static string ToResourceString(this Material material)
    {
        return material switch
        {
            Material.Wood => "wood",
            Material.Stone => "stone",
            _ => ""
        };
    }
}
