using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BattleStates 
{
    NONE,
    WAITING,
    CHOOSING,
    ATTACKING,
    DEFENDING,
    TALKING,
    USING_ITEM,
    RUNNING,
    VICTORY,
    DEFEAT
}

public class BattleActionsManager : MonoBehaviour
{
    private BattleInputManager battleInputManager;

    [Header("ENEMY VARIABLES")]
    [SerializeField] private List<GameObject> enemyCards;
    private int amountOfEnemiesAlive;
    private int totalAmoutOfEnemiesInTheBattle;

    public GameObject enemiesParent;
    [HideInInspector] public List<GameObject> enemiesSpawned;

    [SerializeField] private List<GameObject> oneEnemySpawnPosition;
    [SerializeField] private List<GameObject> twoEnemiesSpawnPositions;
    [SerializeField] private List<GameObject> threeEnemiesSpawnPositions;
    [SerializeField] private List<GameObject> fourEnemiesSpawnPositions;

    [SerializeField] private GameObject cancerFigure;
    [SerializeField] private GameObject capricornFigure;
    [HideInInspector] public string zodiacToFight;

    [SerializeField] private List<GameObject> availableNormalAttacks;
    [SerializeField] private List<GameObject> availableBossAttacks;
    [Space(5)]

    [Header("BATTLE STATE VARIABLES")]
    [SerializeField] private GameObject battleArea;

    [HideInInspector] public BattleStates currentBattleState;
    [HideInInspector] public BattleStates lastBattleState;

    [SerializeField] private GameObject winGameCanvas;
    [SerializeField] private Animator winCircleAnimator;

    [SerializeField] private GameObject loseGameCanvas;
    [SerializeField] private List<GameObject> playerBrokenStarPieces;
    [SerializeField] private AudioClip deathSong;
    [Space(5)]

    [Header("DIALOGUE VARIABLES")]
    [SerializeField] private GameObject dialogueArea;
    [SerializeField] private GameObject dialogueCanvas;

    [HideInInspector] public string textToDisplay;
    public GameObject battleDialogueText;

    private void Start()
    {
        battleInputManager = GetComponent<BattleInputManager>();

        // Uncomment for the trial scene only
        SetUpBattle();
    }

    private void Update()
    {
        if (GameMaster.playerLife <= 0 && currentBattleState != BattleStates.DEFEAT)
        {
            LoseGame();
        }
    }

    public void SetUpBattle()
    {
        totalAmoutOfEnemiesInTheBattle = Random.Range(1, 5);
        amountOfEnemiesAlive = totalAmoutOfEnemiesInTheBattle;

        if (totalAmoutOfEnemiesInTheBattle == 1)
        {
            battleDialogueText.GetComponent<Text>().text = "    YOU ARE UP AGAINST " + totalAmoutOfEnemiesInTheBattle + " ENEMY";
        }
        else
        {
            battleDialogueText.GetComponent<Text>().text = "    YOU ARE UP AGAINST " + totalAmoutOfEnemiesInTheBattle + " ENEMIES";
        }

        textToDisplay = battleDialogueText.GetComponent<Text>().text;
        enemiesSpawned = new List<GameObject>();
    }

    public void SetUpBossBattle(string zodiac)
    {
        totalAmoutOfEnemiesInTheBattle = 1;
        amountOfEnemiesAlive = totalAmoutOfEnemiesInTheBattle;
        zodiacToFight = zodiac;

        battleDialogueText.GetComponent<Text>().text = "    YOU ARE UP AGAINST " + zodiac;

        textToDisplay = battleDialogueText.GetComponent<Text>().text;
        enemiesSpawned = new List<GameObject>();
    }

    public void ManageSpawnBasicEnemies()
    {
        switch(totalAmoutOfEnemiesInTheBattle)
        {
            case 1:
                PlaceEnemiesInScreen(oneEnemySpawnPosition);
                break;

            case 2:
                PlaceEnemiesInScreen(twoEnemiesSpawnPositions);
                break;

            case 3:
                PlaceEnemiesInScreen(threeEnemiesSpawnPositions);
                break;

            case 4:
                PlaceEnemiesInScreen(fourEnemiesSpawnPositions);
                break;

            default:
                break;
        }
    }

