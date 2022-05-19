using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeScreenButton : MonoBehaviour
{
    public Animator screenAnimator;

    public Button startButton;
    public Button exitButton;
    public Button yesButton;
    public Button noButton;

    private Text startText;

    private void Start()
    {
        startText = transform.GetChild(0).gameObject.GetComponent<Text>();
    }

    public void ChangeScreen()
    {
        screenAnimator.SetBool("Change", true);
        startText.color = Color.white;

        //startButton.interactable = false;
        exitButton.interactable = false;
        yesButton.interactable = false;
        noButton.interactable = false;

        StartCoroutine("ActivateButtons");
    }

    IEnumerator ActivateButtons()
    {
        yield return new WaitForSeconds(2.5f);

        startButton.interactable = true;
        exitButton.interactable = true;
        yesButton.interactable = true;
        noButton.interactable = true;
    }
}
