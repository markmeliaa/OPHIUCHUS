using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectButton : MonoBehaviour
{
    private Image buttonSprite;
    private AudioSource audioSource;
    public Sprite borderSelected;
    public Sprite borderUnselected;

    public Text buttonText;
    public GameObject star;

    private void Awake()
    {
        buttonSprite = GetComponent<Image>();
        audioSource = GetComponent<AudioSource>();
    }

    public void OnSelection()
    {
        buttonSprite.sprite = borderSelected;
        buttonText.color = Color.red;
        star.SetActive(true);
        audioSource.Play();
    }

    public void OnExitSelection()
    {
        buttonSprite.sprite = borderUnselected;
        buttonText.color = Color.yellow;
        star.SetActive(false);
    }
}
