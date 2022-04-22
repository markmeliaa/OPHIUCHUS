using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectButton : MonoBehaviour
{
    private Image buttonSprite;
    public Sprite borderSelected;
    public Sprite borderUnselected;

    public Text buttonText;
    public GameObject star;

    private void Awake()
    {
        buttonSprite = GetComponent<Image>();
    }

    public void OnSelection()
    {
        buttonSprite.sprite = borderSelected;
        buttonText.color = Color.red;
        star.SetActive(true);
    }

    public void OnExitSelection()
    {
        buttonSprite.sprite = borderUnselected;
        buttonText.color = Color.yellow;
        star.SetActive(false);
    }
}
