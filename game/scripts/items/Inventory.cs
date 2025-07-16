using System.Collections.Generic;

namespace ProjectGJ.Scripts.Items;

public class Inventory
{
    public List<CasinoGameItem> CasinoGames { get; } = [];
    public List<WorkerItem> Workers { get; } = [];
    public List<StatueItem> Statues { get; } = [];
}
