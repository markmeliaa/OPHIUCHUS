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

	private float waitTime = 6f;
	private bool spawnedBoss;
	public GameObject boss;

	public GameObject generateAgainButton;

	public int minRooms = 15;
	public int maxRooms = 25;

	public Transform roomSpawner;
	public Camera mainCamera;

	public float roomOffset;

	public GameObject currentRoom;
	public GameObject currentRealRoom;

	public List<GameObject> realRooms;
	public List<GameObject> realCreatedRooms;

	public Transform realRoomPlaceholder;

	public bool mapFormed = false;

	public Animator circleAnimator;
	public Animator circleAnimator2;

	public GameObject loadScreen;
	public AudioSource gameMusic;

	public GameObject actualRoom;

	[HideInInspector] public bool changingRoom = false;
	public Animator changeRoomAnim;

	public GameObject mainCharacter;

	public GameObject tpCharacter;

    private void Start()
    {
		generateAgainButton.SetActive(false);
		realCreatedRooms.Add(currentRealRoom);
    }

    // Spawn final zone boss
    private void Update()
    {
		if (waitTime <= 0 && !spawnedBoss)
		{
			if (rooms.Count <= minRooms || rooms.Count > maxRooms)
				RestartGame();

			else
            {
				// Spawn boss room but only in the R, B, T, L Rooms
				if (rooms[rooms.Count - 1].gameObject.name != "R(Clone)" && rooms[rooms.Count - 1].gameObject.name != "L(Clone)" && 
					rooms[rooms.Count - 1].gameObject.name != "B(Clone)" && rooms[rooms.Count - 1].gameObject.name != "T(Clone)")
                {
					RestartGame();
					return;
                }

				Instantiate(boss, rooms[rooms.Count - 1].transform.position, Quaternion.identity, rooms[rooms.Count - 1].transform);
				rooms[rooms.Count - 1].tag = "BossRoom";
				spawnedBoss = true;

				/*
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
				*/

				for (int i = 1; i < rooms.Count; i++)
                {
					rooms[i].SetActive(false);
                }

				actualRoom = Instantiate(actualRoom, rooms[0].transform.position, Quaternion.identity, rooms[0].transform);

				gameMusic.Play();
				loadScreen.SetActive(false);
				circleAnimator.SetBool("Show", true);
				circleAnimator2.SetBool("Show", true);
				AddItems();
				StartCoroutine("StartGame");

				//generateAgainButton.SetActive(true);
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
		mapFormed = false;

        currentRoom = Instantiate(initialRoom, roomSpawner.position, initialRoom.transform.rotation, roomPlaceholder);
    }

	public void AddItems()
	{
		GameMaster.inventory.Add(new ItemObject("Health Potion", objectTypes.health, 1));
		GameMaster.inventory.Add(new ItemObject("Speed Potion", objectTypes.speed, 2));
	}

	public void EnterAnimation()
    {
		mainCharacter.GetComponent<SpriteRenderer>().enabled = false;
		tpCharacter.SetActive(true);
    }

	IEnumerator StartGame()
    {
		yield return new WaitForSeconds(0.5f);
		EnterAnimation();
		yield return new WaitForSeconds(1.0f);

		mapFormed = true;
		mainCharacter.GetComponent<SpriteRenderer>().enabled = true; 
		tpCharacter.SetActive(false);
	}
}
