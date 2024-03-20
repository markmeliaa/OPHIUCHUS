using System.Collections;
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
    private GameObject player;
    private PlayerMovement playerMovement;

    private DungeonMapManager dungeonMapManager;

    private GameRoom thisGameRoom;

    [HideInInspector] public GameObject nextMinimapRoom;
    [HideInInspector] public GameObject nextGameRoomObject;

    [SerializeField] private GameObject playerEntranceSpawn;

    private bool changeRoomKeyPressed;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerMovement = player.GetComponent<PlayerMovement>();

        dungeonMapManager = GameObject.FindGameObjectWithTag("DungeonMngr").GetComponent<DungeonMapManager>();

        thisGameRoom = transform.parent.transform.parent.GetComponent<GameRoom>();
    }

    // Manage the change of rooms
    private void OnTriggerStay2D(Collider2D other)
    {
        bool wasTPInteracted = Input.GetKey(KeyCode.E) && !changeRoomKeyPressed;

        if (!other.CompareTag("Player") || !wasTPInteracted || thisGameRoom.changeRoomAnim.isPlayerTeleporting)
        {
            return;
        }

        changeRoomKeyPressed = true;

        thisGameRoom.thisMinimapRoomObject = dungeonMapManager.currentMinimapRoom;
        thisGameRoom.thisMinimapRoomObject.GetComponent<MinimapRoom>().thisGameRoomObject = thisGameRoom.thisGameRoomObject;

        thisGameRoom.thisMinimapRoomSpawnPoints = thisGameRoom.thisMinimapRoomObject.GetComponent<MinimapRoom>().minimapRoomSpawnPoints;

        Vector3 neighborRoomPos;
        DoorOrientation orientationForOppositeDoor;
        TravelDirection directionToTravel;
        string oppositeTag;

        switch (tag)
        {
            case "North":
                neighborRoomPos = new Vector3(transform.position.x, transform.position.y + 15, transform.position.z);
                orientationForOppositeDoor = DoorOrientation.BOTTOM;
                directionToTravel = TravelDirection.NORTH;
                oppositeTag = "South";
                break;

            case "South":
                neighborRoomPos = new Vector3(transform.position.x, transform.position.y - 15, transform.position.z);
                orientationForOppositeDoor = DoorOrientation.TOP;
                directionToTravel = TravelDirection.SOUTH;
                oppositeTag = "North";
                break;

            case "East":
                neighborRoomPos = new Vector3(transform.position.x + 25, transform.position.y, transform.position.z);
                orientationForOppositeDoor = DoorOrientation.LEFT;
                directionToTravel = TravelDirection.EAST;
                oppositeTag = "West";
                break;

            case "West":
                neighborRoomPos = new Vector3(transform.position.x - 25, transform.position.y, transform.position.z);
                orientationForOppositeDoor = DoorOrientation.RIGHT;
                directionToTravel = TravelDirection.WEST;
                oppositeTag = "East";
                break;

            default:
                neighborRoomPos = Vector3.zero;
                orientationForOppositeDoor = DoorOrientation.INVALID;
                directionToTravel = TravelDirection.NONE;
                oppositeTag = "";
                break;
        }

        ManageTeleportingLogic(neighborRoomPos, orientationForOppositeDoor, oppositeTag, directionToTravel);
    }

    void ManageTeleportingLogic(Vector3 neighborRoomPos, DoorOrientation oppositeOrientation, string oppositeTag,
                                TravelDirection directionToTravel)
    {
        nextGameRoomObject = GetRoomAtPosition(neighborRoomPos);

        if (nextGameRoomObject == null)
        {
            CreateRoomAtPositionWithOrientation(neighborRoomPos, oppositeOrientation);

            bool nextRoomIsBossRoom = nextMinimapRoom.CompareTag("BossRoom");
            bool zodiacExistsInRoom = nextGameRoomObject.transform.childCount > 5;
            if (nextRoomIsBossRoom && zodiacExistsInRoom)
            {
                ManageZodiacBossActivation();
            }
        }
        else
        {
            LinkMinimapRooms(oppositeOrientation);
        }

        GameObject neighbourRoomSpawnPointsParent = GetNeighbourRoomSpawnPointsParent();
        MovePlayerToNextRoomSpawnPoint(oppositeTag, neighbourRoomSpawnPointsParent, directionToTravel);
    }

    GameObject GetRoomAtPosition(Vector3 roomPos)
    {
        foreach (GameObject existingRoom in dungeonMapManager.spawnedGameRooms)
        {
            if (existingRoom.transform.position == roomPos)
            {
                return existingRoom;
            }
        }

        return null;
    }

    void CreateRoomAtPositionWithOrientation(Vector3 roomPos, DoorOrientation roomOrientation)
    {
        GameObject newGameRoomToSpawnPrefab = GetNeededGameRoomToSpawn(roomOrientation);
        nextGameRoomObject = Instantiate(newGameRoomToSpawnPrefab, roomPos,
                                         newGameRoomToSpawnPrefab.transform.rotation, dungeonMapManager.gameRoomsParent);

        dungeonMapManager.spawnedGameRooms.Add(nextGameRoomObject);
    }

    GameObject GetNeededGameRoomToSpawn(DoorOrientation requiredOrientation)
    {
        GameObject newRoomToSpawn = null;
        for (int i = 0; i < thisGameRoom.thisMinimapRoomSpawnPoints.Count; i++)
        {
            MinimapRoomSpawner currentMinimapSpawner = thisGameRoom.thisMinimapRoomSpawnPoints[i].GetComponent<MinimapRoomSpawner>();
            if (currentMinimapSpawner != null && currentMinimapSpawner.doorNeeded == requiredOrientation)
            {
                foreach (GameObject gameRoom in RoomsHolderSingleton.Instance.gameRooms)
                {
                    // Room TB"(Clone)" == "Room "TB(Clone) for example
                    if (gameRoom.name + "(Clone)" == "Room " + currentMinimapSpawner.nextMinimapRoomObject.name)
                    {
                        newRoomToSpawn = gameRoom;
                        nextMinimapRoom = currentMinimapSpawner.nextMinimapRoomObject;

                        break;
                    }
                }

                break;
            }
        }

        return newRoomToSpawn;
    }

    void ManageZodiacBossActivation()
    {
        GameObject enemySpawners = nextGameRoomObject.transform.GetChild(4).gameObject;
        GameObject zodiacFight = nextGameRoomObject.transform.GetChild(5).gameObject;

        enemySpawners.SetActive(false);
        zodiacFight.SetActive(true);
    }

    void LinkMinimapRooms(DoorOrientation roomOrientation)
    {
        for (int i = 0; i < thisGameRoom.thisMinimapRoomSpawnPoints.Count; i++)
        {
            MinimapRoomSpawner currentMinimapSpawner = thisGameRoom.thisMinimapRoomSpawnPoints[i].GetComponent<MinimapRoomSpawner>();
            if (currentMinimapSpawner != null && currentMinimapSpawner.doorNeeded == roomOrientation)
            {
                nextMinimapRoom = currentMinimapSpawner.nextMinimapRoomObject;
            }
        }
    }

    GameObject GetNeighbourRoomSpawnPointsParent()
    {
        GameObject neighbourGameRoomSpawnPoints = null;
        for (int i = 0; i < nextGameRoomObject.transform.childCount; i++)
        {
            if (nextGameRoomObject.transform.GetChild(i).CompareTag("RealSpawnPoints"))
            {
                neighbourGameRoomSpawnPoints = nextGameRoomObject.transform.GetChild(i).gameObject;
            }
        }

        return neighbourGameRoomSpawnPoints;
    }

    void MovePlayerToNextRoomSpawnPoint(string requiredSpawnPointDirection, GameObject neighbourRoomSpawnPointsParent,
                                        TravelDirection directionToTravel)
    {
        for (int i = 0; i < neighbourRoomSpawnPointsParent.transform.childCount; i++)
        {
            GameObject currentSpawnPoint = neighbourRoomSpawnPointsParent.transform.GetChild(i).gameObject;
            if (currentSpawnPoint.CompareTag(requiredSpawnPointDirection))
            {
                thisGameRoom.changeRoomAnim.isPlayerTeleporting = true;
                playerMovement.canMove = false;

                thisGameRoom.changeRoomAnim.SetUpRoomTeleport(directionToTravel);

                GameRoomSpawner selectedNeighbourSpawnPoint = currentSpawnPoint.GetComponent<GameRoomSpawner>();
                StartCoroutine(nameof(MovePlayerToNextRoom), 
                               selectedNeighbourSpawnPoint.playerEntranceSpawn.transform.position);

                StartCoroutine(nameof(MovePlayerLocationInMinimap));
            }
        }
    }

    IEnumerator MovePlayerToNextRoom(Vector3 roomSpawnerDestiny)
    {
        yield return new WaitForSeconds(0.75f);

        nextMinimapRoom.SetActive(true);
        nextGameRoomObject.SetActive(true);

        dungeonMapManager.currentMinimapRoom = nextMinimapRoom;
        dungeonMapManager.currentGameRoom = nextGameRoomObject;

        player.transform.position = roomSpawnerDestiny;
    }

    IEnumerator MovePlayerLocationInMinimap()
    {
        yield return new WaitForSeconds(0.75f);

        dungeonMapManager.playerMinimapPosition.transform.parent = dungeonMapManager.currentMinimapRoom.transform;
        dungeonMapManager.playerMinimapPosition.transform.position = dungeonMapManager.currentMinimapRoom.transform.position;

        yield return new WaitForSeconds(2.0f);

        changeRoomKeyPressed = false;
    }
}