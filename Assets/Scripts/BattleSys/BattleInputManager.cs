using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BattleType
{
    NONE,
    NORMAL,
    BOSS
}

public enum BattleActions
{
    NONE,
    ATTACK,
    TALK,
    USE_ITEM,
    RUN
}

public class BattleInputManager : MonoBehaviour
{
    private BattleActionsManager battleActionsManager;

    private bool leftRightKeyPressed;
    private bool upDownKeyPressed;
    private bool selectionKeyPressed;

    private bool bossBeaten;

    [Header("PLAYER (STAR) VARIABLES")]
    [HideInInspector] public bool playerStarCanMove;
    public GameObject playerStarObject;
    public GameObject playerStarInitialPosition;
    [Space(5)]

    [Header("BATTLE UI VARIABLES")]
    public GameObject battleCanvas;
    public List<GameObject> battleActionButtons;
    [HideInInspector] public int currentActionButtonIndex;
    [HideInInspector] public int currentHoveredTextIndex;

    public List<GameObject> textsForEnemiesInBattle;
    public List<GameObject> textForItemsToBeUsed;

    [SerializeField] private List<GameObject> battleStartStarAnimations;
    [SerializeField] private GameObject blackScreen;
    [Space(5)]

    [Header("DIALOGUE UI VARIABLES")]
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private Text dialogueText;
    [SerializeField] private GameObject nextButton;

    [SerializeField] private Image dialogueImage;
    [SerializeField] private Text dialogueNameText;

    private DialogueBox characterSpeaker;
    [Space(5)]

    [Header("THINGS TO SHOW/HIDE")]
    public GameObject overworldPlayer;
    [SerializeField] private GameObject dungeonGameRoomsParent;
    public GameObject dungeonMinimapRoomsParent;
    [SerializeField] private GameObject menuButton;
    [Space(5)]

    [Header("AUDIO VARIABLES")]
    [SerializeField] private AudioSource globalAudioSource;
    [SerializeField] private AudioClip selectOptionSound;
    [SerializeField] private AudioClip attackEnemyAudio;
    [SerializeField] private AudioClip useItemAudio;

    void Start()
    {
        battleActionsManager = GetComponent<BattleActionsManager>();

        // Uncomment for the trial scene only
        //StartBattle();
    }

