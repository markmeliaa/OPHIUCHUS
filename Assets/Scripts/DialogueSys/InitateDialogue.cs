using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitateDialogue : MonoBehaviour
{
    public GameObject eKey;
    public GameObject playBarrier;
    private ButtonManager buttonManager;
    public string zodiac;

    private bool ePressed = false;

    private void Awake()
    {
        buttonManager = GameObject.FindGameObjectWithTag("Buttons").GetComponent<ButtonManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            eKey.SetActive(true);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            eKey.SetActive(true);

            if (Input.GetKey(KeyCode.E) && !ePressed)
            {
                ePressed = true;
                //Debug.Log("Start Conversation");

                if (zodiac != "")
                    buttonManager.StartBossBattle(zodiac);

                // Leave the access to Andromeda open
                if (playBarrier != null)
                    playBarrier.SetActive(false);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            eKey.SetActive(false);
        }
    }
}
