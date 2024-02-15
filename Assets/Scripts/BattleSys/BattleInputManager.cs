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

public class BattleInputManager : MonoBehaviour
{
    private BattleActionsManager battleActionsManager;

    private bool pressedLeftRightKey;
    private bool pressedUpDownKey;
    private bool pressedSelectionKey;

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
    [SerializeField] private AudioClip selectEnemyAudio;
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
        if (Input.GetKeyUp(KeyCode.Z))
        {
            pressedSelectionKey = false;
        }

        if (battleCanvas.activeSelf)
        {
            // The battle is paused
            if (battleActionsManager.currentBattleState == GameStates.NONE)
            {
                return;
            }

            // The battle has ended
            if (battleActionsManager.currentBattleState == GameStates.DEFEAT)
            {
                if (!pressedSelectionKey && Input.GetKeyDown(KeyCode.Z) && battleActionsManager.zodiacToFight == "")
                {
                    StartCoroutine(nameof(WaitFinishGame));
                }

                else if (!pressedSelectionKey && Input.GetKeyDown(KeyCode.Z) && battleActionsManager.zodiacToFight != "")
                {
                    StartCoroutine(nameof(WaitFinishBossGame));
                }

                return;
            }

            // The enemy is attacking and the player avoids the attacks
            if (battleActionsManager.currentBattleState == GameStates.DEFENDING)
            {
                if (!pressedSelectionKey && Input.GetKeyDown(KeyCode.Z) && !playerStarCanMove)
                {
                    battleActionsManager.StartEnemyAttack();
                    StartCoroutine(nameof(WaitMove));

                    pressedSelectionKey = true;
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

            if (battleActionsManager.currentBattleState == GameStates.VICTORY)
            {
                if (!pressedSelectionKey && Input.GetKeyDown(KeyCode.Z))
                {
                    battleActionsManager.WinBattle();
                    pressedSelectionKey = true;
                }

                return;
            }

            // Go back in the menu
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

                    globalAudioSource.clip = selectEnemyAudio;
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

                    globalAudioSource.clip = selectEnemyAudio;
                    globalAudioSource.Play();
                }
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

                    globalAudioSource.clip = selectEnemyAudio;
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

                    pressedSelectionKey = true;

                    globalAudioSource.clip = selectEnemyAudio;
                    globalAudioSource.Play();
                }

                else if (Input.GetKeyDown(KeyCode.X) && battleActionsManager.lastBattleState == GameStates.USING_ITEM)
                {
                    battleActionsManager.battleDialogueText.GetComponent<Text>().text = battleActionsManager.textToDisplay;

                    battleActionsManager.currentBattleState = GameStates.CHOOSING;
                    battleActionsManager.lastBattleState = GameStates.WAITING;

                    globalAudioSource.clip = selectEnemyAudio;
                    globalAudioSource.Play();
                }

                else if (battleActionsManager.lastBattleState == GameStates.RUNNING)
                {
                    if (GameMaster.playerSpeed >= 5 && Input.GetKeyDown(KeyCode.Z) && !pressedSelectionKey && battleActionsManager.zodiacToFight == "")
                    {
                        battleActionsManager.currentBattleState = GameStates.NONE;
                        battleActionsManager.lastBattleState = GameStates.WAITING;

                        StartCoroutine(nameof(WaitFinishGame));

                        pressedSelectionKey = true;

                        globalAudioSource.clip = selectEnemyAudio;
                        globalAudioSource.Play();
                    }

                    else if ((GameMaster.playerSpeed < 5 || battleActionsManager.zodiacToFight != "") && Input.GetKeyDown(KeyCode.X))
                    {
                        battleActionsManager.currentBattleState = GameStates.CHOOSING;
                        battleActionsManager.lastBattleState = GameStates.WAITING;

                        battleActionsManager.battleDialogueText.GetComponent<Text>().text = battleActionsManager.textToDisplay;

                        globalAudioSource.clip = selectEnemyAudio;
                        globalAudioSource.Play();
                    }
                }

                else
                {
                    return;
                }
            }

            // Change choosing button
            float leftRight = Input.GetAxis("Horizontal");

