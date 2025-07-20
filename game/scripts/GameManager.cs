using Godot;
using ProjectGJ.Characters.Customer;
using ProjectGJ.Characters.Player;
using ProjectGJ.Characters.Worker;
using ProjectGJ.Props.Roulette;
using ProjectGJ.Props.Security;
using ProjectGJ.Props.Slots;
using ProjectGJ.Scripts.Items;
using ProjectGJ.Ui.Hud;
using System.Linq;

namespace ProjectGJ.Scripts;

public partial class GameManager : Node
{
    [Export]
    public Timer? GameTimer;
    [Export]
    public Player? Player;
    [Export]
    public Node? CustomerContainer;
    [Export]
    public Node? WorkerContainer;
    [Export]
    public Hud? Hud;
    [Export]
    public PackedScene? WorkerScene;
    [Export]
    public PackedScene? CustomerScene;

    private GameData _gameData = new();

    public override void _Ready()
    {
        SignalBus.BroadcastGameTimeChanged(_gameData.ElapsedTime);
        Hud?.SetMoney(_gameData.Money);

        if (GameTimer is not null)
        {
            GameTimer.Timeout += OnGameTimerTicked;
        }

        SignalBus.PlayerMoneyTransaction += OnPlayerMoneyTransaction;
        SignalBus.PlayerBuyingCasinoGame += OnPlayerBuyingCasinoGame;
        SignalBus.PlayerHiringWorker += OnPlayerHiringWorker;
        SignalBus.PlayerBuyingStatue += OnPlayerBuyingStatue;
        SignalBus.PlayerBoughtCasinoGame += OnPlayerBoughtCasinoGame;
        SignalBus.PlayerHiredWorker += OnPlayerHiredWorker;
        SignalBus.PlayerBoughtStatue += OnPlayerBoughtStatue;
        SignalBus.PlayerSoldCasinoGame += OnPlayerSoldCasinoGame;
        SignalBus.PlayerFiredWorker += OnPlayerFiredWorker;
        SignalBus.PlayerRepairingSlots += OnPlayerRepairingSlots;
        SignalBus.CustomerGamblingSlots += OnCustomerGamblingSlots;
        SignalBus.CustomerGamblingRoulette += OnCustomerGambling;
        SignalBus.CustomerGamblingBlackjack += OnCustomerGambling;
        SignalBus.CustomerDrinking += OnCustomerDrinking;

        Player?.SetCharacterResource(_gameData.CharacterResource);
    }

    public override void _ExitTree()
    {
        if (GameTimer is not null)
        {
            GameTimer.Timeout -= OnGameTimerTicked;
        }

        SignalBus.PlayerMoneyTransaction -= OnPlayerMoneyTransaction;
        SignalBus.PlayerBuyingCasinoGame -= OnPlayerBuyingCasinoGame;
        SignalBus.PlayerHiringWorker -= OnPlayerHiringWorker;
        SignalBus.PlayerBuyingStatue -= OnPlayerBuyingStatue;
        SignalBus.PlayerBoughtCasinoGame -= OnPlayerBoughtCasinoGame;
        SignalBus.PlayerHiredWorker -= OnPlayerHiredWorker;
        SignalBus.PlayerBoughtStatue -= OnPlayerBoughtStatue;
        SignalBus.PlayerSoldCasinoGame -= OnPlayerSoldCasinoGame;
        SignalBus.PlayerFiredWorker -= OnPlayerFiredWorker;
        SignalBus.PlayerRepairingSlots -= OnPlayerRepairingSlots;
        SignalBus.CustomerGamblingSlots -= OnCustomerGamblingSlots;
        SignalBus.CustomerGamblingRoulette -= OnCustomerGambling;
        SignalBus.CustomerGamblingBlackjack -= OnCustomerGambling;
        SignalBus.CustomerDrinking -= OnCustomerDrinking;
    }

    public void LoadGame()
    {
    }

    public void SaveGame()
    {
    }

