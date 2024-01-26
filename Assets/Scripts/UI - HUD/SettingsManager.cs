using UnityEngine;

public class SettingsManager : MonoBehaviour
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
        /* TODO: Check if this is still needed
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
