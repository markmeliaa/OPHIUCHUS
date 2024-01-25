using UnityEditor;
using UnityEngine;

public enum DoorOrientation
{
	INVALID,
	BOTTOM,
	TOP,
	LEFT,
	RIGHT
};

public class RoomSpawner : MonoBehaviour
{
	private RoomTemplates templates;
	private Camera mainCamera;

    public DoorOrientation doorNeeded;

     public bool hasRoomConnected;
	 public GameObject currentRoom;
	 public GameObject nextRoom;

	private readonly float minimapBottomLimit = 0.038f;
	private readonly float minimapTopLimit = 0.962f;
	private readonly float minimapLeftLimit = 0.03f;
	private readonly float minimapRightLimit = 0.97f;

	void Start()
	{
		templates = GameObject.FindGameObjectWithTag("Rooms").GetComponent<RoomTemplates>();
		mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

		Transform spawnpointsParentTransform = transform.parent;
		currentRoom = spawnpointsParentTransform.parent.gameObject;

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
             other.GetComponent<RoomSpawner>().hasRoomConnected || 
             hasRoomConnected)
		{
            hasRoomConnected = true; // Another room was hit but without connection, so mark it as already connected
			return;
		}

        SpawnRoomWithTwoOrientations(doorNeeded, other.GetComponent<RoomSpawner>().doorNeeded, other);

        other.GetComponent<RoomSpawner>().hasRoomConnected = true;
        hasRoomConnected = true;
    }

	void SpawnRoomWithOrientation(DoorOrientation newRoomOrientation, bool hasToBeLimitRoom = false)
	{
		GameObject roomToSpawn;
        Vector3 newRoomPosition = new Vector3(transform.position.x, transform.position.y - templates.roomOffset, transform.position.z);

        if (hasToBeLimitRoom)
		{
			roomToSpawn = GetLimitRoomOfCertainDirection(newRoomOrientation);
		}
        else
		{
            GameObject[] roomsToSelect = GetTemplateRoomsOfCertainDirection(newRoomOrientation);
			if (roomsToSelect == null)
			{
				return;
			}
            int randomRoomSelected = Random.Range(0, roomsToSelect.Length);

            roomToSpawn = roomsToSelect[randomRoomSelected];
        }

        GameObject newRoom = Instantiate(roomToSpawn, newRoomPosition, roomToSpawn.transform.rotation, templates.roomsParent);
        nextRoom = newRoom;

        DoorOrientation oppositeDoorOrientation = GetOppositeOrientation(newRoomOrientation);
        SetConnectionsBetweenRooms(newRoom, oppositeDoorOrientation);
    }

	DoorOrientation GetOppositeOrientation(DoorOrientation orientation)
	{
		DoorOrientation oppositeOrientation = DoorOrientation.INVALID;
		switch(orientation)
		{
            case DoorOrientation.BOTTOM:
                oppositeOrientation = DoorOrientation.TOP;
                break;

            case DoorOrientation.TOP:
                oppositeOrientation = DoorOrientation.BOTTOM;
                break;

            case DoorOrientation.LEFT:
                oppositeOrientation = DoorOrientation.RIGHT;
                break;

            case DoorOrientation.RIGHT:
                oppositeOrientation = DoorOrientation.LEFT;
                break;

			default:
				break;
        }

		return oppositeOrientation;
	}

    GameObject GetLimitRoomOfCertainDirection(DoorOrientation direction)
    {
        GameObject room = null;
        switch (direction)
        {
            case DoorOrientation.BOTTOM:
                room = templates.B;
                break;

            case DoorOrientation.TOP:
                room = templates.T;
                break;

            case DoorOrientation.LEFT:
                room = templates.L;
                break;

            case DoorOrientation.RIGHT:
                room = templates.R;
                break;

            default:
                break;
        }

        return room;
    }

    GameObject[] GetTemplateRoomsOfCertainDirection(DoorOrientation direction)
	{
		GameObject[] rooms = null;
        switch (direction)
        {
            case DoorOrientation.BOTTOM:
                rooms = templates.bottomRooms;
                break;

            case DoorOrientation.TOP:
                rooms = templates.topRooms;
                break;

            case DoorOrientation.LEFT:
                rooms = templates.leftRooms;
                break;

            case DoorOrientation.RIGHT:
                rooms = templates.rightRooms;
                break;

            default:
                break;
        }

		return rooms;
    }

    void SpawnRoomWithTwoOrientations(DoorOrientation connection1, DoorOrientation connection2, Collider2D otherSpawner)
    {
        GameObject roomToSpawn = GetRoomWithTwoDirections(connection1, connection2);
        Vector3 roomToSpawnPosition = new Vector3(transform.position.x, transform.position.y - templates.roomOffset, transform.position.z);

        GameObject newRoom = Instantiate(roomToSpawn, roomToSpawnPosition, roomToSpawn.transform.rotation, templates.roomsParent);
        nextRoom = newRoom;
        otherSpawner.GetComponent<RoomSpawner>().nextRoom = newRoom;

        DoorOrientation oppositeDoorOrientation1 = GetOppositeOrientation(connection1);
        DoorOrientation oppositeDoorOrientation2 = GetOppositeOrientation(connection2);
        SetConnectionsBetweenRooms(newRoom, oppositeDoorOrientation1, oppositeDoorOrientation2, otherSpawner);
    }

    GameObject GetRoomWithTwoDirections(DoorOrientation orientation1,  DoorOrientation orientation2) 
    {
        GameObject room = null;
        if (orientation2 < orientation1)
        {
            DoorOrientation aux = orientation2;
            orientation2 = orientation1;
            orientation1 = aux;
        }

        switch (orientation1)
        {
            case DoorOrientation.BOTTOM:
                if (orientation2 == DoorOrientation.TOP)
                {
                    room = templates.TB;
                }
                else if (orientation2 == DoorOrientation.LEFT)
                {
                    room = templates.LB;
                }
                else if (orientation2 == DoorOrientation.RIGHT)
                {
                    room = templates.RB;
                }
                break;

            case DoorOrientation.TOP:
                if (orientation2 == DoorOrientation.LEFT)
                {
                    room = templates.TL;
                }
                else if (orientation2 == DoorOrientation.RIGHT)
                {
                    room = templates.TR;
                }
                break;

            case DoorOrientation.LEFT:
                room = templates.LR;
                break;

            default:
                break;
        }

        return room;
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
            RoomSpawner nextRoomSpawner = nextRoomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>();
            if (nextRoomSpawner.doorNeeded == connectionToSet1)
            {
                nextRoomSpawner.hasRoomConnected = true;
                nextRoomSpawner.nextRoom = currentRoom;
            }
            else if (nextRoomSpawner.doorNeeded == connectionToSet2)
            {
                nextRoomSpawner.hasRoomConnected = true;
                nextRoomSpawner.nextRoom = otherSpawner.GetComponent<RoomSpawner>().currentRoom;
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