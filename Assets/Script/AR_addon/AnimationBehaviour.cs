using UnityEngine;
using System.Collections;

public class AnimationBehaviour : MonoBehaviour
{
    [Header("Animation Control")]
    [SerializeField] Animator animator;
    [SerializeField] string stateName = "tutup-panel-2-act"; // The name of your animation state
    [SerializeField] int layerIndex = 0; // The layer index (usually 0)
    [SerializeField] float stepSize = 0.03f; // Adjust for smoothness of playback

    [Header("Visibility Control")]
    [SerializeField] GameObject targetObject; // Object to show/hide
    [SerializeField] bool hideOnStart = true; // Whether to hide the object when the script starts

    private bool isOpen = false;
    private float animationLength = 0f;
    private Coroutine playbackCoroutine = null;

    void Start()
    {
        // Get the Animator component if not assigned
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        // Validate animator
        if (animator == null)
        {
            Debug.LogError("No Animator component found on " + gameObject.name);
            return;
        }

        // Get animation length if possible
        if (animator.runtimeAnimatorController != null)
        {
            foreach (var clip in animator.runtimeAnimatorController.animationClips)
            {
                if (clip.name == stateName)
                {
                    animationLength = clip.length;
                    Debug.Log($"Animation {stateName} length: {animationLength} seconds");
                    break;
                }
            }
        }

        // Make sure animation is not playing initially by setting it to frame 0
        animator.Play(stateName, layerIndex, 0);
        animator.speed = 0;

        // Initially hide the target object if set and available
        if (targetObject != null && hideOnStart)
        {
            targetObject.SetActive(false);
        }
    }

    public void OpenAnimation()
    {
        if (animator == null || isOpen) return;

        // Show the target object immediately when starting the animation
        if (targetObject != null)
        {
            targetObject.SetActive(true);
        }

        // Stop any existing coroutine
        if (playbackCoroutine != null)
        {
            StopCoroutine(playbackCoroutine);
            playbackCoroutine = null;
        }

        // Start forward playback coroutine
        playbackCoroutine = StartCoroutine(PlayForwardAnimation());

        isOpen = true;
        Debug.Log("Playing animation forward using coroutine");
    }

    public void CloseAnimation()
    {
        if (animator == null || !isOpen) return;

        // Stop any existing coroutine
        if (playbackCoroutine != null)
        {
            StopCoroutine(playbackCoroutine);
            playbackCoroutine = null;
        }

        // Use coroutine to play reverse
        playbackCoroutine = StartCoroutine(PlayReverseAnimation());

        isOpen = false;
        Debug.Log("Playing animation backward using coroutine");
    }

    private IEnumerator PlayForwardAnimation()
    {
        // Get current state info to know where we are in the animation
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(layerIndex);
        float startTime = stateInfo.normalizedTime;

        // If near the end, start from the beginning
        if (startTime > 0.9f) startTime = 0f;

        // Clamp between 0 and 1
        startTime = Mathf.Clamp01(startTime);

        // Manually step from current position to the end
        float currentTime = startTime;

        while (currentTime < 1.0f)
        {
            // Update animation position
            animator.Play(stateName, layerIndex, currentTime);

            // Increase time for next frame
            currentTime += stepSize;

            // Wait for next frame
            yield return null;
        }

        // Ensure we end at the last frame
        animator.Play(stateName, layerIndex, 1.0f);

        playbackCoroutine = null;
        Debug.Log("Forward animation complete");
    }

    private IEnumerator PlayReverseAnimation()
    {
        // Get current state info to know where we are in the animation
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(layerIndex);
        float startTime = stateInfo.normalizedTime;

        // If near the beginning, start from the end
        if (startTime < 0.1f) startTime = 1.0f;

        // Clamp between 0 and 1
        startTime = Mathf.Clamp01(startTime);

        // Manually step from current position to beginning
        float currentTime = startTime;

        while (currentTime > 0)
        {
            // Update animation position
            animator.Play(stateName, layerIndex, currentTime);

            // Decrease time for next frame
            currentTime -= stepSize;

            // Wait for next frame
            yield return null;
        }

        // Ensure we end at the beginning
        animator.Play(stateName, layerIndex, 0);

        playbackCoroutine = null;
        Debug.Log("Reverse animation complete");

        // Hide the target object when reverse animation completes
        if (targetObject != null)
        {
            targetObject.SetActive(false);
        }
    }

    public bool IsOpen()
    {
        return isOpen;
    }
}