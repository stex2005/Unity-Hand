using UnityEngine;

public class ChangeMaterial : MonoBehaviour
{
    public Material newMaterial; // The material you want to apply

    private void Start()
    {
        // Change material for the current GameObject
        ChangeMaterialRecursive(transform);
    }

    // Method to recursively change material for this GameObject and its children
    private void ChangeMaterialRecursive(Transform parentTransform)
    {
        // Change material of the current GameObject
        Renderer renderer = parentTransform.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material = newMaterial;  // Apply the new material
        }

        // Recursively change material for all child objects
        foreach (Transform child in parentTransform)
        {
            ChangeMaterialRecursive(child);  // Call recursively for each child
        }
    }
}
