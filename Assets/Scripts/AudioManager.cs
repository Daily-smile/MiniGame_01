using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Clips")]
    public AudioClip perfectFitSound;
    public AudioClip goodFitSound;
    public AudioClip failSound;
    public AudioClip fireSound;

    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void PlaySound(string type)
    {
        AudioClip clip = null;
        switch (type.ToLower())
        {
            case "perfect":
                clip = perfectFitSound;
                break;
            case "good":
                clip = goodFitSound;
                break;
            case "fail":
                clip = failSound;
                break;
            case "fire":
                clip = fireSound;
                break;
        }

        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
        else
        {
            Debug.Log("Playing sound: " + type + " (Placeholder)");
        }
    }
}
