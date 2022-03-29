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
	public GameObject initialRoom;

	public Transform roomPlaceholder;

	public bool shopPlaced = false;

	public List<GameObject> rooms;

	private float waitTime = 2f;
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
			Instantiate(boss, rooms[rooms.Count - 1].transform.position, Quaternion.identity, roomPlaceholder);
			spawnedBoss = true;
			generateAgainButton.SetActive(true);
		}

		else
			waitTime -= Time.deltaTime;
    }

	public void RestartGame()
    {
		foreach (GameObject room in rooms)
			Destroy(room);

		Destroy(GameObject.FindGameObjectWithTag("BossRoom").gameObject);
		rooms = new List<GameObject>();

		spawnedBoss = false;
		waitTime = 2f;
		generateAgainButton.SetActive(false);

		Instantiate(initialRoom, new Vector3(0, 0, 0), initialRoom.transform.rotation, roomPlaceholder);
    }
}
