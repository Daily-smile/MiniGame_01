using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("音频剪辑")]
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
            // 如果剪辑为空，输出占位日志（受 GameLogger 全局开关控制）
            GameLogger.Log("正在播放音效: " + type + " (占位符)", "AUDIO");
        }
    }
}