    void Update()
    {
        if (!battleCanvas.activeSelf || battleActionsManager.currentBattleState == GameStates.NONE)
        {
            return;
        }

        if (Input.GetKeyUp(KeyCode.Z))
        {
            selectionKeyPressed = false;
        }

        float leftRightInput = Input.GetAxis("Horizontal");
        float upDownInput = Input.GetAxis("Vertical");

        if (battleActionsManager.currentBattleState == GameStates.CHOOSING)
        {
            ManageBattleActionSelection(leftRightInput);

            if (currentActionButtonIndex + 1 == (int)BattleActions.ATTACK && Input.GetKeyDown(KeyCode.Z))
            {
                ShowBattleEnemiesTexts();

                battleActionsManager.lastBattleState = battleActionsManager.currentBattleState;
                battleActionsManager.currentBattleState = GameStates.ATTACKING;

                selectionKeyPressed = true;
            }
            else if (currentActionButtonIndex + 1 == (int)BattleActions.TALK && Input.GetKeyDown(KeyCode.Z))
            {
                ShowBattleEnemiesTexts();

                battleActionsManager.lastBattleState = battleActionsManager.currentBattleState;
                battleActionsManager.currentBattleState = GameStates.TALKING;

                selectionKeyPressed = true;
            }
            else if (currentActionButtonIndex + 1 == (int)BattleActions.USE_ITEM && Input.GetKeyDown(KeyCode.Z))
            {
                globalAudioSource.clip = selectOptionSound;
                globalAudioSource.Play();

                if (GameMaster.inventory.Count != 0)
                {
                    ShowInventoryItemsTexts();

                    battleActionsManager.battleDialogueText.SetActive(false);

                    battleActionsManager.lastBattleState = battleActionsManager.currentBattleState;
                    battleActionsManager.currentBattleState = GameStates.USING_ITEM;
                }
                else
                {
                    battleActionsManager.battleDialogueText.GetComponent<Text>().text = "    YOU HAVE NO ITEMS RIGHT NOW";

                    battleActionsManager.lastBattleState = battleActionsManager.currentBattleState;
                    battleActionsManager.currentBattleState = GameStates.WAITING;
                }

                selectionKeyPressed = true;
            }
            // Select RUN action
            else if (currentActionButtonIndex + 1 == (int)BattleActions.RUN && Input.GetKeyDown(KeyCode.Z))
            {
                globalAudioSource.clip = selectOptionSound;
                globalAudioSource.Play();

                battleActionsManager.lastBattleState = battleActionsManager.currentBattleState;
                battleActionsManager.currentBattleState = GameStates.RUNNING;

                battleActionsManager.RunAction();

                selectionKeyPressed = true;
            }
        }

        if (battleActionsManager.currentBattleState == GameStates.ATTACKING ||
            battleActionsManager.currentBattleState == GameStates.TALKING)
        {
            ManageEnemySelection(upDownInput);

            if (battleActionsManager.currentBattleState == GameStates.ATTACKING && 
                Input.GetKeyDown(KeyCode.Z) && !selectionKeyPressed)
            {
                globalAudioSource.clip = attackEnemyAudio;
                globalAudioSource.Play();

                battleActionsManager.AttackAction(textsForEnemiesInBattle, currentHoveredTextIndex);

                selectionKeyPressed = true;
            }
            else if (battleActionsManager.currentBattleState == GameStates.TALKING && 
                     Input.GetKeyDown(KeyCode.Z) && !selectionKeyPressed)
            {
                globalAudioSource.clip = selectOptionSound;
                globalAudioSource.Play();

                battleActionsManager.TalkAction(textsForEnemiesInBattle);

                selectionKeyPressed = true;
            }
        }

        if (battleActionsManager.currentBattleState == GameStates.USING_ITEM)
        {
            ManageItemSelection(upDownInput, leftRightInput);

            if (Input.GetKeyDown(KeyCode.Z) && !selectionKeyPressed)
            {
                globalAudioSource.clip = useItemAudio;
                globalAudioSource.Play();

                battleActionsManager.UseItemAction(textForItemsToBeUsed, currentHoveredTextIndex);

                selectionKeyPressed = true;
            }
        }

        // Move the player star while the enemies are attacking
        if (battleActionsManager.currentBattleState == GameStates.DEFENDING)
        {
            if (!selectionKeyPressed && Input.GetKeyDown(KeyCode.Z) && !playerStarCanMove)
            {
                battleActionsManager.StartEnemyAttack();
                StartCoroutine(nameof(WaitMove));

                selectionKeyPressed = true;
            }

            if (playerStarCanMove)
            {
                Rigidbody2D rb = playerStarObject.GetComponent<Rigidbody2D>();

                Vector2 currentPos = rb.position;

                float horInput = Input.GetAxis("Horizontal");
                float vertInput = Input.GetAxis("Vertical");

                Vector2 inputVect = new Vector2(horInput, vertInput);

                // Prevent diagonal movement to be faster than cardinal direction movement
                inputVect = Vector2.ClampMagnitude(inputVect, 1);

                Vector2 movement = inputVect * GameMaster.playerSpeed;
                Vector2 newPos = currentPos + movement * Time.fixedDeltaTime;

                rb.MovePosition(newPos);
            }

            return;
        }

        // The battle has ended
        if (battleActionsManager.currentBattleState == GameStates.VICTORY)
        {
            if (!selectionKeyPressed && Input.GetKeyDown(KeyCode.Z))
            {
                battleActionsManager.WinBattle();
                selectionKeyPressed = true;
            }

            return;
        }

        if (battleActionsManager.currentBattleState == GameStates.DEFEAT)
        {
            if (!selectionKeyPressed && Input.GetKeyDown(KeyCode.Z) && battleActionsManager.zodiacToFight == "")
            {
                StartCoroutine(nameof(WaitFinishGame));
            }
            else if (!selectionKeyPressed && Input.GetKeyDown(KeyCode.Z) && battleActionsManager.zodiacToFight != "")
            {
                StartCoroutine(nameof(WaitFinishBossGame));
            }

            return;
        }

        // Do nothing
        if (battleActionsManager.currentBattleState == GameStates.WAITING)
        {
            if (Input.GetKeyDown(KeyCode.X) && battleActionsManager.lastBattleState == GameStates.TALKING)
            {
                foreach (GameObject text in textsForEnemiesInBattle)
                {
                    text.SetActive(true);
                }

                battleActionsManager.battleDialogueText.SetActive(false);
                battleActionsManager.currentBattleState = GameStates.TALKING;
                battleActionsManager.lastBattleState = GameStates.WAITING;

                globalAudioSource.clip = selectOptionSound;
                globalAudioSource.Play();
            }
            else if (Input.GetKeyDown(KeyCode.Z) && battleActionsManager.lastBattleState == GameStates.ATTACKING)
            {
                foreach (GameObject text in textsForEnemiesInBattle)
                {
                    text.SetActive(true);
                }

                battleActionsManager.battleDialogueText.SetActive(false);
                battleActionsManager.currentBattleState = GameStates.ATTACKING;
                battleActionsManager.lastBattleState = GameStates.WAITING;

                selectionKeyPressed = true;

                globalAudioSource.clip = selectOptionSound;
                globalAudioSource.Play();
            }
            else if (Input.GetKeyDown(KeyCode.X) && battleActionsManager.lastBattleState == GameStates.USING_ITEM)
            {
                battleActionsManager.battleDialogueText.GetComponent<Text>().text = battleActionsManager.textToDisplay;

                battleActionsManager.currentBattleState = GameStates.CHOOSING;
                battleActionsManager.lastBattleState = GameStates.WAITING;

                globalAudioSource.clip = selectOptionSound;
                globalAudioSource.Play();
            }
            else if (battleActionsManager.lastBattleState == GameStates.RUNNING)
            {
                if (GameMaster.playerSpeed >= 5 && Input.GetKeyDown(KeyCode.Z) && !selectionKeyPressed && battleActionsManager.zodiacToFight == "")
                {
                    battleActionsManager.currentBattleState = GameStates.NONE;
                    battleActionsManager.lastBattleState = GameStates.WAITING;

                    StartCoroutine(nameof(WaitFinishGame));

                    selectionKeyPressed = true;

                    globalAudioSource.clip = selectOptionSound;
                    globalAudioSource.Play();
                }
                else if ((GameMaster.playerSpeed < 5 || battleActionsManager.zodiacToFight != "") && Input.GetKeyDown(KeyCode.X))
                {
                    battleActionsManager.currentBattleState = GameStates.CHOOSING;
                    battleActionsManager.lastBattleState = GameStates.WAITING;

                    battleActionsManager.battleDialogueText.GetComponent<Text>().text = battleActionsManager.textToDisplay;

                    globalAudioSource.clip = selectOptionSound;
                    globalAudioSource.Play();
                }
            }
        }

        // Go back to the previous battle screen
        if (Input.GetKeyDown(KeyCode.X))
        {
            if (battleActionsManager.currentBattleState == GameStates.ATTACKING || battleActionsManager.currentBattleState == GameStates.TALKING)
            {
                battleActionsManager.currentBattleState = GameStates.CHOOSING;
                battleActionsManager.battleDialogueText.GetComponent<Text>().text = battleActionsManager.textToDisplay;

                // Disable enemy texts
                textsForEnemiesInBattle[currentHoveredTextIndex].GetComponent<Text>().enabled = true;
                textsForEnemiesInBattle[currentHoveredTextIndex].transform.GetChild(0).gameObject.SetActive(false);
                textsForEnemiesInBattle[currentHoveredTextIndex].transform.GetChild(1).gameObject.SetActive(false);

                foreach (GameObject text in textsForEnemiesInBattle)
                {
                    text.SetActive(false);
                }

                battleActionsManager.battleDialogueText.SetActive(true);

                globalAudioSource.clip = selectOptionSound;
                globalAudioSource.Play();
            }
            else if (battleActionsManager.currentBattleState == GameStates.USING_ITEM)
            {
                battleActionsManager.currentBattleState = GameStates.CHOOSING;
                battleActionsManager.battleDialogueText.GetComponent<Text>().text = battleActionsManager.textToDisplay;

                // Disable item texts
                textForItemsToBeUsed[currentHoveredTextIndex].GetComponent<Text>().enabled = true;
                textForItemsToBeUsed[currentHoveredTextIndex].transform.GetChild(0).gameObject.SetActive(false);
                textForItemsToBeUsed[currentHoveredTextIndex].transform.GetChild(1).gameObject.SetActive(false);

                foreach (GameObject text in textForItemsToBeUsed)
                {
                    text.SetActive(false);
                }

                battleActionsManager.battleDialogueText.SetActive(true);

                globalAudioSource.clip = selectOptionSound;
                globalAudioSource.Play();
            }
        }
    }
    
