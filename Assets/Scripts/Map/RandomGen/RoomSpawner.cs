using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSpawner : MonoBehaviour
{

	public int openingDirection;
	// 1 --> need bottom door
	// 2 --> need top door
	// 3 --> need left door
	// 4 --> need right door


	private RoomTemplates templates;
	private int rand;
	public bool spawned = false;

	private float waitTime = 4f;

	void Start()
	{
		Destroy(gameObject, waitTime);
		templates = GameObject.FindGameObjectWithTag("Rooms").GetComponent<RoomTemplates>();

		// Call a function after certain time
		Invoke("Spawn", 0.1f);
	}


	void Spawn()
	{
		if (spawned == false)
		{
			if (openingDirection == 1)
			{
				// Need to spawn a room with a BOTTOM door
				rand = Random.Range(0, templates.bottomRooms.Length);
				Instantiate(templates.bottomRooms[rand], transform.position, templates.bottomRooms[rand].transform.rotation, templates.roomPlaceholder);
				//Instantiate(templates.bottomRooms[rand], new Vector3(transform.position.x, transform.position.y - 1, transform.position.z), templates.bottomRooms[rand].transform.rotation, templates.roomPlaceholder);
			}
			else if (openingDirection == 2)
			{
				// Need to spawn a room with a TOP door
				rand = Random.Range(0, templates.topRooms.Length);
				Instantiate(templates.topRooms[rand], transform.position, templates.topRooms[rand].transform.rotation, templates.roomPlaceholder);
				//Instantiate(templates.topRooms[rand], new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), templates.topRooms[rand].transform.rotation, templates.roomPlaceholder);
			}
			else if (openingDirection == 3)
			{
				// Need to spawn a room with a LEFT door
				rand = Random.Range(0, templates.leftRooms.Length);
				Instantiate(templates.leftRooms[rand], transform.position, templates.leftRooms[rand].transform.rotation, templates.roomPlaceholder);
				//Instantiate(templates.leftRooms[rand], new Vector3(transform.position.x - 1, transform.position.y, transform.position.z), templates.leftRooms[rand].transform.rotation, templates.roomPlaceholder);
			}
			else if (openingDirection == 4)
			{
				// Need to spawn a room with a RIGHT door
				rand = Random.Range(0, templates.rightRooms.Length);
				Instantiate(templates.rightRooms[rand], transform.position, templates.rightRooms[rand].transform.rotation, templates.roomPlaceholder);
				//Instantiate(templates.rightRooms[rand], new Vector3(transform.position.x + 1, transform.position.y, transform.position.z), templates.rightRooms[rand].transform.rotation, templates.roomPlaceholder);
			}
			spawned = true;
		}
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("SpawnPoint"))
		{
			if (other.GetComponent<RoomSpawner>().spawned == false && spawned == false)
			{
				if (openingDirection == 1)
					Instantiate(templates.closedRoom, new Vector3(transform.position.x, transform.position.y - 1, transform.position.z), Quaternion.identity, templates.roomPlaceholder);
				else if (openingDirection == 2)
					Instantiate(templates.closedRoom, new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), Quaternion.identity, templates.roomPlaceholder);
				else if (openingDirection == 3)
					Instantiate(templates.closedRoom, new Vector3(transform.position.x - 1, transform.position.y, transform.position.z), Quaternion.identity, templates.roomPlaceholder);
				else if (openingDirection == 4)
					Instantiate(templates.closedRoom, new Vector3(transform.position.x + 1, transform.position.y, transform.position.z), Quaternion.identity, templates.roomPlaceholder);
				//Destroy(gameObject);
			}

			spawned = true;
		}
	}
}