            if (leftRight < 0 && !pressedLeftRightKey && battleActionsManager.currentBattleState == GameStates.CHOOSING)
            {
                pressedLeftRightKey = true;
                if (currentActionButtonIndex == 0)
                {
                    battleActionButtons[currentActionButtonIndex].GetComponent<ManageButtonSelection>().OnExitSelection();
                    currentActionButtonIndex = battleActionButtons.Count - 1;
                    battleActionButtons[currentActionButtonIndex].GetComponent<ManageButtonSelection>().OnSelection();
                }

                else
                {
                    battleActionButtons[currentActionButtonIndex].GetComponent<ManageButtonSelection>().OnExitSelection();
                    currentActionButtonIndex--;
                    battleActionButtons[currentActionButtonIndex].GetComponent<ManageButtonSelection>().OnSelection();
                }
            }

            else if (leftRight > 0 && !pressedLeftRightKey && battleActionsManager.currentBattleState == GameStates.CHOOSING)
            {
                pressedLeftRightKey = true;
                if (currentActionButtonIndex == battleActionButtons.Count - 1)
                {
                    battleActionButtons[currentActionButtonIndex].GetComponent<ManageButtonSelection>().OnExitSelection();
                    currentActionButtonIndex = 0;
                    battleActionButtons[currentActionButtonIndex].GetComponent<ManageButtonSelection>().OnSelection();
                }

                else
                {
                    battleActionButtons[currentActionButtonIndex].GetComponent<ManageButtonSelection>().OnExitSelection();
                    currentActionButtonIndex++;
                    battleActionButtons[currentActionButtonIndex].GetComponent<ManageButtonSelection>().OnSelection();
                }
            }

            else if (leftRight == 0 && pressedLeftRightKey && battleActionsManager.currentBattleState == GameStates.CHOOSING)
            {
                pressedLeftRightKey = false;
            }

            // Attack
            if (currentActionButtonIndex == 0 && Input.GetKeyDown(KeyCode.Z) && (battleActionsManager.currentBattleState != GameStates.ATTACKING && battleActionsManager.currentBattleState != GameStates.TALKING && battleActionsManager.currentBattleState != GameStates.USING_ITEM && battleActionsManager.currentBattleState != GameStates.RUNNING))
            {
                battleActionsManager.currentBattleState = GameStates.ATTACKING;
                battleActionsManager.battleDialogueText.SetActive(false);

                globalAudioSource.clip = selectEnemyAudio;
                globalAudioSource.Play();

                for (int i = 0; i < battleActionsManager.enemiesSpawned.Count; i++)
                {
                    textsForEnemiesInBattle[i].SetActive(true);
                    textsForEnemiesInBattle[i].GetComponent<Text>().text = battleActionsManager.enemiesSpawned[i].GetComponent<EnemyCard>().CardName;
                    textsForEnemiesInBattle[i].transform.GetChild(1).GetComponent<Text>().text = "    " + battleActionsManager.enemiesSpawned[i].GetComponent<EnemyCard>().CardName;

                    // TODO: This variable is never used, check if it is really necessary
                    // battleManager.enemiesSpawned[i].GetComponent<EnemyCard>().nameText = enemyTexts[i];

                    if (battleActionsManager.zodiacToFight != "")
                    {
                        textsForEnemiesInBattle[i].GetComponent<Text>().color = new Color(0.925f, 0.835f, 0.0f);
                        textsForEnemiesInBattle[i].transform.GetChild(1).GetComponent<Text>().color = new Color(0.925f, 0.835f, 0.0f);
                    }

                    else if (battleActionsManager.enemiesSpawned[i].name[0].ToString() == "D" || battleActionsManager.enemiesSpawned[i].name[0].ToString() == "H" || battleActionsManager.enemiesSpawned[i].name[0].ToString() == "P")
                    {
                        textsForEnemiesInBattle[i].GetComponent<Text>().color = new Color(1.0f, 0.0f, 0.31f);
                        textsForEnemiesInBattle[i].transform.GetChild(1).GetComponent<Text>().color = new Color(1.0f, 0.0f, 0.31f);
                    }

                    else if (battleActionsManager.enemiesSpawned[i].name[0].ToString() == "S" || battleActionsManager.enemiesSpawned[i].name[0].ToString() == "C" || battleActionsManager.enemiesSpawned[i].name[0].ToString() == "B")
                    {
                        textsForEnemiesInBattle[i].GetComponent<Text>().color = new Color(0.19f, 0.68f, 1.0f);
                        textsForEnemiesInBattle[i].transform.GetChild(1).GetComponent<Text>().color = new Color(0.19f, 0.68f, 1.0f);
                    }
                }

                currentHoveredTextIndex = 0;
                textsForEnemiesInBattle[currentHoveredTextIndex].GetComponent<Text>().enabled = false;
                textsForEnemiesInBattle[currentHoveredTextIndex].transform.GetChild(0).gameObject.SetActive(true);
                textsForEnemiesInBattle[currentHoveredTextIndex].transform.GetChild(1).gameObject.SetActive(true);

                pressedSelectionKey = true;
            }

