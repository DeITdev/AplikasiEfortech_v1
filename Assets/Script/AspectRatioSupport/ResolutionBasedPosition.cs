using UnityEngine;

public class ResolutionBasedPosition : MonoBehaviour
{
    [SerializeField] private GameObject targetObject;

    private void Start()
    {
        AdjustPosition();
    }

    private void AdjustPosition()
    {
        // Get current screen resolution
        int screenWidth = Screen.width;
        int screenHeight = Screen.height;

        // Get current position
        Vector3 currentPosition = targetObject.transform.position;

        // Adjust Z position based on screen resolution
        if (screenWidth == 2400 && screenHeight == 1080)  // Device 1 in landscape
        {
            currentPosition.z = 0.5f;
        }
        else if (screenWidth == 2304 && screenHeight == 1440)  // Device 2
        {
            currentPosition.z = 0.65f;
        }

        // Apply the new position
        targetObject.transform.position = currentPosition;
    }
}