    public void ManageSpawnBoss()
    {
        GameObject oneSpawnerObject = oneEnemySpawnPosition[0];
        if (zodiacToFight == "CANCER")
        {
            GameObject bossInstance = Instantiate(cancerFigure, oneSpawnerObject.transform.position,
                                                  cancerFigure.transform.rotation, enemiesParent.transform);
            enemiesSpawned.Add(bossInstance);
        }
        else if (zodiacToFight == "CAPRICORN")
        {
            GameObject bossInstance = Instantiate(capricornFigure, oneSpawnerObject.transform.position,
                                                  capricornFigure.transform.rotation, enemiesParent.transform);
            enemiesSpawned.Add(bossInstance);
        }
    }

    // Attack functions -------------------------------------------------------
    public void AttackAction(List<GameObject> enemyTexts, int attackedEnemyIndex)
    {
        SwapToBigDialogue(enemyTexts);

        string attackedEnemyName = enemyTexts[attackedEnemyIndex].GetComponent<Text>().text;
        CalculateAndDisplayDamageInformation(attackedEnemyIndex, attackedEnemyName);

        lastBattleState = currentBattleState;
        currentBattleState = BattleStates.DEFENDING;
    }

    public void UpdateEnemiesInBattle()
    {
        int enemyToRemoveIndex = RemoveAndGetDeadEnemyFromBattle();

        bool playerWon = amountOfEnemiesAlive == 0;

        if (playerWon)
        {
            currentBattleState = BattleStates.VICTORY;
        }
        else
        {
            bool enemyIsNotTheLast = enemyToRemoveIndex != amountOfEnemiesAlive;
            if (enemyIsNotTheLast)
            {
                MoveTextsToAppearContiguous(enemyToRemoveIndex, battleInputManager.textsForEnemiesInBattle);
            }

            int lastEnemyText = battleInputManager.textsForEnemiesInBattle.Count - 1;
            battleInputManager.textsForEnemiesInBattle[lastEnemyText].GetComponent<Text>().enabled = true;
            battleInputManager.textsForEnemiesInBattle[lastEnemyText].transform.GetChild(0).gameObject.SetActive(false);
            battleInputManager.textsForEnemiesInBattle[lastEnemyText].transform.GetChild(1).gameObject.SetActive(false);

            ResetBattleArea();
        }
    }

    public void TriggerEnemyAttack()
    {
        dialogueArea.SetActive(false);
        battleArea.SetActive(true);

        Animator battleAreaAnimator = battleArea.transform.GetChild(0).GetComponent<Animator>();
        battleAreaAnimator.SetBool("Expand", true);

        battleInputManager.playerStarObject.transform.position = battleInputManager.playerStarInitialPosition.transform.position;

        StartCoroutine(nameof(InitiateAttack));
    }

    void ResetBattleArea()
    {
        battleDialogueText.GetComponent<Text>().text = textToDisplay;

        // Disable enemy texts
        battleInputManager.currentHoveredTextIndex = 0;
        battleInputManager.textsForEnemiesInBattle[battleInputManager.currentHoveredTextIndex].GetComponent<Text>().enabled = true;
        battleInputManager.textsForEnemiesInBattle[battleInputManager.currentHoveredTextIndex].transform.GetChild(0).gameObject.SetActive(false);
        battleInputManager.textsForEnemiesInBattle[battleInputManager.currentHoveredTextIndex].transform.GetChild(1).gameObject.SetActive(false);

        SwapToBigDialogue(battleInputManager.textsForEnemiesInBattle);
    }

    // Listen functions -------------------------------------------------------
    public void TalkAction(List<GameObject> enemyTexts)
    {
        SwapToBigDialogue(enemyTexts);

        bool isZodiacFight = zodiacToFight != "";
        if (isZodiacFight)
        {
            battleDialogueText.GetComponent<Text>().text = "    THEY ARE BUSY FIGHTING, CAN'T TALK NOW";
        }
        else
        {
            battleDialogueText.GetComponent<Text>().text = "    YOU CAN'T TALK TO AN NPC";
        }

        lastBattleState = currentBattleState;
        currentBattleState = BattleStates.WAITING; // TODO: Why after talking is not DEFENDING but WAITING instead?
    }

