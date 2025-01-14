using UnityEngine;

public class CameraPanning : MonoBehaviour
{
    public Transform target; // The object to focus on
    public float distance = 1f; // Distance from the target
    public float rotationSpeed = 5.0f; // Speed of rotation in degrees per second
    public float fixedHeight = 0.55f; // Fixed height (Y-axis) for the camera
    public bool enableRotation = true; // Flag to toggle camera rotation animation
    public Vector3 anchorOffset = Vector3.zero; // Offset from the target's position

    private Vector3 anchorPosition; // Anchor position based on the target
    private float currentAngle = 0f; // Current rotation angle on the XZ plane
    private bool rotatingForward = true; // Direction of rotation (true: forward, false: backward)

    void Start()
    {
        if (target == null)
        {
            Debug.LogError("No target assigned for CameraPanning.");
            return;
        }

        // Set the initial anchor position with the offset
        anchorPosition = target.position + anchorOffset;

        // Set the initial camera position
        UpdateCameraPosition(0);
    }

    void Update()
    {
        if (target == null) return;

        // Update anchor position if the target moves
        anchorPosition = target.position + anchorOffset;

        if (enableRotation)
        {
            // Determine the rotation direction and update the angle
            float rotationStep = rotationSpeed * Time.deltaTime;
            if (rotatingForward)
            {
                currentAngle += rotationStep;
                if (currentAngle >= 50.0f)
                {
                    currentAngle = 50.0f;
                    rotatingForward = false; // Reverse direction
                }
            }
            else
            {
                currentAngle -= rotationStep;
                if (currentAngle <= -50.0f)
                {
                    currentAngle = -50.0f;
                    rotatingForward = true; // Reverse direction
                }
            }

            // Update the camera position based on the current angle
            UpdateCameraPosition(currentAngle);
        }
    }

    private void UpdateCameraPosition(float angle)
    {
        // Calculate the camera's position on the XZ plane using the current angle
        float radians = Mathf.Deg2Rad * angle;
        float x = Mathf.Sin(radians) * distance;
        float z = -Mathf.Abs(Mathf.Cos(radians) * distance); // Ensure z is always negative

        // Set the camera's position relative to the anchor and maintain fixed height
        transform.position = anchorPosition + new Vector3(x, fixedHeight, z);

        // Ensure the camera looks at the anchor position
        transform.LookAt(anchorPosition);
    }

    /// <summary>
    /// Updates the anchor offset dynamically.
    /// </summary>
    /// <param name="newOffset">The new offset for the anchor position.</param>
    public void SetAnchorOffset(Vector3 newOffset)
    {
        anchorOffset = newOffset;
    }
}
