using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateParams : MonoBehaviour
{
    public Text lifeText;
    public Text speedText;

    private void Update()
    {
        lifeText.text = GameMaster.playerLife + "/50";
        speedText.text = GameMaster.playerSpeed.ToString();
    }
}
