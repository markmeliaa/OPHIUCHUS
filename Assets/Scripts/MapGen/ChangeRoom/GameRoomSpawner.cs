using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TravelDirection
{
    NONE,
    NORTH,
    SOUTH,
    EAST,
    WEST
}

public class GameRoomSpawner : MonoBehaviour
{
    // PLAYER RELATED STUFF
    private GameObject player;
    private bool canPlayerTriggerTeleport;
    private bool isPlayerTeleporting;

    // TELEPORTING RELATED STUFF
    private PlayerAnimationDirection playerAnimationDirection;

    private Vector3 directionToGoWhileTeleporting;
    private Vector2 directionToLookWhileTeleporting;

    private readonly float teleportingXMovement = 0.045f;
    private readonly float teleportingYMovement = 0.024f;

    // DUNGEON MANAGER - ROOMS RELATED STUFF
    private DungeonMapManager dungeonMapManager;
    [SerializeField] private List<GameObject> roomSpawnPoints;
    private GameObject neighbourGameRoomSpawnPoints;

    private GameObject thisMinimapRoom;
    private GameObject nextMinimapRoom;

    private GameObject thisGameRoom;
    private GameObject nextGameRoom;

    private GameObject connectedGameRoom; // ?????????????
    [SerializeField] private GameObject playerEntranceSpawn;

    private bool changeRoomKeyPressed = false;

    // ANIMATION STUFF

    private float exitAnimTime = 0.75f;
    private float enterAnimTime = 0.75f;

    private void Start()
    {
        dungeonMapManager = GameObject.FindGameObjectWithTag("DungeonMngr").GetComponent<DungeonMapManager>();
        player = GameObject.FindGameObjectWithTag("Player");
        thisGameRoom = transform.parent.transform.parent.gameObject;
        playerAnimationDirection = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerAnimationDirection>();

        dungeonMapManager.spawnedGameRooms.Add(thisGameRoom);
    }

    private void FixedUpdate()
    {
        if (!isPlayerTeleporting)
        {
            return;
        }

        PerformTeleportMovement();
    }

    private void OnTriggerEnter(Collider other)
    {
        canPlayerTriggerTeleport = true;
    }

