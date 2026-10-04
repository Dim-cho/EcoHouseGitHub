using UnityEngine;
using TMPro; // Needed for TextMeshPro

public class CoinScoreUI : MonoBehaviour
{
    [Header("UI Reference")]
    [SerializeField] private TextMeshProUGUI scoreText;

    [Header("Display Settings")]
    [SerializeField] private string prefix = "Coins: ";

    private int score = 0;

    // Listen to the event when this UI is active
    private void OnEnable()
    {
        Coin.OnCoinCollected += AddScore;
    }

    // Stop listening when disabled/destroyed (prevents errors)
    private void OnDisable()
    {
        Coin.OnCoinCollected -= AddScore;
    }

    private void Start()
    {
        UpdateDisplay();
    }

    private void AddScore()
    {
        score++;
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        if (scoreText != null)
        {
            scoreText.text = prefix + score;
        }
    }

    // Public getter in case your minigame win/lose condition needs the total score later
    public int GetScore() => score;
}