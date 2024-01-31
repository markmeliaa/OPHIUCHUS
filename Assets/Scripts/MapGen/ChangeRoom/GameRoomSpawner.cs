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

    private GameObject thisMinimapRoom;
    private GameObject nextMinimapRoom;

    private GameObject thisGameRoom;
    private GameObject nextGameRoom;

    [SerializeField] private GameObject playerEntranceSpawn;

    private bool changeRoomKeyPressed;

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

    // Manage the change of rooms
    private void OnTriggerStay2D(Collider2D other)
    {
        bool wasTPInteracted = Input.GetKey(KeyCode.E) && !changeRoomKeyPressed;

        if (!other.CompareTag("Player") || !wasTPInteracted || isPlayerTeleporting)
        {
            return;
        }

        changeRoomKeyPressed = true;

        thisMinimapRoom = dungeonMapManager.currentMinimapRoom;

        // TODO: Spawnpoints should be per room, not per spawnpoint
        for (int i = 0; i < thisMinimapRoom.transform.childCount; i++)
        {
            if (thisMinimapRoom.transform.GetChild(i).CompareTag("SpawnPoints"))
            {
                GameObject firstSpawnpoint = thisMinimapRoom.transform.GetChild(i).GetChild(0).gameObject;
                minimapRoomSpawnPoints = firstSpawnpoint.GetComponent<MinimapRoomSpawner>().minimapRoomSpawnPoints;
            }
        }

        Vector3 neighborRoomPos;
        DoorOrientation orientationForOppositeDoor;
        TravelDirection directionToTravel;
        string oppositeTag;

        switch(tag)
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

    private void FixedUpdate()
    {
        if (!isPlayerTeleporting)
        {
            return;
        }

        PerformTeleportMovement();
    }

    void ManageTeleportingLogic(Vector3 neighborRoomPos, DoorOrientation oppositeOrientation, string oppositeTag,
                                TravelDirection directionToTravel)
    {
        nextGameRoom = GetRoomAtPosition(neighborRoomPos);

        if (nextGameRoom == null)
        {
            CreateRoomAtPositionWithOrientation(neighborRoomPos, oppositeOrientation);

            bool nextRoomIsBossRoom = nextMinimapRoom.CompareTag("BossRoom");
            bool zodiacExistsInRoom = nextGameRoom.transform.childCount > 5;
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
        nextGameRoom = Instantiate(newGameRoomToSpawnPrefab, roomPos,
                                   newGameRoomToSpawnPrefab.transform.rotation, dungeonMapManager.gameRoomsParent);

        dungeonMapManager.spawnedGameRooms.Add(nextGameRoom);
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

    void ManageZodiacBossActivation()
    {
        GameObject enemySpawners = nextGameRoom.transform.GetChild(4).gameObject;
        GameObject zodiacFight = nextGameRoom.transform.GetChild(5).gameObject;

        enemySpawners.SetActive(false);
        zodiacFight.SetActive(true);
    }

    void LinkMinimapRooms(DoorOrientation roomOrientation)
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

    GameObject GetNeighbourRoomSpawnPointsParent()
    {
        GameObject neighbourGameRoomSpawnPoints = null;
        for (int i = 0; i < nextGameRoom.transform.childCount; i++)
        {
            if (nextGameRoom.transform.GetChild(i).CompareTag("RealSpawnPoints"))
            {
                neighbourGameRoomSpawnPoints = nextGameRoom.transform.GetChild(i).gameObject;
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
                isPlayerTeleporting = true;
                GameRoomSpawner selectedNeighbourSpawnPoint = currentSpawnPoint.GetComponent<GameRoomSpawner>();

                SetUpRoomTeleport(directionToTravel);

                StartCoroutine(nameof(MovePlayerToNextRoom), 
                               selectedNeighbourSpawnPoint.playerEntranceSpawn.transform.position);

                StartCoroutine(nameof(MovePlayerLocationInMinimap));

                // -- THIS ARE JUST TO ASSURE THAT BOTH ROOMS ARE CORRECTLY CONNECTED --
                selectedNeighbourSpawnPoint.thisMinimapRoom = nextMinimapRoom;
                selectedNeighbourSpawnPoint.nextMinimapRoom = thisMinimapRoom;

                selectedNeighbourSpawnPoint.nextGameRoom = thisGameRoom;
                selectedNeighbourSpawnPoint.thisGameRoom = nextGameRoom;
                // ---------------------------------------------------------------------
            }
        }
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
        player.GetComponent<Collider2D>().isTrigger = true;
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
        player.GetComponent<Collider2D>().isTrigger = false;
        exitAnimTime = 0.75f;
        enterAnimTime = 0.75f;
    }
}