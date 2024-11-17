using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    public static AudioController instance;
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

    // Reference to the AudioSource component that will play the music
    private AudioSource audioSource;

    // Index to keep track of the current song in the playlist
    private int currentTrackIndex = 0;

    void Start()
    {
        // Get the AudioSource component
        audioSource = GetComponent<AudioSource>();

        // Make sure we have at least one audio clip in the playlist
        if (audioPlaylist.Length > 0)
        {
            // Start playing the first track
            PlayNextTrack();
        }
        else
        {
            Debug.LogError("Audio Playlist is empty. Please assign audio clips.");
        }
    }

    void Update()
    {
        // Check if the current audio has finished playing
        if (!audioSource.isPlaying)
        {
            // Play the next track in the playlist
            PlayNextTrack();
        }
    }

    void PlayNextTrack()
    {
        // If there are no audio clips in the playlist, do nothing
        if (audioPlaylist.Length == 0)
            return;

        // Set the AudioSource to the current track
        audioSource.clip = audioPlaylist[currentTrackIndex];
        audioSource.Play();

        // Move to the next track in the playlist, loop back to the start if at the end
        currentTrackIndex = (currentTrackIndex + 1) % audioPlaylist.Length;
    }
}
