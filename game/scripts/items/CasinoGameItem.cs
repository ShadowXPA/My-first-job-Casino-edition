namespace ProjectGJ.Scripts.Items;

public class CasinoGameItem : Item
{
    public int Slots { get; set; }
    public int OccupiedSlots { get; set; }
    public bool HasAvailableSlots() => Slots - OccupiedSlots > 0;
    public WorkerItem? Worker { get; private set; }

    public void SetWorker(WorkerItem? worker)
    {
        Worker?.SetStation(null);
        Worker = worker;
        Worker?.SetStation(this);
    }

    public CasinoGameItem Clone()
    {
        return new CasinoGameItem()
        {
            Name = Name,
            Description = Description,
            PriceMultiplier = PriceMultiplier,
            OccupiedSlots = OccupiedSlots,
            Price = Price,
            Resource = Resource,
            Slots = Slots,
            Worker = Worker,
        };
    }
}
