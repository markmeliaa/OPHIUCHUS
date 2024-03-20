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

    private BattleInputManager battleInputManager;
    private BattleActionsManager battleActionsManager;
    private DisplayDialogueArea thisDialogueAreaTrigger;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        gameHUD = GameObject.FindGameObjectWithTag("GameHUD");

        playerMovement = player.GetComponent<PlayerMovement>();

        GameObject battleManagerObject = GameObject.FindGameObjectWithTag("BattleMngr");
        if (battleManagerObject != null)
        {
            battleInputManager = battleManagerObject.GetComponent<BattleInputManager>();
            battleActionsManager = battleManagerObject.GetComponent<BattleActionsManager>();
        }

        thisDialogueAreaTrigger = GetComponent<DisplayDialogueArea>();
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

    private void AdvanceConversation()
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

            if (isBossConversation)
            {
                AdvanceCharacterConversationIndexes();
            }

            StartCoroutine(nameof(HideDialogue));
        }
    }

    private void AdvanceCharacterConversationIndexes()
    {
        switch (battleActionsManager.zodiacToFight)
        {
            case "CANCER":
                ManageCancerConversationIndex();
                break;

            case "CAPRICORN":
                ManageCapricornConversationIndex();
                break;

            default:
                break;
        }
    }

    private void ManageCancerConversationIndex()
    {
        if (GameMaster.cancerIndex == 2 && GameMaster.capricornIndex == 0)
        {
            GameMaster.capricornIndex = 2;
            GameMaster.cancerIndex = 4;
            GameMaster.whoFirst = "CANCER";
        }

        if (GameMaster.cancerIndex == 4 && GameMaster.whoFirst == "CAPRICORN")
        {
            GameMaster.cancerIndex = 6;
        }
    }

    private void ManageCapricornConversationIndex()
    {
        if (GameMaster.capricornIndex == 2 && GameMaster.cancerIndex == 0)
        {
            GameMaster.cancerIndex = 2;
            GameMaster.capricornIndex = 4;
            GameMaster.whoFirst = "CAPRICORN";
        }

        if (GameMaster.capricornIndex == 4 && GameMaster.whoFirst == "CANCER")
        {
            GameMaster.capricornIndex = 6;
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

        ManageConversationOutcomes();
    }

    private void ManageConversationOutcomes()
    {
        if (isBossConversation)
        {
            if (!hasBossFightTriggered)
            {
                TriggerBossFight();
            }
            else
            {
                TriggerVictoryScreen();
            }
        }
        else
        {
            playerMovement.canMove = true;
        }
    }

    private void TriggerBossFight()
    {
        if (battleInputManager == null)
        {
            return;
        }

        battleInputManager.StartBossBattle(characterProfile.speakerName);

        hasBossFightTriggered = true;
        thisDialogueAreaTrigger.interactKeyPressed = false;
    }

    private void TriggerVictoryScreen()
    {
        if (battleActionsManager == null)
        {
            return;
        }

        battleActionsManager.WinZodiacAndFinishLevel();
    }
}
