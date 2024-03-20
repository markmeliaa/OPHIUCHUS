using UnityEngine;
using UnityEngine.UI;

public class ManageButtonSelection : MonoBehaviour
{
    [SerializeField] private Sprite borderWhenSelected;
    [SerializeField] private Sprite borderWhenNotSelected;

    [SerializeField] private Text buttonText;
    [SerializeField] private GameObject star;

    private Image buttonSprite;
    private AudioSource audioSource;

    private void Awake()
    {
        buttonSprite = GetComponent<Image>();
        audioSource = GetComponent<AudioSource>();
    }

    public void OnSelection()
    {
        SelectButton();
    }

    public void OnExitSelection()
    {
        UnselectButton();
    }

    void SelectButton()
    {
        buttonSprite.sprite = borderWhenSelected;

        buttonText.color = Color.red;
        star.SetActive(true);

        audioSource.Play();
    }

    void UnselectButton()
    {
        buttonSprite.sprite = borderWhenNotSelected;

        buttonText.color = Color.yellow;
        star.SetActive(false);

        audioSource.Stop();
    }
}
