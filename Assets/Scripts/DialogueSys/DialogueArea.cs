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

    private void Start()
    {
        playerMovement = player.GetComponent<PlayerMovement>();
        playerAnimationDirection = player.GetComponent<PlayerAnimationDirection>();
    }

    private void Update()
    {
        if (nextButton.activeSelf && Input.GetKeyDown(KeyCode.Z))
        {
            AdvanceConversation();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !interactKeyPressed)
        {
            keyToInteract.SetActive(true);
        }

        // TODO: Check differences when triggering dialogue in scene 2 (buttonManager.PressButton())
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

                TriggerConversation();

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

    private void TriggerConversation()
    {
        playerMovement.StopPlayer();

        SetUpDialogueBox();

        int convoNumber = GetCharacterIndexFromName(characterProfile.name);
        int currentDialogueLine = characterProfile.characterConversations[GameMaster.temperanceIndex].currentDialogueLine;
        StartCoroutine("WriteDialogue", 
                       characterProfile.characterConversations[convoNumber].dialogueLines[currentDialogueLine].dialogueText);

        // TODO: Check differences when triggering dialogue in scene 2 (buttonManager.StartDialogue(characterProfile.name, characterProfile))
    }

    private void SetUpDialogueBox()
    {
        dialogueImage.sprite = characterProfile.imageSpeaker;
        dialogueNameText.text = characterProfile.nameSpeaker;
        dialogueText.text = "";

        dialogueBoxObject.SetActive(true);
        menuButton.SetActive(false);

        // Important!! Start current dialogue line accordingly
        characterProfile.characterConversations[GameMaster.temperanceIndex].currentDialogueLine = 0;
    }

    private void AdvanceConversation()
    {
        nextButton.SetActive(false);

        if (characterProfile.characterConversations[GameMaster.temperanceIndex].currentDialogueLine < characterProfile.characterConversations[GameMaster.temperanceIndex].dialogueLines.Count - 1)
        {
            dialogueText.text = "";
            characterProfile.characterConversations[GameMaster.temperanceIndex].currentDialogueLine++;
            StartCoroutine("WriteDialogue", characterProfile.characterConversations[GameMaster.temperanceIndex].dialogueLines[characterProfile.characterConversations[GameMaster.temperanceIndex].currentDialogueLine].dialogueText);
        }
        else
        {
            if (GameMaster.temperanceIndex < characterProfile.characterConversations.Count - 2)
            {
                GameMaster.temperanceIndex++;
            }
                
            StartCoroutine("HideDialogue");
        }
    }

    IEnumerator WriteDialogue(string dialogueToWrite)
    {
        foreach (char c in dialogueToWrite)
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

    private int GetCharacterIndexFromName(string name)
    {
        int characterIndex = -1;

        switch (name)
        {
            case "TEMPERANCE":
                characterIndex = GameMaster.temperanceIndex;
                break;

            case "CAPRICORN":
                characterIndex = GameMaster.capricornIndex;
                break;

            case "CANCER":
                characterIndex = GameMaster.cancerIndex;
                break;

            default: 
                break;
        }

        return characterIndex;
    }
}