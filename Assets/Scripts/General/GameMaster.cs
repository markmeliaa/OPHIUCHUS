using System.Collections.Generic;

public static class GameMaster
{
    public static int playerLife = 15;
    public static int maxPlayerLife = playerLife;
    public static int playerSpeed = 5;

    public static int runMoney;
    public static int totalMoney;

    public static int currentLevel;
    public static string[] levelsLocations = { "Andromeda", "Triangulum", "Hoags", "Melotte", "Circinus", "The Deep Void" };

    public static List<ItemObject> inventory = new();
    public static int inventoryMaxSpace = 4;

    public static int attempts;
    public static int successfulAttemps;

    public static int temperanceIndex;
    public static int cancerIndex;
    public static int capricornIndex;

    public static string whoFirst;

    public static void Reset()
    {
        inventory = new List<ItemObject>();
        runMoney = 0;
        currentLevel = 0;
    }
}
