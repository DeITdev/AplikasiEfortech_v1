using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;

public class CustomARPlacement : MonoBehaviour
{
    [SerializeField]
    private GameObject placementPrefab;

    [SerializeField]
    private float moveSpeed = 1f;

    private GameObject spawnedObject;
    private ARRaycastManager raycastManager;
    private bool isMoving = false;
    private Vector2 touchPosition;
    private Camera arCamera;

    private void Awake()
    {
        raycastManager = FindObjectOfType<ARRaycastManager>();
        arCamera = FindObjectOfType<Camera>();
    }

    private void Update()
    {
        if (Input.touchCount == 0)
        {
            isMoving = false;
            return;
        }

        Touch touch = Input.GetTouch(0);
        touchPosition = touch.position;

        if (touch.phase == TouchPhase.Began)
        {
            // If no object is spawned, try to place one
            if (spawnedObject == null)
            {
                TryPlaceObject();
            }
            // If object exists, check if we're touching it to move
            else if (IsTouchingObject())
            {
                isMoving = true;
            }
        }
        else if (touch.phase == TouchPhase.Moved && isMoving)
        {
            MoveObject();
        }
    }

    private bool TryPlaceObject()
    {
        List<ARRaycastHit> hits = new List<ARRaycastHit>();

        if (raycastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = hits[0].pose;
            spawnedObject = Instantiate(placementPrefab, hitPose.position, hitPose.rotation);
            return true;
        }

        return false;
    }

    private bool IsTouchingObject()
    {
        Ray ray = arCamera.ScreenPointToRay(touchPosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            return hit.collider.gameObject == spawnedObject;
        }

        return false;
    }

    private void MoveObject()
    {
        List<ARRaycastHit> hits = new List<ARRaycastHit>();

        if (raycastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = hits[0].pose;
            Vector3 targetPosition = Vector3.Lerp(
                spawnedObject.transform.position,
                hitPose.position,
                Time.deltaTime * moveSpeed
            );

            spawnedObject.transform.position = targetPosition;
        }
    }

    public void DeleteSpawnedObject()
    {
        if (spawnedObject != null)
        {
            Destroy(spawnedObject);
            spawnedObject = null;
            isMoving = false;
        }
    }
}