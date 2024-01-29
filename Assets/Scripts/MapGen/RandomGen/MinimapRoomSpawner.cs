using UnityEditor;
using UnityEngine;

public class MinimapRoomSpawner : MonoBehaviour
{
	private DungeonMapManager dungeonMapManager;
	private Camera mainCamera;

    public DoorOrientation doorNeeded;

    [HideInInspector] public bool hasRoomConnected;
	[HideInInspector] public GameObject thisMinimapRoom;
	[HideInInspector] public GameObject nextMinimapRoom;

	private readonly float minimapBottomLimit = 0.038f;
	private readonly float minimapTopLimit = 0.962f;
	private readonly float minimapLeftLimit = 0.03f;
	private readonly float minimapRightLimit = 0.97f;

    // TODO: When in Andromeda, switch this to 0.022f
    private float roomOffset = 0.0f;

	void Start()
	{
		dungeonMapManager = GameObject.FindGameObjectWithTag("DungeonMngr").GetComponent<DungeonMapManager>();
		mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

		Transform spawnpointsParentTransform = transform.parent;
		thisMinimapRoom = spawnpointsParentTransform.parent.gameObject;

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
		if (!other.CompareTag("SpawnPoint") || 
             other.GetComponent<MinimapRoomSpawner>().hasRoomConnected || 
             hasRoomConnected)
		{
            hasRoomConnected = true; // Another room was hit but without connection, so mark it as already connected
			return;
		}

        if (other != null)
        {
            SpawnRoomWithTwoOrientations(doorNeeded, other.GetComponent<MinimapRoomSpawner>().doorNeeded, other);

            other.GetComponent<MinimapRoomSpawner>().hasRoomConnected = true;
            hasRoomConnected = true;
        }
    }

	void SpawnRoomWithOrientation(DoorOrientation newRoomOrientation, bool hasToBeLimitRoom = false)
	{
		GameObject roomToSpawn;
        Vector3 newRoomPosition = new Vector3(transform.position.x, transform.position.y - roomOffset, transform.position.z);

        if (hasToBeLimitRoom)
		{
			roomToSpawn = DoorOrientationToRooms.GetLimitRoomOfOneDirection(newRoomOrientation);
		}
        else
		{
            GameObject[] roomsToSelect = DoorOrientationToRooms.GetTemplateRoomsOfOneDirection(newRoomOrientation);
			if (roomsToSelect == null)
			{
				return;
			}
            int randomRoomSelected = Random.Range(0, roomsToSelect.Length);

            roomToSpawn = roomsToSelect[randomRoomSelected];
        }

        if (roomToSpawn != null)
        {
            GameObject newRoom = Instantiate(roomToSpawn, newRoomPosition, roomToSpawn.transform.rotation, dungeonMapManager.minimapRoomsParent);
            dungeonMapManager.spawnedMinimapRooms.Add(newRoom);
            nextMinimapRoom = newRoom;

            DoorOrientation oppositeDoorOrientation = DoorOrientationToRooms.GetOppositeOrientation(newRoomOrientation);
            SetConnectionsBetweenRooms(newRoom, oppositeDoorOrientation);
        }
    }

    void SpawnRoomWithTwoOrientations(DoorOrientation connection1, DoorOrientation connection2, Collider2D otherRoomToConnect)
    {
        GameObject roomToSpawn = DoorOrientationToRooms.GetTemplateRoomWithTwoDirections(connection1, connection2);
        Vector3 roomToSpawnPosition = new Vector3(transform.position.x, transform.position.y - roomOffset, transform.position.z);

        if (roomToSpawn != null)
        {
            GameObject newRoom = Instantiate(roomToSpawn, roomToSpawnPosition, roomToSpawn.transform.rotation, dungeonMapManager.minimapRoomsParent);
            dungeonMapManager.spawnedMinimapRooms.Add(newRoom);
            nextMinimapRoom = newRoom;
            otherRoomToConnect.GetComponent<MinimapRoomSpawner>().nextMinimapRoom = newRoom;

            DoorOrientation oppositeDoorOrientation1 = DoorOrientationToRooms.GetOppositeOrientation(connection1);
            DoorOrientation oppositeDoorOrientation2 = DoorOrientationToRooms.GetOppositeOrientation(connection2);
            SetConnectionsBetweenRooms(newRoom, oppositeDoorOrientation1, oppositeDoorOrientation2, otherRoomToConnect);
        }
    }

    void SetConnectionsBetweenRooms(GameObject nextRoom, DoorOrientation connectionToSet1, 
                                    DoorOrientation connectionToSet2 = DoorOrientation.INVALID, Collider2D otherSpawner = null)
	{
        GameObject nextRoomSpawnpoints = null;
        for (int i = 0; i < nextRoom.transform.childCount; i++)
        {
            if (nextRoom.transform.GetChild(i).name == "Spawn Points")
			{
                nextRoomSpawnpoints = nextRoom.transform.GetChild(i).gameObject;
            }
        }

        for (int i = 0; i < nextRoomSpawnpoints?.transform.childCount; i++)
        {
            MinimapRoomSpawner nextRoomSpawner = nextRoomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<MinimapRoomSpawner>();
            if (nextRoomSpawner.doorNeeded == connectionToSet1)
            {
                nextRoomSpawner.hasRoomConnected = true;
                nextRoomSpawner.nextMinimapRoom = thisMinimapRoom;
            }
            else if (nextRoomSpawner.doorNeeded == connectionToSet2)
            {
                nextRoomSpawner.hasRoomConnected = true;
                nextRoomSpawner.nextMinimapRoom = otherSpawner.GetComponent<MinimapRoomSpawner>().thisMinimapRoom;
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