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

    private bool didUseItemFail = false;
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
    public List<GameObject> textsForItemsToBeUsed;

    [SerializeField] private List<GameObject> battleStartStarAnimations;
    [SerializeField] private GameObject blackScreen;
    [Space(5)]

    [Header("THINGS TO SHOW/HIDE")]
    public GameObject overworldPlayer;
    public GameObject dungeonMinimapRoomsParent;
    [SerializeField] private GameObject dungeonGameRoomsParent;
    [SerializeField] private GameObject menuButton;
    [Space(5)]

    [Header("AUDIO VARIABLES")]
    [SerializeField] private AudioSource globalAudioSource;
    [SerializeField] private AudioSource battleSoundsAudioSource;
    [SerializeField] private AudioClip selectOptionSound;
    [SerializeField] private AudioClip attackEnemyAudio;
    [SerializeField] private AudioClip useItemAudio;

    void Start()
    {
        battleActionsManager = GetComponent<BattleActionsManager>();

        // Uncomment for the trial scene only
        //StartBossBattle("CAPRICORN");
    }

    void Update()
    {
        if (!battleCanvas.activeSelf || battleActionsManager.currentBattleState == BattleStates.NONE)
        {
            return;
        }

        if (Input.GetKeyUp(KeyCode.Z))
        {
            selectionKeyPressed = false;
        }

        if (battleActionsManager.currentBattleState == BattleStates.CHOOSING)
        {
            ManageBattleActionSelection();

            if (currentActionButtonIndex + 1 == (int)BattleActions.ATTACK && Input.GetKeyDown(KeyCode.Z))
            {
                ShowBattleEnemyNames();

                battleActionsManager.lastBattleState = battleActionsManager.currentBattleState;
                battleActionsManager.currentBattleState = BattleStates.ATTACKING;

                selectionKeyPressed = true;
            }
            else if (currentActionButtonIndex + 1 == (int)BattleActions.TALK && Input.GetKeyDown(KeyCode.Z))
            {
                ShowBattleEnemyNames();

                battleActionsManager.lastBattleState = battleActionsManager.currentBattleState;
                battleActionsManager.currentBattleState = BattleStates.TALKING;

                selectionKeyPressed = true;
            }
            else if (currentActionButtonIndex + 1 == (int)BattleActions.USE_ITEM && Input.GetKeyDown(KeyCode.Z))
            {
                battleSoundsAudioSource.clip = selectOptionSound;
                battleSoundsAudioSource.Play();

                if (GameMaster.inventory.Count != 0)
                {
                    ShowInventoryItemNames();

                    battleActionsManager.battleDialogueText.SetActive(false);

                    battleActionsManager.lastBattleState = battleActionsManager.currentBattleState;
                    battleActionsManager.currentBattleState = BattleStates.USING_ITEM;

                    didUseItemFail = false;
                }
                else
                {
                    battleActionsManager.battleDialogueText.GetComponent<Text>().text = "    YOU HAVE NO ITEMS RIGHT NOW";

                    battleActionsManager.lastBattleState = BattleStates.USING_ITEM;
                    battleActionsManager.currentBattleState = BattleStates.WAITING;

                    didUseItemFail = true;
                }

                selectionKeyPressed = true;
            }
            // Select RUN action
            else if (currentActionButtonIndex + 1 == (int)BattleActions.RUN && Input.GetKeyDown(KeyCode.Z))
            {
                battleSoundsAudioSource.clip = selectOptionSound;
                battleSoundsAudioSource.Play();

                battleActionsManager.lastBattleState = battleActionsManager.currentBattleState;
                battleActionsManager.currentBattleState = BattleStates.RUNNING;

                battleActionsManager.RunAction();

                selectionKeyPressed = true;
            }
        }
        else if (battleActionsManager.currentBattleState == BattleStates.ATTACKING ||
                 battleActionsManager.currentBattleState == BattleStates.TALKING)
        {
            ManageEnemySelection();

            if (battleActionsManager.currentBattleState == BattleStates.ATTACKING &&
                Input.GetKeyDown(KeyCode.Z) && !selectionKeyPressed)
            {
                battleSoundsAudioSource.clip = attackEnemyAudio;
                battleSoundsAudioSource.Play();

                battleActionsManager.AttackAction(textsForEnemiesInBattle, currentHoveredTextIndex);

                selectionKeyPressed = true;
            }
            else if (battleActionsManager.currentBattleState == BattleStates.TALKING &&
                     Input.GetKeyDown(KeyCode.Z) && !selectionKeyPressed)
            {
                battleSoundsAudioSource.clip = selectOptionSound;
                battleSoundsAudioSource.Play();

                battleActionsManager.TalkAction(textsForEnemiesInBattle);

                selectionKeyPressed = true;
            }

            if (Input.GetKeyDown(KeyCode.X))
            {
                CancelActionSelection(battleActionsManager.currentBattleState);
            }
        }
        else if (battleActionsManager.currentBattleState == BattleStates.USING_ITEM)
        {
            ManageItemSelection();

            if (Input.GetKeyDown(KeyCode.Z) && !selectionKeyPressed)
            {
                battleSoundsAudioSource.clip = useItemAudio;
                battleSoundsAudioSource.Play();

                battleActionsManager.UseItemAction(textsForItemsToBeUsed, currentHoveredTextIndex);

                selectionKeyPressed = true;
            }

            if (Input.GetKeyDown(KeyCode.X))
            {
                CancelActionSelection(battleActionsManager.currentBattleState);
            }
        }
        else if (battleActionsManager.currentBattleState == BattleStates.DEFENDING)
        {
            if (!selectionKeyPressed && Input.GetKeyDown(KeyCode.Z) && !playerStarCanMove)
            {
                battleActionsManager.TriggerEnemyAttack();
                StartCoroutine(nameof(SetStarReadyToMove));

                selectionKeyPressed = true;
            }
            else if (playerStarCanMove)
            {
                ManagePlayerStarMovement();
            }
        }
        else if (battleActionsManager.currentBattleState == BattleStates.VICTORY)
        {
            if (!selectionKeyPressed && Input.GetKeyDown(KeyCode.Z))
            {
                battleActionsManager.ManageBattleVictory();
                selectionKeyPressed = true;
            }
        }
        else if (battleActionsManager.currentBattleState == BattleStates.DEFEAT)
        {
            bool isZodiacFight = battleActionsManager.zodiacToFight != "";

            if (!selectionKeyPressed && Input.GetKeyDown(KeyCode.Z) && !isZodiacFight)
            {
                StartCoroutine(nameof(ManageFinishBattle), BattleType.NORMAL);
                selectionKeyPressed = true;
            }
            else if (!selectionKeyPressed && Input.GetKeyDown(KeyCode.Z) && isZodiacFight)
            {
                StartCoroutine(nameof(ManageFinishBattle), BattleType.BOSS);
                selectionKeyPressed = true;
            }
        }
        
        if (battleActionsManager.currentBattleState == BattleStates.WAITING)
        {
            if (battleActionsManager.lastBattleState == BattleStates.RUNNING)
            {
                bool isZodiacFight = battleActionsManager.zodiacToFight != "";
                bool canPlayerEscape = GameMaster.playerSpeed >= 10 && !isZodiacFight;

                if (canPlayerEscape && Input.GetKeyDown(KeyCode.Z) && !selectionKeyPressed)
                {
                    EscapeFromTheFight();
                    selectionKeyPressed = true;
                }
                else if (!canPlayerEscape && Input.GetKeyDown(KeyCode.X))
                {
                    GoBackToBattleAfterTryingToEscape();
                }
            }
            else if (battleActionsManager.lastBattleState == BattleStates.USING_ITEM && didUseItemFail &&
                     Input.GetKeyDown(KeyCode.X))
            {
                GoBackToBattleAfterTryingToUseItem();
            }
            else if (battleActionsManager.lastBattleState == BattleStates.TALKING &&
                     Input.GetKeyDown(KeyCode.X))
            {
                GoBackToEnemySelection();
            }
            else if (battleActionsManager.lastBattleState == BattleStates.ATTACKING &&
                     Input.GetKeyDown(KeyCode.Z))
            {
                battleActionsManager.lastBattleState = battleActionsManager.currentBattleState;
                battleActionsManager.currentBattleState = BattleStates.DEFENDING;
            }
            else if (battleActionsManager.lastBattleState == BattleStates.VICTORY &&
                     Input.GetKeyDown(KeyCode.Z))
            {
                bool isZodiacFight = battleActionsManager.zodiacToFight != "";

                if (!isZodiacFight && !selectionKeyPressed)
                {
                    StartCoroutine(nameof(ManageFinishBattle), BattleType.NORMAL);
                    selectionKeyPressed = true;

                    battleActionsManager.lastBattleState = battleActionsManager.currentBattleState;
                    battleActionsManager.currentBattleState = BattleStates.NONE;
                }
                else if (isZodiacFight && !selectionKeyPressed)
                {
                    StartCoroutine(nameof(ManageFinishBattle), BattleType.BOSS);
                    selectionKeyPressed = true;

                    battleActionsManager.lastBattleState = battleActionsManager.currentBattleState;
                    battleActionsManager.currentBattleState = BattleStates.NONE;
                }
            }
        }
    }
    
    // Start and end battle functions
    public void StartBattle()
    {
        battleActionsManager.SetUpBattle();

        overworldPlayer.GetComponent<PlayerAnimationDirection>().SetDirection(new Vector2(0, 0));
        overworldPlayer.GetComponent<PlayerMovement>().canMove = false;
        dungeonMinimapRoomsParent.SetActive(false);
        menuButton.SetActive(false);

        foreach (GameObject animator in battleStartStarAnimations)
        {
            animator.GetComponent<Animator>().SetBool("Change", true);
            animator.transform.GetChild(1).GetComponent<Animator>().SetBool("Change", true);
        }

        StartCoroutine(nameof(ManageStartBattle), BattleType.NORMAL);
    }

    public void StartBossBattle(string zodiac)
    {
        battleActionsManager.SetUpBossBattle(zodiac);

        overworldPlayer.GetComponent<PlayerMovement>().StopPlayer();
        dungeonMinimapRoomsParent.SetActive(false);
        menuButton.SetActive(false);

        foreach (GameObject animator in battleStartStarAnimations)
        {
            animator.GetComponent<Animator>().SetBool("Change", true);
            animator.transform.GetChild(1).GetComponent<Animator>().SetBool("Change", true);
        }

        StartCoroutine(nameof(ManageStartBattle), BattleType.BOSS);
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

        foreach (GameObject text in textsForItemsToBeUsed)
        {
            text.GetComponent<Text>().text = "";
            text.transform.GetChild(1).GetComponent<Text>().text = "";
        }
    }

    void ManageBattleActionSelection()
    {
        float leftRightInput = Input.GetAxis("Horizontal");

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

    void ShowBattleEnemyNames()
    {
        battleActionsManager.battleDialogueText.SetActive(false);
        battleSoundsAudioSource.clip = selectOptionSound;
        battleSoundsAudioSource.Play();

        for (int i = 0; i < battleActionsManager.enemiesSpawned.Count; i++)
        {
            GameObject currentEnemyText = textsForEnemiesInBattle[i];
            string enemyName = battleActionsManager.enemiesSpawned[i].GetComponent<EnemyCard>().CardName;

            currentEnemyText.SetActive(true);
            currentEnemyText.GetComponent<Text>().text = enemyName;
            currentEnemyText.transform.GetChild(1).GetComponent<Text>().text = "    " + enemyName;

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

    void ShowInventoryItemNames()
    {
        for (int i = 0; i < GameMaster.inventory.Count; i++)
        {
            GameObject currentItemText = textsForItemsToBeUsed[i];
            string itemName = GameMaster.inventory[i].ObjectName;
            int itemLevel = GameMaster.inventory[i].Level;

            currentItemText.SetActive(true);
            currentItemText.GetComponent<Text>().text = itemName + " LVL." + itemLevel;
            currentItemText.transform.GetChild(1).GetComponent<Text>().text = "    " + itemName + " LVL." + itemLevel;

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

        SetFirstElementSelectedByDefault(textsForItemsToBeUsed);
    }

    void SetFirstElementSelectedByDefault(List<GameObject> listOfTexts)
    {
        currentHoveredTextIndex = 0;
        listOfTexts[currentHoveredTextIndex].GetComponent<Text>().enabled = false;
        listOfTexts[currentHoveredTextIndex].transform.GetChild(0).gameObject.SetActive(true);
        listOfTexts[currentHoveredTextIndex].transform.GetChild(1).gameObject.SetActive(true);
    }

    void ManageEnemySelection()
    {
        float upDownInput = Input.GetAxis("Vertical");

        if (upDownInput != 0.0f && !upDownKeyPressed)
        {
            GameObject currentSelectedEnemyText = textsForEnemiesInBattle[currentHoveredTextIndex];
            upDownKeyPressed = true;

            if (upDownInput > 0.0f && currentHoveredTextIndex == 0)
            {
                currentHoveredTextIndex = battleActionsManager.enemiesSpawned.Count - 1;
            }
            else if (upDownInput > 0.0f)
            {
                currentHoveredTextIndex--;
            }
            else if (upDownInput < 0.0f && currentHoveredTextIndex == battleActionsManager.enemiesSpawned.Count - 1)
            {
                currentHoveredTextIndex = 0;
            }
            else if (upDownInput < 0.0f)
            {
                currentHoveredTextIndex++;
            }

            GameObject nextSelectedEnemyText = textsForEnemiesInBattle[currentHoveredTextIndex];

            // TODO: This should be a function, like DeselectText();
            currentSelectedEnemyText.GetComponent<Text>().enabled = true;
            currentSelectedEnemyText.transform.GetChild(0).gameObject.SetActive(false);
            currentSelectedEnemyText.transform.GetChild(1).gameObject.SetActive(false);

            // TODO: And this too, like SelectText();
            nextSelectedEnemyText.GetComponent<Text>().enabled = false;
            nextSelectedEnemyText.transform.GetChild(0).gameObject.SetActive(true);
            nextSelectedEnemyText.transform.GetChild(1).gameObject.SetActive(true);

        }
        else if (upDownInput == 0.0f && upDownKeyPressed)
        {
            upDownKeyPressed = false;
        }
    }

    void ManageItemSelection()
    {
        float leftRightInput = Input.GetAxis("Horizontal");
        float upDownInput = Input.GetAxis("Vertical");

        GameObject currentSelectedItemText = textsForItemsToBeUsed[currentHoveredTextIndex];

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

        GameObject nextSelectedItemText = textsForItemsToBeUsed[currentHoveredTextIndex];

        currentSelectedItemText.GetComponent<Text>().enabled = true;
        currentSelectedItemText.transform.GetChild(0).gameObject.SetActive(false);
        currentSelectedItemText.transform.GetChild(1).gameObject.SetActive(false);

        nextSelectedItemText.GetComponent<Text>().enabled = false;
        nextSelectedItemText.transform.GetChild(0).gameObject.SetActive(true);
        nextSelectedItemText.transform.GetChild(1).gameObject.SetActive(true);
    }

    void ManagePlayerStarMovement()
    {
        Rigidbody2D rb = playerStarObject.GetComponent<Rigidbody2D>();
        Vector2 currentPos = rb.position;

        float horInput = Input.GetAxis("Horizontal");
        float vertInput = Input.GetAxis("Vertical");

        Vector2 inputVect = new Vector2(horInput, vertInput);
        inputVect = Vector2.ClampMagnitude(inputVect, 1);

        Vector2 movement = inputVect * 3.0f;
        Vector2 newPos = currentPos + movement * Time.fixedDeltaTime;

        rb.MovePosition(newPos);
    }

    void EscapeFromTheFight()
    {
        battleActionsManager.lastBattleState = battleActionsManager.currentBattleState;
        battleActionsManager.currentBattleState = BattleStates.NONE;

        StartCoroutine(nameof(ManageFinishBattle), BattleType.NORMAL);

        battleSoundsAudioSource.clip = selectOptionSound;
        battleSoundsAudioSource.Play();
    }

    void GoBackToBattleAfterTryingToEscape()
    {
        battleActionsManager.currentBattleState = BattleStates.CHOOSING;
        battleActionsManager.lastBattleState = BattleStates.WAITING;

        battleActionsManager.battleDialogueText.GetComponent<Text>().text = battleActionsManager.baseTextToDisplay;

        battleSoundsAudioSource.clip = selectOptionSound;
        battleSoundsAudioSource.Play();
    }

    void GoBackToBattleAfterTryingToUseItem()
    {
        battleActionsManager.battleDialogueText.GetComponent<Text>().text = battleActionsManager.baseTextToDisplay;

        battleActionsManager.currentBattleState = BattleStates.CHOOSING;
        battleActionsManager.lastBattleState = BattleStates.WAITING;

        battleSoundsAudioSource.clip = selectOptionSound;
        battleSoundsAudioSource.Play();

        didUseItemFail = false;
    }

    void GoBackToEnemySelection()
    {
        foreach (GameObject text in textsForEnemiesInBattle)
        {
            text.SetActive(true);
        }

        battleActionsManager.battleDialogueText.SetActive(false);

        battleActionsManager.lastBattleState = battleActionsManager.currentBattleState;
        battleActionsManager.currentBattleState = BattleStates.TALKING;

        battleSoundsAudioSource.clip = selectOptionSound;
        battleSoundsAudioSource.Play();
    }

    void CancelActionSelection(BattleStates actionToCancel)
    {
        List<GameObject> textsToShow = new List<GameObject>();
        if (actionToCancel == BattleStates.ATTACKING || actionToCancel == BattleStates.TALKING)
        {
            textsToShow = textsForEnemiesInBattle;
        }
        else if (actionToCancel == BattleStates.USING_ITEM)
        {
            textsToShow = textsForItemsToBeUsed;
        }

        battleActionsManager.lastBattleState = battleActionsManager.currentBattleState;
        battleActionsManager.currentBattleState = BattleStates.CHOOSING;

        battleActionsManager.battleDialogueText.GetComponent<Text>().text = battleActionsManager.baseTextToDisplay;

        textsToShow[currentHoveredTextIndex].GetComponent<Text>().enabled = true;
        textsToShow[currentHoveredTextIndex].transform.GetChild(0).gameObject.SetActive(false);
        textsToShow[currentHoveredTextIndex].transform.GetChild(1).gameObject.SetActive(false);

        foreach (GameObject text in textsToShow)
        {
            text.SetActive(false);
        }

        battleActionsManager.battleDialogueText.SetActive(true);

        battleSoundsAudioSource.clip = selectOptionSound;
        battleSoundsAudioSource.Play();
    }

    IEnumerator SetStarReadyToMove()
    {
        yield return new WaitForSeconds(1f);

        playerStarCanMove = true;
    }

    IEnumerator ManageStartBattle(BattleType battleType)
    {
        yield return new WaitForSeconds(1.2f);

        foreach (GameObject animator in battleStartStarAnimations)
        {
            animator.GetComponent<Animator>().SetBool("Change", false);
            animator.transform.GetChild(1).GetComponent<Animator>().SetBool("Change", false);
        }

        battleCanvas.SetActive(true);
        globalAudioSource.Stop();

        overworldPlayer.GetComponent<SpriteRenderer>().sortingOrder = -10;
        overworldPlayer.GetComponent<AudioSource>().Stop();
        overworldPlayer.GetComponent<CircleCollider2D>().enabled = false;

        dungeonGameRoomsParent.SetActive(false);

        if (battleType == BattleType.NORMAL)
        {
            battleActionsManager.ManageSpawnBasicEnemies();
        }
        else if (battleType == BattleType.BOSS)
        {
            battleActionsManager.ManageSpawnBoss();
        }

        blackScreen.GetComponent<Animator>().SetBool("Change", true);

        yield return new WaitForSeconds(0.25f);
        foreach (GameObject button in battleActionButtons)
        {
            button.GetComponent<ManageButtonSelection>().OnExitSelection();
        }
        battleActionButtons[0].GetComponent<ManageButtonSelection>().OnSelection();
        currentActionButtonIndex = 0;
        battleActionsManager.currentBattleState = BattleStates.CHOOSING;
    }

    IEnumerator ManageFinishBattle(BattleType battleType)
    {
        blackScreen.GetComponent<Animator>().SetBool("Change", false);
        battleActionsManager.currentBattleState = BattleStates.NONE;

        yield return new WaitForSeconds(0.6f);

        foreach (GameObject animator in battleStartStarAnimations)
        {
            animator.GetComponent<Animator>().SetBool("ChangeBack", true);
            animator.transform.GetChild(1).GetComponent<Animator>().SetBool("ChangeBack", true);
        }

        yield return new WaitForSeconds(0.25f);
        ClearGame();

        battleCanvas.SetActive(false);
        globalAudioSource.Play();

        overworldPlayer.GetComponent<SpriteRenderer>().sortingOrder = -3;
        dungeonGameRoomsParent.SetActive(true);

        yield return new WaitForSeconds(1.2f);
        foreach (GameObject animator in battleStartStarAnimations)
        {
            animator.GetComponent<Animator>().SetBool("ChangeBack", false);
            animator.transform.GetChild(1).GetComponent<Animator>().SetBool("ChangeBack", false);
        }

        dungeonMinimapRoomsParent.SetActive(true);
        menuButton.SetActive(true);

        overworldPlayer.GetComponent<CircleCollider2D>().enabled = true;
        overworldPlayer.GetComponent<AudioSource>().Play();
        overworldPlayer.GetComponent<PlayerMovement>().canMove = true;

        foreach (GameObject card in battleActionsManager.enemiesSpawned)
        {
            Destroy(card);
        }

        if (battleType == BattleType.BOSS)
        {
            bossBeaten = true;
        }
    }
}