            if (battleActionsManager.currentBattleState == GameStates.ATTACKING && Input.GetKeyDown(KeyCode.Z) && !pressedSelectionKey)
            {
                pressedSelectionKey = true;
                battleActionsManager.AttackAction(textsForEnemiesInBattle, currentHoveredTextIndex);

                globalAudioSource.clip = attackEnemyAudio;
                globalAudioSource.Play();
            }

            // Listen and talk
            else if (currentActionButtonIndex == 1 && Input.GetKeyDown(KeyCode.Z) && (battleActionsManager.currentBattleState != GameStates.ATTACKING && battleActionsManager.currentBattleState != GameStates.TALKING && battleActionsManager.currentBattleState != GameStates.USING_ITEM && battleActionsManager.currentBattleState != GameStates.RUNNING))
            {
                battleActionsManager.currentBattleState = GameStates.TALKING;
                battleActionsManager.battleDialogueText.SetActive(false);

                globalAudioSource.clip = selectEnemyAudio;
                globalAudioSource.Play();

                for (int i = 0; i < battleActionsManager.enemiesSpawned.Count; i++)
                {
                    textsForEnemiesInBattle[i].SetActive(true);
                    textsForEnemiesInBattle[i].GetComponent<Text>().text = battleActionsManager.enemiesSpawned[i].GetComponent<EnemyCard>().CardName;
                    textsForEnemiesInBattle[i].transform.GetChild(1).GetComponent<Text>().text = "    " + battleActionsManager.enemiesSpawned[i].GetComponent<EnemyCard>().CardName;

                    // TODO: This variable is never used, check if it is really necessary
                    // battleManager.enemiesSpawned[i].GetComponent<EnemyCard>().nameText = enemyTexts[i];

                    if (battleActionsManager.zodiacToFight != "")
                    {
                        textsForEnemiesInBattle[i].GetComponent<Text>().color = new Color(0.925f, 0.835f, 0.0f);
                        textsForEnemiesInBattle[i].transform.GetChild(1).GetComponent<Text>().color = new Color(0.925f, 0.835f, 0.0f);
                    }

                    else if (battleActionsManager.enemiesSpawned[i].name[0].ToString() == "D" || battleActionsManager.enemiesSpawned[i].name[0].ToString() == "H" || battleActionsManager.enemiesSpawned[i].name[0].ToString() == "P")
                    {
                        textsForEnemiesInBattle[i].GetComponent<Text>().color = new Color(1.0f, 0.0f, 0.31f);
                        textsForEnemiesInBattle[i].transform.GetChild(1).GetComponent<Text>().color = new Color(1.0f, 0.0f, 0.31f);
                    }

                    else if (battleActionsManager.enemiesSpawned[i].name[0].ToString() == "S" || battleActionsManager.enemiesSpawned[i].name[0].ToString() == "C" || battleActionsManager.enemiesSpawned[i].name[0].ToString() == "B")
                    {
                        textsForEnemiesInBattle[i].GetComponent<Text>().color = new Color(0.19f, 0.68f, 1.0f);
                        textsForEnemiesInBattle[i].transform.GetChild(1).GetComponent<Text>().color = new Color(0.19f, 0.68f, 1.0f);
                    }
                }

                currentHoveredTextIndex = 0;
                textsForEnemiesInBattle[currentHoveredTextIndex].GetComponent<Text>().enabled = false;
                textsForEnemiesInBattle[currentHoveredTextIndex].transform.GetChild(0).gameObject.SetActive(true);
                textsForEnemiesInBattle[currentHoveredTextIndex].transform.GetChild(1).gameObject.SetActive(true);

                pressedSelectionKey = true;
            }

            if (battleActionsManager.currentBattleState == GameStates.TALKING && Input.GetKeyDown(KeyCode.Z) && !pressedSelectionKey)
            {
                pressedSelectionKey = true;
                battleActionsManager.TalkAction(textsForEnemiesInBattle);

                globalAudioSource.clip = selectEnemyAudio;
                globalAudioSource.Play();
            }