    // Items functions --------------------------------------------------------
    public void UseItemAction(List<GameObject> itemTexts, int itemUsedIndex)
    {
        SwapToBigDialogue(itemTexts);

        ItemObject itemUsed = GameMaster.inventory[itemUsedIndex];
        int itemEffectStrength = (itemUsed.Type == ObjectTypes.HEALTH) ? itemUsed.Level * 5 : itemUsed.Level;
        switch (itemUsed.Type)
        {
            case ObjectTypes.HEALTH:
                UseHealingItem(itemUsed, itemEffectStrength);
                break;

            case ObjectTypes.ATTACK:
                UseBuffingItem(itemUsed, itemEffectStrength, "ATTACK");
                break;

            case ObjectTypes.DEFENSE:
                UseBuffingItem(itemUsed, itemEffectStrength, "DEFENSE");
                break;

            case ObjectTypes.SPEED:
                UseBuffingItem(itemUsed, itemEffectStrength, "SPEED");
                break;

            default:
                break;
        }
        itemUsed.Consumed = true;

        UpdateItemsAvailable();

        lastBattleState = currentBattleState;
        currentBattleState = BattleStates.DEFENDING;
    }

    void UpdateItemsAvailable()
    {
        int itemToRemoveIndex = RemoveAndGetConsumedItem();

        bool itemIsNotTheLast = itemToRemoveIndex != GameMaster.inventory.Count;
        if (itemIsNotTheLast)
        {
            MoveTextsToAppearContiguous(itemToRemoveIndex, battleInputManager.textsForItemsToBeUsed);
        }

        int lastItemText = battleInputManager.textsForItemsToBeUsed.Count - 1;
        battleInputManager.textsForItemsToBeUsed[lastItemText].GetComponent<Text>().enabled = true;
        battleInputManager.textsForItemsToBeUsed[lastItemText].transform.GetChild(0).gameObject.SetActive(false);
        battleInputManager.textsForItemsToBeUsed[lastItemText].transform.GetChild(1).gameObject.SetActive(false);

        ResetBattleArea();
    }

    // Run functions ----------------------------------------------------------
    public void RunAction()
    {
        bool isZodiacFight = zodiacToFight != "";
        if (isZodiacFight)
        {
            battleDialogueText.GetComponent<Text>().text = "    YOU CAN NOT RUN FROM A ZODIAC BATTLE";
        }
        else if (GameMaster.playerSpeed >= 5)
        {
            battleDialogueText.GetComponent<Text>().text = "    YOU WERE ABLE TO RUN FROM THE BATTLE";
        }
        else
        {
            battleDialogueText.GetComponent<Text>().text = "    YOU WERE NOT ABLE TO RUN, NOT ENOUGH SPEED";
        }

        lastBattleState = currentBattleState;
        currentBattleState = BattleStates.WAITING;
    }

    // Battle/Game outcome functions -----------------------------------------------
    public void WinBattle()
    {
        bool isZodiacFight = zodiacToFight != "";
        if (!isZodiacFight)
        {
            GetMoneyAfterBattle();

            int chanceToGetHealingPotion = Random.Range(0, 10);
            if (chanceToGetHealingPotion <= 1) // 20% chance for now
            {
                GetHealingPotion();
            }
        }
        else
        {
            GetMoneyAfterGame();
        }

        battleDialogueText.GetComponent<Text>().text = textToDisplay;
    }

    public void WinGame()
    {
        currentBattleState = BattleStates.VICTORY;
        GameMaster.attempts++;
        GameMaster.successfulAttemps++;
        battleInputManager.dungeonMinimapRoomsParent.SetActive(false);

        battleInputManager.overworldPlayer.GetComponent<SpriteRenderer>().enabled = false;
        battleInputManager.overworldPlayer.transform.GetChild(0).gameObject.SetActive(true);
        GameMaster.temperanceIndex++;

        StartCoroutine(nameof(WaitWinAnim));
        GameMaster.Reset();
    }

    void LoseGame()
    {
        currentBattleState = BattleStates.DEFEAT;
        GameMaster.attempts++;
        dialogueCanvas.SetActive(false);

        if (zodiacToFight == "CAPRICORN")
        {
            GameMaster.capricornIndex--;
        }
        else if (zodiacToFight == "CANCER")
        {
            GameMaster.cancerIndex--;
        }

        StopBattleAndHideUI();
        DisplayDeathStarAnimation();

        StartCoroutine(nameof(WaitLoseAnim));
        GameMaster.Reset();
    }

