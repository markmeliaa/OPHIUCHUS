using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ManageSettings : MonoBehaviour
{
    public RoomTemplates templates;
    public PlayerMovement playerMovement;
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
        playerMovement.canMove = false;
        stepsSound.Stop();
    }

    public void DeactivateSettings()
    {
        templates.changingRoom = false;
        stepsSound.Play();
    }

    public void DeactivateSettings2()
    {
        playerMovement.canMove = true;
        stepsSound.Play();
    }
}
