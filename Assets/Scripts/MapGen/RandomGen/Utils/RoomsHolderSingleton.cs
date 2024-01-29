using System.Collections.Generic;
using UnityEngine;

public class RoomsHolderSingleton : MonoBehaviour
{
    // EVERYTHING INSIDE HERE SHOULD BE PREFABS

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

    public GameObject shopRoom;
    public GameObject healRoom;
    public GameObject bossRoom;
    [Space(5)]

    [Header("GAME ROOM TEMPLATES")]
    [Space(5)]
    public List<GameObject> gameRooms;

    private static RoomsHolderSingleton instance;

    public static RoomsHolderSingleton Instance
    {
        get
        {
            // if instance is null
            if (instance == null)
            {
                // find the generic instance
                instance = FindObjectOfType<RoomsHolderSingleton>();

                // if it's null again create a new object
                // and attach the generic instance
                if (instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(RoomsHolderSingleton).Name;
                    instance = obj.AddComponent<RoomsHolderSingleton>();
                }
            }
            return instance;
        }
    }

    void Awake()
    {
        AvoidDeletingSingletonInstance();
    }

    void AvoidDeletingSingletonInstance()
    {
        if (instance == null)
        {
            //First run, set the instance
            instance = this;
            DontDestroyOnLoad(gameObject);

        }
        else if (instance != this)
        {
            //Instance is not the same as the one we have, destroy old one, and reset to newest one
            Destroy(instance.gameObject);
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}
