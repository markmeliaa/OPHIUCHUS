using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayDialogueArea : MonoBehaviour
{
    [SerializeField] private GameObject keyToInteract;
    private bool interactKeyPressed = false;

    [SerializeField] private GameObject barrierToUnlock = null;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !interactKeyPressed)
        {
            keyToInteract.SetActive(true);
        }

        // TODO: Check differences when triggering dialogue in scene 2
        // buttonManager.PressButton() was used
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !interactKeyPressed)
        {
            keyToInteract.SetActive(true);

            if (Input.GetKey(KeyCode.E))
            {
                interactKeyPressed = true;
                keyToInteract.SetActive(false);

                GetComponent<ConversationManager>()?.TriggerConversation();

                if (barrierToUnlock != null)
                {
                    barrierToUnlock.SetActive(false);
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            keyToInteract.SetActive(false);
        }
    }
}