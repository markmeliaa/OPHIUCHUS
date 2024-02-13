using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameStates 
{
    NONE,
    TALKING,
    WAITING,
    CHOOSING,
    ATTACKING,
    DEFENDING,
    USING_ITEM,
    RUNNING,
    VICTORY,
    DEFEAT
}

public class BattleManager : MonoBehaviour
{
    private ButtonManager buttonManager;

    [Header("ENEMY RELATED VARIABLES")]
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

    [Header("BATTLE STATE RELATED VARIABLES")]
    [SerializeField] private GameObject battleArea;

    [HideInInspector] public GameStates currentBattleState;
    [HideInInspector] public GameStates lastBattleState;

    [SerializeField] private GameObject winGameCanvas;
    [SerializeField] private Animator winCircleAnimator;

    [SerializeField] private GameObject loseGameCanvas;
    [SerializeField] private List<GameObject> playerBrokenStarPieces;
    [SerializeField] private AudioClip deathSong;
    [Space(5)]

    [Header("DIALOGUE RELATED VARIABLES")]
    [SerializeField] private GameObject dialogueArea;
    [SerializeField] private GameObject dialogueCanvas;

    [HideInInspector] public string textToDisplay;
    public GameObject battleDialogueText;

    private void Start()
    {
        buttonManager = GetComponent<ButtonManager>();

        // Uncomment for the trial scene only
        // SetUpBattle();
    }

    private void Update()
    {
        if (GameMaster.playerLife <= 0 && currentBattleState != GameStates.DEFEAT)
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

    // Attack functions
    public void AttackAction(List<GameObject> enemyTexts, int attackedEnemyIndex)
    {
        SwapToBigDialogue(enemyTexts);

        string attackedEnemyName = enemyTexts[attackedEnemyIndex].GetComponent<Text>().text;
        CalculateAndDisplayDamageInformation(attackedEnemyIndex, attackedEnemyName);

        currentBattleState = GameStates.DEFENDING;
        lastBattleState = GameStates.ATTACKING;
    }

    public void UpdateEnemiesInBattle()
    {
        int enemyToRemoveIndex = RemoveAndGetDeadEnemyFromBattle();

        bool playerWon = amountOfEnemiesAlive == 0;

        if (playerWon)
        {
            currentBattleState = GameStates.VICTORY;
        }
        else
        {
            bool enemyIsNotTheLast = enemyToRemoveIndex != amountOfEnemiesAlive;
            if (enemyIsNotTheLast)
            {
                MoveEnemyTextsToAppearContiguous(enemyToRemoveIndex);
            }

            buttonManager.enemyTexts[enemyToRemoveIndex].GetComponent<Text>().enabled = true;
            buttonManager.enemyTexts[enemyToRemoveIndex].transform.GetChild(0).gameObject.SetActive(false);
            buttonManager.enemyTexts[enemyToRemoveIndex].transform.GetChild(1).gameObject.SetActive(false);
            buttonManager.currentTextIndex = 0;

            ResetBattleArea();
        }
    }

    public void StartEnemyAttack()
    {
        dialogueArea.SetActive(false);
        battleArea.SetActive(true);

        Animator battleAreaAnimator = battleArea.transform.GetChild(0).GetComponent<Animator>();
        battleAreaAnimator.SetBool("Expand", true);

        buttonManager.playerStar.transform.position = buttonManager.playerStarBasePosition.transform.position;

        StartCoroutine(nameof(InitiateAttack));
    }

    void ResetBattleArea()
    {
        battleDialogueText.GetComponent<Text>().text = textToDisplay;

        // Disable enemy texts
        buttonManager.enemyTexts[buttonManager.currentTextIndex].GetComponent<Text>().enabled = true;
        buttonManager.enemyTexts[buttonManager.currentTextIndex].transform.GetChild(0).gameObject.SetActive(false);
        buttonManager.enemyTexts[buttonManager.currentTextIndex].transform.GetChild(1).gameObject.SetActive(false);

        SwapToBigDialogue(buttonManager.enemyTexts);
    }

    // Listen functions
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

        currentBattleState = GameStates.WAITING;
        lastBattleState = GameStates.TALKING;
    }

