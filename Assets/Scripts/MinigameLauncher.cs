using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


// Buttons for opening the minigames from the game menu.
public class MinigameLauncher : MonoBehaviour
{
    [Header("Buttons")]
    public Button coinGameButton;
    public Button snakeGameButton;


    [Header("Scenes")]
    public string coinGameScene = "Game 1";
    public string snakeGameScene = "Minigame2";


    void Start()
    {
        if (coinGameButton != null)
            coinGameButton.onClick.AddListener(() => SceneManager.LoadScene(coinGameScene));


        if (snakeGameButton != null)
            snakeGameButton.onClick.AddListener(() => SceneManager.LoadScene(snakeGameScene));
    }
}