            // Open inventory
            else if (currentActionButtonIndex == 2 && Input.GetKeyDown(KeyCode.Z) && (battleActionsManager.currentBattleState != GameStates.ATTACKING && battleActionsManager.currentBattleState != GameStates.TALKING && battleActionsManager.currentBattleState != GameStates.USING_ITEM && battleActionsManager.currentBattleState != GameStates.RUNNING))
            {
                globalAudioSource.clip = selectEnemyAudio;
                globalAudioSource.Play();

                if (GameMaster.inventory.Count == 0)
                {
                    battleActionsManager.currentBattleState = GameStates.WAITING;
                    battleActionsManager.lastBattleState = GameStates.USING_ITEM;

                    battleActionsManager.battleDialogueText.GetComponent<Text>().text = "    YOU HAVE NO ITEMS RIGHT NOW";
                }

                else
                {
                    battleActionsManager.currentBattleState = GameStates.USING_ITEM;
                    battleActionsManager.battleDialogueText.SetActive(false);

                    for (int i = 0; i < GameMaster.inventory.Count; i++)
                    {
                        textForItemsToBeUsed[i].SetActive(true);
                        textForItemsToBeUsed[i].GetComponent<Text>().text = GameMaster.inventory[i].ObjectName + " LVL." + GameMaster.inventory[i].Level;
                        textForItemsToBeUsed[i].transform.GetChild(1).GetComponent<Text>().text = "    " + GameMaster.inventory[i].ObjectName + " LVL." + GameMaster.inventory[i].Level;

                        GameMaster.inventory[i].GameText = textForItemsToBeUsed[i];

                        if (GameMaster.inventory[i].Type == ObjectTypes.HEALTH)
                        {
                            textForItemsToBeUsed[i].GetComponent<Text>().color = Color.red;
                            textForItemsToBeUsed[i].transform.GetChild(1).GetComponent<Text>().color = Color.red;
                        }

                        else if (GameMaster.inventory[i].Type == ObjectTypes.ATTACK)
                        {
                            textForItemsToBeUsed[i].GetComponent<Text>().color = new Color(1.0f, 0.37f, 0.0f);
                            textForItemsToBeUsed[i].transform.GetChild(1).GetComponent<Text>().color = new Color(1.0f, 0.37f, 0.0f);
                        }

                        else if (GameMaster.inventory[i].Type == ObjectTypes.DEFENSE)
                        {
                            textForItemsToBeUsed[i].GetComponent<Text>().color = new Color(0.0f, 0.36f, 1.0f);
                            textForItemsToBeUsed[i].transform.GetChild(1).GetComponent<Text>().color = new Color(0.0f, 0.36f, 1.0f);
                        }

                        else
                        {
                            textForItemsToBeUsed[i].GetComponent<Text>().color = Color.green;
                            textForItemsToBeUsed[i].transform.GetChild(1).GetComponent<Text>().color = Color.green;
                        }
                    }

                    currentHoveredTextIndex = 0;
                    textForItemsToBeUsed[currentHoveredTextIndex].GetComponent<Text>().enabled = false;
                    textForItemsToBeUsed[currentHoveredTextIndex].transform.GetChild(0).gameObject.SetActive(true);
                    textForItemsToBeUsed[currentHoveredTextIndex].transform.GetChild(1).gameObject.SetActive(true);
                }

                pressedSelectionKey = true;
            }

            if (battleActionsManager.currentBattleState == GameStates.USING_ITEM && Input.GetKeyDown(KeyCode.Z) && !pressedSelectionKey)
            {
                pressedSelectionKey = true;
                battleActionsManager.UseItemAction(textForItemsToBeUsed, currentHoveredTextIndex);

                globalAudioSource.clip = useItemAudio;
                globalAudioSource.Play();
            }

            // Run from battle
            else if (currentActionButtonIndex == 3 && Input.GetKeyDown(KeyCode.Z) && (battleActionsManager.currentBattleState != GameStates.ATTACKING && battleActionsManager.currentBattleState != GameStates.TALKING && battleActionsManager.currentBattleState != GameStates.USING_ITEM && battleActionsManager.currentBattleState != GameStates.RUNNING && battleActionsManager.currentBattleState != GameStates.WAITING))
            {
                battleActionsManager.RunAction();
                pressedSelectionKey = true;

                globalAudioSource.clip = selectEnemyAudio;
                globalAudioSource.Play();
            }

