using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InitialLoad : MonoBehaviour
{
    public PlayerMovement playerMovement;

    private float waitTime = 4f;

    public GameObject overScreen;
    public Animator screen;
    public AudioSource globalMusic;

    public Text attemptsText;
    public Text successfulAttemptsText;

    private void Awake()
    {
        playerMovement.canMove = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (waitTime > 0)
            waitTime -= Time.deltaTime;

        else if (waitTime < 0)
        {
            waitTime = 0;
            overScreen.SetActive(false);
            screen.SetBool("Show", true);
            globalMusic.Play();
            StartCoroutine("CanMove");
        }

        attemptsText.text = GameMaster.attempts.ToString();
        successfulAttemptsText.text = GameMaster.successfulAttemps.ToString();
    }

    IEnumerator CanMove()
    {
        yield return new WaitForSeconds(1.0f);
        playerMovement.canMove = true;
    }
}