    // Start and end battle functions
    public void StartBattle()
    {
        battleActionsManager.SetUpBattle();

        overworldPlayer.GetComponent<PlayerAnimationDirection>().SetDirection(new Vector2(0, 0));
        dungeonMinimapRoomsParent.SetActive(false);

        //animCanvas.GetComponent<AudioSource>().Play();

        foreach (GameObject animator in battleStartStarAnimations)
        {
            animator.GetComponent<Animator>().SetBool("Change", true);
            animator.transform.GetChild(1).GetComponent<Animator>().SetBool("Change", true);
        }

        StartCoroutine(nameof(WaitStartGame), BattleType.NORMAL);
    }

    public void StartBossBattle(string zodiac)
    {
        battleActionsManager.SetUpBossBattle(zodiac);

        overworldPlayer.GetComponent<PlayerAnimationDirection>().SetDirection(new Vector2(0, 0));
        dungeonMinimapRoomsParent.SetActive(false);

        //animCanvas.GetComponent<AudioSource>().Play();

        foreach (GameObject animator in battleStartStarAnimations)
        {
            animator.GetComponent<Animator>().SetBool("Change", true);
            animator.transform.GetChild(1).GetComponent<Animator>().SetBool("Change", true);
        }

        StartCoroutine(nameof(WaitStartGame), BattleType.BOSS);
    }

