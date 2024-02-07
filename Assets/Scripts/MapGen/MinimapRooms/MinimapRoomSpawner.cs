using System.Collections.Generic;
using UnityEngine;

public class MinimapRoomSpawner : MonoBehaviour
{
	private DungeonMapManager dungeonMapManager;
	private Camera mainCamera;

    [HideInInspector] public MinimapRoom thisMinimapRoom;

    [HideInInspector] public GameObject nextMinimapRoomObject;

    public DoorOrientation doorNeeded;

    [HideInInspector] public bool hasRoomConnected;

	private readonly float minimapBottomLimit = 0.038f;
	private readonly float minimapTopLimit = 0.962f;
	private readonly float minimapLeftLimit = 0.03f;
	private readonly float minimapRightLimit = 0.97f;

    void Start()
	{
		dungeonMapManager = GameObject.FindGameObjectWithTag("DungeonMngr").GetComponent<DungeonMapManager>();
		mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

		thisMinimapRoom = transform.parent.transform.parent.GetComponent<MinimapRoom>();

        // TODO: Check that the room created matches both sides

		// Delay ManageSpawnRoom() a certain time in case the new room needs more open directions
		Invoke(nameof(ManageSpawnRoom), Time.fixedDeltaTime);
	}

	void ManageSpawnRoom()
	{
		if (hasRoomConnected)
		{
			return;
		}

        if (IsRoomInsideMinimapLimits())
		{
			SpawnRoomWithOrientation(doorNeeded);
		}
		else
		{
            SpawnRoomWithOrientation(doorNeeded, true);
        }

        hasRoomConnected = true;
    }

	void OnTriggerEnter2D(Collider2D other)
	{
		if (!other.CompareTag("SpawnPoint") || hasRoomConnected)
		{
			return;
		}

        // Another room was hit but without connection, so that direction is a dead end
        if (other.GetComponent<MinimapRoomSpawner>().hasRoomConnected &&
            other.GetComponent<MinimapRoomSpawner>().nextMinimapRoomObject != thisMinimapRoom)
        {
            RemoveOrientationFromDoor();
        }

        else if (other != null)
        {
            SpawnRoomWithTwoOrientations(doorNeeded, other.GetComponent<MinimapRoomSpawner>().doorNeeded, other);

            other.GetComponent<MinimapRoomSpawner>().hasRoomConnected = true;
            hasRoomConnected = true;
        }
    }

    void RemoveOrientationFromDoor()
    {
        GameObject limitWall = gameObject.transform.GetChild(0).gameObject;
        limitWall.SetActive(true);

        string spawnPointName = gameObject.name;
        char letterToRemove = spawnPointName[^1];

        string newRoomName = "";
        for (int i = 0; i < thisMinimapRoom.name.Length; i++)
        {
            if (thisMinimapRoom.name[i] == letterToRemove)
            {
                continue;
            }

            newRoomName += thisMinimapRoom.name[i];
        }
        thisMinimapRoom.name = newRoomName;

        this.enabled = false;
        hasRoomConnected = true;
    }

    void SpawnRoomWithOrientation(DoorOrientation newRoomOrientation, bool hasToBeLimitRoom = false)
	{
		GameObject roomToSpawn;
        Vector3 newRoomPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);

        if (hasToBeLimitRoom)
		{
			roomToSpawn = DoorOrientationToRooms.GetMinimapRoomWithOneDirection(newRoomOrientation);
		}
        else
		{
            GameObject[] roomsToSelect = DoorOrientationToRooms.GetMinimapRoomsWithOneDirection(newRoomOrientation);
			if (roomsToSelect == null)
			{
				return;
			}
            int randomRoomSelected = Random.Range(0, roomsToSelect.Length);

            roomToSpawn = roomsToSelect[randomRoomSelected];
        }

        if (roomToSpawn != null)
        {
            GameObject newRoom = Instantiate(roomToSpawn, newRoomPosition, 
                                             roomToSpawn.transform.rotation, dungeonMapManager.minimapRoomsParent);
            dungeonMapManager.spawnedMinimapRooms.Add(newRoom);
            nextMinimapRoomObject = newRoom;

            DoorOrientation oppositeDoorOrientation = DoorOrientationToRooms.GetOppositeOrientation(newRoomOrientation);
            SetConnectionsBetweenRooms(newRoom, oppositeDoorOrientation);
        }
    }

    void SpawnRoomWithTwoOrientations(DoorOrientation connection1, DoorOrientation connection2, Collider2D otherRoomToConnect)
    {
        GameObject roomToSpawn = DoorOrientationToRooms.GetMinimapRoomWithTwoDirections(connection1, connection2);
        Vector3 roomToSpawnPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);

        if (roomToSpawn != null)
        {
            GameObject newRoom = Instantiate(roomToSpawn, roomToSpawnPosition, 
                                             roomToSpawn.transform.rotation, dungeonMapManager.minimapRoomsParent);
            dungeonMapManager.spawnedMinimapRooms.Add(newRoom);
            nextMinimapRoomObject = newRoom;
            otherRoomToConnect.GetComponent<MinimapRoomSpawner>().nextMinimapRoomObject = newRoom;

            DoorOrientation oppositeDoorOrientation1 = DoorOrientationToRooms.GetOppositeOrientation(connection1);
            DoorOrientation oppositeDoorOrientation2 = DoorOrientationToRooms.GetOppositeOrientation(connection2);
            SetConnectionsBetweenRooms(newRoom, oppositeDoorOrientation1, oppositeDoorOrientation2, otherRoomToConnect);
        }
    }

    void SetConnectionsBetweenRooms(GameObject nextMinimapRoom, DoorOrientation connectionToSet1, 
                                    DoorOrientation connectionToSet2 = DoorOrientation.INVALID, Collider2D otherSpawner = null)
	{
        List<MinimapRoomSpawner> nextMinimapRoomSpawnPoints = nextMinimapRoom.GetComponent<MinimapRoom>().minimapRoomSpawnPoints;

        for (int i = 0; i < nextMinimapRoomSpawnPoints.Count; i++)
        {
            MinimapRoomSpawner nextRoomSpawner = nextMinimapRoomSpawnPoints[i].gameObject.GetComponent<MinimapRoomSpawner>();
            if (nextRoomSpawner.doorNeeded == connectionToSet1)
            {
                nextRoomSpawner.hasRoomConnected = true;
                nextRoomSpawner.nextMinimapRoomObject = thisMinimapRoom.thisMinimapRoomObject;
            }
            else if (nextRoomSpawner.doorNeeded == connectionToSet2)
            {
                nextRoomSpawner.hasRoomConnected = true;
                nextRoomSpawner.nextMinimapRoomObject = 
                                otherSpawner.GetComponent<MinimapRoomSpawner>().thisMinimapRoom.thisMinimapRoomObject;
            }
        }
    }

	bool IsRoomInsideMinimapLimits()
	{
		return mainCamera.WorldToViewportPoint(transform.position).x < minimapRightLimit &&
			   mainCamera.WorldToViewportPoint(transform.position).x > minimapLeftLimit &&
			   mainCamera.WorldToViewportPoint(transform.position).y < minimapTopLimit &&
			   mainCamera.WorldToViewportPoint(transform.position).y > minimapBottomLimit;
    }
}