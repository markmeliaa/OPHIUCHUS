using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    public GameObject playerStarSpawn;

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

    public RoomTemplates templates;

    public GameObject dialogueBox;
    public GameObject menuButton;
    public Text dialogueText;
    public GameObject nextButton;

    public PlayerMoveIso playerMove;

    public Image dialogueImage;
    public Text dialogueNameText;

    private DialogueBox thisCharacter;

    private bool bossBeaten = false;

    // Start is called before the first frame update
    void Start()
    {
        /*
        foreach (GameObject animator in starAnimators)
        {
            animator.GetComponent<Animator>().SetBool("Change", true);
            animator.transform.GetChild(1).GetComponent<Animator>().SetBool("Change", true);
        }

        StartCoroutine("WaitStartGame");
        */
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Z))
            pressedZ = false;

        if (battleCanvas.activeSelf)
        {
            // The battle is paused
            if (battleManager.state == gameStates.stop)
                return;

            // The battle has ended
            if (battleManager.state == gameStates.end)
            {
                if (!pressedZ && Input.GetKeyDown(KeyCode.Z) && battleManager.Zodiac == "")
                {
                    StartCoroutine("WaitFinishGame");
                }

                else if (!pressedZ && Input.GetKeyDown(KeyCode.Z) && battleManager.Zodiac != "")
                {
                    StartCoroutine("WaitFinishBossGame");
                }

                return;
            }

            // The enemy is attacking and the player avoids the attacks
            if (battleManager.state == gameStates.defend)
            {
                if (!pressedZ && Input.GetKeyDown(KeyCode.Z) && !playerCanMove)
                {
                    battleManager.StartEnemyAttack();
                    StartCoroutine("WaitMove");

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

            if (battleManager.state == gameStates.win)
            {
                if (!pressedZ && Input.GetKeyDown(KeyCode.Z))
                {
                    battleManager.Win();
                    pressedZ = true;
                }

                return;
            }

            // Go back in the menu
            if (Input.GetKeyDown(KeyCode.X))
            {
                if (battleManager.state == gameStates.attacking || battleManager.state == gameStates.talking)
                {
                    battleManager.state = gameStates.choosing;
                    battleManager.normalText.GetComponent<Text>().text = battleManager.baseText;

                    // Disable enemy texts
                    enemyTexts[currentTextIndex].GetComponent<Text>().enabled = true;
                    enemyTexts[currentTextIndex].transform.GetChild(0).gameObject.SetActive(false);
                    enemyTexts[currentTextIndex].transform.GetChild(1).gameObject.SetActive(false);

                    foreach (GameObject text in enemyTexts)
                        text.SetActive(false);

                    battleManager.normalText.SetActive(true);

                    globalAudioSource.clip = selectEnemy;
                    globalAudioSource.Play();
                }

                else if (battleManager.state == gameStates.inventory)
                {
                    battleManager.state = gameStates.choosing;
                    battleManager.normalText.GetComponent<Text>().text = battleManager.baseText;

                    // Disable item texts
                    itemTexts[currentTextIndex].GetComponent<Text>().enabled = true;
                    itemTexts[currentTextIndex].transform.GetChild(0).gameObject.SetActive(false);
                    itemTexts[currentTextIndex].transform.GetChild(1).gameObject.SetActive(false);

                    foreach (GameObject text in itemTexts)
                        text.SetActive(false);

                    battleManager.normalText.SetActive(true);

                    globalAudioSource.clip = selectEnemy;
                    globalAudioSource.Play();
                }
            }

            // Do nothing
            if (battleManager.state == gameStates.waiting)
            {
                if (Input.GetKeyDown(KeyCode.X) && battleManager.lastState == gameStates.talking)
                {
                    foreach (GameObject text in enemyTexts)
                        text.SetActive(true);

                    battleManager.normalText.SetActive(false);
                    battleManager.state = gameStates.talking;
                    battleManager.lastState = gameStates.waiting;

                    globalAudioSource.clip = selectEnemy;
                    globalAudioSource.Play();
                }

                else if (Input.GetKeyDown(KeyCode.Z) && battleManager.lastState == gameStates.attacking)
                {
                    foreach (GameObject text in enemyTexts)
                        text.SetActive(true);

                    battleManager.normalText.SetActive(false);
                    battleManager.state = gameStates.attacking;
                    battleManager.lastState = gameStates.waiting;

                    pressedZ = true;

                    globalAudioSource.clip = selectEnemy;
                    globalAudioSource.Play();
                }

                else if (Input.GetKeyDown(KeyCode.X) && battleManager.lastState == gameStates.inventory)
                {
                    battleManager.normalText.GetComponent<Text>().text = battleManager.baseText;

                    battleManager.state = gameStates.choosing;
                    battleManager.lastState = gameStates.waiting;

                    globalAudioSource.clip = selectEnemy;
                    globalAudioSource.Play();
                }

                else if (battleManager.lastState == gameStates.run)
                {
                    if (GameMaster.playerSpeed >= 5 && Input.GetKeyDown(KeyCode.Z) && !pressedZ && battleManager.Zodiac == "")
                    {
                        battleManager.state = gameStates.stop;
                        battleManager.lastState = gameStates.waiting;

                        StartCoroutine("WaitFinishGame");

                        pressedZ = true;

                        globalAudioSource.clip = selectEnemy;
                        globalAudioSource.Play();
                    }

                    else if ((GameMaster.playerSpeed < 5 || battleManager.Zodiac != "") && Input.GetKeyDown(KeyCode.X))
                    {
                        battleManager.state = gameStates.choosing;
                        battleManager.lastState = gameStates.waiting;

                        battleManager.normalText.GetComponent<Text>().text = battleManager.baseText;

                        globalAudioSource.clip = selectEnemy;
                        globalAudioSource.Play();
                    }
                }

                else
                    return;
            }

            // Change choosing button
            float leftRight = Input.GetAxis("Horizontal");

            if (leftRight < 0 && !pressedHor && battleManager.state == gameStates.choosing)
            {
                pressedHor = true;
                if (currentButtonIndex == 0)
                {
                    battleButtons[currentButtonIndex].GetComponent<SelectButton>().OnExitSelection();
                    currentButtonIndex = battleButtons.Count - 1;
                    battleButtons[currentButtonIndex].GetComponent<SelectButton>().OnSelection();
                }

                else
                {
                    battleButtons[currentButtonIndex].GetComponent<SelectButton>().OnExitSelection();
                    currentButtonIndex--;
                    battleButtons[currentButtonIndex].GetComponent<SelectButton>().OnSelection();
                }
            }

            else if (leftRight > 0 && !pressedHor && battleManager.state == gameStates.choosing)
            {
                pressedHor = true;
                if (currentButtonIndex == battleButtons.Count - 1)
                {
                    battleButtons[currentButtonIndex].GetComponent<SelectButton>().OnExitSelection();
                    currentButtonIndex = 0;
                    battleButtons[currentButtonIndex].GetComponent<SelectButton>().OnSelection();
                }

                else
                {
                    battleButtons[currentButtonIndex].GetComponent<SelectButton>().OnExitSelection();
                    currentButtonIndex++;
                    battleButtons[currentButtonIndex].GetComponent<SelectButton>().OnSelection();
                }
            }

            else if (leftRight == 0 && pressedHor && battleManager.state == gameStates.choosing)
            {
                pressedHor = false;
            }

            // Attack
            if (currentButtonIndex == 0 && Input.GetKeyDown(KeyCode.Z) && (battleManager.state != gameStates.attacking && battleManager.state != gameStates.talking && battleManager.state != gameStates.inventory && battleManager.state != gameStates.run))
            {
                battleManager.state = gameStates.attacking;
                battleManager.normalText.SetActive(false);

                globalAudioSource.clip = selectEnemy;
                globalAudioSource.Play();

                for (int i = 0; i < battleManager.enemiesSpawned.Count; i++)
                {
                    enemyTexts[i].SetActive(true);
                    enemyTexts[i].GetComponent<Text>().text = battleManager.enemiesSpawned[i].GetComponent<EnemyCard>().cardName;
                    enemyTexts[i].transform.GetChild(1).GetComponent<Text>().text = "    " + battleManager.enemiesSpawned[i].GetComponent<EnemyCard>().cardName;
                    battleManager.enemiesSpawned[i].GetComponent<EnemyCard>().nameText = enemyTexts[i];

                    if (battleManager.Zodiac != "")
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

            if (battleManager.state == gameStates.attacking && Input.GetKeyDown(KeyCode.Z) && !pressedZ)
            {
                pressedZ = true;
                battleManager.Attack(enemyTexts, currentTextIndex);

                globalAudioSource.clip = attackEnemy;
                globalAudioSource.Play();
            }

            // Listen and talk
            else if (currentButtonIndex == 1 && Input.GetKeyDown(KeyCode.Z) && (battleManager.state != gameStates.attacking && battleManager.state != gameStates.talking && battleManager.state != gameStates.inventory && battleManager.state != gameStates.run))
            {
                battleManager.state = gameStates.talking;
                battleManager.normalText.SetActive(false);

                globalAudioSource.clip = selectEnemy;
                globalAudioSource.Play();

                for (int i = 0; i < battleManager.enemiesSpawned.Count; i++)
                {
                    enemyTexts[i].SetActive(true);
                    enemyTexts[i].GetComponent<Text>().text = battleManager.enemiesSpawned[i].GetComponent<EnemyCard>().cardName;
                    enemyTexts[i].transform.GetChild(1).GetComponent<Text>().text = "    " + battleManager.enemiesSpawned[i].GetComponent<EnemyCard>().cardName;
                    battleManager.enemiesSpawned[i].GetComponent<EnemyCard>().nameText = enemyTexts[i];

                    if (battleManager.Zodiac != "")
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

            if (battleManager.state == gameStates.talking && Input.GetKeyDown(KeyCode.Z) && !pressedZ)
            {
                pressedZ = true;
                battleManager.Talk(enemyTexts);

                globalAudioSource.clip = selectEnemy;
                globalAudioSource.Play();
            }

            // Open inventory
            else if (currentButtonIndex == 2 && Input.GetKeyDown(KeyCode.Z) && (battleManager.state != gameStates.attacking && battleManager.state != gameStates.talking && battleManager.state != gameStates.inventory && battleManager.state != gameStates.run))
            {
                globalAudioSource.clip = selectEnemy;
                globalAudioSource.Play();

                if (GameMaster.inventory.Count == 0)
                {
                    battleManager.state = gameStates.waiting;
                    battleManager.lastState = gameStates.inventory;

                    battleManager.normalText.GetComponent<Text>().text = "    YOU HAVE NO ITEMS RIGHT NOW";
                }

                else
                {
                    battleManager.state = gameStates.inventory;
                    battleManager.normalText.SetActive(false);

                    for (int i = 0; i < GameMaster.inventory.Count; i++)
                    {
                        itemTexts[i].SetActive(true);
                        itemTexts[i].GetComponent<Text>().text = GameMaster.inventory[i].objectName + " LVL." + GameMaster.inventory[i].level;
                        itemTexts[i].transform.GetChild(1).GetComponent<Text>().text = "    " + GameMaster.inventory[i].objectName + " LVL." + GameMaster.inventory[i].level;

                        GameMaster.inventory[i].gameText = itemTexts[i];

                        if (GameMaster.inventory[i].type == objectTypes.health)
                        {
                            itemTexts[i].GetComponent<Text>().color = Color.red;
                            itemTexts[i].transform.GetChild(1).GetComponent<Text>().color = Color.red;
                        }

                        else if (GameMaster.inventory[i].type == objectTypes.attack)
                        {
                            itemTexts[i].GetComponent<Text>().color = new Color(1.0f, 0.37f, 0.0f);
                            itemTexts[i].transform.GetChild(1).GetComponent<Text>().color = new Color(1.0f, 0.37f, 0.0f);
                        }

                        else if (GameMaster.inventory[i].type == objectTypes.defense)
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

            if (battleManager.state == gameStates.inventory && Input.GetKeyDown(KeyCode.Z) && !pressedZ)
            {
                pressedZ = true;
                battleManager.Items(itemTexts, currentTextIndex);

                globalAudioSource.clip = useItem;
                globalAudioSource.Play();
            }

            // Run from battle
            else if (currentButtonIndex == 3 && Input.GetKeyDown(KeyCode.Z) && (battleManager.state != gameStates.attacking && battleManager.state != gameStates.talking && battleManager.state != gameStates.inventory && battleManager.state != gameStates.run && battleManager.state != gameStates.waiting))
            {
                battleManager.Run();
                pressedZ = true;

                globalAudioSource.clip = selectEnemy;
                globalAudioSource.Play();
            }

            // Change selected enemy
            float topBottom = Input.GetAxis("Vertical");
            if (topBottom > 0 && !pressedVer && (battleManager.state == gameStates.attacking || battleManager.state == gameStates.talking))
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

            else if (topBottom < 0 && !pressedVer && (battleManager.state == gameStates.attacking || battleManager.state == gameStates.talking))
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
            if (topBottom > 0 && !pressedVer && battleManager.state == gameStates.inventory)
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

            else if (topBottom < 0 && !pressedVer && battleManager.state == gameStates.inventory)
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

            else if (leftRight > 0 && !pressedHor && battleManager.state == gameStates.inventory)
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

            else if (leftRight < 0 && !pressedHor && battleManager.state == gameStates.inventory)
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

        templates.changingRoom = true;
        player.GetComponent<PlayerMoveIso>().horInput = 0;
        player.GetComponent<PlayerMoveIso>().vertInput = 0;
        player.GetComponent<PlayerMoveIso>().rendIso.SetDirection(new Vector2(0, 0));
        miniMap.SetActive(false);

        //animCanvas.GetComponent<AudioSource>().Play();

        foreach (GameObject animator in starAnimators)
        {
            animator.GetComponent<Animator>().SetBool("Change", true);
            animator.transform.GetChild(1).GetComponent<Animator>().SetBool("Change", true);
        }

        StartCoroutine("WaitStartGame", 1);
    }

    public void StartBossBattle(string zodiac)
    {
        battleManager.SetUpBossBattle(zodiac);

        templates.changingRoom = true;
        player.GetComponent<PlayerMoveIso>().horInput = 0;
        player.GetComponent<PlayerMoveIso>().vertInput = 0;
        player.GetComponent<PlayerMoveIso>().rendIso.SetDirection(new Vector2(0, 0));
        miniMap.SetActive(false);

        //animCanvas.GetComponent<AudioSource>().Play();

        foreach (GameObject animator in starAnimators)
        {
            animator.GetComponent<Animator>().SetBool("Change", true);
            animator.transform.GetChild(1).GetComponent<Animator>().SetBool("Change", true);
        }

        StartCoroutine("WaitStartGame", 2);
    }

    public void ClearGame()
    {
        battleButtons[currentButtonIndex].GetComponent<SelectButton>().OnExitSelection();

        for (int i = 2; i < battleManager.parentEnemies.transform.childCount; i++)
        {
            Destroy(battleManager.parentEnemies.transform.GetChild(i).gameObject);
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
        battleManager.Zodiac = zodiac;
        thisCharacter = thisChar;

        templates.mapFormed = false;
        playerMove.rendIso.SetDirection(new Vector2(0, 0));
        playerMove.horInput = 0;
        playerMove.vertInput = 0;

        dialogueImage.sprite = thisChar.imageSpeaker;
        dialogueNameText.text = thisChar.nameSpeaker;

        dialogueText.text = "";
        dialogueBox.SetActive(true);
        menuButton.SetActive(false);

        if (zodiac == "CANCER")
        {
            if (GameMaster.cancerIndex == thisCharacter.characterConversations.Count - 1)
                GameMaster.cancerIndex -= 1;

            thisChar.characterConversations[GameMaster.cancerIndex].currentDialogueLine = 0;
            StartCoroutine("WriteDialogue", thisChar.characterConversations[GameMaster.cancerIndex].dialogueLines[thisChar.characterConversations[GameMaster.cancerIndex].currentDialogueLine].dialogueText);
        }

        else if (zodiac == "CAPRICORN")
        {
            if (GameMaster.capricornIndex == thisCharacter.characterConversations.Count - 1)
                GameMaster.capricornIndex -= 1;

            thisChar.characterConversations[GameMaster.capricornIndex].currentDialogueLine = 0;
            StartCoroutine("WriteDialogue", thisChar.characterConversations[GameMaster.capricornIndex].dialogueLines[thisChar.characterConversations[GameMaster.capricornIndex].currentDialogueLine].dialogueText);
        }
    }

    public void PressButton()
    {
        nextButton.SetActive(false);
        pressedZ = false;

        //Debug.Log(battleManager.Zodiac);

        if (battleManager.Zodiac == "CANCER")
        {
            if (thisCharacter.characterConversations[GameMaster.cancerIndex].currentDialogueLine < thisCharacter.characterConversations[GameMaster.cancerIndex].dialogueLines.Count - 1)
            {
                dialogueText.text = "";
                thisCharacter.characterConversations[GameMaster.cancerIndex].currentDialogueLine++;
                StartCoroutine("WriteDialogue", thisCharacter.characterConversations[GameMaster.cancerIndex].dialogueLines[thisCharacter.characterConversations[GameMaster.cancerIndex].currentDialogueLine].dialogueText);
            }

            else
            {
                if (GameMaster.cancerIndex < thisCharacter.characterConversations.Count - 1)
                    GameMaster.cancerIndex++;

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

                StartCoroutine("HideDialogue");
            }
        }

        else if (battleManager.Zodiac == "CAPRICORN")
        {
            if (thisCharacter.characterConversations[GameMaster.capricornIndex].currentDialogueLine < thisCharacter.characterConversations[GameMaster.capricornIndex].dialogueLines.Count - 1)
            {
                dialogueText.text = "";
                thisCharacter.characterConversations[GameMaster.capricornIndex].currentDialogueLine++;
                StartCoroutine("WriteDialogue", thisCharacter.characterConversations[GameMaster.capricornIndex].dialogueLines[thisCharacter.characterConversations[GameMaster.capricornIndex].currentDialogueLine].dialogueText);
            }

            else
            {
                if (GameMaster.capricornIndex < thisCharacter.characterConversations.Count - 1)
                    GameMaster.capricornIndex++;

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

                StartCoroutine("HideDialogue");
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
        //templates.mapFormed = true;
        dialogueBox.SetActive(false);

        if (bossBeaten == false)
        {
            StartBossBattle(battleManager.Zodiac);
        }

        else
        {
            battleManager.WinGame();
            //miniMap.SetActive(true);
        }

        //Debug.Log(battleManager.Zodiac);
    }

    IEnumerator WaitMove()
    {
        yield return new WaitForSeconds(1f);

        playerCanMove = true;
    }

    IEnumerator WaitStartGame(int option)
    {
        if (option == 1)
        {
            yield return new WaitForSeconds(1.2f);

            foreach (GameObject animator in starAnimators)
            {
                animator.GetComponent<Animator>().SetBool("Change", false);
                animator.transform.GetChild(1).GetComponent<Animator>().SetBool("Change", false);
            }

            //animCanvas.SetActive(false);
            battleCanvas.SetActive(true);

            player.GetComponent<SpriteRenderer>().sortingOrder = -10;
            player.transform.GetChild(1).GetComponent<AudioSource>().Stop();
            player.transform.GetChild(0).GetComponent<CircleCollider2D>().enabled = false;

            realRooms.SetActive(false);

            battleManager.SpawnCards();

            blackScreen.GetComponent<Animator>().SetBool("Change", true);
            //blackScreen.SetActive(false);

            yield return new WaitForSeconds(0.25f);
            foreach (GameObject button in battleButtons)
            {
                button.GetComponent<SelectButton>().OnExitSelection();
            }
            battleButtons[0].GetComponent<SelectButton>().OnSelection();
            currentButtonIndex = 0;
            battleManager.state = gameStates.choosing;
        }
        
        else if (option == 2)
        {
            yield return new WaitForSeconds(1.2f);

            foreach (GameObject animator in starAnimators)
            {
                animator.GetComponent<Animator>().SetBool("Change", false);
                animator.transform.GetChild(1).GetComponent<Animator>().SetBool("Change", false);
            }

            //animCanvas.SetActive(false);
            battleCanvas.SetActive(true);

            player.GetComponent<SpriteRenderer>().sortingOrder = -10;
            player.transform.GetChild(1).GetComponent<AudioSource>().Stop();
            player.transform.GetChild(0).GetComponent<CircleCollider2D>().enabled = false;

            realRooms.SetActive(false);

            battleManager.SpawnBoss();

            blackScreen.GetComponent<Animator>().SetBool("Change", true);
            //blackScreen.SetActive(false);

            yield return new WaitForSeconds(0.25f);
            foreach (GameObject button in battleButtons)
            {
                button.GetComponent<SelectButton>().OnExitSelection();
            }
            battleButtons[0].GetComponent<SelectButton>().OnSelection();
            currentButtonIndex = 0;
            battleManager.state = gameStates.choosing;
        }
    }

    IEnumerator WaitFinishGame()
    {
        blackScreen.GetComponent<Animator>().SetBool("Change", false);
        battleManager.state = gameStates.stop;

        yield return new WaitForSeconds(0.6f);

        //animCanvas.GetComponent<AudioSource>().Play();

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
        templates.changingRoom = false;
    }

    IEnumerator WaitFinishBossGame()
    {
        blackScreen.GetComponent<Animator>().SetBool("Change", false);
        battleManager.state = gameStates.stop;

        yield return new WaitForSeconds(0.6f);

        //animCanvas.GetComponent<AudioSource>().Play();

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
        templates.changingRoom = false;

        bossBeaten = true;
        StartDialogue(battleManager.Zodiac, thisCharacter);
    }
}
