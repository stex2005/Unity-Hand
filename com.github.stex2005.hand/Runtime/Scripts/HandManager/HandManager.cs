using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class HandManager : MonoBehaviour
{
    [System.Serializable]
    public class JointInputMapping
    {
        public RevoluteJointXZ joint; // Reference to the joint
        public int inputIndex;        // Index in the input vector
    }

    [System.Serializable]
    public class HandConfig
    {
        public string name;
        public float[] angles;
    }

    [System.Serializable]
    public class HandConfigList
    {
        public HandConfig[] configurations; // Array of angle configurations
    }

    private enum InputMode { Slider, Json, External }; // Define input types
    private HandConfigList configList;

    [Header("Hand Settings")]
    [SerializeField]
    private GameObject handObject; // The parent object containing joints

    [Header("Input Settings")]
    [SerializeField] 
    private string jsonFileName = "angles.json";
    private string jsonFilePath;

    [SerializeField]
    private string configurationName = "Default";

    [SerializeField]
    private int configurationNumber = 0;

    [SerializeField]
    private InputMode inputMode = InputMode.Slider;
    [SerializeField, Range(-180.0f, 180.0f)]
    private List<float> inputAngles = new List<float> { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f };
    private List<float> externalAngles = new List<float> { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f };

    private RevoluteJointXZ[] joints;

    private void Awake()
    {
        jsonFilePath = Path.Combine(Application.streamingAssetsPath, jsonFileName);
    }
    private void Start()
    {   
        if (handObject != null)
        {
            joints = handObject.GetComponentsInChildren<RevoluteJointXZ>();
            if (joints.Length > 0)
            {
                Debug.Log($"Found {joints.Length} joints.");
                foreach (RevoluteJointXZ joint in joints)
                {
                    Debug.Log("Found joint: " + joint.name);
                }
            }
            else
            {
                Debug.LogError("Revolute joints not found.");
            }
        }
        else
        {
            Debug.LogError("Hand GameObject not assigned.");
        }

        LoadAnglesFromJson();

    }

    private void Update()
    {
        if (inputMode == InputMode.Json)
        { 
            HandConfig config = configList.configurations[configurationNumber];
            inputAngles = new List<float>(config.angles);
        }
        else if (inputMode == InputMode.External)
        {   
            inputAngles = new List<float>(externalAngles); 
            // Handle external input if in External mode
        }
        else if (inputMode == InputMode.Slider)
        {
            // Handle slider updates only if in Slider mode
        }

        for (int i = 0; i < joints.Length && i < inputAngles.Count; i++)
        {
            float angle = inputAngles[i];
            joints[i].SetTargetAngle(angle);
        }
    }

    private void LoadAnglesFromJson()
    {
        if (File.Exists(jsonFilePath))
        {
            try
            {
                string jsonContent = File.ReadAllText(jsonFilePath);
                configList = JsonUtility.FromJson<HandConfigList>(jsonContent);

                if (configList == null)
                {
                    Debug.LogError("Failed to deserialize JSON into AngleConfigList.");
                    return;
                }

                // Ensure configurations are not null or empty
                if (configList.configurations != null && configList.configurations.Length > 0)
                {
                    foreach (var config in configList.configurations)
                    {
                        Debug.Log($"Loaded configuration: {config.name}");
                    }
                }
                else
                {
                    Debug.LogError("No configurations found in the JSON.");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error loading angles from JSON: " + e.Message);
            }
        }
        else
        {
            Debug.LogError("JSON file not found: " + jsonFilePath);
        }
    }
    public void SetConfiguration(int config)
    {
        configurationNumber = config;
        configurationName = configList.configurations[configurationNumber].name;
    }

    public List<float> GetCurrentAngles()
    {
        return new List<float>(inputAngles);
    }

    public bool SetExternalInput(List<float> input)
    {
        if (inputMode == InputMode.External)
        {
            // Modify the input angles based on external input (this is an example)
            externalAngles = new List<float>(input); // new List<float> { 30.0f, 45.0f, -15.0f, 60.0f, 30.0f, 45.0f, -15.0f, 60.0f, 30.0f, 45.0f, -15.0f, 60.0f }; // Dummy angles for the example
            return true;
        }
        else
        {
            Debug.LogWarning("Input mode is not set to External. Input not processed.");
            return false;
        }
    }

    public void SetVisualFeedback(bool enable)
    {

        foreach (Renderer r in handObject.GetComponentsInChildren<Renderer>())
        {
            r.enabled = enable;
        }
    }
}
