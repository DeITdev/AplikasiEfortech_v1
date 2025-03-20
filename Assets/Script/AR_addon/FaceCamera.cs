using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    Transform cam;
    Vector3 targetAngle = Vector3.zero;
    void Start()
    {
        cam = Camera.main.transform;
    }
    void Update()
    {
        // Calculate a position behind the object, away from the camera
        Vector3 lookAtPos = transform.position + (transform.position - cam.position);
        transform.LookAt(lookAtPos);
        targetAngle = transform.localEulerAngles;
        targetAngle.x = 0;
        targetAngle.z = 0;
        transform.localEulerAngles = targetAngle;
    }
}