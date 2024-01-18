using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeScreenButton : MonoBehaviour
{
    [SerializeField] private Animator screenAnimator;

    [SerializeField] private List<Button> mainMenuButtons;
    [SerializeField] private Text startText;

    [SerializeField] private float initialAnimationDuration = 3.0f;
    [SerializeField] private float changeScreenAnimationDuration = 2.5f;

    void Start()
    {
        DisableButtons();
    }

    void Update()
    {
        if (initialAnimationDuration >= 0.0f)
        {
            initialAnimationDuration -= Time.deltaTime;

            if (initialAnimationDuration <= 0.0f)
            {
                EnableButtons();
            }
        }
    }

    public void ChangeScreen()
    {
        screenAnimator.SetBool("Change", true);
        startText.color = Color.white;

        DisableButtons();
        StartCoroutine("ActivateButtonsAfterAnimation");
    }

    IEnumerator ActivateButtonsAfterAnimation()
    {
        yield return new WaitForSeconds(changeScreenAnimationDuration);
        EnableButtons();
    }

    void DisableButtons()
    {
        foreach (Button button in mainMenuButtons)
        {
            button.interactable = false;
        }
    }

    void EnableButtons()
    {
        foreach (Button button in mainMenuButtons)
        {
            button.interactable = true;
        }
    }
}
