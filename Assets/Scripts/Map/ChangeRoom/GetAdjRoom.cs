using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetAdjRoom : MonoBehaviour
{
    private bool teleport = true;
    private RoomTemplates templates;
    private GameObject spawnPoints;

    private void Start()
    {
        templates = GameObject.FindGameObjectWithTag("Rooms").GetComponent<RoomTemplates>();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && Input.GetButton("Fire1") && teleport == true)
        {
            teleport = false;

            for (int i = 0; i < templates.currentRoom.transform.childCount; i++)
            {
                if (templates.currentRoom.transform.GetChild(i).CompareTag("SpawnPoint"))
                {
                    spawnPoints = templates.currentRoom.transform.GetChild(i).gameObject;
                }
            }

            if (CompareTag("North"))
            {
                for (int i = 0; i < spawnPoints.transform.childCount; i++)
                {
                    if (spawnPoints.transform.GetChild(i).GetComponent<RoomSpawner>()?.openingDirection == 1)
                    {
                        Debug.Log(spawnPoints.transform.GetChild(i).GetComponent<RoomSpawner>().nextRoom.name);
                    }
                }
            }

            else if (CompareTag("South"))
            {
                for (int i = 0; i < spawnPoints.transform.childCount; i++)
                {
                    if (spawnPoints.transform.GetChild(i).GetComponent<RoomSpawner>()?.openingDirection == 2)
                    {
                        Debug.Log(spawnPoints.transform.GetChild(i).GetComponent<RoomSpawner>().nextRoom.name);
                    }
                }
            }

            else if (CompareTag("East"))
            {
                for (int i = 0; i < spawnPoints.transform.childCount; i++)
                {
                    if (spawnPoints.transform.GetChild(i).GetComponent<RoomSpawner>()?.openingDirection == 3)
                    {
                        Debug.Log(spawnPoints.transform.GetChild(i).GetComponent<RoomSpawner>().nextRoom.name);
                    }
                }
            }

            else if (CompareTag("West"))
            {
                for (int i = 0; i < spawnPoints.transform.childCount; i++)
                {
                    if (spawnPoints.transform.GetChild(i).GetComponent<RoomSpawner>()?.openingDirection == 4)
                    {
                        Debug.Log(spawnPoints.transform.GetChild(i).GetComponent<RoomSpawner>().nextRoom.name);
                    }
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        teleport = true;
    }
}
