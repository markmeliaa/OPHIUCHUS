using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum gameStates { stop, choosing, attacking, talking, inventory, run, waiting, defend, win, end }

public class BattleManager : MonoBehaviour
{
    public List<GameObject> enemyCards;
    private int amountSpawn;
    private int baseAmountSpawn;
    public Transform leftLimitSpawn;
    public Transform rightLimitSpawn;

    public GameObject parentEnemies;
    public List<GameObject> enemiesSpawned;

    [HideInInspector] public gameStates state = gameStates.stop;
    [HideInInspector] public gameStates lastState;

    public GameObject normalText;
    [HideInInspector] public string baseText;

    public ButtonManager buttonManager;

    public GameObject textArea;
    public GameObject battleArea;

    public List<GameObject> attacks;

    private void Start()
    {
        /*
        //amountSpawn = 1;
        amountSpawn = Random.Range(1, 5);
        baseAmountSpawn = amountSpawn;

        if (amountSpawn == 1)
            normalText.GetComponent<Text>().text = "    YOU ARE UP AGAINST " + amountSpawn + " ENEMY";
        else
            normalText.GetComponent<Text>().text = "    YOU ARE UP AGAINST " + amountSpawn + " ENEMIES";

        baseText = normalText.GetComponent<Text>().text;
        enemiesSpawned = new List<GameObject>();
        */
    }

    private void Update()
    {
        //Debug.Log(state);
    }

    public void SetUpBattle()
    {
        //amountSpawn = 1;
        amountSpawn = Random.Range(1, 5);
        baseAmountSpawn = amountSpawn;

        if (amountSpawn == 1)
            normalText.GetComponent<Text>().text = "    YOU ARE UP AGAINST " + amountSpawn + " ENEMY";
        else
            normalText.GetComponent<Text>().text = "    YOU ARE UP AGAINST " + amountSpawn + " ENEMIES";

        baseText = normalText.GetComponent<Text>().text;
        enemiesSpawned = new List<GameObject>();
    }

