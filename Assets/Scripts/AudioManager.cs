using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] public AudioSource sfxSource;
    
    [Header("Clips")]
    [SerializeField] private AudioClip flipClip;
    [SerializeField] private AudioClip matchClip;
    [SerializeField] private AudioClip mismatchClip;
    [SerializeField] private AudioClip gameOverClip;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            if (sfxSource == null) sfxSource = gameObject.AddComponent<AudioSource>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayFlip() 
    {
        sfxSource.pitch = 1.0f;
        PlaySFX(flipClip);
    }
    
    public void PlayMatch(int combo) 
    {
        // Increase pitch by 0.1 for each combo, capped at 2.0
        sfxSource.pitch = Mathf.Min(1.0f + (combo * 0.1f), 2.0f);
        PlaySFX(matchClip);
    }
    
    public void PlayMismatch() 
    {
        sfxSource.pitch = 1.0f;
        PlaySFX(mismatchClip);
    }
    
    public void PlayGameOver() 
    {
        sfxSource.pitch = 1.0f;
        PlaySFX(gameOverClip);
    }

    private void PlaySFX(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }
}