    public void ClearGame()
    {
        battleActionButtons[currentActionButtonIndex].GetComponent<ManageButtonSelection>().OnExitSelection();

        for (int i = 2; i < battleActionsManager.enemiesParent.transform.childCount; i++)
        {
            Destroy(battleActionsManager.enemiesParent.transform.GetChild(i).gameObject);
        }

        foreach (GameObject text in textsForEnemiesInBattle)
        {
            text.GetComponent<Text>().text = "";
            text.transform.GetChild(1).GetComponent<Text>().text = "";
        }

        foreach (GameObject text in textForItemsToBeUsed)
        {
            text.GetComponent<Text>().text = "";
            text.transform.GetChild(1).GetComponent<Text>().text = "";
        }
    }

    // Dialogue functions
    public void StartDialogue(string zodiac, DialogueBox thisChar)
    {
        dungeonMinimapRoomsParent.SetActive(false);
        battleActionsManager.zodiacToFight = zodiac;
        characterSpeaker = thisChar;

        overworldPlayer.GetComponent<PlayerAnimationDirection>().SetDirection(new Vector2(0, 0));

        dialogueImage.sprite = thisChar.speakerImage;
        dialogueNameText.text = thisChar.speakerName;

        dialogueText.text = "";
        dialogueBox.SetActive(true);
        menuButton.SetActive(false);

        if (zodiac == "CANCER")
        {
            if (GameMaster.cancerIndex == characterSpeaker.characterConversations.Count - 1)
            {
                GameMaster.cancerIndex--;
            }

            thisChar.characterConversations[GameMaster.cancerIndex].currentDialogueLine = 0;
            StartCoroutine(nameof(WriteDialogue), thisChar.characterConversations[GameMaster.cancerIndex].dialogueLines[thisChar.characterConversations[GameMaster.cancerIndex].currentDialogueLine].dialogueText);
        }

        else if (zodiac == "CAPRICORN")
        {
            if (GameMaster.capricornIndex == characterSpeaker.characterConversations.Count - 1)
            {
                GameMaster.capricornIndex--;
            }

            thisChar.characterConversations[GameMaster.capricornIndex].currentDialogueLine = 0;
            StartCoroutine(nameof(WriteDialogue), thisChar.characterConversations[GameMaster.capricornIndex].dialogueLines[thisChar.characterConversations[GameMaster.capricornIndex].currentDialogueLine].dialogueText);
        }
    }

