using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DisplayChangeRoomArea : MonoBehaviour
{
    private GameObject arrow;

    private GameObject player;
    private PlayerMovement playerMovement;
    private GameObject tpAnim;

    private bool interactKeyPressed;

    private ScenesManager sceneLoader;

    void Start()
    {
        // They need to be assigned this way because they belong to prefabs
        arrow = gameObject.transform.GetChild(0).gameObject;

        player = GameObject.FindGameObjectWithTag("Player");
        playerMovement = player.GetComponent<PlayerMovement>();
        tpAnim = player.transform.GetChild(0).gameObject;

        sceneLoader = GetComponent<ScenesManager>();
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