    // Items functions
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

        RedistributeItemTexts();

        currentBattleState = GameStates.DEFENDING;
        lastBattleState = GameStates.USING_ITEM;
    }

    void RedistributeItemTexts()
    {
        int deleteTarget = 0;
        for (int i = 0; i < GameMaster.inventory.Count; i++)
        {
            if (GameMaster.inventory[i].Consumed)
            {
                deleteTarget = i;
                break;
            }
        }

        if (deleteTarget != GameMaster.inventory.Count - 1)
        {
            for (int i = deleteTarget; i < GameMaster.inventory.Count; i++)
            {
                buttonManager.itemTexts[i].GetComponent<Text>().text = buttonManager.itemTexts[i + 1].GetComponent<Text>().text;
                buttonManager.itemTexts[i].GetComponent<Text>().color = buttonManager.itemTexts[i + 1].GetComponent<Text>().color;
                buttonManager.itemTexts[i].transform.GetChild(1).GetComponent<Text>().text = "    " + buttonManager.itemTexts[i + 1].GetComponent<Text>().text;
                buttonManager.itemTexts[i].transform.GetChild(1).GetComponent<Text>().color = buttonManager.itemTexts[i + 1].GetComponent<Text>().color;
            }

            GameMaster.inventory.RemoveAt(deleteTarget);
            //buttonManager.itemTexts.RemoveAt(GameMaster.inventory.Count);

            buttonManager.itemTexts[GameMaster.inventory.Count - 1].GetComponent<Text>().text = "";
            buttonManager.itemTexts[GameMaster.inventory.Count - 1].transform.GetChild(1).GetComponent<Text>().text = "";

            buttonManager.itemTexts[deleteTarget].GetComponent<Text>().enabled = true;
            buttonManager.itemTexts[deleteTarget].transform.GetChild(0).gameObject.SetActive(false);
            buttonManager.itemTexts[deleteTarget].transform.GetChild(1).gameObject.SetActive(false);
            buttonManager.currentTextIndex = 0;
            buttonManager.itemTexts[buttonManager.currentTextIndex].GetComponent<Text>().enabled = false;
            buttonManager.itemTexts[buttonManager.currentTextIndex].transform.GetChild(0).gameObject.SetActive(true);
            buttonManager.itemTexts[buttonManager.currentTextIndex].transform.GetChild(1).gameObject.SetActive(true);
        }

        else
        {
            buttonManager.itemTexts[GameMaster.inventory.Count - 1].GetComponent<Text>().text = "";
            buttonManager.itemTexts[GameMaster.inventory.Count - 1].transform.GetChild(1).GetComponent<Text>().text = "";

            GameMaster.inventory.RemoveAt(GameMaster.inventory.Count - 1);
            //buttonManager.itemTexts.RemoveAt(GameMaster.inventory.Count);

            buttonManager.itemTexts[GameMaster.inventory.Count].GetComponent<Text>().enabled = true;
            buttonManager.itemTexts[GameMaster.inventory.Count].transform.GetChild(0).gameObject.SetActive(false);
            buttonManager.itemTexts[GameMaster.inventory.Count].transform.GetChild(1).gameObject.SetActive(false);
            buttonManager.currentTextIndex = 0;
            buttonManager.itemTexts[buttonManager.currentTextIndex].GetComponent<Text>().enabled = false;
            buttonManager.itemTexts[buttonManager.currentTextIndex].transform.GetChild(0).gameObject.SetActive(true);
            buttonManager.itemTexts[buttonManager.currentTextIndex].transform.GetChild(1).gameObject.SetActive(true);
        }
    }

    // Run functions
    public void RunAction()
    {
        if (zodiacToFight != "")
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

        currentBattleState = GameStates.WAITING;
        lastBattleState = GameStates.RUNNING;
    }

    // Battle outcome functions
    public void Win()
    {
        if (zodiacToFight == "")
        {
            int moneyWon = Random.Range(1, 6) * totalAmoutOfEnemiesInTheBattle;
            GameMaster.runMoney += moneyWon;

            currentBattleState = GameStates.DEFEAT;
            if (moneyWon == 1)
            {
                textToDisplay = "    ALL ENEMIES DEFEATED, YOU RECIEVED " + moneyWon + " COIN FOR THE VICTORY!\n";
            }
            else
            {
                textToDisplay = "    ALL ENEMIES DEFEATED, YOU RECIEVED " + moneyWon + " COINS FOR THE VICTORY!\n";
            }
        }
        
        else
        {
            int moneyWon = 50;
            GameMaster.runMoney += moneyWon;
            GameMaster.totalMoney += GameMaster.runMoney;

            currentBattleState = GameStates.DEFEAT;
            textToDisplay = "    BOSS DEFEATED, YOU RECIEVED " + moneyWon + " COINS FOR THE VICTORY!\n";
        }

        if (Random.Range(0,3) == 2)
        {
            textToDisplay += "YOU ALSO RECIEVED A LVL.1 HEALTH POTION!";

            if (GameMaster.inventory.Count < 8)
            {
                GameMaster.inventory.Add(new ItemObject("Health Potion", ObjectTypes.HEALTH, 1));
            }

            else
            {
                textToDisplay += " (but you have no space in your inventory).";
            }
        }

        battleDialogueText.GetComponent<Text>().text = textToDisplay;
    }

    void LoseGame()
    {
        currentBattleState = GameStates.DEFEAT;
        GameMaster.attempts++;
        dialogueCanvas.SetActive(false);

        //Debug.Log("You Died");

        if (zodiacToFight == "CAPRICORN")
        {
            GameMaster.capricornIndex--;
        }
        else if (zodiacToFight == "CANCER")
        {
            GameMaster.cancerIndex--;
        }

        for (int i = 1; i < buttonManager.battleCanvas.transform.childCount; i++)
        {
            if (!buttonManager.battleCanvas.transform.GetChild(i).gameObject.CompareTag("Player"))
                buttonManager.battleCanvas.transform.GetChild(i).gameObject.SetActive(false);
            else
            {
                for (int j = 0; j < buttonManager.battleCanvas.transform.GetChild(i).gameObject.transform.childCount; j++)
                {
                    if (!buttonManager.battleCanvas.transform.GetChild(i).gameObject.transform.GetChild(j).CompareTag("Player"))
                        buttonManager.battleCanvas.transform.GetChild(i).gameObject.transform.GetChild(j).gameObject.SetActive(false);
                }
            }
        }
        buttonManager.battleCanvas.GetComponent<AudioSource>().Stop();

        buttonManager.playerStar.GetComponent<SpriteRenderer>().enabled = false;

        int direction = 0;
        Vector2[] pushStarDirections = new Vector2[] 
        { 
            new Vector2(0, 1.25f), 
            new Vector2(1.25f, 1.5f), 
            new Vector2(0.5f, 0.5f), 
            new Vector2(-0.5f, 0.5f), 
            new Vector2(-1.25f, 1.5f)
        };
        
        foreach(GameObject piece in playerBrokenStarPieces)
        {
            piece.SetActive(true);
            piece.GetComponent<Rigidbody2D>().AddForce(pushStarDirections[direction] * 2, ForceMode2D.Impulse);
            piece.GetComponent<Rigidbody2D>().AddTorque(360, ForceMode2D.Impulse);
            direction++;
        }
        StartCoroutine(nameof(WaitAnim));

        GameMaster.temperanceIndex = 1;
        GameMaster.Reset();
    }

    public void WinGame()
    {
        GameMaster.attempts++;
        GameMaster.successfulAttemps++;
        buttonManager.miniMap.SetActive(false);

        buttonManager.player.GetComponent<SpriteRenderer>().enabled = false;
        buttonManager.player.transform.GetChild(3).gameObject.SetActive(true);
        GameMaster.temperanceIndex = 2;

        StartCoroutine(nameof(WaitWinAnim));
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
        buttonManager.enemyTexts[amountOfEnemiesAlive].GetComponent<Text>().text = "";
        buttonManager.enemyTexts[amountOfEnemiesAlive].transform.GetChild(1).GetComponent<Text>().text = "";

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

    void MoveEnemyTextsToAppearContiguous(int enemyToRemoveIndex)
    {
        for (int i = enemyToRemoveIndex; i < amountOfEnemiesAlive; i++)
        {
            Text currentEnemyText = buttonManager.enemyTexts[i].GetComponent<Text>();
            Text currentEnemyTextChild = buttonManager.enemyTexts[i].transform.GetChild(1).GetComponent<Text>();

            Text nextEnemyText = buttonManager.enemyTexts[i + 1].GetComponent<Text>();

            currentEnemyText.text = nextEnemyText.text;
            currentEnemyText.color = nextEnemyText.color;
            currentEnemyTextChild.text = "    " + nextEnemyText.text;
            currentEnemyTextChild.color = nextEnemyText.color;
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

    IEnumerator WaitAnim()
    {
        yield return new WaitForSeconds(0.55f);

        buttonManager.player.transform.GetChild(1).GetComponent<AudioSource>().clip = deathSong;
        buttonManager.player.transform.GetChild(1).GetComponent<AudioSource>().Play();

        yield return new WaitForSeconds(0.65f);
        loseGameCanvas.SetActive(true);

        yield return new WaitForSeconds(1.0f);
        loseGameCanvas.transform.GetChild(3).gameObject.GetComponent<Image>().enabled = false;
    }

    IEnumerator WaitWinAnim()
    {
        winCircleAnimator.SetBool("Show", false);
        yield return new WaitForSeconds(1.75f);

        dialogueCanvas.SetActive(false);
        winGameCanvas.SetActive(true);
        buttonManager.player.transform.GetChild(1).GetComponent<AudioSource>().clip = deathSong;
        buttonManager.player.transform.GetChild(1).GetComponent<AudioSource>().Play();

        yield return new WaitForSeconds(1.0f);
        winGameCanvas.transform.GetChild(3).gameObject.GetComponent<Image>().enabled = false;
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
                yield return new WaitForSeconds(11f);

                availableNormalAttacks[numAttack].GetComponent<ActivateChildren>().DeactivateMeteos();
            }
            else
            {
                // TODO: Check what this waitTime did (it waited specific time for each attack), important to check and keep
                // yield return new WaitForSeconds(attacks[numAttack].transform.GetChild(0).GetComponent<DealDamageToPlayer>().waitTime);
                
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
        if (currentBattleState != GameStates.DEFEAT)
        {
            currentBattleState = GameStates.NONE;
        }
        buttonManager.playerCanMove = false;
        battleArea.transform.GetChild(0).GetComponent<Animator>().SetBool("Expand", false);

        yield return new WaitForSeconds(1f);
        if (currentBattleState != GameStates.DEFEAT)
        {
            dialogueArea.SetActive(true);
            battleArea.SetActive(false);
        }
        ResetBattleArea();
        buttonManager.battleButtons[buttonManager.currentButtonIndex].GetComponent<ManageButtonSelection>().OnExitSelection();
        buttonManager.currentButtonIndex = 0;
        buttonManager.battleButtons[buttonManager.currentButtonIndex].GetComponent<ManageButtonSelection>().OnSelection();

        yield return new WaitForSeconds(0.25f);
        if (currentBattleState != GameStates.DEFEAT)
        {
            currentBattleState = GameStates.CHOOSING;
        }
    }
}