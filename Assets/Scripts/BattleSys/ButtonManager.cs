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

public class ButtonManager : MonoBehaviour
{
    public List<GameObject> battleButtons;
    public BattleManager battleManager;

    private bool pressedHor = false;
    private bool pressedVer = false;
    [HideInInspector] public bool pressedZ = false;

    [HideInInspector] public int currentButtonIndex;
    [HideInInspector] public int currentTextIndex;

    public List<GameObject> enemyTexts;
    public List<GameObject> itemTexts;

    [HideInInspector] public bool playerCanMove = false;
    public GameObject playerStar;
    public GameObject playerStarBasePosition;

    public List<GameObject> starAnimators;
    public GameObject blackScreen;

    public GameObject animCanvas;
    public GameObject player;
    public GameObject realRooms;
    public GameObject miniMap;

    public GameObject battleCanvas;

    public AudioSource globalAudioSource;
    public AudioClip selectEnemy;
    public AudioClip attackEnemy;
    public AudioClip useItem;

    public DungeonMapManager templates;

    public GameObject dialogueBox;
    public GameObject menuButton;
    public Text dialogueText;
    public GameObject nextButton;

    public Image dialogueImage;
    public Text dialogueNameText;

    private DialogueBox thisCharacter;

    private bool bossBeaten = false;

