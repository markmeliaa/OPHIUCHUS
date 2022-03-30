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

	public GameObject TB;
	public GameObject TL;
	public GameObject TR;

	public GameObject LB;
	public GameObject RB;

	public GameObject LR;

	public GameObject shopRoom;
	public GameObject healRoom;
	public GameObject initialRoom;
	public GameObject normalRoom;

	public Transform roomPlaceholder;

	public List<GameObject> rooms;

	private float waitTime = 0.5f;
	private bool spawnedBoss;
	public GameObject boss;

	public GameObject generateAgainButton;

    private void Start()
    {
		generateAgainButton.SetActive(false);
    }

    // Spawn final zone boss
    private void Update()
    {
		if (waitTime <= 0 && !spawnedBoss)
		{
			if (rooms.Count < 15 || rooms.Count > 25)
				RestartGame();

			else
            {
				// Spawn boss room
				Instantiate(boss, rooms[rooms.Count - 1].transform.position, Quaternion.identity, rooms[rooms.Count - 1].transform);
				rooms[rooms.Count - 1].tag = "BossRoom";
				spawnedBoss = true;

				// Spawn heal and money room
				int rand = Random.Range(1, rooms.Count - 1);
				int rand2 = Random.Range(1, rooms.Count - 1);

				while (rand == rand2)
					rand2 = Random.Range(1, rooms.Count - 1);

				//Debug.Log(rooms.Count / 10);

				for (int i = 1; i <= rooms.Count - 2; i++)
				{
					if (i == rand && Random.Range(0, 10) < rooms.Count / 10 * 2)
					{
						Instantiate(shopRoom, rooms[i].transform.position, Quaternion.identity, rooms[i].transform);
						rooms[i].tag = "ShopRoom";
					}

					else if (i == rand2 && Random.Range(0, 10) < rooms.Count / 10)
					{
						Instantiate(healRoom, rooms[i].transform.position, Quaternion.identity, rooms[i].transform);
						rooms[i].tag = "HealRoom";
					}
				}

				generateAgainButton.SetActive(true);
			}
		}

		else
			waitTime -= Time.deltaTime;
	}

	public void RestartGame()
    {
		foreach (GameObject room in rooms)
			Destroy(room);

		rooms = new List<GameObject>();

		spawnedBoss = false;
		waitTime = 1f;
		generateAgainButton.SetActive(false);

        Instantiate(initialRoom, new Vector3(0, 0, 0), initialRoom.transform.rotation, roomPlaceholder);
    }
}
