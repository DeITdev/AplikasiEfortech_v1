using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.Interaction.Toolkit.AR;
using System.Collections.Generic;
public class SingleObjectPlacement : ARBaseGestureInteractable
{
    public GameObject placementPrefab;
    private GameObject spawnedObject = null;
    private ARRaycastManager raycastManager;

    protected override void Awake()
    {
        base.Awake();
        raycastManager = FindObjectOfType<ARRaycastManager>();
    }
    protected override bool CanStartManipulationForGesture(TapGesture gesture)
    {
        return true;
    }
    protected override void OnEndManipulation(TapGesture gesture)
    {
        if (gesture.isCanceled)
            return;
        List<ARRaycastHit> hits = new List<ARRaycastHit>();

        if (raycastManager.Raycast(gesture.startPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = hits[0].pose;
            if (spawnedObject == null)
            {
                spawnedObject = Instantiate(placementPrefab, hitPose.position, hitPose.rotation);
            }
            else
            {
                spawnedObject.transform.position = hitPose.position;
                spawnedObject.transform.rotation = hitPose.rotation;
            }
        }
    }
    // Simple delete function - just call this from a UI button
    public void DeleteSpawnedObject()
    {
        if (spawnedObject != null)
        {
            Destroy(spawnedObject);
            spawnedObject = null;
        }
    }
}