using UnityEngine;

public enum DoorOrientation
{
	BOTTOM,
	TOP,
	LEFT,
	RIGHT
};

public class RoomSpawner : MonoBehaviour
{
	public DoorOrientation doorNeeded;

	private RoomTemplates templates;
	private int rand;
	public bool spawned = false;
	public GameObject actualRoom;
	public GameObject nextRoom;

	void Start()
	{
		templates = GameObject.FindGameObjectWithTag("Rooms").GetComponent<RoomTemplates>();
		actualRoom = transform.parent.transform.parent.gameObject;

		// Call a function after certain time
		Invoke("Spawn", Time.fixedDeltaTime);
	}


	void Spawn()
	{
		if (spawned == false && templates.mainCamera.WorldToViewportPoint(transform.position).x < 1 - 0.03
			&& templates.mainCamera.WorldToViewportPoint(transform.position).x > 0 + 0.03
			&& templates.mainCamera.WorldToViewportPoint(transform.position).y < 1 - 0.038
			&& templates.mainCamera.WorldToViewportPoint(transform.position).y > 0 + 0.038)
		{
			if (doorNeeded == DoorOrientation.BOTTOM)
			{
				// Need to spawn a room with a BOTTOM door
				rand = Random.Range(0, templates.bottomRooms.Length);
				//Instantiate(templates.bottomRooms[rand], transform.position, templates.bottomRooms[rand].transform.rotation, templates.roomPlaceholder);
				GameObject newRoom = Instantiate(templates.bottomRooms[rand], new Vector3(transform.position.x, transform.position.y - templates.roomOffset, transform.position.z), templates.bottomRooms[rand].transform.rotation, templates.roomPlaceholder);
				nextRoom = newRoom;

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
						roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().spawned = true;
						roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().nextRoom = actualRoom;
					}
						
				}
			}
			else if (doorNeeded == DoorOrientation.TOP)
			{
				// Need to spawn a room with a TOP door
				rand = Random.Range(0, templates.topRooms.Length);
				//Instantiate(templates.topRooms[rand], transform.position, templates.topRooms[rand].transform.rotation, templates.roomPlaceholder);
				GameObject newRoom = Instantiate(templates.topRooms[rand], new Vector3(transform.position.x, transform.position.y + templates.roomOffset, transform.position.z), templates.topRooms[rand].transform.rotation, templates.roomPlaceholder);
				nextRoom = newRoom;

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
						roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().spawned = true;
						roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().nextRoom = actualRoom;
					}
				}
			}
			else if (doorNeeded == DoorOrientation.LEFT)
			{
				// Need to spawn a room with a LEFT door
				rand = Random.Range(0, templates.leftRooms.Length);
				//Instantiate(templates.leftRooms[rand], transform.position, templates.leftRooms[rand].transform.rotation, templates.roomPlaceholder);
				GameObject newRoom = Instantiate(templates.leftRooms[rand], new Vector3(transform.position.x - templates.roomOffset, transform.position.y, transform.position.z), templates.leftRooms[rand].transform.rotation, templates.roomPlaceholder);
				nextRoom = newRoom;

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
						roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().spawned = true;
						roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().nextRoom = actualRoom;
					}
				}
			}
			else if (doorNeeded == DoorOrientation.RIGHT)
			{
				// Need to spawn a room with a RIGHT door
				rand = Random.Range(0, templates.rightRooms.Length);
				//Instantiate(templates.rightRooms[rand], transform.position, templates.rightRooms[rand].transform.rotation, templates.roomPlaceholder);
				GameObject newRoom = Instantiate(templates.rightRooms[rand], new Vector3(transform.position.x + templates.roomOffset, transform.position.y, transform.position.z), templates.rightRooms[rand].transform.rotation, templates.roomPlaceholder);
				nextRoom = newRoom;

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
						roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().spawned = true;
						roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().nextRoom = actualRoom;
					}
				}
			}
		}

		// Check that rooms do not leave the camera area
		else if (!spawned && templates.mainCamera.WorldToViewportPoint(transform.position).y >= 1 - 0.038) // 63
		{
			GameObject newRoom = Instantiate(templates.B, new Vector3(transform.position.x, transform.position.y - 0.025f, transform.position.z), templates.B.transform.rotation, templates.roomPlaceholder);
			nextRoom = newRoom;

			GameObject roomSpawnpoints = null;
			for (int i = 0; i < newRoom.transform.childCount; i++)
			{
				if (newRoom.transform.GetChild(i).name == "Spawn Points")
					roomSpawnpoints = newRoom.transform.GetChild(i).gameObject;
			}

			for (int i = 0; i < roomSpawnpoints?.transform.childCount; i++)
			{
				roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().spawned = true;
				roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().nextRoom = actualRoom;
			}
		}

		else if (!spawned && templates.mainCamera.WorldToViewportPoint(transform.position).y <= 0 + 0.038)
		{
			GameObject newRoom = Instantiate(templates.T, new Vector3(transform.position.x, transform.position.y + 0.01f, transform.position.z), templates.T.transform.rotation, templates.roomPlaceholder);
			nextRoom = newRoom;

			GameObject roomSpawnpoints = null;
			for (int i = 0; i < newRoom.transform.childCount; i++)
			{
				if (newRoom.transform.GetChild(i).name == "Spawn Points")
					roomSpawnpoints = newRoom.transform.GetChild(i).gameObject;
			}

			for (int i = 0; i < roomSpawnpoints?.transform.childCount; i++)
			{
				roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().spawned = true;
				roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().nextRoom = actualRoom;
			}
		}


		else if (!spawned && templates.mainCamera.WorldToViewportPoint(transform.position).x >= 1 - 0.03) // 117
		{
			GameObject newRoom = Instantiate(templates.L, new Vector3(transform.position.x - 0.025f, transform.position.y, transform.position.z), templates.L.transform.rotation, templates.roomPlaceholder);
			nextRoom = newRoom;

			GameObject roomSpawnpoints = null;
			for (int i = 0; i < newRoom.transform.childCount; i++)
			{
				roomSpawnpoints = newRoom.transform.GetChild(i).gameObject;
			}

			for (int i = 0; i < roomSpawnpoints?.transform.childCount; i++)
			{
				roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().spawned = true;
				roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().nextRoom = actualRoom;
			}
		}

		else if (!spawned && templates.mainCamera.WorldToViewportPoint(transform.position).x <= 0 + 0.03)
        {
			GameObject newRoom = Instantiate(templates.R, new Vector3(transform.position.x + 0.01f, transform.position.y, transform.position.z), templates.R.transform.rotation, templates.roomPlaceholder);
			nextRoom = newRoom;

			GameObject roomSpawnpoints = null;
			for (int i = 0; i < newRoom.transform.childCount; i++)
			{
				roomSpawnpoints = newRoom.transform.GetChild(i).gameObject;
			}

			for (int i = 0; i < roomSpawnpoints?.transform.childCount; i++)
			{
				roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().spawned = true;
				roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().nextRoom = actualRoom;
			}
		}

		spawned = true;
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		templates = GameObject.FindGameObjectWithTag("Rooms").GetComponent<RoomTemplates>();
		if (other.CompareTag("SpawnPoint"))
		{
			if (other.GetComponent<RoomSpawner>().spawned == false && !spawned)
			{
				//Debug.Log("Collision at " + this.gameObject.transform.parent.transform.parent.transform.position + " with " + other.GetComponent<RoomSpawner>().gameObject.transform.parent.transform.parent.transform.position);
				//Debug.Log(openingDirection + ", " + other.GetComponent<RoomSpawner>().openingDirection);
				
				if (doorNeeded == DoorOrientation.BOTTOM)
                {
					if (other.GetComponent<RoomSpawner>().doorNeeded == DoorOrientation.TOP)
                    {
						//Debug.Log("Spawn TB");
						// Need to spawn a room with a BOTTOM TOP door
						GameObject newRoom = Instantiate(templates.TB, new Vector3(transform.position.x, transform.position.y - templates.roomOffset, transform.position.z), templates.TB.transform.rotation, templates.roomPlaceholder);
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
								roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().spawned = true;
								roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().nextRoom = gameObject.transform.parent.transform.parent.gameObject;
							}

							if (roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().doorNeeded == DoorOrientation.BOTTOM)
							{
								roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().spawned = true;
								roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().nextRoom = other.GetComponent<RoomSpawner>().gameObject.transform.parent.transform.parent.gameObject;
							}
						}
					}

					else if (other.GetComponent<RoomSpawner>().doorNeeded == DoorOrientation.LEFT)
					{
						//Debug.Log("Spawn LB");
						// Need to spawn a room with a BOTTOM LEFT door
						GameObject newRoom = Instantiate(templates.LB, new Vector3(transform.position.x, transform.position.y - templates.roomOffset, transform.position.z), templates.LB.transform.rotation, templates.roomPlaceholder);
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
								roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().spawned = true;
								roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().nextRoom = gameObject.transform.parent.transform.parent.gameObject;
							}

							if (roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().doorNeeded == DoorOrientation.RIGHT)
							{
								roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().spawned = true;
								roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().nextRoom = other.GetComponent<RoomSpawner>().gameObject.transform.parent.transform.parent.gameObject;
							}
						}
					}

					else if (other.GetComponent<RoomSpawner>().doorNeeded == DoorOrientation.RIGHT)
					{
						//Debug.Log("Spawn RB");
						// Need to spawn a room with a BOTTOM RIGHT door
						GameObject newRoom = Instantiate(templates.RB, new Vector3(transform.position.x, transform.position.y - templates.roomOffset, transform.position.z), templates.RB.transform.rotation, templates.roomPlaceholder);
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
								roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().spawned = true;
								roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().nextRoom = gameObject.transform.parent.transform.parent.gameObject;
							}

							if (roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().doorNeeded == DoorOrientation.LEFT)
							{
								roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().spawned = true;
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
						GameObject newRoom = Instantiate(templates.TB, new Vector3(transform.position.x, transform.position.y + templates.roomOffset, transform.position.z), templates.TB.transform.rotation, templates.roomPlaceholder);
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
								roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().spawned = true;
								roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().nextRoom = gameObject.transform.parent.transform.parent.gameObject;
							}

							if (roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().doorNeeded == DoorOrientation.TOP)
							{
								roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().spawned = true;
								roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().nextRoom = other.GetComponent<RoomSpawner>().gameObject.transform.parent.transform.parent.gameObject;
							}
						}
					}

					else if (other.GetComponent<RoomSpawner>().doorNeeded == DoorOrientation.LEFT)
					{
						//Debug.Log("Spawn TL");
						// Need to spawn a room with a TOP LEFT door
						GameObject newRoom = Instantiate(templates.TL, new Vector3(transform.position.x, transform.position.y + templates.roomOffset, transform.position.z), templates.TL.transform.rotation, templates.roomPlaceholder);
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
								roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().spawned = true;
								roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().nextRoom = gameObject.transform.parent.transform.parent.gameObject;
							}

							if (roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().doorNeeded == DoorOrientation.RIGHT)
							{
								roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().spawned = true;
								roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().nextRoom = other.GetComponent<RoomSpawner>().gameObject.transform.parent.transform.parent.gameObject;
							}
						}
					}

					else if (other.GetComponent<RoomSpawner>().doorNeeded == DoorOrientation.RIGHT)
					{
						//Debug.Log("Spawn TR");
						// Need to spawn a room with a TOP RIGHT door
						GameObject newRoom = Instantiate(templates.TR, new Vector3(transform.position.x, transform.position.y + templates.roomOffset, transform.position.z), templates.TR.transform.rotation, templates.roomPlaceholder);
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
								roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().spawned = true;
								roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().nextRoom = gameObject.transform.parent.transform.parent.gameObject;
							}

							if (roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().doorNeeded == DoorOrientation.LEFT)
							{
								roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().spawned = true;
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
						GameObject newRoom = Instantiate(templates.LB, new Vector3(transform.position.x - templates.roomOffset, transform.position.y, transform.position.z), templates.LB.transform.rotation, templates.roomPlaceholder);
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
								roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().spawned = true;
								roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().nextRoom = gameObject.transform.parent.transform.parent.gameObject;
							}

							if (roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().doorNeeded == DoorOrientation.TOP)
							{
								roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().spawned = true;
								roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().nextRoom = other.GetComponent<RoomSpawner>().gameObject.transform.parent.transform.parent.gameObject;
							}
						}
					}

					else if (other.GetComponent<RoomSpawner>().doorNeeded == DoorOrientation.TOP)
					{
						//Debug.Log("Spawn TL");
						// Need to spawn a room with a LEFT TOP door
						GameObject newRoom = Instantiate(templates.TL, new Vector3(transform.position.x - templates.roomOffset, transform.position.y, transform.position.z), templates.TL.transform.rotation, templates.roomPlaceholder);
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
								roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().spawned = true;
								roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().nextRoom = gameObject.transform.parent.transform.parent.gameObject;
							}

							if (roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().doorNeeded == DoorOrientation.BOTTOM)
							{
								roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().spawned = true;
								roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().nextRoom = other.GetComponent<RoomSpawner>().gameObject.transform.parent.transform.parent.gameObject;
							}
						}
					}

					else if (other.GetComponent<RoomSpawner>().doorNeeded == DoorOrientation.RIGHT)
					{
						//Debug.Log("Spawn LR");
						// Need to spawn a room with a LEFT RIGHT door
						GameObject newRoom = Instantiate(templates.LR, new Vector3(transform.position.x - templates.roomOffset, transform.position.y, transform.position.z), templates.LR.transform.rotation, templates.roomPlaceholder);
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
								roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().spawned = true;
								roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().nextRoom = gameObject.transform.parent.transform.parent.gameObject;
							}

							if (roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().doorNeeded == DoorOrientation.LEFT)
							{
								roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().spawned = true;
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
						GameObject newRoom = Instantiate(templates.RB, new Vector3(transform.position.x + templates.roomOffset, transform.position.y, transform.position.z), templates.RB.transform.rotation, templates.roomPlaceholder);
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
								roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().spawned = true;
								roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().nextRoom = gameObject.transform.parent.transform.parent.gameObject;
							}

							if (roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().doorNeeded == DoorOrientation.TOP)
							{
								roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().spawned = true;
								roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().nextRoom = other.GetComponent<RoomSpawner>().gameObject.transform.parent.transform.parent.gameObject;
							}
						}
					}

					else if (other.GetComponent<RoomSpawner>().doorNeeded == DoorOrientation.TOP)
					{
						//Debug.Log("Spawn TR");
						// Need to spawn a room with a RIGHT TOP door
						GameObject newRoom = Instantiate(templates.TR, new Vector3(transform.position.x + templates.roomOffset, transform.position.y, transform.position.z), templates.TR.transform.rotation, templates.roomPlaceholder);
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
								roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().spawned = true;
								roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().nextRoom = gameObject.transform.parent.transform.parent.gameObject;
							}

							if (roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().doorNeeded == DoorOrientation.BOTTOM)
							{
								roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().spawned = true;
								roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().nextRoom = other.GetComponent<RoomSpawner>().gameObject.transform.parent.transform.parent.gameObject;
							}
						}
					}

					else if (other.GetComponent<RoomSpawner>().doorNeeded == DoorOrientation.LEFT)
					{
						//Debug.Log("Spawn LR");
						// Need to spawn a room with a RIGHT LEFT door
						GameObject newRoom = Instantiate(templates.LR, new Vector3(transform.position.x + templates.roomOffset, transform.position.y, transform.position.z), templates.LR.transform.rotation, templates.roomPlaceholder);
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
								roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().spawned = true;
								roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().nextRoom = gameObject.transform.parent.transform.parent.gameObject;
							}
								

							if (roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().doorNeeded == DoorOrientation.RIGHT)
							{
								roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().spawned = true;
								roomSpawnpoints.transform.GetChild(i).gameObject.GetComponent<RoomSpawner>().nextRoom = other.GetComponent<RoomSpawner>().gameObject.transform.parent.transform.parent.gameObject;
							}
						}
					}
				}
			}

			other.GetComponent<RoomSpawner>().spawned = true;
			spawned = true;
		}
	}
}
