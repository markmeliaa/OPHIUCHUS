using System.Collections.Generic;
using UnityEngine;

public enum SpecialRooms
{
    NONE,
    BOSS,
    SHOP,
    HEAL
}

public class DungeonMapManager : MonoBehaviour
{
    [SerializeField] private bool isGame = true;
    [SerializeField] private Transform initialMinimapRoomPosition;

    [Header("ROOM RELATED FIELDS")]
    [Space(5)]
    public Transform minimapRoomsParent;
    public Transform gameRoomsParent;
    [Space(5)]
    public GameObject currentMinimapRoom;
    [Tooltip("If 'isGame' above is marked as false, this field can be null")]
    public GameObject currentGameRoom;

    [HideInInspector] public GameObject playerMinimapPosition;

    [HideInInspector] public List<GameObject> spawnedMinimapRooms;
    [HideInInspector] public List<GameObject> spawnedGameRooms;

    [Space(10)]

    [Header("MAP RELATED FIELDS")]
    [Space(5)]
    [Range(5, 15)]
    [SerializeField] private int minRooms = 15;
    [Range(20, 30)]
    [SerializeField] private int maxRooms = 25;

    private bool levelSetUp;
    private float timeToGenerateMap = 1.0f;

    [Range(0.0f, 100.0f)]
    [SerializeField] private float shopRoomChance;
    [Range(0.0f, 100.0f)]
    [SerializeField] private float healRoomChance;

    [Tooltip("If 'isGame' above is marked as false, this field can be null")]
    public Animator changeRoomAnim;

    private void Start()
    {
        // This triggers the domino effect that creates the map
        spawnedMinimapRooms.Add(currentMinimapRoom);

        spawnedGameRooms.Add(currentGameRoom);
    }

    private void Update()
    {
        if (levelSetUp)
        {
            return;
        }

        if (timeToGenerateMap <= 0.0f)
        {
            bool roomCountNotWithinTheConstraints = spawnedMinimapRooms.Count <= minRooms || spawnedMinimapRooms.Count > maxRooms;

            GameObject lastSpawnedRoom = spawnedMinimapRooms[spawnedMinimapRooms.Count - 1];
            bool bossCanNotBeSpawned = !CanBeBossRoom(lastSpawnedRoom);

            if (roomCountNotWithinTheConstraints || bossCanNotBeSpawned)
            {
                RegenerateDungeonMap();
            }

            else
            {
                SpawnSpecialRoom(SpecialRooms.BOSS);

                if (Random.Range(0.0f, 100.0f) <= shopRoomChance)
                {
                    SpawnSpecialRoom(SpecialRooms.SHOP);
                }

                if (Random.Range(0.0f, 100.0f) <= healRoomChance)
                {
                    SpawnSpecialRoom(SpecialRooms.HEAL);
                }

                if (isGame)
                {
                    HideUndiscoveredRooms();

                    LevelSetUpManager runManager = GetComponent<LevelSetUpManager>();
                    runManager.SetUpInitialAttributes();
                }

                levelSetUp = true;
            }
        }
        else
        {
            timeToGenerateMap -= Time.deltaTime;
        }
    }

	public void RegenerateDungeonMap()
    {
		foreach (GameObject room in spawnedMinimapRooms)
		{
            Destroy(room);
        }

        spawnedMinimapRooms = new List<GameObject>();

		levelSetUp = false;
		timeToGenerateMap = 1.0f;

		GameObject initialRoom = RoomsHolderSingleton.Instance.allDirectionsMinimapRoom;
        currentMinimapRoom = Instantiate(initialRoom, initialMinimapRoomPosition.position,
                                         initialRoom.transform.rotation, minimapRoomsParent);
        spawnedMinimapRooms.Add(currentMinimapRoom);
        playerMinimapPosition = currentMinimapRoom.transform.GetChild(currentMinimapRoom.transform.childCount - 1).gameObject;
    }

    private bool CanBeBossRoom(GameObject selectedRoom)
    {
        // Only limit rooms can be boss rooms (for now)

        return selectedRoom.gameObject.name == "B(Clone)" ||
               selectedRoom.gameObject.name == "T(Clone)" ||
               selectedRoom.gameObject.name == "L(Clone)" ||
               selectedRoom.gameObject.name == "R(Clone)";
    }

    private void SpawnSpecialRoom(SpecialRooms roomType)
    {
        GameObject specialRoom;
        Transform specialRoomTransform;
        string tagForTheRoom;

        switch(roomType)
        {
            case SpecialRooms.BOSS:
                specialRoom = RoomsHolderSingleton.Instance.bossRoom;
                specialRoomTransform = spawnedMinimapRooms[spawnedMinimapRooms.Count - 1].transform;
                tagForTheRoom = "BossRoom";
                break;

            case SpecialRooms.SHOP:
                specialRoom = RoomsHolderSingleton.Instance.shopRoom;
                int shopRoomPlace = Random.Range(1, spawnedMinimapRooms.Count - 1);
                specialRoomTransform = spawnedMinimapRooms[shopRoomPlace].transform;
                tagForTheRoom = "ShopRoom";
                break;

            case SpecialRooms.HEAL:
                specialRoom = RoomsHolderSingleton.Instance.healRoom;
                int healRoomPlace = Random.Range(1, spawnedMinimapRooms.Count - 1);
                specialRoomTransform = spawnedMinimapRooms[healRoomPlace].transform;
                tagForTheRoom = "HealRoom";
                break;

            default:
                return;
        }

        Instantiate(specialRoom, specialRoomTransform.position, specialRoomTransform.rotation, specialRoomTransform);
        specialRoomTransform.gameObject.tag = tagForTheRoom;
    }

    private void HideUndiscoveredRooms()
    {
        for (int i = 1; i < spawnedMinimapRooms.Count; i++)
        {
            spawnedMinimapRooms[i].SetActive(false);
        }
    }
}