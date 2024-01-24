using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ManageScenes : MonoBehaviour
{
    [SerializeField] private Text loadSceneText;
    [SerializeField] private Animator changeSceneAnim;

    [SerializeField] private string stateToChange;
    [SerializeField] private bool stateValue = true;
    [SerializeField] private float animationDuration;

    public void LoadSelectedScene(int sceneLoad)
    {
        if (loadSceneText != null)
        {
            loadSceneText.color = Color.white;
        }

        if (changeSceneAnim != null)
        {
            changeSceneAnim.SetBool(stateToChange, stateValue);
        }

        StartCoroutine("ChangeSceneAfterAnimation", sceneLoad);
    }

    IEnumerator ChangeSceneAfterAnimation(int sceneLoad)
    {
        yield return new WaitForSeconds(animationDuration);

        SceneManager.LoadScene(sceneLoad);
    }

    public int GetCurrentSceneIndex()
    {
        return SceneManager.GetActiveScene().buildIndex;
    }

    public string GetCurrentSceneName()
    {
        return SceneManager.GetActiveScene().name;
    }
}
