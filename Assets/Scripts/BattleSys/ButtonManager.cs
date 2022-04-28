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
    private bool pressedZ = false;

    [HideInInspector] public int currentButtonIndex;
    [HideInInspector] public int currentTextIndex;

    public List<GameObject> enemyTexts;
    public List<GameObject> itemTexts;

    [HideInInspector] public bool playerCanMove = false;
    public GameObject playerStar;

    public List<GameObject> starAnimators;
    public GameObject blackScreen;

    public GameObject animCanvas;
    public GameObject battleCanvas;

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
        if (Input.GetKeyDown(KeyCode.Space) && battleManager.state == gameStates.stop)
            StartBattle();

        if (Input.GetKeyUp(KeyCode.Z))
            pressedZ = false;

        // The battle is paused
        if (battleManager.state == gameStates.stop)
            return;

        // The battle has ended
        if (battleManager.state == gameStates.end)
        {
            if (!pressedZ && Input.GetKeyDown(KeyCode.Z))
            {
                StartCoroutine("WaitFinishGame");
            }

            return;
        }

        // The enemy is attacking and the player avoids the attacks
        if (battleManager.state == gameStates.defend)
        {
            if (!pressedZ && Input.GetKeyDown(KeyCode.Z))
            {
                battleManager.StartEnemyAttack();
                StartCoroutine("WaitMove");
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
            }

            else if (Input.GetKeyDown(KeyCode.Z) && battleManager.lastState == gameStates.attacking)
            {
                foreach (GameObject text in enemyTexts)
                    text.SetActive(true);

                battleManager.normalText.SetActive(false);
                battleManager.state = gameStates.attacking;
                battleManager.lastState = gameStates.waiting;

                pressedZ = true;
            }

            else if (Input.GetKeyDown(KeyCode.X) && battleManager.lastState == gameStates.inventory)
            {
                battleManager.normalText.GetComponent<Text>().text = battleManager.baseText;

                battleManager.state = gameStates.choosing;
                battleManager.lastState = gameStates.waiting;
            }

            else if (battleManager.lastState == gameStates.run)
            {
                if (GameMaster.playerSpeed >= 5 && Input.GetKeyDown(KeyCode.Z) && !pressedZ)
                {
                    battleManager.state = gameStates.stop;
                    battleManager.lastState = gameStates.waiting;

                    StartCoroutine("WaitFinishGame");

                    pressedZ = true;
                }

                else if (GameMaster.playerSpeed < 5 && Input.GetKeyDown(KeyCode.X))
                {
                    battleManager.state = gameStates.choosing;
                    battleManager.lastState = gameStates.waiting;

                    battleManager.normalText.GetComponent<Text>().text = battleManager.baseText;
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

            for (int i = 0; i < battleManager.enemiesSpawned.Count; i++)
            {
                enemyTexts[i].SetActive(true);
                enemyTexts[i].GetComponent<Text>().text = battleManager.enemiesSpawned[i].GetComponent<EnemyCard>().cardName;
                enemyTexts[i].transform.GetChild(1).GetComponent<Text>().text = "    " + battleManager.enemiesSpawned[i].GetComponent<EnemyCard>().cardName;
                battleManager.enemiesSpawned[i].GetComponent<EnemyCard>().nameText = enemyTexts[i];

                if (battleManager.enemiesSpawned[i].name[0].ToString() == "D" || battleManager.enemiesSpawned[i].name[0].ToString() == "H" || battleManager.enemiesSpawned[i].name[0].ToString() == "P")
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
        }

        // Listen and talk
        else if (currentButtonIndex == 1 && Input.GetKeyDown(KeyCode.Z) && (battleManager.state != gameStates.attacking && battleManager.state != gameStates.talking && battleManager.state != gameStates.inventory && battleManager.state != gameStates.run))
        {
            battleManager.state = gameStates.talking;
            battleManager.normalText.SetActive(false);

            for (int i = 0; i < battleManager.enemiesSpawned.Count; i++)
            {
                enemyTexts[i].SetActive(true);
                enemyTexts[i].GetComponent<Text>().text = battleManager.enemiesSpawned[i].GetComponent<EnemyCard>().cardName;
                enemyTexts[i].transform.GetChild(1).GetComponent<Text>().text = "    " + battleManager.enemiesSpawned[i].GetComponent<EnemyCard>().cardName;
                battleManager.enemiesSpawned[i].GetComponent<EnemyCard>().nameText = enemyTexts[i];

                if (battleManager.enemiesSpawned[i].name[0].ToString() == "D" || battleManager.enemiesSpawned[i].name[0].ToString() == "H" || battleManager.enemiesSpawned[i].name[0].ToString() == "P")
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
        }

        // Open inventory
        else if (currentButtonIndex == 2 && Input.GetKeyDown(KeyCode.Z) && (battleManager.state != gameStates.attacking && battleManager.state != gameStates.talking && battleManager.state != gameStates.inventory && battleManager.state != gameStates.run))
        {

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
        }

        // Run from battle
        else if (currentButtonIndex == 3 && Input.GetKeyDown(KeyCode.Z) && (battleManager.state != gameStates.attacking && battleManager.state != gameStates.talking && battleManager.state != gameStates.inventory && battleManager.state != gameStates.run && battleManager.state != gameStates.waiting))
        {
            battleManager.Run();
            pressedZ = true;
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

    public void StartBattle()
    {
        battleManager.SetUpBattle();

        //animCanvas.GetComponent<AudioSource>().Play();

        foreach (GameObject animator in starAnimators)
        {
            animator.GetComponent<Animator>().SetBool("Change", true);
            animator.transform.GetChild(1).GetComponent<Animator>().SetBool("Change", true);
        }

        StartCoroutine("WaitStartGame");
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

    IEnumerator WaitMove()
    {
        yield return new WaitForSeconds(1f);

        playerCanMove = true;
    }

    IEnumerator WaitStartGame()
    {
        yield return new WaitForSeconds(1.2f);

        foreach (GameObject animator in starAnimators)
        {
            animator.GetComponent<Animator>().SetBool("Change", false);
            animator.transform.GetChild(1).GetComponent<Animator>().SetBool("Change", false);
        }

        //animCanvas.SetActive(false);
        battleCanvas.SetActive(true);
        battleManager.SpawnCards();
        battleManager.AddItems();

        blackScreen.GetComponent<Animator>().SetBool("Change", true);
        //blackScreen.SetActive(false);

        yield return new WaitForSeconds(0.25f);
        battleButtons[0].GetComponent<SelectButton>().OnSelection();
        currentButtonIndex = 0;
        battleManager.state = gameStates.choosing;
    }

    IEnumerator WaitFinishGame()
    {
        blackScreen.GetComponent<Animator>().SetBool("Change", false);
        battleManager.state = gameStates.stop;

        yield return new WaitForSeconds(0.6f);

        animCanvas.GetComponent<AudioSource>().Play();

        foreach (GameObject animator in starAnimators)
        {
            animator.GetComponent<Animator>().SetBool("ChangeBack", true);
            animator.transform.GetChild(1).GetComponent<Animator>().SetBool("ChangeBack", true);
        }

        yield return new WaitForSeconds(0.25f);
        ClearGame();
        battleCanvas.SetActive(false);

        yield return new WaitForSeconds(1.2f);
        foreach (GameObject animator in starAnimators)
        {
            animator.GetComponent<Animator>().SetBool("ChangeBack", false);
            animator.transform.GetChild(1).GetComponent<Animator>().SetBool("ChangeBack", false);
        }
    }
}
