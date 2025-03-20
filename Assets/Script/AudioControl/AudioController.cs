using UnityEngine;

public class AudioController : MonoBehaviour
{
    public static AudioController instance;
    private float pausedTime = 0f;
    private bool isPaused = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public AudioClip[] audioPlaylist;
    private AudioSource audioSource;
    private int currentTrackIndex = 0;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioPlaylist.Length > 0)
        {
            PlayNextTrack();
        }
        else
        {
            Debug.LogError("Audio Playlist is empty. Please assign audio clips.");
        }
    }

    void Update()
    {
        if (!audioSource.isPlaying && !isPaused)
        {
            PlayNextTrack();
        }
    }

    void PlayNextTrack()
    {
        if (audioPlaylist.Length == 0) return;

        audioSource.clip = audioPlaylist[currentTrackIndex];
        audioSource.time = 0f;  // Reset time position
        pausedTime = 0f;        // Reset pause time
        audioSource.Play();
        currentTrackIndex = (currentTrackIndex + 1) % audioPlaylist.Length;
    }

    public void PauseMusic()
    {
        if (audioSource.isPlaying)
        {
            pausedTime = audioSource.time;
            audioSource.Pause();
            isPaused = true;
        }
    }

    public void ResumeMusic()
    {
        if (isPaused)
        {
            audioSource.time = pausedTime;
            audioSource.Play();
            isPaused = false;
        }
    }
}