using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Settings")]
    public AudioSource bgmSource;  
    public AudioClip bgmClip;     

    [Header("Sound Effects")]
    public List<AudioClip> soundEffects;  
    public AudioSource sfxSource;        

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        PlayBGM();
    }

    public void PlayBGM()
    {
        if (bgmSource != null && bgmClip != null)
        {
            bgmSource.clip = bgmClip;
            bgmSource.loop = true;
            bgmSource.Play();
        }
        else
        {
            Debug.LogWarning("BGM Source or Clip is not assigned.");
        }
    }

    public void PlayClip(int id)
    {

        if (id >= 0 && id < soundEffects.Count && sfxSource != null)
        {
            sfxSource.PlayOneShot(soundEffects[id]);
        }
        else
        {
            Debug.LogWarning("Invalid sound effect ID or SFX Source is not assigned.");
        }
    }
}
