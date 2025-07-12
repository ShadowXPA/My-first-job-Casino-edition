public static class OrientationExtensions
{
    public static string ToResourceString(this Orientation orientation)
    {
        return orientation switch
        {
            Orientation.North => "N",
            Orientation.East => "E",
            Orientation.South => "S",
            Orientation.West => "W",
            _ => ""
        };
    }
}
