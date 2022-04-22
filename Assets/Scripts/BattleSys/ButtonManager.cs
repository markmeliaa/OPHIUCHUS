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

    private int currentButtonIndex;
    private int currentTextIndex;

    public List<GameObject> enemyTexts;

    // Start is called before the first frame update
    void Start()
    {
        battleButtons[0].GetComponent<PressBattleButton>().OnSelection();
        currentButtonIndex = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Z))
            pressedZ = false;

        // The enemy is attacking
        if (battleManager.state == gameStates.defend)
            return;

        // Go back in the menu
        if (Input.GetKeyDown(KeyCode.X) && battleManager.state != gameStates.choosing && battleManager.state != gameStates.waiting)
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
                battleButtons[currentButtonIndex].GetComponent<PressBattleButton>().OnExitSelection();
                currentButtonIndex = battleButtons.Count - 1;
                battleButtons[currentButtonIndex].GetComponent<PressBattleButton>().OnSelection();
            }

            else
            {
                battleButtons[currentButtonIndex].GetComponent<PressBattleButton>().OnExitSelection();
                currentButtonIndex--;
                battleButtons[currentButtonIndex].GetComponent<PressBattleButton>().OnSelection();
            }
        }

        else if (leftRight > 0 && !pressedHor && battleManager.state == gameStates.choosing)
        {
            pressedHor = true;
            if (currentButtonIndex == battleButtons.Count - 1)
            {
                battleButtons[currentButtonIndex].GetComponent<PressBattleButton>().OnExitSelection();
                currentButtonIndex = 0;
                battleButtons[currentButtonIndex].GetComponent<PressBattleButton>().OnSelection();
            }

            else
            {
                battleButtons[currentButtonIndex].GetComponent<PressBattleButton>().OnExitSelection();
                currentButtonIndex++;
                battleButtons[currentButtonIndex].GetComponent<PressBattleButton>().OnSelection();
            }
        }

        else if (leftRight == 0 && pressedHor && battleManager.state == gameStates.choosing)
        {
            pressedHor = false;
        }

        // Attack
        if (currentButtonIndex == 0 && Input.GetKeyDown(KeyCode.Z) && (battleManager.state != gameStates.attacking && battleManager.state != gameStates.talking))
        {
            battleManager.state = gameStates.attacking;
            battleManager.normalText.SetActive(false);

            for (int i = 0; i < battleManager.enemiesSpawned.Count; i++)
            {
                enemyTexts[i].SetActive(true);
                enemyTexts[i].GetComponent<Text>().text = battleManager.enemiesSpawned[i].GetComponent<EnemyCard>().cardName;
                enemyTexts[i].transform.GetChild(1).GetComponent<Text>().text = "    " + battleManager.enemiesSpawned[i].GetComponent<EnemyCard>().cardName;

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
            battleManager.Attack(currentTextIndex, enemyTexts, currentTextIndex);
        }

        // Listen and talk
        else if (currentButtonIndex == 1 && Input.GetKeyDown(KeyCode.Z) && (battleManager.state != gameStates.attacking && battleManager.state != gameStates.talking))
        {
            battleManager.state = gameStates.talking;
            battleManager.normalText.SetActive(false);

            for (int i = 0; i < battleManager.enemiesSpawned.Count; i++)
            {
                enemyTexts[i].SetActive(true);
                enemyTexts[i].GetComponent<Text>().text = battleManager.enemiesSpawned[i].GetComponent<EnemyCard>().cardName;
                enemyTexts[i].transform.GetChild(1).GetComponent<Text>().text = "    " + battleManager.enemiesSpawned[i].GetComponent<EnemyCard>().cardName;

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
    }
}
