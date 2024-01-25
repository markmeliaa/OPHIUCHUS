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

    [HideInInspector] public bool hasRoomConnected;
	[HideInInspector] public GameObject currentRoom;
	[HideInInspector] public GameObject nextRoom;

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

		// Call a function after certain time
		Invoke(nameof(Spawn), Time.fixedDeltaTime);
	}

	void Spawn()
	{
		if (hasRoomConnected)
		{
			return;
		}

        hasRoomConnected = true;

        if (IsRoomInsideMinimapLimits())
		{
			SpawnRoomWithOrientation(doorNeeded);
		}
		else
		{
            SpawnRoomWithOrientation(doorNeeded, true);
        }
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		templates = GameObject.FindGameObjectWithTag("Rooms").GetComponent<RoomTemplates>();
		if (other.CompareTag("SpawnPoint"))
		{
			if (other.GetComponent<RoomSpawner>().hasRoomConnected == false && !hasRoomConnected)
			{
				//Debug.Log("Collision at " + this.gameObject.transform.parent.transform.parent.transform.position + " with " + other.GetComponent<RoomSpawner>().gameObject.transform.parent.transform.parent.transform.position);
				//Debug.Log(openingDirection + ", " + other.GetComponent<RoomSpawner>().openingDirection);
				
				if (doorNeeded == DoorOrientation.BOTTOM)
                {
					if (other.GetComponent<RoomSpawner>().doorNeeded == DoorOrientation.TOP)
                    {
						//Debug.Log("Spawn TB");
						// Need to spawn a room with a BOTTOM TOP door
						GameObject newRoom = Instantiate(templates.TB, new Vector3(transform.position.x, transform.position.y - templates.roomOffset, transform.position.z), templates.TB.transform.rotation, templates.roomsParent);
						nextRoom = newRoom;
						other.GetComponent<RoomSpawner>().nextRoom = newRoom;

						GameObject roomSpawnpoints = null;
						for (int i = 0; i < newRoom.transform.childCount; i++)
						{
							if (newRoom.transform.GetChild(i).name == "Spawn Points")
								roomSpawnpoints = newRoom.transform.GetChild(i).gameObject;
						}

						for (int i = 0; i < roomSpawnpoints?.transform.childCount; i++)
						{
							if (roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().doorNeeded == DoorOrientation.TOP)
							{
								roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().hasRoomConnected = true;
								roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().nextRoom = gameObject.transform.parent.transform.parent.gameObject;
							}

							if (roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().doorNeeded == DoorOrientation.BOTTOM)
							{
								roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().hasRoomConnected = true;
								roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().nextRoom = other.GetComponent<RoomSpawner>().gameObject.transform.parent.transform.parent.gameObject;
							}
						}
					}

					else if (other.GetComponent<RoomSpawner>().doorNeeded == DoorOrientation.LEFT)
					{
						//Debug.Log("Spawn LB");
						// Need to spawn a room with a BOTTOM LEFT door
						GameObject newRoom = Instantiate(templates.LB, new Vector3(transform.position.x, transform.position.y - templates.roomOffset, transform.position.z), templates.LB.transform.rotation, templates.roomsParent);
						nextRoom = newRoom;
						other.GetComponent<RoomSpawner>().nextRoom = newRoom;

						GameObject roomSpawnpoints = null;
						for (int i = 0; i < newRoom.transform.childCount; i++)
						{
							if (newRoom.transform.GetChild(i).name == "Spawn Points")
								roomSpawnpoints = newRoom.transform.GetChild(i).gameObject;
						}

						for (int i = 0; i < roomSpawnpoints?.transform.childCount; i++)
						{
							if (roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().doorNeeded == DoorOrientation.TOP)
							{
								roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().hasRoomConnected = true;
								roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().nextRoom = gameObject.transform.parent.transform.parent.gameObject;
							}

							if (roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().doorNeeded == DoorOrientation.RIGHT)
							{
								roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().hasRoomConnected = true;
								roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().nextRoom = other.GetComponent<RoomSpawner>().gameObject.transform.parent.transform.parent.gameObject;
							}
						}
					}

					else if (other.GetComponent<RoomSpawner>().doorNeeded == DoorOrientation.RIGHT)
					{
						//Debug.Log("Spawn RB");
						// Need to spawn a room with a BOTTOM RIGHT door
						GameObject newRoom = Instantiate(templates.RB, new Vector3(transform.position.x, transform.position.y - templates.roomOffset, transform.position.z), templates.RB.transform.rotation, templates.roomsParent);
						nextRoom = newRoom;
						other.GetComponent<RoomSpawner>().nextRoom = newRoom;

						GameObject roomSpawnpoints = null;
						for (int i = 0; i < newRoom.transform.childCount; i++)
						{
							if (newRoom.transform.GetChild(i).name == "Spawn Points")
								roomSpawnpoints = newRoom.transform.GetChild(i).gameObject;
						}

						for (int i = 0; i < roomSpawnpoints?.transform.childCount; i++)
						{
							if (roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().doorNeeded == DoorOrientation.TOP)
							{
								roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().hasRoomConnected = true;
								roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().nextRoom = gameObject.transform.parent.transform.parent.gameObject;
							}

							if (roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().doorNeeded == DoorOrientation.LEFT)
							{
								roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().hasRoomConnected = true;
								roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().nextRoom = other.GetComponent<RoomSpawner>().gameObject.transform.parent.transform.parent.gameObject;
							}
						}
					}
				}

				else if (doorNeeded == DoorOrientation.TOP)
                {
					if (other.GetComponent<RoomSpawner>().doorNeeded == DoorOrientation.BOTTOM)
					{
						//Debug.Log("Spawn TB");
						// Need to spawn a room with a TOP BOTTOM door
						GameObject newRoom = Instantiate(templates.TB, new Vector3(transform.position.x, transform.position.y + templates.roomOffset, transform.position.z), templates.TB.transform.rotation, templates.roomsParent);
						nextRoom = newRoom;
						other.GetComponent<RoomSpawner>().nextRoom = newRoom;

						GameObject roomSpawnpoints = null;
						for (int i = 0; i < newRoom.transform.childCount; i++)
						{
							if (newRoom.transform.GetChild(i).name == "Spawn Points")
								roomSpawnpoints = newRoom.transform.GetChild(i).gameObject;
						}

						for (int i = 0; i < roomSpawnpoints?.transform.childCount; i++)
						{
							if (roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().doorNeeded == DoorOrientation.BOTTOM)
							{
								roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().hasRoomConnected = true;
								roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().nextRoom = gameObject.transform.parent.transform.parent.gameObject;
							}

							if (roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().doorNeeded == DoorOrientation.TOP)
							{
								roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().hasRoomConnected = true;
								roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().nextRoom = other.GetComponent<RoomSpawner>().gameObject.transform.parent.transform.parent.gameObject;
							}
						}
					}

					else if (other.GetComponent<RoomSpawner>().doorNeeded == DoorOrientation.LEFT)
					{
						//Debug.Log("Spawn TL");
						// Need to spawn a room with a TOP LEFT door
						GameObject newRoom = Instantiate(templates.TL, new Vector3(transform.position.x, transform.position.y + templates.roomOffset, transform.position.z), templates.TL.transform.rotation, templates.roomsParent);
						nextRoom = newRoom;
						other.GetComponent<RoomSpawner>().nextRoom = newRoom;

						GameObject roomSpawnpoints = null;
						for (int i = 0; i < newRoom.transform.childCount; i++)
						{
							if (newRoom.transform.GetChild(i).name == "Spawn Points")
								roomSpawnpoints = newRoom.transform.GetChild(i).gameObject;
						}

						for (int i = 0; i < roomSpawnpoints?.transform.childCount; i++)
						{
							if (roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().doorNeeded == DoorOrientation.BOTTOM)
							{
								roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().hasRoomConnected = true;
								roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().nextRoom = gameObject.transform.parent.transform.parent.gameObject;
							}

							if (roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().doorNeeded == DoorOrientation.RIGHT)
							{
								roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().hasRoomConnected = true;
								roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().nextRoom = other.GetComponent<RoomSpawner>().gameObject.transform.parent.transform.parent.gameObject;
							}
						}
					}

					else if (other.GetComponent<RoomSpawner>().doorNeeded == DoorOrientation.RIGHT)
					{
						//Debug.Log("Spawn TR");
						// Need to spawn a room with a TOP RIGHT door
						GameObject newRoom = Instantiate(templates.TR, new Vector3(transform.position.x, transform.position.y + templates.roomOffset, transform.position.z), templates.TR.transform.rotation, templates.roomsParent);
						nextRoom = newRoom;
						other.GetComponent<RoomSpawner>().nextRoom = newRoom;

						GameObject roomSpawnpoints = null;
						for (int i = 0; i < newRoom.transform.childCount; i++)
						{
							if (newRoom.transform.GetChild(i).name == "Spawn Points")
								roomSpawnpoints = newRoom.transform.GetChild(i).gameObject;
						}

						for (int i = 0; i < roomSpawnpoints?.transform.childCount; i++)
						{
							if (roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().doorNeeded == DoorOrientation.BOTTOM)
							{
								roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().hasRoomConnected = true;
								roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().nextRoom = gameObject.transform.parent.transform.parent.gameObject;
							}

							if (roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().doorNeeded == DoorOrientation.LEFT)
							{
								roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().hasRoomConnected = true;
								roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().nextRoom = other.GetComponent<RoomSpawner>().gameObject.transform.parent.transform.parent.gameObject;
							}
						}
					}
				}

				else if (doorNeeded == DoorOrientation.LEFT)
				{
					if (other.GetComponent<RoomSpawner>().doorNeeded == DoorOrientation.BOTTOM)
					{
						//Debug.Log("Spawn LB");
						// Need to spawn a room with a LEFT BOTTOM door
						GameObject newRoom = Instantiate(templates.LB, new Vector3(transform.position.x - templates.roomOffset, transform.position.y, transform.position.z), templates.LB.transform.rotation, templates.roomsParent);
						nextRoom = newRoom;
						other.GetComponent<RoomSpawner>().nextRoom = newRoom;

						GameObject roomSpawnpoints = null;
						for (int i = 0; i < newRoom.transform.childCount; i++)
						{
							if (newRoom.transform.GetChild(i).name == "Spawn Points")
								roomSpawnpoints = newRoom.transform.GetChild(i).gameObject;
						}

						for (int i = 0; i < roomSpawnpoints?.transform.childCount; i++)
						{
							if (roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().doorNeeded == DoorOrientation.RIGHT)
							{
								roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().hasRoomConnected = true;
								roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().nextRoom = gameObject.transform.parent.transform.parent.gameObject;
							}

							if (roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().doorNeeded == DoorOrientation.TOP)
							{
								roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().hasRoomConnected = true;
								roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().nextRoom = other.GetComponent<RoomSpawner>().gameObject.transform.parent.transform.parent.gameObject;
							}
						}
					}

					else if (other.GetComponent<RoomSpawner>().doorNeeded == DoorOrientation.TOP)
					{
						//Debug.Log("Spawn TL");
						// Need to spawn a room with a LEFT TOP door
						GameObject newRoom = Instantiate(templates.TL, new Vector3(transform.position.x - templates.roomOffset, transform.position.y, transform.position.z), templates.TL.transform.rotation, templates.roomsParent);
						nextRoom = newRoom;
						other.GetComponent<RoomSpawner>().nextRoom = newRoom;

						GameObject roomSpawnpoints = null;
						for (int i = 0; i < newRoom.transform.childCount; i++)
						{
							if (newRoom.transform.GetChild(i).name == "Spawn Points")
								roomSpawnpoints = newRoom.transform.GetChild(i).gameObject;
						}

						for (int i = 0; i < roomSpawnpoints?.transform.childCount; i++)
						{
							if (roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().doorNeeded == DoorOrientation.RIGHT)
							{
								roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().hasRoomConnected = true;
								roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().nextRoom = gameObject.transform.parent.transform.parent.gameObject;
							}

							if (roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().doorNeeded == DoorOrientation.BOTTOM)
							{
								roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().hasRoomConnected = true;
								roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().nextRoom = other.GetComponent<RoomSpawner>().gameObject.transform.parent.transform.parent.gameObject;
							}
						}
					}

					else if (other.GetComponent<RoomSpawner>().doorNeeded == DoorOrientation.RIGHT)
					{
						//Debug.Log("Spawn LR");
						// Need to spawn a room with a LEFT RIGHT door
						GameObject newRoom = Instantiate(templates.LR, new Vector3(transform.position.x - templates.roomOffset, transform.position.y, transform.position.z), templates.LR.transform.rotation, templates.roomsParent);
						nextRoom = newRoom;
						other.GetComponent<RoomSpawner>().nextRoom = newRoom;

						GameObject roomSpawnpoints = null;
						for (int i = 0; i < newRoom.transform.childCount; i++)
						{
							if (newRoom.transform.GetChild(i).name == "Spawn Points")
								roomSpawnpoints = newRoom.transform.GetChild(i).gameObject;
						}

						for (int i = 0; i < roomSpawnpoints?.transform.childCount; i++)
						{
							if (roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().doorNeeded == DoorOrientation.RIGHT)
							{
								roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().hasRoomConnected = true;
								roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().nextRoom = gameObject.transform.parent.transform.parent.gameObject;
							}

							if (roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().doorNeeded == DoorOrientation.LEFT)
							{
								roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().hasRoomConnected = true;
								roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().nextRoom = other.GetComponent<RoomSpawner>().gameObject.transform.parent.transform.parent.gameObject;
							}
						}
					}
				}

				else if (doorNeeded == DoorOrientation.RIGHT)
				{
					if (other.GetComponent<RoomSpawner>().doorNeeded == DoorOrientation.BOTTOM)
					{
						//Debug.Log("Spawn RB");
						// Need to spawn a room with a RIGHT BOTTOM door
						GameObject newRoom = Instantiate(templates.RB, new Vector3(transform.position.x + templates.roomOffset, transform.position.y, transform.position.z), templates.RB.transform.rotation, templates.roomsParent);
						nextRoom = newRoom;
						other.GetComponent<RoomSpawner>().nextRoom = newRoom;

						GameObject roomSpawnpoints = null;
						for (int i = 0; i < newRoom.transform.childCount; i++)
						{
							if (newRoom.transform.GetChild(i).name == "Spawn Points")
								roomSpawnpoints = newRoom.transform.GetChild(i).gameObject;
						}

						for (int i = 0; i < roomSpawnpoints?.transform.childCount; i++)
						{
							if (roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().doorNeeded == DoorOrientation.LEFT)
							{
								roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().hasRoomConnected = true;
								roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().nextRoom = gameObject.transform.parent.transform.parent.gameObject;
							}

							if (roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().doorNeeded == DoorOrientation.TOP)
							{
								roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().hasRoomConnected = true;
								roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().nextRoom = other.GetComponent<RoomSpawner>().gameObject.transform.parent.transform.parent.gameObject;
							}
						}
					}

					else if (other.GetComponent<RoomSpawner>().doorNeeded == DoorOrientation.TOP)
					{
						//Debug.Log("Spawn TR");
						// Need to spawn a room with a RIGHT TOP door
						GameObject newRoom = Instantiate(templates.TR, new Vector3(transform.position.x + templates.roomOffset, transform.position.y, transform.position.z), templates.TR.transform.rotation, templates.roomsParent);
						nextRoom = newRoom;
						other.GetComponent<RoomSpawner>().nextRoom = newRoom;

						GameObject roomSpawnpoints = null;
						for (int i = 0; i < newRoom.transform.childCount; i++)
						{
							if (newRoom.transform.GetChild(i).name == "Spawn Points")
								roomSpawnpoints = newRoom.transform.GetChild(i).gameObject;
						}

						for (int i = 0; i < roomSpawnpoints?.transform.childCount; i++)
						{
							if (roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().doorNeeded == DoorOrientation.LEFT)
							{
								roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().hasRoomConnected = true;
								roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().nextRoom = gameObject.transform.parent.transform.parent.gameObject;
							}

							if (roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().doorNeeded == DoorOrientation.BOTTOM)
							{
								roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().hasRoomConnected = true;
								roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().nextRoom = other.GetComponent<RoomSpawner>().gameObject.transform.parent.transform.parent.gameObject;
							}
						}
					}

					else if (other.GetComponent<RoomSpawner>().doorNeeded == DoorOrientation.LEFT)
					{
						//Debug.Log("Spawn LR");
						// Need to spawn a room with a RIGHT LEFT door
						GameObject newRoom = Instantiate(templates.LR, new Vector3(transform.position.x + templates.roomOffset, transform.position.y, transform.position.z), templates.LR.transform.rotation, templates.roomsParent);
						nextRoom = newRoom;
						other.GetComponent<RoomSpawner>().nextRoom = newRoom;

						GameObject roomSpawnpoints = null;
						for (int i = 0; i < newRoom.transform.childCount; i++)
						{
							if (newRoom.transform.GetChild(i).name == "Spawn Points")
								roomSpawnpoints = newRoom.transform.GetChild(i).gameObject;
						}

						for (int i = 0; i < roomSpawnpoints?.transform.childCount; i++)
						{
							if (roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().doorNeeded == DoorOrientation.LEFT)
                            {
								roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().hasRoomConnected = true;
								roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().nextRoom = gameObject.transform.parent.transform.parent.gameObject;
							}
								

							if (roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().doorNeeded == DoorOrientation.RIGHT)
							{
								roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().hasRoomConnected = true;
								roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().nextRoom = other.GetComponent<RoomSpawner>().gameObject.transform.parent.transform.parent.gameObject;
							}
						}
					}
				}
			}

			other.GetComponent<RoomSpawner>().hasRoomConnected = true;
			hasRoomConnected = true;
		}
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
        DoorOrientation oppositeDoorOrientation = GetRoomOppositeOrientation(newRoomOrientation);

        SetConnectionBetweenRooms(newRoom, oppositeDoorOrientation);
    }

	DoorOrientation GetRoomOppositeOrientation(DoorOrientation orientation)
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

    void SetConnectionBetweenRooms(GameObject nextRoom, DoorOrientation connectionToSet)
	{
        GameObject roomSpawnpoints = null;
        for (int i = 0; i < nextRoom.transform.childCount; i++)
        {
            if (nextRoom.transform.GetChild(i).name == "Spawn Points")
			{
                roomSpawnpoints = nextRoom.transform.GetChild(i).gameObject;
            }
        }

        for (int i = 0; i < roomSpawnpoints?.transform.childCount; i++)
        {
            if (roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().doorNeeded == connectionToSet)
            {
                roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().hasRoomConnected = true;
                roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().nextRoom = currentRoom;
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