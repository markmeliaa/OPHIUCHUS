using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ConversationManager : MonoBehaviour
{
    [SerializeField] private DialogueBox characterProfile;
    private int characterIndex;

    [SerializeField] private bool isBossConversation = true;
    private bool hasBossFightTriggered;

    // Things to show
    [SerializeField] private GameObject dialogueBoxObject;
    [SerializeField] private Image dialogueImage;
    [SerializeField] private Text dialogueNameText;
    [SerializeField] private Text dialogueText;
    [SerializeField] private GameObject nextButton;

    // Things to hide
    private GameObject player;
    private PlayerMovement playerMovement;
    private GameObject gameHUD;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        gameHUD = GameObject.FindGameObjectWithTag("GameHUD");

        playerMovement = player.GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        if (nextButton.activeSelf && Input.GetKeyDown(KeyCode.Z))
        {
            AdvanceConversation();
        }
    }

    public void TriggerNextConversation()
    {
        playerMovement.StopPlayer();

        SetUpDialogueBox();

        characterIndex = NameToCharIndex.GetCharacterIndexFromName(characterProfile.speakerName);
        int currentDialogueLine = characterProfile.characterConversations[characterIndex].currentDialogueLine;
        StartCoroutine(nameof(WriteDialogue),
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
        int maxDialogueLine = characterProfile.characterConversations[characterIndex].dialogueLines.Count - 1;

        if (currentDialogueLine < maxDialogueLine)
        {
            dialogueText.text = "";

            int nextDialogueLine = ++characterProfile.characterConversations[characterIndex].currentDialogueLine;
            StartCoroutine(nameof(WriteDialogue),
                           characterProfile.characterConversations[characterIndex].dialogueLines[nextDialogueLine].dialogueText);
        }
        else
        {
            if (characterIndex <= characterProfile.characterConversations.Count - 1)
            {
                NameToCharIndex.SetCharacterIndexFromName(characterProfile.speakerName, characterIndex + 1);
            }

            StartCoroutine(nameof(HideDialogue));
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

        dialogueBoxObject.SetActive(false);

        if (isBossConversation)
        { 
            if (!hasBossFightTriggered)
            {
                // Trigger Boss Fight
                BattleActionsManager battleActions = GameObject.FindGameObjectWithTag("BattleMngr").GetComponent<BattleActionsManager>();
                BattleInputManager inputManager = GameObject.FindGameObjectWithTag("BattleMngr").GetComponent<BattleInputManager>();

                battleActions.SetUpBossBattle(characterProfile.speakerName);
                inputManager.StartBossBattle(characterProfile.speakerName);

                hasBossFightTriggered = true;
            }
            else
            {
                // Trigger End Game (Victory)
            }
        }
        else
        {
            playerMovement.canMove = true;
        }
    }
}
