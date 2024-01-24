using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ManagePantwoLoad : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;

    [SerializeField] private float waitTimeToLoad;
    [SerializeField] private GameObject loadingText;
    [SerializeField] private Animator loadingAnim;
    [SerializeField] private float showGameAnimationDuration;
    [SerializeField] private AudioSource globalMusic;

    [SerializeField] private Text attemptsText;
    [SerializeField] private Text successfulAttemptsText;

    private void Awake()
    {
        playerMovement.canMove = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (waitTimeToLoad > 0.0f)
        {
            waitTimeToLoad -= Time.deltaTime;
            if (waitTimeToLoad <= 0.0f)
            {
                SetGameReady();
            }
        }
    }

    private void SetGameReady()
    {
        loadingText.SetActive(false);
        loadingAnim.SetBool("Show", true);
        globalMusic.Play();

        attemptsText.text = GameMaster.attempts.ToString();
        successfulAttemptsText.text = GameMaster.successfulAttemps.ToString();

        StartCoroutine("EnablePlayerMovementAfterLoad");
    }

    IEnumerator EnablePlayerMovementAfterLoad()
    {
        yield return new WaitForSeconds(showGameAnimationDuration);
        playerMovement.canMove = true;
    }
}