    public void PressButton()
    {
        nextButton.SetActive(false);
        selectionKeyPressed = false;

        if (battleActionsManager.zodiacToFight == "CANCER")
        {
            if (characterSpeaker.characterConversations[GameMaster.cancerIndex].currentDialogueLine < characterSpeaker.characterConversations[GameMaster.cancerIndex].dialogueLines.Count - 1)
            {
                dialogueText.text = "";
                characterSpeaker.characterConversations[GameMaster.cancerIndex].currentDialogueLine++;
                StartCoroutine(nameof(WriteDialogue), characterSpeaker.characterConversations[GameMaster.cancerIndex].dialogueLines[characterSpeaker.characterConversations[GameMaster.cancerIndex].currentDialogueLine].dialogueText);
            }

            else
            {
                if (GameMaster.cancerIndex < characterSpeaker.characterConversations.Count - 1)
                {
                    GameMaster.cancerIndex++;
                }

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

                StartCoroutine(nameof(HideDialogue));
            }
        }

        else if (battleActionsManager.zodiacToFight == "CAPRICORN")
        {
            if (characterSpeaker.characterConversations[GameMaster.capricornIndex].currentDialogueLine < characterSpeaker.characterConversations[GameMaster.capricornIndex].dialogueLines.Count - 1)
            {
                dialogueText.text = "";
                characterSpeaker.characterConversations[GameMaster.capricornIndex].currentDialogueLine++;
                StartCoroutine(nameof(WriteDialogue), characterSpeaker.characterConversations[GameMaster.capricornIndex].dialogueLines[characterSpeaker.characterConversations[GameMaster.capricornIndex].currentDialogueLine].dialogueText);
            }

            else
            {
                if (GameMaster.capricornIndex < characterSpeaker.characterConversations.Count - 1)
                {
                    GameMaster.capricornIndex++;
                }

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

                StartCoroutine(nameof(HideDialogue));
            }
        }
    }

    void ManageBattleActionSelection(float leftRightInput)
    {
        if (leftRightInput != 0.0f && !leftRightKeyPressed)
        {
            leftRightKeyPressed = true;
            ManageButtonSelection currentButtonSelection =
                battleActionButtons[currentActionButtonIndex].GetComponent<ManageButtonSelection>();

            if (leftRightInput < 0.0f && currentActionButtonIndex == 0)
            {
                currentActionButtonIndex = battleActionButtons.Count - 1;
            }
            else if (leftRightInput < 0.0f)
            {
                currentActionButtonIndex--;
            }
            else if (leftRightInput > 0.0f && currentActionButtonIndex == battleActionButtons.Count - 1)
            {
                currentActionButtonIndex = 0;
            }
            else if (leftRightInput > 0.0f)
            {
                currentActionButtonIndex++;
            }

            ManageButtonSelection nextButtonSelection =
                battleActionButtons[currentActionButtonIndex].GetComponent<ManageButtonSelection>();

            currentButtonSelection.OnExitSelection();
            nextButtonSelection.OnSelection();
        }
        else if (leftRightInput == 0.0f && leftRightKeyPressed)
        {
            leftRightKeyPressed = false;
        }
    }