    void SwapToBigDialogue(List<GameObject> textsToDeactivate)
    {
        foreach (GameObject text in textsToDeactivate)
        {
            text.SetActive(false);
        }

        battleDialogueText.SetActive(true);
    }

    void PlaceEnemiesInScreen(List<GameObject> cardsPositions)
    {
        foreach(GameObject card in cardsPositions)
        {
            int numberCard = Random.Range(0, enemyCards.Count);
            GameObject newEnemyCard = Instantiate(enemyCards[numberCard], card.transform.position,
                                                  enemyCards[numberCard].transform.rotation, enemiesParent.transform);
            enemiesSpawned.Add(newEnemyCard);
        }
    }

    void CalculateAndDisplayDamageInformation(int attackedEnemyIndex, string attackedEnemyName)
    {
        int randomDamage = Random.Range(10, 21);

        battleDialogueText.GetComponent<Text>().text = "    YOU DEALT " + randomDamage + " DAMAGE POINTS TO " + attackedEnemyName;

        if (enemiesSpawned[attackedEnemyIndex].GetComponent<EnemyCard>().Life - randomDamage <= 0)
        {
            battleDialogueText.GetComponent<Text>().text += ", YOU KILLED " + attackedEnemyName;
        }

        enemiesSpawned[attackedEnemyIndex].GetComponent<EnemyCard>().Life -= randomDamage;
    }
    
    int RemoveAndGetDeadEnemyFromBattle()
    {
        int enemyToRemoveIndex = 0;
        for (int i = 0; i < amountOfEnemiesAlive; i++)
        {
            if (enemiesSpawned[i].GetComponent<EnemyCard>().Life <= 0)
            {
                enemyToRemoveIndex = i;
                break;
            }
        }

        amountOfEnemiesAlive--;

        enemiesSpawned.RemoveAt(enemyToRemoveIndex);
        battleInputManager.textsForEnemiesInBattle[amountOfEnemiesAlive].GetComponent<Text>().text = "";
        battleInputManager.textsForEnemiesInBattle[amountOfEnemiesAlive].transform.GetChild(1).GetComponent<Text>().text = "";

        if (amountOfEnemiesAlive == 1)
        {
            textToDisplay = "    YOU ARE UP AGAINST " + amountOfEnemiesAlive + " ENEMY";
        }
        else
        {
            textToDisplay = "    YOU ARE UP AGAINST " + amountOfEnemiesAlive + " ENEMIES";
        }

        return enemyToRemoveIndex;
    }

    void MoveTextsToAppearContiguous(int indexToRemove, List<GameObject> listToRemove)
    {
        for (int i = indexToRemove; i < listToRemove.Count - 1; i++)
        {
            Text currentText = listToRemove[i].GetComponent<Text>();
            Text currentTextChild = listToRemove[i].transform.GetChild(1).GetComponent<Text>();

            Text nextText = listToRemove[i + 1].GetComponent<Text>();

            currentText.text = nextText.text;
            currentText.color = nextText.color;
            currentTextChild.text = "    " + nextText.text;
            currentTextChild.color = nextText.color;
        }
    }

    void UseHealingItem(ItemObject itemUsed, int itemEffectStrength)
    {
        battleDialogueText.GetComponent<Text>().text = "    YOU USED " + itemUsed.ObjectName + " LVL." + itemUsed.Level +
                                                       " YOU HEALED " + itemEffectStrength + " HP";

        GameMaster.playerLife = Mathf.Min(GameMaster.maxPlayerLife, GameMaster.playerLife + itemEffectStrength);
    }

    void UseBuffingItem(ItemObject itemUsed, int itemEffectStrength, string buffedStat)
    {
        battleDialogueText.GetComponent<Text>().text = "    YOU USED " + itemUsed.ObjectName + " LVL." + itemUsed.Level +
                                                       " YOUR " + buffedStat + " INCREASED BY " + itemEffectStrength;

        switch (buffedStat)
        {
            case "ATTACK":
                // TODO: Increase attack value
                break;

            case "DEFENSE":
                // TODO: Increase defense value
                break;

            case "SPEED":
                GameMaster.playerSpeed += itemEffectStrength;
                break;

            default:
                break;
        }
    }