    private void OnGameTimerTicked()
    {
        _gameData.ElapsedTime++;
        SignalBus.BroadcastGameTimeChanged(_gameData.ElapsedTime);

        var (hour, minutes) = Utils.GetHoursAndMinutes(_gameData.ElapsedTime);
        var days = Utils.GetDays(_gameData.ElapsedTime);

        var machineBreakMultiplier = _gameData.Inventory.Statues
            .Where(statue => statue.ChanceMachineBreakMultiplier is not null)
            .Aggregate(1.0f, (previousValue, statue) => previousValue * (float)statue.ChanceMachineBreakMultiplier!);
        var machineBreakRate = Constants.SLOT_MACHINE_BREAK_RATE * machineBreakMultiplier;

        if (GD.Randf() < machineBreakRate)
        {
            var brokenSlots = (Slots)GetTree().GetNodesInGroup("slots").PickRandom();
            brokenSlots.Break();
        }

        if (minutes % 15 == 0)
        {
            if (CustomerScene is not null && CustomerContainer is not null && CustomerContainer.GetChildCount() < Constants.MAX_NUMBER_CUSTOMERS)
            {
                var cheaterMultiplier = _gameData.Inventory.Statues
                    .Where(statue => statue.CustomerCheatersMultiplier is not null)
                    .Aggregate(1.0f, (previousValue, statue) => previousValue * (float)statue.CustomerCheatersMultiplier!);
                var securityMultiplier = _gameData.Inventory.Workers
                    .Where(worker => worker.Profession == Profession.Security)
                    .Aggregate(1.0f, (previous, worker) => previous * Constants.SECURITY_MULTIPLIER);
                cheaterMultiplier *= securityMultiplier;
                var addictMultiplier = _gameData.Inventory.Statues
                    .Where(statue => statue.CustomerAddictsMultiplier is not null)
                    .Aggregate(1.0f, (previousValue, statue) => previousValue * (float)statue.CustomerAddictsMultiplier!);
                var averageTimeMultiplier = _gameData.Inventory.Statues
                    .Where(statue => statue.CustomerAvgTimeSpentMultiplier is not null)
                    .Aggregate(1.0f, (previousValue, statue) => previousValue * (float)statue.CustomerAvgTimeSpentMultiplier!);

                var customerItem = Utils.GenerateRandomCustomer(_gameData.CharacterResource,
                    cheaterMultiplier: cheaterMultiplier, addictMultiplier: addictMultiplier, averageTimeMultiplier: averageTimeMultiplier);
                var customer = CustomerScene.Instantiate<Customer>();
                CustomerContainer.AddChild(customer);
                customer.SetCustomerItem(customerItem);
                customer.GlobalPosition = ((Node2D)GetTree().GetNodesInGroup("customer_spawner").PickRandom()).GlobalPosition;
                customer.NextActivity();
            }
        }

        if (minutes % 60 == 0)
        {
            if (hour % 12 == 0)
            {
                SignalBus.BroadcastRefreshShops();
            }

            if (hour % 24 == 0)
            {
                foreach (var worker in _gameData.Inventory.Workers)
                {
                    worker.DaysWorked++;
                }

                SignalBus.BroadcastNewDay();

                if (days % 30 == 0)
                {
                    var salaries = 0;

                    foreach (var worker in _gameData.Inventory.Workers)
                    {
                        salaries += Mathf.FloorToInt(worker.FinalPrice / 30 * Mathf.Min(worker.DaysWorked, 30));
                    }

                    var casinoRent = Constants.CASINO_RENT;
                    var maintenanceFees = Constants.MAINTENANCE_FEES;

                    var payments = salaries + casinoRent + maintenanceFees;

                    var lastTransactions = _gameData.Transactions.Where(transaction => transaction.Day > (days - 30));
                    var profits = lastTransactions.Where(transaction => transaction.TransactionAmount > 0)
                        .Sum(transaction => transaction.TransactionAmount);
                    var losses = lastTransactions.Where(transaction => transaction.TransactionAmount < 0)
                        .Sum(transaction => transaction.TransactionAmount) + payments;

                    var realProfit = profits - losses;
                    var taxes = realProfit <= 0 ? 0 : Mathf.FloorToInt(realProfit * Constants.TAX_RATE);

                    var expenses = -(payments + taxes);

                    var transaction = new Transaction()
                    {
                        Description = "Monthly expenses including: salaries, taxes, rent, other fees",
                    };
                    transaction.AmountBeforeTransaction = _gameData.Money;
                    _gameData.Money += expenses;
                    transaction.AmountAfterTransaction = _gameData.Money;
                    transaction.TransactionAmount = expenses;
                    SignalBus.BroadcastPlayerMoneyTransaction(transaction);
                    SignalBus.BroadcastNotifyPlayer("30 days have passed, you have paid your expenses.");
                }
            }
        }
    }

    private void OnPlayerMoneyTransaction(Transaction transaction)
    {
        transaction.Day = Utils.GetDays(_gameData.ElapsedTime);
        _gameData.Transactions.Add(transaction);
        SignalBus.BroadcastTransactionComplete(transaction);
    }