    void ShowBattleEnemiesTexts()
    {
        battleActionsManager.battleDialogueText.SetActive(false);
        globalAudioSource.clip = selectOptionSound;
        globalAudioSource.Play();

        for (int i = 0; i < battleActionsManager.enemiesSpawned.Count; i++)
        {
            GameObject currentEnemyText = textsForEnemiesInBattle[i];
            string enemyName = battleActionsManager.enemiesSpawned[i].GetComponent<EnemyCard>().CardName;

            currentEnemyText.SetActive(true);
            currentEnemyText.GetComponent<Text>().text = enemyName;
            currentEnemyText.transform.GetChild(1).GetComponent<Text>().text = "    " + enemyName;

            // TODO: This variable is never used, check if it is really necessary
            // battleManager.enemiesSpawned[i].GetComponent<EnemyCard>().nameText = enemyTexts[i];

            Color bossColor = new Color(0.925f, 0.835f, 0.0f);
            Color redCardsColor = new Color(1.0f, 0.0f, 0.31f);
            Color blueCardsColor = new Color(0.19f, 0.68f, 1.0f);

            bool isBossFight = battleActionsManager.zodiacToFight != "";
            bool isRedCard = battleActionsManager.enemiesSpawned[i].name[0].ToString() == "D" ||
                             battleActionsManager.enemiesSpawned[i].name[0].ToString() == "H" ||
                             battleActionsManager.enemiesSpawned[i].name[0].ToString() == "P";

            if (isBossFight)
            {
                currentEnemyText.GetComponent<Text>().color = bossColor;
                currentEnemyText.transform.GetChild(1).GetComponent<Text>().color = bossColor;
            }
            else if (isRedCard)
            {
                currentEnemyText.GetComponent<Text>().color = redCardsColor;
                currentEnemyText.transform.GetChild(1).GetComponent<Text>().color = redCardsColor;
            }
            else
            {
                currentEnemyText.GetComponent<Text>().color = blueCardsColor;
                currentEnemyText.transform.GetChild(1).GetComponent<Text>().color = blueCardsColor;
            }
        }

        SetFirstElementSelectedByDefault(textsForEnemiesInBattle);
    }

    void ShowInventoryItemsTexts()
    {
        for (int i = 0; i < GameMaster.inventory.Count; i++)
        {
            GameObject currentItemText = textForItemsToBeUsed[i];
            string itemName = GameMaster.inventory[i].ObjectName;
            int itemLevel = GameMaster.inventory[i].Level;

            currentItemText.SetActive(true);
            currentItemText.GetComponent<Text>().text = itemName + " LVL." + itemLevel;
            currentItemText.transform.GetChild(1).GetComponent<Text>().text = "    " + itemName + " LVL." + itemLevel;

            // TODO: This variable is never used, check if it is really necessary
            //GameMaster.inventory[i].GameText = textForItemsToBeUsed[i];

            Color colorForItem = Color.white;
            switch (GameMaster.inventory[i].Type)
            {
                case ObjectTypes.HEALTH:
                    colorForItem = Color.red;
                    break;

                case ObjectTypes.ATTACK:
                    colorForItem = new Color(1.0f, 0.37f, 0.0f); // Orange
                    break;

                case ObjectTypes.DEFENSE:
                    colorForItem = new Color(0.0f, 0.36f, 1.0f); // Blue
                    break;

                case ObjectTypes.SPEED:
                    colorForItem = Color.green;
                    break;

                default:
                    break;
            }

            currentItemText.GetComponent<Text>().color = colorForItem;
            currentItemText.transform.GetChild(1).GetComponent<Text>().color = colorForItem;
        }

        SetFirstElementSelectedByDefault(textForItemsToBeUsed);
    }

    void SetFirstElementSelectedByDefault(List<GameObject> listOfTexts)
    {
        currentHoveredTextIndex = 0;
        listOfTexts[currentHoveredTextIndex].GetComponent<Text>().enabled = false;
        listOfTexts[currentHoveredTextIndex].transform.GetChild(0).gameObject.SetActive(true);
        listOfTexts[currentHoveredTextIndex].transform.GetChild(1).gameObject.SetActive(true);
    }

