using UnityEngine;
using UnityEngine.InputSystem; // Required for Unity 6's New Input System

public class Space : MonoBehaviour
{
    void Update()
    {
        // Checks if a keyboard is connected and if Space was pressed this frame
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            // Teleports to (7, 0) while keeping the original Z position
            transform.position = new Vector3(7f, 0f, transform.position.z);
        }
    }
}