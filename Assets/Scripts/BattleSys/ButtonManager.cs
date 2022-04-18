using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonManager : MonoBehaviour
{
    public List<GameObject> battleButtons;

    public bool pressed = false;
    private int currentButtonIndex;

    // Start is called before the first frame update
    void Start()
    {
        battleButtons[0].GetComponent<PressBattleButton>().OnSelection();
        currentButtonIndex = 0;
    }

    // Update is called once per frame
    void Update()
    {
        float leftRight = Input.GetAxis("Horizontal");

        if (leftRight < 0 && !pressed)
        {
            pressed = true;
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

        else if (leftRight > 0 && !pressed)
        {
            pressed = true;
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

        else if (leftRight == 0 && pressed)
        {
            pressed = false;
        }
    }
}
