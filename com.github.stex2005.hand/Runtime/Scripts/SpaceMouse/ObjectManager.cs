using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    private Vector3 movementInput;
    private float moveX;

    // This method will be used by another script to pass the movement input
    public void SetMoveX(float x)
    {
        moveX = x;
    }

    void Update()
    {
        // Call MoveObject to apply movement every frame
        MoveObject();
    }

    private void MoveObject()
    {
        // Move the object using transform (no Rigidbody required)
        transform.Translate(moveX * Time.deltaTime * 5f,0,0); // Adjust speed (5f) as needed
    }
}
