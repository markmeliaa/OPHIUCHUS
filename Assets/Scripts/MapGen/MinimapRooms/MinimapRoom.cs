using System.Collections.Generic;
using UnityEngine;

public class MinimapRoom : MonoBehaviour
{
    [HideInInspector] public GameObject thisMinimapRoomObject;
    [HideInInspector] public GameObject thisGameRoomObject;

    public List<MinimapRoomSpawner> minimapRoomSpawnPoints;

    private void Start()
    {
        thisMinimapRoomObject = gameObject;
    }
}
