using System.Collections.Generic;

public static class GameMaster
{
    public static int playerLife;
    public static int maxPlayerLife;
    public static int playerSpeed;

    public static int runMoney;
    public static int totalMoney;

    public static int runRealmNumber;
    public static string[] realmLocations = { "Andromeda", "Triangulum", "Hoags", "Melotte", "Circinus", "The Deep Void" };

    public static List<ItemObject> inventory = new();

    public static int attempts;
    public static int successfulAttemps;

    public static int temperanceIndex;
    public static int cancerIndex;
    public static int capricornIndex;

    public static string whoFirst;

    public static void Reset()
    {
        playerLife = 10;
        playerSpeed = 3;
        inventory = new List<ItemObject>();
        runMoney = 0;
        runRealmNumber = 0;
    }
}