    void ManageEnemySelection(float upDownInput)
    {
        if (upDownInput != 0.0f && !upDownKeyPressed)
        {
            GameObject currentSelectedEnemyText = textsForEnemiesInBattle[currentHoveredTextIndex];
            upDownKeyPressed = true;

            if (upDownInput < 0.0f && currentHoveredTextIndex == 0)
            {
                currentHoveredTextIndex = battleActionsManager.enemiesSpawned.Count - 1;
            }
            else if (upDownInput < 0.0f)
            {
                currentHoveredTextIndex--;
            }
            else if (upDownInput > 0.0f && currentHoveredTextIndex == battleActionsManager.enemiesSpawned.Count - 1)
            {
                currentHoveredTextIndex = 0;
            }
            else if (upDownInput > 0.0f)
            {
                currentHoveredTextIndex++;
            }

            GameObject nextSelectedEnemyText = textsForEnemiesInBattle[currentHoveredTextIndex];

            currentSelectedEnemyText.GetComponent<Text>().enabled = true;
            currentSelectedEnemyText.transform.GetChild(0).gameObject.SetActive(false);
            currentSelectedEnemyText.transform.GetChild(1).gameObject.SetActive(false);

            nextSelectedEnemyText.GetComponent<Text>().enabled = false;
            nextSelectedEnemyText.transform.GetChild(0).gameObject.SetActive(true);
            nextSelectedEnemyText.transform.GetChild(1).gameObject.SetActive(true);

        }
        else if (upDownInput == 0.0f && upDownKeyPressed)
        {
            upDownKeyPressed = false;
        }
    }

    void ManageItemSelection(float upDownInput, float leftRightInput)
    {
        GameObject currentSelectedItemText = textForItemsToBeUsed[currentHoveredTextIndex];

        if (upDownInput != 0.0f && !upDownKeyPressed)
        {
            upDownKeyPressed = true;

            if (upDownInput > 0.0f && currentHoveredTextIndex != 0)
            {
                currentHoveredTextIndex--;

            }
            else if (upDownInput < 0.0f && currentHoveredTextIndex != GameMaster.inventory.Count - 1)
            {
                currentHoveredTextIndex++;
            }
        }
        else if (upDownInput == 0.0f && upDownKeyPressed)
        {
            upDownKeyPressed = false;
        }

        if (leftRightInput != 0.0f && !leftRightKeyPressed)
        {
            leftRightKeyPressed = true;

            if (leftRightInput > 0.0f && currentHoveredTextIndex + 4 <= GameMaster.inventory.Count - 1)
            {
                currentHoveredTextIndex += 4;
            }
            else if (leftRightInput < 0.0f && currentHoveredTextIndex - 4 >= 0)
            {
                currentHoveredTextIndex -= 4;
            }

        }
        else if (leftRightInput == 0.0f && leftRightKeyPressed)
        {
            leftRightKeyPressed = false;
        }

        GameObject nextSelectedItemText = textForItemsToBeUsed[currentHoveredTextIndex];

        currentSelectedItemText.GetComponent<Text>().enabled = true;
        currentSelectedItemText.transform.GetChild(0).gameObject.SetActive(false);
        currentSelectedItemText.transform.GetChild(1).gameObject.SetActive(false);

        nextSelectedItemText.GetComponent<Text>().enabled = false;
        nextSelectedItemText.transform.GetChild(0).gameObject.SetActive(true);
        nextSelectedItemText.transform.GetChild(1).gameObject.SetActive(true);
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
        dialogueBox.SetActive(false);

        if (bossBeaten == false)
        {
            StartBossBattle(battleActionsManager.zodiacToFight);
        }

        else
        {
            battleActionsManager.WinGame();
        }
    }

    IEnumerator WaitMove()
    {
        yield return new WaitForSeconds(1f);

        playerStarCanMove = true;
    }

