using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartGameButton : MonoBehaviour
{
    public Animator changeSceneAnim;
    public Text startGameText;

    private HoverButton hoverButton;

    private void Start()
    {
        hoverButton = GetComponent<HoverButton>();
    }

    public void ChangeScene()
    {
        if (hoverButton.waitTime > 0)
            return;

        startGameText.color = Color.white;
        changeSceneAnim.SetBool("StartGame", true);
        StartCoroutine("ShowGame");
    }

    IEnumerator ShowGame()
    {
        yield return new WaitForSeconds(3.05f);

        SceneManager.LoadScene(1);
    }
}