            // Change selected enemy
            float topBottom = Input.GetAxis("Vertical");
            if (topBottom > 0 && !pressedUpDownKey && (battleActionsManager.currentBattleState == GameStates.ATTACKING || battleActionsManager.currentBattleState == GameStates.TALKING))
            {
                pressedUpDownKey = true;
                if (currentHoveredTextIndex == 0)
                {
                    textsForEnemiesInBattle[currentHoveredTextIndex].GetComponent<Text>().enabled = true;
                    textsForEnemiesInBattle[currentHoveredTextIndex].transform.GetChild(0).gameObject.SetActive(false);
                    textsForEnemiesInBattle[currentHoveredTextIndex].transform.GetChild(1).gameObject.SetActive(false);

                    currentHoveredTextIndex = battleActionsManager.enemiesSpawned.Count - 1;

                    textsForEnemiesInBattle[currentHoveredTextIndex].GetComponent<Text>().enabled = false;
                    textsForEnemiesInBattle[currentHoveredTextIndex].transform.GetChild(0).gameObject.SetActive(true);
                    textsForEnemiesInBattle[currentHoveredTextIndex].transform.GetChild(1).gameObject.SetActive(true);
                }

                else
                {
                    textsForEnemiesInBattle[currentHoveredTextIndex].GetComponent<Text>().enabled = true;
                    textsForEnemiesInBattle[currentHoveredTextIndex].transform.GetChild(0).gameObject.SetActive(false);
                    textsForEnemiesInBattle[currentHoveredTextIndex].transform.GetChild(1).gameObject.SetActive(false);

                    currentHoveredTextIndex--;

                    textsForEnemiesInBattle[currentHoveredTextIndex].GetComponent<Text>().enabled = false;
                    textsForEnemiesInBattle[currentHoveredTextIndex].transform.GetChild(0).gameObject.SetActive(true);
                    textsForEnemiesInBattle[currentHoveredTextIndex].transform.GetChild(1).gameObject.SetActive(true);
                }
            }

            else if (topBottom < 0 && !pressedUpDownKey && (battleActionsManager.currentBattleState == GameStates.ATTACKING || battleActionsManager.currentBattleState == GameStates.TALKING))
            {
                pressedUpDownKey = true;
                if (currentHoveredTextIndex == battleActionsManager.enemiesSpawned.Count - 1)
                {
                    textsForEnemiesInBattle[currentHoveredTextIndex].GetComponent<Text>().enabled = true;
                    textsForEnemiesInBattle[currentHoveredTextIndex].transform.GetChild(0).gameObject.SetActive(false);
                    textsForEnemiesInBattle[currentHoveredTextIndex].transform.GetChild(1).gameObject.SetActive(false);

                    currentHoveredTextIndex = 0;

                    textsForEnemiesInBattle[currentHoveredTextIndex].GetComponent<Text>().enabled = false;
                    textsForEnemiesInBattle[currentHoveredTextIndex].transform.GetChild(0).gameObject.SetActive(true);
                    textsForEnemiesInBattle[currentHoveredTextIndex].transform.GetChild(1).gameObject.SetActive(true);
                }

                else
                {
                    textsForEnemiesInBattle[currentHoveredTextIndex].GetComponent<Text>().enabled = true;
                    textsForEnemiesInBattle[currentHoveredTextIndex].transform.GetChild(0).gameObject.SetActive(false);
                    textsForEnemiesInBattle[currentHoveredTextIndex].transform.GetChild(1).gameObject.SetActive(false);

                    currentHoveredTextIndex++;

                    textsForEnemiesInBattle[currentHoveredTextIndex].GetComponent<Text>().enabled = false;
                    textsForEnemiesInBattle[currentHoveredTextIndex].transform.GetChild(0).gameObject.SetActive(true);
                    textsForEnemiesInBattle[currentHoveredTextIndex].transform.GetChild(1).gameObject.SetActive(true);
                }
            }

            else if (topBottom == 0 && pressedUpDownKey)
            {
                pressedUpDownKey = false;
            }

