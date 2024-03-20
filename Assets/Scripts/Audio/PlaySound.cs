using UnityEngine;

public class PlaySound : MonoBehaviour
{
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayEffect(AudioClip audio)
    {
        audioSource.clip = audio;
        audioSource.Play();
    }
}
