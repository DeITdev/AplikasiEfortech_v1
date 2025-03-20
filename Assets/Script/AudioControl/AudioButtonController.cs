using UnityEngine;
using UnityEngine.UI;

public class AudioButtonController : MonoBehaviour
{
    private AudioController audioController;

    void Start()
    {
        audioController = FindObjectOfType<AudioController>();
    }

    public void OnPauseClick()
    {
        audioController.PauseMusic();
    }

    public void OnResumeClick()
    {
        audioController.ResumeMusic();
    }
}