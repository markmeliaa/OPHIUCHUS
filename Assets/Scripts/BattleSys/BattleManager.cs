using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum gameStates { choosing, attacking, talking, inventory, waiting, defend, win }

public class BattleManager : MonoBehaviour
{
    public List<GameObject> enemyCards;
    private int amountSpawn;
    public Transform leftLimitSpawn;
    public Transform rightLimitSpawn;

    public GameObject parentEnemies;
    public List<GameObject> enemiesSpawned;

    [HideInInspector] public gameStates state = gameStates.choosing;
    [HideInInspector] public gameStates lastState;

    public GameObject normalText;
    [HideInInspector] public string baseText;

    public ButtonManager buttonManager;

    private void Start()
    {
        amountSpawn = Random.Range(1, 5);
        if (amountSpawn == 1)
            normalText.GetComponent<Text>().text = "    YOU ARE UP AGAINST " + amountSpawn + " ENEMY";
        else
            normalText.GetComponent<Text>().text = "    YOU ARE UP AGAINST " + amountSpawn + " ENEMIES";

        baseText = normalText.GetComponent<Text>().text;
        enemiesSpawned = new List<GameObject>();

        SpawnCards();
    }

    private void Update()
    {
        //Debug.Log(state);
    }

    private void SpawnCards()
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

    // Attack functions
    public void Attack(int target, List<GameObject> enemyTexts, int attackedEnemy)
    {
        foreach (GameObject text in enemyTexts)
            text.SetActive(false);

        int random = Random.Range(10, 21);
        normalText.SetActive(true);
        if (enemiesSpawned[target].GetComponent<EnemyCard>().life - random <= 0)
            normalText.GetComponent<Text>().text = "    YOU DEALED " + random + " DAMAGE TO " + enemyTexts[attackedEnemy].GetComponent<Text>().text + ", YOU KILLED " + enemyTexts[attackedEnemy].GetComponent<Text>().text;
        else
            normalText.GetComponent<Text>().text = "    YOU DEALED " + random + " DAMAGE TO " + enemyTexts[attackedEnemy].GetComponent<Text>().text;

        enemiesSpawned[target].GetComponent<EnemyCard>().life -= random;

        state = gameStates.waiting;
        lastState = gameStates.attacking;
    }

    public void RedistributeTexts()
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
            buttonManager.enemyTexts.RemoveAt(amountSpawn);

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
            buttonManager.enemyTexts.RemoveAt(amountSpawn);

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
            buttonManager.enemyTexts.RemoveAt(amountSpawn);

            buttonManager.enemyTexts[amountSpawn].GetComponent<Text>().enabled = true;
            buttonManager.enemyTexts[amountSpawn].transform.GetChild(0).gameObject.SetActive(false);
            buttonManager.enemyTexts[amountSpawn].transform.GetChild(1).gameObject.SetActive(false);
            buttonManager.currentTextIndex = 0;
            buttonManager.enemyTexts[buttonManager.currentTextIndex].GetComponent<Text>().enabled = false;
            buttonManager.enemyTexts[buttonManager.currentTextIndex].transform.GetChild(0).gameObject.SetActive(true);
            buttonManager.enemyTexts[buttonManager.currentTextIndex].transform.GetChild(1).gameObject.SetActive(true);
        }
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
    public void Items()
    {

    }
}
