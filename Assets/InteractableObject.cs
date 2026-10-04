using UnityEngine;
using TMPro; // Needed for TextMeshPro

public class InteractableObject : MonoBehaviour
{
    [Header("UI References")]
    public GameObject popUpPanel;
    public TextMeshProUGUI popUpText;

    [Header("Settings")]
    public string message = "You clicked the object!";

    // Variables to handle the hover visual effect
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }

    // Triggers when the mouse enters the object's collider
    void OnMouseEnter()
    {
        spriteRenderer.color = Color.gray; // Darkens the sprite to show it's clickable
    }

    // Triggers when the mouse leaves the collider
    void OnMouseExit()
    {
        spriteRenderer.color = originalColor; // Returns to normal color
    }

    // Triggers when you click the object
    void OnMouseDown()
    {
        popUpPanel.SetActive(true); // Show the panel
        popUpText.text = message;   // Update the text inside it
    }
}