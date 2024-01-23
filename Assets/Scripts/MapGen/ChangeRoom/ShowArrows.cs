using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShowArrows : MonoBehaviour
{
    private GameObject arrow;
    private RoomTemplates templates;

    public GameObject mainChar;
    public GameObject upAnim;
    public PlayerMovement playerMovement;

    public Animator hideGame;

    private bool ePressed;

    // Start is called before the first frame update
    void Start()
    {
        arrow = gameObject.transform.GetChild(0).gameObject;
        templates = GameObject.FindGameObjectWithTag("Rooms")?.GetComponent<RoomTemplates>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (templates != null)
            {
                if (!templates.changingRoom)
                    arrow.SetActive(true);
            }

            else
            {
                arrow.SetActive(true);
                if (Input.GetKey(KeyCode.E) && !ePressed)
                {
                    ePressed = true;
                    mainChar.GetComponent<SpriteRenderer>().enabled = false;
                    mainChar.GetComponent<AudioSource>().Stop();

                    upAnim.SetActive(true);
                    playerMovement.StopPlayer();

                    hideGame.SetBool("Show", false);
                    StartCoroutine("ChangeScene");
                }
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (templates != null)
            {
                if (!templates.changingRoom)
                    arrow.SetActive(true);
            }

            else
            {
                arrow.SetActive(true);
                if (Input.GetKey(KeyCode.E) && !ePressed)
                {
                    ePressed = true;
                    mainChar.GetComponent<SpriteRenderer>().enabled = false;
                    mainChar.GetComponent<AudioSource>().Stop();

                    upAnim.SetActive(true);
                    playerMovement.StopPlayer();

                    hideGame.SetBool("Show", false);
                    StartCoroutine("ChangeScene");
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            arrow.SetActive(false);
    }

    IEnumerator ChangeScene()
    {
        yield return new WaitForSeconds(1.75f);
        SceneManager.LoadScene(2);
    }
}
