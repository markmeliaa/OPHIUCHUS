using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonMapManager : MonoBehaviour
{
    [SerializeField] private Transform initialMinimapRoomPosition;

    [Header("ROOM RELATED FIELDS")]
    [Space(5)]
    public Transform minimapRoomsParent;
    public Transform gameRoomsParent;
    [Space(5)]
    public GameObject currentMinimapRoom;
    public GameObject currentGameRoom;

    [HideInInspector] public List<GameObject> spawnedMinimapRooms;
    [HideInInspector] public List<GameObject> spawnedGameRooms;

    [Space(10)]

    [Range(5, 10)]
    [SerializeField] private int minRooms = 15;
    [Range(20, 25)]
    [SerializeField] private int maxRooms = 25;

    private bool spawnedBoss;
    private float availableTimeToGenerateTheMap = 5.0f;

	[SerializeField] private bool isGame = true;

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

				GameObject bossRoom = RoomsHolderSingleton.Instance.bossRoom;
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

				if (isGame)
				{
					LevelSetUpManager runManager = GetComponent<LevelSetUpManager>();
					runManager.SetUpInitialAttributes();
				}
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

		GameObject initialRoom = RoomsHolderSingleton.Instance.allDirectionsMinimapRoom;

		if (isGame)
		{
            currentGameRoom = Instantiate(initialRoom, initialMinimapRoomPosition.position,
										  initialRoom.transform.rotation, minimapRoomsParent);
        }
    }
}