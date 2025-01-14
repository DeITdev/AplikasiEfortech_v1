using UnityEngine;

public class CameraFocus : MonoBehaviour
{
    public Transform target; // The 3D object to focus on
    public float distance = 1.5f; // Distance from the object
    public Vector3 targetOffset = Vector3.zero; // Offset from the target

    public float minDistance = 0.5f; // Minimum distance (closer to object)
    public float maxDistance = 20.0f; // Maximum distance (further from object)
    public float minYAngle = 10.0f; // Minimum vertical angle to limit the camera
    public float maxYAngle = 80.0f; // Maximum vertical angle to limit the camera

    [Range(0, 360)]
    public float rotationX = 0.0f; // Horizontal rotation angle
    [Range(10, 80)]
    public float rotationY = 30.0f; // Vertical rotation angle

    void LateUpdate()
    {
        if (target == null)
        {
            Debug.LogWarning("Target not assigned in CameraFocus script.");
            return;
        }

        // Clamp distance and rotation values to ensure they remain within limits
        distance = Mathf.Clamp(distance, minDistance, maxDistance);
        rotationY = Mathf.Clamp(rotationY, minYAngle, maxYAngle);

        // Calculate the camera's position and rotation
        Quaternion rotation = Quaternion.Euler(rotationY, rotationX, 0);
        Vector3 direction = new Vector3(0, 0, -distance);
        transform.position = target.position + targetOffset + rotation * direction;

        // Always look at the target with the offset applied
        transform.LookAt(target.position + targetOffset);
    }
}
