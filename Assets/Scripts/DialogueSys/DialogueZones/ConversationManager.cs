using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ConversationManager : MonoBehaviour
{
    [SerializeField] private DialogueBox characterProfile;
    private int characterIndex;

    // Things to show
    [SerializeField] private GameObject dialogueBoxObject;
    [SerializeField] private Image dialogueImage;
    [SerializeField] private Text dialogueNameText;
    [SerializeField] private Text dialogueText;
    [SerializeField] private GameObject nextButton;

    // Things to hide
    [SerializeField] private GameObject player;
    private PlayerMovement playerMovement;
    [SerializeField] private GameObject gameHUD;

    private void Start()
    {
        playerMovement = player.GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        if (nextButton.activeSelf && Input.GetKeyDown(KeyCode.Z))
        {
            AdvanceConversation();
        }
    }

    public void TriggerConversation()
    {
        playerMovement.StopPlayer();

        SetUpDialogueBox();

        characterIndex = NameToCharIndex.GetCharacterIndexFromName(characterProfile.speakerName);
        int currentDialogueLine = characterProfile.characterConversations[characterIndex].currentDialogueLine;
        StartCoroutine("WriteDialogue",
                       characterProfile.characterConversations[characterIndex].dialogueLines[currentDialogueLine].dialogueText);

        // TODO: Check differences when triggering dialogue in scene 2
        // buttonManager.StartDialogue(characterProfile.nameSpeaker, characterProfile) was used
    }

    private void SetUpDialogueBox()
    {
        dialogueImage.sprite = characterProfile.speakerImage;
        dialogueNameText.text = characterProfile.speakerName;
        dialogueText.text = "";

        dialogueBoxObject.SetActive(true);
        gameHUD.SetActive(false);

        // Important!! Start current dialogue line to 0 each time a conversation is triggered (for safety)
        characterProfile.characterConversations[characterIndex].currentDialogueLine = 0;
    }

    public void AdvanceConversation()
    {
        nextButton.SetActive(false);

        int currentDialogueLine = characterProfile.characterConversations[characterIndex].currentDialogueLine;
        int lastDialogueLine = characterProfile.characterConversations[characterIndex].dialogueLines.Count - 1;

        if (currentDialogueLine < lastDialogueLine)
        {
            dialogueText.text = "";

            int nextDialogueLine = ++characterProfile.characterConversations[characterIndex].currentDialogueLine;
            StartCoroutine("WriteDialogue",
                           characterProfile.characterConversations[characterIndex].dialogueLines[nextDialogueLine].dialogueText);
        }
        else
        {
            if (characterIndex <= characterProfile.characterConversations.Count - 1)
            {
                NameToCharIndex.SetCharacterIndexFromName(characterProfile.speakerName, characterIndex + 1);
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
}
