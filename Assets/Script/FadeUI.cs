using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class FadeUI : MonoBehaviour
{
    [Header("Fade Settings")]
    public float animationSpeed = 1.0f; // Multiplier for fade speed (higher = faster)

    private CanvasGroup canvasGroup;
    private bool isFading;
    private float startAlpha;
    private float endAlpha;
    private float duration = 1.0f; // Duration of the fade animation

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            Debug.LogError("FadeUI: No CanvasGroup found. Please attach this script to a GameObject with a CanvasGroup.");
            return;
        }
    }

    /// <summary>
    /// Fades in the UI element, enabling it first.
    /// </summary>
    public void FadeIn()
    {
        if (!isFading)
        {
            gameObject.SetActive(true); // Enable GameObject first
            startAlpha = 0f;
            endAlpha = 1f;
            StartCoroutine(BeginFade());
        }
    }

    /// <summary>
    /// Fades out the UI element, disabling it after fade completes.
    /// </summary>
    public void FadeOut()
    {
        if (!isFading)
        {
            startAlpha = 1f;
            endAlpha = 0f;
            StartCoroutine(BeginFade(disableOnComplete: true));
        }
    }

    private IEnumerator BeginFade(bool disableOnComplete = false)
    {
        isFading = true;
        canvasGroup.alpha = startAlpha;

        // Calculate adjusted duration based on animation speed
        float adjustedDuration = duration / animationSpeed;
        float timer = 0f;

        while (timer < adjustedDuration)
        {
            // Lerp the alpha value from startAlpha to endAlpha over adjusted duration
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, timer / adjustedDuration);
            timer += Time.deltaTime;
            yield return null;
        }

        // Ensure the final alpha value is set
        canvasGroup.alpha = endAlpha;

        if (disableOnComplete && endAlpha == 0f)
        {
            canvasGroup.blocksRaycasts = false; // Disable raycasts
            gameObject.SetActive(false); // Disable GameObject after fade out
        }
        else
        {
            canvasGroup.blocksRaycasts = true; // Enable raycasts when visible
        }

        isFading = false;
    }
}
