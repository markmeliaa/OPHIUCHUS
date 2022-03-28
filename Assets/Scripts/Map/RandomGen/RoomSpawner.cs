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

	private float waitTime = 3f;

	void Start()
	{
		//Destroy(gameObject, waitTime);
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
				//Instantiate(templates.bottomRooms[rand], transform.position, templates.bottomRooms[rand].transform.rotation, templates.roomPlaceholder);
				Instantiate(templates.bottomRooms[rand], new Vector3(transform.position.x, transform.position.y - 1, transform.position.z), templates.bottomRooms[rand].transform.rotation, templates.roomPlaceholder);
			}
			else if (openingDirection == 2)
			{
				// Need to spawn a room with a TOP door
				rand = Random.Range(0, templates.topRooms.Length);
				//Instantiate(templates.topRooms[rand], transform.position, templates.topRooms[rand].transform.rotation, templates.roomPlaceholder);
				Instantiate(templates.topRooms[rand], new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), templates.topRooms[rand].transform.rotation, templates.roomPlaceholder);
			}
			else if (openingDirection == 3)
			{
				// Need to spawn a room with a LEFT door
				rand = Random.Range(0, templates.leftRooms.Length);
				//Instantiate(templates.leftRooms[rand], transform.position, templates.leftRooms[rand].transform.rotation, templates.roomPlaceholder);
				Instantiate(templates.leftRooms[rand], new Vector3(transform.position.x - 1, transform.position.y, transform.position.z), templates.leftRooms[rand].transform.rotation, templates.roomPlaceholder);
			}
			else if (openingDirection == 4)
			{
				// Need to spawn a room with a RIGHT door
				rand = Random.Range(0, templates.rightRooms.Length);
				//Instantiate(templates.rightRooms[rand], transform.position, templates.rightRooms[rand].transform.rotation, templates.roomPlaceholder);
				Instantiate(templates.rightRooms[rand], new Vector3(transform.position.x + 1, transform.position.y, transform.position.z), templates.rightRooms[rand].transform.rotation, templates.roomPlaceholder);
			}
		}

		spawned = true;
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		templates = GameObject.FindGameObjectWithTag("Rooms").GetComponent<RoomTemplates>();
		if (other.CompareTag("SpawnPoint"))
		{
			if (other.GetComponent<RoomSpawner>().spawned == false && spawned == false && transform.position.x != 0 && transform.position.y != 0)
			{
				if (openingDirection == 1)
					Instantiate(templates.closedRoom, new Vector3(transform.position.x, transform.position.y - 1, transform.position.z), Quaternion.identity, templates.roomPlaceholder);
				else if (openingDirection == 2)
					Instantiate(templates.closedRoom, new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), Quaternion.identity, templates.roomPlaceholder);
				else if (openingDirection == 3)
					Instantiate(templates.closedRoom, new Vector3(transform.position.x - 1, transform.position.y, transform.position.z), Quaternion.identity, templates.roomPlaceholder);
				else if (openingDirection == 4)
					Instantiate(templates.closedRoom, new Vector3(transform.position.x + 1, transform.position.y, transform.position.z), Quaternion.identity, templates.roomPlaceholder);
				Destroy(gameObject);

				/*
				if (other.GetComponent<RoomSpawner>().openingDirection == 1)
                {
					if (openingDirection == 2)
						Instantiate(templates.TB, new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), templates.TB.transform.rotation, templates.roomPlaceholder);
					else if (openingDirection == 3)
						Instantiate(templates.LB, new Vector3(transform.position.x - 1, transform.position.y, transform.position.z), templates.LB.transform.rotation, templates.roomPlaceholder);
					else if (openingDirection == 4)
						Instantiate(templates.RB, new Vector3(transform.position.x + 1, transform.position.y, transform.position.z), templates.RB.transform.rotation, templates.roomPlaceholder);
				}

				else if (other.GetComponent<RoomSpawner>().openingDirection == 2)
				{
					if (openingDirection == 1)
						Instantiate(templates.TB, new Vector3(transform.position.x, transform.position.y - 1, transform.position.z), templates.TB.transform.rotation, templates.roomPlaceholder);
					else if (openingDirection == 3)
						Instantiate(templates.TL, new Vector3(transform.position.x - 1, transform.position.y, transform.position.z), templates.TL.transform.rotation, templates.roomPlaceholder);
					else if (openingDirection == 4)
						Instantiate(templates.TR, new Vector3(transform.position.x + 1, transform.position.y, transform.position.z), templates.TR.transform.rotation, templates.roomPlaceholder);
				}

				else if (other.GetComponent<RoomSpawner>().openingDirection == 3)
				{
					if (openingDirection == 1)
						Instantiate(templates.LB, new Vector3(transform.position.x, transform.position.y - 1, transform.position.z), templates.LB.transform.rotation, templates.roomPlaceholder);
					else if (openingDirection == 2)
						Instantiate(templates.TL, new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), templates.TL.transform.rotation, templates.roomPlaceholder);
					else if (openingDirection == 4)
						Instantiate(templates.LR, new Vector3(transform.position.x + 1, transform.position.y, transform.position.z), templates.LR.transform.rotation, templates.roomPlaceholder);
				}

				else if (other.GetComponent<RoomSpawner>().openingDirection == 4)
				{
					if (openingDirection == 1)
						Instantiate(templates.RB, new Vector3(transform.position.x, transform.position.y - 1, transform.position.z), templates.RB.transform.rotation, templates.roomPlaceholder);
					else if (openingDirection == 2)
						Instantiate(templates.TR, new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), templates.TR.transform.rotation, templates.roomPlaceholder);
					else if (openingDirection == 3)
						Instantiate(templates.LR, new Vector3(transform.position.x - 1, transform.position.y, transform.position.z), templates.LR.transform.rotation, templates.roomPlaceholder);
				}
				*/
			}

			other.GetComponent<RoomSpawner>().spawned = true;
		}
	}
}
