using System.Collections;
using UnityEngine;

public class FadeUI : MonoBehaviour
{
    public static FadeUI instance;

    [Header("Fade Settings")]
    public CanvasGroup canvasGroup; // Assign the CanvasGroup of the target Canvas
    public float startAlpha = 1f; // Alpha value at the start (1 = fully visible)
    public float endAlpha = 0f;  // Alpha value at the end (0 = fully transparent)
    public float duration = 1.0f;

    private bool isFading;

    private void Awake()
    {
        // Ensure there is only one instance of FadeManager
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (canvasGroup == null)
        {
            Debug.LogError("FadeManager: 'canvasGroup' is not assigned. Please assign a CanvasGroup component.");
            return;
        }

        // Set the initial alpha of the canvas
        canvasGroup.alpha = startAlpha;
        canvasGroup.blocksRaycasts = true;

        // Start the FadeIn effect
        FadeIn();
    }

    public void FadeIn()
    {
        if (!isFading)
        {
            StartCoroutine(BeginFade());
        }
    }

    private IEnumerator BeginFade()
    {
        isFading = true;

        float timer = 0f;

        while (timer < duration)
        {
            // Lerp the alpha value from startAlpha to endAlpha over 'duration' time
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }

        // Ensure the final alpha value is set
        canvasGroup.alpha = endAlpha;

        isFading = false;

        // Optionally disable raycast blocking when fully transparent
        if (endAlpha == 0f)
        {
            canvasGroup.blocksRaycasts = false;
        }
    }
}
