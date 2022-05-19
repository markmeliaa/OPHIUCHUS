using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManageSettings : MonoBehaviour
{
    public RoomTemplates templates;
    public PlayerMoveIso2 playerMove;
    public Button menuButton;
    public AudioSource stepsSound;

    public Button exitMenuButton;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (templates?.mapFormed == false || templates?.changingRoom == true)
            menuButton.gameObject.SetActive(false);

        else if (templates != null)
            menuButton.gameObject.SetActive(true);
    }

    public void ActivateSettings()
    {
        templates.changingRoom = true;
        stepsSound.Stop();
    }

    public void ActivateSettings2()
    {
        playerMove.moving = false;
        stepsSound.Stop();
    }

    public void DeactivateSettings()
    {
        templates.changingRoom = false;
        stepsSound.Play();
    }

    public void DeactivateSettings2()
    {
        playerMove.moving = true;
        stepsSound.Play();
    }

    public void ExitGame()
    {
        if (exitMenuButton != null)
        {
            exitMenuButton.gameObject.transform.GetChild(0).gameObject.GetComponent<Text>().color = Color.white;
        }

        Application.Quit();
    }
}
