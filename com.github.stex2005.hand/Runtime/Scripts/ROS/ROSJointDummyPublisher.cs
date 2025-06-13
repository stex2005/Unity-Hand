using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Std; // For Float64MultiArrayMsg
using System.Collections.Generic;

public class ROSJointDummyPublisher : MonoBehaviour
{
    [System.Serializable]
    public class JointControl
    {
        public string jointName; // Name for identification
        [Range(-180f, 180f)] public float jointAngle; // Angle in degrees for the slider
    }

    [Header("ROS Settings")]
    public string topicName = "maestro3/status/human_joints"; // ROS topic to publish to
    public float publishRate = 100f; // Frequency to publish messages (Hz)

    // ROS Connection instance
    private ROSConnection ros;

    [Header("Joints and Slider Mapping")]
    public List<JointControl> jointControls = new List<JointControl>(); // List of joint controls

    private float publishTimer;

    private void Start()
    {
        // Ensure that the jointControls list has predefined joints with meaningful names
        if (jointControls.Count == 0)
        {
            jointControls.Add(new JointControl { jointName = "Thumb Abd ", jointAngle = 0f });
            jointControls.Add(new JointControl { jointName = "Thumb CMC", jointAngle = 0f });
            jointControls.Add(new JointControl { jointName = "Thumb MCP", jointAngle = 0f });
            jointControls.Add(new JointControl { jointName = "Thumb IP", jointAngle = 0f });
            jointControls.Add(new JointControl { jointName = "Index Abd", jointAngle = 0f });
            jointControls.Add(new JointControl { jointName = "Index MCP", jointAngle = 0f });
            jointControls.Add(new JointControl { jointName = "Index PIP", jointAngle = 0f });
            jointControls.Add(new JointControl { jointName = "Middle Abd", jointAngle = 0f });
            jointControls.Add(new JointControl { jointName = "Middle MCP", jointAngle = 0f });
            jointControls.Add(new JointControl { jointName = "Middle PIP", jointAngle = 0f });
        }

        // Get or create the ROS connection instance
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<Float64MultiArrayMsg>(topicName);
    }

    private void Update()
    {
        // Publish at a fixed rate
        publishTimer += Time.deltaTime;
        if (publishTimer >= 1f / publishRate)
        {
            publishTimer = 0f;
            PublishJointAngles();
        }
    }

    private void PublishJointAngles()
    {
        // Create a Float64MultiArrayMsg for joint angles
        Float64MultiArrayMsg msg = new Float64MultiArrayMsg();
        msg.data = new double[jointControls.Count];

        // Convert slider angles (degrees) to radians and populate the message
        for (int i = 0; i < jointControls.Count; i++)
        {
            msg.data[i] = jointControls[i].jointAngle * Mathf.Deg2Rad; // Convert to radians
        }

        // Publish the message
        ros.Publish(topicName, msg);

        // Debug information to confirm the message was sent
        Debug.Log($"Published joint angles");
    }
}
