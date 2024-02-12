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
        //amountSpawn = 2;
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

    public void SpawnCards()
    {
        // Only one card spawns
        if (amountOfEnemiesAlive == 1)
        {
            int numberCard = Random.Range(0, enemyCards.Count);
            GameObject oneSpawnerObject = oneEnemySpawnPosition[0];
            enemiesSpawned.Add(Instantiate(enemyCards[numberCard], oneSpawnerObject.transform.position, enemyCards[numberCard].transform.rotation, enemiesParent.transform));
        }

        // Two cards spawn
        else if (amountOfEnemiesAlive == 2)
        {
            foreach (GameObject spawn in twoEnemiesSpawnPositions)
            {
                int numberCard = Random.Range(0, enemyCards.Count);
                enemiesSpawned.Add(Instantiate(enemyCards[numberCard], spawn.transform.position, enemyCards[numberCard].transform.rotation, enemiesParent.transform));
            }
        }

        // Three cards spawn
        else if (amountOfEnemiesAlive == 3)
        {
            foreach (GameObject spawn in threeEnemiesSpawnPositions)
            {
                int numberCard = Random.Range(0, enemyCards.Count);
                enemiesSpawned.Add(Instantiate(enemyCards[numberCard], spawn.transform.position, enemyCards[numberCard].transform.rotation, enemiesParent.transform));
            }
        }

        // Four cards spawn
        else
        {
            foreach (GameObject spawn in fourEnemiesSpawnPositions)
            {
                int numberCard = Random.Range(0, enemyCards.Count);
                enemiesSpawned.Add(Instantiate(enemyCards[numberCard], spawn.transform.position, enemyCards[numberCard].transform.rotation, enemiesParent.transform));
            }
        }
    }

    public void SpawnBoss()
    {
        GameObject oneSpawnerObject = oneEnemySpawnPosition[0];
        if (zodiacToFight == "CANCER")
        {
            enemiesSpawned.Add(Instantiate(cancerFigure, oneSpawnerObject.transform.position, cancerFigure.transform.rotation, enemiesParent.transform));
        }
        else if (zodiacToFight == "CAPRICORN")
        {
            enemiesSpawned.Add(Instantiate(capricornFigure, oneSpawnerObject.transform.position, capricornFigure.transform.rotation, enemiesParent.transform));
        }
    }

    // Attack functions
    public void Attack(List<GameObject> enemyTexts, int attackedEnemy)
    {
        foreach (GameObject text in enemyTexts)
        {
            text.SetActive(false);
        }

        battleDialogueText.SetActive(true);

        int random = Random.Range(10, 21);
        if (enemiesSpawned[attackedEnemy].GetComponent<EnemyCard>().Life - random <= 0)
        {
            battleDialogueText.GetComponent<Text>().text = "    YOU DEALED " + random + " DAMAGE TO " + enemyTexts[attackedEnemy].GetComponent<Text>().text + ", YOU KILLED " + enemyTexts[attackedEnemy].GetComponent<Text>().text;
        }
        else
        {
            battleDialogueText.GetComponent<Text>().text = "    YOU DEALED " + random + " DAMAGE TO " + enemyTexts[attackedEnemy].GetComponent<Text>().text;
        }

        enemiesSpawned[attackedEnemy].GetComponent<EnemyCard>().Life -= random;

        currentBattleState = GameStates.DEFENDING;
        lastBattleState = GameStates.ATTACKING;
    }

    public void RedistributeEnemyTexts()
    {
        amountOfEnemiesAlive--;
        if (amountOfEnemiesAlive == 1)
        {
            textToDisplay = "    YOU ARE UP AGAINST " + amountOfEnemiesAlive + " ENEMY";
        }
        else
        {
            textToDisplay = "    YOU ARE UP AGAINST " + amountOfEnemiesAlive + " ENEMIES";
        }

        int deleteTarget = 0;
        for (int i = 0; i <= amountOfEnemiesAlive; i++)
        {
            if (enemiesSpawned[i].GetComponent<EnemyCard>().Life <= 0)
            {
                deleteTarget = i;
                break;
            }
        }

        // If 0 enemies, the player wins
        if (amountOfEnemiesAlive == 0)
        {
            buttonManager.enemyTexts[deleteTarget].GetComponent<Text>().text = "";
            buttonManager.enemyTexts[deleteTarget].transform.GetChild(1).GetComponent<Text>().text = "";

            enemiesSpawned.RemoveAt(deleteTarget);
            //buttonManager.enemyTexts.RemoveAt(amountSpawn);

            currentBattleState = GameStates.VICTORY;
        }

        else if (deleteTarget != amountOfEnemiesAlive)
        {
            for (int i = deleteTarget; i < amountOfEnemiesAlive; i++)
            {
                buttonManager.enemyTexts[i].GetComponent<Text>().text = buttonManager.enemyTexts[i + 1].GetComponent<Text>().text;
                buttonManager.enemyTexts[i].GetComponent<Text>().color = buttonManager.enemyTexts[i + 1].GetComponent<Text>().color;
                buttonManager.enemyTexts[i].transform.GetChild(1).GetComponent<Text>().text = "    " + buttonManager.enemyTexts[i + 1].GetComponent<Text>().text;
                buttonManager.enemyTexts[i].transform.GetChild(1).GetComponent<Text>().color = buttonManager.enemyTexts[i + 1].GetComponent<Text>().color;
            }

            enemiesSpawned.RemoveAt(deleteTarget);
            //buttonManager.enemyTexts.RemoveAt(amountSpawn);

            buttonManager.enemyTexts[amountOfEnemiesAlive].GetComponent<Text>().text = "";
            buttonManager.enemyTexts[amountOfEnemiesAlive].transform.GetChild(1).GetComponent<Text>().text = "";

            buttonManager.enemyTexts[deleteTarget].GetComponent<Text>().enabled = true;
            buttonManager.enemyTexts[deleteTarget].transform.GetChild(0).gameObject.SetActive(false);
            buttonManager.enemyTexts[deleteTarget].transform.GetChild(1).gameObject.SetActive(false);
            buttonManager.currentTextIndex = 0;
            buttonManager.enemyTexts[buttonManager.currentTextIndex].GetComponent<Text>().enabled = false;
            buttonManager.enemyTexts[buttonManager.currentTextIndex].transform.GetChild(0).gameObject.SetActive(true);
            buttonManager.enemyTexts[buttonManager.currentTextIndex].transform.GetChild(1).gameObject.SetActive(true);
        }

        else
        {
            buttonManager.enemyTexts[amountOfEnemiesAlive].GetComponent<Text>().text = "";
            buttonManager.enemyTexts[amountOfEnemiesAlive].transform.GetChild(1).GetComponent<Text>().text = "";

            enemiesSpawned.RemoveAt(amountOfEnemiesAlive);
            //buttonManager.enemyTexts.RemoveAt(amountSpawn);

            buttonManager.enemyTexts[amountOfEnemiesAlive].GetComponent<Text>().enabled = true;
            buttonManager.enemyTexts[amountOfEnemiesAlive].transform.GetChild(0).gameObject.SetActive(false);
            buttonManager.enemyTexts[amountOfEnemiesAlive].transform.GetChild(1).gameObject.SetActive(false);
            buttonManager.currentTextIndex = 0;
            buttonManager.enemyTexts[buttonManager.currentTextIndex].GetComponent<Text>().enabled = false;
            buttonManager.enemyTexts[buttonManager.currentTextIndex].transform.GetChild(0).gameObject.SetActive(true);
            buttonManager.enemyTexts[buttonManager.currentTextIndex].transform.GetChild(1).gameObject.SetActive(true);
        }
    }

    public void StartEnemyAttack()
    {
        dialogueArea.SetActive(false);
        battleArea.SetActive(true);

        battleArea.transform.GetChild(0).GetComponent<Animator>().SetBool("Expand", true);
        buttonManager.playerStar.transform.position = buttonManager.playerStarSpawn.transform.position;

        StartCoroutine(nameof(InitiateAttack));
    }

    public void ResetBattleArea()
    {
        battleDialogueText.GetComponent<Text>().text = textToDisplay;

        // Disable enemy texts
        buttonManager.enemyTexts[buttonManager.currentTextIndex].GetComponent<Text>().enabled = true;
        buttonManager.enemyTexts[buttonManager.currentTextIndex].transform.GetChild(0).gameObject.SetActive(false);
        buttonManager.enemyTexts[buttonManager.currentTextIndex].transform.GetChild(1).gameObject.SetActive(false);

        foreach (GameObject text in buttonManager.enemyTexts)
        {
            text.SetActive(false);
        }

        battleDialogueText.SetActive(true);
    }

    // Listen functions
    public void Talk(List<GameObject> enemyTexts)
    {
        foreach (GameObject text in enemyTexts)
            text.SetActive(false);

        battleDialogueText.SetActive(true);
        if (zodiacToFight != "")
        {
            battleDialogueText.GetComponent<Text>().text = "    SHE IS BUSY FIGHTING, CAN'T TALK NOW";
        }
        else
        {
            battleDialogueText.GetComponent<Text>().text = "    YOU CAN'T TALK TO AN NPC";
        }

        currentBattleState = GameStates.WAITING;
        lastBattleState = GameStates.TALKING;
    }

    // Items functions
    public void Items(List<GameObject> itemTexts, int itemUsed)
    {
        foreach (GameObject text in itemTexts)
            text.SetActive(false);

        battleDialogueText.SetActive(true);

        int usefulness = GameMaster.inventory[itemUsed].Type == ObjectTypes.HEALTH ? GameMaster.inventory[itemUsed].Level * 5 : GameMaster.inventory[itemUsed].Level;
        if (GameMaster.inventory[itemUsed].Type == ObjectTypes.HEALTH)
        {
            battleDialogueText.GetComponent<Text>().text = "    YOU USED " + GameMaster.inventory[itemUsed].ObjectName + " LVL." + GameMaster.inventory[itemUsed].Level + " YOU HEALED " + usefulness + " HP";
            GameMaster.playerLife += usefulness;
            if (GameMaster.playerLife > GameMaster.maxPlayerLife)
            {
                GameMaster.playerLife = GameMaster.maxPlayerLife;
            }
        }

        else if (GameMaster.inventory[itemUsed].Type == ObjectTypes.ATTACK)
        {
            battleDialogueText.GetComponent<Text>().text = "    YOU USED " + GameMaster.inventory[itemUsed].ObjectName + " LVL." + GameMaster.inventory[itemUsed].Level + " YOUR ATTACK INCREASED BY " + usefulness;
        }

        else if (GameMaster.inventory[itemUsed].Type == ObjectTypes.DEFENSE)
        {
            battleDialogueText.GetComponent<Text>().text = "    YOU USED " + GameMaster.inventory[itemUsed].ObjectName + " LVL." + GameMaster.inventory[itemUsed].Level + " YOUR DEFENSE INCREASED BY " + usefulness;
        }

        else
        {
            battleDialogueText.GetComponent<Text>().text = "    YOU USED " + GameMaster.inventory[itemUsed].ObjectName + " LVL." + GameMaster.inventory[itemUsed].Level + " YOUR SPEED INCREASED BY " + usefulness;
            GameMaster.playerSpeed += usefulness;
        }

        GameMaster.inventory[itemUsed].Consumed = true;
        RedistributeItemTexts();

        currentBattleState = GameStates.DEFENDING;
        lastBattleState = GameStates.USING_ITEM;
    }

    public void RedistributeItemTexts()
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
    public void Run()
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

    // Win functions
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

    public void LoseGame()
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

        if (zodiacToFight == "")
        {
            int numAttack = Random.Range(0, availableNormalAttacks.Count);

            availableNormalAttacks[numAttack].SetActive(true);

            if (availableNormalAttacks[numAttack].GetComponent<ActivateChildren>() != null)
            {
                availableNormalAttacks[numAttack].GetComponent<ActivateChildren>().ActivateMeteos();
                yield return new WaitForSeconds(11f);
                StartCoroutine(nameof(EndBattle));
                availableNormalAttacks[numAttack].GetComponent<ActivateChildren>().DeactivateMeteos();
            }

            else
            {
                // TODO: Check what this waitTime did
                // yield return new WaitForSeconds(attacks[numAttack].transform.GetChild(0).GetComponent<DealDamageToPlayer>().waitTime);
                StartCoroutine(nameof(EndBattle));
                availableNormalAttacks[numAttack].SetActive(false);
            }
        }
        
        else
        {
            int numAttack = Random.Range(0, availableBossAttacks.Count);
            availableBossAttacks[numAttack].SetActive(true);

            if (numAttack != 2)
            {
                //yield return new WaitForSeconds(5.25f);
                int activatedOnes = 0;

                while (activatedOnes < 3)
                {
                    int activate = Random.Range(0, availableBossAttacks[numAttack].transform.childCount);

                    if (!availableBossAttacks[numAttack].transform.GetChild(activate).gameObject.activeSelf)
                    {
                        activatedOnes++;
                        availableBossAttacks[numAttack].transform.GetChild(activate).gameObject.SetActive(true);
                        yield return new WaitForSeconds(1.55f);
                        //bossAttacks[numAttack].transform.GetChild(activate).gameObject.SetActive(false);
                    }
                }

                for (int i = 0; i < availableBossAttacks[numAttack].transform.childCount; i++)
                {
                    availableBossAttacks[numAttack].transform.GetChild(i).gameObject.SetActive(false);
                }
            }

            else
            {
                //yield return new WaitForSeconds(8.55f);
                int activatedOnes = 0;

                while (activatedOnes < 5)
                {
                    int activate = Random.Range(0, availableBossAttacks[numAttack].transform.childCount);

                    if (!availableBossAttacks[numAttack].transform.GetChild(activate).gameObject.activeSelf)
                    {
                        activatedOnes++;
                        availableBossAttacks[numAttack].transform.GetChild(activate).gameObject.SetActive(true);
                        yield return new WaitForSeconds(1.55f);
                        //bossAttacks[numAttack].transform.GetChild(activate).gameObject.SetActive(false);
                    }
                }

                for (int i = 0; i < availableBossAttacks[numAttack].transform.childCount; i++)
                {
                    availableBossAttacks[numAttack].transform.GetChild(i).gameObject.SetActive(false);
                }
            }

            availableBossAttacks[numAttack].SetActive(false);
            StartCoroutine(nameof(EndBattle));
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