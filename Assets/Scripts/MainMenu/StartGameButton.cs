using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartGameButton : MonoBehaviour
{
    [SerializeField] private Animator changeSceneAnim;
    [SerializeField] private Text startGameText;

    public void ChangeScene()
    {
        startGameText.color = Color.white;

        changeSceneAnim.SetBool("StartGame", true);
        StartCoroutine("ShowGameAfterAnimation");
    }

    IEnumerator ShowGameAfterAnimation()
    {
        yield return new WaitForSeconds(3.05f);

        SceneManager.LoadScene(1);
    }
}
