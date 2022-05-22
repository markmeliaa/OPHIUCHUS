using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameMaster
{
    public static int playerLife = 10;
    public static int maxPlayerLife = 20;
    public static int playerSpeed = 3;

    public static int runMoney = 0;
    public static int totalMoney = 0;

    public static int runRealmNumber = 0;
    public static string[] realmLocations = { "Andromeda", "Triangulum", "Hoags", "Melotte", "Circinus", "The Deep Void" };

    public static List<ItemObject> inventory = new List<ItemObject>();

    public static int attempts = 0;
    public static int successfulAttemps = 0;

    public static int temperanceIndex = 0;
    public static int cancerIndex = 0;
    public static int capricornIndex = 0;

    public static void Reset()
    {
        playerLife = 10;
        playerSpeed = 3;
        inventory = new List<ItemObject>();
        runMoney = 0;
        runRealmNumber = 0;
    }
}
