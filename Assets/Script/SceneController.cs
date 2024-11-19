using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    // Delay time in seconds before the scene change
    [SerializeField] private float sceneChangeDelay = 2f;

    public void GameMenu()
    {
        StartCoroutine(LoadSceneWithDelay("Main Menu"));
    }

    public void Onboard()
    {
        StartCoroutine(LoadSceneWithDelay("Onboard"));
    }

    public void ARGame()
    {
        StartCoroutine(LoadSceneWithDelay("AR Game"));
    }

    // Coroutine to handle the scene change with a delay
    private IEnumerator LoadSceneWithDelay(string sceneName)
    {
        // Wait for the specified delay time
        yield return new WaitForSeconds(sceneChangeDelay);

        // Load the scene asynchronously after the delay
        SceneManager.LoadSceneAsync(sceneName);
    }
}