    private void OnPlayerBuyingCasinoGame(CasinoGameItem casinoGame)
    {
        if (_gameData.Money < casinoGame.FinalPrice)
        {
            SignalBus.BroadcastNotifyPlayer($"You cannot afford to buy this casino game: {casinoGame.Name}");
            return;
        }

        var transaction = new Transaction()
        {
            Description = $"Bought casino game: {casinoGame.Name}",
        };

        transaction.AmountBeforeTransaction = _gameData.Money;
        _gameData.Money += -casinoGame.FinalPrice;
        transaction.AmountAfterTransaction = _gameData.Money;
        transaction.TransactionAmount = -casinoGame.FinalPrice;

        SignalBus.BroadcastPlayerBoughtCasinoGame(casinoGame.Clone());
        SignalBus.BroadcastPlayerMoneyTransaction(transaction);
    }

    private void OnPlayerBoughtCasinoGame(CasinoGameItem casinoGame)
    {
        _gameData.Inventory.CasinoGames.Add(casinoGame);
    }

    private void OnPlayerHiringWorker(WorkerItem worker)
    {
        if (WorkerScene is null || WorkerContainer is null) return;

        if (_gameData.Money < worker.FinalPrice)
        {
            SignalBus.BroadcastNotifyPlayer($"You cannot afford to hire this person: {worker.Name}");
            return;
        }

        switch (worker.Profession)
        {
            case Profession.Security:
                {
                    var guardPosts = GetTree().GetNodesInGroup("security").Cast<GuardPost>();
                    var emptyGuardPosts = guardPosts.Where(guardPost => guardPost.Worker is null).ToList();

                    if (emptyGuardPosts.Count == 0)
                    {
                        SignalBus.BroadcastNotifyPlayer($"You cannot hire more {worker.Profession} workers");
                        return;
                    }

                    var guardPost = emptyGuardPosts[GD.RandRange(0, emptyGuardPosts.Count - 1)];
                    var staff = WorkerScene.Instantiate<Worker>();
                    WorkerContainer.AddChild(staff);
                    staff.SetWorker(worker);
                    guardPost.SetWorker(staff);
                    staff.GlobalPosition = guardPost.GlobalPosition;
                    break;
                }
            case Profession.Bartender:
                {
                    var bars = GetTree().GetNodesInGroup("bar").Cast<WorkerStation>();
                    var emptyBars = bars.Where(bar => bar.Worker is null).ToList();

                    if (emptyBars.Count == 0)
                    {
                        SignalBus.BroadcastNotifyPlayer($"You cannot hire more {worker.Profession} workers");
                        return;
                    }

                    var bar = emptyBars[GD.RandRange(0, emptyBars.Count - 1)];
                    var staff = WorkerScene.Instantiate<Worker>();
                    WorkerContainer.AddChild(staff);
                    staff.SetWorker(worker);
                    bar.SetWorker(staff);
                    staff.GlobalPosition = bar.WorkerSpawner!.GlobalPosition;
                    break;
                }
            case Profession.Dealer:
                {
                    var tables = GetTree().GetNodesInGroup("dealer").Cast<WorkerStation>();
                    var emptyTables = tables.Where(table => table.Worker is null).ToList();

                    if (emptyTables.Count == 0)
                    {
                        SignalBus.BroadcastNotifyPlayer($"You cannot hire more {worker.Profession} workers");
                        return;
                    }

                    var table = emptyTables[GD.RandRange(0, emptyTables.Count - 1)];
                    var staff = WorkerScene.Instantiate<Worker>();
                    WorkerContainer.AddChild(staff);
                    staff.SetWorker(worker);
                    table.SetWorker(staff);
                    staff.GlobalPosition = table.WorkerSpawner!.GlobalPosition;
                    break;
                }
            default:
                return;
        }

        SignalBus.BroadcastPlayerHiredWorker(worker);
    }

    private void OnPlayerHiredWorker(WorkerItem worker)
    {
        _gameData.Inventory.Workers.Add(worker);
    }