    // Manage the change of rooms
    private void OnTriggerStay2D(Collider2D other)
    {
        thisMinimapRoom = dungeonMapManager.currentMinimapRoom;

        if (other.CompareTag("Player") && Input.GetKey(KeyCode.E) && !changeRoomKeyPressed && canPlayerTriggerTeleport == true && !isPlayerTeleporting)
        {
            changeRoomKeyPressed = true;
            canPlayerTriggerTeleport = false;

            if (CompareTag("North"))
            {
                foreach (GameObject createdRoom in dungeonMapManager.spawnedGameRooms)
                {
                    if (createdRoom.transform.position == new Vector3(transform.position.x, transform.position.y + 15, transform.position.z))
                    {
                        connectedGameRoom = createdRoom;
                    }
                }

                if (connectedGameRoom == null)
                {
                    GameObject newRoom = null;
                    for (int i = 0; i < roomSpawnPoints.Count; i++)
                    {
                        if (roomSpawnPoints[i].GetComponent<MinimapRoomSpawner>()?.doorNeeded == DoorOrientation.BOTTOM)
                        {
                            foreach (GameObject room in RoomsHolderSingleton.Instance.realRooms)
                            {
                                //Debug.Log(spawnPoints.transform.parent.gameObject.name + ", " + spawnPoints.gameObject.name);
                                if (room.name + "(Clone)" == "Room " + roomSpawnPoints[i].GetComponent<MinimapRoomSpawner>().nextMinimapRoom.name)
                                {
                                    newRoom = room;
                                    thisMinimapRoom = roomSpawnPoints[i].GetComponent<MinimapRoomSpawner>().thisMinimapRoom;
                                    nextMinimapRoom = roomSpawnPoints[i].GetComponent<MinimapRoomSpawner>().nextMinimapRoom;
                                    dungeonMapManager.currentMinimapRoom = nextMinimapRoom;

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
                        for (int i = 0; i < nextMinimapRoom?.transform.childCount; i++)
                        {
                            if (nextMinimapRoom.transform.GetChild(i).CompareTag("SpawnPoint"))
                            {
                                newSpawnpoints = nextMinimapRoom.transform.GetChild(i).gameObject;
                            }
                        }

                        for (int i = 0; i < newSpawnpoints.transform.childCount; i++)
                        {
                            //Debug.Log(spawnPoints.transform.GetChild(i).GetComponent<RoomSpawner>().nextRoom);
                            if (newSpawnpoints.transform.GetChild(i).GetComponent<MinimapRoomSpawner>().nextMinimapRoom != null)
                                openings++;
                        }

                        //Debug.Log(openings);
                        //Debug.Log(newRoom.name);

                        if (openings > 1)
                        {
                            newRoom = Instantiate(newRoom, new Vector3(transform.position.x, transform.position.y + 15, transform.position.z), newRoom.transform.rotation, dungeonMapManager.gameRoomsParent);
                            connectedGameRoom = newRoom;
                            dungeonMapManager.spawnedGameRooms.Add(newRoom);

                            if (nextMinimapRoom.CompareTag("BossRoom") && newRoom.transform.childCount > 5)
                            {
                                newRoom.transform.GetChild(4).gameObject.SetActive(false);
                                newRoom.transform.GetChild(5).gameObject.SetActive(true);
                            }
                        }

                        else
                        {
                            if (newRoom.name == "Room TB" || newRoom.name == "Room LB" || newRoom.name == "Room RB")
                            {
                                foreach (GameObject room in RoomsHolderSingleton.Instance.realRooms)
                                {
                                    if (room.name == "Room B")
                                        newRoom = room;
                                }

                                newRoom = Instantiate(newRoom, new Vector3(transform.position.x, transform.position.y + 15, transform.position.z), newRoom.transform.rotation, dungeonMapManager.gameRoomsParent);
                                connectedGameRoom = newRoom;
                                dungeonMapManager.spawnedGameRooms.Add(newRoom);

                                if (nextMinimapRoom.CompareTag("BossRoom") && newRoom.transform.childCount > 5)
                                {
                                    newRoom.transform.GetChild(4).gameObject.SetActive(false);
                                    newRoom.transform.GetChild(5).gameObject.SetActive(true);
                                }

                                nextGameRoom = Instantiate(RoomsHolderSingleton.Instance.bottomMinimapRoom, nextMinimapRoom.transform.position, RoomsHolderSingleton.Instance.bottomMinimapRoom.transform.rotation, dungeonMapManager.minimapRoomsParent);
                                nextGameRoom.SetActive(false);
                            }
                        }
                    }

                    else
                    {
                        newRoom = Instantiate(newRoom, new Vector3(transform.position.x, transform.position.y + 15, transform.position.z), newRoom.transform.rotation, dungeonMapManager.gameRoomsParent);
                        connectedGameRoom = newRoom;
                        dungeonMapManager.spawnedGameRooms.Add(newRoom);

                        if (nextMinimapRoom.CompareTag("BossRoom") && newRoom.transform.childCount > 5)
                        {
                            newRoom.transform.GetChild(4).gameObject.SetActive(false);
                            newRoom.transform.GetChild(5).gameObject.SetActive(true);
                        }
                    }

                    // Get the player spawnpoints
                    for (int i = 0; i < newRoom.transform.childCount; i++)
                    {
                        if (newRoom.transform.GetChild(i).CompareTag("RealSpawnPoint"))
                        {
                            neighbourGameRoomSpawnPoints = newRoom.transform.GetChild(i).gameObject;
                        }
                    }

                    // Move the player to the new room
                    for (int i = 0; i < neighbourGameRoomSpawnPoints.transform.childCount; i++)
                    {
                        if (neighbourGameRoomSpawnPoints.transform.GetChild(i).CompareTag("South"))
                        {
                            SetUpRoomTeleport(TravelDirection.NORTH);
                            isPlayerTeleporting = true;

                            StartCoroutine("MovePlayerToNextRoom", neighbourGameRoomSpawnPoints.transform.GetChild(i).GetComponent<GameRoomSpawner>().playerEntranceSpawn.transform.position);

                            neighbourGameRoomSpawnPoints.transform.GetChild(i).GetComponent<GameRoomSpawner>().connectedGameRoom = thisGameRoom;
                            neighbourGameRoomSpawnPoints.transform.GetChild(i).GetComponent<GameRoomSpawner>().thisMinimapRoom = nextMinimapRoom;
                            neighbourGameRoomSpawnPoints.transform.GetChild(i).GetComponent<GameRoomSpawner>().nextMinimapRoom = thisMinimapRoom;

                            // Follow player location
                            StartCoroutine(nameof(MovePlayerLocationInMinimap));
                        }
                    }
                }

                else
                {
                    // Get the player spawnpoints
                    for (int i = 0; i < connectedGameRoom.transform.childCount; i++)
                    {
                        if (connectedGameRoom.transform.GetChild(i).CompareTag("RealSpawnPoint"))
                        {
                            neighbourGameRoomSpawnPoints = connectedGameRoom.transform.GetChild(i).gameObject;
                        }
                    }

                    for (int i = 0; i < roomSpawnPoints.Count; i++)
                    {
                        if (roomSpawnPoints[i].GetComponent<MinimapRoomSpawner>()?.doorNeeded == DoorOrientation.BOTTOM)
                        {
                            nextMinimapRoom = roomSpawnPoints[i].GetComponent<MinimapRoomSpawner>().nextMinimapRoom;
                        }
                    }

                    // Move the player to the new room
                    for (int i = 0; i < neighbourGameRoomSpawnPoints.transform.childCount; i++)
                    {
                        if (neighbourGameRoomSpawnPoints.transform.GetChild(i).CompareTag("South"))
                        {
                            SetUpRoomTeleport(TravelDirection.NORTH);
                            isPlayerTeleporting = true;

                            StartCoroutine("MovePlayerToNextRoom", neighbourGameRoomSpawnPoints.transform.GetChild(i).GetComponent<GameRoomSpawner>().playerEntranceSpawn.transform.position);

                            neighbourGameRoomSpawnPoints.transform.GetChild(i).GetComponent<GameRoomSpawner>().thisMinimapRoom = nextMinimapRoom;
                            neighbourGameRoomSpawnPoints.transform.GetChild(i).GetComponent<GameRoomSpawner>().nextMinimapRoom = thisMinimapRoom;
                            dungeonMapManager.currentMinimapRoom = nextMinimapRoom;

                            // Follow player location
                            StartCoroutine(nameof(MovePlayerLocationInMinimap));
                        }
                    }
                }
            }

            else if (CompareTag("South"))
            {
                foreach (GameObject createdRoom in dungeonMapManager.spawnedGameRooms)
                {
                    if (createdRoom.transform.position == new Vector3(transform.position.x, transform.position.y - 15, transform.position.z))
                    {
                        connectedGameRoom = createdRoom;
                    }
                }

                if (connectedGameRoom == null)
                {
                    GameObject newRoom = null;
                    for (int i = 0; i < roomSpawnPoints.Count; i++)
                    {
                        if (roomSpawnPoints[i].GetComponent<MinimapRoomSpawner>()?.doorNeeded == DoorOrientation.TOP)
                        {
                            foreach (GameObject room in RoomsHolderSingleton.Instance.realRooms)
                            {
                                //Debug.Log(spawnPoints.transform.parent.gameObject.name + ", " + spawnPoints.gameObject.name);
                                if (room.name + "(Clone)" == "Room " + roomSpawnPoints[i].GetComponent<MinimapRoomSpawner>().nextMinimapRoom.name)
                                {
                                    newRoom = room;
                                    thisMinimapRoom = roomSpawnPoints[i].GetComponent<MinimapRoomSpawner>().thisMinimapRoom;
                                    nextMinimapRoom = roomSpawnPoints[i].GetComponent<MinimapRoomSpawner>().nextMinimapRoom;
                                    dungeonMapManager.currentMinimapRoom = nextMinimapRoom;

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
                        for (int i = 0; i < nextMinimapRoom?.transform.childCount; i++)
                        {
                            if (nextMinimapRoom.transform.GetChild(i).CompareTag("SpawnPoint"))
                            {
                                newSpawnpoints = nextMinimapRoom.transform.GetChild(i).gameObject;
                            }
                        }

                        for (int i = 0; i < newSpawnpoints.transform.childCount; i++)
                        {
                            //Debug.Log(spawnPoints.transform.GetChild(i).GetComponent<RoomSpawner>().nextRoom);
                            if (newSpawnpoints.transform.GetChild(i).GetComponent<MinimapRoomSpawner>().nextMinimapRoom != null)
                                openings++;
                        }

                        //Debug.Log(openings);
                        //Debug.Log(newRoom.name);

                        if (openings > 1)
                        {
                            newRoom = Instantiate(newRoom, new Vector3(transform.position.x, transform.position.y - 15, transform.position.z), newRoom.transform.rotation, dungeonMapManager.gameRoomsParent);
                            connectedGameRoom = newRoom;
                            dungeonMapManager.spawnedGameRooms.Add(newRoom);

                            if (nextMinimapRoom.CompareTag("BossRoom") && newRoom.transform.childCount > 5)
                            {
                                newRoom.transform.GetChild(4).gameObject.SetActive(false);
                                newRoom.transform.GetChild(5).gameObject.SetActive(true);
                            }
                        }

                        else
                        {
                            if (newRoom.name == "Room TB" || newRoom.name == "Room TL" || newRoom.name == "Room TR")
                            {
                                foreach (GameObject room in RoomsHolderSingleton.Instance.realRooms)
                                {
                                    if (room.name == "Room T")
                                        newRoom = room;
                                }

                                newRoom = Instantiate(newRoom, new Vector3(transform.position.x, transform.position.y - 15, transform.position.z), newRoom.transform.rotation, dungeonMapManager.gameRoomsParent);
                                connectedGameRoom = newRoom;
                                dungeonMapManager.spawnedGameRooms.Add(newRoom);

                                if (nextMinimapRoom.CompareTag("BossRoom") && newRoom.transform.childCount > 5)
                                {
                                    newRoom.transform.GetChild(4).gameObject.SetActive(false);
                                    newRoom.transform.GetChild(5).gameObject.SetActive(true);
                                }

                                nextGameRoom = Instantiate(RoomsHolderSingleton.Instance.topMinimapRoom, nextMinimapRoom.transform.position, RoomsHolderSingleton.Instance.topMinimapRoom.transform.rotation, dungeonMapManager.minimapRoomsParent);
                                nextGameRoom.SetActive(false);
                            }
                        }
                    }

                    else
                    {
                        newRoom = Instantiate(newRoom, new Vector3(transform.position.x, transform.position.y - 15, transform.position.z), newRoom.transform.rotation, dungeonMapManager.gameRoomsParent);
                        connectedGameRoom = newRoom;
                        dungeonMapManager.spawnedGameRooms.Add(newRoom);

                        if (nextMinimapRoom.CompareTag("BossRoom") && newRoom.transform.childCount > 5)
                        {
                            newRoom.transform.GetChild(4).gameObject.SetActive(false);
                            newRoom.transform.GetChild(5).gameObject.SetActive(true);
                        }
                    }

                    // Get the player spawnpoints
                    for (int i = 0; i < newRoom.transform.childCount; i++)
                    {
                        if (newRoom.transform.GetChild(i).CompareTag("RealSpawnPoint"))
                        {
                            neighbourGameRoomSpawnPoints = newRoom.transform.GetChild(i).gameObject;
                        }
                    }

                    // Move the player to the new room
                    for (int i = 0; i < neighbourGameRoomSpawnPoints.transform.childCount; i++)
                    {
                        if (neighbourGameRoomSpawnPoints.transform.GetChild(i).CompareTag("North"))
                        {
                            SetUpRoomTeleport(TravelDirection.SOUTH);
                            isPlayerTeleporting = true;

                            StartCoroutine("MovePlayerToNextRoom", neighbourGameRoomSpawnPoints.transform.GetChild(i).GetComponent<GameRoomSpawner>().playerEntranceSpawn.transform.position);

                            neighbourGameRoomSpawnPoints.transform.GetChild(i).GetComponent<GameRoomSpawner>().connectedGameRoom = thisGameRoom;
                            neighbourGameRoomSpawnPoints.transform.GetChild(i).GetComponent<GameRoomSpawner>().thisMinimapRoom = nextMinimapRoom;
                            neighbourGameRoomSpawnPoints.transform.GetChild(i).GetComponent<GameRoomSpawner>().nextMinimapRoom = thisMinimapRoom;

                            // Follow player location
                            StartCoroutine(nameof(MovePlayerLocationInMinimap));
                        }
                    }
                }

                else
                {
                    // Get the player spawnpoints
                    for (int i = 0; i < connectedGameRoom.transform.childCount; i++)
                    {
                        if (connectedGameRoom.transform.GetChild(i).CompareTag("RealSpawnPoint"))
                        {
                            neighbourGameRoomSpawnPoints = connectedGameRoom.transform.GetChild(i).gameObject;
                        }
                    }

                    for (int i = 0; i < roomSpawnPoints.Count; i++)
                    {
                        if (roomSpawnPoints[i].GetComponent<MinimapRoomSpawner>()?.doorNeeded == DoorOrientation.TOP)
                        {
                            nextMinimapRoom = roomSpawnPoints[i].GetComponent<MinimapRoomSpawner>().nextMinimapRoom;
                        }
                    }

                    // Move the player to the new room
                    for (int i = 0; i < neighbourGameRoomSpawnPoints.transform.childCount; i++)
                    {
                        if (neighbourGameRoomSpawnPoints.transform.GetChild(i).CompareTag("North"))
                        {
                            SetUpRoomTeleport(TravelDirection.SOUTH);
                            isPlayerTeleporting = true;

                            StartCoroutine("MovePlayerToNextRoom", neighbourGameRoomSpawnPoints.transform.GetChild(i).GetComponent<GameRoomSpawner>().playerEntranceSpawn.transform.position);

                            neighbourGameRoomSpawnPoints.transform.GetChild(i).GetComponent<GameRoomSpawner>().thisMinimapRoom = nextMinimapRoom;
                            neighbourGameRoomSpawnPoints.transform.GetChild(i).GetComponent<GameRoomSpawner>().nextMinimapRoom = thisMinimapRoom;
                            dungeonMapManager.currentMinimapRoom = nextMinimapRoom;

                            // Follow player location
                            StartCoroutine(nameof(MovePlayerLocationInMinimap));
                        }
                    }
                }
            }

            else if (CompareTag("East"))
            {
                foreach (GameObject createdRoom in dungeonMapManager.spawnedGameRooms)
                {
                    if (createdRoom.transform.position == new Vector3(transform.position.x + 25, transform.position.y, transform.position.z))
                    {
                        connectedGameRoom = createdRoom;
                    }
                }

                if (connectedGameRoom == null)
                {
                    GameObject newRoom = null;
                    for (int i = 0; i < roomSpawnPoints.Count; i++)
                    {
                        if (roomSpawnPoints[i].GetComponent<MinimapRoomSpawner>()?.doorNeeded == DoorOrientation.LEFT)
                        {
                            foreach (GameObject room in RoomsHolderSingleton.Instance.realRooms)
                            {
                                //Debug.Log(spawnPoints.transform.parent.gameObject.name + ", " + spawnPoints.gameObject.name);
                                if (room.name + "(Clone)" == "Room " + roomSpawnPoints[i].GetComponent<MinimapRoomSpawner>().nextMinimapRoom.name)
                                {
                                    newRoom = room;
                                    thisMinimapRoom = roomSpawnPoints[i].GetComponent<MinimapRoomSpawner>().thisMinimapRoom;
                                    nextMinimapRoom = roomSpawnPoints[i].GetComponent<MinimapRoomSpawner>().nextMinimapRoom;
                                    dungeonMapManager.currentMinimapRoom = nextMinimapRoom;

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
                        for (int i = 0; i < nextMinimapRoom?.transform.childCount; i++)
                        {
                            if (nextMinimapRoom.transform.GetChild(i).CompareTag("SpawnPoint"))
                            {
                                newSpawnpoints = nextMinimapRoom.transform.GetChild(i).gameObject;
                            }
                        }

                        for (int i = 0; i < newSpawnpoints.transform.childCount; i++)
                        {
                            //Debug.Log(spawnPoints.transform.GetChild(i).GetComponent<RoomSpawner>().nextRoom);
                            if (newSpawnpoints.transform.GetChild(i).GetComponent<MinimapRoomSpawner>().nextMinimapRoom != null)
                                openings++;
                        }

                        //Debug.Log(openings);
                        //Debug.Log(newRoom.name);

                        if (openings > 1)
                        {
                            newRoom = Instantiate(newRoom, new Vector3(transform.position.x + 25, transform.position.y, transform.position.z), newRoom.transform.rotation, dungeonMapManager.gameRoomsParent);
                            connectedGameRoom = newRoom;
                            dungeonMapManager.spawnedGameRooms.Add(newRoom);

                            if (nextMinimapRoom.CompareTag("BossRoom") && newRoom.transform.childCount > 5)
                            {
                                newRoom.transform.GetChild(4).gameObject.SetActive(false);
                                newRoom.transform.GetChild(5).gameObject.SetActive(true);
                            }
                        }

                        else
                        {
                            if (newRoom.name == "Room TL" || newRoom.name == "Room LB" || newRoom.name == "Room LR")
                            {
                                foreach (GameObject room in RoomsHolderSingleton.Instance.realRooms)
                                {
                                    if (room.name == "Room L")
                                        newRoom = room;
                                }

                                newRoom = Instantiate(newRoom, new Vector3(transform.position.x + 25, transform.position.y, transform.position.z), newRoom.transform.rotation, dungeonMapManager.gameRoomsParent);
                                connectedGameRoom = newRoom;
                                dungeonMapManager.spawnedGameRooms.Add(newRoom);

                                if (nextMinimapRoom.CompareTag("BossRoom") && newRoom.transform.childCount > 5)
                                {
                                    newRoom.transform.GetChild(4).gameObject.SetActive(false);
                                    newRoom.transform.GetChild(5).gameObject.SetActive(true);
                                }

                                nextGameRoom = Instantiate(RoomsHolderSingleton.Instance.leftMinimapRoom, nextMinimapRoom.transform.position, RoomsHolderSingleton.Instance.leftMinimapRoom.transform.rotation, dungeonMapManager.minimapRoomsParent);
                                nextGameRoom.SetActive(false);
                            }
                        }
                    }

                    else
                    {
                        newRoom = Instantiate(newRoom, new Vector3(transform.position.x + 25, transform.position.y, transform.position.z), newRoom.transform.rotation, dungeonMapManager.gameRoomsParent);
                        connectedGameRoom = newRoom;
                        dungeonMapManager.spawnedGameRooms.Add(newRoom);

                        if (nextMinimapRoom.CompareTag("BossRoom") && newRoom.transform.childCount > 5)
                        {
                            newRoom.transform.GetChild(4).gameObject.SetActive(false);
                            newRoom.transform.GetChild(5).gameObject.SetActive(true);
                        }
                    }

                    // Get the player spawnpoints
                    for (int i = 0; i < newRoom.transform.childCount; i++)
                    {
                        if (newRoom.transform.GetChild(i).CompareTag("RealSpawnPoint"))
                        {
                            neighbourGameRoomSpawnPoints = newRoom.transform.GetChild(i).gameObject;
                        }
                    }

                    // Move the player to the new room
                    for (int i = 0; i < neighbourGameRoomSpawnPoints.transform.childCount; i++)
                    {
                        if (neighbourGameRoomSpawnPoints.transform.GetChild(i).CompareTag("West"))
                        {
                            SetUpRoomTeleport(TravelDirection.EAST);
                            isPlayerTeleporting = true;

                            StartCoroutine("MovePlayerToNextRoom", neighbourGameRoomSpawnPoints.transform.GetChild(i).GetComponent<GameRoomSpawner>().playerEntranceSpawn.transform.position);

                            neighbourGameRoomSpawnPoints.transform.GetChild(i).GetComponent<GameRoomSpawner>().connectedGameRoom = thisGameRoom;
                            neighbourGameRoomSpawnPoints.transform.GetChild(i).GetComponent<GameRoomSpawner>().thisMinimapRoom = nextMinimapRoom;
                            neighbourGameRoomSpawnPoints.transform.GetChild(i).GetComponent<GameRoomSpawner>().nextMinimapRoom = thisMinimapRoom;

                            // Follow player location
                            StartCoroutine(nameof(MovePlayerLocationInMinimap));
                        }
                    }
                }

                else
                {
                    // Get the player spawnpoints
                    for (int i = 0; i < connectedGameRoom.transform.childCount; i++)
                    {
                        if (connectedGameRoom.transform.GetChild(i).CompareTag("RealSpawnPoint"))
                        {
                            neighbourGameRoomSpawnPoints = connectedGameRoom.transform.GetChild(i).gameObject;
                        }
                    }

                    for (int i = 0; i < roomSpawnPoints.Count; i++)
                    {
                        if (roomSpawnPoints[i].GetComponent<MinimapRoomSpawner>()?.doorNeeded == DoorOrientation.LEFT)
                        {
                            thisMinimapRoom = roomSpawnPoints[i].GetComponent<MinimapRoomSpawner>().thisMinimapRoom;
                            nextMinimapRoom = roomSpawnPoints[i].GetComponent<MinimapRoomSpawner>().nextMinimapRoom;
                        }
                    }

                    // Move the player to the new room
                    for (int i = 0; i < neighbourGameRoomSpawnPoints.transform.childCount; i++)
                    {
                        if (neighbourGameRoomSpawnPoints.transform.GetChild(i).CompareTag("West"))
                        {
                            SetUpRoomTeleport(TravelDirection.EAST);
                            isPlayerTeleporting = true;

                            StartCoroutine(nameof(MovePlayerToNextRoom), neighbourGameRoomSpawnPoints.transform.GetChild(i).GetComponent<GameRoomSpawner>().playerEntranceSpawn.transform.position);

                            neighbourGameRoomSpawnPoints.transform.GetChild(i).GetComponent<GameRoomSpawner>().thisMinimapRoom = nextMinimapRoom;
                            neighbourGameRoomSpawnPoints.transform.GetChild(i).GetComponent<GameRoomSpawner>().nextMinimapRoom = thisMinimapRoom;
                            dungeonMapManager.currentMinimapRoom = nextMinimapRoom;

                            // Follow player location
                            StartCoroutine(nameof(MovePlayerLocationInMinimap));
                        }
                    }
                }
            }

            else if (CompareTag("West"))
            {
                foreach (GameObject createdRoom in dungeonMapManager.spawnedGameRooms)
                {
                    if (createdRoom.transform.position == new Vector3(transform.position.x - 25, transform.position.y, transform.position.z))
                    {
                        connectedGameRoom = createdRoom;
                    }
                }

                if (connectedGameRoom == null)
                {
                    GameObject newRoom = null;
                    for (int i = 0; i < roomSpawnPoints.Count; i++)
                    {
                        if (roomSpawnPoints[i].GetComponent<MinimapRoomSpawner>()?.doorNeeded == DoorOrientation.RIGHT)
                        {
                            foreach (GameObject room in RoomsHolderSingleton.Instance.realRooms)
                            {
                                //Debug.Log(spawnPoints.transform.parent.gameObject.name + ", " + spawnPoints.gameObject.name);
                                if (room.name + "(Clone)" == "Room " + roomSpawnPoints[i].GetComponent<MinimapRoomSpawner>().nextMinimapRoom.name)
                                {
                                    newRoom = room;
                                    thisMinimapRoom = roomSpawnPoints[i].GetComponent<MinimapRoomSpawner>().thisMinimapRoom;
                                    nextMinimapRoom = roomSpawnPoints[i].GetComponent<MinimapRoomSpawner>().nextMinimapRoom;
                                    dungeonMapManager.currentMinimapRoom = nextMinimapRoom;

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
                        for (int i = 0; i < nextMinimapRoom?.transform.childCount; i++)
                        {
                            if (nextMinimapRoom.transform.GetChild(i).CompareTag("SpawnPoint"))
                            {
                                newSpawnpoints = nextMinimapRoom.transform.GetChild(i).gameObject;
                            }
                        }

                        for (int i = 0; i < newSpawnpoints.transform.childCount; i++)
                        {
                            //Debug.Log(spawnPoints.transform.GetChild(i).GetComponent<RoomSpawner>().nextRoom);
                            if (newSpawnpoints.transform.GetChild(i).GetComponent<MinimapRoomSpawner>().nextMinimapRoom != null)
                                openings++;
                        }

                        //Debug.Log(openings);
                        //Debug.Log(newRoom.name);

                        if (openings > 1)
                        {
                            newRoom = Instantiate(newRoom, new Vector3(transform.position.x - 25, transform.position.y, transform.position.z), newRoom.transform.rotation, dungeonMapManager.gameRoomsParent);
                            connectedGameRoom = newRoom;
                            dungeonMapManager.spawnedGameRooms.Add(newRoom);

                            if (nextMinimapRoom.CompareTag("BossRoom") && newRoom.transform.childCount > 5)
                            {
                                newRoom.transform.GetChild(4).gameObject.SetActive(false);
                                newRoom.transform.GetChild(5).gameObject.SetActive(true);
                            }
                        }

                        else
                        {
                            if (newRoom.name == "Room TR" || newRoom.name == "Room LR" || newRoom.name == "Room RB")
                            {
                                foreach (GameObject room in RoomsHolderSingleton.Instance.realRooms)
                                {
                                    if (room.name == "Room R")
                                        newRoom = room;
                                }

                                newRoom = Instantiate(newRoom, new Vector3(transform.position.x - 25, transform.position.y, transform.position.z), newRoom.transform.rotation, dungeonMapManager.gameRoomsParent);
                                connectedGameRoom = newRoom;
                                dungeonMapManager.spawnedGameRooms.Add(newRoom);

                                if (nextMinimapRoom.CompareTag("BossRoom") && newRoom.transform.childCount > 5)
                                {
                                    newRoom.transform.GetChild(4).gameObject.SetActive(false);
                                    newRoom.transform.GetChild(5).gameObject.SetActive(true);
                                }

                                nextGameRoom = Instantiate(RoomsHolderSingleton.Instance.rightMinimapRoom, nextMinimapRoom.transform.position, RoomsHolderSingleton.Instance.rightMinimapRoom.transform.rotation, dungeonMapManager.minimapRoomsParent);
                                nextGameRoom.SetActive(false);
                            }
                        }
                    }

                    else
                    {
                        newRoom = Instantiate(newRoom, new Vector3(transform.position.x - 25, transform.position.y, transform.position.z), newRoom.transform.rotation, dungeonMapManager.gameRoomsParent);
                        connectedGameRoom = newRoom;
                        dungeonMapManager.spawnedGameRooms.Add(newRoom);

                        if (nextMinimapRoom.CompareTag("BossRoom") && newRoom.transform.childCount > 5)
                        {
                            newRoom.transform.GetChild(4).gameObject.SetActive(false);
                            newRoom.transform.GetChild(5).gameObject.SetActive(true);
                        }
                    }

                    // Get the player spawnpoints
                    for (int i = 0; i < newRoom.transform.childCount; i++)
                    {
                        if (newRoom.transform.GetChild(i).CompareTag("RealSpawnPoint"))
                        {
                            neighbourGameRoomSpawnPoints = newRoom.transform.GetChild(i).gameObject;
                        }
                    }

                    // Move the player to the new room
                    for (int i = 0; i < neighbourGameRoomSpawnPoints.transform.childCount; i++)
                    {
                        if (neighbourGameRoomSpawnPoints.transform.GetChild(i).CompareTag("East"))
                        {
                            SetUpRoomTeleport(TravelDirection.WEST);
                            isPlayerTeleporting = true;

                            StartCoroutine(nameof(MovePlayerToNextRoom), neighbourGameRoomSpawnPoints.transform.GetChild(i).GetComponent<GameRoomSpawner>().playerEntranceSpawn.transform.position);

                            neighbourGameRoomSpawnPoints.transform.GetChild(i).GetComponent<GameRoomSpawner>().connectedGameRoom = thisGameRoom;
                            neighbourGameRoomSpawnPoints.transform.GetChild(i).GetComponent<GameRoomSpawner>().thisMinimapRoom = nextMinimapRoom;
                            neighbourGameRoomSpawnPoints.transform.GetChild(i).GetComponent<GameRoomSpawner>().nextMinimapRoom = thisMinimapRoom;

                            // Follow player location
                            StartCoroutine(nameof(MovePlayerLocationInMinimap));
                        }
                    }
                }

                else
                {
                    // Get the player spawnpoints
                    for (int i = 0; i < connectedGameRoom.transform.childCount; i++)
                    {
                        if (connectedGameRoom.transform.GetChild(i).CompareTag("RealSpawnPoint"))
                        {
                            neighbourGameRoomSpawnPoints = connectedGameRoom.transform.GetChild(i).gameObject;
                        }
                    }

                    for (int i = 0; i < roomSpawnPoints.Count; i++)
                    {
                        if (roomSpawnPoints[i].GetComponent<MinimapRoomSpawner>()?.doorNeeded == DoorOrientation.RIGHT)
                        {
                            nextMinimapRoom = roomSpawnPoints[i].GetComponent<MinimapRoomSpawner>().nextMinimapRoom;
                        }
                    }

                    // Move the player to the new room
                    for (int i = 0; i < neighbourGameRoomSpawnPoints.transform.childCount; i++)
                    {
                        if (neighbourGameRoomSpawnPoints.transform.GetChild(i).CompareTag("East"))
                        {
                            SetUpRoomTeleport(TravelDirection.WEST);
                            isPlayerTeleporting = true;

                            StartCoroutine(nameof(MovePlayerToNextRoom), neighbourGameRoomSpawnPoints.transform.GetChild(i).GetComponent<GameRoomSpawner>().playerEntranceSpawn.transform.position);

                            neighbourGameRoomSpawnPoints.transform.GetChild(i).GetComponent<GameRoomSpawner>().thisMinimapRoom = nextMinimapRoom;
                            neighbourGameRoomSpawnPoints.transform.GetChild(i).GetComponent<GameRoomSpawner>().nextMinimapRoom = thisMinimapRoom;
                            dungeonMapManager.currentMinimapRoom = nextMinimapRoom;

                            // Follow player location
                            StartCoroutine(nameof(MovePlayerLocationInMinimap));
                        }
                    }
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        canPlayerTriggerTeleport = false;
    }

    void SetUpRoomTeleport(TravelDirection travelDirection)
    {
        switch (travelDirection)
        {
            case TravelDirection.NORTH:
                directionToGoWhileTeleporting = new Vector2(-teleportingXMovement, teleportingYMovement);
                directionToLookWhileTeleporting = new Vector2(-1, 1);
                break;

            case TravelDirection.SOUTH:
                directionToGoWhileTeleporting = new Vector2(teleportingXMovement, -teleportingYMovement);
                directionToLookWhileTeleporting = new Vector2(1, -1);
                break;

            case TravelDirection.EAST:
                directionToGoWhileTeleporting = new Vector2(teleportingXMovement, teleportingYMovement);
                directionToLookWhileTeleporting = new Vector2(1, 1);
                break;

            case TravelDirection.WEST:
                directionToGoWhileTeleporting = new Vector2(-teleportingXMovement, -teleportingYMovement);
                directionToLookWhileTeleporting = new Vector2(-1, -1);
                break;

            default:
                directionToGoWhileTeleporting = Vector2.zero;
                directionToLookWhileTeleporting = Vector2.zero;
                break;
        }
    }

    void PerformTeleportMovement()
    {
        if (exitAnimTime >= 0.0f)
        {
            ManageExitRoomTeleportAnimation();
            exitAnimTime -= Time.deltaTime;
        }
        else if (enterAnimTime >= 0.0f)
        {
            ManageEnterRoomTeleportAnimation();
            enterAnimTime -= Time.deltaTime;
        }
        else
        {
            RestoreDefaultTeleportAnimationValues();
            isPlayerTeleporting = false;
        }
    }

    void ManageExitRoomTeleportAnimation()
    {
        dungeonMapManager.changeRoomAnim.SetBool("ChangeRoom", true);
        player.transform.GetChild(0).GetComponent<Collider2D>().isTrigger = true;
        player.transform.position += directionToGoWhileTeleporting;
        playerAnimationDirection.SetDirection(directionToLookWhileTeleporting);
    }

    void ManageEnterRoomTeleportAnimation()
    {
        dungeonMapManager.changeRoomAnim.SetBool("ChangeRoom", false);
        player.transform.position += directionToGoWhileTeleporting;
        playerAnimationDirection.SetDirection(directionToLookWhileTeleporting);
    }

    void RestoreDefaultTeleportAnimationValues()
    {
        playerAnimationDirection.SetDirection(new Vector2(0, 0));
        player.transform.GetChild(0).GetComponent<Collider2D>().isTrigger = false;
        exitAnimTime = 0.75f;
        enterAnimTime = 0.75f;
    }

    IEnumerator MovePlayerToNextRoom(Vector3 roomSpawnerDestiny)
    {
        yield return new WaitForSeconds(0.75f);

        nextMinimapRoom.SetActive(true);
        nextGameRoom.SetActive(true);

        player.transform.position = roomSpawnerDestiny;
    }

    IEnumerator MovePlayerLocationInMinimap()
    {
        yield return new WaitForSeconds(0.75f);

        dungeonMapManager.playerMinimapPosition.transform.parent = nextMinimapRoom.transform;
        dungeonMapManager.playerMinimapPosition.transform.position = nextMinimapRoom.transform.position;

        yield return new WaitForSeconds(2.0f);

        changeRoomKeyPressed = false;
    }
}