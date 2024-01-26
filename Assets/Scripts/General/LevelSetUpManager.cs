using System.Collections;
using UnityEngine;

public class LevelSetUpManager : MonoBehaviour
{
    private GameObject mainCharacter;
    private GameObject tpCharacter;

    [SerializeField] private AudioSource gameMusic;

    [SerializeField] private GameObject overLoadScreen;
    [SerializeField] private Animator loadSceneCircleAnimator;
    [SerializeField] private Animator winCircleAnimator;

    private void Start()
    {
        mainCharacter = GameObject.FindGameObjectWithTag("Player");
        tpCharacter = mainCharacter?.transform.GetChild(0).gameObject;
    }

    public void SetUpInitialAttributes()
    {
        gameMusic.Play();

        overLoadScreen.SetActive(false);
        loadSceneCircleAnimator.SetBool("Show", true);
        winCircleAnimator.SetBool("Show", true);

        AddItems();

        StartCoroutine(nameof(StartGame));
    }

    public void AddItems() // TODO: Modularize this function for future levels
    {
        GameMaster.inventory.Add(new ItemObject("Health Potion", ObjectTypes.HEALTH, 1));
        GameMaster.inventory.Add(new ItemObject("Speed Potion", ObjectTypes.SPEED, 2));
    }

    public void SpawnCharacterInLevel()
    {
        mainCharacter.GetComponent<SpriteRenderer>().enabled = false;
        tpCharacter.SetActive(true);
    }

    IEnumerator StartGame()
    {
        yield return new WaitForSeconds(0.5f);
        SpawnCharacterInLevel();
        yield return new WaitForSeconds(1.0f);

        mainCharacter.GetComponent<SpriteRenderer>().enabled = true;
        tpCharacter.SetActive(false);
    }
}