    private void OnPlayerBuyingStatue(StatueItem statue)
    {
        if (_gameData.Money < statue.FinalPrice)
        {
            SignalBus.BroadcastNotifyPlayer($"You cannot afford to buy this statue: {statue.Name}");
            return;
        }

        var transaction = new Transaction()
        {
            Description = $"Bought statue: {statue.Name}",
        };

        transaction.AmountBeforeTransaction = _gameData.Money;
        _gameData.Money += -statue.FinalPrice;
        transaction.AmountAfterTransaction = _gameData.Money;
        transaction.TransactionAmount = -statue.FinalPrice;

        SignalBus.BroadcastPlayerBoughtStatue(statue);
        SignalBus.BroadcastPlayerMoneyTransaction(transaction);
    }

    private void OnPlayerBoughtStatue(StatueItem statue)
    {
        _gameData.Inventory.Statues.Add(statue);
        SignalBus.BroadcastNotifyPlayer($"Got permanent buff: {statue.Description}");
    }

    private void OnPlayerSoldCasinoGame(CasinoGameItem casinoGame)
    {
        var sellValue = Mathf.FloorToInt(casinoGame.FinalPrice * Constants.SELL_PERCENTAGE);

        _gameData.Inventory.CasinoGames.Remove(casinoGame);

        var transaction = new Transaction()
        {
            Description = $"Sold casino game: {casinoGame.Name}",
        };

        transaction.AmountBeforeTransaction = _gameData.Money;
        _gameData.Money += sellValue;
        transaction.AmountAfterTransaction = _gameData.Money;
        transaction.TransactionAmount = sellValue;

        SignalBus.BroadcastPlayerMoneyTransaction(transaction);
    }

    private void OnPlayerFiredWorker(WorkerItem worker)
    {
        var sellValue = -Mathf.FloorToInt(worker.FinalPrice / 30 * (worker.DaysWorked % 30));

        switch (worker.Profession)
        {
            case Profession.Security:
                {
                    var guardPosts = GetTree().GetNodesInGroup("security").Cast<GuardPost>();
                    var occupiedGuardPosts = guardPosts.Where(guardPost => guardPost.Worker is not null).ToList();

                    if (occupiedGuardPosts.Count == 0)
                    {
                        return;
                    }

                    foreach (var guardPost in occupiedGuardPosts)
                    {
                        if (guardPost.Worker?.WorkerItem == worker)
                        {
                            var staff = guardPost.Worker;
                            guardPost.SetWorker(null);
                            staff.QueueFree();
                            break;
                        }
                    }
                    break;
                }
            case Profession.Bartender:
                {
                    var bars = GetTree().GetNodesInGroup("bar").Cast<WorkerStation>();
                    var occupiedBars = bars.Where(bar => bar.Worker is not null).ToList();

                    if (occupiedBars.Count == 0)
                    {
                        return;
                    }

                    foreach (var bar in occupiedBars)
                    {
                        if (bar.Worker?.WorkerItem == worker)
                        {
                            var staff = bar.Worker;
                            bar.SetWorker(null);
                            staff.QueueFree();
                            break;
                        }
                    }
                    break;
                }
            case Profession.Dealer:
                {
                    var tables = GetTree().GetNodesInGroup("dealer").Cast<WorkerStation>();
                    var occupiedTables = tables.Where(bar => bar.Worker is not null).ToList();

                    if (occupiedTables.Count == 0)
                    {
                        return;
                    }

                    foreach (var table in occupiedTables)
                    {
                        if (table.Worker?.WorkerItem == worker)
                        {
                            var staff = table.Worker;
                            table.SetWorker(null);
                            staff.QueueFree();
                            break;
                        }
                    }
                    break;
                }
            default:
                return;
        }

        worker.Station?.SetWorker(null);
        _gameData.Inventory.Workers.Remove(worker);

        var transaction = new Transaction()
        {
            Description = $"Fired worker: {worker.Name}, after having worked for: {worker.DaysWorked} days",
        };

        transaction.AmountBeforeTransaction = _gameData.Money;
        _gameData.Money += sellValue;
        transaction.AmountAfterTransaction = _gameData.Money;
        transaction.TransactionAmount = sellValue;

        SignalBus.BroadcastPlayerMoneyTransaction(transaction);
    }

    private void OnPlayerRepairingSlots(Slots slots)
    {
        var repairCost = Constants.SLOT_MACHINE_REPAIR_FEE;

        if (_gameData.Money < repairCost)
        {
            SignalBus.BroadcastNotifyPlayer("You cannot afford to repair this slot machine");
            return;
        }

        var transaction = new Transaction()
        {
            Description = "Slot machine repair fee"
        };

        transaction.AmountBeforeTransaction = _gameData.Money;
        _gameData.Money -= repairCost;
        transaction.AmountAfterTransaction = _gameData.Money;
        transaction.TransactionAmount = -repairCost;

        slots.Repair();

        SignalBus.BroadcastPlayerMoneyTransaction(transaction);
    }

