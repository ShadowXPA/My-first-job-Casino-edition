using System.Collections.Generic;

namespace ProjectGJ.Scripts.Items;

public class CustomerItem : Item
{
    public CustomerType CustomerType { get; set; }
    public float BonusWinRate { get; set; }
    public List<CustomerActivity> Activities { get; set; } = [new CustomerActivity() { Activity = ActivityType.Home }];
}
