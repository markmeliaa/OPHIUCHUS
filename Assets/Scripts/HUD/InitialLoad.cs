using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InitialLoad : MonoBehaviour
{
    public PlayerMoveIso2 playerMove;

    private float waitTime = 4f;

    public GameObject overScreen;
    public Animator screen;
    public AudioSource globalMusic;

    public Text attemptsText;

    private void Awake()
    {
        playerMove.moving = false;
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
    }

    IEnumerator CanMove()
    {
        yield return new WaitForSeconds(1.0f);
        playerMove.moving = true;
    }
}
