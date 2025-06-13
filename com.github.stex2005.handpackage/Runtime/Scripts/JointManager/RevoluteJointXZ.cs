using UnityEngine;

public class RevoluteJointXZ : MonoBehaviour
{
    [System.Serializable]
    public class JointSettings
    {
        public float inputOffset;   // Offset to add to the input value
        public float inputGain;    // Gain to scale the input value
        public RotationAxis rotationAxis; // Default rotation axis is Z
        public Vector2 axisLimits; // Limits for rotation
    }

    public enum RotationAxis { X, Z } // Define the rotation axis choices
    public enum RotationInput { Key, Slider, Manager, Grasp } // Define input types

    [SerializeField]
    public JointSettings jointSettings = new JointSettings { inputOffset = 0.0f, inputGain = 1.0f, rotationAxis = RotationAxis.Z, axisLimits = new Vector2(-45f, 45f) };

    [Header("Control Settings")]
    public RotationInput rotationInput = RotationInput.Key; // Default control type
    public float rotationSpeed = 100f;
    [SerializeField, Range(-45f, 45f)] // Placeholder range
    private float jointAngle; // Slider for manual input in the Inspector

    private float currentAngle;
    private KeyCode increaseKey;
    private KeyCode decreaseKey;

    [Header("Grasp Motion Settings")]
    public float graspSpeed = 1f;  // Speed of the grasp motion (frequency of oscillation)
    public float graspAmplitude = 30f;  // Amplitude of the grasp motion (angle range)

    private float graspTime = 0f;  // Internal timer for the grasping motion

    private void Start()
    {
        // Initialize the current angle based on the selected axis
        //currentAngle = NormalizeAngle(
        //    jointSettings.rotationAxis == RotationAxis.X
        //        ? transform.localEulerAngles.x
        //        : transform.localEulerAngles.z
        //);
        currentAngle = jointSettings.rotationAxis == RotationAxis.X
                ? transform.localEulerAngles.x
                : transform.localEulerAngles.z;

        // Set default key bindings based on the chosen rotation axis
        if (jointSettings.rotationAxis == RotationAxis.X)
        {
            increaseKey = KeyCode.D; // Increase X with D
            decreaseKey = KeyCode.A; // Decrease X with A
        }
        else if (jointSettings.rotationAxis == RotationAxis.Z)
        {
            increaseKey = KeyCode.W; // Increase Z with W
            decreaseKey = KeyCode.S; // Decrease Z with S
        }

        // Synchronize slider value with the current angle
        jointAngle = Mathf.Clamp(currentAngle, jointSettings.axisLimits.x, jointSettings.axisLimits.y);
    }

    private void Update()
    {
        float rawInput = currentAngle;

        // Handle input based on the selected control type
        if (rotationInput == RotationInput.Key)
        {
            if (Input.GetKey(increaseKey))
                rawInput += rotationSpeed * Time.deltaTime;

            if (Input.GetKey(decreaseKey))
                rawInput -= rotationSpeed * Time.deltaTime;
            currentAngle = rawInput;
        }
        else if (rotationInput == RotationInput.Slider)
        {
            currentAngle = jointAngle;
        }
        else if (rotationInput == RotationInput.Grasp)
        {
            PerformGraspingMotion();
        }

        // Apply offset, gain, and clamp the result
         // ClampWithGain((rawInput + jointSettings.inputOffset) * jointSettings.inputGain, jointSettings.axisLimits, jointSettings.inputGain);

        // Apply the rotation to the selected axis
        ApplyRotation();
    }

    //private void ApplyRotation()
    //{
    //    if (jointSettings.rotationAxis == RotationAxis.X)
    //    {
    //        transform.localRotation = Quaternion.Euler(
    //            new Vector3(currentAngle, transform.localEulerAngles.y, transform.localEulerAngles.z)
    //        );
    //    }
    //    else if (jointSettings.rotationAxis == RotationAxis.Z)
    //    {
    //        transform.localRotation = Quaternion.Euler(
    //            new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, currentAngle)
    //        );
    //    }
    //}

    private void ApplyRotation()
    {
        Quaternion targetRotation;
        if (jointSettings.rotationAxis == RotationAxis.X)
        {
            targetRotation = Quaternion.AngleAxis(currentAngle, Vector3.right);
        }
        else // RotationAxis.Z
        {
            targetRotation = Quaternion.AngleAxis(currentAngle, Vector3.forward);
        }
        transform.localRotation = targetRotation;
    }

    private void PerformGraspingMotion()
    {
        graspTime += Time.deltaTime * graspSpeed;

        float sineValue = Mathf.Sin(graspTime);
        float graspAngle = sineValue * graspAmplitude;

        currentAngle = ClampWithGain((graspAngle * jointSettings.inputGain) + jointSettings.inputOffset, jointSettings.axisLimits, jointSettings.inputGain);
    }

    private float ClampWithGain(float value, Vector2 limits, float gain)
    {
        if (gain < 0)
        {
            // Swap limits if gain is negative
            return Mathf.Clamp(value, limits.y, limits.x);
        }
        else
        {
            return Mathf.Clamp(value, limits.x, limits.y);
        }
    }

    public void SetTargetAngle(float value)
    {
        if (rotationInput == RotationInput.Manager)
        {
            currentAngle = (value * jointSettings.inputGain) + jointSettings.inputOffset; // ClampWithGain((value + jointSettings.inputOffset) * jointSettings.inputGain, jointSettings.axisLimits, jointSettings.inputGain);
            //ApplyRotation();
        }
    }

    private float NormalizeAngle(float angle)
    {
        angle = (angle + 360) % 360; // Convert negative angles to [0, 360]
        if (angle > 180) angle -= 360; // Shift angles greater than 180 to [-180, 180]
        return angle;
    }
}
