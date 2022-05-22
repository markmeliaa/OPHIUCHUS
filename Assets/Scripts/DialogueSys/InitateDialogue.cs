using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InitateDialogue : MonoBehaviour
{
    public GameObject eKey;
    public GameObject playBarrier;
    private ButtonManager buttonManager;
    public string zodiac;

    public GameObject dialogueBox;
    public GameObject menuButton;
    public Text dialogueText;
    public GameObject nextButton;

    public PlayerMoveIso2 playerMove;

    private bool ePressed = false;
    private bool pressedZ = false;

    public DialogueBox thisCharacter;
    public Image dialogueImage;
    public Text dialogueNameText;

    private void Awake()
    {
        if (SceneManager.GetActiveScene().buildIndex != 1)
            buttonManager = GameObject.FindGameObjectWithTag("Buttons").GetComponent<ButtonManager>();
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            if (nextButton.activeSelf && Input.GetKey(KeyCode.Z) && !pressedZ)
            {
                pressedZ = true;
                PressButton();
            }
        }

        else
        {
            if (buttonManager.nextButton.activeSelf && Input.GetKey(KeyCode.Z) && !pressedZ)
            {
                buttonManager.pressedZ = true;
                buttonManager.PressButton();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (!ePressed)
                eKey.SetActive(true);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (!ePressed)
                eKey.SetActive(true);

            if (Input.GetKey(KeyCode.E) && !ePressed)
            {
                ePressed = true;
                eKey.SetActive(false);
                //Debug.Log("Start Conversation");

                if (SceneManager.GetActiveScene().buildIndex != 1)
                {
                    buttonManager.StartDialogue(zodiac, thisCharacter);
                }

                else
                {
                    playerMove.moving = false;
                    playerMove.rendIso.SetDirection(new Vector2(0, 0));
                    playerMove.horInput = 0;
                    playerMove.vertInput = 0;

                    dialogueImage.sprite = thisCharacter.imageSpeaker;
                    dialogueNameText.text = thisCharacter.nameSpeaker;

                    dialogueText.text = "";
                    dialogueBox.SetActive(true);
                    menuButton.SetActive(false);
                    thisCharacter.characterConversations[GameMaster.temperanceIndex].currentDialogueLine = 0;
                    StartCoroutine("WriteDialogue", thisCharacter.characterConversations[GameMaster.temperanceIndex].dialogueLines[thisCharacter.characterConversations[GameMaster.temperanceIndex].currentDialogueLine].dialogueText);
                }

                // Leave the access to Andromeda open
                if (playBarrier != null)
                    playBarrier.SetActive(false);
            }
        }
    }

    public void PressButton()
    {
        nextButton.SetActive(false);
        pressedZ = false;

        if (thisCharacter.characterConversations[GameMaster.temperanceIndex].currentDialogueLine < thisCharacter.characterConversations[GameMaster.temperanceIndex].dialogueLines.Count - 1)
        {
            dialogueText.text = "";
            thisCharacter.characterConversations[GameMaster.temperanceIndex].currentDialogueLine++;
            StartCoroutine("WriteDialogue", thisCharacter.characterConversations[GameMaster.temperanceIndex].dialogueLines[thisCharacter.characterConversations[GameMaster.temperanceIndex].currentDialogueLine].dialogueText);
        }

        else
        {
            if (GameMaster.temperanceIndex < thisCharacter.characterConversations.Count - 2)
                GameMaster.temperanceIndex++;
            StartCoroutine("HideDialogue");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            eKey.SetActive(false);
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
        dialogueBox.GetComponent<Animator>().SetBool("Hide", true);

        yield return new WaitForSeconds(0.2f);
        playerMove.moving = true;
        dialogueBox.SetActive(false);
    }
}
