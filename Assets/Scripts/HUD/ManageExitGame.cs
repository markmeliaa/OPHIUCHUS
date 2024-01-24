using UnityEngine;
using UnityEngine.UI;

public class ManageExitGame : MonoBehaviour
{
    [SerializeField] private Text exitGameText;

    public void ExitGame()
    {
        if (exitGameText != null)
        {
            exitGameText.color = Color.white;
        }

        Application.Quit();
    }
}
