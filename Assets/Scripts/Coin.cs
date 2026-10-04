using System;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Time in seconds before the coin disappears if not clicked.")]
    [SerializeField] private float lifetime = 0.5f;

    // Any script (like your ScoreManager or UI) can subscribe to this event
    public static event Action OnCoinCollected;

    private bool isCollected = false;

    private void Start()
    {
        // Automatically destroy after lifetime expires
        Destroy(gameObject, lifetime);
    }

    // Called automatically by Unity when the player clicks/taps this collider
    private void OnMouseDown()
    {
        if (isCollected) return;
        isCollected = true;

        // Fire the event to notify that a coin was clicked
        OnCoinCollected?.Invoke();

        // Optional: Play a sound or spawn a small particle effect here

        // Destroy the coin immediately
        Destroy(gameObject);
    }
}