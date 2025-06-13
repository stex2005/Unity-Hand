using System.Diagnostics;
using UnityEditor;
using UnityEngine;

public class GlobalInputManagerEditor : EditorWindow
{
    private RevoluteJointXZ.RotationInput selectedInputOption;

    [MenuItem("Hand Settings/Input")]
    public static void ShowWindow()
    {
        GetWindow<GlobalInputManagerEditor>("Input Manager");
    }

    void OnGUI()
    {
        GUILayout.Label("Set Input Option", EditorStyles.boldLabel);
        selectedInputOption = (RevoluteJointXZ.RotationInput)EditorGUILayout.EnumPopup("Input Option", selectedInputOption);

        if (GUILayout.Button("Apply to All Joints"))
        {
            ApplyGlobalInput();
        }
    }

    private void ApplyGlobalInput()
    {
        // Find all GameObjects with a RevoluteJointXZ component
        RevoluteJointXZ[] allJoints = FindObjectsOfType<RevoluteJointXZ>();

        foreach (RevoluteJointXZ joint in allJoints)
        {
            joint.rotationInput = selectedInputOption;
            EditorUtility.SetDirty(joint); // Mark the object as dirty for saving changes
        }

        UnityEngine.Debug.Log($"Applied input option {selectedInputOption} to {allJoints.Length} joints.");
    }
}

