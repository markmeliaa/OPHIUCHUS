using UnityEngine;
using UnityEngine.UI;

public class UpdateParams : MonoBehaviour
{
    public Text lifeText;
    public Text globalLifeText;

    public Text speedText;
    public Text globalSpeedText;

    public Text realmText;

    public Text moneyText;

    private void Update()
    {
        lifeText.text = GameMaster.playerLife + "/" + GameMaster.maxPlayerLife;
        globalLifeText.text = GameMaster.playerLife + "/" + GameMaster.maxPlayerLife;

        speedText.text = GameMaster.playerSpeed.ToString();
        globalSpeedText.text = GameMaster.playerSpeed.ToString();

        switch (GameMaster.runRealmNumber)
        {
            case 0:
                realmText.text = "First Realm: ";
                realmText.text += GameMaster.realmLocations[GameMaster.runRealmNumber];
                break;

            case 1:
                realmText.text = "Second Realm: ";
                realmText.text += GameMaster.realmLocations[GameMaster.runRealmNumber];
                break;

            case 2:
                realmText.text = "Third Realm: ";
                realmText.text += GameMaster.realmLocations[GameMaster.runRealmNumber];
                break;

            case 3:
                realmText.text = "Fourth Realm: ";
                realmText.text += GameMaster.realmLocations[GameMaster.runRealmNumber];
                break;

            case 4:
                realmText.text = "Fifth Realm: ";
                realmText.text += GameMaster.realmLocations[GameMaster.runRealmNumber];
                break;

            case 5:
                realmText.text = "Final Realm: ";
                realmText.text += GameMaster.realmLocations[GameMaster.runRealmNumber];
                break;

            default:
                break;
        }

        moneyText.text = GameMaster.runMoney.ToString();
    }
}
