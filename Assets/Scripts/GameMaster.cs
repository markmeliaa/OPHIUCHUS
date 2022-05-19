using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameMaster
{
    public static int playerLife = 2;
    public static int maxPlayerLife = 20;
    public static int playerSpeed = 3;

    public static int runMoney = 0;

    public static int runRealmNumber = 0;
    public static string[] realmLocations = { "Andromeda", "Triangulum", "Hoags", "Melotte", "Circinus", "The Deep Void" };

    public static List<ItemObject> inventory = new List<ItemObject>();
}
