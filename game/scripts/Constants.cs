namespace ProjectGJ.Scripts;

public static class Constants
{
    public readonly static string CHARACTER_RESOURCE_BASE_PATH = "res://characters/resource";
    public readonly static string ASSET_SPRITE_CASINO_BASE_PATH = "res://assets/sprites/casino";

    public readonly static int MIN_SALARY = 1000;
    public readonly static int MAX_SALARY = 10000;
    public readonly static float SELL_PERCENTAGE = 0.75f;
    public readonly static float TAX_RATE = 0.15f;
    public readonly static int CASINO_RENT = 4500;
    public readonly static int MAINTENANCE_FEES = 750;
    public readonly static float SLOT_MACHINE_BREAK_RATE = 0.001f;
    public readonly static int SLOT_MACHINE_REPAIR_FEE = 150;
    public readonly static float RANDOM_GENDER_RATE = 0.5f;
    public readonly static float RANDOM_PROFESSION_RATE = 0.33f;
    public readonly static float RANDOM_CHEATER_RATE = 0.15f;
    public readonly static float RANDOM_ADDICT_RATE = 0.3f;
    public readonly static float SECURITY_MULTIPLIER = 1.0f - 0.02f;
    public readonly static float CUSTOMER_BASE_WIN_RATE = 0.25f;
    public readonly static int MAX_NUMBER_CUSTOMERS = 10;
    public readonly static int MIN_ACTIVITY_TIME = 60 * 3;
    public readonly static float MIN_GAMBLE_TIME = 1.0f;
    public readonly static float MAX_GAMBLE_TIME = 3.0f;
    public readonly static int MIN_MONEY_WON_OR_LOST = 25;
    public readonly static int MAX_MONEY_WON_OR_LOST = 250;

    public static class CustomerBonusWinRate
    {
        public readonly static float MIN_NORMAL = -0.05f;
        public readonly static float MAX_NORMAL = 0.15f;
        public readonly static float MIN_CHEATER = 0.05f;
        public readonly static float MAX_CHEATER = 0.25f;
        public readonly static float MIN_ADDICT = -0.1f;
        public readonly static float MAX_ADDICT = 0.1f;
    }

    public static class CustomerAverageTime
    {
        public readonly static int MIN_NORMAL = 3 * 3 * 60;
        public readonly static int MAX_NORMAL = 3 * 4 * 60;
        public readonly static int MIN_CHEATER = 3 * 1 * 60;
        public readonly static int MAX_CHEATER = 3 * 3 * 60;
        public readonly static int MIN_ADDICT = 3 * 4 * 60;
        public readonly static int MAX_ADDICT = 3 * 8 * 60;
    }
}
