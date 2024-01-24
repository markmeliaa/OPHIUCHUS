using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShowArrows : MonoBehaviour
{
    private GameObject arrow;
    private RoomTemplates templates;

    private GameObject player;
    private PlayerMovement playerMovement;
    private GameObject tpAnim;

    private bool interactKeyPressed;

    private ManageScenes sceneLoader;

    void Start()
    {
        // They need to be assigned this way because they belong to prefabs
        arrow = gameObject.transform.GetChild(0).gameObject;
        templates = GameObject.FindGameObjectWithTag("Rooms")?.GetComponent<RoomTemplates>();

        player = GameObject.FindGameObjectWithTag("Player");
        playerMovement = player.GetComponent<PlayerMovement>();
        tpAnim = player.transform.GetChild(0).gameObject;

        sceneLoader = GetComponent<ManageScenes>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            arrow.SetActive(true);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (sceneLoader != null && sceneLoader.GetCurrentSceneIndex() == 1)
            {
                arrow.SetActive(true);

                if (Input.GetKey(KeyCode.E) && !interactKeyPressed)
                {
                    interactKeyPressed = true;
                    ChangeToGameScene();
                }
            }
            else if (templates != null && !templates.changingRoom)
            {
                arrow.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            arrow.SetActive(false);
        }
    }

    void ChangeToGameScene()
    {
        player.GetComponent<SpriteRenderer>().enabled = false;
        player.GetComponent<AudioSource>().Stop();

        tpAnim.SetActive(true);
        playerMovement.StopPlayer();

        sceneLoader.LoadSelectedScene(2);
    }
}