    private void OnCustomerGamblingSlots(Slots slots, Customer customer)
    {
        var baseWinMultiplier = _gameData.Inventory.Statues
                    .Where(statue => statue.CustomerBaseWinRateMultiplier is not null)
                    .Aggregate(1.0f, (previousValue, statue) => previousValue * (float)statue.CustomerBaseWinRateMultiplier!);
        var winChance = baseWinMultiplier * Constants.CUSTOMER_BASE_WIN_RATE + customer.CustomerItem!.BonusWinRate;
        var winRandom = GD.Randf();
        var moneyWonOrLost = GD.RandRange(Constants.MIN_MONEY_WON_OR_LOST, Constants.MAX_MONEY_WON_OR_LOST);

        if (winRandom < winChance)
        {
            var transaction = new Transaction()
            {
                Description = $"Customer {customer.CustomerItem.Name} won the gamble at slots"
            };

            transaction.AmountBeforeTransaction = _gameData.Money;
            _gameData.Money -= moneyWonOrLost;
            transaction.AmountAfterTransaction = _gameData.Money;
            transaction.TransactionAmount = -moneyWonOrLost;

            SignalBus.BroadcastPlayerMoneyTransaction(transaction);

            customer.AddTransaction(moneyWonOrLost);
            slots.Win();
        }
        else
        {
            var transaction = new Transaction()
            {
                Description = $"Customer {customer.CustomerItem.Name} lost the gamble at slots"
            };

            transaction.AmountBeforeTransaction = _gameData.Money;
            _gameData.Money += moneyWonOrLost;
            transaction.AmountAfterTransaction = _gameData.Money;
            transaction.TransactionAmount = moneyWonOrLost;

            SignalBus.BroadcastPlayerMoneyTransaction(transaction);

            customer.AddTransaction(-moneyWonOrLost);
            slots.Lose();
        }
    }

    private void OnCustomerDrinking(Customer customer)
    {
        var moneySpent = GD.RandRange(Constants.MIN_MONEY_WON_OR_LOST, Constants.MAX_MONEY_WON_OR_LOST);

        var transaction = new Transaction()
        {
            Description = $"Customer {customer.CustomerItem!.Name} bought a drink at the bar"
        };

        transaction.AmountBeforeTransaction = _gameData.Money;
        _gameData.Money += moneySpent;
        transaction.AmountAfterTransaction = _gameData.Money;
        transaction.TransactionAmount = moneySpent;

        SignalBus.BroadcastPlayerMoneyTransaction(transaction);
        customer.AddTransaction(-moneySpent);
    }

    private void OnCustomerGambling(Customer customer)
    {
        var baseWinMultiplier = _gameData.Inventory.Statues
                    .Where(statue => statue.CustomerBaseWinRateMultiplier is not null)
                    .Aggregate(1.0f, (previousValue, statue) => previousValue * (float)statue.CustomerBaseWinRateMultiplier!);
        var winChance = baseWinMultiplier * Constants.CUSTOMER_BASE_WIN_RATE + customer.CustomerItem!.BonusWinRate;
        var winRandom = GD.Randf();
        var moneyWonOrLost = GD.RandRange(Constants.MIN_MONEY_WON_OR_LOST, Constants.MAX_MONEY_WON_OR_LOST);

        if (winRandom < winChance)
        {
            var transaction = new Transaction()
            {
                Description = $"Customer {customer.CustomerItem.Name} won the gamble"
            };

            transaction.AmountBeforeTransaction = _gameData.Money;
            _gameData.Money -= moneyWonOrLost;
            transaction.AmountAfterTransaction = _gameData.Money;
            transaction.TransactionAmount = -moneyWonOrLost;

            SignalBus.BroadcastPlayerMoneyTransaction(transaction);

            customer.AddTransaction(moneyWonOrLost);
        }
        else
        {
            var transaction = new Transaction()
            {
                Description = $"Customer {customer.CustomerItem.Name} lost the gamble"
            };

            transaction.AmountBeforeTransaction = _gameData.Money;
            _gameData.Money += moneyWonOrLost;
            transaction.AmountAfterTransaction = _gameData.Money;
            transaction.TransactionAmount = moneyWonOrLost;

            SignalBus.BroadcastPlayerMoneyTransaction(transaction);

            customer.AddTransaction(-moneyWonOrLost);
        }
    }
}
