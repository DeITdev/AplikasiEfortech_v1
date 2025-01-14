using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.Interaction.Toolkit.AR;

public class ARResetManager : MonoBehaviour
{
    private ARSession arSession;
    private SingleObjectPlacement objectPlacement;
    private ARPlacementInteractable arPlacementInteractable;

    private void Awake()
    {
        arSession = FindObjectOfType<ARSession>();
        objectPlacement = FindObjectOfType<SingleObjectPlacement>();
        arPlacementInteractable = FindObjectOfType<ARPlacementInteractable>();
    }

    public void ResetARSession()
    {
        // Reset AR Session
        if (arSession != null)
        {
            arSession.Reset();
        }

        // Delete object from SingleObjectPlacement if exists
        if (objectPlacement != null)
        {
            objectPlacement.DeleteSpawnedObject();
        }

        // Delete any objects placed by ARPlacementInteractable
        if (arPlacementInteractable != null)
        {
            // Find and delete all objects placed by ARPlacementInteractable
            foreach (Transform child in arPlacementInteractable.transform)
            {
                Destroy(child.gameObject);
            }
        }
    }
}