    public void SpawnCards()
    {
        // Only one card spawns
        if (amountSpawn == 1)
        {
            int numberCard = Random.Range(0, enemyCards.Count);
            Vector3 halfScreenPosition = new Vector3(leftLimitSpawn.position.x + (Mathf.Abs(rightLimitSpawn.position.x) + Mathf.Abs(leftLimitSpawn.position.x)) / 2, rightLimitSpawn.position.y, rightLimitSpawn.position.z);
            enemiesSpawned.Add(Instantiate(enemyCards[numberCard], halfScreenPosition, enemyCards[numberCard].transform.rotation, parentEnemies.transform));
        }

        // Two cards spawn
        else if (amountSpawn == 2)
        {
            int numberCard = Random.Range(0, enemyCards.Count);
            Vector3 halfScreenPosition = new Vector3(leftLimitSpawn.position.x + (Mathf.Abs(rightLimitSpawn.position.x) + Mathf.Abs(leftLimitSpawn.position.x)) / 2, rightLimitSpawn.position.y, rightLimitSpawn.position.z);
            
            Vector3 spawnPosition = new Vector3(leftLimitSpawn.position.x + (Mathf.Abs(leftLimitSpawn.position.x) + Mathf.Abs(halfScreenPosition.x)) / 2 + 1, rightLimitSpawn.position.y, rightLimitSpawn.position.z);
            enemiesSpawned.Add(Instantiate(enemyCards[numberCard], spawnPosition, enemyCards[numberCard].transform.rotation, parentEnemies.transform));

            numberCard = Random.Range(0, enemyCards.Count);
            spawnPosition = new Vector3(halfScreenPosition.x + (Mathf.Abs(leftLimitSpawn.position.x) + Mathf.Abs(halfScreenPosition.x)) / 2 - 1, rightLimitSpawn.position.y, rightLimitSpawn.position.z);
            enemiesSpawned.Add(Instantiate(enemyCards[numberCard], spawnPosition, enemyCards[numberCard].transform.rotation, parentEnemies.transform));
        }

        // Three cards spawn
        else if (amountSpawn == 3)
        {
            int numberCard = Random.Range(0, enemyCards.Count);
            Vector3 halfScreenPosition = new Vector3(leftLimitSpawn.position.x + (Mathf.Abs(rightLimitSpawn.position.x) + Mathf.Abs(leftLimitSpawn.position.x)) / 2, rightLimitSpawn.position.y, rightLimitSpawn.position.z);
            
            Vector3 spawnPosition = new Vector3(leftLimitSpawn.position.x + (Mathf.Abs(leftLimitSpawn.position.x) + Mathf.Abs(halfScreenPosition.x)) / 2, rightLimitSpawn.position.y, rightLimitSpawn.position.z);
            enemiesSpawned.Add(Instantiate(enemyCards[numberCard], spawnPosition, enemyCards[numberCard].transform.rotation, parentEnemies.transform));

            numberCard = Random.Range(0, enemyCards.Count);
            enemiesSpawned.Add(Instantiate(enemyCards[numberCard], halfScreenPosition, enemyCards[numberCard].transform.rotation, parentEnemies.transform));

            numberCard = Random.Range(0, enemyCards.Count);
            spawnPosition = new Vector3(halfScreenPosition.x + (Mathf.Abs(leftLimitSpawn.position.x) + Mathf.Abs(halfScreenPosition.x)) / 2, rightLimitSpawn.position.y, rightLimitSpawn.position.z);
            enemiesSpawned.Add(Instantiate(enemyCards[numberCard], spawnPosition, enemyCards[numberCard].transform.rotation, parentEnemies.transform));
        }

        // Four cards spawn
        else
        {
            int numberCard = Random.Range(0, enemyCards.Count);
            Vector3 halfScreenPosition = new Vector3(leftLimitSpawn.position.x + (Mathf.Abs(rightLimitSpawn.position.x) + Mathf.Abs(leftLimitSpawn.position.x)) / 4, rightLimitSpawn.position.y, rightLimitSpawn.position.z);
            Vector3 halfhalfScreenPosition = new Vector3(leftLimitSpawn.position.x + (Mathf.Abs(rightLimitSpawn.position.x) + Mathf.Abs(leftLimitSpawn.position.x)) / 8, rightLimitSpawn.position.y, rightLimitSpawn.position.z);

            enemiesSpawned.Add(Instantiate(enemyCards[numberCard], halfhalfScreenPosition, enemyCards[numberCard].transform.rotation, parentEnemies.transform));

            numberCard = Random.Range(0, enemyCards.Count);
            Vector3 spawnPosition = new Vector3(leftLimitSpawn.position.x - halfhalfScreenPosition.x, halfhalfScreenPosition.y, halfhalfScreenPosition.z);
            enemiesSpawned.Add(Instantiate(enemyCards[numberCard], spawnPosition, enemyCards[numberCard].transform.rotation, parentEnemies.transform));

            numberCard = Random.Range(0, enemyCards.Count);
            spawnPosition = new Vector3(halfScreenPosition.x - halfhalfScreenPosition.x, halfhalfScreenPosition.y, halfhalfScreenPosition.z);
            enemiesSpawned.Add(Instantiate(enemyCards[numberCard], spawnPosition, enemyCards[numberCard].transform.rotation, parentEnemies.transform));

            numberCard = Random.Range(0, enemyCards.Count);
            spawnPosition = new Vector3(-halfhalfScreenPosition.x, halfhalfScreenPosition.y, halfhalfScreenPosition.z);
            enemiesSpawned.Add(Instantiate(enemyCards[numberCard], spawnPosition, enemyCards[numberCard].transform.rotation, parentEnemies.transform));
        }
    }

    public void AddItems()
    {
        GameMaster.inventory.Add(new ItemObject("Health Potion", objectTypes.health, 1));
        GameMaster.inventory.Add(new ItemObject("Speed Potion", objectTypes.speed, 2));
    }

    // Attack functions
    public void Attack(List<GameObject> enemyTexts, int attackedEnemy)
    {
        foreach (GameObject text in enemyTexts)
            text.SetActive(false);

        normalText.SetActive(true);

        int random = Random.Range(10, 21);
        if (enemiesSpawned[attackedEnemy].GetComponent<EnemyCard>().life - random <= 0)
            normalText.GetComponent<Text>().text = "    YOU DEALED " + random + " DAMAGE TO " + enemyTexts[attackedEnemy].GetComponent<Text>().text + ", YOU KILLED " + enemyTexts[attackedEnemy].GetComponent<Text>().text;
        else
            normalText.GetComponent<Text>().text = "    YOU DEALED " + random + " DAMAGE TO " + enemyTexts[attackedEnemy].GetComponent<Text>().text;

        enemiesSpawned[attackedEnemy].GetComponent<EnemyCard>().life -= random;

        state = gameStates.defend;
        lastState = gameStates.attacking;
    }

