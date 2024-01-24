using UnityEngine;
using UnityEngine.Audio;

public class ManageVolume : MonoBehaviour
{
    [SerializeField] private AudioMixer masterMixer;

    public void SetVolume(float sliderValue)
    {
        masterMixer.SetFloat("MusicVol", Mathf.Log10(sliderValue) * 20.0f);
    }
}
