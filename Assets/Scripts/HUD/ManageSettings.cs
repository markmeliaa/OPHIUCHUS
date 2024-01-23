using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ManageSettings : MonoBehaviour
{
    [SerializeField] private GameObject player;
    private PlayerMovement playerMovement;

    [SerializeField] private GameObject settingsMenu;

    //public Button menuButton;
    //public RoomTemplates templates;

    private void Start()
    {
        playerMovement = player.GetComponent<PlayerMovement>();
    }

    void Update()
    {
        /*
            if (templates?.mapFormed == false || templates?.changingRoom == true)
                menuButton.gameObject.SetActive(false);

            else if (templates != null)
                menuButton.gameObject.SetActive(true);
        */

        // TODO: Hide menu button while loading a room
    }

    public void ActivateSettings()
    {
        playerMovement.StopPlayer();
        settingsMenu.SetActive(true);
    }

    public void DeactivateSettings()
    {
        playerMovement.canMove = true;
        settingsMenu.SetActive(false);
    }
}