    IEnumerator WaitStartGame(BattleType battleType)
    {
        if (battleType == BattleType.NORMAL)
        {
            yield return new WaitForSeconds(1.2f);

            foreach (GameObject animator in battleStartStarAnimations)
            {
                animator.GetComponent<Animator>().SetBool("Change", false);
                animator.transform.GetChild(1).GetComponent<Animator>().SetBool("Change", false);
            }

            battleCanvas.SetActive(true);

            overworldPlayer.GetComponent<SpriteRenderer>().sortingOrder = -10;
            overworldPlayer.transform.GetChild(1).GetComponent<AudioSource>().Stop();
            overworldPlayer.transform.GetChild(0).GetComponent<CircleCollider2D>().enabled = false;

            dungeonGameRoomsParent.SetActive(false);

            battleActionsManager.ManageSpawnBasicEnemies();

            blackScreen.GetComponent<Animator>().SetBool("Change", true);

            yield return new WaitForSeconds(0.25f);
            foreach (GameObject button in battleActionButtons)
            {
                button.GetComponent<ManageButtonSelection>().OnExitSelection();
            }
            battleActionButtons[0].GetComponent<ManageButtonSelection>().OnSelection();
            currentActionButtonIndex = 0;
            battleActionsManager.currentBattleState = GameStates.CHOOSING;
        }
        
        else if (battleType == BattleType.BOSS)
        {
            yield return new WaitForSeconds(1.2f);

            foreach (GameObject animator in battleStartStarAnimations)
            {
                animator.GetComponent<Animator>().SetBool("Change", false);
                animator.transform.GetChild(1).GetComponent<Animator>().SetBool("Change", false);
            }

            battleCanvas.SetActive(true);

            overworldPlayer.GetComponent<SpriteRenderer>().sortingOrder = -10;
            overworldPlayer.transform.GetChild(1).GetComponent<AudioSource>().Stop();
            overworldPlayer.transform.GetChild(0).GetComponent<CircleCollider2D>().enabled = false;

            dungeonGameRoomsParent.SetActive(false);

            battleActionsManager.ManageSpawnBoss();

            blackScreen.GetComponent<Animator>().SetBool("Change", true);

            yield return new WaitForSeconds(0.25f);
            foreach (GameObject button in battleActionButtons)
            {
                button.GetComponent<ManageButtonSelection>().OnExitSelection();
            }
            battleActionButtons[0].GetComponent<ManageButtonSelection>().OnSelection();
            currentActionButtonIndex = 0;
            battleActionsManager.currentBattleState = GameStates.CHOOSING;
        }
    }

    IEnumerator WaitFinishGame()
    {
        blackScreen.GetComponent<Animator>().SetBool("Change", false);
        battleActionsManager.currentBattleState = GameStates.NONE;

        yield return new WaitForSeconds(0.6f);

        foreach (GameObject animator in battleStartStarAnimations)
        {
            animator.GetComponent<Animator>().SetBool("ChangeBack", true);
            animator.transform.GetChild(1).GetComponent<Animator>().SetBool("ChangeBack", true);
        }

        yield return new WaitForSeconds(0.25f);
        ClearGame();
        battleCanvas.SetActive(false);

        overworldPlayer.GetComponent<SpriteRenderer>().sortingOrder = -3;
        overworldPlayer.transform.GetChild(1).GetComponent<AudioSource>().Play();
        dungeonGameRoomsParent.SetActive(true);

        yield return new WaitForSeconds(1.2f);
        foreach (GameObject animator in battleStartStarAnimations)
        {
            animator.GetComponent<Animator>().SetBool("ChangeBack", false);
            animator.transform.GetChild(1).GetComponent<Animator>().SetBool("ChangeBack", false);
        }

        dungeonMinimapRoomsParent.SetActive(true);
        overworldPlayer.transform.GetChild(0).GetComponent<CircleCollider2D>().enabled = true;
    }

    IEnumerator WaitFinishBossGame()
    {
        blackScreen.GetComponent<Animator>().SetBool("Change", false);
        battleActionsManager.currentBattleState = GameStates.NONE;

        yield return new WaitForSeconds(0.6f);

        foreach (GameObject animator in battleStartStarAnimations)
        {
            animator.GetComponent<Animator>().SetBool("ChangeBack", true);
            animator.transform.GetChild(1).GetComponent<Animator>().SetBool("ChangeBack", true);
        }

        yield return new WaitForSeconds(0.25f);
        ClearGame();
        battleCanvas.SetActive(false);

        overworldPlayer.GetComponent<SpriteRenderer>().sortingOrder = -3;
        overworldPlayer.transform.GetChild(1).GetComponent<AudioSource>().Play();
        dungeonGameRoomsParent.SetActive(true);

        yield return new WaitForSeconds(1.2f);
        foreach (GameObject animator in battleStartStarAnimations)
        {
            animator.GetComponent<Animator>().SetBool("ChangeBack", false);
            animator.transform.GetChild(1).GetComponent<Animator>().SetBool("ChangeBack", false);
        }

        dungeonMinimapRoomsParent.SetActive(true);
        overworldPlayer.transform.GetChild(0).GetComponent<CircleCollider2D>().enabled = true;

        bossBeaten = true;
        StartDialogue(battleActionsManager.zodiacToFight, characterSpeaker);
    }
}