    void Start()
    {
        /*
        foreach (GameObject animator in starAnimators)
        {
            animator.GetComponent<Animator>().SetBool("Change", true);
            animator.transform.GetChild(1).GetComponent<Animator>().SetBool("Change", true);
        }

        StartCoroutine(nameof(WaitStartGame), BattleType.NORMAL);
        */
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Z))
        {
            pressedZ = false;
        }

        if (battleCanvas.activeSelf)
        {
            // The battle is paused
            if (battleManager.currentBattleState == GameStates.NONE)
            {
                return;
            }

            // The battle has ended
            if (battleManager.currentBattleState == GameStates.DEFEAT)
            {
                if (!pressedZ && Input.GetKeyDown(KeyCode.Z) && battleManager.zodiacToFight == "")
                {
                    StartCoroutine(nameof(WaitFinishGame));
                }

                else if (!pressedZ && Input.GetKeyDown(KeyCode.Z) && battleManager.zodiacToFight != "")
                {
                    StartCoroutine(nameof(WaitFinishBossGame));
                }

                return;
            }

            // The enemy is attacking and the player avoids the attacks
            if (battleManager.currentBattleState == GameStates.DEFENDING)
            {
                if (!pressedZ && Input.GetKeyDown(KeyCode.Z) && !playerCanMove)
                {
                    battleManager.StartEnemyAttack();
                    StartCoroutine(nameof(WaitMove));

                    pressedZ = true;
                }

                if (playerCanMove)
                {
                    Rigidbody2D rb = playerStar.GetComponent<Rigidbody2D>();

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

            if (battleManager.currentBattleState == GameStates.VICTORY)
            {
                if (!pressedZ && Input.GetKeyDown(KeyCode.Z))
                {
                    battleManager.WinBattle();
                    pressedZ = true;
                }

                return;
            }

            // Go back in the menu
            if (Input.GetKeyDown(KeyCode.X))
            {
                if (battleManager.currentBattleState == GameStates.ATTACKING || battleManager.currentBattleState == GameStates.TALKING)
                {
                    battleManager.currentBattleState = GameStates.CHOOSING;
                    battleManager.battleDialogueText.GetComponent<Text>().text = battleManager.textToDisplay;

                    // Disable enemy texts
                    enemyTexts[currentTextIndex].GetComponent<Text>().enabled = true;
                    enemyTexts[currentTextIndex].transform.GetChild(0).gameObject.SetActive(false);
                    enemyTexts[currentTextIndex].transform.GetChild(1).gameObject.SetActive(false);

                    foreach (GameObject text in enemyTexts)
                    {
                        text.SetActive(false);
                    }

                    battleManager.battleDialogueText.SetActive(true);

                    globalAudioSource.clip = selectEnemy;
                    globalAudioSource.Play();
                }

                else if (battleManager.currentBattleState == GameStates.USING_ITEM)
                {
                    battleManager.currentBattleState = GameStates.CHOOSING;
                    battleManager.battleDialogueText.GetComponent<Text>().text = battleManager.textToDisplay;

                    // Disable item texts
                    itemTexts[currentTextIndex].GetComponent<Text>().enabled = true;
                    itemTexts[currentTextIndex].transform.GetChild(0).gameObject.SetActive(false);
                    itemTexts[currentTextIndex].transform.GetChild(1).gameObject.SetActive(false);

                    foreach (GameObject text in itemTexts)
                    {
                        text.SetActive(false);
                    }

                    battleManager.battleDialogueText.SetActive(true);

                    globalAudioSource.clip = selectEnemy;
                    globalAudioSource.Play();
                }
            }

            // Do nothing
            if (battleManager.currentBattleState == GameStates.WAITING)
            {
                if (Input.GetKeyDown(KeyCode.X) && battleManager.lastBattleState == GameStates.TALKING)
                {
                    foreach (GameObject text in enemyTexts)
                    {
                        text.SetActive(true);
                    }

                    battleManager.battleDialogueText.SetActive(false);
                    battleManager.currentBattleState = GameStates.TALKING;
                    battleManager.lastBattleState = GameStates.WAITING;

                    globalAudioSource.clip = selectEnemy;
                    globalAudioSource.Play();
                }

                else if (Input.GetKeyDown(KeyCode.Z) && battleManager.lastBattleState == GameStates.ATTACKING)
                {
                    foreach (GameObject text in enemyTexts)
                    {
                        text.SetActive(true);
                    }

                    battleManager.battleDialogueText.SetActive(false);
                    battleManager.currentBattleState = GameStates.ATTACKING;
                    battleManager.lastBattleState = GameStates.WAITING;

                    pressedZ = true;

                    globalAudioSource.clip = selectEnemy;
                    globalAudioSource.Play();
                }

                else if (Input.GetKeyDown(KeyCode.X) && battleManager.lastBattleState == GameStates.USING_ITEM)
                {
                    battleManager.battleDialogueText.GetComponent<Text>().text = battleManager.textToDisplay;

                    battleManager.currentBattleState = GameStates.CHOOSING;
                    battleManager.lastBattleState = GameStates.WAITING;

                    globalAudioSource.clip = selectEnemy;
                    globalAudioSource.Play();
                }

                else if (battleManager.lastBattleState == GameStates.RUNNING)
                {
                    if (GameMaster.playerSpeed >= 5 && Input.GetKeyDown(KeyCode.Z) && !pressedZ && battleManager.zodiacToFight == "")
                    {
                        battleManager.currentBattleState = GameStates.NONE;
                        battleManager.lastBattleState = GameStates.WAITING;

                        StartCoroutine(nameof(WaitFinishGame));

                        pressedZ = true;

                        globalAudioSource.clip = selectEnemy;
                        globalAudioSource.Play();
                    }

                    else if ((GameMaster.playerSpeed < 5 || battleManager.zodiacToFight != "") && Input.GetKeyDown(KeyCode.X))
                    {
                        battleManager.currentBattleState = GameStates.CHOOSING;
                        battleManager.lastBattleState = GameStates.WAITING;

                        battleManager.battleDialogueText.GetComponent<Text>().text = battleManager.textToDisplay;

                        globalAudioSource.clip = selectEnemy;
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

            if (leftRight < 0 && !pressedHor && battleManager.currentBattleState == GameStates.CHOOSING)
            {
                pressedHor = true;
                if (currentButtonIndex == 0)
                {
                    battleButtons[currentButtonIndex].GetComponent<ManageButtonSelection>().OnExitSelection();
                    currentButtonIndex = battleButtons.Count - 1;
                    battleButtons[currentButtonIndex].GetComponent<ManageButtonSelection>().OnSelection();
                }

                else
                {
                    battleButtons[currentButtonIndex].GetComponent<ManageButtonSelection>().OnExitSelection();
                    currentButtonIndex--;
                    battleButtons[currentButtonIndex].GetComponent<ManageButtonSelection>().OnSelection();
                }
            }

            else if (leftRight > 0 && !pressedHor && battleManager.currentBattleState == GameStates.CHOOSING)
            {
                pressedHor = true;
                if (currentButtonIndex == battleButtons.Count - 1)
                {
                    battleButtons[currentButtonIndex].GetComponent<ManageButtonSelection>().OnExitSelection();
                    currentButtonIndex = 0;
                    battleButtons[currentButtonIndex].GetComponent<ManageButtonSelection>().OnSelection();
                }

                else
                {
                    battleButtons[currentButtonIndex].GetComponent<ManageButtonSelection>().OnExitSelection();
                    currentButtonIndex++;
                    battleButtons[currentButtonIndex].GetComponent<ManageButtonSelection>().OnSelection();
                }
            }

            else if (leftRight == 0 && pressedHor && battleManager.currentBattleState == GameStates.CHOOSING)
            {
                pressedHor = false;
            }

            // Attack
            if (currentButtonIndex == 0 && Input.GetKeyDown(KeyCode.Z) && (battleManager.currentBattleState != GameStates.ATTACKING && battleManager.currentBattleState != GameStates.TALKING && battleManager.currentBattleState != GameStates.USING_ITEM && battleManager.currentBattleState != GameStates.RUNNING))
            {
                battleManager.currentBattleState = GameStates.ATTACKING;
                battleManager.battleDialogueText.SetActive(false);

                globalAudioSource.clip = selectEnemy;
                globalAudioSource.Play();

                for (int i = 0; i < battleManager.enemiesSpawned.Count; i++)
                {
                    enemyTexts[i].SetActive(true);
                    enemyTexts[i].GetComponent<Text>().text = battleManager.enemiesSpawned[i].GetComponent<EnemyCard>().CardName;
                    enemyTexts[i].transform.GetChild(1).GetComponent<Text>().text = "    " + battleManager.enemiesSpawned[i].GetComponent<EnemyCard>().CardName;

                    // TODO: This variable is never used, check if it is really necessary
                    // battleManager.enemiesSpawned[i].GetComponent<EnemyCard>().nameText = enemyTexts[i];

                    if (battleManager.zodiacToFight != "")
                    {
                        enemyTexts[i].GetComponent<Text>().color = new Color(0.925f, 0.835f, 0.0f);
                        enemyTexts[i].transform.GetChild(1).GetComponent<Text>().color = new Color(0.925f, 0.835f, 0.0f);
                    }

                    else if (battleManager.enemiesSpawned[i].name[0].ToString() == "D" || battleManager.enemiesSpawned[i].name[0].ToString() == "H" || battleManager.enemiesSpawned[i].name[0].ToString() == "P")
                    {
                        enemyTexts[i].GetComponent<Text>().color = new Color(1.0f, 0.0f, 0.31f);
                        enemyTexts[i].transform.GetChild(1).GetComponent<Text>().color = new Color(1.0f, 0.0f, 0.31f);
                    }

                    else if (battleManager.enemiesSpawned[i].name[0].ToString() == "S" || battleManager.enemiesSpawned[i].name[0].ToString() == "C" || battleManager.enemiesSpawned[i].name[0].ToString() == "B")
                    {
                        enemyTexts[i].GetComponent<Text>().color = new Color(0.19f, 0.68f, 1.0f);
                        enemyTexts[i].transform.GetChild(1).GetComponent<Text>().color = new Color(0.19f, 0.68f, 1.0f);
                    }
                }

                currentTextIndex = 0;
                enemyTexts[currentTextIndex].GetComponent<Text>().enabled = false;
                enemyTexts[currentTextIndex].transform.GetChild(0).gameObject.SetActive(true);
                enemyTexts[currentTextIndex].transform.GetChild(1).gameObject.SetActive(true);

                pressedZ = true;
            }

            if (battleManager.currentBattleState == GameStates.ATTACKING && Input.GetKeyDown(KeyCode.Z) && !pressedZ)
            {
                pressedZ = true;
                battleManager.AttackAction(enemyTexts, currentTextIndex);

                globalAudioSource.clip = attackEnemy;
                globalAudioSource.Play();
            }

            // Listen and talk
            else if (currentButtonIndex == 1 && Input.GetKeyDown(KeyCode.Z) && (battleManager.currentBattleState != GameStates.ATTACKING && battleManager.currentBattleState != GameStates.TALKING && battleManager.currentBattleState != GameStates.USING_ITEM && battleManager.currentBattleState != GameStates.RUNNING))
            {
                battleManager.currentBattleState = GameStates.TALKING;
                battleManager.battleDialogueText.SetActive(false);

                globalAudioSource.clip = selectEnemy;
                globalAudioSource.Play();

                for (int i = 0; i < battleManager.enemiesSpawned.Count; i++)
                {
                    enemyTexts[i].SetActive(true);
                    enemyTexts[i].GetComponent<Text>().text = battleManager.enemiesSpawned[i].GetComponent<EnemyCard>().CardName;
                    enemyTexts[i].transform.GetChild(1).GetComponent<Text>().text = "    " + battleManager.enemiesSpawned[i].GetComponent<EnemyCard>().CardName;

                    // TODO: This variable is never used, check if it is really necessary
                    // battleManager.enemiesSpawned[i].GetComponent<EnemyCard>().nameText = enemyTexts[i];

                    if (battleManager.zodiacToFight != "")
                    {
                        enemyTexts[i].GetComponent<Text>().color = new Color(0.925f, 0.835f, 0.0f);
                        enemyTexts[i].transform.GetChild(1).GetComponent<Text>().color = new Color(0.925f, 0.835f, 0.0f);
                    }

                    else if (battleManager.enemiesSpawned[i].name[0].ToString() == "D" || battleManager.enemiesSpawned[i].name[0].ToString() == "H" || battleManager.enemiesSpawned[i].name[0].ToString() == "P")
                    {
                        enemyTexts[i].GetComponent<Text>().color = new Color(1.0f, 0.0f, 0.31f);
                        enemyTexts[i].transform.GetChild(1).GetComponent<Text>().color = new Color(1.0f, 0.0f, 0.31f);
                    }

                    else if (battleManager.enemiesSpawned[i].name[0].ToString() == "S" || battleManager.enemiesSpawned[i].name[0].ToString() == "C" || battleManager.enemiesSpawned[i].name[0].ToString() == "B")
                    {
                        enemyTexts[i].GetComponent<Text>().color = new Color(0.19f, 0.68f, 1.0f);
                        enemyTexts[i].transform.GetChild(1).GetComponent<Text>().color = new Color(0.19f, 0.68f, 1.0f);
                    }
                }

                currentTextIndex = 0;
                enemyTexts[currentTextIndex].GetComponent<Text>().enabled = false;
                enemyTexts[currentTextIndex].transform.GetChild(0).gameObject.SetActive(true);
                enemyTexts[currentTextIndex].transform.GetChild(1).gameObject.SetActive(true);

                pressedZ = true;
            }

            if (battleManager.currentBattleState == GameStates.TALKING && Input.GetKeyDown(KeyCode.Z) && !pressedZ)
            {
                pressedZ = true;
                battleManager.TalkAction(enemyTexts);

                globalAudioSource.clip = selectEnemy;
                globalAudioSource.Play();
            }

            // Open inventory
            else if (currentButtonIndex == 2 && Input.GetKeyDown(KeyCode.Z) && (battleManager.currentBattleState != GameStates.ATTACKING && battleManager.currentBattleState != GameStates.TALKING && battleManager.currentBattleState != GameStates.USING_ITEM && battleManager.currentBattleState != GameStates.RUNNING))
            {
                globalAudioSource.clip = selectEnemy;
                globalAudioSource.Play();

                if (GameMaster.inventory.Count == 0)
                {
                    battleManager.currentBattleState = GameStates.WAITING;
                    battleManager.lastBattleState = GameStates.USING_ITEM;

                    battleManager.battleDialogueText.GetComponent<Text>().text = "    YOU HAVE NO ITEMS RIGHT NOW";
                }

                else
                {
                    battleManager.currentBattleState = GameStates.USING_ITEM;
                    battleManager.battleDialogueText.SetActive(false);

                    for (int i = 0; i < GameMaster.inventory.Count; i++)
                    {
                        itemTexts[i].SetActive(true);
                        itemTexts[i].GetComponent<Text>().text = GameMaster.inventory[i].ObjectName + " LVL." + GameMaster.inventory[i].Level;
                        itemTexts[i].transform.GetChild(1).GetComponent<Text>().text = "    " + GameMaster.inventory[i].ObjectName + " LVL." + GameMaster.inventory[i].Level;

                        GameMaster.inventory[i].GameText = itemTexts[i];

                        if (GameMaster.inventory[i].Type == ObjectTypes.HEALTH)
                        {
                            itemTexts[i].GetComponent<Text>().color = Color.red;
                            itemTexts[i].transform.GetChild(1).GetComponent<Text>().color = Color.red;
                        }

                        else if (GameMaster.inventory[i].Type == ObjectTypes.ATTACK)
                        {
                            itemTexts[i].GetComponent<Text>().color = new Color(1.0f, 0.37f, 0.0f);
                            itemTexts[i].transform.GetChild(1).GetComponent<Text>().color = new Color(1.0f, 0.37f, 0.0f);
                        }

                        else if (GameMaster.inventory[i].Type == ObjectTypes.DEFENSE)
                        {
                            itemTexts[i].GetComponent<Text>().color = new Color(0.0f, 0.36f, 1.0f);
                            itemTexts[i].transform.GetChild(1).GetComponent<Text>().color = new Color(0.0f, 0.36f, 1.0f);
                        }

                        else
                        {
                            itemTexts[i].GetComponent<Text>().color = Color.green;
                            itemTexts[i].transform.GetChild(1).GetComponent<Text>().color = Color.green;
                        }
                    }

                    currentTextIndex = 0;
                    itemTexts[currentTextIndex].GetComponent<Text>().enabled = false;
                    itemTexts[currentTextIndex].transform.GetChild(0).gameObject.SetActive(true);
                    itemTexts[currentTextIndex].transform.GetChild(1).gameObject.SetActive(true);
                }

                pressedZ = true;
            }

            if (battleManager.currentBattleState == GameStates.USING_ITEM && Input.GetKeyDown(KeyCode.Z) && !pressedZ)
            {
                pressedZ = true;
                battleManager.UseItemAction(itemTexts, currentTextIndex);

                globalAudioSource.clip = useItem;
                globalAudioSource.Play();
            }

            // Run from battle
            else if (currentButtonIndex == 3 && Input.GetKeyDown(KeyCode.Z) && (battleManager.currentBattleState != GameStates.ATTACKING && battleManager.currentBattleState != GameStates.TALKING && battleManager.currentBattleState != GameStates.USING_ITEM && battleManager.currentBattleState != GameStates.RUNNING && battleManager.currentBattleState != GameStates.WAITING))
            {
                battleManager.RunAction();
                pressedZ = true;

                globalAudioSource.clip = selectEnemy;
                globalAudioSource.Play();
            }

            // Change selected enemy
            float topBottom = Input.GetAxis("Vertical");
            if (topBottom > 0 && !pressedVer && (battleManager.currentBattleState == GameStates.ATTACKING || battleManager.currentBattleState == GameStates.TALKING))
            {
                pressedVer = true;
                if (currentTextIndex == 0)
                {
                    enemyTexts[currentTextIndex].GetComponent<Text>().enabled = true;
                    enemyTexts[currentTextIndex].transform.GetChild(0).gameObject.SetActive(false);
                    enemyTexts[currentTextIndex].transform.GetChild(1).gameObject.SetActive(false);

                    currentTextIndex = battleManager.enemiesSpawned.Count - 1;

                    enemyTexts[currentTextIndex].GetComponent<Text>().enabled = false;
                    enemyTexts[currentTextIndex].transform.GetChild(0).gameObject.SetActive(true);
                    enemyTexts[currentTextIndex].transform.GetChild(1).gameObject.SetActive(true);
                }

                else
                {
                    enemyTexts[currentTextIndex].GetComponent<Text>().enabled = true;
                    enemyTexts[currentTextIndex].transform.GetChild(0).gameObject.SetActive(false);
                    enemyTexts[currentTextIndex].transform.GetChild(1).gameObject.SetActive(false);

                    currentTextIndex--;

                    enemyTexts[currentTextIndex].GetComponent<Text>().enabled = false;
                    enemyTexts[currentTextIndex].transform.GetChild(0).gameObject.SetActive(true);
                    enemyTexts[currentTextIndex].transform.GetChild(1).gameObject.SetActive(true);
                }
            }

            else if (topBottom < 0 && !pressedVer && (battleManager.currentBattleState == GameStates.ATTACKING || battleManager.currentBattleState == GameStates.TALKING))
            {
                pressedVer = true;
                if (currentTextIndex == battleManager.enemiesSpawned.Count - 1)
                {
                    enemyTexts[currentTextIndex].GetComponent<Text>().enabled = true;
                    enemyTexts[currentTextIndex].transform.GetChild(0).gameObject.SetActive(false);
                    enemyTexts[currentTextIndex].transform.GetChild(1).gameObject.SetActive(false);

                    currentTextIndex = 0;

                    enemyTexts[currentTextIndex].GetComponent<Text>().enabled = false;
                    enemyTexts[currentTextIndex].transform.GetChild(0).gameObject.SetActive(true);
                    enemyTexts[currentTextIndex].transform.GetChild(1).gameObject.SetActive(true);
                }

                else
                {
                    enemyTexts[currentTextIndex].GetComponent<Text>().enabled = true;
                    enemyTexts[currentTextIndex].transform.GetChild(0).gameObject.SetActive(false);
                    enemyTexts[currentTextIndex].transform.GetChild(1).gameObject.SetActive(false);

                    currentTextIndex++;

                    enemyTexts[currentTextIndex].GetComponent<Text>().enabled = false;
                    enemyTexts[currentTextIndex].transform.GetChild(0).gameObject.SetActive(true);
                    enemyTexts[currentTextIndex].transform.GetChild(1).gameObject.SetActive(true);
                }
            }

            else if (topBottom == 0 && pressedVer)
            {
                pressedVer = false;
            }

            // Change selected item
            if (topBottom > 0 && !pressedVer && battleManager.currentBattleState == GameStates.USING_ITEM)
            {
                pressedVer = true;
                if (currentTextIndex == 0)
                {
                    // Nothing
                }

                else
                {
                    itemTexts[currentTextIndex].GetComponent<Text>().enabled = true;
                    itemTexts[currentTextIndex].transform.GetChild(0).gameObject.SetActive(false);
                    itemTexts[currentTextIndex].transform.GetChild(1).gameObject.SetActive(false);

                    currentTextIndex--;

                    itemTexts[currentTextIndex].GetComponent<Text>().enabled = false;
                    itemTexts[currentTextIndex].transform.GetChild(0).gameObject.SetActive(true);
                    itemTexts[currentTextIndex].transform.GetChild(1).gameObject.SetActive(true);
                }
            }

            else if (topBottom < 0 && !pressedVer && battleManager.currentBattleState == GameStates.USING_ITEM)
            {
                pressedVer = true;
                if (currentTextIndex == GameMaster.inventory.Count - 1)
                {
                    // Nothing
                }

                else
                {
                    itemTexts[currentTextIndex].GetComponent<Text>().enabled = true;
                    itemTexts[currentTextIndex].transform.GetChild(0).gameObject.SetActive(false);
                    itemTexts[currentTextIndex].transform.GetChild(1).gameObject.SetActive(false);

                    currentTextIndex++;

                    itemTexts[currentTextIndex].GetComponent<Text>().enabled = false;
                    itemTexts[currentTextIndex].transform.GetChild(0).gameObject.SetActive(true);
                    itemTexts[currentTextIndex].transform.GetChild(1).gameObject.SetActive(true);
                }
            }

            else if (leftRight > 0 && !pressedHor && battleManager.currentBattleState == GameStates.USING_ITEM)
            {
                pressedHor = true;
                if (currentTextIndex + 4 > GameMaster.inventory.Count - 1)
                {
                    // Nothing
                }

                else
                {
                    itemTexts[currentTextIndex].GetComponent<Text>().enabled = true;
                    itemTexts[currentTextIndex].transform.GetChild(0).gameObject.SetActive(false);
                    itemTexts[currentTextIndex].transform.GetChild(1).gameObject.SetActive(false);

                    currentTextIndex += 4;

                    itemTexts[currentTextIndex].GetComponent<Text>().enabled = false;
                    itemTexts[currentTextIndex].transform.GetChild(0).gameObject.SetActive(true);
                    itemTexts[currentTextIndex].transform.GetChild(1).gameObject.SetActive(true);
                }
            }

            else if (leftRight < 0 && !pressedHor && battleManager.currentBattleState == GameStates.USING_ITEM)
            {
                pressedHor = true;
                if (currentTextIndex - 4 < 0)
                {
                    // Nothing
                }

                else
                {
                    itemTexts[currentTextIndex].GetComponent<Text>().enabled = true;
                    itemTexts[currentTextIndex].transform.GetChild(0).gameObject.SetActive(false);
                    itemTexts[currentTextIndex].transform.GetChild(1).gameObject.SetActive(false);

                    currentTextIndex -= 4;

                    itemTexts[currentTextIndex].GetComponent<Text>().enabled = false;
                    itemTexts[currentTextIndex].transform.GetChild(0).gameObject.SetActive(true);
                    itemTexts[currentTextIndex].transform.GetChild(1).gameObject.SetActive(true);
                }
            }

            else if (topBottom == 0 && pressedVer)
            {
                pressedVer = false;
            }

            else if (leftRight == 0 && pressedHor)
            {
                pressedHor = false;
            }
        }
    }
    
    // Start and end battle functions
    public void StartBattle()
    {
        battleManager.SetUpBattle();

        player.GetComponent<PlayerAnimationDirection>().SetDirection(new Vector2(0, 0));
        miniMap.SetActive(false);

        //animCanvas.GetComponent<AudioSource>().Play();

        foreach (GameObject animator in starAnimators)
        {
            animator.GetComponent<Animator>().SetBool("Change", true);
            animator.transform.GetChild(1).GetComponent<Animator>().SetBool("Change", true);
        }

        StartCoroutine(nameof(WaitStartGame), BattleType.NORMAL);
    }

    public void StartBossBattle(string zodiac)
    {
        battleManager.SetUpBossBattle(zodiac);

        player.GetComponent<PlayerAnimationDirection>().SetDirection(new Vector2(0, 0));
        miniMap.SetActive(false);

        //animCanvas.GetComponent<AudioSource>().Play();

        foreach (GameObject animator in starAnimators)
        {
            animator.GetComponent<Animator>().SetBool("Change", true);
            animator.transform.GetChild(1).GetComponent<Animator>().SetBool("Change", true);
        }

        StartCoroutine(nameof(WaitStartGame), BattleType.BOSS);
    }

    public void ClearGame()
    {
        battleButtons[currentButtonIndex].GetComponent<ManageButtonSelection>().OnExitSelection();

        for (int i = 2; i < battleManager.enemiesParent.transform.childCount; i++)
        {
            Destroy(battleManager.enemiesParent.transform.GetChild(i).gameObject);
        }

        foreach (GameObject text in enemyTexts)
        {
            text.GetComponent<Text>().text = "";
            text.transform.GetChild(1).GetComponent<Text>().text = "";
        }

        foreach (GameObject text in itemTexts)
        {
            text.GetComponent<Text>().text = "";
            text.transform.GetChild(1).GetComponent<Text>().text = "";
        }
    }

    // Dialogue functions
    public void StartDialogue(string zodiac, DialogueBox thisChar)
    {
        miniMap.SetActive(false);
        battleManager.zodiacToFight = zodiac;
        thisCharacter = thisChar;

        // TODO: What was this for ->>>> templates.mapFormed = false;
        player.GetComponent<PlayerAnimationDirection>().SetDirection(new Vector2(0, 0));

        dialogueImage.sprite = thisChar.speakerImage;
        dialogueNameText.text = thisChar.speakerName;

        dialogueText.text = "";
        dialogueBox.SetActive(true);
        menuButton.SetActive(false);

        if (zodiac == "CANCER")
        {
            if (GameMaster.cancerIndex == thisCharacter.characterConversations.Count - 1)
            {
                GameMaster.cancerIndex--;
            }

            thisChar.characterConversations[GameMaster.cancerIndex].currentDialogueLine = 0;
            StartCoroutine(nameof(WriteDialogue), thisChar.characterConversations[GameMaster.cancerIndex].dialogueLines[thisChar.characterConversations[GameMaster.cancerIndex].currentDialogueLine].dialogueText);
        }

        else if (zodiac == "CAPRICORN")
        {
            if (GameMaster.capricornIndex == thisCharacter.characterConversations.Count - 1)
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
        pressedZ = false;

        if (battleManager.zodiacToFight == "CANCER")
        {
            if (thisCharacter.characterConversations[GameMaster.cancerIndex].currentDialogueLine < thisCharacter.characterConversations[GameMaster.cancerIndex].dialogueLines.Count - 1)
            {
                dialogueText.text = "";
                thisCharacter.characterConversations[GameMaster.cancerIndex].currentDialogueLine++;
                StartCoroutine(nameof(WriteDialogue), thisCharacter.characterConversations[GameMaster.cancerIndex].dialogueLines[thisCharacter.characterConversations[GameMaster.cancerIndex].currentDialogueLine].dialogueText);
            }

            else
            {
                if (GameMaster.cancerIndex < thisCharacter.characterConversations.Count - 1)
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

        else if (battleManager.zodiacToFight == "CAPRICORN")
        {
            if (thisCharacter.characterConversations[GameMaster.capricornIndex].currentDialogueLine < thisCharacter.characterConversations[GameMaster.capricornIndex].dialogueLines.Count - 1)
            {
                dialogueText.text = "";
                thisCharacter.characterConversations[GameMaster.capricornIndex].currentDialogueLine++;
                StartCoroutine(nameof(WriteDialogue), thisCharacter.characterConversations[GameMaster.capricornIndex].dialogueLines[thisCharacter.characterConversations[GameMaster.capricornIndex].currentDialogueLine].dialogueText);
            }

            else
            {
                if (GameMaster.capricornIndex < thisCharacter.characterConversations.Count - 1)
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
            StartBossBattle(battleManager.zodiacToFight);
        }

        else
        {
            battleManager.WinGame();
        }
    }

    IEnumerator WaitMove()
    {
        yield return new WaitForSeconds(1f);

        playerCanMove = true;
    }

    IEnumerator WaitStartGame(BattleType battleType)
    {
        if (battleType == BattleType.NORMAL)
        {
            yield return new WaitForSeconds(1.2f);

            foreach (GameObject animator in starAnimators)
            {
                animator.GetComponent<Animator>().SetBool("Change", false);
                animator.transform.GetChild(1).GetComponent<Animator>().SetBool("Change", false);
            }

            battleCanvas.SetActive(true);

            player.GetComponent<SpriteRenderer>().sortingOrder = -10;
            player.transform.GetChild(1).GetComponent<AudioSource>().Stop();
            player.transform.GetChild(0).GetComponent<CircleCollider2D>().enabled = false;

            realRooms.SetActive(false);

            battleManager.ManageSpawnBasicEnemies();

            blackScreen.GetComponent<Animator>().SetBool("Change", true);

            yield return new WaitForSeconds(0.25f);
            foreach (GameObject button in battleButtons)
            {
                button.GetComponent<ManageButtonSelection>().OnExitSelection();
            }
            battleButtons[0].GetComponent<ManageButtonSelection>().OnSelection();
            currentButtonIndex = 0;
            battleManager.currentBattleState = GameStates.CHOOSING;
        }
        
        else if (battleType == BattleType.BOSS)
        {
            yield return new WaitForSeconds(1.2f);

            foreach (GameObject animator in starAnimators)
            {
                animator.GetComponent<Animator>().SetBool("Change", false);
                animator.transform.GetChild(1).GetComponent<Animator>().SetBool("Change", false);
            }

            battleCanvas.SetActive(true);

            player.GetComponent<SpriteRenderer>().sortingOrder = -10;
            player.transform.GetChild(1).GetComponent<AudioSource>().Stop();
            player.transform.GetChild(0).GetComponent<CircleCollider2D>().enabled = false;

            realRooms.SetActive(false);

            battleManager.ManageSpawnBoss();

            blackScreen.GetComponent<Animator>().SetBool("Change", true);

            yield return new WaitForSeconds(0.25f);
            foreach (GameObject button in battleButtons)
            {
                button.GetComponent<ManageButtonSelection>().OnExitSelection();
            }
            battleButtons[0].GetComponent<ManageButtonSelection>().OnSelection();
            currentButtonIndex = 0;
            battleManager.currentBattleState = GameStates.CHOOSING;
        }
    }

    IEnumerator WaitFinishGame()
    {
        blackScreen.GetComponent<Animator>().SetBool("Change", false);
        battleManager.currentBattleState = GameStates.NONE;

        yield return new WaitForSeconds(0.6f);

        foreach (GameObject animator in starAnimators)
        {
            animator.GetComponent<Animator>().SetBool("ChangeBack", true);
            animator.transform.GetChild(1).GetComponent<Animator>().SetBool("ChangeBack", true);
        }

        yield return new WaitForSeconds(0.25f);
        ClearGame();
        battleCanvas.SetActive(false);

        player.GetComponent<SpriteRenderer>().sortingOrder = -3;
        player.transform.GetChild(1).GetComponent<AudioSource>().Play();
        realRooms.SetActive(true);

        yield return new WaitForSeconds(1.2f);
        foreach (GameObject animator in starAnimators)
        {
            animator.GetComponent<Animator>().SetBool("ChangeBack", false);
            animator.transform.GetChild(1).GetComponent<Animator>().SetBool("ChangeBack", false);
        }

        miniMap.SetActive(true);
        player.transform.GetChild(0).GetComponent<CircleCollider2D>().enabled = true;
    }

    IEnumerator WaitFinishBossGame()
    {
        blackScreen.GetComponent<Animator>().SetBool("Change", false);
        battleManager.currentBattleState = GameStates.NONE;

        yield return new WaitForSeconds(0.6f);

        foreach (GameObject animator in starAnimators)
        {
            animator.GetComponent<Animator>().SetBool("ChangeBack", true);
            animator.transform.GetChild(1).GetComponent<Animator>().SetBool("ChangeBack", true);
        }

        yield return new WaitForSeconds(0.25f);
        ClearGame();
        battleCanvas.SetActive(false);

        player.GetComponent<SpriteRenderer>().sortingOrder = -3;
        player.transform.GetChild(1).GetComponent<AudioSource>().Play();
        realRooms.SetActive(true);

        yield return new WaitForSeconds(1.2f);
        foreach (GameObject animator in starAnimators)
        {
            animator.GetComponent<Animator>().SetBool("ChangeBack", false);
            animator.transform.GetChild(1).GetComponent<Animator>().SetBool("ChangeBack", false);
        }

        miniMap.SetActive(true);
        player.transform.GetChild(0).GetComponent<CircleCollider2D>().enabled = true;

        bossBeaten = true;
        StartDialogue(battleManager.zodiacToFight, thisCharacter);
    }
}