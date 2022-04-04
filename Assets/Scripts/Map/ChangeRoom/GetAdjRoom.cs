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
    private GameObject thisRealRoom;

    [HideInInspector] public GameObject connectedRoom = null;
    public GameObject playerSpawn = null;
    [HideInInspector] public GameObject neighbourRealSpawnpoints = null;

    private void Start()
    {
        templates = GameObject.FindGameObjectWithTag("Rooms").GetComponent<RoomTemplates>();
        player = GameObject.FindGameObjectWithTag("Player");
        thisRoom = templates.initialRoom;
        thisRealRoom = transform.parent.transform.parent.gameObject;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        Debug.Log(thisRoom);

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
                foreach (GameObject createdRoom in templates.realCreatedRooms)
                {
                    if (createdRoom.transform.position == new Vector3(transform.position.x, transform.position.y + 15, transform.position.z))
                    {
                        connectedRoom = createdRoom;
                    }
                }

                if (connectedRoom == null)
                {
                    GameObject newRoom = null;
                    for (int i = 0; i < spawnPoints.transform.childCount; i++)
                    {
                        if (spawnPoints.transform.GetChild(i).GetComponent<RoomSpawner>()?.openingDirection == 1)
                        {
                            foreach (GameObject room in templates.realRooms)
                            {
                                if (room.name + "(Clone)" == "Room " + spawnPoints.transform.GetChild(i).GetComponent<RoomSpawner>().nextRoom.name)
                                {
                                    thisRoom = spawnPoints.transform.GetChild(i).GetComponent<RoomSpawner>().nextRoom;
                                    templates.currentRoom = thisRoom;
                                    newRoom = Instantiate(room, new Vector3(transform.position.x, transform.position.y + 15, transform.position.z), room.transform.rotation, templates.realRoomPlaceholder);
                                    connectedRoom = newRoom;
                                    templates.realCreatedRooms.Add(newRoom);
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
                            neighbourRealSpawnpoints.transform.GetChild(i).GetComponent<GetAdjRoom>().connectedRoom = thisRealRoom;
                        }
                    }
                }

                else
                {
                    for (int i = 0; i < connectedRoom.transform.childCount; i++)
                    {
                        if (connectedRoom.transform.GetChild(i).CompareTag("RealSpawnPoint"))
                        {
                            neighbourRealSpawnpoints = connectedRoom.transform.GetChild(i).gameObject;
                        }
                    }

                    // Move the player to the new room
                    for (int i = 0; i < neighbourRealSpawnpoints.transform.childCount; i++)
                    {
                        if (neighbourRealSpawnpoints.transform.GetChild(i).CompareTag("South"))
                        {
                            player.transform.position = neighbourRealSpawnpoints.transform.GetChild(i).GetComponent<GetAdjRoom>().playerSpawn.transform.position;
                            neighbourRealSpawnpoints.transform.GetChild(i).GetComponent<GetAdjRoom>().connectedRoom = thisRealRoom;
                            templates.currentRoom = thisRoom;
                        }
                    }
                }
            }

            else if (CompareTag("South"))
            {
                foreach (GameObject createdRoom in templates.realCreatedRooms)
                {
                    if (createdRoom.transform.position == new Vector3(transform.position.x, transform.position.y - 15, transform.position.z))
                    {
                        connectedRoom = createdRoom;
                    }
                }

                if (connectedRoom == null)
                {
                    GameObject newRoom = null;
                    for (int i = 0; i < spawnPoints.transform.childCount; i++)
                    {
                        if (spawnPoints.transform.GetChild(i).GetComponent<RoomSpawner>()?.openingDirection == 2)
                        {
                            foreach (GameObject room in templates.realRooms)
                            {
                                if (room.name + "(Clone)" == "Room " + spawnPoints.transform.GetChild(i).GetComponent<RoomSpawner>().nextRoom.name)
                                {
                                    thisRoom = spawnPoints.transform.GetChild(i).GetComponent<RoomSpawner>().nextRoom;
                                    templates.currentRoom = thisRoom;
                                    newRoom = Instantiate(room, new Vector3(transform.position.x, transform.position.y - 15, transform.position.z), room.transform.rotation, templates.realRoomPlaceholder);
                                    connectedRoom = newRoom;
                                    templates.realCreatedRooms.Add(newRoom);
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
                            neighbourRealSpawnpoints.transform.GetChild(i).GetComponent<GetAdjRoom>().connectedRoom = thisRealRoom;
                        }
                    }
                }

                else
                {
                    for (int i = 0; i < connectedRoom.transform.childCount; i++)
                    {
                        if (connectedRoom.transform.GetChild(i).CompareTag("RealSpawnPoint"))
                        {
                            neighbourRealSpawnpoints = connectedRoom.transform.GetChild(i).gameObject;
                        }
                    }

                    // Move the player to the new room
                    for (int i = 0; i < neighbourRealSpawnpoints.transform.childCount; i++)
                    {
                        if (neighbourRealSpawnpoints.transform.GetChild(i).CompareTag("North"))
                        {
                            player.transform.position = neighbourRealSpawnpoints.transform.GetChild(i).GetComponent<GetAdjRoom>().playerSpawn.transform.position;
                            neighbourRealSpawnpoints.transform.GetChild(i).GetComponent<GetAdjRoom>().connectedRoom = thisRealRoom;
                            templates.currentRoom = thisRoom;
                        }
                    }
                }
            }

            else if (CompareTag("East"))
            {
                foreach (GameObject createdRoom in templates.realCreatedRooms)
                {
                    if (createdRoom.transform.position == new Vector3(transform.position.x + 25, transform.position.y, transform.position.z))
                    {
                        connectedRoom = createdRoom;
                    }
                }

                if (connectedRoom == null)
                {
                    GameObject newRoom = null;
                    for (int i = 0; i < spawnPoints.transform.childCount; i++)
                    {
                        if (spawnPoints.transform.GetChild(i).GetComponent<RoomSpawner>()?.openingDirection == 3)
                        {
                            foreach (GameObject room in templates.realRooms)
                            {
                                if (room.name + "(Clone)" == "Room " + spawnPoints.transform.GetChild(i).GetComponent<RoomSpawner>().nextRoom.name)
                                {
                                    thisRoom = spawnPoints.transform.GetChild(i).GetComponent<RoomSpawner>().nextRoom;
                                    templates.currentRoom = thisRoom;
                                    newRoom = Instantiate(room, new Vector3(transform.position.x + 25, transform.position.y, transform.position.z), room.transform.rotation, templates.realRoomPlaceholder);
                                    connectedRoom = newRoom;
                                    templates.realCreatedRooms.Add(newRoom);
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
                            neighbourRealSpawnpoints.transform.GetChild(i).GetComponent<GetAdjRoom>().connectedRoom = thisRealRoom;
                        }
                    }
                }

                else
                {
                    for (int i = 0; i < connectedRoom.transform.childCount; i++)
                    {
                        if (connectedRoom.transform.GetChild(i).CompareTag("RealSpawnPoint"))
                        {
                            neighbourRealSpawnpoints = connectedRoom.transform.GetChild(i).gameObject;
                        }
                    }

                    // Move the player to the new room
                    for (int i = 0; i < neighbourRealSpawnpoints.transform.childCount; i++)
                    {
                        if (neighbourRealSpawnpoints.transform.GetChild(i).CompareTag("West"))
                        {
                            player.transform.position = neighbourRealSpawnpoints.transform.GetChild(i).GetComponent<GetAdjRoom>().playerSpawn.transform.position;
                            neighbourRealSpawnpoints.transform.GetChild(i).GetComponent<GetAdjRoom>().connectedRoom = thisRealRoom;
                            templates.currentRoom = thisRoom;
                        }
                    }
                }
            }

            else if (CompareTag("West"))
            {
                foreach (GameObject createdRoom in templates.realCreatedRooms)
                {
                    if (createdRoom.transform.position == new Vector3(transform.position.x - 25, transform.position.y, transform.position.z))
                    {
                        connectedRoom = createdRoom;
                    }
                }

                if (connectedRoom == null)
                {
                    GameObject newRoom = null;
                    for (int i = 0; i < spawnPoints.transform.childCount; i++)
                    {
                        if (spawnPoints.transform.GetChild(i).GetComponent<RoomSpawner>()?.openingDirection == 4)
                        {
                            foreach (GameObject room in templates.realRooms)
                            {
                                if (room.name + "(Clone)" == "Room " + spawnPoints.transform.GetChild(i).GetComponent<RoomSpawner>().nextRoom.name)
                                {
                                    thisRoom = spawnPoints.transform.GetChild(i).GetComponent<RoomSpawner>().nextRoom;
                                    templates.currentRoom = thisRoom;
                                    newRoom = Instantiate(room, new Vector3(transform.position.x - 25, transform.position.y, transform.position.z), room.transform.rotation, templates.realRoomPlaceholder);
                                    connectedRoom = newRoom;
                                    templates.realCreatedRooms.Add(newRoom);
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
                            neighbourRealSpawnpoints.transform.GetChild(i).GetComponent<GetAdjRoom>().connectedRoom = thisRealRoom;
                        }
                    }
                }

                else
                {
                    for (int i = 0; i < connectedRoom.transform.childCount; i++)
                    {
                        if (connectedRoom.transform.GetChild(i).CompareTag("RealSpawnPoint"))
                        {
                            neighbourRealSpawnpoints = connectedRoom.transform.GetChild(i).gameObject;
                        }
                    }

                    // Move the player to the new room
                    for (int i = 0; i < neighbourRealSpawnpoints.transform.childCount; i++)
                    {
                        if (neighbourRealSpawnpoints.transform.GetChild(i).CompareTag("East"))
                        {
                            player.transform.position = neighbourRealSpawnpoints.transform.GetChild(i).GetComponent<GetAdjRoom>().playerSpawn.transform.position;
                            neighbourRealSpawnpoints.transform.GetChild(i).GetComponent<GetAdjRoom>().connectedRoom = thisRealRoom;
                            templates.currentRoom = thisRoom;
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
