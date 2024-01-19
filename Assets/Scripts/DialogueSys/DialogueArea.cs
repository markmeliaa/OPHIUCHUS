using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DialogueArea : MonoBehaviour
{
    [SerializeField] private GameObject keyToInteract;
    [SerializeField] private GameObject barrierToUnlock = null;
    [SerializeField] private ButtonManager buttonManager = null;

    private string characterSpeaker;
    [SerializeField] private DialogueBox characterProfile;

    // Things to show
    [SerializeField] private GameObject dialogueBoxObject;
    [SerializeField] private Image dialogueImage;
    [SerializeField] private Text dialogueNameText;
    [SerializeField] private Text dialogueText;
    [SerializeField] private GameObject nextButton;

    // Things to hide
    [SerializeField] private GameObject player;
    private PlayerMovement playerMovement;
    private PlayerAnimationDirection playerAnimationDirection;
    [SerializeField] private GameObject menuButton;

    private bool interactKeyPressed = false;
    private bool advanceTextKeyPressed = false;

    private void Start()
    {
        playerMovement = player.GetComponent<PlayerMovement>();
        playerAnimationDirection = player.GetComponent<PlayerAnimationDirection>();

        characterSpeaker = characterProfile.nameSpeaker;
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            if (nextButton.activeSelf && Input.GetKey(KeyCode.Z) && !advanceTextKeyPressed)
            {
                advanceTextKeyPressed = true;
                PressButton();
            }
        }
        else
        {
            if (buttonManager.nextButton.activeSelf && Input.GetKey(KeyCode.Z) && !advanceTextKeyPressed)
            {
                buttonManager.pressedZ = true;
                buttonManager.PressButton();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !interactKeyPressed)
        {
            keyToInteract.SetActive(true);
        }
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

                if (SceneManager.GetActiveScene().buildIndex != 1)
                {
                    buttonManager.StartDialogue(characterSpeaker, characterProfile);
                }
                else
                {
                    playerMovement.canMove = false;
                    playerAnimationDirection.SetDirection(new Vector2(0, 0));

                    dialogueImage.sprite = characterProfile.imageSpeaker;
                    dialogueNameText.text = characterProfile.nameSpeaker;

                    dialogueText.text = "";
                    dialogueBoxObject.SetActive(true);
                    menuButton.SetActive(false);
                    characterProfile.characterConversations[GameMaster.temperanceIndex].currentDialogueLine = 0;
                    StartCoroutine("WriteDialogue", characterProfile.characterConversations[GameMaster.temperanceIndex].dialogueLines[characterProfile.characterConversations[GameMaster.temperanceIndex].currentDialogueLine].dialogueText);
                }

                if (barrierToUnlock != null)
                {
                    barrierToUnlock.SetActive(false);
                }
            }
        }
    }

    public void PressButton()
    {
        nextButton.SetActive(false);
        advanceTextKeyPressed = false;

        if (characterProfile.characterConversations[GameMaster.temperanceIndex].currentDialogueLine < characterProfile.characterConversations[GameMaster.temperanceIndex].dialogueLines.Count - 1)
        {
            dialogueText.text = "";
            characterProfile.characterConversations[GameMaster.temperanceIndex].currentDialogueLine++;
            StartCoroutine("WriteDialogue", characterProfile.characterConversations[GameMaster.temperanceIndex].dialogueLines[characterProfile.characterConversations[GameMaster.temperanceIndex].currentDialogueLine].dialogueText);
        }
        else
        {
            if (GameMaster.temperanceIndex < characterProfile.characterConversations.Count - 2)
                GameMaster.temperanceIndex++;
            StartCoroutine("HideDialogue");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            keyToInteract.SetActive(false);
        }
    }

    IEnumerator WriteDialogue(string writeDialogue)
    {
        foreach (char c in writeDialogue)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(0.02f);
        }

        nextButton.SetActive(true);
    }

    IEnumerator HideDialogue()
    {
        dialogueBoxObject.GetComponent<Animator>().SetBool("Hide", true);

        yield return new WaitForSeconds(0.2f);
        playerMovement.canMove = true;
        dialogueBoxObject.SetActive(false);
    }
}