    int RemoveAndGetConsumedItem()
    {
        int itemToRemoveIndex = 0;
        for (int i = 0; i < GameMaster.inventory.Count; i++)
        {
            if (GameMaster.inventory[i].Consumed)
            {
                itemToRemoveIndex = i;
                break;
            }
        }

        GameMaster.inventory.RemoveAt(itemToRemoveIndex);
        battleInputManager.textsForItemsToBeUsed[GameMaster.inventory.Count - 1].GetComponent<Text>().text = "";
        battleInputManager.textsForItemsToBeUsed[GameMaster.inventory.Count - 1].transform.GetChild(1).GetComponent<Text>().text = "";

        return itemToRemoveIndex;
    }

    void GetMoneyAfterBattle()
    {
        int moneyWon = Random.Range(1, 6) * totalAmoutOfEnemiesInTheBattle;
        GameMaster.runMoney += moneyWon;

        currentBattleState = BattleStates.VICTORY;
        if (moneyWon == 1)
        {
            textToDisplay = "    ALL ENEMIES DEFEATED, YOU RECIEVED " + moneyWon + " COIN FOR THE VICTORY!\n";
        }
        else
        {
            textToDisplay = "    ALL ENEMIES DEFEATED, YOU RECIEVED " + moneyWon + " COINS FOR THE VICTORY!\n";
        }
    }

    void GetMoneyAfterGame()
    {
        int moneyWon = 50;
        GameMaster.runMoney += moneyWon;
        GameMaster.totalMoney += GameMaster.runMoney;

        currentBattleState = BattleStates.VICTORY;
        textToDisplay = "    BOSS DEFEATED, YOU RECIEVED " + moneyWon + " COINS FOR THE VICTORY!\n";
    }

    void GetHealingPotion()
    {
        textToDisplay += "YOU ALSO RECIEVED A LVL." + GameMaster.currentLevel + " HEALTH POTION!";

        if (GameMaster.inventory.Count < GameMaster.inventoryMaxSpace)
        {
            GameMaster.inventory.Add(new ItemObject("Health Potion", ObjectTypes.HEALTH, GameMaster.currentLevel));
        }
        else
        {
            textToDisplay += " (but you have no space in your inventory).";
        }
    }

    void StopBattleAndHideUI()
    {
        for (int i = 1; i < battleInputManager.battleCanvas.transform.childCount; i++)
        {
            if (!battleInputManager.battleCanvas.transform.GetChild(i).gameObject.CompareTag("Player"))
            {
                battleInputManager.battleCanvas.transform.GetChild(i).gameObject.SetActive(false);
            }
            else
            {
                for (int j = 0; j < battleInputManager.battleCanvas.transform.GetChild(i).gameObject.transform.childCount; j++)
                {
                    if (!battleInputManager.battleCanvas.transform.GetChild(i).gameObject.transform.GetChild(j).CompareTag("Player"))
                    {
                        battleInputManager.battleCanvas.transform.GetChild(i).gameObject.transform.GetChild(j).gameObject.SetActive(false);
                    }
                }
            }
        }
        battleInputManager.battleCanvas.GetComponent<AudioSource>().Stop();
    }

    void DisplayDeathStarAnimation()
    {
        battleInputManager.playerStarObject.GetComponent<SpriteRenderer>().enabled = false;

        int direction = 0;
        Vector2[] pushStarDirections = new Vector2[]
        {
            new Vector2(0, 1.25f),
            new Vector2(1.25f, 1.5f),
            new Vector2(0.5f, 0.5f),
            new Vector2(-0.5f, 0.5f),
            new Vector2(-1.25f, 1.5f)
        };

        foreach (GameObject piece in playerBrokenStarPieces)
        {
            piece.SetActive(true);
            piece.GetComponent<Rigidbody2D>().AddForce(pushStarDirections[direction] * 2, ForceMode2D.Impulse);
            piece.GetComponent<Rigidbody2D>().AddTorque(360, ForceMode2D.Impulse);
            direction++;
        }
    }

