using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetAdjRoom : MonoBehaviour
{
    private GameObject player;

    private bool teleport = true;
    private RoomTemplates templates;
    private GameObject spawnPoints;
    private GameObject thisRoom;

    [HideInInspector] public GameObject connectedRoom = null;
    public GameObject playerSpawn = null;
    [HideInInspector] public GameObject neighbourRealSpawnpoints = null;

    private void Start()
    {
        templates = GameObject.FindGameObjectWithTag("Rooms").GetComponent<RoomTemplates>();
        player = GameObject.FindGameObjectWithTag("Player");
        thisRoom = transform.parent.transform.parent.gameObject;
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
                GameObject newRoom = connectedRoom;

                if (connectedRoom == null)
                {
                    for (int i = 0; i < spawnPoints.transform.childCount; i++)
                    {
                        if (spawnPoints.transform.GetChild(i).GetComponent<RoomSpawner>()?.openingDirection == 1)
                        {
                            foreach (GameObject room in templates.realRooms)
                            {
                                if (room.name + "(Clone)" == "Room " + spawnPoints.transform.GetChild(i).GetComponent<RoomSpawner>().nextRoom.name)
                                {
                                    templates.currentRoom = spawnPoints.transform.GetChild(i).GetComponent<RoomSpawner>().nextRoom;
                                    newRoom = Instantiate(room, new Vector3(transform.position.x, transform.position.y + 15, transform.position.z), room.transform.rotation, templates.realRoomPlaceholder);
                                    connectedRoom = newRoom;
                                    break;
                                }
                            }

                            break;
                        }
                    }

                    for (int i = 0; i < newRoom.transform.childCount; i++)
                    {
                        if (newRoom.transform.GetChild(i).CompareTag("RealSpawnPoint"))
                        {
                            neighbourRealSpawnpoints = newRoom.transform.GetChild(i).gameObject;
                        }
                    }

                    // Move the player to the new room
                    for (int i = 0; i < neighbourRealSpawnpoints.transform.childCount; i++)
                    {
                        if (neighbourRealSpawnpoints.transform.GetChild(i).CompareTag("South"))
                        {
                            player.transform.position = neighbourRealSpawnpoints.transform.GetChild(i).GetComponent<GetAdjRoom>().playerSpawn.transform.position;
                            neighbourRealSpawnpoints.transform.GetChild(i).GetComponent<GetAdjRoom>().connectedRoom = thisRoom;
                        }
                    }
                }

                else
                {
                    for (int i = 0; i < newRoom.transform.childCount; i++)
                    {
                        if (newRoom.transform.GetChild(i).CompareTag("RealSpawnPoint"))
                        {
                            neighbourRealSpawnpoints = newRoom.transform.GetChild(i).gameObject;
                        }
                    }

                    // Move the player to the new room
                    for (int i = 0; i < neighbourRealSpawnpoints.transform.childCount; i++)
                    {
                        if (neighbourRealSpawnpoints.transform.GetChild(i).CompareTag("South"))
                        {
                            player.transform.position = neighbourRealSpawnpoints.transform.GetChild(i).GetComponent<GetAdjRoom>().playerSpawn.transform.position;
                            neighbourRealSpawnpoints.transform.GetChild(i).GetComponent<GetAdjRoom>().connectedRoom = thisRoom;
                            templates.currentRoom = connectedRoom;
                        }
                    }
                }
            }

            else if (CompareTag("South"))
            {
                GameObject newRoom = connectedRoom;

                if (connectedRoom == null)
                {
                    for (int i = 0; i < spawnPoints.transform.childCount; i++)
                    {
                        if (spawnPoints.transform.GetChild(i).GetComponent<RoomSpawner>()?.openingDirection == 2)
                        {
                            foreach (GameObject room in templates.realRooms)
                            {
                                if (room.name + "(Clone)" == "Room " + spawnPoints.transform.GetChild(i).GetComponent<RoomSpawner>().nextRoom.name)
                                {
                                    templates.currentRoom = spawnPoints.transform.GetChild(i).GetComponent<RoomSpawner>().nextRoom;
                                    newRoom = Instantiate(room, new Vector3(transform.position.x, transform.position.y - 15, transform.position.z), room.transform.rotation, templates.realRoomPlaceholder);
                                    connectedRoom = newRoom;
                                    break;
                                }
                            }

                            break;
                        }
                    }

                    for (int i = 0; i < newRoom.transform.childCount; i++)
                    {
                        if (newRoom.transform.GetChild(i).CompareTag("RealSpawnPoint"))
                        {
                            neighbourRealSpawnpoints = newRoom.transform.GetChild(i).gameObject;
                        }
                    }

                    // Move the player to the new room
                    for (int i = 0; i < neighbourRealSpawnpoints.transform.childCount; i++)
                    {
                        if (neighbourRealSpawnpoints.transform.GetChild(i).CompareTag("North"))
                        {
                            player.transform.position = neighbourRealSpawnpoints.transform.GetChild(i).GetComponent<GetAdjRoom>().playerSpawn.transform.position;
                            neighbourRealSpawnpoints.transform.GetChild(i).GetComponent<GetAdjRoom>().connectedRoom = thisRoom;
                        }
                    }
                }

                else
                {
                    for (int i = 0; i < newRoom.transform.childCount; i++)
                    {
                        if (newRoom.transform.GetChild(i).CompareTag("RealSpawnPoint"))
                        {
                            neighbourRealSpawnpoints = newRoom.transform.GetChild(i).gameObject;
                        }
                    }

                    // Move the player to the new room
                    for (int i = 0; i < neighbourRealSpawnpoints.transform.childCount; i++)
                    {
                        if (neighbourRealSpawnpoints.transform.GetChild(i).CompareTag("North"))
                        {
                            player.transform.position = neighbourRealSpawnpoints.transform.GetChild(i).GetComponent<GetAdjRoom>().playerSpawn.transform.position;
                            neighbourRealSpawnpoints.transform.GetChild(i).GetComponent<GetAdjRoom>().connectedRoom = thisRoom;
                            templates.currentRoom = connectedRoom;
                        }
                    }
                }
            }

            else if (CompareTag("East"))
            {
                GameObject newRoom = connectedRoom;

                if (connectedRoom == null)
                {
                    for (int i = 0; i < spawnPoints.transform.childCount; i++)
                    {
                        if (spawnPoints.transform.GetChild(i).GetComponent<RoomSpawner>()?.openingDirection == 3)
                        {
                            foreach (GameObject room in templates.realRooms)
                            {
                                if (room.name + "(Clone)" == "Room " + spawnPoints.transform.GetChild(i).GetComponent<RoomSpawner>().nextRoom.name)
                                {
                                    templates.currentRoom = spawnPoints.transform.GetChild(i).GetComponent<RoomSpawner>().nextRoom;
                                    newRoom = Instantiate(room, new Vector3(transform.position.x + 25, transform.position.y, transform.position.z), room.transform.rotation, templates.realRoomPlaceholder);
                                    connectedRoom = newRoom;
                                    break;
                                }
                            }

                            break;
                        }
                    }

                    for (int i = 0; i < newRoom.transform.childCount; i++)
                    {
                        if (newRoom.transform.GetChild(i).CompareTag("RealSpawnPoint"))
                        {
                            neighbourRealSpawnpoints = newRoom.transform.GetChild(i).gameObject;
                        }
                    }

                    // Move the player to the new room
                    for (int i = 0; i < neighbourRealSpawnpoints.transform.childCount; i++)
                    {
                        if (neighbourRealSpawnpoints.transform.GetChild(i).CompareTag("West"))
                        {
                            player.transform.position = neighbourRealSpawnpoints.transform.GetChild(i).GetComponent<GetAdjRoom>().playerSpawn.transform.position;
                            neighbourRealSpawnpoints.transform.GetChild(i).GetComponent<GetAdjRoom>().connectedRoom = thisRoom;
                        }
                    }
                }

                else
                {
                    for (int i = 0; i < newRoom.transform.childCount; i++)
                    {
                        if (newRoom.transform.GetChild(i).CompareTag("RealSpawnPoint"))
                        {
                            neighbourRealSpawnpoints = newRoom.transform.GetChild(i).gameObject;
                        }
                    }

                    // Move the player to the new room
                    for (int i = 0; i < neighbourRealSpawnpoints.transform.childCount; i++)
                    {
                        if (neighbourRealSpawnpoints.transform.GetChild(i).CompareTag("West"))
                        {
                            player.transform.position = neighbourRealSpawnpoints.transform.GetChild(i).GetComponent<GetAdjRoom>().playerSpawn.transform.position;
                            neighbourRealSpawnpoints.transform.GetChild(i).GetComponent<GetAdjRoom>().connectedRoom = thisRoom;
                            templates.currentRoom = connectedRoom;
                        }
                    }
                }
            }

            else if (CompareTag("West"))
            {
                GameObject newRoom = connectedRoom;

                if (connectedRoom == null)
                {
                    for (int i = 0; i < spawnPoints.transform.childCount; i++)
                    {
                        if (spawnPoints.transform.GetChild(i).GetComponent<RoomSpawner>()?.openingDirection == 4)
                        {
                            foreach (GameObject room in templates.realRooms)
                            {
                                if (room.name + "(Clone)" == "Room " + spawnPoints.transform.GetChild(i).GetComponent<RoomSpawner>().nextRoom.name)
                                {
                                    templates.currentRoom = spawnPoints.transform.GetChild(i).GetComponent<RoomSpawner>().nextRoom;
                                    newRoom = Instantiate(room, new Vector3(transform.position.x - 25, transform.position.y, transform.position.z), room.transform.rotation, templates.realRoomPlaceholder);
                                    connectedRoom = newRoom;
                                    break;
                                }
                            }

                            break;
                        }
                    }

                    for (int i = 0; i < newRoom.transform.childCount; i++)
                    {
                        if (newRoom.transform.GetChild(i).CompareTag("RealSpawnPoint"))
                        {
                            neighbourRealSpawnpoints = newRoom.transform.GetChild(i).gameObject;
                        }
                    }

                    // Move the player to the new room
                    for (int i = 0; i < neighbourRealSpawnpoints.transform.childCount; i++)
                    {
                        if (neighbourRealSpawnpoints.transform.GetChild(i).CompareTag("East"))
                        {
                            player.transform.position = neighbourRealSpawnpoints.transform.GetChild(i).GetComponent<GetAdjRoom>().playerSpawn.transform.position;
                            neighbourRealSpawnpoints.transform.GetChild(i).GetComponent<GetAdjRoom>().connectedRoom = thisRoom;
                        }
                    }
                }

                else
                {
                    for (int i = 0; i < newRoom.transform.childCount; i++)
                    {
                        if (newRoom.transform.GetChild(i).CompareTag("RealSpawnPoint"))
                        {
                            neighbourRealSpawnpoints = newRoom.transform.GetChild(i).gameObject;
                        }
                    }

                    // Move the player to the new room
                    for (int i = 0; i < neighbourRealSpawnpoints.transform.childCount; i++)
                    {
                        if (neighbourRealSpawnpoints.transform.GetChild(i).CompareTag("East"))
                        {
                            player.transform.position = neighbourRealSpawnpoints.transform.GetChild(i).GetComponent<GetAdjRoom>().playerSpawn.transform.position;
                            neighbourRealSpawnpoints.transform.GetChild(i).GetComponent<GetAdjRoom>().connectedRoom = thisRoom;
                            templates.currentRoom = connectedRoom;
                        }
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
