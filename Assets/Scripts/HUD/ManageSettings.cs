using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManageSettings : MonoBehaviour
{
    public RoomTemplates templates;
    public Button menuButton;
    public AudioSource stepsSound;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (templates?.mapFormed == false || templates?.changingRoom == true)
            menuButton.enabled = false;

        else if (templates != null)
            menuButton.enabled = true;
    }

    public void ActivateSettings()
    {
        templates.changingRoom = true;
        stepsSound.Stop();
    }

    public void DeactivateSettings()
    {
        templates.changingRoom = false;
        stepsSound.Play();
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}