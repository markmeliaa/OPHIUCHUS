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
    private GameObject nextRoom;
    private GameObject otherRoom;
    private GameObject thisRealRoom;

    [HideInInspector] public GameObject connectedRoom = null;
    public GameObject playerSpawn = null;
    [HideInInspector] public GameObject neighbourRealSpawnpoints = null;

    private PlayerRendIso playerRendIso;
    private bool north = false;
    private bool south = false;
    private bool east = false;
    private bool west = false;

    private bool isCorrect = true;
    private float animTime = 0.75f;
    private float animTime2 = 0.75f;

    private void Start()
    {
        templates = GameObject.FindGameObjectWithTag("Rooms").GetComponent<RoomTemplates>();
        player = GameObject.FindGameObjectWithTag("Player");
        thisRealRoom = transform.parent.transform.parent.gameObject;
        playerRendIso = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerRendIso>();
    }

    // Manage change room animation
    private void FixedUpdate()
    {
        if (templates.changingRoom)
        {
            if (north)
            {
                if (animTime > 0)
                {
                    templates.changeRoomAnim.SetBool("ChangeRoom", true);
                    player.transform.GetChild(0).GetComponent<Collider2D>().isTrigger = true;
                    player.transform.position += new Vector3(-0.045f, 0.024f, 0);
                    playerRendIso.SetDirection(new Vector2(-1, 1));
                    animTime -= Time.deltaTime;
                }

                else if (animTime2 > 0)
                {
                    templates.changeRoomAnim.SetBool("ChangeRoom", false);
                    player.transform.position += new Vector3(-0.045f, 0.024f, 0);
                    playerRendIso.SetDirection(new Vector2(-1, 1));
                    animTime2 -= Time.deltaTime;
                }

                else
                {
                    templates.changingRoom = false;
                    north = false;
                    playerRendIso.SetDirection(new Vector2(0, 0));
                    player.transform.GetChild(0).GetComponent<Collider2D>().isTrigger = false;
                    animTime = 0.75f;
                    animTime2 = 0.75f;
                }
            }

            else if (south)
            {
                if (animTime > 0)
                {
                    templates.changeRoomAnim.SetBool("ChangeRoom", true);
                    player.transform.GetChild(0).GetComponent<Collider2D>().isTrigger = true;
                    player.transform.position += new Vector3(0.045f, -0.024f, 0);
                    playerRendIso.SetDirection(new Vector2(1, -1));
                    animTime -= Time.deltaTime;
                }

                else if (animTime2 > 0)
                {
                    templates.changeRoomAnim.SetBool("ChangeRoom", false);
                    player.transform.position += new Vector3(0.045f, -0.024f, 0);
                    playerRendIso.SetDirection(new Vector2(1, -1));
                    animTime2 -= Time.deltaTime;
                }

                else
                {
                    templates.changingRoom = false;
                    south = false;
                    playerRendIso.SetDirection(new Vector2(0, 0));
                    player.transform.GetChild(0).GetComponent<Collider2D>().isTrigger = false;
                    animTime = 0.75f;
                    animTime2 = 0.75f;
                }
            }

            else if (east)
            {
                if (animTime > 0)
                {
                    templates.changeRoomAnim.SetBool("ChangeRoom", true);
                    player.transform.GetChild(0).GetComponent<Collider2D>().isTrigger = true;
                    player.transform.position += new Vector3(0.045f, 0.024f, 0);
                    playerRendIso.SetDirection(new Vector2(1, 1));
                    animTime -= Time.deltaTime;
                }

                else if (animTime2 > 0)
                {
                    templates.changeRoomAnim.SetBool("ChangeRoom", false);
                    player.transform.position += new Vector3(0.045f, 0.024f, 0);
                    playerRendIso.SetDirection(new Vector2(1, 1));
                    animTime2 -= Time.deltaTime;
                }

                else
                {
                    templates.changingRoom = false;
                    east = false;
                    playerRendIso.SetDirection(new Vector2(0, 0));
                    player.transform.GetChild(0).GetComponent<Collider2D>().isTrigger = false;
                    animTime = 0.75f;
                    animTime2 = 0.75f;
                }
            }

            else if (west)
            {
                if (animTime > 0)
                {
                    templates.changeRoomAnim.SetBool("ChangeRoom", true);
                    player.transform.GetChild(0).GetComponent<Collider2D>().isTrigger = true;
                    player.transform.position += new Vector3(-0.045f, -0.024f, 0);
                    playerRendIso.SetDirection(new Vector2(-1, -1));
                    animTime -= Time.deltaTime;
                }

                else if (animTime2 > 0)
                {
                    templates.changeRoomAnim.SetBool("ChangeRoom", false);
                    player.transform.position += new Vector3(-0.045f, -0.024f, 0);
                    playerRendIso.SetDirection(new Vector2(-1, -1));
                    animTime2 -= Time.deltaTime;
                }

                else
                {
                    templates.changingRoom = false;
                    west = false;
                    playerRendIso.SetDirection(new Vector2(0, 0));
                    player.transform.GetChild(0).GetComponent<Collider2D>().isTrigger = false;
                    animTime = 0.75f;
                    animTime2 = 0.75f;
                }
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        thisRoom = templates.currentRoom;
        //Debug.Log(templates.currentRoom);

        if (other.CompareTag("Player") && Input.GetKeyDown(KeyCode.E) && teleport == true)
        {
            Debug.Log(true);
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
                                //Debug.Log(spawnPoints.transform.parent.gameObject.name + ", " + spawnPoints.gameObject.name);
                                if (room.name + "(Clone)" == "Room " + spawnPoints.transform.GetChild(i).GetComponent<RoomSpawner>().nextRoom.name)
                                {
                                    newRoom = room;
                                    thisRoom = spawnPoints.transform.GetChild(i).GetComponent<RoomSpawner>().actualRoom;
                                    nextRoom = spawnPoints.transform.GetChild(i).GetComponent<RoomSpawner>().nextRoom;
                                    templates.currentRoom = nextRoom;

                                    break;
                                }
                            }
                            break;
                        }
                    }

                    if (newRoom.name != "Room T" && newRoom.name != "Room B" && newRoom.name != "Room L" && newRoom.name != "Room R")
                    {
                        GameObject newSpawnpoints = null;
                        int openings = 0;
                        // Check if the number of openings is correct
                        for (int i = 0; i < nextRoom?.transform.childCount; i++)
                        {
                            if (nextRoom.transform.GetChild(i).CompareTag("SpawnPoint"))
                            {
                                newSpawnpoints = nextRoom.transform.GetChild(i).gameObject;
                            }
                        }

                        for (int i = 0; i < newSpawnpoints.transform.childCount; i++)
                        {
                            //Debug.Log(spawnPoints.transform.GetChild(i).GetComponent<RoomSpawner>().nextRoom);
                            if (newSpawnpoints.transform.GetChild(i).GetComponent<RoomSpawner>().nextRoom != null)
                                openings++;
                        }

                        //Debug.Log(openings);
                        //Debug.Log(newRoom.name);

                        if (openings > 1)
                        {
                            newRoom = Instantiate(newRoom, new Vector3(transform.position.x, transform.position.y + 15, transform.position.z), newRoom.transform.rotation, templates.realRoomPlaceholder);
                            connectedRoom = newRoom;
                            templates.realCreatedRooms.Add(newRoom);
                        }

                        else
                        {
                            if (newRoom.name == "Room TB" || newRoom.name == "Room LB" || newRoom.name == "Room RB")
                            {
                                foreach (GameObject room in templates.realRooms)
                                {
                                    if (room.name == "Room B")
                                        newRoom = room;
                                }

                                newRoom = Instantiate(newRoom, new Vector3(transform.position.x, transform.position.y + 15, transform.position.z), newRoom.transform.rotation, templates.realRoomPlaceholder);
                                connectedRoom = newRoom;
                                templates.realCreatedRooms.Add(newRoom);

                                isCorrect = false;
                                otherRoom = Instantiate(templates.B, nextRoom.transform.position, templates.B.transform.rotation, templates.roomPlaceholder);
                                otherRoom.SetActive(false);
                            }
                        }
                    }

                    else
                    {
                        newRoom = Instantiate(newRoom, new Vector3(transform.position.x, transform.position.y + 15, transform.position.z), newRoom.transform.rotation, templates.realRoomPlaceholder);
                        connectedRoom = newRoom;
                        templates.realCreatedRooms.Add(newRoom);
                    }

                    // Get the player spawnpoints
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
                            //player.transform.position = neighbourRealSpawnpoints.transform.GetChild(i).GetComponent<GetAdjRoom>().playerSpawn.transform.position;
                            templates.changingRoom = true;
                            north = true;
                            StartCoroutine("MovePlayerRoom", neighbourRealSpawnpoints.transform.GetChild(i).GetComponent<GetAdjRoom>().playerSpawn.transform.position);

                            neighbourRealSpawnpoints.transform.GetChild(i).GetComponent<GetAdjRoom>().connectedRoom = thisRealRoom;
                            neighbourRealSpawnpoints.transform.GetChild(i).GetComponent<GetAdjRoom>().thisRoom = nextRoom;
                            neighbourRealSpawnpoints.transform.GetChild(i).GetComponent<GetAdjRoom>().nextRoom = thisRoom;

                            // Follow player location
                            StartCoroutine("MovePlayerLocation");
                        }
                    }
                }

                else
                {
                    // Get the player spawnpoints
                    for (int i = 0; i < connectedRoom.transform.childCount; i++)
                    {
                        if (connectedRoom.transform.GetChild(i).CompareTag("RealSpawnPoint"))
                        {
                            neighbourRealSpawnpoints = connectedRoom.transform.GetChild(i).gameObject;
                        }
                    }

                    for (int i = 0; i < spawnPoints.transform.childCount; i++)
                    {
                        if (spawnPoints.transform.GetChild(i).GetComponent<RoomSpawner>()?.openingDirection == 1)
                        {
                            nextRoom = spawnPoints.transform.GetChild(i).GetComponent<RoomSpawner>().nextRoom;
                        }
                    }

                    // Move the player to the new room
                    for (int i = 0; i < neighbourRealSpawnpoints.transform.childCount; i++)
                    {
                        if (neighbourRealSpawnpoints.transform.GetChild(i).CompareTag("South"))
                        {
                            //player.transform.position = neighbourRealSpawnpoints.transform.GetChild(i).GetComponent<GetAdjRoom>().playerSpawn.transform.position;
                            templates.changingRoom = true;
                            north = true;
                            StartCoroutine("MovePlayerRoom", neighbourRealSpawnpoints.transform.GetChild(i).GetComponent<GetAdjRoom>().playerSpawn.transform.position);

                            neighbourRealSpawnpoints.transform.GetChild(i).GetComponent<GetAdjRoom>().thisRoom = nextRoom;
                            neighbourRealSpawnpoints.transform.GetChild(i).GetComponent<GetAdjRoom>().nextRoom = thisRoom;
                            templates.currentRoom = nextRoom;

                            // Follow player location
                            StartCoroutine("MovePlayerLocation");
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
                                //Debug.Log(spawnPoints.transform.parent.gameObject.name + ", " + spawnPoints.gameObject.name);
                                if (room.name + "(Clone)" == "Room " + spawnPoints.transform.GetChild(i).GetComponent<RoomSpawner>().nextRoom.name)
                                {
                                    newRoom = room;
                                    thisRoom = spawnPoints.transform.GetChild(i).GetComponent<RoomSpawner>().actualRoom;
                                    nextRoom = spawnPoints.transform.GetChild(i).GetComponent<RoomSpawner>().nextRoom;
                                    templates.currentRoom = nextRoom;

                                    break;
                                }
                            }
                            break;
                        }
                    }

                    if (newRoom.name != "Room T" && newRoom.name != "Room B" && newRoom.name != "Room L" && newRoom.name != "Room R")
                    {
                        GameObject newSpawnpoints = null;
                        int openings = 0;
                        // Check if the number of openings is correct
                        for (int i = 0; i < nextRoom?.transform.childCount; i++)
                        {
                            if (nextRoom.transform.GetChild(i).CompareTag("SpawnPoint"))
                            {
                                newSpawnpoints = nextRoom.transform.GetChild(i).gameObject;
                            }
                        }

                        for (int i = 0; i < newSpawnpoints.transform.childCount; i++)
                        {
                            //Debug.Log(spawnPoints.transform.GetChild(i).GetComponent<RoomSpawner>().nextRoom);
                            if (newSpawnpoints.transform.GetChild(i).GetComponent<RoomSpawner>().nextRoom != null)
                                openings++;
                        }

                        //Debug.Log(openings);
                        //Debug.Log(newRoom.name);

                        if (openings > 1)
                        {
                            newRoom = Instantiate(newRoom, new Vector3(transform.position.x, transform.position.y - 15, transform.position.z), newRoom.transform.rotation, templates.realRoomPlaceholder);
                            connectedRoom = newRoom;
                            templates.realCreatedRooms.Add(newRoom);
                        }

                        else
                        {
                            if (newRoom.name == "Room TB" || newRoom.name == "Room TL" || newRoom.name == "Room TR")
                            {
                                foreach (GameObject room in templates.realRooms)
                                {
                                    if (room.name == "Room T")
                                        newRoom = room;
                                }

                                newRoom = Instantiate(newRoom, new Vector3(transform.position.x, transform.position.y - 15, transform.position.z), newRoom.transform.rotation, templates.realRoomPlaceholder);
                                connectedRoom = newRoom;
                                templates.realCreatedRooms.Add(newRoom);

                                isCorrect = false;
                                otherRoom = Instantiate(templates.T, nextRoom.transform.position, templates.T.transform.rotation, templates.roomPlaceholder);
                                otherRoom.SetActive(false);
                            }
                        }
                    }

                    else
                    {
                        newRoom = Instantiate(newRoom, new Vector3(transform.position.x, transform.position.y - 15, transform.position.z), newRoom.transform.rotation, templates.realRoomPlaceholder);
                        connectedRoom = newRoom;
                        templates.realCreatedRooms.Add(newRoom);
                    }

                    // Get the player spawnpoints
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
                            //player.transform.position = neighbourRealSpawnpoints.transform.GetChild(i).GetComponent<GetAdjRoom>().playerSpawn.transform.position;
                            templates.changingRoom = true;
                            south = true;
                            StartCoroutine("MovePlayerRoom", neighbourRealSpawnpoints.transform.GetChild(i).GetComponent<GetAdjRoom>().playerSpawn.transform.position);

                            neighbourRealSpawnpoints.transform.GetChild(i).GetComponent<GetAdjRoom>().connectedRoom = thisRealRoom;
                            neighbourRealSpawnpoints.transform.GetChild(i).GetComponent<GetAdjRoom>().thisRoom = nextRoom;
                            neighbourRealSpawnpoints.transform.GetChild(i).GetComponent<GetAdjRoom>().nextRoom = thisRoom;

                            // Follow player location
                            StartCoroutine("MovePlayerLocation");
                        }
                    }
                }

                else
                {
                    // Get the player spawnpoints
                    for (int i = 0; i < connectedRoom.transform.childCount; i++)
                    {
                        if (connectedRoom.transform.GetChild(i).CompareTag("RealSpawnPoint"))
                        {
                            neighbourRealSpawnpoints = connectedRoom.transform.GetChild(i).gameObject;
                        }
                    }

                    for (int i = 0; i < spawnPoints.transform.childCount; i++)
                    {
                        if (spawnPoints.transform.GetChild(i).GetComponent<RoomSpawner>()?.openingDirection == 2)
                        {
                            nextRoom = spawnPoints.transform.GetChild(i).GetComponent<RoomSpawner>().nextRoom;
                        }
                    }

                    // Move the player to the new room
                    for (int i = 0; i < neighbourRealSpawnpoints.transform.childCount; i++)
                    {
                        if (neighbourRealSpawnpoints.transform.GetChild(i).CompareTag("North"))
                        {
                            //player.transform.position = neighbourRealSpawnpoints.transform.GetChild(i).GetComponent<GetAdjRoom>().playerSpawn.transform.position;
                            templates.changingRoom = true;
                            south = true;
                            StartCoroutine("MovePlayerRoom", neighbourRealSpawnpoints.transform.GetChild(i).GetComponent<GetAdjRoom>().playerSpawn.transform.position);

                            neighbourRealSpawnpoints.transform.GetChild(i).GetComponent<GetAdjRoom>().thisRoom = nextRoom;
                            neighbourRealSpawnpoints.transform.GetChild(i).GetComponent<GetAdjRoom>().nextRoom = thisRoom;
                            templates.currentRoom = nextRoom;

                            // Follow player location
                            StartCoroutine("MovePlayerLocation");
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
                                //Debug.Log(spawnPoints.transform.parent.gameObject.name + ", " + spawnPoints.gameObject.name);
                                if (room.name + "(Clone)" == "Room " + spawnPoints.transform.GetChild(i).GetComponent<RoomSpawner>().nextRoom.name)
                                {
                                    newRoom = room;
                                    thisRoom = spawnPoints.transform.GetChild(i).GetComponent<RoomSpawner>().actualRoom;
                                    nextRoom = spawnPoints.transform.GetChild(i).GetComponent<RoomSpawner>().nextRoom;
                                    templates.currentRoom = nextRoom;

                                    break;
                                }
                            }
                            break;
                        }
                    }

                    if (newRoom.name != "Room T" && newRoom.name != "Room B" && newRoom.name != "Room L" && newRoom.name != "Room R")
                    {
                        GameObject newSpawnpoints = null;
                        int openings = 0;
                        // Check if the number of openings is correct
                        for (int i = 0; i < nextRoom?.transform.childCount; i++)
                        {
                            if (nextRoom.transform.GetChild(i).CompareTag("SpawnPoint"))
                            {
                                newSpawnpoints = nextRoom.transform.GetChild(i).gameObject;
                            }
                        }

                        for (int i = 0; i < newSpawnpoints.transform.childCount; i++)
                        {
                            //Debug.Log(spawnPoints.transform.GetChild(i).GetComponent<RoomSpawner>().nextRoom);
                            if (newSpawnpoints.transform.GetChild(i).GetComponent<RoomSpawner>().nextRoom != null)
                                openings++;
                        }

                        //Debug.Log(openings);
                        //Debug.Log(newRoom.name);

                        if (openings > 1)
                        {
                            newRoom = Instantiate(newRoom, new Vector3(transform.position.x + 25, transform.position.y, transform.position.z), newRoom.transform.rotation, templates.realRoomPlaceholder);
                            connectedRoom = newRoom;
                            templates.realCreatedRooms.Add(newRoom);
                        }

                        else
                        {
                            if (newRoom.name == "Room TL" || newRoom.name == "Room LB" || newRoom.name == "Room LR")
                            {
                                foreach (GameObject room in templates.realRooms)
                                {
                                    if (room.name == "Room L")
                                        newRoom = room;
                                }

                                newRoom = Instantiate(newRoom, new Vector3(transform.position.x + 25, transform.position.y, transform.position.z), newRoom.transform.rotation, templates.realRoomPlaceholder);
                                connectedRoom = newRoom;
                                templates.realCreatedRooms.Add(newRoom);

                                isCorrect = false;
                                otherRoom = Instantiate(templates.L, nextRoom.transform.position, templates.L.transform.rotation, templates.roomPlaceholder);
                                otherRoom.SetActive(false);
                            }
                        }
                    }

                    else
                    {
                        newRoom = Instantiate(newRoom, new Vector3(transform.position.x + 25, transform.position.y, transform.position.z), newRoom.transform.rotation, templates.realRoomPlaceholder);
                        connectedRoom = newRoom;
                        templates.realCreatedRooms.Add(newRoom);
                    }

                    // Get the player spawnpoints
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
                            //player.transform.position = neighbourRealSpawnpoints.transform.GetChild(i).GetComponent<GetAdjRoom>().playerSpawn.transform.position;
                            templates.changingRoom = true;
                            east = true;
                            StartCoroutine("MovePlayerRoom", neighbourRealSpawnpoints.transform.GetChild(i).GetComponent<GetAdjRoom>().playerSpawn.transform.position);

                            neighbourRealSpawnpoints.transform.GetChild(i).GetComponent<GetAdjRoom>().connectedRoom = thisRealRoom;
                            neighbourRealSpawnpoints.transform.GetChild(i).GetComponent<GetAdjRoom>().thisRoom = nextRoom;
                            neighbourRealSpawnpoints.transform.GetChild(i).GetComponent<GetAdjRoom>().nextRoom = thisRoom;

                            // Follow player location
                            StartCoroutine("MovePlayerLocation");
                        }
                    }
                }

                else
                {
                    // Get the player spawnpoints
                    for (int i = 0; i < connectedRoom.transform.childCount; i++)
                    {
                        if (connectedRoom.transform.GetChild(i).CompareTag("RealSpawnPoint"))
                        {
                            neighbourRealSpawnpoints = connectedRoom.transform.GetChild(i).gameObject;
                        }
                    }

                    for (int i = 0; i < spawnPoints.transform.childCount; i++)
                    {
                        if (spawnPoints.transform.GetChild(i).GetComponent<RoomSpawner>()?.openingDirection == 3)
                        {
                            thisRoom = spawnPoints.transform.GetChild(i).GetComponent<RoomSpawner>().actualRoom;
                            nextRoom = spawnPoints.transform.GetChild(i).GetComponent<RoomSpawner>().nextRoom;
                        }
                    }

                    // Move the player to the new room
                    for (int i = 0; i < neighbourRealSpawnpoints.transform.childCount; i++)
                    {
                        if (neighbourRealSpawnpoints.transform.GetChild(i).CompareTag("West"))
                        {
                            //player.transform.position = neighbourRealSpawnpoints.transform.GetChild(i).GetComponent<GetAdjRoom>().playerSpawn.transform.position;
                            templates.changingRoom = true;
                            east = true;
                            StartCoroutine("MovePlayerRoom", neighbourRealSpawnpoints.transform.GetChild(i).GetComponent<GetAdjRoom>().playerSpawn.transform.position);

                            neighbourRealSpawnpoints.transform.GetChild(i).GetComponent<GetAdjRoom>().thisRoom = nextRoom;
                            neighbourRealSpawnpoints.transform.GetChild(i).GetComponent<GetAdjRoom>().nextRoom = thisRoom;
                            templates.currentRoom = nextRoom;

                            // Follow player location
                            StartCoroutine("MovePlayerLocation");
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
                                //Debug.Log(spawnPoints.transform.parent.gameObject.name + ", " + spawnPoints.gameObject.name);
                                if (room.name + "(Clone)" == "Room " + spawnPoints.transform.GetChild(i).GetComponent<RoomSpawner>().nextRoom.name)
                                {
                                    newRoom = room;
                                    thisRoom = spawnPoints.transform.GetChild(i).GetComponent<RoomSpawner>().actualRoom;
                                    nextRoom = spawnPoints.transform.GetChild(i).GetComponent<RoomSpawner>().nextRoom;
                                    templates.currentRoom = nextRoom;

                                    break;
                                }
                            }
                            break;
                        }
                    }

                    if (newRoom.name != "Room T" && newRoom.name != "Room B" && newRoom.name != "Room L" && newRoom.name != "Room R")
                    {
                        GameObject newSpawnpoints = null;
                        int openings = 0;
                        // Check if the number of openings is correct
                        for (int i = 0; i < nextRoom?.transform.childCount; i++)
                        {
                            if (nextRoom.transform.GetChild(i).CompareTag("SpawnPoint"))
                            {
                                newSpawnpoints = nextRoom.transform.GetChild(i).gameObject;
                            }
                        }

                        for (int i = 0; i < newSpawnpoints.transform.childCount; i++)
                        {
                            //Debug.Log(spawnPoints.transform.GetChild(i).GetComponent<RoomSpawner>().nextRoom);
                            if (newSpawnpoints.transform.GetChild(i).GetComponent<RoomSpawner>().nextRoom != null)
                                openings++;
                        }

                        //Debug.Log(openings);
                        //Debug.Log(newRoom.name);

                        if (openings > 1)
                        {
                            newRoom = Instantiate(newRoom, new Vector3(transform.position.x - 25, transform.position.y, transform.position.z), newRoom.transform.rotation, templates.realRoomPlaceholder);
                            connectedRoom = newRoom;
                            templates.realCreatedRooms.Add(newRoom);
                        }

                        else
                        {
                            if (newRoom.name == "Room TR" || newRoom.name == "Room LR" || newRoom.name == "Room RB")
                            {
                                foreach (GameObject room in templates.realRooms)
                                {
                                    if (room.name == "Room R")
                                        newRoom = room;
                                }

                                newRoom = Instantiate(newRoom, new Vector3(transform.position.x - 25, transform.position.y, transform.position.z), newRoom.transform.rotation, templates.realRoomPlaceholder);
                                connectedRoom = newRoom;
                                templates.realCreatedRooms.Add(newRoom);

                                isCorrect = false;
                                otherRoom = Instantiate(templates.R, nextRoom.transform.position, templates.R.transform.rotation, templates.roomPlaceholder);
                                otherRoom.SetActive(false);
                            }
                        }
                    }

                    else
                    {
                        newRoom = Instantiate(newRoom, new Vector3(transform.position.x - 25, transform.position.y, transform.position.z), newRoom.transform.rotation, templates.realRoomPlaceholder);
                        connectedRoom = newRoom;
                        templates.realCreatedRooms.Add(newRoom);
                    }

                    // Get the player spawnpoints
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
                            //player.transform.position = neighbourRealSpawnpoints.transform.GetChild(i).GetComponent<GetAdjRoom>().playerSpawn.transform.position;
                            templates.changingRoom = true;
                            west = true;
                            StartCoroutine("MovePlayerRoom", neighbourRealSpawnpoints.transform.GetChild(i).GetComponent<GetAdjRoom>().playerSpawn.transform.position);

                            neighbourRealSpawnpoints.transform.GetChild(i).GetComponent<GetAdjRoom>().connectedRoom = thisRealRoom;
                            neighbourRealSpawnpoints.transform.GetChild(i).GetComponent<GetAdjRoom>().thisRoom = nextRoom;
                            neighbourRealSpawnpoints.transform.GetChild(i).GetComponent<GetAdjRoom>().nextRoom = thisRoom;

                            // Follow player location
                            StartCoroutine("MovePlayerLocation");
                        }
                    }
                }

                else
                {
                    // Get the player spawnpoints
                    for (int i = 0; i < connectedRoom.transform.childCount; i++)
                    {
                        if (connectedRoom.transform.GetChild(i).CompareTag("RealSpawnPoint"))
                        {
                            neighbourRealSpawnpoints = connectedRoom.transform.GetChild(i).gameObject;
                        }
                    }

                    for (int i = 0; i < spawnPoints.transform.childCount; i++)
                    {
                        if (spawnPoints.transform.GetChild(i).GetComponent<RoomSpawner>()?.openingDirection == 4)
                        {
                            nextRoom = spawnPoints.transform.GetChild(i).GetComponent<RoomSpawner>().nextRoom;
                        }
                    }

                    // Move the player to the new room
                    for (int i = 0; i < neighbourRealSpawnpoints.transform.childCount; i++)
                    {
                        if (neighbourRealSpawnpoints.transform.GetChild(i).CompareTag("East"))
                        {
                            //player.transform.position = neighbourRealSpawnpoints.transform.GetChild(i).GetComponent<GetAdjRoom>().playerSpawn.transform.position;
                            templates.changingRoom = true;
                            west = true;
                            StartCoroutine("MovePlayerRoom", neighbourRealSpawnpoints.transform.GetChild(i).GetComponent<GetAdjRoom>().playerSpawn.transform.position);

                            neighbourRealSpawnpoints.transform.GetChild(i).GetComponent<GetAdjRoom>().thisRoom = nextRoom;
                            neighbourRealSpawnpoints.transform.GetChild(i).GetComponent<GetAdjRoom>().nextRoom = thisRoom;
                            templates.currentRoom = nextRoom;

                            // Follow player location
                            StartCoroutine("MovePlayerLocation");
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

    IEnumerator MovePlayerRoom(Vector3 moveTo)
    {
        yield return new WaitForSeconds(0.75f);
        player.transform.position = moveTo;
    }

    IEnumerator MovePlayerLocation()
    {
        yield return new WaitForSeconds(0.75f);
        nextRoom.SetActive(true);
        if (!isCorrect)
            otherRoom.SetActive(true);

        templates.actualRoom.transform.parent = nextRoom.transform;
        templates.actualRoom.transform.position = nextRoom.transform.position;
    }
}