    IEnumerator WaitWinAnim()
    {
        winCircleAnimator.SetBool("Show", false);
        yield return new WaitForSeconds(1.75f);

        dialogueCanvas.SetActive(false);
        winGameCanvas.SetActive(true);
        battleInputManager.overworldPlayer.GetComponent<AudioSource>().clip = deathSong;
        battleInputManager.overworldPlayer.GetComponent<AudioSource>().Play();

        yield return new WaitForSeconds(1.0f);
        winGameCanvas.transform.GetChild(3).gameObject.GetComponent<Image>().enabled = false;
    }

    IEnumerator WaitLoseAnim()
    {
        yield return new WaitForSeconds(0.55f);

        battleInputManager.overworldPlayer.GetComponent<AudioSource>().clip = deathSong;
        battleInputManager.overworldPlayer.GetComponent<AudioSource>().Play();

        yield return new WaitForSeconds(0.65f);
        loseGameCanvas.SetActive(true);

        yield return new WaitForSeconds(1.0f);
        loseGameCanvas.transform.GetChild(3).gameObject.GetComponent<Image>().enabled = false;
    }

    IEnumerator InitiateAttack()
    {
        yield return new WaitForSeconds(1f);

        bool isNotZodiacFight = zodiacToFight == "";

        if (isNotZodiacFight)
        {
            int numAttack = Random.Range(0, availableNormalAttacks.Count);
            availableNormalAttacks[numAttack].SetActive(true);
            bool isMeteoritesAttack = availableNormalAttacks[numAttack].GetComponent<ActivateChildren>() != null;

            if (isMeteoritesAttack)
            {
                availableNormalAttacks[numAttack].GetComponent<ActivateChildren>().ActivateMeteos();
                yield return new WaitForSeconds(11.0f);

                availableNormalAttacks[numAttack].GetComponent<ActivateChildren>().DeactivateMeteos();
            }
            else
            {
                // TODO: Check if this time is correct for all the attacks, and, if not, adjust it
                yield return new WaitForSeconds(5.0f);

                availableNormalAttacks[numAttack].SetActive(false);
            }
        }
        else
        {
            int numAttack = Random.Range(0, availableBossAttacks.Count);
            availableBossAttacks[numAttack].SetActive(true);

            StartCoroutine(nameof(ManageBossStarAttacks), numAttack);

            availableBossAttacks[numAttack].SetActive(false);
        }

        StartCoroutine(nameof(EndBattle));
    }

    IEnumerator ManageBossStarAttacks(int numAttack)
    {
        bool isLongerAttack = numAttack == 2;
        int starsToSpawn = isLongerAttack ? 5 : 3;

        int activatedOnes = 0;
        while (activatedOnes < starsToSpawn)
        {
            int activate = Random.Range(0, availableBossAttacks[numAttack].transform.childCount);

            if (!availableBossAttacks[numAttack].transform.GetChild(activate).gameObject.activeSelf)
            {
                activatedOnes++;
                availableBossAttacks[numAttack].transform.GetChild(activate).gameObject.SetActive(true);
                yield return new WaitForSeconds(1.55f);
            }
        }

        for (int i = 0; i < availableBossAttacks[numAttack].transform.childCount; i++)
        {
            availableBossAttacks[numAttack].transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    IEnumerator EndBattle()
    {
        if (currentBattleState != BattleStates.DEFEAT)
        {
            currentBattleState = BattleStates.NONE;
        }
        battleInputManager.playerStarCanMove = false;
        battleArea.transform.GetChild(0).GetComponent<Animator>().SetBool("Expand", false);

        yield return new WaitForSeconds(1f);
        if (currentBattleState != BattleStates.DEFEAT)
        {
            dialogueArea.SetActive(true);
            battleArea.SetActive(false);
        }
        ResetBattleArea();
        battleInputManager.battleActionButtons[battleInputManager.currentActionButtonIndex].GetComponent<ManageButtonSelection>().OnExitSelection();
        battleInputManager.currentActionButtonIndex = 0;
        battleInputManager.battleActionButtons[battleInputManager.currentActionButtonIndex].GetComponent<ManageButtonSelection>().OnSelection();

        yield return new WaitForSeconds(0.25f);
        if (currentBattleState != BattleStates.DEFEAT)
        {
            currentBattleState = BattleStates.CHOOSING;
        }
    }
}