    public void RedistributeEnemyTexts()
    {
        amountSpawn--;
        if (amountSpawn == 1)
            baseText = "    YOU ARE UP AGAINST " + amountSpawn + " ENEMY";
        else
            baseText = "    YOU ARE UP AGAINST " + amountSpawn + " ENEMIES";

        int deleteTarget = 0;
        for (int i = 0; i <= amountSpawn; i++)
        {
            if (enemiesSpawned[i].GetComponent<EnemyCard>().life <= 0)
            {
                deleteTarget = i;
                break;
            }
        }

        // If 0 enemies, the player wins
        if (amountSpawn == 0)
        {
            buttonManager.enemyTexts[deleteTarget].GetComponent<Text>().text = "";
            buttonManager.enemyTexts[deleteTarget].transform.GetChild(1).GetComponent<Text>().text = "";

            enemiesSpawned.RemoveAt(deleteTarget);
            //buttonManager.enemyTexts.RemoveAt(amountSpawn);

            state = gameStates.win;
        }

        else if (deleteTarget != amountSpawn)
        {
            for (int i = deleteTarget; i < amountSpawn; i++)
            {
                buttonManager.enemyTexts[i].GetComponent<Text>().text = buttonManager.enemyTexts[i + 1].GetComponent<Text>().text;
                buttonManager.enemyTexts[i].GetComponent<Text>().color = buttonManager.enemyTexts[i + 1].GetComponent<Text>().color;
                buttonManager.enemyTexts[i].transform.GetChild(1).GetComponent<Text>().text = "    " + buttonManager.enemyTexts[i + 1].GetComponent<Text>().text;
                buttonManager.enemyTexts[i].transform.GetChild(1).GetComponent<Text>().color = buttonManager.enemyTexts[i + 1].GetComponent<Text>().color;
            }

            enemiesSpawned.RemoveAt(deleteTarget);
            //buttonManager.enemyTexts.RemoveAt(amountSpawn);

            buttonManager.enemyTexts[amountSpawn].GetComponent<Text>().text = "";
            buttonManager.enemyTexts[amountSpawn].transform.GetChild(1).GetComponent<Text>().text = "";

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
            buttonManager.enemyTexts[amountSpawn].GetComponent<Text>().text = "";
            buttonManager.enemyTexts[amountSpawn].transform.GetChild(1).GetComponent<Text>().text = "";

            enemiesSpawned.RemoveAt(amountSpawn);
            //buttonManager.enemyTexts.RemoveAt(amountSpawn);

            buttonManager.enemyTexts[amountSpawn].GetComponent<Text>().enabled = true;
            buttonManager.enemyTexts[amountSpawn].transform.GetChild(0).gameObject.SetActive(false);
            buttonManager.enemyTexts[amountSpawn].transform.GetChild(1).gameObject.SetActive(false);
            buttonManager.currentTextIndex = 0;
            buttonManager.enemyTexts[buttonManager.currentTextIndex].GetComponent<Text>().enabled = false;
            buttonManager.enemyTexts[buttonManager.currentTextIndex].transform.GetChild(0).gameObject.SetActive(true);
            buttonManager.enemyTexts[buttonManager.currentTextIndex].transform.GetChild(1).gameObject.SetActive(true);
        }
    }

    public void StartEnemyAttack()
    {
        textArea.SetActive(false);
        battleArea.SetActive(true);

        battleArea.transform.GetChild(0).GetComponent<Animator>().SetBool("Expand", true);
        buttonManager.playerStar.transform.position = new Vector3(0, -0.7937222f, 0);

        StartCoroutine("InitiateAttack");

        //StartCoroutine("EndBattle");
    }

    public void ResetBattleArea()
    {
        normalText.GetComponent<Text>().text = baseText;

        // Disable enemy texts
        buttonManager.enemyTexts[buttonManager.currentTextIndex].GetComponent<Text>().enabled = true;
        buttonManager.enemyTexts[buttonManager.currentTextIndex].transform.GetChild(0).gameObject.SetActive(false);
        buttonManager.enemyTexts[buttonManager.currentTextIndex].transform.GetChild(1).gameObject.SetActive(false);

        foreach (GameObject text in buttonManager.enemyTexts)
            text.SetActive(false);

        normalText.SetActive(true);
    }

    // Listen functions
    public void Talk(List<GameObject> enemyTexts)
    {
        foreach (GameObject text in enemyTexts)
            text.SetActive(false);

        normalText.SetActive(true);
        normalText.GetComponent<Text>().text = "    YOU CAN'T TALK TO AN NPC LOL";

        state = gameStates.waiting;
        lastState = gameStates.talking;
    }

