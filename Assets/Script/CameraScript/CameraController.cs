using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target; // The 3D object to focus on
    public float distance = 1.5f; // Initial distance from the object
    public float zoomSpeed = 0.5f; // Speed of zooming in/out for mouse
    public float touchZoomSpeed = 0.05f; // Reduced speed of zooming for touch
    public float rotationSpeed = 5.0f; // Speed of rotating around the object for mouse
    public float touchRotationSpeed = 0.05f; // Reduced speed of rotating for touch
    public float offsetSpeed = 0.1f; // Speed of adjusting offset
    public float touchOffsetSpeed = 0.02f; // Speed of offset adjustment for touch
    public float smoothTime = 0.2f; // Smoothing factor for transitions

    public Vector3 targetOffset = Vector3.zero; // The current offset from the target
    public float maxOffset = 2.0f; // Maximum offset distance

    private float currentX = 0.0f;
    private float currentY = 0.0f;
    private float velocityX = 0.0f; // Velocity for smooth stopping in X direction
    private float velocityY = 0.0f; // Velocity for smooth stopping in Y direction
    private float zoomVelocity = 0.0f; // Velocity for smooth stopping of zoom
    public float minYAngle = 10.0f; // Minimum vertical angle to limit the camera
    public float maxYAngle = 80.0f; // Maximum vertical angle to limit the camera

    public float minDistance = 0.5f; // Minimum distance (closer to object)
    public float maxDistance = 20.0f; // Maximum distance (further from object)

    private bool isRotating = false; // Flag to check if the camera is rotating
    private bool isZooming = false;  // Flag to check if the camera is zooming
    private bool isOffsetting = false; // Flag to check if the camera is offsetting

    private Vector3 newAnchorPosition; // New anchor position for the orbit

    void Start()
    {
        // Initialize the new anchor position with the current target's position
        newAnchorPosition = target.position;
    }

    void Update()
    {
        // --- MOUSE INPUT ---
        if (Input.GetMouseButton(1)) // Right mouse button for rotation
        {
            isRotating = true;
            velocityX = Input.GetAxis("Mouse X") * rotationSpeed;
            velocityY = -Input.GetAxis("Mouse Y") * rotationSpeed;
        }
        else
        {
            isRotating = false;
        }

        // Zoom with mouse scroll wheel
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            isZooming = true;
            zoomVelocity = scroll * zoomSpeed;
        }
        else
        {
            isZooming = false;
        }

        // Offset adjustment with mouse middle button
        if (Input.GetMouseButton(2)) // Middle mouse button for offset
        {
            isOffsetting = true;
            Vector3 offsetInput = new Vector3(-Input.GetAxis("Mouse X"), -Input.GetAxis("Mouse Y"), 0) * offsetSpeed;
            targetOffset += offsetInput;
        }
        else if (isOffsetting) // When releasing the middle mouse button
        {
            isOffsetting = false;
            // Update the new anchor position to include the offset
            newAnchorPosition += targetOffset;
            targetOffset = Vector3.zero; // Reset the offset
        }

        // --- TOUCH INPUT ---
        if (Input.touchCount == 1) // One finger for rotation
        {
            isRotating = true;

            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Moved)
            {
                velocityX = touch.deltaPosition.x * touchRotationSpeed;
                velocityY = -touch.deltaPosition.y * touchRotationSpeed;
            }
        }
        else if (Input.touchCount == 2) // Two fingers for pinch-to-zoom
        {
            isZooming = true;

            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            float deltaMagnitudeDiff = touchDeltaMag - prevTouchDeltaMag;

            zoomVelocity = deltaMagnitudeDiff * touchZoomSpeed;
        }
        else if (Input.touchCount == 3) // Three fingers for offset
        {
            isOffsetting = true;

            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Moved)
            {
                Vector3 offsetInput = new Vector3(-touch.deltaPosition.x, -touch.deltaPosition.y, 0) * touchOffsetSpeed;
                targetOffset += offsetInput;
            }
        }
        else if (isOffsetting) // When releasing three fingers
        {
            isOffsetting = false;
            newAnchorPosition += targetOffset;
            targetOffset = Vector3.zero; // Reset the offset
        }
    }

    void LateUpdate()
    {
        // Handle camera rotation (for both mouse and touch)
        if (isRotating)
        {
            currentX += velocityX;
            currentY += velocityY;
        }
        else
        {
            velocityX = Mathf.Lerp(velocityX, 0, smoothTime);
            velocityY = Mathf.Lerp(velocityY, 0, smoothTime);
            currentX += velocityX;
            currentY += velocityY;
        }

        currentY = Mathf.Clamp(currentY, minYAngle, maxYAngle);

        // Handle camera zooming
        if (isZooming)
        {
            distance -= zoomVelocity;
        }
        else
        {
            zoomVelocity = Mathf.Lerp(zoomVelocity, 0, smoothTime);
            distance -= zoomVelocity;
        }

        distance = Mathf.Clamp(distance, minDistance, maxDistance);

        // Calculate the camera's position and rotation
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
        Vector3 direction = new Vector3(0, 0, -distance);
        transform.position = newAnchorPosition + targetOffset + rotation * direction;

        // Always look at the adjusted anchor point
        transform.LookAt(newAnchorPosition + targetOffset);
    }
}
