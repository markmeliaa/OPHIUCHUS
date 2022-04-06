using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    }
}