    // Items functions
    public void Items(List<GameObject> itemTexts, int itemUsed)
    {
        foreach (GameObject text in itemTexts)
            text.SetActive(false);

        normalText.SetActive(true);

        int usefulness = GameMaster.inventory[itemUsed].type == objectTypes.health ? GameMaster.inventory[itemUsed].level * 5 : GameMaster.inventory[itemUsed].level;
        if (GameMaster.inventory[itemUsed].type == objectTypes.health)
        {
            normalText.GetComponent<Text>().text = "    YOU USED " + GameMaster.inventory[itemUsed].objectName + " LVL." + GameMaster.inventory[itemUsed].level + " YOU HEALED " + usefulness + " HP";
            GameMaster.playerLife += usefulness;
            if (GameMaster.playerLife > GameMaster.maxPlayerLife)
                GameMaster.playerLife = GameMaster.maxPlayerLife;
        }

        else if (GameMaster.inventory[itemUsed].type == objectTypes.attack)
        {
            normalText.GetComponent<Text>().text = "    YOU USED " + GameMaster.inventory[itemUsed].objectName + " LVL." + GameMaster.inventory[itemUsed].level + " YOUR ATTACK INCREASED BY " + usefulness;
        }

        else if (GameMaster.inventory[itemUsed].type == objectTypes.defense)
        {
            normalText.GetComponent<Text>().text = "    YOU USED " + GameMaster.inventory[itemUsed].objectName + " LVL." + GameMaster.inventory[itemUsed].level + " YOUR DEFENSE INCREASED BY " + usefulness;
        }

        else
        {
            normalText.GetComponent<Text>().text = "    YOU USED " + GameMaster.inventory[itemUsed].objectName + " LVL." + GameMaster.inventory[itemUsed].level + " YOUR SPEED INCREASED BY " + usefulness;
            GameMaster.playerSpeed += usefulness;
        }

        GameMaster.inventory[itemUsed].used = true;
        RedistributeItemTexts();

        state = gameStates.defend;
        lastState = gameStates.inventory;
    }

    public void RedistributeItemTexts()
    {
        int deleteTarget = 0;
        for (int i = 0; i < GameMaster.inventory.Count; i++)
        {
            if (GameMaster.inventory[i].used)
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
        if (GameMaster.playerSpeed >= 5)
            normalText.GetComponent<Text>().text = "    YOU WERE ABLE TO RUN FROM THE BATTLE";
        else
            normalText.GetComponent<Text>().text = "    YOU WERE NOT ABLE TO RUN, NOT ENOUGH SPEED";

        state = gameStates.waiting;
        lastState = gameStates.run;
    }

    // Win functions
    public void Win()
    {
        int moneyWon = Random.Range(1, 6) * baseAmountSpawn;
        GameMaster.runMoney += moneyWon;

        state = gameStates.end;
        if (moneyWon == 1)
            baseText = "    ALL ENEMIES DEFEATED, YOU RECIEVED " + moneyWon + " COIN FOR THE VICTORY!\n";
        else
            baseText = "    ALL ENEMIES DEFEATED, YOU RECIEVED " + moneyWon + " COINS FOR THE VICTORY!\n";

        if (Random.Range(0,3) == 2)
        {
            baseText += "YOU ALSO RECIEVED A LVL.1 HEALTH POTION!";
            GameMaster.inventory.Add(new ItemObject("Health Potion", objectTypes.health, 1));
        }

        normalText.GetComponent<Text>().text = baseText;
    }

    IEnumerator InitiateAttack()
    {
        yield return new WaitForSeconds(1f);

        int numAttack = Random.Range(0, attacks.Count);

        attacks[numAttack].SetActive(true);

        yield return new WaitForSeconds(3.5f);

        StartCoroutine("EndBattle");
        attacks[numAttack].SetActive(false);
    }

    IEnumerator EndBattle()
    {
        //yield return new WaitForSeconds(2f);
        state = gameStates.stop;
        buttonManager.playerCanMove = false;
        battleArea.transform.GetChild(0).GetComponent<Animator>().SetBool("Expand", false);
        //buttonManager.playerStar.SetActive(false);

        yield return new WaitForSeconds(1f);
        textArea.SetActive(true);
        battleArea.SetActive(false);
        ResetBattleArea();
        buttonManager.battleButtons[buttonManager.currentButtonIndex].GetComponent<SelectButton>().OnExitSelection();
        buttonManager.currentButtonIndex = 0;
        buttonManager.battleButtons[buttonManager.currentButtonIndex].GetComponent<SelectButton>().OnSelection();

        yield return new WaitForSeconds(0.25f);
        state = gameStates.choosing;
    }
}