            // Change selected item
            if (topBottom > 0 && !pressedUpDownKey && battleActionsManager.currentBattleState == GameStates.USING_ITEM)
            {
                pressedUpDownKey = true;
                if (currentHoveredTextIndex == 0)
                {
                    // Nothing
                }

                else
                {
                    textForItemsToBeUsed[currentHoveredTextIndex].GetComponent<Text>().enabled = true;
                    textForItemsToBeUsed[currentHoveredTextIndex].transform.GetChild(0).gameObject.SetActive(false);
                    textForItemsToBeUsed[currentHoveredTextIndex].transform.GetChild(1).gameObject.SetActive(false);

                    currentHoveredTextIndex--;

                    textForItemsToBeUsed[currentHoveredTextIndex].GetComponent<Text>().enabled = false;
                    textForItemsToBeUsed[currentHoveredTextIndex].transform.GetChild(0).gameObject.SetActive(true);
                    textForItemsToBeUsed[currentHoveredTextIndex].transform.GetChild(1).gameObject.SetActive(true);
                }
            }

            else if (topBottom < 0 && !pressedUpDownKey && battleActionsManager.currentBattleState == GameStates.USING_ITEM)
            {
                pressedUpDownKey = true;
                if (currentHoveredTextIndex == GameMaster.inventory.Count - 1)
                {
                    // Nothing
                }

                else
                {
                    textForItemsToBeUsed[currentHoveredTextIndex].GetComponent<Text>().enabled = true;
                    textForItemsToBeUsed[currentHoveredTextIndex].transform.GetChild(0).gameObject.SetActive(false);
                    textForItemsToBeUsed[currentHoveredTextIndex].transform.GetChild(1).gameObject.SetActive(false);

                    currentHoveredTextIndex++;

                    textForItemsToBeUsed[currentHoveredTextIndex].GetComponent<Text>().enabled = false;
                    textForItemsToBeUsed[currentHoveredTextIndex].transform.GetChild(0).gameObject.SetActive(true);
                    textForItemsToBeUsed[currentHoveredTextIndex].transform.GetChild(1).gameObject.SetActive(true);
                }
            }

            else if (leftRight > 0 && !pressedLeftRightKey && battleActionsManager.currentBattleState == GameStates.USING_ITEM)
            {
                pressedLeftRightKey = true;
                if (currentHoveredTextIndex + 4 > GameMaster.inventory.Count - 1)
                {
                    // Nothing
                }

                else
                {
                    textForItemsToBeUsed[currentHoveredTextIndex].GetComponent<Text>().enabled = true;
                    textForItemsToBeUsed[currentHoveredTextIndex].transform.GetChild(0).gameObject.SetActive(false);
                    textForItemsToBeUsed[currentHoveredTextIndex].transform.GetChild(1).gameObject.SetActive(false);

                    currentHoveredTextIndex += 4;

                    textForItemsToBeUsed[currentHoveredTextIndex].GetComponent<Text>().enabled = false;
                    textForItemsToBeUsed[currentHoveredTextIndex].transform.GetChild(0).gameObject.SetActive(true);
                    textForItemsToBeUsed[currentHoveredTextIndex].transform.GetChild(1).gameObject.SetActive(true);
                }
            }

            else if (leftRight < 0 && !pressedLeftRightKey && battleActionsManager.currentBattleState == GameStates.USING_ITEM)
            {
                pressedLeftRightKey = true;
                if (currentHoveredTextIndex - 4 < 0)
                {
                    // Nothing
                }

                else
                {
                    textForItemsToBeUsed[currentHoveredTextIndex].GetComponent<Text>().enabled = true;
                    textForItemsToBeUsed[currentHoveredTextIndex].transform.GetChild(0).gameObject.SetActive(false);
                    textForItemsToBeUsed[currentHoveredTextIndex].transform.GetChild(1).gameObject.SetActive(false);

                    currentHoveredTextIndex -= 4;

                    textForItemsToBeUsed[currentHoveredTextIndex].GetComponent<Text>().enabled = false;
                    textForItemsToBeUsed[currentHoveredTextIndex].transform.GetChild(0).gameObject.SetActive(true);
                    textForItemsToBeUsed[currentHoveredTextIndex].transform.GetChild(1).gameObject.SetActive(true);
                }
            }

            else if (topBottom == 0 && pressedUpDownKey)
            {
                pressedUpDownKey = false;
            }

            else if (leftRight == 0 && pressedLeftRightKey)
            {
                pressedLeftRightKey = false;
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
        pressedSelectionKey = false;

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