namespace ProjectGJ.Scripts;

public class Transaction
{
    public required string Description { get; set; }
    public int AmountBeforeTransaction { get; set; }
    public int AmountAfterTransaction { get; set; }
    public int TransactionAmount { get; set; }
    public int Day { get; set; }
}
