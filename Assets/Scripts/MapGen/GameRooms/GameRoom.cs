using System.Collections.Generic;
using UnityEngine;

public class GameRoom : MonoBehaviour
{
    [HideInInspector] public GameObject thisMinimapRoomObject;
    [HideInInspector] public GameObject thisGameRoomObject;

    public List<GameRoomSpawner> thisRoomSpawnPoints;
    [HideInInspector] public List<MinimapRoomSpawner> thisMinimapRoomSpawnPoints;

    [HideInInspector] public ManageChangeRoomAnim changeRoomAnim;

    private void Start()
    {
        thisGameRoomObject = gameObject;

        changeRoomAnim = GetComponent<ManageChangeRoomAnim>();
    }
}
