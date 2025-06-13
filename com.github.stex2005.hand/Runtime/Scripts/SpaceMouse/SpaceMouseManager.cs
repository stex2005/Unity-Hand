using UnityEngine;
using UnityEngine.InputSystem;
using SpaceNavigatorDriver;

public class SpaceMouseManager : MonoBehaviour
{
    [SerializeField]
    private GameObject targetObject;
    [SerializeField]
    private bool firstPersonView = false;  // Toggle between local and global movement
    [SerializeField]
    private bool lockTranslation = false;  // Lock translation if true
    [SerializeField]
    private bool lockRotation = false;  // Lock rotation if true

    [System.Serializable]
    private struct SpaceMouseSettings
    {
        public float translationGain;
        public float translationDeadband;
        public float rotationGain;
        public float rotationDeadband;
    }
    [SerializeField]
    private SpaceMouseSettings spaceMouseSettings = new SpaceMouseSettings { translationGain = 1.0f, translationDeadband = 0.001f, rotationGain = 1.0f, rotationDeadband = 0.001f };


    [System.Serializable]
    private struct LogSettings
    {
        public bool enableLog;
        public float logCycle;  // Time between log outputs
    }
    [SerializeField]
    private LogSettings logSettings = new LogSettings { enableLog = false, logCycle = 1.0f };

    // Time settings for logging
    private float elapsedTime = 0f;

    private void FixedUpdate()
    {
        HandleTranslation();
        HandleRotation();
        LogSpaceMouseData();
    }

    private void HandleTranslation()
    {
        if (lockTranslation) return;

        // Get and process translation input
        Vector3 translationInput = SpaceNavigatorHID.current.Translation.ReadValue();
        translationInput *= spaceMouseSettings.translationGain;
        translationInput = ApplyDeadband(translationInput, spaceMouseSettings.translationDeadband);

        // Apply translation to the target object
        Vector3 movement = firstPersonView ? targetObject.transform.TransformDirection(translationInput) : translationInput;
        targetObject.transform.position += movement;
    }

    private void HandleRotation()
    {
        if (lockRotation) return;

        // Get and process rotation input
        Vector3 rotationInput = SpaceNavigatorHID.current.Rotation.ReadValue();
        rotationInput *= spaceMouseSettings.rotationGain;
        rotationInput = ApplyDeadband(rotationInput, spaceMouseSettings.rotationDeadband);

        // Convert to Quaternion for application
        Quaternion rotationDelta = Quaternion.Euler(rotationInput);

        // Apply rotation to the target object
        if (firstPersonView)
            targetObject.transform.rotation *= rotationDelta;
        else
            targetObject.transform.rotation = rotationDelta * targetObject.transform.rotation;
    }

    private Vector3 ApplyDeadband(Vector3 input, float deadband)
    {
        return new Vector3(
            Mathf.Abs(input.x) < deadband ? 0f : input.x,
            Mathf.Abs(input.y) < deadband ? 0f : input.y,
            Mathf.Abs(input.z) < deadband ? 0f : input.z
        );
    }

    private void LogSpaceMouseData()
    {
        if (!logSettings.enableLog) return;

        elapsedTime += Time.deltaTime;
        if (elapsedTime >= logSettings.logCycle)
        {
            Debug.Log($"Translation: {SpaceNavigatorHID.current.Translation.ReadValue()}");
            Debug.Log($"Rotation: {SpaceNavigatorHID.current.Rotation.ReadValue()}");
            elapsedTime -= logSettings.logCycle;
        }
    }
}
