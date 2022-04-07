using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGameButton : MonoBehaviour
{
    public Animator changeSceneAnim;
    private HoverButton hoverButton;

    private void Start()
    {
        hoverButton = GetComponent<HoverButton>();
    }

    public void ChangeScene()
    {
        if (hoverButton.waitTime > 0)
            return;

        changeSceneAnim.SetBool("StartGame", true);
        StartCoroutine("ShowGame");
    }

    IEnumerator ShowGame()
    {
        yield return new WaitForSeconds(3.05f);

        SceneManager.LoadScene(1);
    }
}
