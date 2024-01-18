using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ManageLoadScene : MonoBehaviour
{
    [SerializeField] private Text loadSceneText;
    [SerializeField] private Animator changeSceneAnim;

    [SerializeField] private string animationToTrigger;
    [SerializeField] private float animationDuration;

    public void LoadNextScene(int sceneLoad)
    {
        if (loadSceneText != null)
        {
            loadSceneText.color = Color.white;
        }

        if (changeSceneAnim != null)
        {
            changeSceneAnim.SetBool(animationToTrigger, true);
        }

        StartCoroutine("ChangeSceneAfterAnimation", sceneLoad);
    }

    IEnumerator ChangeSceneAfterAnimation(int sceneLoad)
    {
        yield return new WaitForSeconds(animationDuration);

        SceneManager.LoadScene(sceneLoad);
    }
}
