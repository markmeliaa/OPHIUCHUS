using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameMaster
{
    public static int playerLife = 48; 
    public static int maxPlayerLife = 50; 
    public static int playerSpeed = 3;

    public static int runMoney = 0;

    public static List<ItemObject> inventory = new List<ItemObject>();
}
