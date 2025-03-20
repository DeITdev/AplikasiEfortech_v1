using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;
using UnityEngine.UI;

public class CustomARPlacement : MonoBehaviour
{
    public static event System.Action OnObjectSpawned;
    public static event System.Action OnObjectDestroyed;

    [System.Serializable]
    public class PrefabOption
    {
        public GameObject prefab;
        public Button buttonReference;
    }

    [SerializeField]
    private PrefabOption[] prefabOptions;
    [SerializeField]
    private float moveSpeed = 4f;

    private GameObject spawnedObject;
    private ARRaycastManager raycastManager;
    private ARPlaneManager planeManager;
    private bool isMoving = false;
    private bool isLocked = false;
    private Vector2 touchPosition;
    private Vector3 lockedPosition;
    private Camera arCamera;
    private GameObject currentPrefab;

    private void Awake()
    {
        raycastManager = FindObjectOfType<ARRaycastManager>();
        planeManager = FindObjectOfType<ARPlaneManager>();
        arCamera = FindObjectOfType<Camera>();
    }

    private void Start()
    {
        InitializePrefabButtons();

        // Set initial prefab if options exist
        if (prefabOptions != null && prefabOptions.Length > 0)
        {
            currentPrefab = prefabOptions[0].prefab;
        }
    }

    private void InitializePrefabButtons()
    {
        // Setup buttons for each prefab option
        for (int i = 0; i < prefabOptions.Length; i++)
        {
            int index = i; // Capture the index for the lambda
            if (prefabOptions[i].buttonReference != null)
            {
                prefabOptions[i].buttonReference.onClick.AddListener(() => ChangePrefab(index));
            }
        }
    }

    private void ChangePrefab(int prefabIndex)
    {
        if (prefabIndex >= 0 && prefabIndex < prefabOptions.Length)
        {
            GameObject newPrefab = prefabOptions[prefabIndex].prefab;
            if (newPrefab != currentPrefab)
            {
                currentPrefab = newPrefab;
                if (spawnedObject != null)
                {
                    // Store current transform
                    Vector3 currentPosition = spawnedObject.transform.position;
                    Quaternion currentRotation = spawnedObject.transform.rotation;

                    // Destroy and respawn with new prefab
                    DeleteSpawnedObject();
                    spawnedObject = Instantiate(currentPrefab, currentPosition, currentRotation);
                    OnObjectSpawned?.Invoke();
                }
            }
        }
    }

    private void Update()
    {
        if (isLocked && spawnedObject != null)
        {
            spawnedObject.transform.position = lockedPosition;
            ProcessTouchInput();
            return;
        }

        ProcessTouchInput();
    }

    private void ProcessTouchInput()
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
            if (spawnedObject == null && currentPrefab != null)
            {
                TryPlaceObject();
            }
            else if (IsTouchingObject())
            {
                isMoving = true;
            }
        }
        else if (touch.phase == TouchPhase.Moved && isMoving && !isLocked)
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
            spawnedObject = Instantiate(currentPrefab, hitPose.position, hitPose.rotation);
            OnObjectSpawned?.Invoke();
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
        if (!isLocked)
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
    }

    public void DeleteSpawnedObject()
    {
        if (spawnedObject != null)
        {
            Destroy(spawnedObject);
            OnObjectDestroyed?.Invoke();
            spawnedObject = null;
            isMoving = false;
        }
    }

    public bool IsObjectSpawned()
    {
        return spawnedObject != null;
    }

    public void SetLockState(bool locked)
    {
        isLocked = locked;
        if (locked && spawnedObject != null)
        {
            lockedPosition = spawnedObject.transform.position;
        }
    }
}