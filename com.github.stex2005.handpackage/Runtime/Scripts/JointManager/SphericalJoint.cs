using UnityEngine;

public class SphericalJoint : MonoBehaviour
{
    [Header("Joint Settings")]

    // Rotation speed for the joint
    public float rotationSpeed = 50f;
    // Minimum and maximum angles for joint rotation
    public Vector2 xAxisLimits = new Vector2(-45f, 45f); // Min and Max for X axis
    public Vector2 zAxisLimits = new Vector2(-45f, 45f); // Min and Max for Z axis

    [Header("Key Bindings")]
    // Key bindings
    public KeyCode increaseXKey = KeyCode.D; // Rotate positively on X axis
    public KeyCode decreaseXKey = KeyCode.A; // Rotate negatively on X axis
    public KeyCode increaseZKey = KeyCode.W; // Rotate positively on Z axis
    public KeyCode decreaseZKey = KeyCode.S; // Rotate negatively on Z axis

    // The current angles of the joint
    private float currentXAngle;
    private float currentZAngle;

    private void Start()
    {
        // Initialize the current angles based on the joint's initial rotation
        currentXAngle = transform.localEulerAngles.x;
        currentZAngle = transform.localEulerAngles.z;
    }

    private void Update()
    {
        // Get input for X axis
        if (Input.GetKey(increaseXKey))
        {
            currentXAngle += rotationSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(decreaseXKey))
        {
            currentXAngle -= rotationSpeed * Time.deltaTime;
        }

        // Clamp the X angle within limits
        currentXAngle = Mathf.Clamp(currentXAngle, xAxisLimits.x, xAxisLimits.y);

        // Get input for Z axis
        if (Input.GetKey(increaseZKey))
        {
            currentZAngle += rotationSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(decreaseZKey))
        {
            currentZAngle -= rotationSpeed * Time.deltaTime;
        }

        // Clamp the Z angle within limits
        currentZAngle = Mathf.Clamp(currentZAngle, zAxisLimits.x, zAxisLimits.y);

        // Apply the rotation to the joint
        transform.localEulerAngles = new Vector3(
            currentXAngle,
            transform.localEulerAngles.y, // Keep Y unchanged
            currentZAngle
        );
    }
}
