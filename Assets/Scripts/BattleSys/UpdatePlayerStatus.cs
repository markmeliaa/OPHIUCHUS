using UnityEngine;
using UnityEngine.UI;

public class UpdatePlayerStatus : MonoBehaviour
{
    [SerializeField] private bool isGame = true;
    [Header("BATTLE PARAMS")]
    [SerializeField] private Text battleLifeText;
    [SerializeField] private Text battleSpeedText;
    [Space(5)]
    [Header("SETTINGS PARAMS")]
    [SerializeField] private Text globalLifeText;
    [SerializeField] private Text globalSpeedText;

    [SerializeField] private Text globalMoneyText;
    [SerializeField] private Text globalRealmText;

    private void Update()
    {
        string lifeToDisplay = GameMaster.playerLife + "/" + GameMaster.maxPlayerLife;
        string speedToDisplay = GameMaster.playerSpeed.ToString();

        battleLifeText.text = lifeToDisplay;
        battleSpeedText.text = speedToDisplay;

        if (!isGame)
        {
            return;
        }

        globalLifeText.text = lifeToDisplay;
        globalSpeedText.text = speedToDisplay;
        globalMoneyText.text = GameMaster.runMoney.ToString();

        switch (GameMaster.runRealmNumber)
        {
            case 0:
                globalRealmText.text = "First Realm: ";
                globalRealmText.text += GameMaster.realmLocations[GameMaster.runRealmNumber];
                break;

            case 1:
                globalRealmText.text = "Second Realm: ";
                globalRealmText.text += GameMaster.realmLocations[GameMaster.runRealmNumber];
                break;

            case 2:
                globalRealmText.text = "Third Realm: ";
                globalRealmText.text += GameMaster.realmLocations[GameMaster.runRealmNumber];
                break;

            case 3:
                globalRealmText.text = "Fourth Realm: ";
                globalRealmText.text += GameMaster.realmLocations[GameMaster.runRealmNumber];
                break;

            case 4:
                globalRealmText.text = "Fifth Realm: ";
                globalRealmText.text += GameMaster.realmLocations[GameMaster.runRealmNumber];
                break;

            case 5:
                globalRealmText.text = "Final Realm: ";
                globalRealmText.text += GameMaster.realmLocations[GameMaster.runRealmNumber];
                break;

            default:
                break;
        }
    }
}
