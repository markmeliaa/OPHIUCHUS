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

    private string dialogue = "Hola muy buenas Theo Lof, a�sldkfj� las�ldkf a�slkdfj �j �lkasjd lksadjf� sldfj  sa�lkdfj�lsakdfj�alskfdj�lsakfd " +
        "j�lksadjf �lksajd� fksjf� alkjsa fs�alkjdf�lkalksdjf�lasdjf� kja� lksja d�lkfjas�dlk fjas�dlkf j�salkdj f�lksajdf� kajsd�flk jsa�lkf jsa� �aksjd �lfsajd �flskadjf� sa�l";

    private bool ePressed = false;
    private bool pressedZ = false;

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
                pressedZ = true;
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
                    buttonManager.StartDialogue(zodiac);
                }

                else
                {
                    playerMove.moving = false;
                    playerMove.rendIso.SetDirection(new Vector2(0, 0));
                    playerMove.horInput = 0;
                    playerMove.vertInput = 0;

                    dialogueText.text = "";
                    dialogueBox.SetActive(true);
                    menuButton.SetActive(false);
                    StartCoroutine("WriteDialogue", dialogue);
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

        StartCoroutine("HideDialogue");
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
            yield return new WaitForSeconds(0.012f);
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
