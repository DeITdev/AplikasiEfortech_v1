using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARManager : MonoBehaviour
{
    private CustomARPlacement customPlacement;
    private ARPlaneManager planeManager;
    private ARSession arSession;

    private void Awake()
    {
        customPlacement = FindObjectOfType<CustomARPlacement>();
        planeManager = FindObjectOfType<ARPlaneManager>();
        arSession = FindObjectOfType<ARSession>();
    }

    // Delete the current placed object
    public void DeletePlacedObject()
    {
        if (customPlacement != null)
        {
            customPlacement.DeleteSpawnedObject();
        }
    }

    // Reset only the plane detection
    public void ResetPlaneDetection()
    {
        if (planeManager != null)
        {
            // Disable plane manager
            planeManager.enabled = false;

            // Clear all detected planes
            foreach (var plane in planeManager.trackables)
            {
                Destroy(plane.gameObject);
            }

            // Re-enable plane detection
            planeManager.enabled = true;
        }
    }

    // Full reset of everything
    public void FullReset()
    {
        // First delete any placed object
        DeletePlacedObject();

        // Reset AR Session
        if (arSession != null)
        {
            arSession.Reset();
        }

        // Reset plane detection
        if (planeManager != null)
        {
            planeManager.enabled = false;
            foreach (var plane in planeManager.trackables)
            {
                Destroy(plane.gameObject);
            }
            planeManager.enabled = true;
        }
    }

    // Optional: Quick reset of everything without full AR session reset
    public void QuickReset()
    {
        DeletePlacedObject();
        ResetPlaneDetection();
    }
}