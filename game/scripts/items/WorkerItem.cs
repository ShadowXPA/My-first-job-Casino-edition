namespace ProjectGJ.Scripts.Items;

public class WorkerItem : Item
{
    public required Profession Profession { get; set; }
    public override string PriceString() => $"{base.PriceString()}/30 days";
    public int DaysWorked { get; set; } = 1;
    public CasinoGameItem? Station { get; private set; }

    public void SetStation(CasinoGameItem? station)
    {
        Station = station;
    }
}
