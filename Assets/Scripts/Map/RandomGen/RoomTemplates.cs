using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTemplates : MonoBehaviour 
{
	public GameObject[] bottomRooms;
	public GameObject[] topRooms;
	public GameObject[] leftRooms;
	public GameObject[] rightRooms;

	public GameObject T;
	public GameObject B;
	public GameObject L;
	public GameObject R;

	public GameObject shopRoom;
	public GameObject healRoom;

	public Transform roomPlaceholder;

	public bool shopPlaced = false;

	public List<GameObject> rooms;

	private float waitTime = 2f;
	private bool spawnedBoss;
	public GameObject boss;

	// Spawn final zone boss
    private void Update()
    {
		if (waitTime <= 0 && !spawnedBoss)
		{
			Instantiate(boss, rooms[rooms.Count - 1].transform.position, Quaternion.identity, roomPlaceholder);
			spawnedBoss = true;
		}

		else
			waitTime -= Time.deltaTime;
    }
}
