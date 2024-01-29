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

    private List<GameObject> minimapRoomSpawnPoints;
    private List<GameObject> neighbourMinimapRoomSpawnPoints;

    private GameObject neighbourGameRoomSpawnPoints;

    private GameObject thisMinimapRoom;
    private GameObject nextMinimapRoom;

    private GameObject thisGameRoom; // TODO: Link between game rooms is broken now, fix it
    private GameObject nextGameRoom;

    [SerializeField] private GameObject playerEntranceSpawn;

    private bool changeRoomKeyPressed = false;

    // ANIMATION STUFF

    private float exitAnimTime = 0.75f;
    private float enterAnimTime = 0.75f;

    private void Start()
    {
        dungeonMapManager = GameObject.FindGameObjectWithTag("DungeonMngr").GetComponent<DungeonMapManager>();

        thisGameRoom = transform.parent.transform.parent.gameObject;

        player = GameObject.FindGameObjectWithTag("Player");
        playerAnimationDirection = player.GetComponent<PlayerAnimationDirection>();
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
        if (other.CompareTag("Player") && 
            Input.GetKey(KeyCode.E) && !changeRoomKeyPressed && 
            canPlayerTriggerTeleport && !isPlayerTeleporting)
        {
            changeRoomKeyPressed = true;
            canPlayerTriggerTeleport = false;

            thisMinimapRoom = dungeonMapManager.currentMinimapRoom;
            minimapRoomSpawnPoints = thisMinimapRoom.GetComponent<MinimapRoomSpawner>().minimapRoomSpawnPoints;

            if (CompareTag("North"))
            {
                Vector3 neighborRoomPos = new Vector3(transform.position.x, transform.position.y + 15, transform.position.z);

                if (!IsThereARoomAtPosition(neighborRoomPos))
                {
                    CreateRoomAtPositionWithOrientation(neighborRoomPos, DoorOrientation.BOTTOM);

                    bool nextRoomIsBossRoom = nextMinimapRoom.CompareTag("BossRoom");
                    bool zodiacExistsInRoom = nextGameRoom.transform.childCount > 5;
                    if (nextRoomIsBossRoom && zodiacExistsInRoom)
                    {
                        ManageZodiacBossActivation();
                    }
                }
                else
                {
                    LinkExistingRoomWithCurrentRoom(DoorOrientation.BOTTOM);
                }

                // Get the player spawnpoints
                for (int i = 0; i < nextGameRoom.transform.childCount; i++)
                {
                    if (nextGameRoom.transform.GetChild(i).CompareTag("RealSpawnPoint"))
                    {
                        neighbourGameRoomSpawnPoints = nextGameRoom.transform.GetChild(i).gameObject;
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

            // --------------------------------------------------------------

            else if (CompareTag("South"))
            {
                Vector3 neighborRoomPos = new Vector3(transform.position.x, transform.position.y - 15, transform.position.z);

                if (!IsThereARoomAtPosition(neighborRoomPos))
                {
                    CreateRoomAtPositionWithOrientation(neighborRoomPos, DoorOrientation.TOP);

                    bool nextRoomIsBossRoom = nextMinimapRoom.CompareTag("BossRoom");
                    bool zodiacExistsInRoom = nextGameRoom.transform.childCount > 5;
                    if (nextRoomIsBossRoom && zodiacExistsInRoom)
                    {
                        ManageZodiacBossActivation();
                    }
                }
                else
                {
                    LinkExistingRoomWithCurrentRoom(DoorOrientation.TOP);
                }

                // Get the player spawnpoints
                for (int i = 0; i < nextGameRoom.transform.childCount; i++)
                {
                    if (nextGameRoom.transform.GetChild(i).CompareTag("RealSpawnPoint"))
                    {
                        neighbourGameRoomSpawnPoints = nextGameRoom.transform.GetChild(i).gameObject;
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

            // --------------------------------------------------------------

            else if (CompareTag("East"))
            {
                Vector3 neighborRoomPos = new Vector3(transform.position.x + 25, transform.position.y, transform.position.z);

                if (!IsThereARoomAtPosition(neighborRoomPos))
                {
                    CreateRoomAtPositionWithOrientation(neighborRoomPos, DoorOrientation.LEFT);

                    bool nextRoomIsBossRoom = nextMinimapRoom.CompareTag("BossRoom");
                    bool zodiacExistsInRoom = nextGameRoom.transform.childCount > 5;
                    if (nextRoomIsBossRoom && zodiacExistsInRoom)
                    {
                        ManageZodiacBossActivation();
                    }
                }
                else
                {
                    LinkExistingRoomWithCurrentRoom(DoorOrientation.LEFT);
                }

                // Get the player spawnpoints
                for (int i = 0; i < nextGameRoom.transform.childCount; i++)
                {
                    if (nextGameRoom.transform.GetChild(i).CompareTag("RealSpawnPoint"))
                    {
                        neighbourGameRoomSpawnPoints = nextGameRoom.transform.GetChild(i).gameObject;
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

            // --------------------------------------------------------------

            else if (CompareTag("West"))
            {
                Vector3 neighborRoomPos = new Vector3(transform.position.x - 25, transform.position.y, transform.position.z);

                if (!IsThereARoomAtPosition(neighborRoomPos))
                {
                    CreateRoomAtPositionWithOrientation(neighborRoomPos, DoorOrientation.RIGHT);

                    bool nextRoomIsBossRoom = nextMinimapRoom.CompareTag("BossRoom");
                    bool zodiacExistsInRoom = nextGameRoom.transform.childCount > 5;
                    if (nextRoomIsBossRoom && zodiacExistsInRoom)
                    {
                        ManageZodiacBossActivation();
                    }
                }
                else
                {
                    LinkExistingRoomWithCurrentRoom(DoorOrientation.RIGHT);
                }

                // Get the player spawnpoints
                for (int i = 0; i < nextGameRoom.transform.childCount; i++)
                {
                    if (nextGameRoom.transform.GetChild(i).CompareTag("RealSpawnPoint"))
                    {
                        neighbourGameRoomSpawnPoints = nextGameRoom.transform.GetChild(i).gameObject;
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

                        // Follow player location
                        StartCoroutine(nameof(MovePlayerLocationInMinimap));
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

    GameObject GetNeededGameRoomToSpawn(DoorOrientation requiredOrientation)
    {
        GameObject newRoomToSpawn = null;
        for (int i = 0; i < minimapRoomSpawnPoints.Count; i++)
        {
            MinimapRoomSpawner currentMinimapSpawner = minimapRoomSpawnPoints[i].GetComponent<MinimapRoomSpawner>();
            if (currentMinimapSpawner != null && currentMinimapSpawner.doorNeeded == requiredOrientation)
            {
                foreach (GameObject gameRoom in RoomsHolderSingleton.Instance.gameRooms)
                {
                    // Room TB"(Clone)" == "Room "TB(Clone) for example
                    if (gameRoom.name + "(Clone)" == "Room " + currentMinimapSpawner.nextMinimapRoom.name)
                    {
                        newRoomToSpawn = gameRoom;
                        nextMinimapRoom = currentMinimapSpawner.nextMinimapRoom;

                        break;
                    }
                }

                break;
            }
        }

        return newRoomToSpawn;
    }

    bool IsThereARoomAtPosition(Vector3 roomPos)
    {
        foreach (GameObject createdRoom in dungeonMapManager.spawnedGameRooms)
        {
            if (createdRoom.transform.position == roomPos)
            {
                nextGameRoom = createdRoom;
                return true;
            }
        }

        return false;
    }

    void CreateRoomAtPositionWithOrientation(Vector3 roomPos, DoorOrientation roomOrientation)
    {
        GameObject newGameRoomToSpawnPrefab = GetNeededGameRoomToSpawn(roomOrientation);
        nextGameRoom = Instantiate(newGameRoomToSpawnPrefab, roomPos,
                                   newGameRoomToSpawnPrefab.transform.rotation, dungeonMapManager.gameRoomsParent);

        dungeonMapManager.spawnedGameRooms.Add(nextGameRoom);
    }

    void ManageZodiacBossActivation()
    {
        GameObject enemySpawners = nextGameRoom.transform.GetChild(4).gameObject;
        GameObject zodiacFight = nextGameRoom.transform.GetChild(5).gameObject;

        enemySpawners.SetActive(false);
        zodiacFight.SetActive(true);
    }

    void LinkExistingRoomWithCurrentRoom(DoorOrientation roomOrientation)
    {
        for (int i = 0; i < minimapRoomSpawnPoints.Count; i++)
        {
            MinimapRoomSpawner currentMinimapSpawner = minimapRoomSpawnPoints[i].GetComponent<MinimapRoomSpawner>();
            if (currentMinimapSpawner != null && currentMinimapSpawner.doorNeeded == roomOrientation)
            {
                nextMinimapRoom = currentMinimapSpawner.nextMinimapRoom;
            }
        }
    }

    IEnumerator MovePlayerToNextRoom(Vector3 roomSpawnerDestiny)
    {
        yield return new WaitForSeconds(0.75f);

        nextMinimapRoom.SetActive(true);
        nextGameRoom.SetActive(true);

        dungeonMapManager.currentMinimapRoom = nextMinimapRoom;
        dungeonMapManager.currentGameRoom = nextGameRoom;

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