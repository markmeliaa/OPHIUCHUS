using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateDungeonMapManager : MonoBehaviour 
{
    [Header("MINIMAP ROOM TEMPLATES")]
    [Space(5)]
    public GameObject[] bottomMinimapRooms;
	public GameObject[] topMinimapRooms;
	public GameObject[] leftMinimapRooms;
	public GameObject[] rightMinimapRooms;
    [Space(5)]
    public GameObject topMinimapRoom;
	public GameObject bottomMinimapRoom;
	public GameObject leftMinimapRoom;
	public GameObject rightMinimapRoom;
    [Space(5)]
    public GameObject topBottomMinimapRoom;
	public GameObject topLeftMinimapRoom;
	public GameObject topRightMinimapRoom;
    [Space(5)]
    public GameObject leftBottomMinimapRoom;
	public GameObject rightBottomMinimapRoom;
    [Space(5)]
    public GameObject leftRightMinimapRoom;
    [Space(5)]
    public GameObject allDirectionsMinimapRoom;

    [Space(20)]

    [SerializeField] private GameObject shopRoom;
	[SerializeField] private GameObject healRoom;
    [SerializeField] private GameObject bossRoom;
    [Space(5)]

    private bool spawnedBoss;

    [Header("GAME ROOM TEMPLATES")]
    [Space(5)]
    public List<GameObject> realRooms;

    [Space(20)]

    public Transform minimapRoomsParent;
    public Transform gameRoomsParent;
    [Space(5)]
    [HideInInspector] public List<GameObject> spawnedMinimapRooms;
    [HideInInspector] public List<GameObject> spawnedGameRooms;
    [Space(5)]
    public GameObject currentMinimapRoom;
    public GameObject currentGameRoom;

    [Range(5, 10)]
    [SerializeField] private int minRooms = 15;
    [Range(20, 25)]
    [SerializeField] private int maxRooms = 25;

    [SerializeField] private Transform initialMinimapRoomPosition;

    // ------------------------------------------------

    private float availableTimeToGenerateTheMap = 5.0f;

    [SerializeField] private Animator circleAnimator;
	public Animator circleAnimator2;

	[SerializeField] private GameObject loadScreen;
	[SerializeField] private AudioSource gameMusic;

	[SerializeField] private GameObject mainCharacter;

	[SerializeField] private GameObject tpCharacter;

    private void Start()
    {
		spawnedGameRooms.Add(currentGameRoom);
    }

    // Spawn final zone boss
    private void Update()
    {
		if (availableTimeToGenerateTheMap <= 0 && !spawnedBoss)
		{
			if (spawnedMinimapRooms.Count <= minRooms || spawnedMinimapRooms.Count > maxRooms)
				RestartGame();

			else
            {
				// Spawn boss room but only in the R, B, T, L Rooms
				if (spawnedMinimapRooms[spawnedMinimapRooms.Count - 1].gameObject.name != "R(Clone)" && spawnedMinimapRooms[spawnedMinimapRooms.Count - 1].gameObject.name != "L(Clone)" && 
					spawnedMinimapRooms[spawnedMinimapRooms.Count - 1].gameObject.name != "B(Clone)" && spawnedMinimapRooms[spawnedMinimapRooms.Count - 1].gameObject.name != "T(Clone)")
                {
					RestartGame();
					return;
                }

				Instantiate(bossRoom, spawnedMinimapRooms[spawnedMinimapRooms.Count - 1].transform.position, Quaternion.identity, spawnedMinimapRooms[spawnedMinimapRooms.Count - 1].transform);
				spawnedMinimapRooms[spawnedMinimapRooms.Count - 1].tag = "BossRoom";
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

				for (int i = 1; i < spawnedMinimapRooms.Count; i++)
                {
					spawnedMinimapRooms[i].SetActive(false);
                }

				if (currentMinimapRoom != null)
				{
                    currentMinimapRoom = Instantiate(currentMinimapRoom, spawnedMinimapRooms[0].transform.position, Quaternion.identity, spawnedMinimapRooms[0].transform);
                }

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
			availableTimeToGenerateTheMap -= Time.deltaTime;
	}

	public void RestartGame()
    {
		foreach (GameObject room in spawnedMinimapRooms)
			Destroy(room);

		spawnedMinimapRooms = new List<GameObject>();

		spawnedBoss = false;
		availableTimeToGenerateTheMap = 1f;

        currentGameRoom = Instantiate(allDirectionsMinimapRoom, initialMinimapRoomPosition.position, allDirectionsMinimapRoom.transform.rotation, minimapRoomsParent);
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

		mainCharacter.GetComponent<SpriteRenderer>().enabled = true; 
		tpCharacter.SetActive(false);
	}
}
