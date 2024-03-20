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
