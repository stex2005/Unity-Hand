using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Std; // For Float64MultiArrayMsg
using System.Collections.Generic;

public class ROSJointSubscriber : MonoBehaviour
{
    [System.Serializable]
    public class JointInputMapping
    {
        public RevoluteJointXZ joint; // Reference to the joint
        public int inputIndex;        // Index in the ROS message array
    }

    [SerializeField]
    private HandManager handManager; // Reference to the HandManager


    [Header("ROS Settings")]
    public string topicName = "maestro3/status/human_joints"; // ROS topic to subscribe to

    // ROS Connection instance
    private ROSConnection ros;

    [Header("Debug Settings")]
    public bool showOnScreen = true; // Toggle to show received values on screen

    private List<float> inputVector = new List<float> { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f };

    private void Start()
    {
        // Get or create the ROS connection instance
        ros = ROSConnection.GetOrCreateInstance();

        // Subscribe to the ROS topic
        ros.Subscribe<Float64MultiArrayMsg>(topicName, OnReceiveJointAngles);
    }

    private void Update()
    {

    }

    // Callback to process incoming joint angles from ROS
    private void OnReceiveJointAngles(Float64MultiArrayMsg msg)
    {
        // Clear the input vector and update it with the new data
        inputVector.Clear();

        foreach (var value in msg.data)
        {
            // Convert ROS Float64 to Unity float and log to the console
            float angle = (float)(value * 180.0 / Mathf.PI); // Convert radians to degrees
            inputVector.Add(angle);
        }
        handManager.SetExternalInput(inputVector);
        // Log the count of items in the list for debugging
        Debug.Log($"Received {inputVector.Count} joint angles.");
    }

    private void OnGUI()
    {
        if (!showOnScreen) return;

        // Display the received joint angles on the screen
        GUI.Box(new Rect(0, 30, 150, 250), $"Received Joint Angles:");

        for (int i = 0; i < inputVector.Count; i++)
        {
            GUI.Label(new Rect(30, 60 + i * 20, 150, 20), $"Joint {i + 1}: {inputVector[i]:F2}°");
        }
    }

}
